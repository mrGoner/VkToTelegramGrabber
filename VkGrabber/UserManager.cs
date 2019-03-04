using System;
using System.Linq;
using VkGrabber.DataLayer;

namespace VkGrabber
{
    public class UserManager
    {
        //подумать про проверку токена
        public void AddUser(string _key, string _token)
        {
            using (var context = new GrabberDbContext())
            {
                var existUser = context.DbUsers.FirstOrDefault(_dbUser => _dbUser.Key == _key);

                if (existUser == null)
                {
                    var dbUser = new DbUser
                    {
                        Token = _token,
                        Key = _key
                    };

                    context.DbUsers.Add(dbUser);

                    context.SaveChanges();
                }
                else
                    throw new ArgumentException("User already exists!");
            }
        }

        public User GetUser(string _key)
        {
            if (string.IsNullOrWhiteSpace(_key))
                throw new ArgumentException("Key can not be null or white space!", nameof(_key));

            using (var context = new GrabberDbContext())
            {
                var dbUser = context.DbUsers.FirstOrDefault(_user => _user.Key == _key);

                if (dbUser != null)
                {
                    var userGroups =
                    context.DbGroups.Where(_dbGroup => _dbGroup.DbUser.Id == dbUser.Id).Select(_dbGroup =>
                        new Group(_dbGroup.GroupId, _dbGroup.UpdatePeriod, _dbGroup.GroupName)).ToArray();
                    var user = new User(dbUser.Token, dbUser.Key, userGroups);

                    return user;
                }

                return null;
            }
        }

        public void AddGroupToUser(string _key, Group _group)
        {
            if (string.IsNullOrWhiteSpace(_key))
                throw new ArgumentException("Key can not be null or white space!", nameof(_key));

            if (_group == null)
                throw new ArgumentNullException(nameof(_group));

            using (var context = new GrabberDbContext())
            {
                var dbUser = context.DbUsers.FirstOrDefault(_dbUser => _dbUser.Key == _key);

                if (dbUser == null)
                    throw new ArgumentException($"User with key {_key} not found!");

                var existedGroup = context.DbGroups.FirstOrDefault(_dbGroup => 
                    _dbGroup.GroupId == _group.GroupId && _dbGroup.DbUser.Id == dbUser.Id);

                if (existedGroup == null)
                {
                    var dbGroup = new DbGroup
                    {
                        GroupId = _group.GroupId,
                        GroupPrefix = _group.Prefix,
                        GroupName = _group.Name,
                        UpdatePeriod = _group.UpdatePeriod,
                        DbUser = dbUser,
                        LastUpdateDateTime = DateTime.Now.ToUniversalTime()
                    };

                    context.DbGroups.Add(dbGroup);
                }
                else
                    throw new ArgumentException($"User already has group with id {_group.GroupId}");

                context.SaveChanges();
            }
        }

        public bool RemoveUser(string _key)
        {
            if (string.IsNullOrWhiteSpace(_key))
                throw new ArgumentException("Key can not be null or white space!", nameof(_key));

            using (var context = new GrabberDbContext())
            {
                var dbUser = context.DbUsers.FirstOrDefault(_dbUser => _dbUser.Key == _key);

                if (dbUser == null)
                    return false;

                var userGroups = context.DbGroups.Where(_dbGroup => _dbGroup.DbUser.Id == dbUser.Id).ToArray();
                context.DbGroups.RemoveRange(userGroups);

                context.DbUsers.Remove(dbUser);

                context.SaveChanges();

                return true;
            }
        }

        public User[] GetUsers()
        {
            using (var context = new GrabberDbContext())
            {
                var users = context.DbUsers.Select(_dbUser => new User(_dbUser.Token, _dbUser.Key,
                    context.DbGroups.Where(_dbGroup => _dbGroup.DbUser.Id == _dbUser.Id).Select(_dbGroup =>
                        new Group(_dbGroup.GroupId, _dbGroup.UpdatePeriod, _dbGroup.GroupName)).ToArray()));

                return users.ToArray();
            }
        }

        public bool RemoveGroupFromUser(string _key, Group _group)
        {
            if (string.IsNullOrWhiteSpace(_key))
                throw new ArgumentException("Key can not be null or white space!", nameof(_key));

            if (_group == null)
                throw new ArgumentNullException(nameof(_group));

            using (var context = new GrabberDbContext())
            {
               var dbUser = context.DbUsers.FirstOrDefault(_dbUser => _dbUser.Key == _key);

                if (dbUser != null)
                {
                    var dbGroup = context.DbGroups.FirstOrDefault(_dbGroup =>
                        _dbGroup.GroupPrefix == _group.Prefix && 
                        _dbGroup.GroupId == _group.GroupId && 
                        _dbGroup.DbUser.Id == dbUser.Id);

                    if (dbGroup != null)
                    {
                        context.DbGroups.Remove(dbGroup);

                        context.SaveChanges();

                        return true;
                    }
                }

                return false;
            }
        }
    }
}