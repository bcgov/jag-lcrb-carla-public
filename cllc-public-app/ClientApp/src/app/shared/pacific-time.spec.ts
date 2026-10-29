import { fromPacificWallClock, toPacificWallClock } from './pacific-time';

// BC is on permanent UTC-7 from 2026-11-01; earlier winters were UTC-8.
describe('pacific-time', () => {
  const fields = (d: Date) => [d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes()];

  describe('fromPacificWallClock', () => {
    it('uses UTC-7 for winter dates after the switch to permanent daylight time', () => {
      expect(fromPacificWallClock(new Date(2026, 10, 9, 9, 0)).toISOString()).toBe('2026-11-09T16:00:00.000Z');
      expect(fromPacificWallClock(new Date(2027, 0, 15, 9, 0)).toISOString()).toBe('2027-01-15T16:00:00.000Z');
    });

    it('uses UTC-7 for summer dates', () => {
      expect(fromPacificWallClock(new Date(2026, 6, 9, 9, 0)).toISOString()).toBe('2026-07-09T16:00:00.000Z');
      expect(fromPacificWallClock(new Date(2027, 6, 9, 9, 0)).toISOString()).toBe('2027-07-09T16:00:00.000Z');
    });

    it('keeps UTC-8 for historical standard time dates', () => {
      expect(fromPacificWallClock(new Date(2026, 0, 15, 9, 0)).toISOString()).toBe('2026-01-15T17:00:00.000Z');
    });

    it('handles the 2026-11-01 transition', () => {
      expect(fromPacificWallClock(new Date(2026, 9, 31, 23, 0)).toISOString()).toBe('2026-11-01T06:00:00.000Z');
      expect(fromPacificWallClock(new Date(2026, 10, 1, 1, 30)).toISOString()).toBe('2026-11-01T08:30:00.000Z');
      expect(fromPacificWallClock(new Date(2026, 10, 1, 3, 0)).toISOString()).toBe('2026-11-01T10:00:00.000Z');
    });

    it('handles times after midnight', () => {
      expect(fromPacificWallClock(new Date(2026, 10, 10, 1, 30)).toISOString()).toBe('2026-11-10T08:30:00.000Z');
    });

    it('passes through null and undefined', () => {
      expect(fromPacificWallClock(null)).toBeNull();
      expect(fromPacificWallClock(undefined)).toBeUndefined();
    });
  });

  describe('toPacificWallClock', () => {
    it('shows UTC-7 in local fields for winter dates after the switch', () => {
      expect(fields(toPacificWallClock('2026-11-09T16:00:00.000Z'))).toEqual([2026, 10, 9, 9, 0]);
      expect(fields(toPacificWallClock('2027-01-15T16:00:00.000Z'))).toEqual([2027, 0, 15, 9, 0]);
    });

    it('shows UTC-7 in local fields for summer dates', () => {
      expect(fields(toPacificWallClock('2026-07-09T16:00:00.000Z'))).toEqual([2026, 6, 9, 9, 0]);
    });

    it('shows UTC-8 in local fields for historical standard time dates', () => {
      expect(fields(toPacificWallClock('2026-01-15T17:00:00.000Z'))).toEqual([2026, 0, 15, 9, 0]);
    });

    it('uses the previous Pacific day for early-UTC instants', () => {
      expect(fields(toPacificWallClock('2026-11-10T03:00:00.000Z'))).toEqual([2026, 10, 9, 20, 0]);
    });

    it('passes through null and undefined', () => {
      expect(toPacificWallClock(null)).toBeNull();
      expect(toPacificWallClock(undefined)).toBeUndefined();
    });
  });

  it('round trips regardless of the time of day', () => {
    [new Date(2026, 0, 15), new Date(2026, 9, 31), new Date(2026, 10, 1), new Date(2026, 10, 9), new Date(2027, 6, 9)].forEach(
      (day) => {
        for (let hour = 0; hour < 24; hour++) {
          const wallClock = new Date(day.getFullYear(), day.getMonth(), day.getDate(), hour, 30);
          const roundTripped = toPacificWallClock(fromPacificWallClock(wallClock));
          expect(roundTripped.getTime()).toBe(wallClock.getTime());
        }
      }
    );
  });
});
