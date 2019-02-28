using VkGrabber.DataLayer;

namespace VkGrabber
{
    internal static class Helpers
    {
        public static string ConvertGroupToSourceId(DbGroup _group)
        {
            if (string.IsNullOrWhiteSpace(_group.GroupPrefix))
                return $"{_group.GroupPrefix}{_group.GroupId}";

            return _group.GroupId.ToString();
        }
    }
}