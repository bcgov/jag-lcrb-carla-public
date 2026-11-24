import { InjectionToken } from '@angular/core';
import type { createSampleBuild } from './mock/legal-entity';
import { MockLegalEntityRepository } from './legal-entity.repository.mock';

/** LegalEntity repository (backend area: 6.3 Legal entity). */
export interface LegalEntityRepository {
  readonly sampleBuild: typeof createSampleBuild;
  /** The entity-directory API (`/api/entity-directory`). */
  fetchDirectory(query: string): Promise<Response>;
}

export const LEGAL_ENTITY_REPOSITORY = new InjectionToken<LegalEntityRepository>('LegalEntityRepository', {
  providedIn: 'root',
  factory: () => new MockLegalEntityRepository(),
});
