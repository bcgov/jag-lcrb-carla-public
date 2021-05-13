import { Account } from "./account.model";
import { Contact } from "./contact.model";

export class SepApplicationSummary {
  specialEventId: string; // server side primary key
  eventStartDate: Date;
  eventName: string;
  typeOfEvent: number;
  eventStatus: number;
  maximumNumberOfGuests: number;
  dateSubmitted: Date;

  policeAccount?: Account;
  policeDecisionBy?: Contact;
  policeDecision?: number;
  dateOfPoliceDecision?: Date;
}
