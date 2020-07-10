import { License } from './license.model';
import { ApplicationType } from './application-type.model';
import { LicenceEvent } from './licence-event.model';
import { TermsAndConditions } from './terms-and-conditions.model';
import { ServiceArea } from './service-area.model';

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
  transferRequested: boolean;
  dormant: boolean;
  suspended: boolean;
  operated: boolean;
  tpoRequested: boolean; // indicates a Third Party Application in Progress

  thirdPartyOperatorAccountName: string;
  isOperated: boolean; // only used on the client side
  isDeemed: boolean; // only used on the client side

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

  headerRowSpan: number;
  serviceAreas: ServiceArea[];
}

export interface LicenceActionApplication {
  applicationId: string;
  applicationTypeName: string;
  isPaid: boolean;
  applicationStatus: string;
}
