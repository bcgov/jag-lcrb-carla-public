import { License } from './license.model';
import { ApplicationType } from './application-type.model';

export class ApplicationLicenseSummary {
  id: string;

  establishmentName: string;  
  establishmentAddressStreet: string;
  establishmentAddressCity: string;
  establishmentAddressPostalCode: string;

  applicationStatus: string;
  licenseId: string;
  licenseType: string;
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
  allowedActions: ApplicationType[];
}
