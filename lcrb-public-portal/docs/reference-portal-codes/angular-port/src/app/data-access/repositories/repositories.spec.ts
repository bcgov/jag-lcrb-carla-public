/** The mock repositories return exactly what the underlying seed functions and hooks return for the same storage state */
import { TestBed } from '@angular/core/testing';
import { describe, expect, it } from 'vitest';
import { ACCOUNT_REPOSITORY } from './account/account.repository';
import { ACTIVITY_REPOSITORY } from './activity/activity.repository';
import { AI_REPOSITORY } from './ai/ai.repository';
import { APPLICATIONS_REPOSITORY } from './applications/applications.repository';
import { COMPLIANCE_REPOSITORY } from './compliance/compliance.repository';
import { DOCUMENTS_REPOSITORY } from './documents/documents.repository';
import { LEGAL_ENTITY_REPOSITORY } from './legal-entity/legal-entity.repository';
import { LG_REVIEW_REPOSITORY } from './lg-review/lg-review.repository';
import { LICENCES_REPOSITORY } from './licences/licences.repository';
import { MAP_REPOSITORY } from './map/map.repository';
import { MESSAGING_REPOSITORY } from './messaging/messaging.repository';
import { NOTIFICATIONS_REPOSITORY } from './notifications/notifications.repository';
import { PAYMENTS_REPOSITORY } from './payments/payments.repository';
import { POLICE_REVIEW_REPOSITORY } from './police-review/police-review.repository';
import { SCREENING_REPOSITORY } from './screening/screening.repository';
import { SEARCH_REPOSITORY } from './search/search.repository';
import { SPECIAL_EVENTS_REPOSITORY } from './special-events/special-events.repository';
import { STAFF_REPOSITORY } from './staff/staff.repository';
import { TIED_HOUSE_REPOSITORY } from './tied-house/tied-house.repository';
import { getAccountConfig, defaultNotificationState } from './account/mock/account-settings';
import { getBusinessProfile } from './account/mock/business-profile';
import { APPLICATION_CATALOG, getCatalogEntry, getEntriesByType } from './applications/mock/applications';
import { MOST_USED_TITLES, searchCatalog } from './applications/mock/application-search';
import { MY_APPLICATIONS, referralsFor } from './applications/mock/my-applications';
import { getSubmittedApplication } from './applications/mock/submitted-applications';
import { injectDrafts, getDraft, saveDraft, deleteDraft } from './applications/drafts';
import { injectSubmission } from './applications/submissions';
import { submitApplication } from './applications/submit-application';
import { cancelApplication } from './applications/cancellation-service';
import { injectCancellation, injectCancellations } from './applications/applications.signals';
import { COMPLIANCE_HISTORY, ENFORCEMENT_ACTIONS } from './compliance/mock/enforcement';
import { injectEnforcementActions, injectOpenEnforcementCount, submitResponse } from './compliance/enforcement-store';
import { MOCK_LICENCES, MOCK_OPERATED_LICENCES, MOCK_AFFILIATE_LICENCES, getLicenceById } from './licences/mock/licences';
import { getScheduledInspectionsForLicence, getPastInspectionsForLicence, getNextInspectionForLicence } from './licences/mock/inspections';
import { computeLicenceCap } from './licences/licence-cap-engine';
import { LG_APPLICATIONS, LG_REVIEWER, getLgApplication } from './lg-review/mock/lg-applications';
import { injectLgStore } from './lg-review/lg-store-provider';
import { injectLgCommenting } from './lg-review/lg-commenting-provider';
import { allLicenses, inspectors, regions } from './map/mock/license-data';
import { ANNOUNCEMENTS, latestAnnouncementId, injectUnreadAnnouncements } from './notifications/mock/announcements';
import { NOTICES } from './notifications/mock/notices';
import { injectInboxFeed } from './notifications/inbox';
import { SEP_RECORDS, getSepById, getUpcomingSeps } from './special-events/mock/seps';
import { sepGuidedConfig } from './special-events/mock/sep-flow';
import { injectSepStore } from './special-events/sep-store-provider';
import { injectAutoApprovalPolicy } from './special-events/special-events.signals';
import { search } from './search/search-service';
import { recordPageView } from './staff/site-analytics';
import { computeFitAndProper } from './screening/fit-and-proper-engine';
import { searchAssociableLicences, computeFees } from './tied-house/mock/tied-house';
import { createSampleBuild } from './legal-entity/mock/legal-entity';
import { requestAiJson, hashKey } from './ai/ai-client';
import { gatherPortalSnapshot } from './ai/gather-snapshot';
import { injectReceipts } from './payments/payments.signals';
import { injectDocuments } from './documents/documents.signals';
import { injectThreads } from './messaging/messaging.signals';
import { injectAuditTrail } from './activity/activity.signals';
import { listSchedules } from './police-review/sep-report-schedule-service';
import { computeFeasibility, computeReadiness } from './applications/establishment-feasibility';
import { scoreEntry } from './applications/mock/application-search';
import { injectAiAutoRetry } from './ai/use-ai-auto-retry';
import { siteAnalyticsCollection } from './staff/site-analytics';
import { lgReportScheduleCollection } from './lg-review/lg-report-schedule-service';
import { buildCalendarEvents } from './compliance/compliance-calendar';
import { enforcementCollection } from './compliance/enforcement-store';
import { reportScheduleCollection } from './police-review/sep-report-schedule-service';

