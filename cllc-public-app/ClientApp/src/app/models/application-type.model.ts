
import { LicenseType } from './license-type.model';
import { ApplicationContentType } from './application-content-type.model';

export class ApplicationType {
  id: string;
  actionText: string;
  name: string;
  licenseType: LicenseType;

  title: string;
  // preamble: string;
  // beforeStarting: string;
  // nextSteps: string;
  typeContents: ApplicationContentType[];

  showPropertyDetails: boolean;
  showCurrentProperty: boolean;
  showHoursOfSale: boolean;
  showAssociatesFormUpload: boolean;
  showFinancialIntegrityFormUpload: boolean;
  showSupportingDocuments: boolean;
  showDeclarations: boolean;
}

export enum ApplicationTypeNames {
  CannabisRetailStore = 'Cannabis Retail Store',
  CRSLocationChange = 'CRS Location Change',
  CRSStructuralChange = 'CRS Structural Change',
  CRSTransferofOwnership = 'CRS Transfer of Ownership',
  Marketer= 'Marketer'
}
