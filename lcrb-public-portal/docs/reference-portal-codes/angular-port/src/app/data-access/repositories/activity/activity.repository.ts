import { InjectionToken } from '@angular/core';
import type { recordAudit, listAudit, filterAudit } from './audit-service';
import type { pushCase, retrySync } from './dynamics-service';
import type { injectAuditTrail, injectSubjectAudit, injectSyncRecords } from './activity.signals';
import { MockActivityRepository } from './activity.repository.mock';

/** Activity repository (backend area: none (audit trail and Dynamics sync log)). */
export interface ActivityRepository {
  readonly auditTrail: typeof injectAuditTrail;
  readonly subjectAudit: typeof injectSubjectAudit;
  readonly syncRecords: typeof injectSyncRecords;
  readonly recordAudit: typeof recordAudit;
  readonly listAudit: typeof listAudit;
  readonly filterAudit: typeof filterAudit;
  readonly pushCase: typeof pushCase;
  readonly retrySync: typeof retrySync;
}

export const ACTIVITY_REPOSITORY = new InjectionToken<ActivityRepository>('ActivityRepository', {
  providedIn: 'root',
  factory: () => new MockActivityRepository(),
});
