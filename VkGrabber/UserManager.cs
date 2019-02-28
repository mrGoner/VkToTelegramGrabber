using System;
using System.Linq;
using VkGrabber.DataLayer;

namespace VkGrabber
{
    public class UserManager
    {
        //подумать про проверку токена
        public void AddUser(string _token, string _key)
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new ArgumentException("Token can not be null or whitespace!");
            if (string.IsNullOrWhiteSpace(_key))
                throw new ArgumentException("Key can not be null or whitespace!");

            using (var context = new GrabberDbContext())
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
            using (var context = new GrabberDbContext())
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
}