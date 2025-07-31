export type AccountSummaryLicence = {
  licenceId: string;
  LicenseType: string;
  ExpiryDate?: string;
  StatusCode?: number;
};

export type AccountSummaryApplications = {
  applicationId: string;
  Name: string;
  ApplicationType: string;
  ApplicantType: number;
  ApplicationStatus: number;
};

export class AccountSummary {
  accountId: string;
  applications: AccountSummaryApplications[];
  licences: AccountSummaryLicence[];
}
