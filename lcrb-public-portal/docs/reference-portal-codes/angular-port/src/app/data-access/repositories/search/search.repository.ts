import { InjectionToken } from '@angular/core';
import type { search } from './search-service';
import { MockSearchRepository } from './search.repository.mock';

/** Search repository (backend area: none (global search)). */
export interface SearchRepository {
  readonly search: typeof search;
}

export const SEARCH_REPOSITORY = new InjectionToken<SearchRepository>('SearchRepository', {
  providedIn: 'root',
  factory: () => new MockSearchRepository(),
});
