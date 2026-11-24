import { Injectable } from '@angular/core';
import { signal } from '@angular/core';
import { SEP_RECORDS, getSepById, getUpcomingSeps, getActionableSeps, getSepsOnDate } from './mock/seps';
import { distinctCities } from '@domain/special-events/seps';
import { sepGuidedConfig } from './mock/sep-flow';
import { injectSepStore } from './sep-store-provider';
import { setAutoApprovalEnabled } from './sep-auto-approval';
import { injectAutoApprovalPolicy } from './special-events.signals';
import type { SepRecord } from '@domain/models/special-events';
import type { FlowConfig } from '@domain/forms/flow-config';
import type { SpecialEventsRepository } from './special-events.repository';

/** Mock SpecialEventsRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockSpecialEventsRepository implements SpecialEventsRepository {
  readonly records = signal(SEP_RECORDS).asReadonly();

  record(id: string): SepRecord | undefined {
    this.records();
    return getSepById(id);
  }

  readonly upcoming = getUpcomingSeps;

  readonly actionable = getActionableSeps;

  readonly onDate = getSepsOnDate;

  readonly distinctCities = distinctCities;

  readonly guidedConfig = signal(sepGuidedConfig).asReadonly();

  readonly store = injectSepStore;

  readonly autoApprovalPolicy = injectAutoApprovalPolicy;

  readonly setAutoApprovalEnabled = setAutoApprovalEnabled;
}
