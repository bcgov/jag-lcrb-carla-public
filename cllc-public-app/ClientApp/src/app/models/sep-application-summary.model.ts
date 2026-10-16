import { Account } from './account.model';
import { Contact } from './contact.model';

export class SepApplicationSummary {
  localId: string; // local memory primary key
  lastStepCompleted: string;
  specialEventId: string; // server side primary key
  eventStartDate: Date | string;
  modifiedOn?: Date | string; // server-stamped; dictates which Pacific rule eventStartDate was saved under
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
