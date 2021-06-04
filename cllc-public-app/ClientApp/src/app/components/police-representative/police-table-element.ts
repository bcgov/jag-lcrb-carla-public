import { SepApplicationSummary } from "@models/sep-application-summary.model";

// Show text labels instead of numeric enum values in the table; e.g. Status = "In Progress" vs. 100,000,001
export interface PoliceTableElement extends SepApplicationSummary {
    eventStatusLabel?: string;
    policeDecisionByLabel?: string;
    typeOfEventLabel?: string;
  }
