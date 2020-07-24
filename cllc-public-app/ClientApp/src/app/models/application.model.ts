import { Account } from './account.model';
import { License } from './license.model';
import { Invoice } from './invoice.model';
import { ApplicationType } from './application-type.model';
import { TiedHouseConnection } from './tied-house-connection.model';
import { ServiceArea } from './service-area.model';

export class Application {

  previousApplication: number;
  previousApplicationDetails: string;
  ruralAgencyStoreAppointment: number;
  liquorIndustryConnections: number;
  liquorIndustryConnectionsDetails: string;
  otherBusinesses: number;
  otherBusinessesDetails: string;
  invoicetrigger: number;

  id: string;
  adoxioInvoiceId: string;
  account: Account;
  additionalPropertyInformation: string;
  applicantType: string;
  applicationStatus: string;
  applicationType: ApplicationType;
  applyingPerson: string;
  assignedLicence: License;
  authorizedToSubmit: boolean;
  contactPersonEmail: string;
  contactPersonFirstName: string;
  contactPersonLastName: string;
  contactPersonPhone: string;
  contactPersonRole: string;
  createdOn: Date;
  establishmentAddress: string;
  establishmentName: string;
  establishmentAddressCity: string;
  establishmentAddressPostalCode: string;
  establishmentAddressStreet: string;
  establishmentParcelId: string;
  establishmentPhone: string;
  establishmentEmail: string;
  isLocationChangeInProgress: boolean;
  isPaid: boolean;
  isSubmitted: boolean;
  jobNumber: string;
  licenceFeeInvoice: Invoice;
  licenceFeeInvoicePaid: boolean;
  licenseType: string;
  modifiedOn: Date;
  name: string;
  paymentReceivedDate: Date;
  registeredEstablishment: number;
  prevPaymentFailed: boolean;
  serviceHoursFridayClose: string;
  serviceHoursFridayOpen: string;
  serviceHoursMondayClose: string;
  serviceHoursMondayOpen: string;
  serviceHoursSaturdayClose: string;
  serviceHoursSaturdayOpen: string;
  serviceHoursSundayClose: string;
  serviceHoursSundayOpen: string;
  serviceHoursThursdayClose: string;
  serviceHoursThursdayOpen: string;
  serviceHoursTuesdayClose: string;
  serviceHoursTuesdayOpen: string;
  serviceHoursWednesdayClose: string;
  serviceHoursWednesdayOpen: string;
  servicehHoursStandardHours: boolean;
  signatureAgreement: boolean;
  tiedHouse: TiedHouseConnection;
  indigenousNationId: string;
  policeJurisdictionId: string;
  indigenousNation: any;
  policeJurisdiction: any;
  federalProducerNames: string;

  renewalCriminalOffenceCheck: string;
  renewalUnreportedSaleOfBusiness: string;
  renewalBusinessType: string;
  renewalTiedhouse: string;
  tiedhouseFederalInterest: string;
  renewalOrgLeadership: string;
  renewalkeypersonnel: string;
  renewalShareholders: string;
  renewalOutstandingFines: string;

  renewalBranding: string;
  renewalSignage: string;
  renewalEstablishmentAddress: string;
  renewalValidInterest: string;
  renewalZoning: string;
  renewalFloorPlan: string;
  renewalSiteMap: string;

  renewalDUI: string;
  renewalThirdParty: string;

  description1: string;

  isReadyValidInterest: boolean;
  isReadyWorkers: boolean;
  isReadyNameBranding: boolean;
  isReadyDisplays: boolean;
  isReadyIntruderAlarm: boolean;
  isReadyFireAlarm: boolean;
  isReadyLockedCases: boolean;
  isReadyLockedStorage: boolean;
  isReadyPerimeter: boolean;
  isReadyRetailArea: boolean;
  isReadyStorage: boolean;
  isReadyExtranceExit: boolean;
  isReadySurveillanceNotice: boolean;
  isReadyProductNotVisibleOutside: boolean;
  establishmentopeningdate: Date;
  isApplicationComplete: string;

  lGNameOfOfficial: string;
  lGTitlePosition: string;
  lGContactPhone: string;
  lGContactEmail: string;
  lGApprovalDecision: string;
  lGDecisionSubmissionDate: Date;
  lGDecisionComments: string;
  resolutionDocsUploaded: boolean;
  lgZoning: string;

  applicant: Account;

  serviceAreas: ServiceArea[];
  outsideAreas: ServiceArea[];


  // Manufactuer
  licenceSubCategory: string;
  isPackaging: boolean;

  mfgBrewPubOnSite: string;
  mfgPipedInProduct: string;

  // these are just optional int - not picklist references.
  mfgAcresOfFruit: number;
  mfgAcresOfGrapes: number;
  mfgAcresOfHoney: number;

  mfgMeetsProductionMinimum: boolean;
  mfgStepBlending: boolean;
  mfgStepCrushing: boolean;
  mfgStepFiltering: boolean;
  mfgStepSecFermOrCarb: boolean;
  isAlr: boolean;
  pidList: string; 

  // Manufactuer Structural Change - Patio
  patioCompDescription: string;
  patioLocationDescription: string; 
  patioAccessDescription: string;
  patioIsLiquorCarried: boolean;
  patioLiquorCarriedDescription: string;
  patioAccessControlDescription: string;
        
  locatedAboveDescription: number;
  patioServiceBar: number;


}
