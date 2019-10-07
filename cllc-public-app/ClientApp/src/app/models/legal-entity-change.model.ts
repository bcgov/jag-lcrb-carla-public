import { Account } from './account.model';
import { Application } from './application.model';
import { LegalEntity } from './legal-entity.model';
import { Contact } from './contact.model';

export class LicenseeChangeLog {
  id: string; // guid

  isDirectorNew: boolean;
  isDirectorOld: boolean;
  isManagerNew: boolean;
  isManagerOld: boolean;
  isOfficerNew: boolean;
  isOfficerOld: boolean;
  isShareholderNew: boolean;
  isShareholderOld: boolean;
  isTrusteeNew: boolean;
  isTrusteeOld: boolean;
  BusinessAccountType: string;
  NumberofSharesNew: string;
  NumberofSharesOld: string;
  EmailNew: string;
  EmailOld: string;
  FirstNameNew: string;
  FirstNameOld: string;
  JobNumber: string;
  LastNameNew: string;
  LastNameOld: string;
  LicenseeChangelogid: string;
  Name: string;
  DateofBirthNew: Date;
  DateofBirthOld: Date;

  BusinessAccount: Account;
  Contact: Contact;
  Application: Application;
  ApplicationType: string;
  LegalEntity: LegalEntity;
  ParentLinceseeChangeLogId: LicenseeChangeLog;
  LicenseechangelogLicenseechangelogs: LicenseeChangeLog[];
}
