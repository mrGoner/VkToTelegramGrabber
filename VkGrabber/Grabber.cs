using System;
using System.Collections.Generic;
using System.Linq;
using VkGrabber.DataLayer;
using VkApi;
using System.Timers;
using VkGrabber.Converters;
using VkGrabber.Model;

namespace VkGrabber
{
    public class Grabber
    {
        private readonly Processor m_processor;
        public delegate void NewDataGrabbed(string _userKey, Posts _posts);
        public event NewDataGrabbed NewDataGrabbedEventHandler;
        private Vk m_vkApi;
        private readonly Timer m_updateTimer;
        private readonly NewsFeedToPostsConverter m_feedToPostsConverter;

        public Grabber(string _vkVersion, TimeSpan _updateSpan)
        {
            m_processor = new Processor(20);
            m_vkApi = new Vk(_vkVersion);
            m_updateTimer = new Timer(_updateSpan.TotalMilliseconds);
            m_updateTimer.Elapsed += UpdateTimer_Elapsed;
            m_feedToPostsConverter = new NewsFeedToPostsConverter();
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

            using (var context = new GrabberDbContext())
            {
                foreach (var dbUser in context.DbUsers.ToList())
                {
                    var dtNow = DateTime.Now.ToUniversalTime();

                    var groupsToUpdate = context.DbGroups.Where(_dbGroup =>
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

            m_updateTimer.Start();
        }

        private void GetPostsFromGroup(DbUser _user, DateTime _start, DateTime _end, List<DbGroup> _groups)
        {
            try
            {
                var idsList = new List<string>();

                foreach(var dbGroup in _groups)
                {
                    idsList.Add(Helpers.ConvertGroupToSourceId(dbGroup));
                }

                var sourceIds = string.Join(',', idsList);

                var newsFeed = m_vkApi.GetNewsFeed(_user.Token, _start, _end, sourceIds);

                var posts = m_feedToPostsConverter.Convert(newsFeed, 
                    _groups.ToDictionary(_key => _key.GroupId, _value => _value.GroupName));

                using (var context = new GrabberDbContext())
                {
                    foreach (var group in _groups)
                    {
                        var postsFromGroup = posts.Where(_post => _post.GroupId == group.GroupId).ToList();

                         var lastUpdatedElem = posts.FirstOrDefault(_post => _post.PostId == group.LastUpdatedPostId);

                        var dbGroup = context.DbGroups.FirstOrDefault(_dbGroup =>
                            _dbGroup.GroupId == group.GroupId && _dbGroup.DbUser.Id == group.DbUser.Id);

                        if (lastUpdatedElem != null)
                        {
                            var index = postsFromGroup.IndexOf(lastUpdatedElem);

                            postsFromGroup.RemoveRange(index + 1, postsFromGroup.Count - index - 1);
                            postsFromGroup.ForEach(_post => posts.Remove(_post));
                        }

                        if (postsFromGroup.Count > 0)
                        {
                            if (dbGroup != null)
                            {
                                dbGroup.LastUpdatedPostId = postsFromGroup.Last().PostId;
                            }
                        }

                        dbGroup.IsUpdating = false; 
                        dbGroup.LastUpdateDateTime = DateTime.Now.ToUniversalTime();

                    }

                    context.SaveChanges();
                }

                NewDataGrabbedEventHandler?.Invoke(_user.Key, posts);
            }
            catch(Exception ex)
            {

            }
        }
    }
}