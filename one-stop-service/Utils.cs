using System;

namespace Gov.Jag.Lcrb.OneStopService
{
    public static class Utils
    {
        public static string FormatPostalCode(string input)
        {
            string result = null;
            if (input != null)
            {
                result = input.ToUpper().Replace(" ", "");
            }
            return result;
        }

        public static string ParseGuid(string guid)
        {
            string result = null;
            Guid guidValue;
            if (Guid.TryParse(guid, out guidValue))
            {
                result = guidValue.ToString();
            }
            return result;
        }

        public static string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd-hh.mm.ss.000000");
        }

        public static DateTime GetPacificNow()
        {
            // BC moves to permanent UTC-7 on 2026-11-01 at 09:00 UTC; applied explicitly so it doesn't depend on the host's tz database.
            var utcNow = DateTime.UtcNow;
            if (utcNow >= new DateTime(2026, 11, 1, 9, 0, 0, DateTimeKind.Utc))
            {
                return DateTime.SpecifyKind(utcNow.AddHours(-7), DateTimeKind.Unspecified);
            }

            TimeZoneInfo pacificZone;
            try
            {
                pacificZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
            }
            catch (TimeZoneNotFoundException)
            {
                pacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            }

            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, pacificZone);
        }
    }
}
