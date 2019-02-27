using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkGrabber.DataLayer;
using VkApi;
using System.Timers;

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

        public Grabber(string _vkVersion, TimeSpan _updateSpan)
        {
            m_processor = new Processor(20);
            m_vkApi = new Vk(_vkVersion);
            m_userManager = new UserManager();
            m_updateTimer = new Timer(_updateSpan.TotalMilliseconds);
            m_updateTimer.Elapsed += UpdateTimer_Elapsed;
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

                //todo convert

                NewDataGrabbedEventHandler?.Invoke(_user.Key, null);
            }
            catch(Exception ex)
            {
                //todo logging
            }
        }
    }

    public class Posts: List<Post>
    {

    }

    public class Post
    {
        public DateTime PublishTime { get; }
        public string GroupName { get; }

        IPostItem[] Items { get; }

        public Post(IPostItem[] _postItems)
        {
            Items = _postItems;
        }
    }

    public interface IPostItem
    {

    }

    public struct ImageItem : IPostItem
    {
        public string UrlSmall { get; }
        public string UrlMedium { get; }
        public string UrlLarge { get; }
    }

    public struct TextItem : IPostItem
    {
        public string Text { get; }

        public TextItem(string _text)
        {
            Text = _text;
        }
    }

    public struct VideoItem : IPostItem
    {
        public string Url { get; }
    }

    public struct Document : IPostItem
    {
        public string Url { get; set; }
    }

    public struct Music : IPostItem
    {
        public string Name { get; }
        public string Artist { get; }
        public string Url { get; }
    }

    internal static class Helpers
    {
        public static string ConvertGroupToSourceId(DbGroup _group)
        {
            if (string.IsNullOrWhiteSpace(_group.GroupPrefix))
                return $"{_group.GroupPrefix}{_group.GroupId}";

            return _group.GroupId.ToString();
        }
    }

    public class Processor
    {
        private readonly Queue<Task> m_queue;
        private readonly int m_maxPerformed;
        private readonly object m_syncObject = new object();
        private readonly List<Task> m_performedTasks;

        public Processor(int _maxPerformed)
        {
            m_queue = new Queue<Task>();
            m_performedTasks = new List<Task>();
            m_maxPerformed = _maxPerformed;
        }

        private void PerformTask()
        {
            lock (m_syncObject)
            {
                while (m_queue.Count > 0 && m_performedTasks.Count < m_maxPerformed)
                {
                    var task = m_queue.Dequeue();

                    task.ContinueWith((arg) =>
                    {
                        lock (m_syncObject)
                        {
                            m_performedTasks.Remove(task);
                            PerformTask();
                        }
                    });

                    m_performedTasks.Add(task);

                    task.Start();
                }
            }
        }

        public void Add(Action _action)
        {
            lock (m_syncObject)
            {
                m_queue.Enqueue(new Task(_action.Invoke));
                PerformTask();
            }
        }
    }

    public class UserManager
    {
        //подумать про проверку токена
        public void AddUser(string _token, string _key)
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new ArgumentException("Token can not be null or whitespace!");
            if (string.IsNullOrWhiteSpace(_key))
                throw new ArgumentException("Key can not be null or whitespace!");

            using(var context = new GrabberDbContext())
            {
                var existUser = context.Users.FirstOrDefault(_dbUser => _dbUser.Token == _token && _dbUser.Key != _key);

                if (existUser == null)
                {
                    var dbUser = new DbUser
                    {
                        Token = _token,
                        Key = _key
                    };

                    context.Users.Add(dbUser);
                }
                else
                    throw new ArgumentException("User already exists!");

                context.SaveChanges();
            }
        }

        public User GetUser(string _key)
        {
            using(var context = new GrabberDbContext())
            {
                var dbUser = context.Users.FirstOrDefault(_user => _user.Key == _key);

                if (dbUser != null)
                {
                    var userGroups = 
                    dbUser.Groups.Select(_dbGroup => new Group(_dbGroup.GroupId, _dbGroup.GroupPrefix, _dbGroup.UpdatePeriod)).ToArray();
                    var user = new User(dbUser.Token, dbUser.Key, userGroups);

                    return user;
                }

                return null;
            }
        }

        public void AddGroupToUser(string _key, Group _group)
        {
            if (_group == null)
                throw new ArgumentNullException(nameof(_group));

            using (var context = new GrabberDbContext())
            {
                var dbUser = context.Users.FirstOrDefault(_dbUser => _dbUser.Key == _key);

                if (dbUser == null)
                    throw new ArgumentException($"User with key {_key} not found!");

                var existedGroup = dbUser.Groups.FirstOrDefault(_dbGroup => _dbGroup.GroupId == _group.GroupId);

                if (existedGroup == null)
                {
                    var dbGroup = new DbGroup
                    {
                        GroupId = _group.GroupId,
                        GroupPrefix = _group.Prefix
                    };

                    context.Groups.Add(dbGroup);
                }
                else
                    throw new ArgumentException($"User already has group with id {_group.GroupId}");

                context.SaveChanges();
            }
        }

        public bool RemoveUser(string _key)
        {
            using (var context = new GrabberDbContext())
            {
                var dbUser = context.Users.FirstOrDefault(_dbUser => _dbUser.Key == _key);

                if (dbUser == null)
                    return false;

                context.Users.Remove(dbUser);

                context.SaveChanges();

                return true;
            }
        }

        public User[] GetUsers()
        {
            using (var context = new GrabberDbContext())
            {
                var users = context.Users.Select(_dbUser => new User(_dbUser.Token, _dbUser.Key,
                    _dbUser.Groups.Select(_dbGroup => new Group(_dbGroup.GroupId, _dbGroup.GroupPrefix, _dbGroup.UpdatePeriod)).ToArray())).ToArray();

                return users;
            }
        }
    }

    public class User
    {
        public string Token { get; }
        public string Key { get; }
        public Group[] Groups { get; }

        public User(string _token, string _key, Group[] _groups)
        {
            Token = _token;
            Key = _key;
            Groups = _groups;
        }
    }

    public class Group
    {
        public string Prefix { get; }
        public int GroupId { get; }
        public TimeSpan UpdatePeriod { get; }

        public Group(int _groupId, string _prefix, TimeSpan _updatePeriod)
        {
            GroupId = _groupId;
            Prefix = _prefix;
            UpdatePeriod = _updatePeriod;
        }
    }
}