using System;
using System.Collections.Generic;
using System.Linq;
using VkGrabber.DataLayer;
using VkApi;
using VkGrabber.Converters;
using VkGrabber.Model;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VkApi.Serializers;

namespace VkGrabber;

public class Grabber : IDisposable
{
    private readonly Processor _processor;

    public delegate void NewDataGrabbed(string userKey, Posts posts);

    public event NewDataGrabbed? NewDataGrabbedEventHandler;
    private readonly Vk _vkApi;
    private readonly NewsFeedToPostsConverter _feedToPostsConverter;
    private readonly PostComparer _postComparer = new();
    private CancellationTokenSource? _tokenSource;
    private readonly ILogger<Grabber> _logger;
    private bool _isDisposed;
    private bool _isStarted;
    private readonly TimeSpan _updateSpan;
    private readonly DbContextFactory _contextFactory;

    public Grabber(TimeSpan updateSpan, int maxPerformed, int bufferCapacity, string pathToDb,
        ILogger<Grabber> logger)
    {
        if (maxPerformed <= 0)
            throw new ArgumentOutOfRangeException($"{nameof(maxPerformed)} must be greater than zero");

        if (bufferCapacity <= 0)
            throw new ArgumentOutOfRangeException($"{nameof(bufferCapacity)} must be greater than zero");

        if (updateSpan == TimeSpan.Zero)
            throw new ArgumentException($"{nameof(updateSpan)} must be set");

        _processor = new Processor(maxPerformed, bufferCapacity);
        _vkApi = new Vk(); 
        _updateSpan = updateSpan;
        _feedToPostsConverter = new NewsFeedToPostsConverter();
        _logger = logger;
        _contextFactory = new DbContextFactory(pathToDb);
    }

