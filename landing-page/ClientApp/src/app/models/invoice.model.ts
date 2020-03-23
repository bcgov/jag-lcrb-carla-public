import { Account } from './account.model';

export class Invoice {
  id: string; // guid
  name: string;
  invoicenumber: string;
  totaltax: number;
  totalamount: number;
  customer: Account;
}
