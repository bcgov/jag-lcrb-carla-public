export const Cannabis = 'Cannabis';
export const Liquor = 'Liquor';

export enum LicenceTypeCategory {
  Cannabis = 845280000,
  Liquor = 845280001
}

export type AccountSummaryLicence = {
  licenceId: string;
  licenceType: string;
  licenceTypeCategory?: LicenceTypeCategory;
  expiryDate?: string;
  statusCode?: number;
};

export type AccountSummaryApplications = {
  applicationId: string;
  name: string;
  applicationType: string;
  applicantType: number;
  applicationStatus: number;
};

export class AccountSummary {
  accountId: string;
  applications: AccountSummaryApplications[];
  licences: AccountSummaryLicence[];
}
