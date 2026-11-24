import { InjectionToken } from '@angular/core';
import type { Signal } from '@angular/core';
import type { LgApplication, LgContact } from '@domain/models/lg-review';
import type { grandfatheredReferrals } from './mock/lg-commenting';
import type { injectLgStore } from './lg-store-provider';
import type { injectLgCommenting } from './lg-commenting-provider';
import type { listSchedules, createSchedule, setScheduleActive, deleteSchedule, recordScheduleRun, getDueSchedules, filterApplicationsForSchedule } from './lg-report-schedule-service';
import { MockLgReviewRepository } from './lg-review.repository.mock';
import type { lgReportScheduleCollection } from './lg-report-schedule-service';

/** LgReview repository (backend area: 6.8 Local and Indigenous government review). */
export interface LgReviewRepository {
  readonly applications: Signal<LgApplication[]>;
  application(jobNumber: string): LgApplication | undefined;
  readonly reviewer: Signal<LgContact>;
  readonly grandfatheredReferrals: typeof grandfatheredReferrals;
  /** The LG review store (stateful). */
  readonly store: typeof injectLgStore;
  /** The LG commenting preference (stateful). */
  readonly commenting: typeof injectLgCommenting;
  readonly listSchedules: typeof listSchedules;
  readonly createSchedule: typeof createSchedule;
  readonly setScheduleActive: typeof setScheduleActive;
  readonly deleteSchedule: typeof deleteSchedule;
  readonly recordScheduleRun: typeof recordScheduleRun;
  readonly getDueSchedules: typeof getDueSchedules;
  readonly filterApplicationsForSchedule: typeof filterApplicationsForSchedule;
  readonly reportScheduleCollection: typeof lgReportScheduleCollection;
}

export const LG_REVIEW_REPOSITORY = new InjectionToken<LgReviewRepository>('LgReviewRepository', {
  providedIn: 'root',
  factory: () => new MockLgReviewRepository(),
});
