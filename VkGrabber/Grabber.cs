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

namespace VkGrabber
{
    public class Grabber : IDisposable
    {
        private readonly Processor m_processor;
        public delegate void NewDataGrabbed(string _userKey, Posts _posts);
        public event NewDataGrabbed NewDataGrabbedEventHandler;
        private readonly Vk m_vkApi;
        private readonly NewsFeedToPostsConverter m_feedToPostsConverter;
        private readonly PostComparer m_postComparer = new PostComparer();
        private CancellationTokenSource m_tokenSource;
        private readonly ILogger m_logger;
        private bool m_isDisposed;
        private bool m_isStarted;
        private readonly TimeSpan m_updateSpan;
        private readonly DbContextFactory m_contextFactory;

        public Grabber(TimeSpan _updateSpan, int _maxPerformed, int _bufferCapacity, string _pathToDb, ILoggerFactory _loggerFactory)
        {
            if (_maxPerformed <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(_maxPerformed)} must be greater than zero");

            if (_bufferCapacity <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(_bufferCapacity)} must be greater than zero");

            if (_updateSpan == default)
                throw new ArgumentException($"{nameof(_updateSpan)} must be set");

            m_processor = new Processor(_maxPerformed, _bufferCapacity);
            m_vkApi = new Vk();
            m_updateSpan = _updateSpan;
            m_feedToPostsConverter = new NewsFeedToPostsConverter();
            m_logger = _loggerFactory.CreateLogger(typeof(Grabber));
            m_contextFactory = new DbContextFactory(_pathToDb);

            m_logger.LogInformation("Grabber init with update interval: {UpdateSpan}", _updateSpan);
        }

        public async ValueTask Start(CancellationToken _cancellationToken)
        {
            if (m_isDisposed)
                throw new ObjectDisposedException(nameof(Grabber));

            if (m_isStarted)
                return;

            m_tokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);

            await ResetUpdatingStateForGroups(m_tokenSource.Token);

            _ = UpdateProcessor(_cancellationToken);
        }

        public void Stop()
        {
            if (!m_isStarted)
                return;

            m_isStarted = false;
            m_tokenSource.Cancel(false);
        }

        private async Task UpdateProcessor(CancellationToken _cancellationToken)
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await using var context = m_contextFactory.CreateContext();
                    foreach (var dbUser in await context.DbUsers.ToListAsync(_cancellationToken))
                    {
                        var dtNow = DateTime.Now.ToUniversalTime();

                        var groupsToUpdate = await context.DbGroups.AsAsyncEnumerable().Where(_dbGroup =>
                            (_dbGroup.LastUpdateDateTime + _dbGroup.UpdatePeriod) < dtNow &&
                            _dbGroup.DbUser.Id == dbUser.Id && !_dbGroup.IsUpdating).ToListAsync(_cancellationToken);

                        if (groupsToUpdate.Any())
                        {
                            groupsToUpdate.ForEach(_group => _group.IsUpdating = true);

                            await context.SaveChangesAsync(_cancellationToken);

                            var startDateTime = groupsToUpdate.Min(group => group.LastUpdateDateTime);

                            await m_processor.AddAsync(async () => await GetPostsFromGroup(
                                new UserInfo(dbUser.Id, dbUser.Key, dbUser.Token), startDateTime, dtNow,
                                groupsToUpdate.Select(group => new GroupInfo(group.GroupPrefix, group.GroupId, group.GroupName)).ToArray(),
                                _cancellationToken), _cancellationToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_logger.LogError(ex, "Failed to update by interval");
                }

                await Task.Delay(m_updateSpan, _cancellationToken);
            }
        }

