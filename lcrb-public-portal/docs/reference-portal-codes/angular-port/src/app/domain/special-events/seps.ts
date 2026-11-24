import { type SepStatus, type SepRecord, type ReferralInput, type ReferralAssessment } from "@domain/models/special-events"

/** The auto-approval threshold: events under this attendance are auto-approved. */
export const AUTO_APPROVAL_THRESHOLD = 500

/** Referral thresholds beyond attendance; the values are prototype numbers, not policy. The form has no indoor/outdoor field, so the indoor 2 a.m. cutoff is used. */
export const REFERRAL_RULES = {
  /** Events with this many standard drinks or more are referred. */
  drinksThreshold: 2000,
  /** Latest permitted service end, as minutes after midnight (02:00). */
  latestEndMinutes: 2 * 60,
  /** Earliest "late" start, as minutes after midnight (09:00); service before this is unusual. */
  earliestStartMinutes: 9 * 60,
} as const

function parseMinutes(hhmm?: string): number | null {
  if (!hhmm) return null
  const m = /^(\d{1,2}):(\d{2})$/.exec(hhmm.trim())
  if (!m) return null
  const h = Number(m[1])
  const min = Number(m[2])
  if (h > 23 || min > 59) return null
  return h * 60 + min
}

/** Decides whether a submission is auto-approved or referred, and why; pure so the confirmation page and reviewer timeline share the logic. */
export function assessSepReferral(input: ReferralInput): ReferralAssessment {
  const reasons: string[] = []

  if (input.attendance >= AUTO_APPROVAL_THRESHOLD) {
    reasons.push(`Expected attendance of ${input.attendance.toLocaleString()} is ${AUTO_APPROVAL_THRESHOLD} or more.`)
  }

  const end = parseMinutes(input.endTime)
  const start = parseMinutes(input.startTime)
  if (end !== null) {
    // An end time from 00:01 to 02:00 is a wrap past midnight; after 02:00 but before the earliest start is "too late"; at or after the earliest start is a same-day evening end, fine up to 23:59.
    const wrapsPastMidnight = start !== null && end < start
    const endsAfterCutoff =
      wrapsPastMidnight && end > REFERRAL_RULES.latestEndMinutes && end < REFERRAL_RULES.earliestStartMinutes
    if (endsAfterCutoff) {
      reasons.push(`Liquor service ends at ${input.endTime}, after the 2:00 a.m. cutoff.`)
    }
  }

  if (typeof input.estimatedDrinks === "number" && input.estimatedDrinks >= REFERRAL_RULES.drinksThreshold) {
    reasons.push(
      `Estimated ${input.estimatedDrinks.toLocaleString()} standard drinks is ${REFERRAL_RULES.drinksThreshold.toLocaleString()} or more.`,
    )
  }

  if (input.isPublicProperty) {
    reasons.push("Event is on public property and needs local government or First Nation approval on file.")
  }

  if (input.eventType === "public" && input.hasSecurity === false && input.attendance >= 200) {
    reasons.push("Public event with 200 or more guests has no security staff planned.")
  }

  return { autoApprove: reasons.length === 0, reasons }
}

export const STANDARD_CONDITIONS: string[] = [
  "All liquor service must cease 30 minutes before the event end time.",
  "Minors are not permitted in designated liquor service areas unless accompanied by a parent or guardian.",
  "A copy of this permit must be displayed prominently at the event and available on request.",
  "Sufficient food and non-alcoholic beverages must be available throughout the event.",
  "Security and staffing levels must be appropriate for the expected attendance.",
]

export const SEP_STATUS_LABELS: Record<SepStatus, string> = {
  submitted: "Submitted",
  "under-review": "Under Review",
  "changes-requested": "Changes Requested",
  approved: "Approved",
  denied: "Denied",
  "auto-approved": "Auto-Approved",
  cancelled: "Cancelled",
}

