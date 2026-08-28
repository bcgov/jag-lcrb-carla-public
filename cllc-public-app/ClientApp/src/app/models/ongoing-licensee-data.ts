import { ApplicationLicenseSummary } from './application-license-summary.model';
import { Application } from './application.model';
import { LegalEntity } from './legal-entity.model';
import { LicenseeChangeLog } from './licensee-change-log.model';

export class OngoingLicenseeData {
  application: Application;
  changeLogs: LicenseeChangeLog[];
  nonTerminatedApplications: number;
  currentHierarchy: LegalEntity;
  licenses: ApplicationLicenseSummary[];
  treeRoot: LicenseeChangeLog;
}
