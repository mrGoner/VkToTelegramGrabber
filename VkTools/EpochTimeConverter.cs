using System;

namespace VkTools
{
    internal static class EpochTimeConverter
    {
        private static readonly DateTime m_startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ConvertToDateTime(int _seconds)
        {
            return m_startTime.AddSeconds(_seconds);
        }
    }
}
