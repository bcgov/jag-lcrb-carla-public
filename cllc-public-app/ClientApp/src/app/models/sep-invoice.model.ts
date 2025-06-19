import { Account } from '@models/account.model';

/**
 * Special Event Application Invoice Model.
 *
 * @export
 * @class SepInvoice
 */
export class SepInvoice {
  id: string;
  name: string;
  invoicenumber: string;
  totaltax: number;
  totalamount: number;
  customer: Account | null;
  transactionId: 'string';
  returnedTransactionId: string | null;
  statecode: number;
  statuscode: number;
  description: string;
  duedate: null | Date;
}
