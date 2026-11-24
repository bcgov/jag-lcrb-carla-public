import { type SepStatus } from "./special-events"

export type ReportCadence = "daily" | "weekly"

export interface ReportScheduleFilters {
  /** City name or "all". */
  city: string
  /** SEP status or "all". */
  status: SepStatus | "all"
  /** Free-text search, or empty string. */
  query: string
}
