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
  NumberofSharesNew: number;
  NumberofSharesOld: number;
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

  /**
   * Create from LegalEntity
   */
  constructor(legalEntity: LegalEntity) {
    this.isDirectorNew = legalEntity.isDirector;
    this.isDirectorOld = legalEntity.isDirector;
    this.isManagerNew = legalEntity.isSeniorManagement;
    this.isManagerOld = legalEntity.isSeniorManagement;
    this.isOfficerNew = legalEntity.isOfficer;
    this.isOfficerOld = legalEntity.isOfficer;
    this.isShareholderNew = legalEntity.isShareholder;
    this.isShareholderOld = legalEntity.isShareholder;
    // this.isTrusteeNew = legalEntity.isTrustee;
    // this.isTrusteeOld = legalEntity.isTrustee;
    // this.BusinessAccountType = legalEntity.BusinessAccountType;
    this.NumberofSharesNew = legalEntity.percentageVotingShares;
    this.NumberofSharesOld = legalEntity.percentageVotingShares;
    this.EmailNew = legalEntity.email;
    this.EmailOld = legalEntity.email;
    this.FirstNameNew = legalEntity.firstname;
    this.FirstNameOld = legalEntity.firstname;
    this.LastNameNew = legalEntity.lastname;
    this.LastNameOld = legalEntity.lastname;
    this.Name = legalEntity.name;
    this.DateofBirthNew = legalEntity.dateofbirth;
    this.DateofBirthOld = legalEntity.dateofbirth;

    // this.LicenseeChangelogid = legalEntity.LicenseeChangelogid;
    // this.BusinessAccount = legalEntity.BusinessAccount ;
    // this.Contact = legalEntity.Contact;
    // this.Application = legalEntity.Application;
    // this.ApplicationType = legalEntity.ApplicationType ;
    // this.LegalEntity = legalEntity.LegalEntity ;
    // this.ParentLinceseeChangeLogId = legalEntity.ParentLinceseeChangeLogId;
    // this.LicenseechangelogLicenseechangelogs = legalEntity.LicenseechangelogLicenseechangelogs ;
  }
}
