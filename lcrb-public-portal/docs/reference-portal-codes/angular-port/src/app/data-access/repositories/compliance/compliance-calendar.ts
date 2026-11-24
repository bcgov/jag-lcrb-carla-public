"use client"

/** Compliance calendar aggregator + iCal (.ics) export. */

import { MOCK_LICENCES } from "../licences/mock/licences"
import { ENFORCEMENT_ACTIONS } from "./mock/enforcement"
import { SCHEDULED_INSPECTIONS } from "../licences/mock/inspections"
import { INSPECTION_KIND_META } from "@domain/licences/inspections"
import { responseFor } from "./enforcement-store"
import { type CalendarEvent } from "@domain/models/compliance"
import { shiftDays } from "@domain/compliance/compliance-calendar"

/** Build the full, chronologically sorted event feed. */
export function buildCalendarEvents(): CalendarEvent[] {
  const events: CalendarEvent[] = []

  for (const lic of MOCK_LICENCES) {
    events.push({
      id: `renewal-${lic.id}`,
      type: "renewal",
      title: `${lic.type} renewal — ${lic.name}`,
      date: lic.expiryDate,
      description: `Licence #${lic.id} (${lic.name}) expires on this date. Submit your renewal before it lapses to avoid an interruption to operations.`,
      licenceId: lic.id,
      establishmentName: lic.name,
      href: `/licences/${lic.id}`,
    })
    events.push({
      id: `fee-${lic.id}`,
      type: "fee",
      title: `Annual fee due — ${lic.name}`,
      date: shiftDays(lic.expiryDate, -30),
      description: `Annual licensing fee for licence #${lic.id} (${lic.name}) is due 30 days before renewal.`,
      licenceId: lic.id,
      establishmentName: lic.name,
      href: `/licences/${lic.id}`,
    })
  }

  for (const inspection of SCHEDULED_INSPECTIONS) {
    const kind = INSPECTION_KIND_META[inspection.kind].label
    events.push({
      id: `inspection-${inspection.id}`,
      type: "inspection",
      title: `${kind} inspection — ${inspection.establishmentName}`,
      date: inspection.scheduledDate,
      description: `${inspection.window}. Conducted by ${inspection.office}${
        inspection.officer ? ` (${inspection.officer})` : ""
      }. Focus: ${inspection.focusAreas.join("; ")}.`,
      licenceId: inspection.licenceId,
      establishmentName: inspection.establishmentName,
      href: `/licences/${inspection.licenceId}?tab=inspections`,
    })
  }

  for (const action of ENFORCEMENT_ACTIONS) {
    const response = responseFor(action.id)
    const status = response?.status ?? action.baseStatus
    if (status === "awaiting-response") {
      events.push({
        id: `enf-${action.id}`,
        type: "enforcement-deadline",
        title: `Response due — ${action.referenceNumber}`,
        date: action.responseDeadline,
        description: `Written response deadline for enforcement action ${action.referenceNumber} (${action.allegation}) at ${action.establishmentName}.`,
        licenceId: action.licenceId,
        establishmentName: action.establishmentName,
        href: "/compliance",
      })
    }
    if (response?.hearingDate) {
      events.push({
        id: `hearing-${action.id}`,
        type: "hearing",
        title: `Enforcement hearing — ${action.referenceNumber}`,
        date: response.hearingDate,
        description: `Enforcement hearing for ${action.referenceNumber} (${action.allegation}) at ${action.establishmentName}.`,
        licenceId: action.licenceId,
        establishmentName: action.establishmentName,
        href: "/compliance",
      })
    }
  }

  return events.sort((a, b) => (a.date < b.date ? -1 : a.date > b.date ? 1 : 0))
}
