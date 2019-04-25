import { ApplicationType } from './application-type.model';

export class LicenseType {
  id: string;
  code: string;
  name: string;
  allowedActions: ApplicationType[];
}
