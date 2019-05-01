
import { LicenseType } from './license-type.model';

export class ApplicationType {
  id: string;
  actionText: string;
  name: string;
  licenseType: LicenseType;

  title: string;
  preamble: string;
  beforeStarting: string;
  nextSteps: string;

  showPropertyDetails: boolean;
  showCurrentProperty: boolean;
  showHoursOfSale: boolean;
  showAssociatesFormUpload: boolean;
  showFinancialIntegrityFormUpload: boolean;
  showSupportingDocuments: boolean;
  showDeclarations: boolean;
}
