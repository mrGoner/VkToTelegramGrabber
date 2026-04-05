using System;

namespace VkApi.Converters;

public static class EpochTimeConverter
{
    private static readonly DateTime s_startTime = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime ConvertToDateTime(long seconds)
    {
        return s_startTime.AddSeconds(seconds);
    }

    public static long ConvertFromDateTime(DateTime dateTime)
    {
        var span = dateTime - s_startTime;

        var seconds = (long)span.TotalSeconds;

        return seconds;
    }
}