/** Tailwind class sets for status badges (kept consistent across roles). */
export const SEP_STATUS_STYLES: Record<SepStatus, string> = {
  submitted: "bg-blue-100 text-blue-800 border-blue-200",
  "under-review": "bg-amber-100 text-amber-800 border-amber-200",
  "changes-requested": "bg-orange-100 text-orange-800 border-orange-200",
  approved: "bg-green-100 text-green-800 border-green-200",
  denied: "bg-red-100 text-red-800 border-red-200",
  "auto-approved": "bg-slate-100 text-slate-700 border-slate-200",
  cancelled: "bg-zinc-100 text-zinc-600 border-zinc-200",
}

/** Whether a SEP is active on a calendar day (multi-day events included); used for the police one-day PDF report. */
export function sepOccursOnDate(record: SepRecord, dateIso: string): boolean {
  const day = dateIso.slice(0, 10)
  return record.eventStart.slice(0, 10) <= day && day <= record.eventEnd.slice(0, 10)
}

/** Today's date as an ISO (YYYY-MM-DD) string in local time. */
export function todayIso(): string {
  const d = new Date()
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`
}

/** Whether the permit was auto-approved, read from the recorded status rather than attendance, since a 200-person event can legitimately sit in the review queue. */
export function isAutoApproved(record: SepRecord): boolean {
  return record.status === "auto-approved"
}

export function formatEventDate(record: SepRecord): string {
  const start = new Date(record.eventStart + "T00:00:00")
  const end = new Date(record.eventEnd + "T00:00:00")
  const opts: Intl.DateTimeFormatOptions = { month: "short", day: "numeric", year: "numeric" }
  if (record.eventStart === record.eventEnd) {
    return start.toLocaleDateString("en-CA", opts)
  }
  return `${start.toLocaleDateString("en-CA", { month: "short", day: "numeric" })} – ${end.toLocaleDateString("en-CA", opts)}`
}

/** Formats a stored 24-hour "HH:MM" for display ("18:00" -> "6:00 PM"). Stored zero-padded 24-hour because views sort with `localeCompare`; convert at render time only. */
export function formatTimeOfDay(time: string): string {
  const match = /^(\d{1,2}):(\d{2})$/.exec(time.trim())
  if (!match) return time || "—"
  const hours = Number(match[1])
  if (Number.isNaN(hours) || hours > 23) return time
  const suffix = hours < 12 ? "AM" : "PM"
  const hour12 = hours % 12 === 0 ? 12 : hours % 12
  return `${hour12}:${match[2]} ${suffix}`
}

/** Daily liquor-service window for a permit, e.g. "6:00 PM – 11:30 PM". */
export function formatServiceWindow(record: Pick<SepRecord, "startTime" | "endTime">): string {
  if (!record.startTime && !record.endTime) return "—"
  return `${formatTimeOfDay(record.startTime)} – ${formatTimeOfDay(record.endTime)}`
}

/** Maps a city to its policing jurisdiction and approximate coordinates so a new SEP lands in the right queue and on the map. TODO: use the authoritative jurisdiction lookup. */
export function jurisdictionForCity(city: string): { name: string; lat: number; lng: number } {
  const table: Record<string, { name: string; lat: number; lng: number }> = {
    nanaimo: { name: "Nanaimo RCMP", lat: 49.1697, lng: -123.9362 },
    langford: { name: "West Shore RCMP", lat: 48.4486, lng: -123.5054 },
    duncan: { name: "North Cowichan / Duncan RCMP", lat: 48.7787, lng: -123.7079 },
    victoria: { name: "Victoria Police Department", lat: 48.4284, lng: -123.3656 },
    kelowna: { name: "Kelowna RCMP", lat: 49.888, lng: -119.496 },
    surrey: { name: "Surrey RCMP", lat: 49.1913, lng: -122.849 },
    vancouver: { name: "Vancouver Police Department", lat: 49.2827, lng: -123.1207 },
  }
  const key = city.trim().toLowerCase()
  return table[key] ?? { name: "Victoria Police Department", lat: 48.4284, lng: -123.3656 }
}

/** Distinct cities present in a set of SEP records, sorted alphabetically. */
export function distinctCities(records: SepRecord[]): string[] {
  return Array.from(new Set(records.map((r) => r.city))).sort((a, b) => a.localeCompare(b))
}
