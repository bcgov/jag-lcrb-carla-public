import { InjectionToken } from '@angular/core';
import type { Signal } from '@angular/core';
import type { ComplianceRecord, EnforcementAction } from '@domain/models/compliance';
import type { injectEnforcementActions, injectOpenEnforcementCount, submitResponse } from './enforcement-store';
import { MockComplianceRepository } from './compliance.repository.mock';
import type { buildCalendarEvents } from './compliance-calendar';
import type { enforcementCollection } from './enforcement-store';

/** Compliance repository (backend area: 6.9 Compliance and enforcement). */
export interface ComplianceRepository {
  readonly complianceHistory: Signal<ComplianceRecord[]>;
  readonly enforcementActions: Signal<EnforcementAction[]>;
  readonly enforcementViews: typeof injectEnforcementActions;
  readonly openEnforcementCount: typeof injectOpenEnforcementCount;
  readonly submitResponse: typeof submitResponse;
  readonly buildCalendarEvents: typeof buildCalendarEvents;
  readonly enforcementCollection: typeof enforcementCollection;
}

export const COMPLIANCE_REPOSITORY = new InjectionToken<ComplianceRepository>('ComplianceRepository', {
  providedIn: 'root',
  factory: () => new MockComplianceRepository(),
});
