using System;

namespace VkApi.Requests
{
    /// <summary>
    /// Permissions. For more https://vk.com/dev/permissions
    /// </summary>
    [Flags]
    public enum Permissions
    {
        Friends = 2,
        Photos = 4,
        Audio = 8,
        Video = 16,
        Messages = 32,
        Offline = 64,
        Groups = 128,
        Docs = 256,
        Wall = 512,
    }
}
