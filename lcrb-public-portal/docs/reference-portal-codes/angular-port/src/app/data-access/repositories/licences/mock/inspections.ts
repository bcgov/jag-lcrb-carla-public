/** Scheduled establishment inspections (prototype). */

import { COMPLIANCE_HISTORY } from "../../compliance/mock/enforcement"
import { daysUntil } from "@domain/compliance/enforcement"
import { type ComplianceRecord } from "@domain/models/compliance"
import { type ScheduledInspection } from "@domain/models/licences"

/** Days from today (negative = past) as an ISO datetime string. */
function isoFromNow(days: number, hour = 10, minute = 0): string {
  const d = new Date()
  d.setDate(d.getDate() + days)
  d.setHours(hour, minute, 0, 0)
  return d.toISOString()
}

/** Upcoming inspections C&E has placed on the calendar. */
export const SCHEDULED_INSPECTIONS: ScheduledInspection[] = [
  {
    id: "INS-2026-0511",
    licenceId: "326739",
    establishmentName: "OneStop Test 1",
    licenceType: "Liquor Primary",
    scheduledDate: isoFromNow(9, 19, 30),
    window: "Evening service · 7:30–9:30 PM",
    kind: "routine",
    status: "confirmed",
    focusAreas: [
      "Hours of liquor service",
      "Overcrowding / approved occupancy",
      "Service to intoxicated patrons",
      "Serving It Right certification on shift",
    ],
    officer: "Insp. D. Marsh",
    office: "Compliance & Enforcement — Vancouver Island",
    note: "Routine periodic inspection. No action needed in advance; please ensure your current licence and floor plan are on site.",
  },
  {
    id: "INS-2026-0498",
    licenceId: "326741",
    establishmentName: "Green Leaf Victoria",
    licenceType: "Cannabis Retail Store",
    scheduledDate: isoFromNow(23, 14, 0),
    window: "Afternoon · from 2:00 PM",
    kind: "follow-up",
    status: "scheduled",
    focusAreas: [
      "Product visibility from outside the store",
      "Age verification procedures",
      "Inventory reconciliation records",
    ],
    officer: "Insp. R. Okafor",
    office: "Compliance & Enforcement — Vancouver Island",
    note: "Follow-up to the written warning issued on your last inspection (product display). The inspector will confirm the corrective steps taken.",
    confirmationRequested: true,
  },
  {
    id: "INS-2026-0532",
    licenceId: "326740",
    establishmentName: "Restaurant ABC",
    licenceType: "Food Primary",
    scheduledDate: isoFromNow(41, 11, 0),
    window: "Late morning · from 11:00 AM",
    kind: "education",
    status: "scheduled",
    focusAreas: [
      "Primary food service ratio (liquor secondary to meals)",
      "Approved floor plan and service area",
      "Signage and licence display",
    ],
    office: "Compliance & Enforcement — Vancouver Island",
    note: "Educational visit — an inspector will walk through Food Primary service expectations. No enforcement outcome results from an educational inspection.",
  },
]


/** Upcoming scheduled inspections for a licence, soonest first. */
export function getScheduledInspectionsForLicence(licenceId: string): ScheduledInspection[] {
  return SCHEDULED_INSPECTIONS.filter((i) => i.licenceId === licenceId).sort((a, b) =>
    a.scheduledDate < b.scheduledDate ? -1 : a.scheduledDate > b.scheduledDate ? 1 : 0,
  )
}

/** Closed inspection-related history for a licence (most recent first). */
export function getPastInspectionsForLicence(licenceId: string): ComplianceRecord[] {
  return COMPLIANCE_HISTORY.filter(
    (c) => c.licenceId === licenceId && (c.kind === "inspection" || c.kind === "contravention" || c.kind === "warning"),
  ).sort((a, b) => (a.date < b.date ? 1 : a.date > b.date ? -1 : 0))
}

/** The single next upcoming inspection for a licence, if any. */
export function getNextInspectionForLicence(licenceId: string): ScheduledInspection | undefined {
  return getScheduledInspectionsForLicence(licenceId).find((i) => daysUntil(i.scheduledDate) >= 0)
}
