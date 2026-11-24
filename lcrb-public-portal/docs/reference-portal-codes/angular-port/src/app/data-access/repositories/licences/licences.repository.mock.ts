import { Injectable } from '@angular/core';
import { signal } from '@angular/core';
import { MOCK_LICENCES, MOCK_OPERATED_LICENCES, MOCK_AFFILIATE_LICENCES, getLicenceById } from './mock/licences';
import { getScheduledInspectionsForLicence, getPastInspectionsForLicence, getNextInspectionForLicence } from './mock/inspections';
import { computeLicenceCap } from './licence-cap-engine';
import type { Licence, ScheduledInspection } from '@domain/models/licences';
import type { ComplianceRecord } from '@domain/models/compliance';
import type { LicencesRepository } from './licences.repository';

/** Mock LicencesRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockLicencesRepository implements LicencesRepository {
  readonly licences = signal(MOCK_LICENCES).asReadonly();

  readonly operatedLicences = signal(MOCK_OPERATED_LICENCES).asReadonly();

  readonly affiliateLicences = signal(MOCK_AFFILIATE_LICENCES).asReadonly();

  licence(id: string): Licence | undefined {
    this.licences();
    return getLicenceById(id);
  }

  scheduledInspections(licenceId: string): ScheduledInspection[] {
    return getScheduledInspectionsForLicence(licenceId);
  }

  pastInspections(licenceId: string): ComplianceRecord[] {
    return getPastInspectionsForLicence(licenceId);
  }

  nextInspection(licenceId: string): ScheduledInspection | undefined {
    return getNextInspectionForLicence(licenceId);
  }

  readonly licenceCap = computeLicenceCap;
}
