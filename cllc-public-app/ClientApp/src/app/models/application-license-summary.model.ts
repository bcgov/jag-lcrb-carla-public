import { License } from './license.model';
import { ApplicationType } from './application-type.model';

export class ApplicationLicenseSummary {

  establishmentName: string;
  establishmentAddressStreet: string;
  establishmentAddressCity: string;
  establishmentAddressPostalCode: string;

  status: string;
  licenseId: string;
  applicationId: string;
  licenceTypeName: string;
  licenseNumber: string;
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
  transferRequested: string;
}

export interface LicenceActionApplication {
  applicationId: string;
  applicationTypeName: string;
  isPaid: boolean;
  applicationStatus: string;
}
