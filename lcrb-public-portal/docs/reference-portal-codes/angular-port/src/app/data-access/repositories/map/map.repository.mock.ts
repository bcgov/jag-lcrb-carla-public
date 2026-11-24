import { Injectable } from '@angular/core';
import { signal } from '@angular/core';
import { allLicenses, inspectors, regions } from './mock/license-data';
import type { License } from '@domain/models/map';
import type { MapRepository } from './map.repository';

/** Mock MapRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockMapRepository implements MapRepository {
  readonly licenses = signal(allLicenses).asReadonly();

  readonly inspectors = signal(inspectors).asReadonly();

  readonly regions = signal(regions).asReadonly();
}
