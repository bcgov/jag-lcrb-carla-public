import { ApplicationExtension } from "@models/application.model";

export class ApplicationSummary {
  id: string;
  applicationStatus: string;
  establishmentName: string;
  name: string;
  jobNumber: string;
  applicationTypeName: string;
  applicationTypeCategory: string;
  isIndigenousNation: boolean;
  licenceId: string;
  isPaid: boolean;
  isStructuralChange: boolean;
  isApplicationComplete: string;
  portallabel: string;
  lgHasApproved: boolean;
  endorsements: string[];
  isForLicence: boolean;
  dateApplicationSubmitted: Date;
  dateApplicantSentToLG: Date;
  applicationExtension?: ApplicationExtension;
}
