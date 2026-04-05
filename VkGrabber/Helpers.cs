using VkGrabber.DataLayer;

namespace VkGrabber;

internal static class Helpers
{
    public static string ConvertGroupToSourceId(GroupInfo group)
    {
        if (!string.IsNullOrWhiteSpace(group.Prefix))
            return $"{group.Prefix}{group.Id}";

        return group.Id.ToString();
    }
}