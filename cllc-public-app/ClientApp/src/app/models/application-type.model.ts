
import { LicenseType } from './license-type.model';
import { ApplicationContentType } from './application-content-type.model';

export class ApplicationType {
  id: string;
  actionText: string;
  name: string;
  licenseType: LicenseType;

  title: string;
  // preamble: string;
  // beforeStarting: string;
  // nextSteps: string;
  contentTypes: ApplicationContentType[];

  showPropertyDetails: boolean;
  showCurrentProperty: boolean;
  showHoursOfSale: boolean;
  showAssociatesFormUpload: boolean;
  showFinancialIntegrityFormUpload: boolean;
  showSupportingDocuments: boolean;
  showDeclarations: boolean;
  establishmetNameIsReadOnly: boolean;

  storeContactInfo: FormControlState;
  establishmentName: FormControlState;
  newEstablishmentAddress: FormControlState;
  currentEstablishmentAddress: FormControlState;
  signage: FormControlState;
  validInterest: FormControlState;
  floorPlan: FormControlState;
  sitePlan: FormControlState;

}

export enum FormControlState {
  Show = 'Yes',
  Hide = 'No',
  Reaonly = 'Readonly'
}

export enum ApplicationTypeNames {
  CannabisRetailStore = 'Cannabis Retail Store',
  CRSLocationChange = 'CRS Location Change',
  CRSStructuralChange = 'CRS Structural Change',
  CRSTransferofOwnership = 'CRS Transfer of Ownership',
  CRSRenewal = 'CRS Renewal',
  CRSRenewalLate30 = 'CRS Late Renewal - 30 Day',
  CRSRenewalLate6Months = 'CRS Late Renewal - 30 Day to 6 Months',
  Marketer = 'Marketer'
}
