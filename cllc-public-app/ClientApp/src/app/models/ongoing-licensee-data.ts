import { Application } from "./application.model";
import { LicenseeChangeLog } from "./licensee-change-log.model";
import { LegalEntity } from "./legal-entity.model";
import { ApplicationLicenseSummary } from "./application-license-summary.model";

export class OngoingLicenseeData {
  application: Application;
  changeLogs: LicenseeChangeLog[];
  nonTerminatedApplications: number;
  currentHierarchy: LegalEntity;
  licenses: ApplicationLicenseSummary[];
  treeRoot: LicenseeChangeLog;
}
