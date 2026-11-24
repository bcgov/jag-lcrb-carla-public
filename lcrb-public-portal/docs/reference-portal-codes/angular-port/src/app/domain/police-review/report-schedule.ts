import { SEP_STATUS_LABELS } from "@domain/special-events/seps"
import { type ReportCadence, type ReportScheduleFilters } from "@domain/models/police-review"

/** Human label for a cadence. */
export function cadenceLabel(cadence: ReportCadence): string {
  return cadence === "daily" ? "Daily" : "Weekly"
}

/** A one-line description of the filters applied to a schedule. */
export function describeFilters(filters: ReportScheduleFilters): string {
  const bits: string[] = []
  bits.push(filters.city === "all" ? "All cities" : filters.city)
  bits.push(filters.status === "all" ? "All statuses" : SEP_STATUS_LABELS[filters.status])
  if (filters.query.trim()) bits.push(`Search: "${filters.query.trim()}"`)
  return bits.join("  ·  ")
}
