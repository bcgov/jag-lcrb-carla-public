import { License } from "./license.model";
import { ApplicationType } from "./application-type.model";
import { LicenceEvent } from "./licence-event.model";
import { TermsAndConditions } from "./terms-and-conditions.model";
import { ServiceArea } from "./service-area.model";
import { Subscription } from "rxjs";
import { OffsiteStorage } from "./offsite-storage.model";

export class ApplicationLicenseSummary {

  establishmentId: string;
  establishmentName: string;
  establishmentAddressStreet: string;
  establishmentAddressCity: string;
  establishmentAddressPostalCode: string;
  establishmentPhoneNumber: string;
  establishmentEmail: string;
  establishmentIsOpen: boolean;

  status: string;
  licenseId: string;
  applicationId: string;
  licenceTypeName: string;
  applicationType: ApplicationType;
  applicationTypeName: string;
  applicationTypeCategory: string;
  licenseNumber: string;
  // subcategory is generally blank, except for wine stores
  // using the same mispelled naming convention for consistency
  licenseSubCategory: string;
  name: string;
  jobNumber: string;
  isPaid: boolean;
  paymentreceiveddate: Date;
  createdon: Date;
  modifiedon: Date;
  applicationFormFileUrl: string;
  fileName: string;
  assignedLicense: License;
  expiryDate: Date;
  allowedActions: ApplicationType[];
  storeInspected: boolean;

  actionApplications: LicenceActionApplication[];
  events: LicenceEvent[];
  eventsBusy: Subscription;
  transferRequested: boolean;
  dormant: boolean;
  suspended: boolean;
  operated: boolean;
  checklistConclusivelyDeem: boolean;
  tpoRequested: boolean; // indicates a Third Party Application in Progress
  missingFirstYearLicenceFee: boolean;

  thirdPartyOperatorAccountName: string;
  currentOwner: string;
  isOperated: boolean; // only used on the client side
  hasPaidForRenewalApplication: boolean; // client side use only

  licenceTypeCategory: string;
  representativeFullName: string;
  representativePhoneNumber: string;
  representativeEmail: string;
  representativeCanSubmitPermanentChangeApplications: boolean;
  representativeCanSignTemporaryChangeApplications: boolean;
  representativeCanObtainLicenceInformation: boolean;
  representativeCanSignGroceryStoreProofOfSale: boolean;
  representativeCanAttendEducationSessions: boolean;
  representativeCanAttendComplianceMeetings: boolean;
  representativeCanRepresentAtHearings: boolean;

  termsAndConditions: TermsAndConditions[];
  termsAndConditionsBusy: Subscription;

  headerRowSpan: number;
  serviceAreas: ServiceArea[];
  offsiteStorageLocations: OffsiteStorage[];
}

export interface LicenceActionApplication {
  applicationId: string;
  applicationTypeName: string;
  isPaid: boolean;
  isStructuralChange: boolean;
  applicationStatus: string;
}
