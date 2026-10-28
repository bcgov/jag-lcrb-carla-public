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
