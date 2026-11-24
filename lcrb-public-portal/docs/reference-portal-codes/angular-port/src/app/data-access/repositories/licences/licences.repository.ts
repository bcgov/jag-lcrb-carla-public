import { InjectionToken } from '@angular/core';
import type { Signal } from '@angular/core';
import type { Licence, ScheduledInspection } from '@domain/models/licences';
import type { ComplianceRecord } from '@domain/models/compliance';
import type { computeLicenceCap } from './licence-cap-engine';
import { MockLicencesRepository } from './licences.repository.mock';

/** Licences repository (backend area: 6.2 Licences and renewals). */
export interface LicencesRepository {
  readonly licences: Signal<Licence[]>;
  readonly operatedLicences: Signal<Licence[]>;
  readonly affiliateLicences: Signal<Licence[]>;
  licence(id: string): Licence | undefined;
  scheduledInspections(licenceId: string): ScheduledInspection[];
  pastInspections(licenceId: string): ComplianceRecord[];
  nextInspection(licenceId: string): ScheduledInspection | undefined;
  readonly licenceCap: typeof computeLicenceCap;
}

export const LICENCES_REPOSITORY = new InjectionToken<LicencesRepository>('LicencesRepository', {
  providedIn: 'root',
  factory: () => new MockLicencesRepository(),
});
