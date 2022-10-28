using System;

namespace VkApi.Converters
{
    public static class EpochTimeConverter
    {
        private static readonly DateTime m_startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ConvertToDateTime(long _seconds)
        {
            return m_startTime.AddSeconds(_seconds);
        }

        public static long ConvertFromDateTime(DateTime _dateTime)
        {
            var span = _dateTime - m_startTime;

            var seconds = (long)span.TotalSeconds;

            return seconds;
        }
    }
}
