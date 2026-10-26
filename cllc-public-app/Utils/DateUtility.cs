using System;

namespace Gov.Lclb.Cllb.Public.Utils
{
    /// <summary>
    /// Helper methods for working with dates and timezones.
    /// </summary>
    public class DateUtility
    {
        /// <summary>
        /// Zones that keep the US daylight saving rules (PST/PDT), as BC did before Nov 1, 2026.
        /// </summary>
        private static readonly string[] legacyPacificTimeZoneIds = new[]
        {
            "America/Los_Angeles", // IANA time zone code for US Pacific Time (PST/PDT)
            "Pacific Standard Time", // Traditional Windows time zone code for Pacific Time (PST/PDT)
        };

        /// <summary>
        /// The instant BC moves to permanent UTC-7 (2026-11-01 02:00 PDT).
        /// </summary>
        private static readonly DateTimeOffset permanentPacificOffsetStart = new DateTimeOffset(
            2026,
            11,
            1,
            9,
            0,
            0,
            TimeSpan.Zero
        );

        /// <summary>
        /// Finds the first available time zone from the given list of IDs.
        /// </summary>
        /// <param name="ids">The list of time zone IDs to search for.</param>
        /// <returns>The first available time zone from the list.</returns>
        private static TimeZoneInfo FindTimeZone(string[] ids)
        {
            foreach (var id in ids)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch (TimeZoneNotFoundException)
                {
                    // Do not throw, try next ID
                }
            }

            throw new TimeZoneNotFoundException("No Pacific timezone found for this platform.");
        }

        /// <summary>
        /// Gets the zone with the US PST/PDT rules that BC followed before Nov 1, 2026.
        /// </summary>
        /// <returns>The legacy Pacific Time Zone information.</returns>
        private static TimeZoneInfo GetLegacyPacificTimeZone()
        {
            return FindTimeZone(legacyPacificTimeZoneIds);
        }

        /// <summary>
        /// Gets today's date in Pacific Time.
        /// </summary>
        /// <returns>Today's date in Pacific Time, with no time component.</returns>
        public static DateTime GetPacificToday()
        {
            return FormatDatePacific(DateTimeOffset.UtcNow).Value.Date;
        }

        /// <summary>
        /// Converts the given UTC date to Pacific Time.
        /// </summary>
        /// <param name="inputDate">The UTC date to convert.</param>
        /// <param name="capturedOn">
        /// When the "inputDate" value was actually captured. If it was captured before Nov 1, 2026 then the date was 
        /// created under the old PST/PDT rules when it was saved to Dynamics. The same rules should be applied when 
        /// converting the date back to Pacific Time, even if the date value itself is on or after Nov 1, 2026.
        /// </param>
        /// <returns>The converted date in Pacific Time, or null if the input date is null.</returns>
        public static DateTime? FormatDatePacific(DateTimeOffset? inputDate, DateTimeOffset? capturedOn = null)
        {
            if (!inputDate.HasValue)
            {
                return null;
            }

            // A value captured before Nov 1, 2026 was saved under the legacy PST/PDT rules, even if it's dated after.
            var capturedUnderLegacyRules = capturedOn.HasValue && capturedOn.Value < permanentPacificOffsetStart;

            // Fixed UTC-7 after Nov 1, 2026, so hosts without BC's updated timezone data don't fall back to PST (UTC-8)
            if (inputDate.Value >= permanentPacificOffsetStart && !capturedUnderLegacyRules)
            {
                return DateTime.SpecifyKind(inputDate.Value.UtcDateTime.AddHours(-7), DateTimeKind.Unspecified);
            }

            // A value captured before Nov 1, 2026 should be converted using the legacy PST/PDT rules.
            return TimeZoneInfo.ConvertTimeFromUtc(inputDate.Value.UtcDateTime, GetLegacyPacificTimeZone());
        }
    }
}
