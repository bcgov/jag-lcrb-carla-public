import { ApplicationType } from "./application-type.model";

export class LicenseType {
  id: string;
  code: string;
  name: string;
  allowedActions: ApplicationType[];
}

export enum LicenceTypeNames {
  S119 = "S119 CRS Authorization",
  CRS = "Cannabis Retail Store Licence"
}
