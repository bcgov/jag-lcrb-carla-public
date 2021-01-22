import { SecurityScreeningStatusItem } from "./security-screening-item.model";

export class SecurityScreeningCategorySummary {
  outstandingItems: SecurityScreeningStatusItem[];
  completedItems: SecurityScreeningStatusItem[];
}
