import { DynamicsAccount } from './dynamics-account.model';

export class AdoxioLegalEntity {
  shareholderType: string;
  //email: string;
  //dateIssued: Date;
  
  id: string; // string form of the guid
  name: string;
  isindividual: boolean;
  sameasapplyingperson: boolean;
  legalentitytype: string;
  otherlegalentitytype: string;
  firstname: string;
  middlename: string;
  lastname: string;
  position: string;
  dateofbirth: Date;
  interestpercentage: number;
  commonvotingshares: number;
  preferredvotingshares: number;
  commonnonvotingshares: number;
  preferrednonvotingshares: number;
  account: DynamicsAccount;
  relatedentities: AdoxioLegalEntity[];
}
