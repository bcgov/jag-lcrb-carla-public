import { LG_STATUS_LABELS } from "@domain/models/lg-review"
import { type ReportCadence, type LgReportFilters } from "@domain/models/lg-review"

export function cadenceLabel(cadence: ReportCadence): string {
  return cadence === "daily" ? "Daily" : "Weekly"
}

export function describeFilters(filters: LgReportFilters): string {
  const bits: string[] = []
  bits.push(filters.status === "all" ? "All statuses" : LG_STATUS_LABELS[filters.status])
  if (filters.query.trim()) bits.push(`Search: "${filters.query.trim()}"`)
  return bits.join("  ·  ")
}
