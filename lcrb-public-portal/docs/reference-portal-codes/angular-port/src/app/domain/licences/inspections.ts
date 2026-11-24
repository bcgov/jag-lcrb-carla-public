import { type InspectionKind, type ScheduledInspectionStatus } from "@domain/models/licences"


export const INSPECTION_KIND_META: Record<
  InspectionKind,
  { label: string; chip: string; blurb: string }
> = {
  routine: {
    label: "Routine",
    chip: "bg-blue-100 text-blue-800 border-blue-200",
    blurb: "Scheduled periodic check of ongoing compliance.",
  },
  "follow-up": {
    label: "Follow-up",
    chip: "bg-orange-100 text-orange-800 border-orange-200",
    blurb: "Verifies corrective action from a previous inspection.",
  },
  "pre-opening": {
    label: "Pre-opening",
    chip: "bg-purple-100 text-purple-800 border-purple-200",
    blurb: "Confirms the establishment is ready to open under its licence.",
  },
  complaint: {
    label: "Complaint-driven",
    chip: "bg-red-100 text-red-800 border-red-200",
    blurb: "Prompted by a complaint or reported concern.",
  },
  education: {
    label: "Educational",
    chip: "bg-green-100 text-green-800 border-green-200",
    blurb: "Guidance visit — no enforcement outcome.",
  },
}

export const INSPECTION_STATUS_META: Record<
  ScheduledInspectionStatus,
  { label: string; chip: string }
> = {
  scheduled: { label: "Scheduled", chip: "bg-slate-100 text-slate-700 border-slate-200" },
  confirmed: { label: "Confirmed", chip: "bg-green-100 text-green-800 border-green-200" },
  rescheduled: { label: "Rescheduled", chip: "bg-yellow-100 text-yellow-800 border-yellow-200" },
}

/** Format an ISO datetime as a date + time for display. */
export function formatInspectionDateTime(iso: string): string {
  const d = new Date(iso)
  return `${d.toLocaleDateString("en-CA", { year: "numeric", month: "long", day: "numeric" })} · ${d.toLocaleTimeString(
    "en-CA",
    { hour: "numeric", minute: "2-digit" },
  )}`
}