function repos() {
  return <T>(token: Parameters<typeof TestBed.inject<T>>[0]) => TestBed.inject(token);
}

describe('mock repositories', () => {
  it('every token defaults to its mock, with no provider registered', () => {
    const tokens = [ACCOUNT_REPOSITORY, ACTIVITY_REPOSITORY, AI_REPOSITORY, APPLICATIONS_REPOSITORY, COMPLIANCE_REPOSITORY, DOCUMENTS_REPOSITORY, LEGAL_ENTITY_REPOSITORY, LG_REVIEW_REPOSITORY, LICENCES_REPOSITORY, MAP_REPOSITORY, MESSAGING_REPOSITORY, NOTIFICATIONS_REPOSITORY, PAYMENTS_REPOSITORY, POLICE_REVIEW_REPOSITORY, SCREENING_REPOSITORY, SEARCH_REPOSITORY, SPECIAL_EVENTS_REPOSITORY, STAFF_REPOSITORY, TIED_HOUSE_REPOSITORY];
    expect(tokens).toHaveLength(19);
    for (const token of tokens) expect((TestBed.inject(token as never) as object).constructor.name).toMatch(/Mock\w+Repository$/);
  });

  it('account', () => {
    const repo = repos()(ACCOUNT_REPOSITORY);
    expect(repo.businessProfile('Coastal Taphouse Ltd.')).toEqual(getBusinessProfile('Coastal Taphouse Ltd.'));
    expect(repo.businessProfile()).toEqual(getBusinessProfile());
    expect(repo.accountConfig('licensee')).toEqual(getAccountConfig('licensee'));
    expect(repo.defaultNotificationState('police')).toEqual(defaultNotificationState('police'));
  });

  it('licences', () => {
    const repo = repos()(LICENCES_REPOSITORY);
    expect(repo.licences()).toBe(MOCK_LICENCES);
    expect(repo.operatedLicences()).toBe(MOCK_OPERATED_LICENCES);
    expect(repo.affiliateLicences()).toBe(MOCK_AFFILIATE_LICENCES);
    const id = MOCK_LICENCES[0].id;
    expect(repo.licence(id)).toBe(getLicenceById(id));
    expect(repo.licence('no-such-licence')).toBeUndefined();
    expect(repo.scheduledInspections(id)).toEqual(getScheduledInspectionsForLicence(id));
    expect(repo.pastInspections(id)).toEqual(getPastInspectionsForLicence(id));
    expect(repo.nextInspection(id)).toEqual(getNextInspectionForLicence(id));
    expect(repo.licenceCap).toBe(computeLicenceCap);
  });

  it('applications', () => {
    const repo = repos()(APPLICATIONS_REPOSITORY);
    expect(repo.catalogue()).toBe(APPLICATION_CATALOG);
    const slug = APPLICATION_CATALOG[0].slug;
    expect(repo.catalogueEntry(slug)).toBe(getCatalogEntry(slug));
    expect(repo.entriesByType('food-primary')).toEqual(getEntriesByType('food-primary'));
    expect(repo.searchCatalogue).toBe(searchCatalog);
    expect(repo.mostUsedTitles()).toBe(MOST_USED_TITLES);
    expect(repo.myApplications()).toBe(MY_APPLICATIONS);
    expect(repo.referralsFor).toBe(referralsFor);
    expect(repo.submittedApplication('LP-125998')).toBe(getSubmittedApplication('LP-125998'));
    expect([repo.drafts, repo.draft, repo.saveDraft, repo.deleteDraft]).toEqual([injectDrafts, getDraft, saveDraft, deleteDraft]);
    expect([repo.submission, repo.submit, repo.cancel]).toEqual([injectSubmission, submitApplication, cancelApplication]);
    expect([repo.cancellation, repo.cancellations]).toEqual([injectCancellation, injectCancellations]);
  });

  it('compliance', () => {
    const repo = repos()(COMPLIANCE_REPOSITORY);
    expect(repo.complianceHistory()).toBe(COMPLIANCE_HISTORY);
    expect(repo.enforcementActions()).toBe(ENFORCEMENT_ACTIONS);
    expect([repo.enforcementViews, repo.openEnforcementCount, repo.submitResponse]).toEqual([
      injectEnforcementActions,
      injectOpenEnforcementCount,
      submitResponse,
    ]);
  });

  it('lg review', () => {
    const repo = repos()(LG_REVIEW_REPOSITORY);
    expect(repo.applications()).toBe(LG_APPLICATIONS);
    expect(repo.reviewer()).toBe(LG_REVIEWER);
    const job = LG_APPLICATIONS[0].jobNumber;
    expect(repo.application(job)).toBe(getLgApplication(job));
    expect([repo.store, repo.commenting]).toEqual([injectLgStore, injectLgCommenting]);
  });

  it('special events', () => {
    const repo = repos()(SPECIAL_EVENTS_REPOSITORY);
    expect(repo.records()).toBe(SEP_RECORDS);
    expect(repo.record(SEP_RECORDS[0].id)).toBe(getSepById(SEP_RECORDS[0].id));
    expect(repo.upcoming).toBe(getUpcomingSeps);
    expect(repo.guidedConfig()).toBe(sepGuidedConfig);
    expect([repo.store, repo.autoApprovalPolicy]).toEqual([injectSepStore, injectAutoApprovalPolicy]);
  });

  it('notifications', () => {
    const repo = repos()(NOTIFICATIONS_REPOSITORY);
    expect(repo.announcements()).toBe(ANNOUNCEMENTS);
    expect(repo.notices()).toBe(NOTICES);
    expect([repo.latestAnnouncementId, repo.unreadAnnouncements, repo.inboxFeed]).toEqual([
      latestAnnouncementId,
      injectUnreadAnnouncements,
      injectInboxFeed,
    ]);
  });

  it('map', () => {
    const repo = repos()(MAP_REPOSITORY);
    expect(repo.licenses()).toBe(allLicenses);
    expect(repo.inspectors()).toBe(inspectors);
    expect(repo.regions()).toBe(regions);
  });

  it('the remaining modules delegate to the underlying functions and hooks', () => {
    const get = repos();
    expect(get(SEARCH_REPOSITORY).search).toBe(search);
    expect(get(STAFF_REPOSITORY).recordPageView).toBe(recordPageView);
    expect(get(SCREENING_REPOSITORY).fitAndProper).toBe(computeFitAndProper);
    expect(get(TIED_HOUSE_REPOSITORY).searchAssociableLicences).toBe(searchAssociableLicences);
    expect(get(TIED_HOUSE_REPOSITORY).computeFees).toBe(computeFees);
    expect(get(LEGAL_ENTITY_REPOSITORY).sampleBuild).toBe(createSampleBuild);
    expect(get(AI_REPOSITORY).requestJson).toBe(requestAiJson);
    expect(get(AI_REPOSITORY).hashKey).toBe(hashKey);
    expect(get(AI_REPOSITORY).portalSnapshot).toBe(gatherPortalSnapshot);
    expect(get(PAYMENTS_REPOSITORY).receipts).toBe(injectReceipts);
    expect(get(DOCUMENTS_REPOSITORY).documents).toBe(injectDocuments);
    expect(get(MESSAGING_REPOSITORY).threads).toBe(injectThreads);
    expect(get(ACTIVITY_REPOSITORY).auditTrail).toBe(injectAuditTrail);
    expect(get(POLICE_REVIEW_REPOSITORY).listSchedules).toBe(listSchedules);
    // Members added in .0 for the feature classes.
    expect(get(APPLICATIONS_REPOSITORY).computeFeasibility).toBe(computeFeasibility);
    expect(get(APPLICATIONS_REPOSITORY).computeReadiness).toBe(computeReadiness);
    expect(get(APPLICATIONS_REPOSITORY).scoreEntry).toBe(scoreEntry);
    expect(get(AI_REPOSITORY).autoRetry).toBe(injectAiAutoRetry);
    expect(get(STAFF_REPOSITORY).siteAnalyticsCollection).toBe(siteAnalyticsCollection);
    expect(get(LG_REVIEW_REPOSITORY).reportScheduleCollection).toBe(lgReportScheduleCollection);
    expect(get(COMPLIANCE_REPOSITORY).buildCalendarEvents).toBe(buildCalendarEvents);
    expect(get(COMPLIANCE_REPOSITORY).enforcementCollection).toBe(enforcementCollection);
    expect(get(POLICE_REVIEW_REPOSITORY).reportScheduleCollection).toBe(reportScheduleCollection);
  });
});