        private async Task GetPostsFromGroup(UserInfo _user, DateTime _start, DateTime _end, IReadOnlyCollection<GroupInfo> _groups, CancellationToken _cancellationToken)
        {
            try
            {
                var idsList = _groups.Select(Helpers.ConvertGroupToSourceId);

                var sourceIds = string.Join(',', idsList);

                m_logger.LogInformation(
                    "GetPostFromGroup with params User Id: {UserId} User key: {UserKey} from {Start} to {End} with groups {SourceIds}",
                    _user.Id, _user.Key, _start, _end, sourceIds);

                try
                {
                    var newsFeed = await m_vkApi.GetNewsFeedAsync(_user.Token, _start, _end, sourceIds, _cancellationToken);

                    var postsGroup = m_feedToPostsConverter.Convert(newsFeed,
                        _groups.ToDictionary(_key => _key.Id, _value => _value.Name)).GroupBy(_post => _post.GroupId);

                    var posts = new Posts();

                    await using var context = m_contextFactory.CreateContext();

                    foreach (var postGroup in postsGroup)
                    {
                        _cancellationToken.ThrowIfCancellationRequested();

                        var dbGroup = await context.DbGroups.FirstOrDefaultAsync(_dbGroup =>
                            _dbGroup.GroupId == postGroup.Key && _dbGroup.DbUser.Id == _user.Id, _cancellationToken);

                        if (dbGroup != null)
                        {
                            var postsFromGroup = postGroup.ToList();
                            postsFromGroup.Sort(m_postComparer);

                            var lastUpdatedElem =
                                postsFromGroup.FirstOrDefault(_post => _post.PostId == dbGroup.LastUpdatedPostId);

                            if (lastUpdatedElem != null)
                            {
                                postsFromGroup = postsFromGroup.SkipWhile(_x => _x.PublishTime <= lastUpdatedElem.PublishTime)
                                    .ToList();

                                m_logger.LogDebug(
                                    "Find post equals to lastUpdatedPost with id {DbGroupLastUpdatedPostId}, cutting completed",
                                    dbGroup.LastUpdatedPostId);
                            }

                            if (postsFromGroup.Count > 0)
                            {
                                dbGroup.LastUpdatedPostId = postsFromGroup.Last().PostId;
                                posts.AddRange(postsFromGroup);
                            }
                        }
                        else
                            m_logger.LogError("Can not find group with id {PostGroupKey}", postGroup.Key);
                    }

                    await context.SaveChangesAsync(_cancellationToken);

                    await CompleteGroupsUpdatingForUser(_user, _groups, _end, _cancellationToken);

                    m_logger.LogDebug("Getting {PostsCount} posts", posts.Count);

                    if (posts.Any())
                    {
                        NewDataGrabbedEventHandler?.Invoke(_user.Key, posts);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception)
                {
                    await CompleteGroupsUpdatingForUser(_user, _groups, _end, _cancellationToken);
                    throw;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "Error, while get posts from group");
            }
        }

        private async Task CompleteGroupsUpdatingForUser(UserInfo _userInfo, IEnumerable<GroupInfo> _groups, DateTime _updateTime, CancellationToken _cancellationToken)
        {
            await using var context = m_contextFactory.CreateContext();
            foreach (var group in _groups)
            {
                var dbGroup = context.DbGroups.FirstOrDefault(_dbGroup =>
                    _dbGroup.GroupId == group.Id && _dbGroup.DbUser.Id == _userInfo.Id);

                if (dbGroup == null)
                    continue;

                dbGroup.LastUpdateDateTime = _updateTime;
                dbGroup.IsUpdating = false;
            }

            await context.SaveChangesAsync(_cancellationToken);
        }

        private async Task ResetUpdatingStateForGroups(CancellationToken _cancellationToken)
        {
            await using var context = m_contextFactory.CreateContext();

            await foreach (var group in context.DbGroups.Where(_group => _group.IsUpdating).AsAsyncEnumerable().WithCancellation(_cancellationToken))
            {
                group.IsUpdating = false;
            }

            await context.SaveChangesAsync(_cancellationToken);
        }

        private class PostComparer : IComparer<Post>
        {
            public int Compare(Post _x, Post _y)
            {
                if (_x is null && _y is null)
                    return 0;

                if (_x is null)
                    return 1;
                if (_y is null)
                    return -1;

                return _x.PublishTime.CompareTo(_y.PublishTime);
            }
        }

        public void Dispose()
        {
            if (m_isDisposed)
                return;

            m_isDisposed = true;

            m_processor.Dispose();
            m_tokenSource.Cancel(false);
            m_tokenSource.Dispose();
        }
    }

    internal record GroupInfo(string Prefix, int Id, string Name);
    internal record UserInfo(int Id, string Key, string Token);
}