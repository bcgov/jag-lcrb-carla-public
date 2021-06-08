import { Account } from "./account.model";
import { Contact } from "./contact.model";

export class SepApplicationSummary {
  specialEventId: string; // server side primary key
  eventStartDate: Date | string;
  eventName: string;
  typeOfEvent: number;
  eventStatus: string;
  maximumNumberOfGuests: number;
  dateSubmitted: Date | string;
  invoiceId: string;
  isInvoicePaid: boolean;

  policeAccount?: Account;
  policeDecisionBy?: Contact;
  policeApproval?: string;
  lcrbApproval?: string;
  denialReason?: string;
  cancelReason?: string;
  dateOfPoliceDecision?: Date | string;
}
