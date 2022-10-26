using System;
using System.Collections.Generic;
using System.Linq;
using VkGrabber.DataLayer;
using VkApi;
using System.Timers;
using VkGrabber.Converters;
using VkGrabber.Model;
using static VkGrabber.MyLogger;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace VkGrabber
{
    public class Grabber : IDisposable
    {
        private readonly Processor m_processor;
        public delegate void NewDataGrabbed(string _userKey, Posts _posts);
        public event NewDataGrabbed NewDataGrabbedEventHandler;
        private readonly Vk m_vkApi;
        private readonly System.Timers.Timer m_updateTimer;
        private readonly NewsFeedToPostsConverter m_feedToPostsConverter;
        private readonly PostComparer m_postComparer = new PostComparer();
        private readonly CancellationTokenSource m_tokenSource;

        public Grabber(TimeSpan _updateSpan, int _maxPerformed, int _bufferCapacity)
        {
            if (_maxPerformed <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(_maxPerformed)} must be greater than zero");
            
            if(_bufferCapacity <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(_bufferCapacity)} must be greater than zero");

            if (_updateSpan == default)
                throw new ArgumentException($"{nameof(_updateSpan)} must be set");
            
            m_processor = new Processor(_maxPerformed, _bufferCapacity);
            m_vkApi = new Vk();
            m_updateTimer = new System.Timers.Timer(_updateSpan.TotalMilliseconds);
            m_updateTimer.Elapsed += UpdateTimer_Elapsed;
            m_feedToPostsConverter = new NewsFeedToPostsConverter();
            m_tokenSource = new CancellationTokenSource();
            
            Log.Info($"Grabber init with update interval: {_updateSpan}");
        }

        public void Start()
        {
            m_updateTimer.Start();
        }

        public void Stop()
        {
            m_updateTimer.Stop();
        }

        private async void UpdateTimer_Elapsed(object _sender, ElapsedEventArgs _e)
        {
            m_updateTimer.Stop();

            try
            {
                await using var context = new GrabberDbContext();
                foreach (var dbUser in await context.DbUsers.ToListAsync())
                {
                    var dtNow = DateTime.Now.ToUniversalTime();

                    var groupsToUpdate = await context.DbGroups.Where(_dbGroup =>
                        (_dbGroup.LastUpdateDateTime + _dbGroup.UpdatePeriod) < dtNow &&
                        _dbGroup.DbUser.Id == dbUser.Id && !_dbGroup.IsUpdating).ToListAsync();

                    if (groupsToUpdate.Any())
                    {
                        var startDateTime = groupsToUpdate.Select(_group => _group.LastUpdateDateTime).Min();

                        groupsToUpdate.ForEach(_group => _group.IsUpdating = true);

                        await m_processor.AddAsync(async () => await GetPostsFromGroup(
                            new UserInfo(dbUser.Id, dbUser.Key, dbUser.Token), startDateTime, dtNow,
                            groupsToUpdate.Select(_x => new GroupInfo(_x.GroupPrefix, _x.GroupId, _x.GroupName)).ToArray()
                            , m_tokenSource.Token), m_tokenSource.Token);
                    }
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error("Failed to updateTick", ex);
            }
            finally
            {
                m_updateTimer.Start();
            }
        }

        private async Task GetPostsFromGroup(UserInfo _user, DateTime _start, DateTime _end, IReadOnlyCollection<GroupInfo> _groups, CancellationToken _cancellationToken)
        {
            try
            {
                var idsList = _groups.Select(Helpers.ConvertGroupToSourceId);

                var sourceIds = string.Join(',', idsList);

                Log.Debug($"GetPostFromGroup with params User Id: {_user.Id} " +
                    $"User key: {_user.Key} from {_start} to {_end} with groups {sourceIds}");

                try
                {
                    var newsFeed = await m_vkApi.GetNewsFeedAsync(_user.Token, _start, _end, sourceIds, _cancellationToken);

                    var postsGroup = m_feedToPostsConverter.Convert(newsFeed,
                        _groups.ToDictionary(_key => _key.Id, _value => _value.Name)).GroupBy(_post => _post.GroupId);

                    var posts = new Posts();

                    await using var context = new GrabberDbContext();

                    foreach (var postGroup in postsGroup)
                    {
                        var dbGroup = context.DbGroups.FirstOrDefault(_dbGroup =>
                            _dbGroup.GroupId == postGroup.Key && _dbGroup.DbUser.Id == _user.Id);

                        if (dbGroup != null)
                        {
                            var postsFromGroup = postGroup.ToList();
                            postsFromGroup.Sort(m_postComparer);

                            var lastUpdatedElem =
                                postsFromGroup.FirstOrDefault(_post => _post.PostId == dbGroup.LastUpdatedPostId);

                            if (lastUpdatedElem != null)
                            {
                                postsFromGroup = postsFromGroup.SkipWhile(_x => _x.PostId == lastUpdatedElem.PostId)
                                    .ToList();

                                Log.Debug("Find post equals to " +
                                          $"lastUpdatedPost with id {dbGroup.LastUpdatedPostId}, cutting completed");
                            }

                            if (postsFromGroup.Count > 0)
                            {
                                dbGroup.LastUpdatedPostId = postsFromGroup.Last().PostId;
                            }

                            dbGroup.LastUpdateDateTime = DateTime.Now.ToUniversalTime();
                            dbGroup.IsUpdating = false;
                        }
                        else
                            Log.Error($"Can not find group with id {postGroup.Key}");
                    }

                    await context.SaveChangesAsync(_cancellationToken);

                    Log.Debug($"Getting {posts.Count} posts");

                    if (posts.Any())
                        NewDataGrabbedEventHandler?.Invoke(_user.Key, posts);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception)
                {
                    await using var context = new GrabberDbContext();
                    foreach (var group in _groups)
                    {
                        var dbGroup = context.DbGroups.FirstOrDefault(_dbGroup =>
                            _dbGroup.GroupId == group.Id && _dbGroup.DbUser.Id == _user.Id);

                        if (dbGroup != null)
                            dbGroup.IsUpdating = false;
                    }

                    await context.SaveChangesAsync(_cancellationToken);

                    throw;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.Error("Error, while get posts from group", ex);
            }
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
            m_updateTimer.Stop();
            m_updateTimer.Elapsed -= UpdateTimer_Elapsed;
            m_tokenSource.Cancel();
            m_tokenSource.Dispose();
        }
    }

    internal record GroupInfo(string Prefix, int Id, string Name);
    internal record UserInfo(int Id, string Key, string Token);
}