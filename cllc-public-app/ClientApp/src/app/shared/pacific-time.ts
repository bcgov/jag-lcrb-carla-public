/**
 * Event dates and times are always entered and displayed as Pacific Time, regardless of the browser's timezone.
 *
 * Within the UI, a "Pacific wall-clock" Date is a Date whose local fields (getHours(), getDate(), format(), etc.)
 * read as the Pacific Time value. At the API boundary these are converted to/from real instants (UTC).
 */
export const PACIFIC_TIME_ZONE = 'America/Vancouver';

const pacificFormat = new Intl.DateTimeFormat('en-US', {
  timeZone: PACIFIC_TIME_ZONE,
  hourCycle: 'h23',
  year: 'numeric',
  month: 'numeric',
  day: 'numeric',
  hour: 'numeric',
  minute: 'numeric',
  second: 'numeric'
} as Intl.DateTimeFormatOptions);

interface DateFields {
  year: number;
  month: number; // 0-11
  day: number;
  hour: number;
  minute: number;
  second: number;
}

/**
 * BC moves to permanent UTC-7 when DST ends on 2026-11-01 (02:00 PDT = 09:00 UTC). It is applied explicitly so results
 * don't depend on the browser's tz database already having the new rules. Earlier instants use the IANA rules.
 */
const PERMANENT_UTC_MINUS_7_START_MS = Date.UTC(2026, 10, 1, 9, 0, 0);
const PERMANENT_UTC_MINUS_7_OFFSET_MS = -7 * 60 * 60 * 1000;

function getPacificFields(instantMs: number): DateFields {
  const fields = {} as DateFields;
  if (instantMs >= PERMANENT_UTC_MINUS_7_START_MS) {
    const shifted = new Date(instantMs + PERMANENT_UTC_MINUS_7_OFFSET_MS);
    fields.year = shifted.getUTCFullYear();
    fields.month = shifted.getUTCMonth();
    fields.day = shifted.getUTCDate();
    fields.hour = shifted.getUTCHours();
    fields.minute = shifted.getUTCMinutes();
    fields.second = shifted.getUTCSeconds();
    return fields;
  }
  const keys: { [type: string]: keyof DateFields } = {
    year: 'year',
    month: 'month',
    day: 'day',
    hour: 'hour',
    minute: 'minute',
    second: 'second'
  };
  pacificFormat.formatToParts(new Date(instantMs)).forEach((part) => {
    const key = keys[part.type];
    if (key) {
      fields[key] = parseInt(part.value, 10);
    }
  });
  fields.month -= 1;
  fields.hour = fields.hour % 24;
  return fields;
}

/** Offset (ms) to add to a UTC instant to get Pacific wall-clock time, at the given instant. */
function getPacificOffsetMs(instantMs: number): number {
  const wholeSecondMs = Math.floor(instantMs / 1000) * 1000;
  const f = getPacificFields(wholeSecondMs);
  return Date.UTC(f.year, f.month, f.day, f.hour, f.minute, f.second) - wholeSecondMs;
}

function toValidDate(value: Date | string | number | null | undefined): Date | null {
  if (value === null || value === undefined || value === '') {
    return null;
  }
  const date = value instanceof Date ? value : new Date(value);
  return isNaN(date.getTime()) ? null : date;
}

/**
 * Converts an instant (Date, or ISO string from the API) to a Date whose local fields show the Pacific Time.
 * Returns the input unchanged if it is null/undefined/invalid.
 */
export function toPacificWallClock(value: Date | string): Date;
export function toPacificWallClock(value: null | undefined): null | undefined;
export function toPacificWallClock(value: Date | string | null | undefined): Date | null | undefined {
  const date = toValidDate(value);
  if (!date) {
    return value as null | undefined;
  }
  const f = getPacificFields(date.getTime());
  return new Date(f.year, f.month, f.day, f.hour, f.minute, f.second, ((date.getTime() % 1000) + 1000) % 1000);
}

/**
 * Converts a Date whose local fields represent Pacific Time (e.g. a date picker value plus a selected time)
 * to the real instant. Returns the input unchanged if it is null/undefined/invalid.
 */
export function fromPacificWallClock(value: Date | string): Date;
export function fromPacificWallClock(value: null | undefined): null | undefined;
export function fromPacificWallClock(value: Date | string | null | undefined): Date | null | undefined {
  const date = toValidDate(value);
  if (!date) {
    return value as null | undefined;
  }
  const wallClockAsUtcMs = Date.UTC(
    date.getFullYear(),
    date.getMonth(),
    date.getDate(),
    date.getHours(),
    date.getMinutes(),
    date.getSeconds(),
    date.getMilliseconds()
  );
  let instantMs = wallClockAsUtcMs - getPacificOffsetMs(wallClockAsUtcMs);
  // The offset must be evaluated at the resulting instant, which differs near a DST change.
  const adjustedOffsetMs = getPacificOffsetMs(instantMs);
  instantMs = wallClockAsUtcMs - adjustedOffsetMs;
  return new Date(instantMs);
}
