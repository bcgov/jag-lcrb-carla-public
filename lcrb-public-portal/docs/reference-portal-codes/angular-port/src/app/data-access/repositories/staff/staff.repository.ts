import { InjectionToken } from '@angular/core';
import type { loadCases, computeIntakeAnalytics } from './lcrb-analytics';
import type { loadPageViews, computeSiteAnalytics, recordPageView } from './site-analytics';
import { MockStaffRepository } from './staff.repository.mock';
import type { siteAnalyticsCollection } from './site-analytics';

/** Staff repository (backend area: 6.15 LCRB staff surface). */
export interface StaffRepository {
  readonly cases: typeof loadCases;
  readonly intakeAnalytics: typeof computeIntakeAnalytics;
  readonly pageViews: typeof loadPageViews;
  readonly siteAnalytics: typeof computeSiteAnalytics;
  readonly recordPageView: typeof recordPageView;
  readonly siteAnalyticsCollection: typeof siteAnalyticsCollection;
}

export const STAFF_REPOSITORY = new InjectionToken<StaffRepository>('StaffRepository', {
  providedIn: 'root',
  factory: () => new MockStaffRepository(),
});
