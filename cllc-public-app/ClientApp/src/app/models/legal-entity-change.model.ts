import { Account } from './account.model';
import { Application } from './application.model';
import { LegalEntity } from './legal-entity.model';
import { Contact } from './contact.model';

export class LicenseeChangeLog {
  id: string; // guid
  typeOfChange: string;
  isIndividual: boolean;
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
  businessAccountType: string;
  numberofSharesNew: number;
  numberofSharesOld: number;
  emailNew: string;
  emailOld: string;
  firstNameNew: string;
  firstNameOld: string;
  lastNameNew: string;
  lastNameOld: string;
  LicenseeChangelogid: string;
  nameNew: string;
  nameOld: string;
  dateofBirthNew: Date;
  dateofBirthOld: Date;
  titleNew: string;
  titleOld: string;


  businessAccount: Account;
  contact: Contact;
  applicationId: string;
  applicationType: string;
  legalEntityId: string;
  parentLegalEntityId: string;
  parentLinceseeChangeLog: LicenseeChangeLog;
  children: LicenseeChangeLog[];

  isRoot: boolean; // This is only used on the client side
  lastTypeOfChange: string; // This is only used on the client side

  /**
   * Create from LegalEntity
   */
  constructor(legalEntity: LegalEntity = null) {
    if (legalEntity) {
      this.legalEntityId = legalEntity.id;
      this.parentLegalEntityId = legalEntity.parentLegalEntityId;
      this.typeOfChange = 'unchanged';
      this.isIndividual = legalEntity.isindividual;
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
      this.numberofSharesNew = legalEntity.percentageVotingShares;
      this.numberofSharesOld = legalEntity.percentageVotingShares;
      this.emailNew = legalEntity.email;
      this.emailOld = legalEntity.email;
      this.firstNameNew = legalEntity.firstname;
      this.firstNameOld = legalEntity.firstname;
      this.lastNameNew = legalEntity.lastname;
      this.lastNameOld = legalEntity.lastname;
      this.nameNew = legalEntity.name;
      this.nameOld = legalEntity.name;
      this.dateofBirthNew = legalEntity.dateofbirth;
      this.dateofBirthOld = legalEntity.dateofbirth;
      this.titleNew = legalEntity.jobTitle;
      this.titleOld = legalEntity.jobTitle;

      this.parentLegalEntityId = legalEntity.parentLegalEntityId;

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

  getNewLeadershipPosition(): string {
    let position = '';
    if (this.isDirectorNew) {
      position += 'Director, ';
    }
    if (this.isManagerNew) {
      position += 'Manager, ';
    }
    if (this.isOfficerNew) {
      position += 'Officer, ';
    }
    if (this.isTrusteeNew) {
      position += 'Trustee, ';
    }
    if (this.titleNew) {
      position += `${this.titleNew}, `;
    }
    position = position.substring(0, position.length - 2);
    return position;
  }
  getOldLeadershipPosition(): string {
    let position = '';
    if (this.isDirectorOld) {
      position += 'Director, ';
    }
    if (this.isManagerOld) {
      position += 'Manager, ';
    }
    if (this.isOfficerOld) {
      position += 'Officer, ';
    }
    if (this.isTrusteeOld) {
      position += 'Trustee, ';
    }
    if (this.titleOld) {
      position += `${this.titleOld}, `;
    }
    position = position.substring(0, position.length - 2);
    return position;
  }
}
