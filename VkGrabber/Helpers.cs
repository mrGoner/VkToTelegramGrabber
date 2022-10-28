using VkGrabber.DataLayer;

namespace VkGrabber
{
    internal static class Helpers
    {
        public static string ConvertGroupToSourceId(GroupInfo _group)
        {
            if (!string.IsNullOrWhiteSpace(_group.Prefix))
                return $"{_group.Prefix}{_group.Id}";

            return _group.Id.ToString();
        }
    }
}