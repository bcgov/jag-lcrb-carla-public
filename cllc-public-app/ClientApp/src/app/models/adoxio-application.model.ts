import { DynamicsAccount } from './dynamics-account.model';

export class AdoxioApplication {
  id: string;
  name: string;
  applyingPerson: string;
  jobNumber: string;
  licenseType: string;
  establishmentName: string;
  establishmentAddress: string;
  applicationStatus: string;
  applicantType: string;
  account: DynamicsAccount;
  signatureagreement: boolean;
  authorizedtosubmit: boolean;
  isPaid: boolean;
}
