
import { LicenseType } from './license-type.model';
import { ApplicationContentType } from './application-content-type.model';
import { DynamicsForm } from './dynamics-form.model';

export class ApplicationType {
  id: string;
  actionText: string;
  name: string;
  licenseType: LicenseType;
  category: string;

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
  showLiquorDeclarations: boolean;
  ShowOwnershipDeclaration: boolean;
  establishmetNameIsReadOnly: boolean;
  formReference: string;
  showDescription1: boolean;

  storeContactInfo: FormControlState;
  establishmentName: FormControlState;
  newEstablishmentAddress: FormControlState;
  currentEstablishmentAddress: FormControlState;
  signage: FormControlState;
  validInterest: FormControlState;
  floorPlan: FormControlState;
  sitePlan: FormControlState;
  connectedGroceryStore: FormControlState;
  sitePhotos: FormControlState;
  publicCooler: FormControlState;
  showLiquorSitePlan: FormControlState;
  proofofZoning: FormControlState;
  lGandPoliceSelectors: FormControlState;
  isShowLGINApproval: boolean;
  isShowLGZoningConfirmation: boolean;
  isFree: boolean;
  isEndorsement: boolean;
  requiresSecurityScreening: boolean;

  dynamicsForm: DynamicsForm;

  serviceAreas: boolean;
  outsideAreas: boolean;
  capacityArea: boolean;
}

export enum FormControlState {
  Show = 'Yes',
  Hide = 'No',
  ReadOnly = 'Readonly'
}

export enum ApplicationTypeNames {
  Catering = 'Catering',
  CRSEstablishmentNameChange = 'CRS Establishment Name Change',
  CRSLocationChange = 'CRS Location Change',
  CRSRenewal = 'CRS Renewal',
  CRSRenewalLate30 = 'CRS Late Renewal - 30 Day',
  CRSRenewalLate6Months = 'CRS Late Renewal - 30 Day to 6 Months',
  CRSStructuralChange = 'CRS Structural Change',
  CRSTransferofOwnership = 'CRS Transfer of Ownership',
  CannabisRetailStore = 'Cannabis Retail Store',
  LicenseeChanges = 'Licensee Changes',
  Marketer = 'Marketing',
  LGINClaim = 'LG/IN Claim',
  LiquorRelocation = 'Liquor Licence Relocation',
  LiquorRenewal = 'Liquor Licence Renewal',
  WineStore = 'Wine Store',
  RAS = "Rural Agency Store",
  MFG = "Manufacturer"
}
