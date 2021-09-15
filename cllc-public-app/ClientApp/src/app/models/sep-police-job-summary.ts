import { SepApplicationSummary } from "./sep-application-summary.model";

export class SepPoliceJobSummary {
    inProgress: SepApplicationSummary[]; 
    policeApproved : SepApplicationSummary[];
    policeDenied : SepApplicationSummary[];
}
