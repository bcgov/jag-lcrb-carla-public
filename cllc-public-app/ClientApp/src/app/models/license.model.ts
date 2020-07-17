import { Endorsement } from './endorsement.model';

export class License {
  id: string;
  licenseType: string;
  licenseStatus: string;
  licenseNumber: string;
  // subcategory is generally blank, except for wine stores
  // using the same mispelled naming convention for consistency
  licenseSubCategory: string;
  establishmentName: string;
  establishmentAddress: string;

  establishmentAddressStreet: string;
  establishmentAddressCity: string;
  establishmentAddressPostalCode: string;
  establishmentParcelId: string;
  endorsements: Endorsement[];

  expiryDate: Date;

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
}
