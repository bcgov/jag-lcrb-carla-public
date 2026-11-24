import { Injectable } from '@angular/core';
import { createSampleBuild } from './mock/legal-entity';
import type { LegalEntityRepository } from './legal-entity.repository';

/** Mock LegalEntityRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockLegalEntityRepository implements LegalEntityRepository {
  readonly sampleBuild = createSampleBuild;

  fetchDirectory(query: string): Promise<Response> {
    return fetch(`/api/entity-directory?${query}`);
  }
}
