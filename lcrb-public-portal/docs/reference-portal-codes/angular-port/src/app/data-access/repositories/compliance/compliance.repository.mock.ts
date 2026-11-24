import { Injectable } from '@angular/core';
import { signal } from '@angular/core';
import { COMPLIANCE_HISTORY, ENFORCEMENT_ACTIONS } from './mock/enforcement';
import { injectEnforcementActions, injectOpenEnforcementCount, submitResponse } from './enforcement-store';
import type { ComplianceRecord, EnforcementAction } from '@domain/models/compliance';
import type { ComplianceRepository } from './compliance.repository';
import { buildCalendarEvents } from './compliance-calendar';
import { enforcementCollection } from './enforcement-store';

/** Mock ComplianceRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockComplianceRepository implements ComplianceRepository {
  readonly complianceHistory = signal(COMPLIANCE_HISTORY).asReadonly();

  readonly enforcementActions = signal(ENFORCEMENT_ACTIONS).asReadonly();

  readonly enforcementViews = injectEnforcementActions;

  readonly openEnforcementCount = injectOpenEnforcementCount;

  readonly submitResponse = submitResponse;

  readonly buildCalendarEvents = buildCalendarEvents;

  readonly enforcementCollection = enforcementCollection;
}
