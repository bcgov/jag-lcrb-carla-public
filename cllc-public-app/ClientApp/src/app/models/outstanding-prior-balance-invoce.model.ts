import { Account } from "./account.model";

export class OutstandingPriorBalanceInvoice {
  applicationId: string;
  invoice: {
    id: string;
    name: string;
    invoicenumber: string;
    totaltax: number;
    totalamount: number;
    customer: Account;
    transactionId: string;
    returnedTransactionId: string;
    statecode: number;
    statuscode: number;
    description: string;
    duedate: Date
  }; 
}

