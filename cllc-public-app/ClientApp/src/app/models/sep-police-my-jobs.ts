import { SepApplicationSummary } from "./sep-application-summary.model";

export class SepPoliceMyJobs {
    inProgress: SepApplicationSummary[]; 
    policeApproved : SepApplicationSummary[];
    issued : SepApplicationSummary[];
}