    public async ValueTask Start(CancellationToken cancellationToken)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(Grabber));

        if (_isStarted)
            return;

        _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        await ResetUpdatingStateForGroups(_tokenSource.Token);

        _ = UpdateProcessor(cancellationToken);

        _logger.LogInformation("Grabber started with update interval: {UpdateSpan}", _updateSpan);
    }

    public void Stop()
    {
        if (!_isStarted)
            return;

        _isStarted = false;
        _tokenSource?.Cancel(false);
    }

    private async Task UpdateProcessor(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await using var context = _contextFactory.CreateContext();
                foreach (var dbUser in await context.DbUsers.ToListAsync(cancellationToken))
                {
                    var dtNow = DateTime.Now.ToUniversalTime();

                    var groupsToUpdate = await context.DbGroups.AsAsyncEnumerable().Where(dbGroup =>
                        dbGroup.LastUpdateDateTime + dbGroup.UpdatePeriod < dtNow &&
                        dbGroup.DbUser.Id == dbUser.Id && !dbGroup.IsUpdating).ToListAsync(cancellationToken);

                    if (groupsToUpdate.Any())
                    {
                        groupsToUpdate.ForEach(group => group.IsUpdating = true);

                        await context.SaveChangesAsync(cancellationToken);

                        var startDateTime = groupsToUpdate.Min(group => group.LastUpdateDateTime);

                        await _processor.AddAsync(async () =>
                        {
                            var userInfo = new UserInfo(dbUser.Id, dbUser.Key, dbUser.Token);
                            var groupsInfo = groupsToUpdate.Select(group =>
                                new GroupInfo(group.GroupPrefix, group.GroupId, group.GroupName)).ToArray();

                            await GetPostsFromGroup(userInfo, startDateTime, dtNow, groupsInfo, cancellationToken);
                        }, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update by interval");
            }

            await Task.Delay(_updateSpan, cancellationToken);
        }
    }

    private async Task GetPostsFromGroup(UserInfo user, DateTime start, DateTime end,
        IReadOnlyCollection<GroupInfo> groups, CancellationToken cancellationToken)
    {
        try
        {
            var idsList = groups.Select(Helpers.ConvertGroupToSourceId);

            var sourceIds = string.Join(',', idsList);

            _logger.LogInformation(
                "GetPostFromGroup with params User Id: {UserId} User key: {UserKey} from {Start} to {End} with groups {SourceIds}",
                user.Id, user.Key, start, end, sourceIds);

            try
            {
                var newsFeed = await _vkApi.GetNewsFeedAsync(user.Token, start, end, sourceIds, cancellationToken);

                var postsGroup = _feedToPostsConverter
                    .Convert(newsFeed, groups.ToDictionary(key => key.Id, value => value.Name))
                    .GroupBy(post => post.GroupId);

                var posts = new Posts();

                await using var context = _contextFactory.CreateContext();

                foreach (var postGroup in postsGroup)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var dbGroup = await context.DbGroups.FirstOrDefaultAsync(
                        group => group.GroupId == postGroup.Key && group.DbUser.Id == user.Id, cancellationToken);

                    if (dbGroup != null)
                    {
                        var postsFromGroup = postGroup.ToList();
                        postsFromGroup.Sort(_postComparer);

                        var lastUpdatedElem =
                            postsFromGroup.FirstOrDefault(post => post.PostId == dbGroup.LastUpdatedPostId);

                        if (lastUpdatedElem != null)
                        {
                            _logger.LogInformation("Groups before cutting : {Ids}", string.Join(",", postsFromGroup.Select(post => post.PostId)));

                            postsFromGroup = postsFromGroup.SkipWhile(post => post.PostId <= lastUpdatedElem.PostId)
                                .ToList();

                            _logger.LogDebug(
                                "Find post equals to lastUpdatedPost with id {DbGroupLastUpdatedPostId}, cutting completed",
                                dbGroup.LastUpdatedPostId);

                            _logger.LogInformation("Groups after cutting : {Ids}",
                                string.Join(",", postsFromGroup.Select(post => post.PostId)));
                        }

                        if (postsFromGroup.Count > 0)
                        {
                            dbGroup.LastUpdatedPostId = postsFromGroup.Last().PostId;
                            posts.AddRange(postsFromGroup);
                        }
                    }
                    else
                    {
                        _logger.LogError("Can not find group with id {PostGroupKey}", postGroup.Key);
                    }
                }

                await context.SaveChangesAsync(cancellationToken);

                await CompleteGroupsUpdatingForUser(user, groups, end, cancellationToken);

                _logger.LogDebug("Getting {PostsCount} posts", posts.Count);

                if (posts.Any())
                    NewDataGrabbedEventHandler?.Invoke(user.Key, posts);
            }
            catch (DeserializerException)
            {
                await CompleteGroupsUpdatingForUser(user, groups, end, cancellationToken);
                throw;
            }
            catch (VkException)
            {
                await ResetGroupUpdatingForUser(user, cancellationToken);
                throw;
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error, while get posts from group");
        }
    }

    private async Task CompleteGroupsUpdatingForUser(UserInfo userInfo, IEnumerable<GroupInfo> groups,
        DateTime updateTime, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.CreateContext();
        foreach (var group in groups)
        {
            var dbGroup = context.DbGroups.FirstOrDefault(dbGroup => dbGroup.GroupId == group.Id && dbGroup.DbUser.Id == userInfo.Id);

            if (dbGroup == null)
                continue;

            dbGroup.LastUpdateDateTime = updateTime;
            dbGroup.IsUpdating = false;
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task ResetGroupUpdatingForUser(UserInfo userInfo, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.CreateContext();

        foreach (var dbGroup in context.DbGroups.Where(dbGroup => dbGroup.DbUser.Id == userInfo.Id))
            dbGroup.IsUpdating = false;
        
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task ResetUpdatingStateForGroups(CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.CreateContext();

        foreach (var group in context.DbGroups.Where(dbGroup => dbGroup.IsUpdating))
            group.IsUpdating = false;

        await context.SaveChangesAsync(cancellationToken);
    }

    private class PostComparer : IComparer<Post>
    {
        public int Compare(Post? x, Post? y)
        {
            if (x is null && y is null)
                return 0;
            if (x is null)
                return 1;
            if (y is null)
                return -1;

            return x.PostId.CompareTo(y.PostId);
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        _processor.Dispose();
        _tokenSource?.Cancel(false);
        _tokenSource?.Dispose();

        GC.SuppressFinalize(this);
    }
}