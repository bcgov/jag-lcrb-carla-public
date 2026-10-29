using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Utils
{
    public class DateUtility
    {
        // BC moves to permanent UTC-7 when DST ends on 2026-11-01 (02:00 PDT = 09:00 UTC). It is applied explicitly so
        // results don't depend on the host's tz database (e.g. a container base image) having the new rules.
        private static readonly DateTime PermanentUtcMinus7StartUtc = new DateTime(2026, 11, 1, 9, 0, 0, DateTimeKind.Utc);
        private static readonly TimeSpan PermanentUtcMinus7Offset = TimeSpan.FromHours(-7);

        /// <summary>
        /// Converts a UTC instant to Pacific Time (wall clock, Kind=Unspecified).
        /// </summary>
        public static DateTime ConvertUtcToPacific(DateTime utc)
        {
            var utcValue = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            if (utcValue >= PermanentUtcMinus7StartUtc)
            {
                return DateTime.SpecifyKind(utcValue + PermanentUtcMinus7Offset, DateTimeKind.Unspecified);
            }

            TimeZoneInfo hwZone;
            try
            {
                hwZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
            }
            catch (TimeZoneNotFoundException)
            {
                hwZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            }

            return TimeZoneInfo.ConvertTimeFromUtc(utcValue, hwZone);
        }

        public static DateTime? FormatDatePacific(DateTimeOffset? inputDate)
        {
            return inputDate.HasValue
                ? ConvertUtcToPacific(inputDate.Value.UtcDateTime)
                : (DateTime?)null;
        }

        /// <summary>
        /// Today's date in Pacific Time, regardless of the server's timezone.
        /// </summary>
        public static DateTime GetPacificToday()
        {
            return FormatDatePacific(DateTimeOffset.UtcNow).Value.Date;
        }
    }
}
