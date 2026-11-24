import { InjectionToken } from '@angular/core';
import type { Signal } from '@angular/core';
import type { CatalogEntry, LicenceType, MyApplications } from '@domain/models/applications';
import type { SubmittedApplication } from '@domain/models/applications';
import type { searchCatalog } from './mock/application-search';
import type { referralsFor } from './mock/my-applications';
import type { getDraft, saveDraft, deleteDraft, injectDrafts } from './drafts';
import type { injectSubmission } from './submissions';
import type { submitApplication } from './submit-application';
import type { cancelApplication } from './cancellation-service';
import type { injectCancellation, injectCancellations } from './applications.signals';
import { MockApplicationsRepository } from './applications.repository.mock';
import type { computeFeasibility } from './establishment-feasibility';
import type { computeReadiness } from './establishment-feasibility';
import type { scoreEntry } from './mock/application-search';

/** Applications repository (backend area: 6.1 Licence applications). */
export interface ApplicationsRepository {
  readonly catalogue: Signal<CatalogEntry[]>;
  catalogueEntry(slug: string): CatalogEntry | undefined;
  entriesByType(type: LicenceType): CatalogEntry[];
  readonly searchCatalogue: typeof searchCatalog;
  readonly mostUsedTitles: Signal<string[]>;
  readonly myApplications: Signal<MyApplications>;
  readonly referralsFor: typeof referralsFor;
  submittedApplication(id: string): SubmittedApplication | undefined;
  readonly drafts: typeof injectDrafts;
  readonly draft: typeof getDraft;
  readonly saveDraft: typeof saveDraft;
  readonly deleteDraft: typeof deleteDraft;
  readonly submission: typeof injectSubmission;
  readonly submit: typeof submitApplication;
  readonly cancellation: typeof injectCancellation;
  readonly cancellations: typeof injectCancellations;
  readonly cancel: typeof cancelApplication;
  readonly computeFeasibility: typeof computeFeasibility;
  readonly computeReadiness: typeof computeReadiness;
  readonly scoreEntry: typeof scoreEntry;
}

export const APPLICATIONS_REPOSITORY = new InjectionToken<ApplicationsRepository>('ApplicationsRepository', {
  providedIn: 'root',
  factory: () => new MockApplicationsRepository(),
});
