using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VkGrabber.DataLayer;

namespace VkGrabber
{
    public class UserManager
    {
        //подумать про проверку токена
        public async Task AddUserAsync(string _key, string _token, CancellationToken _cancellationToken)
        {
            await using var context = new GrabberDbContext();
            var existUser =
                await context.DbUsers.FirstOrDefaultAsync(_dbUser => _dbUser.Key == _key, _cancellationToken);

            if (existUser == null)
            {
                var dbUser = new DbUser
                {
                    Token = _token,
                    Key = _key
                };

                context.DbUsers.Add(dbUser);

                await context.SaveChangesAsync(_cancellationToken);
            }
            else
                throw new InvalidOperationException("User already exists!");
        }

        public async Task<User> GetUserAsync(string _key, CancellationToken _cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_key))
                throw new ArgumentException("Key can not be null or white space!", nameof(_key));

            await using var context = new GrabberDbContext();
            var dbUser = await context.DbUsers.FirstOrDefaultAsync(_user => _user.Key == _key, _cancellationToken);

            if (dbUser == null)
                return null;

            var userGroups = await context.DbGroups.Where(_dbGroup => _dbGroup.DbUser.Id == dbUser.Id).Select(
                    _dbGroup =>
                        new Group(_dbGroup.GroupId, _dbGroup.UpdatePeriod, _dbGroup.GroupName))
                .ToArrayAsync(_cancellationToken);

            var user = new User(dbUser.Token, dbUser.Key, userGroups);

            return user;
        }

        public async Task AddGroupToUser(string _key, Group _group, CancellationToken _cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_key))
                throw new ArgumentException("Key can not be null or white space!", nameof(_key));

            if (_group == null)
                throw new ArgumentNullException(nameof(_group));

            await using var context = new GrabberDbContext();
            var dbUser = await context.DbUsers.FirstOrDefaultAsync(_dbUser => _dbUser.Key == _key, _cancellationToken);

            if (dbUser == null)
                throw new ArgumentException($"User with key {_key} not found!");

            var existedGroup = await context.DbGroups.FirstOrDefaultAsync(_dbGroup =>
                _dbGroup.GroupId == _group.GroupId && _dbGroup.DbUser.Id == dbUser.Id, _cancellationToken);

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

                await context.DbGroups.AddAsync(dbGroup, _cancellationToken);
            }
            else
                throw new InvalidOperationException($"User already has group with id {_group.GroupId}");

            await context.SaveChangesAsync(_cancellationToken);
        }

        public async Task<bool> RemoveUser(string _key, CancellationToken _cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_key))
                throw new ArgumentException("Key can not be null or white space!", nameof(_key));

            await using var context = new GrabberDbContext();
            var dbUser = await context.DbUsers.FirstOrDefaultAsync(_dbUser => _dbUser.Key == _key, _cancellationToken);

            if (dbUser == null)
                return false;

            var userGroups = await context.DbGroups.Where(_dbGroup => _dbGroup.DbUser.Id == dbUser.Id)
                .ToArrayAsync(_cancellationToken);
            context.DbGroups.RemoveRange(userGroups);

            context.DbUsers.Remove(dbUser);

            await context.SaveChangesAsync(_cancellationToken);

            return true;
        }

        public User[] GetUsers()
        {
            using var context = new GrabberDbContext();
            var users = context.DbUsers.Include(x => x.DbGroups).Select(_dbUser => new User(_dbUser.Token, _dbUser.Key,
                _dbUser.DbGroups.Select(_dbGroup =>
                    new Group(_dbGroup.GroupId, _dbGroup.UpdatePeriod, _dbGroup.GroupName)).ToArray()));

            return users.ToArray();
        }

        public async Task<bool> RemoveGroupFromUser(string _key, Group _group, CancellationToken _cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_key))
                throw new ArgumentException("Key can not be null or white space!", nameof(_key));

            if (_group == null)
                throw new ArgumentNullException(nameof(_group));

            await using var context = new GrabberDbContext();
            var dbUser = await context.DbUsers.FirstOrDefaultAsync(_dbUser => _dbUser.Key == _key, _cancellationToken);

            if (dbUser == null)
                throw new InvalidOperationException("Failed find user");

            var dbGroup = context.DbGroups.FirstOrDefault(_dbGroup =>
                _dbGroup.GroupPrefix == _group.Prefix &&
                _dbGroup.GroupId == _group.GroupId &&
                _dbGroup.DbUser.Id == dbUser.Id);

            if (dbGroup == null)
                return false;

            context.DbGroups.Remove(dbGroup);
            await context.SaveChangesAsync(_cancellationToken);

            return true;
        }
    }
}