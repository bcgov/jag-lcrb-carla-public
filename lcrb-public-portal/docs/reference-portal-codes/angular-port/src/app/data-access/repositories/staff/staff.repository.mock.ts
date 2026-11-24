import { Injectable } from '@angular/core';
import { loadCases, computeIntakeAnalytics } from './lcrb-analytics';
import { loadPageViews, computeSiteAnalytics, recordPageView } from './site-analytics';
import type { StaffRepository } from './staff.repository';
import { siteAnalyticsCollection } from './site-analytics';

/** Mock StaffRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockStaffRepository implements StaffRepository {
  readonly cases = loadCases;

  readonly intakeAnalytics = computeIntakeAnalytics;

  readonly pageViews = loadPageViews;

  readonly siteAnalytics = computeSiteAnalytics;

  readonly recordPageView = recordPageView;

  readonly siteAnalyticsCollection = siteAnalyticsCollection;
}
