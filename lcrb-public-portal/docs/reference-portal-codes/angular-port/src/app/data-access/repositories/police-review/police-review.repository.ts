import { InjectionToken } from '@angular/core';
import type { listSchedules, createSchedule, setScheduleActive, deleteSchedule, recordScheduleRun, getDueSchedules, filterRecordsForSchedule } from './sep-report-schedule-service';
import { MockPoliceReviewRepository } from './police-review.repository.mock';
import type { reportScheduleCollection } from './sep-report-schedule-service';

/** PoliceReview repository (backend area: 6.7 Police review). */
export interface PoliceReviewRepository {
  readonly listSchedules: typeof listSchedules;
  readonly createSchedule: typeof createSchedule;
  readonly setScheduleActive: typeof setScheduleActive;
  readonly deleteSchedule: typeof deleteSchedule;
  readonly recordScheduleRun: typeof recordScheduleRun;
  readonly getDueSchedules: typeof getDueSchedules;
  readonly filterRecordsForSchedule: typeof filterRecordsForSchedule;
  readonly reportScheduleCollection: typeof reportScheduleCollection;
}

export const POLICE_REVIEW_REPOSITORY = new InjectionToken<PoliceReviewRepository>('PoliceReviewRepository', {
  providedIn: 'root',
  factory: () => new MockPoliceReviewRepository(),
});
