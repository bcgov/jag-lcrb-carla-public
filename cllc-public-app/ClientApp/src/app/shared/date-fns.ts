import { FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';

export const DEFAULT_START_TIME = {
  hour: 9,
  minute: 0
};

export const DEFAULT_END_TIME = {
  hour: 23,
  minute: 0
};

export const DAYS = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

const PST_OFFSET_MS = -8 * 60 * 60 * 1000; // -8 hours - Pacific Standard Time (winter)
const PDT_OFFSET_MS = -7 * 60 * 60 * 1000; // -7 hours - Pacific Daylight Time (summer)
const PCT_OFFSET_MS = -7 * 60 * 60 * 1000; // -7 hours - Pacific Constant Time (permanent UTC-7)

// BC moves to permanent UTC-7 at this instant (2026-11-01 02:00 PDT).
const PCT_OFFSET_START = new Date('2026-11-01T09:00:00Z');

const legacyPacificFormat = new Intl.DateTimeFormat('en-US', {
  timeZone: 'America/Los_Angeles',
  year: 'numeric',
  month: 'numeric',
  day: 'numeric',
  hour: 'numeric',
  minute: 'numeric',
  second: 'numeric',
  hour12: false
});

/**
 * Determine the legacy pacific offset in effect based on the provided instant.
 *
 * @param {number} instantMs
 * @return {*}  {number}
 */
function getLegacyPacificOffset(instantMs: number): number {
  const parts = legacyPacificFormat.formatToParts(new Date(instantMs));

  const part = (type: string) => Number(parts.find((p) => p.type === type)?.value);

  // some engines format midnight as hour 24
  const wallAsUtc = Date.UTC(
    part('year'),
    part('month') - 1,
    part('day'),
    part('hour') % 24,
    part('minute'),
    part('second')
  );

  return wallAsUtc - Math.floor(instantMs / 1000) * 1000; // offset in milliseconds
}

/**
 * True if values captured at this time were saved under the permanent UTC-7 rules, which takes effect Nov 1, 2026.
 *
 * @param {(Date | string | null)} [capturedOn] The date the value was captured. Defaults to now.
 * @return {*}  {boolean}
 */
function isCapturedUnderPermanentOffset(capturedOn?: Date | string | null): boolean {
  const captured = capturedOn ? new Date(capturedOn) : new Date();
  return captured >= PCT_OFFSET_START;
}

/**
 * Converts a UTC instant from the server into the Pacific wall-clock time to show the user, for display purposes or to populate a form control.
 *
 * The returned Date's local fields (getHours(), getDate(), ...) hold the Pacific wall-clock time in any browser timezone.
 * Its underlying instant is not the real moment, so pass it through fromPacificWallClockToUtc before saving or comparing it to real instants.
 *
 * Pass capturedOn for stored values. A value saved before BC moved to permanent UTC-7 (Nov 1, 2026) used the legacy PST/PDT rules,
 * even if it is dated after the move, so capturedOn dictates which rule to use when decoding it.
 *
 * @param value The UTC instant to convert
 * @param capturedOn When the value was saved (e.g. the record's createdOn); defaults to now
 * @example
 * // Saved Oct 15, before the move, so Nov 20 is decoded as PST (UTC-8).
 * // Returns Nov 20, 2026 09:00 in local fields, whatever the device timezone is.
 * fromUtcToPacificWallClock('2026-11-20T17:00:00Z', '2026-10-15T00:00:00Z');
 *
 * // Saved Nov 2, after the move, so Nov 20 is decoded as PCT (UTC-7).
 * // Returns Nov 20, 2026 09:00 in local fields, whatever the device timezone is.
 * fromUtcToPacificWallClock('2026-11-20T16:00:00Z', '2026-11-02T00:00:00Z');
 */
export function fromUtcToPacificWallClock(utcInstant: Date | string, capturedOn?: Date | string | null): Date {
  const instant = new Date(utcInstant);

  const instantMS = instant.getTime();

  if (isNaN(instantMS)) {
    return instant;
  }

  // Returns true if the instant AND capturedOn date are both after Nov 1, 2026. Returns false otherwise.
  const isUsingPermanentOffset = instant >= PCT_OFFSET_START && isCapturedUnderPermanentOffset(capturedOn);

  // Returns -7 hours if using permanent offset. Returns either -7 or -8 hours depending on legacy PST/PDT rules otherwise.
  const offset = isUsingPermanentOffset ? PCT_OFFSET_MS : getLegacyPacificOffset(instantMS);

  // Pacific wall-clock time.
  const pacificWallClock = new Date(instantMS + offset);

  return new Date(
    pacificWallClock.getUTCFullYear(),
    pacificWallClock.getUTCMonth(),
    pacificWallClock.getUTCDate(),
    pacificWallClock.getUTCHours(),
    pacificWallClock.getUTCMinutes(),
    pacificWallClock.getUTCSeconds(),
    pacificWallClock.getUTCMilliseconds()
  );
}

/**
 * Treats a form value as Pacific wall-clock time, regardless of the device timezone, and converts it to the UTC instant to send to the server.
 *
 * Use it on any date or time taken from a form before saving, so the stored instant doesn't depend on the browser's timezone.
 * The rule is chosen by when the value is saved (now): before Nov 1, 2026 it uses legacy PST/PDT, after it uses PCT (UTC-7).
 * A value for a date after the move that is saved before it is still encoded as PST/PDT.
 *
 * @param pacificWallClock The Pacific wall-clock time, held in the Date's local fields (e.g. the form value)
 * @example
 * // Entered Nov 20, 09:00 on Oct 15 (before the move): encoded as PST (UTC-8), on any device timezone.
 * fromPacificWallClockToUtc(new Date(2026, 10, 20, 9, 0)); // 2026-11-20T17:00:00.000Z
 *
 * // Entered Nov 20, 09:00 on Nov 2 (after the move): encoded as PCT (UTC-7), on any device timezone.
 * fromPacificWallClockToUtc(new Date(2026, 10, 20, 9, 0)); // 2026-11-20T16:00:00.000Z
 */
export function fromPacificWallClockToUtc(pacificWallClock: Date): Date {
  const wallAsUtc = Date.UTC(
    pacificWallClock.getFullYear(),
    pacificWallClock.getMonth(),
    pacificWallClock.getDate(),
    pacificWallClock.getHours(),
    pacificWallClock.getMinutes(),
    pacificWallClock.getSeconds(),
    pacificWallClock.getMilliseconds()
  );

  if (isNaN(wallAsUtc)) {
    return pacificWallClock;
  }

  // Calculate the instant assuming the permanent Pacific offset (UTC-7) is in effect.
  const permanentInstant = wallAsUtc - PCT_OFFSET_MS;

  // If the permanent offset applies, return the permanent instant immediately.
  if (permanentInstant >= PCT_OFFSET_START.getTime() && isCapturedUnderPermanentOffset()) {
    return new Date(permanentInstant);
  }

  // The offset depends on the instant, so estimate it from PST and then re-check after.
  const estimate = wallAsUtc - getLegacyPacificOffset(wallAsUtc - PST_OFFSET_MS); // Subtract the negative PST offset

  return new Date(wallAsUtc - getLegacyPacificOffset(estimate));
}

/* JavaScript getMonthlyWeekday Function:
 * Written by Ian L. of Jafty.com
 *
 * Description:
 * Gets Nth weekday for given month/year. For example, it can give you the date of the first monday in January, 2017 or it could give you the third Friday of June, 1999. Can get up to the fifth weekday of any given month, but will return FALSE if there is no fifth day in the given month/year.
 *
 *
 * Parameters:
 *    n = 1-5 for first, second, third, fourth or fifth weekday of the month
 *    d = full spelled out weekday Monday-Friday
 *    m = Full spelled out month like June
 *    y = Four digit representation of the year like 2017
 *
 * Return Values:
 * returns 1-31 for the date of the queried month/year that the nth weekday falls on.
 * returns false if there isn’t an nth weekday in the queried month/year
 */
export function getMonthlyWeekday(n, d, m, y) {
  var targetDay,
    curDay = 0,
    i = 1,
    seekDay;
  if (d == 'sunday') seekDay = 0;
  if (d == 'monday') seekDay = 1;
  if (d == 'tuesday') seekDay = 2;
  if (d == 'wednesday') seekDay = 3;
  if (d == 'thursday') seekDay = 4;
  if (d == 'friday') seekDay = 5;
  if (d == 'saturday') seekDay = 6;
  while (curDay < n && i < 31) {
    targetDay = new Date(i++ + ' ' + m + ' ' + y);
    if (targetDay.getDay() == seekDay) curDay++;
  }
  if (curDay == n) {
    targetDay = targetDay.getDate();
    return targetDay;
  } else {
    return false;
  }
} //end getMonthlyWeekday JS function

/**
 * Returns all days within a date range
 * @param start The start date
 * @param end The end date
 */
export function getDaysArray(start: string | Date, end: string | Date): Date[] {
  start = new Date(start);
  end = new Date(end);
  const arr: Date[] = [];
  for (let dt = start; dt <= end; dt.setDate(dt.getDate() + 1)) {
    arr.push(new Date(dt));
  }
  return arr;
}

/**
 * Validates a date range
 * @param firstControl The start date field
 * @param secondControl The end date field
 */
export function dateRangeValidator(firstControl: string, secondControl: string): ValidatorFn {
  return (fg: FormGroup): ValidationErrors | null => {
    const start: string | Date = fg.get(firstControl)?.value;
    const end: string | Date = fg.get(secondControl)?.value;

    if (start !== null && start !== '' && end !== null && end !== '') {
      const startDate = new Date(start);
      const endDate = new Date(end);
      return startDate <= endDate ? null : { dateRange: true };
    }

    // No errors - skip validation if one (or both) fields are empty
    return null;
  };
}

/**
 * Safely converts a date to ISO string format.
 * Returns null for invalid dates or null/undefined inputs.
 *
 * @example
 * ```typescript
 * const date = safeToDateString('2023-03-15');
 * console.log(date); // Output: '2023-03-15T00:00:00.000Z'
 * ```
 *
 * @example
 * ```typescript
 * const date = safeToDateString(new Date());
 * console.log(date); // Output: '2023-03-15T00:00:00.000Z'
 * ```
 *
 * @example
 * ```typescript
 * const date = safeToDateString(null);
 * console.log(date); // Output: null
 * ```
 *
 * @export
 * @param {(Date | string | null | undefined)} date
 * @return {(string | null)}
 */
export function safeToISOString(date: Date | string | null | undefined): string | null {
  if (!date) {
    return null;
  }

  try {
    const dateObj = date instanceof Date ? date : new Date(date);

    if (isNaN(dateObj.getTime())) {
      return null;
    }

    return dateObj.toISOString();
  } catch {
    return null;
  }
}

/**
 * Safely converts a date to an ISO String and returns the date portion.
 *
 * @example
 * ```typescript
 * const date = safeToDateString('2023-03-15T12:34:56Z');
 * console.log(date); // Output: '2023-03-15'
 * ```
 *
 * @example
 * ```typescript
 * const date = safeToDateString(new Date());
 * console.log(date); // Output: '2023-03-15'
 * ```
 *
 * @example
 * ```typescript
 * const date = safeToDateString(null);
 * console.log(date); // Output: null
 * ```
 *
 * @export
 * @param {(Date | string | null | undefined)} date
 * @return {(string | null)}
 */
export function safeToDateString(dateString: Date | string | null | undefined): string | null {
  const isoString = safeToISOString(dateString);

  if (!isoString) {
    return null;
  }

  return isoString.split('T')[0];
}
