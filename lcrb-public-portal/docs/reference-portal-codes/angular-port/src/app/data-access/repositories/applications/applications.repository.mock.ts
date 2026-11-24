import { Injectable } from '@angular/core';
import { signal } from '@angular/core';
import { APPLICATION_CATALOG, getCatalogEntry, getEntriesByType } from './mock/applications';
import { searchCatalog, MOST_USED_TITLES } from './mock/application-search';
import { MY_APPLICATIONS, referralsFor } from './mock/my-applications';
import { getSubmittedApplication } from './mock/submitted-applications';
import { getDraft, saveDraft, deleteDraft, injectDrafts } from './drafts';
import { injectSubmission } from './submissions';
import { submitApplication } from './submit-application';
import { cancelApplication } from './cancellation-service';
import { injectCancellation, injectCancellations } from './applications.signals';
import type { CatalogEntry, LicenceType, MyApplications } from '@domain/models/applications';
import type { SubmittedApplication } from '@domain/models/applications';
import type { ApplicationsRepository } from './applications.repository';
import { computeFeasibility } from './establishment-feasibility';
import { computeReadiness } from './establishment-feasibility';
import { scoreEntry } from './mock/application-search';

/** Mock ApplicationsRepository over the prototype's seed data and mock services; same logic and storage keys as the prototype. */
@Injectable()
export class MockApplicationsRepository implements ApplicationsRepository {
  readonly catalogue = signal(APPLICATION_CATALOG).asReadonly();

  catalogueEntry(slug: string): CatalogEntry | undefined {
    this.catalogue();
    return getCatalogEntry(slug);
  }

  entriesByType(type: LicenceType): CatalogEntry[] {
    this.catalogue();
    return getEntriesByType(type);
  }

  readonly searchCatalogue = searchCatalog;

  readonly mostUsedTitles = signal(MOST_USED_TITLES).asReadonly();

  readonly myApplications = signal(MY_APPLICATIONS).asReadonly();

  readonly referralsFor = referralsFor;

  submittedApplication(id: string): SubmittedApplication | undefined {
    return getSubmittedApplication(id);
  }

  readonly drafts = injectDrafts;

  readonly draft = getDraft;

  readonly saveDraft = saveDraft;

  readonly deleteDraft = deleteDraft;

  readonly submission = injectSubmission;

  readonly submit = submitApplication;

  readonly cancellation = injectCancellation;

  readonly cancellations = injectCancellations;

  readonly cancel = cancelApplication;

  readonly computeFeasibility = computeFeasibility;

  readonly computeReadiness = computeReadiness;

  readonly scoreEntry = scoreEntry;
}
