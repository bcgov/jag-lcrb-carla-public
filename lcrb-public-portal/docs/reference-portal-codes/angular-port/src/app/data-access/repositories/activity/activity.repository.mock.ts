import { Injectable } from '@angular/core';
import { recordAudit, listAudit, filterAudit } from './audit-service';
import { pushCase, retrySync } from './dynamics-service';
import { injectAuditTrail, injectSubjectAudit, injectSyncRecords } from './activity.signals';
import type { ActivityRepository } from './activity.repository';

/** Mock ActivityRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockActivityRepository implements ActivityRepository {
  readonly auditTrail = injectAuditTrail;

  readonly subjectAudit = injectSubjectAudit;

  readonly syncRecords = injectSyncRecords;

  readonly recordAudit = recordAudit;

  readonly listAudit = listAudit;

  readonly filterAudit = filterAudit;

  readonly pushCase = pushCase;

  readonly retrySync = retrySync;
}
