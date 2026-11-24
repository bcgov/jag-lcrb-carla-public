import { InjectionToken } from '@angular/core';
import type { Signal } from '@angular/core';
import type { SepRecord } from '@domain/models/special-events';
import type { FlowConfig } from '@domain/forms/flow-config';
import type { getUpcomingSeps, getActionableSeps, getSepsOnDate } from './mock/seps';
import type { distinctCities } from '@domain/special-events/seps';
import type { injectSepStore } from './sep-store-provider';
import type { setAutoApprovalEnabled } from './sep-auto-approval';
import type { injectAutoApprovalPolicy } from './special-events.signals';
import { MockSpecialEventsRepository } from './special-events.repository.mock';

/** SpecialEvents repository (backend area: 6.6 Special Event Permits). */
export interface SpecialEventsRepository {
  readonly records: Signal<SepRecord[]>;
  record(id: string): SepRecord | undefined;
  readonly upcoming: typeof getUpcomingSeps;
  readonly actionable: typeof getActionableSeps;
  readonly onDate: typeof getSepsOnDate;
  readonly distinctCities: typeof distinctCities;
  /** The SEP application form metadata (metadata-driven form). */
  readonly guidedConfig: Signal<FlowConfig>;
  /** The SEP store (stateful). */
  readonly store: typeof injectSepStore;
  readonly autoApprovalPolicy: typeof injectAutoApprovalPolicy;
  readonly setAutoApprovalEnabled: typeof setAutoApprovalEnabled;
}

export const SPECIAL_EVENTS_REPOSITORY = new InjectionToken<SpecialEventsRepository>('SpecialEventsRepository', {
  providedIn: 'root',
  factory: () => new MockSpecialEventsRepository(),
});
