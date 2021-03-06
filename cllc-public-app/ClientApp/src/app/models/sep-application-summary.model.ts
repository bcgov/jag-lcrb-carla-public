import { Account } from "./account.model";
import { Contact } from "./contact.model";
import { SepCity } from "./sep-city.model";

export class SepApplicationSummary {
  localId: string; // local memory primary key
  lastStepCompleted: string;
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
