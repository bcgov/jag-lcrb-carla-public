import { Injectable } from '@angular/core';
import { signal } from '@angular/core';
import { LG_APPLICATIONS, LG_REVIEWER, getLgApplication } from './mock/lg-applications';
import { grandfatheredReferrals } from './mock/lg-commenting';
import { injectLgStore } from './lg-store-provider';
import { injectLgCommenting } from './lg-commenting-provider';
import { listSchedules, createSchedule, setScheduleActive, deleteSchedule, recordScheduleRun, getDueSchedules, filterApplicationsForSchedule } from './lg-report-schedule-service';
import type { LgApplication, LgContact } from '@domain/models/lg-review';
import type { LgReviewRepository } from './lg-review.repository';
import { lgReportScheduleCollection } from './lg-report-schedule-service';

/** Mock LgReviewRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockLgReviewRepository implements LgReviewRepository {
  readonly applications = signal(LG_APPLICATIONS).asReadonly();

  application(jobNumber: string): LgApplication | undefined {
    this.applications();
    return getLgApplication(jobNumber);
  }

  readonly reviewer = signal(LG_REVIEWER).asReadonly();

  readonly grandfatheredReferrals = grandfatheredReferrals;

  readonly store = injectLgStore;

  readonly commenting = injectLgCommenting;

  readonly listSchedules = listSchedules;

  readonly createSchedule = createSchedule;

  readonly setScheduleActive = setScheduleActive;

  readonly deleteSchedule = deleteSchedule;

  readonly recordScheduleRun = recordScheduleRun;

  readonly getDueSchedules = getDueSchedules;

  readonly filterApplicationsForSchedule = filterApplicationsForSchedule;

  readonly reportScheduleCollection = lgReportScheduleCollection;
}
