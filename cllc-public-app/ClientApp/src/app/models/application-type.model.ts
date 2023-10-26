
import { LicenseType } from "./license-type.model";
import { ApplicationContentType } from "./application-content-type.model";
import { DynamicsForm } from "./dynamics-form.model";

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
  showLgNoObjection: boolean;
  showLiquorDeclarations: boolean;
  showOwnershipDeclaration: boolean;
  establishmetNameIsReadOnly: boolean;
  formReference: string;
  showDescription1: boolean;
  hasLESection: boolean;
  showPatio: boolean;

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
  isStructural: boolean;
  isDefault: boolean;
  requiresSecurityScreening: boolean;
  letterOfIntent: string;
  dynamicsForm: DynamicsForm;

  serviceAreas: boolean;
  outsideAreas: boolean;
  capacityArea: boolean;

  hasALRQuestion: boolean;
  showZoningDeclarations: boolean;
  isRelocation: boolean;
}

export enum FormControlState {
  Show = "Yes",
  Hide = "No",
  ReadOnly = "Readonly"
}

export enum ApplicationTypeNames {
  Agent = "Agent",
  Catering = "Catering",
  CRSEstablishmentNameChange = "CRS Establishment Name Change",
  CRSLocationChange = "CRS Location Change",
  CRSRenewal = "CRS Renewal",
  CRSRenewalLate30 = "CRS Late Renewal - 30 Day",
  CRSRenewalLate6Months = "CRS Late Renewal - 30 Day to 6 Months",
  CRSStructuralChange = "CRS Structural Change",
  CRSTransferofOwnership = "CRS Transfer of Ownership",
  CannabisRetailStore = "Cannabis Retail Store",
  ProductionRetailStore = "Producer Retail Store",
  ETHYL = "Ethyl Alcohol Permit",
  FP = "Food Primary",
  FPRelo = "Food Primary Relocation",
  F2G = "Farm to Gate",
  LicenseeChanges = "Licensee Changes",
  LP = "Liquor Primary",
  LPC = "Liquor Primary Club",
  LPR = 'LP Relocation',
  Marketer = "Marketing",
  MarketingRenewal = "Marketing Renewal",
  LGINClaim = "LG/IN Claim",
  PoliceClaim = "Police Claim",
  LRSTransferofLocation = "LRS Transfer of Location",
  LiquorRenewal = "Liquor Licence Renewal",
  LiquorLicenceTransfer = "Liquor Licence Transfer",
  PermanentChangeToALicensee = "Permanent Change to a Licensee",
  PermanentChangeToAnApplicant = "Permanent Change to an Applicant",
  WineStore = "Wine Store",
  RAS = "Rural Agency Store",
  RLRS = "Rural Licensee Retail Store",
  MFG = "Manufacturer",
  UBV = "UBrew and UVin",
  LoungeAreaEndorsment = "Lounge Area Endorsement",
  SpecialEventAreaEndorsement = "Special Event Area Endorsement",
  LRSStructuralChange = "LRS Structural Change",
  RequestTermChange = "Non-Default T&C Change Application",
  TiedHouseExemption = "Tied House Exemption",
  OutstandingPriorBalanceInvoice = "Outstanding Prior Balance Invoice - LIQ",
  TiedHouseExemptionRemoval = "Tied House Exemption Removal",
  TiedHouseExemptionApplication = "Tied House Exemption Application",
}
