import { DynamicsAccount } from './dynamics-account.model';
import { AdoxioLicense } from './adoxio-license.model';
import { Invoice } from './invoice.model';

export class AdoxioApplication {
  id: string;
  account: DynamicsAccount;
  applicantType: string;
  applicationStatus: string;
  applyingPerson: string;
  assignedLicence: AdoxioLicense;
  authorizedtosubmit: boolean;
  contactpersonemail: string;
  contactpersonfirstname: string;
  contactpersonlastname: string;
  contactpersonphone: string;
  contactpersonrole: string;
  createdon: Date;
  establishmentAddress: string;
  establishmentName: string;
  establishmentaddresscity: string;
  establishmentaddresspostalcode: string;
  establishmentaddressstreet: string;
  establishmentparcelid: string;
  isPaid: boolean;
  isSubmitted: boolean;
  jobNumber: string;
  licenceFeeInvoice: Invoice;
  licenceFeeInvoicePaid: boolean;
  licenseType: string;
  modifiedon: Date;
  name: string;
  paymentreceiveddate: Date;
  prevPaymentFailed: boolean;
  signatureagreement: boolean;

  servicehHoursStandardHours: boolean;
  serviceHoursSundayOpen: string;
  serviceHoursSundayClose: string;
  serviceHoursMondayOpen: string;
  serviceHoursMondayClose: string;
  serviceHoursTuesdayOpen: string;
  serviceHoursTuesdayClose: string;
  serviceHoursWednesdayOpen: string;
  serviceHoursWednesdayClose: string;
  serviceHoursThursdayOpen: string;
  serviceHoursThursdayClose: string;
  serviceHoursFridayOpen: string;
  serviceHoursFridayClose: string;
  serviceHoursSaturdayOpen: string;
  serviceHoursSaturdayClose: string;

  authorizedToSubmit: boolean;
  signatureAgreement: boolean;

  //isTransferInProgress: boolean;
  isLocationChangeInProgress: boolean;
}
