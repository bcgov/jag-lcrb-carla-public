import { Account } from './account.model';

export class LegalEntity {
  id: string; // guid
  name: string;
  isindividual: boolean;
  sameasapplyingperson: boolean;
  legalentitytype: string;
  otherlegalentitytype: string;
  firstname: string;
  middlename: string;
  lastname: string;
  isOfficer: boolean;
  isApplicant: boolean;
  isDirector: boolean;
  isSeniorManagement: boolean;
  isShareholder: boolean;
  isPartner: boolean;
  partnerType: string;
  isOwner: boolean;
  dateofbirth: Date;
  interestpercentage: number;
  commonvotingshares: number;
  preferredvotingshares: number;
  commonnonvotingshares: number;
  preferrednonvotingshares: number;
  account: Account;
  relatedentities: LegalEntity[];
  email: string;
  dateofappointment: Date;
  securityAssessmentEmailSentOn: Date;
  accountId: string;
  shareholderAccountId: string;
  // helper fields
  shareholderType: string;
  sendConsentRequest: boolean;
  parentLegalEntityId: string;
  position: string;
}
