export class Contact {
  id: string;
  fullname: string;
  firstname: string;
  lastname: string;
  emailaddress1: string;
  telephone1: string;
  address1_line1: string;
  address1_city: string;
  address1_stateorprovince: string;
  address1_country: string;
  address1_postalcode: string;
  jobTitle: string;
  birthDate: Date;

  birthPlace: string;
  gender: string;
  mobilePhone: string;
  primaryIdNumber: string;
  secondaryIdNumber: string;
  isWorker: boolean;
  selfDisclosure: string;
  secondaryIdentificationType: string;
  primaryIdentificationType: string;

  phsConnectionsDetails: string;
  phsLivesInCanada: string;
  phsHasLivedInCanada: string;
  phsExpired: string;
  phsComplete: string;
  phsConnectionsToOtherLicences: string;
  phsCanadianDrugAlchoholDrivingOffence: string;
  phsDateSubmitted: Date;
  phsForeignDrugAlchoholOffence: string;
}
