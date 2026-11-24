import { Injectable } from '@angular/core';
import { search } from './search-service';
import type { SearchRepository } from './search.repository';

/** Mock SearchRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockSearchRepository implements SearchRepository {
  readonly search = search;
}
