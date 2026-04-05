using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VkGrabber.DataLayer;

namespace VkGrabber;

public class UserManager(string pathToDb)
{
    private readonly DbContextFactory _contextFactory = new(pathToDb);

    //подумать про проверку токена
    public async Task AddUserAsync(string key, string token, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.CreateContext();
        var existUser = await context.DbUsers.FirstOrDefaultAsync(user => user.Key == key, cancellationToken);

        if (existUser == null)
        {
            var dbUser = new DbUser
            {
                Token = token,
                Key = key
            };

            context.DbUsers.Add(dbUser);

            await context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("User already exists!");
        }
    }

    public async Task<User?> GetUserAsync(string key, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or white space!", nameof(key));

        await using var context = _contextFactory.CreateContext();
        var dbUser = await context.DbUsers.FirstOrDefaultAsync(user => user.Key == key, cancellationToken);

        if (dbUser == null)
            return null;

        var userGroups = await context.DbGroups
            .Where(dbGroup => dbGroup.DbUser.Id == dbUser.Id)
            .Select(dbGroup => new Group(dbGroup.GroupId, dbGroup.UpdatePeriod, dbGroup.GroupName))
            .ToArrayAsync(cancellationToken);

        var user = new User(dbUser.Token, dbUser.Key, userGroups);

        return user;
    }

    public async Task AddGroupToUserAsync(string key, Group group, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or white space!", nameof(key));

        if (group == null)
            throw new ArgumentNullException(nameof(group));

        await using var context = _contextFactory.CreateContext();
        var dbUser = await context.DbUsers.FirstOrDefaultAsync(dbUser => dbUser.Key == key, cancellationToken);

        if (dbUser == null)
            throw new ArgumentException($"User with key {key} not found!");

        var existedGroup = await context.DbGroups
            .FirstOrDefaultAsync(dbGroup => dbGroup.GroupId == group.GroupId && dbGroup.DbUser.Id == dbUser.Id, cancellationToken);

        if (existedGroup == null)
        {
            var dbGroup = new DbGroup
            {
                GroupId = group.GroupId,
                GroupPrefix = group.Prefix,
                GroupName = group.Name,
                UpdatePeriod = group.UpdatePeriod,
                DbUser = dbUser,
                LastUpdateDateTime = DateTime.Now.ToUniversalTime()
            };

            await context.DbGroups.AddAsync(dbGroup, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException($"User already has group with id {group.GroupId}");
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RemoveUser(string key, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or white space!", nameof(key));

        await using var context = _contextFactory.CreateContext();
        var dbUser = await context.DbUsers.FirstOrDefaultAsync(dbUser => dbUser.Key == key, cancellationToken);

        if (dbUser == null)
            return false;

        var userGroups = await context.DbGroups.Where(dbGroup => dbGroup.DbUser.Id == dbUser.Id)
            .ToArrayAsync(cancellationToken);
        
        context.DbGroups.RemoveRange(userGroups);

        context.DbUsers.Remove(dbUser);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RemoveGroupFromUser(string key, Group group, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or white space!", nameof(key));

        if (group == null)
            throw new ArgumentNullException(nameof(group));

        await using var context = _contextFactory.CreateContext();
        var dbUser = await context.DbUsers.FirstOrDefaultAsync(dbUser => dbUser.Key == key, cancellationToken);

        if (dbUser == null)
            throw new InvalidOperationException("Failed find user");

        var dbGroup = context.DbGroups.FirstOrDefault(dbGroup => dbGroup.GroupPrefix == group.Prefix &&
                                                                 dbGroup.GroupId == group.GroupId &&
                                                                 dbGroup.DbUser.Id == dbUser.Id);

        if (dbGroup == null)
            return false;

        context.DbGroups.Remove(dbGroup);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}