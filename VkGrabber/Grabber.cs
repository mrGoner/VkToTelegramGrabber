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
        public Vk m_vkApi;
        public UserManager m_userManager;
        public Timer m_updateTimer;
        public readonly NewsFeedToPostsConverter m_feedToPostsConverter;

        public Grabber(string _vkVersion, TimeSpan _updateSpan)
        {
            m_processor = new Processor(20);
            m_vkApi = new Vk(_vkVersion);
            m_userManager = new UserManager();
            m_updateTimer = new Timer(_updateSpan.TotalMilliseconds);
            m_updateTimer.Elapsed += UpdateTimer_Elapsed;
            m_feedToPostsConverter = new NewsFeedToPostsConverter();
        }

        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_updateTimer.Stop();

            using (var context = new GrabberDbContext())
            {
                foreach (var dbUser in context.Users.ToList())
                {
                    var dtNow = DateTime.Now.ToUniversalTime();

                    var groupsToUpdate = dbUser.Groups.Where(_dbGroup =>
                            _dbGroup.LastUpdateDateTime + _dbGroup.UpdatePeriod < dtNow).ToList();

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

                var posts = m_feedToPostsConverter.Convert(newsFeed);

                NewDataGrabbedEventHandler?.Invoke(_user.Key, posts);
            }
            catch(Exception ex)
            {
                //todo logging
            }
        }
    }
}