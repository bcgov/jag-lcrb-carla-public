using System;

namespace Gov.Lclb.Cllb.Public.Utils
{
    /// <summary>
    /// Helper methods for working with dates and timezones.
    /// </summary>
    public class DateUtility
    {
        /// <summary>
        /// <list type="bullet">
        /// <item>"America/Vancouver" is the IANA time zone code used by most Unix-based systems.</item>
        /// <item>"British Columbia Standard Time" is the new Windows time zone code for Pacific Time in British Columbia post Nov 1, 2026.</item>
        /// <item>"Pacific Standard Time" is the traditional Windows time zone code for Pacific Time.</item>
        /// </list>
        /// </summary>
        private static readonly string[] pacificTimeZoneIds = new[]
        {
            "America/Vancouver",
            "British Columbia Standard Time",
            "Pacific Standard Time",
        };

        /// <summary>
        /// Zones that keep the US daylight saving rules (PST/PDT), as BC did before Nov 1, 2026.
        /// </summary>
        private static readonly string[] legacyPacificTimeZoneIds = new[]
        {
            "America/Los_Angeles",
            "Pacific Standard Time",
        };

        /// <summary>
        /// The instant BC moves to permanent UTC-7 (2026-11-01 02:00 PDT).
        /// </summary>
        private static readonly DateTimeOffset permanentPacificOffsetStart = new DateTimeOffset(
            2026,
            10,
            15,
            23,
            59,
            59,
            TimeSpan.Zero
        );

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
        /// Gets the Pacific Time Zone information for the current platform.
        /// </summary>
        /// <returns>The Pacific Time Zone information.</returns>
        public static TimeZoneInfo GetPacificTimeZone()
        {
            return FindTimeZone(pacificTimeZoneIds);
        }

        private static TimeZoneInfo GetLegacyPacificTimeZone()
        {
            return FindTimeZone(legacyPacificTimeZoneIds);
        }

        /// <summary>
        /// Converts the given UTC date to Pacific Time.
        /// </summary>
        /// <param name="inputDate">The UTC date to convert.</param>
        /// <param name="capturedOn">
        /// When the value was entered. If it was entered before Nov 1, 2026 but contains a value on or after Nov 1,2026
        /// then the date was created under the old PST/PDT rules when it was saved to Dynamics. The same rules should
        /// be applied when converting the date back to Pacific Time..
        /// </param>
        /// <returns>The converted date in Pacific Time, or null if the input date is null.</returns>
        public static DateTime? FormatDatePacific(DateTimeOffset? inputDate, DateTimeOffset? capturedOn = null)
        {
            if (!inputDate.HasValue)
            {
                return null;
            }

            // Determine if the inputDate was captured before Nov 1, 2026 but contains a value on or after Nov 1, 2026.
            // Such a date would be affected by the legacy PST/PDT rules rather than the new PCT rules.
            var capturedUnderLegacyRules =
                capturedOn.HasValue
                && capturedOn.Value < permanentPacificOffsetStart
                && inputDate.Value >= permanentPacificOffsetStart;

            var pacificTimeZone = capturedUnderLegacyRules ? GetLegacyPacificTimeZone() : GetPacificTimeZone();

            return TimeZoneInfo.ConvertTimeFromUtc(inputDate.Value.DateTime, pacificTimeZone);
        }
    }
}
