import { Injectable } from '@angular/core';
import { searchAssociableLicences, getAssociableLicence, sampleExistingRelationships, computeFees } from './mock/tied-house';
import type { TiedHouseRepository } from './tied-house.repository';

/** Mock TiedHouseRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockTiedHouseRepository implements TiedHouseRepository {
  readonly searchAssociableLicences = searchAssociableLicences;

  readonly associableLicence = getAssociableLicence;

  readonly sampleExistingRelationships = sampleExistingRelationships;

  readonly computeFees = computeFees;
}
