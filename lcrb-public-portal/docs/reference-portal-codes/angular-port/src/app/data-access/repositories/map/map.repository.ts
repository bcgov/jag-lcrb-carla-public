import { InjectionToken } from '@angular/core';
import type { Signal } from '@angular/core';
import type { License } from '@domain/models/map';
import type { allLicenses, inspectors, regions } from './mock/license-data';
import { MockMapRepository } from './map.repository.mock';

/** Map repository (backend area: 6.14 Map, geocoding and federal reporting). */
export interface MapRepository {
  readonly licenses: Signal<License[]>;
  readonly inspectors: Signal<typeof inspectors>;
  readonly regions: Signal<typeof regions>;
}

export const MAP_REPOSITORY = new InjectionToken<MapRepository>('MapRepository', {
  providedIn: 'root',
  factory: () => new MockMapRepository(),
});
