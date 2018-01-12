using System;

namespace Euricom.IoT.Common.Utilities
{
    public static class DateTimeHelpers
    {
        public static string Timestamp()
        {
            return DateTime.Now.ToString("yyyMMddHHmmssfff");
        }
        public static string Timestamp(this DateTime datetime)
        {
            return datetime.ToString("yyyMMddHHmmssfff");
        }
    }
}
