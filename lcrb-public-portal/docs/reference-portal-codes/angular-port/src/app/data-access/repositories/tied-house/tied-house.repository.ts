import { InjectionToken } from '@angular/core';
import type { searchAssociableLicences, getAssociableLicence, sampleExistingRelationships, computeFees } from './mock/tied-house';
import { MockTiedHouseRepository } from './tied-house.repository.mock';

/** TiedHouse repository (backend area: 6.4 Tied house). */
export interface TiedHouseRepository {
  readonly searchAssociableLicences: typeof searchAssociableLicences;
  readonly associableLicence: typeof getAssociableLicence;
  readonly sampleExistingRelationships: typeof sampleExistingRelationships;
  readonly computeFees: typeof computeFees;
}

export const TIED_HOUSE_REPOSITORY = new InjectionToken<TiedHouseRepository>('TiedHouseRepository', {
  providedIn: 'root',
  factory: () => new MockTiedHouseRepository(),
});
