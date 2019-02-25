using System.Collections.Generic;

namespace VkGrabber
{
    public interface IUserManager
    {
        void AddGroupsToUser(User _user, List<Group> _groups);
        void AddUser(User _user);
        User GetUser(User _user);
        bool RemoveUser(User _user);
    }
}