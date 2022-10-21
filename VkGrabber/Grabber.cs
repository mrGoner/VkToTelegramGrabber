using System;
using System.Collections.Generic;
using System.Linq;
using VkGrabber.DataLayer;
using VkApi;
using System.Timers;
using VkGrabber.Converters;
using VkGrabber.Model;
using static VkGrabber.MyLogger;

namespace VkGrabber
{
    public class Grabber
    {
        private readonly Processor m_processor;
        public delegate void NewDataGrabbed(string _userKey, Posts _posts);
        public event NewDataGrabbed NewDataGrabbedEventHandler;
        private readonly Vk m_vkApi;
        private readonly Timer m_updateTimer;
        private readonly NewsFeedToPostsConverter m_feedToPostsConverter;
        private readonly PostComparer m_postComparer = new PostComparer();

        public Grabber(string _vkVersion, TimeSpan _updateSpan)
        {
            m_processor = new Processor(20);
            m_vkApi = new Vk(_vkVersion);
            m_updateTimer = new Timer(_updateSpan.TotalMilliseconds);
            m_updateTimer.Elapsed += UpdateTimer_Elapsed;
            m_feedToPostsConverter = new NewsFeedToPostsConverter();
            Log.Info($"Grabber inited with vk version: {_vkVersion}, updated span: {_updateSpan}");
        }

        public void Start()
        {
            m_updateTimer.Start();
        }

        public void Stop()
        {
            m_updateTimer.Stop();
        }

        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_updateTimer.Stop();

            try
            {
                using (var context = new GrabberDbContext())
                {
                    foreach (var dbUser in context.DbUsers.ToList())
                    {
                        var dtNow = DateTime.Now.ToUniversalTime();

                        var groupsToUpdate = context.DbGroups.ToList().Where(_dbGroup =>
                                (_dbGroup.LastUpdateDateTime + _dbGroup.UpdatePeriod) < dtNow &&
                                _dbGroup.DbUser.Id == dbUser.Id && !_dbGroup.IsUpdating).ToList();

                        if (groupsToUpdate.Any())
                        {
                            var startDateTime = groupsToUpdate.Select(_group => _group.LastUpdateDateTime).Min();

                            groupsToUpdate.ForEach(_group => _group.IsUpdating = true);

                            m_processor.Add(() => GetPostsFromGroup(dbUser, startDateTime, dtNow, groupsToUpdate));
                        }
                    }

                    context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Log.Error("Failed to updateTick", ex);
            }

            m_updateTimer.Start();
        }

        private void GetPostsFromGroup(DbUser _user, DateTime _start, DateTime _end, List<DbGroup> _groups)
        {
            try
            {
                var idsList = _groups.Select(Helpers.ConvertGroupToSourceId).ToArray();

                var sourceIds = string.Join(',', idsList);

                Log.Debug($"GetPostFromGroup with params User Id: {_user.Id} " +
                    $"User key: {_user.Key} from {_start} to {_end} with groups {sourceIds}");

                try
                {
                    var newsFeed = m_vkApi.GetNewsFeed(_user.Token, _start, _end, sourceIds);

                    var posts = m_feedToPostsConverter.Convert(newsFeed,
                        _groups.ToDictionary(_key => _key.GroupId, _value => _value.GroupName));

                    Log.Debug($"Getting {posts.Count} posts");

                    using (var context = new GrabberDbContext())
                    {
                        foreach (var group in _groups)
                        {
                            var dbGroup = context.DbGroups.FirstOrDefault(_dbGroup =>
                                _dbGroup.GroupId == group.GroupId && _dbGroup.DbUser.Id == group.DbUser.Id);

                            if (dbGroup != null)
                            {
                                var postsFromGroup = posts.Where(_post => _post.GroupId == group.GroupId).ToList();

                                postsFromGroup.Sort(m_postComparer);

                                var lastUpdatedElem = postsFromGroup.FirstOrDefault(_post => _post.PostId == group.LastUpdatedPostId);

                                if (lastUpdatedElem != null)
                                {
                                    var index = postsFromGroup.IndexOf(lastUpdatedElem);

                                    postsFromGroup.Take(index + 1).ToList().ForEach(_post => posts.Remove(_post));

                                    postsFromGroup = postsFromGroup.Skip(index + 1).Take(postsFromGroup.Count - index + 1).ToList();

                                    Log.Debug("Finded post equals to " +
                                        $"lastUpdatedPost wiht id {group.LastUpdatedPostId}, cutting completed");
                                }

                                if (postsFromGroup.Count > 0)
                                {
                                    dbGroup.LastUpdatedPostId = postsFromGroup.Last().PostId;
                                }

                                dbGroup.LastUpdateDateTime = DateTime.Now.ToUniversalTime();
                                dbGroup.IsUpdating = false;
                            }
                            else
                                Log.Error($"Can not find group with id {group.Id}");

                        }

                        context.SaveChanges();
                    }

                    NewDataGrabbedEventHandler?.Invoke(_user.Key, posts);
                }
                catch (Exception ex)
                {
                    using (var context = new GrabberDbContext())
                    {
                        foreach (var group in _groups)
                        {
                            var dbGroup = context.DbGroups.FirstOrDefault(_dbGroup =>
                                _dbGroup.GroupId == group.GroupId && _dbGroup.DbUser.Id == group.DbUser.Id);

                            if (dbGroup != null)
                                dbGroup.IsUpdating = false;
                        }

                        context.SaveChanges();
                    }

                    throw;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error, while get posts from group", ex);
            }
        }

        class PostComparer : IComparer<Post>
        {
            public int Compare(Post _x, Post _y)
            {
                return _x.PublishTime.CompareTo(_y.PublishTime);
            }
        }
    }
}