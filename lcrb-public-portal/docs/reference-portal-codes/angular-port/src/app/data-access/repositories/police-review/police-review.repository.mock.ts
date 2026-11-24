import { Injectable } from '@angular/core';
import { listSchedules, createSchedule, setScheduleActive, deleteSchedule, recordScheduleRun, getDueSchedules, filterRecordsForSchedule } from './sep-report-schedule-service';
import type { PoliceReviewRepository } from './police-review.repository';
import { reportScheduleCollection } from './sep-report-schedule-service';

/** Mock PoliceReviewRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockPoliceReviewRepository implements PoliceReviewRepository {
  readonly listSchedules = listSchedules;

  readonly createSchedule = createSchedule;

  readonly setScheduleActive = setScheduleActive;

  readonly deleteSchedule = deleteSchedule;

  readonly recordScheduleRun = recordScheduleRun;

  readonly getDueSchedules = getDueSchedules;

  readonly filterRecordsForSchedule = filterRecordsForSchedule;

  readonly reportScheduleCollection = reportScheduleCollection;
}
