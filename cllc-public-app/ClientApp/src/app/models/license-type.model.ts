import { ApplicationType } from "./application-type.model";

export class LicenseType {
  id: string;
  code: string;
  name: string;
  allowedActions: ApplicationType[];
}

export enum LicenceTypeNames {
  S119 = "Section 119 Authorization",
  S119CRS = "S119 CRS Authorization",
  S119PRS = "S119 PRS Authorization",
  CRS = "Cannabis Retail Store Licence",
  PRS = "Producer Retail Store",
  Marketing = "Marketing"
}
