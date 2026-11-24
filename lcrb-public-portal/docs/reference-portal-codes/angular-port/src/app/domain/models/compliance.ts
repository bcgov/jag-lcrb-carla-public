export type ContraventionOutcome =
  | "no-contravention"
  | "warning"
  | "monetary-penalty"
  | "suspension"
  | "cancellation"

export interface ComplianceRecord {
  id: string
  /** ISO date of the inspection / incident. */
  date: string
  licenceId: string
  establishmentName: string
  licenceType: string
  /** Category of the compliance event. */
  kind: "inspection" | "contravention" | "warning"
  /** Section of the Act / regulation, e.g. "s.75 LCLA". */
  section?: string
  /** What was observed / alleged. */
  description: string
  outcome: ContraventionOutcome
  /** Monetary penalty amount in CAD, if applicable. */
  penaltyAmount?: number
  /** Suspension length in days, if applicable. */
  suspensionDays?: number
  /** Issuing / inspecting officer (display name). */
  officer: string
  /** All items here are historical / closed. */
  status: "closed"
}

export type EnforcementBaseStatus = "awaiting-response"

export interface EnforcementAction {
  id: string
  /** LCRB enforcement file reference, e.g. "EA-2026-00142". */
  referenceNumber: string
  licenceId: string
  establishmentName: string
  licenceType: string
  /** ISO date the NEA was issued. */
  issuedDate: string
  /** Section of the Act alleged to be contravened. */
  section: string
  /** Short allegation title. */
  allegation: string
  /** Fuller description of the alleged contravention. */
  details: string
  /** The enforcement action LCRB proposes to take. */
  proposedAction: string
  /** ISO deadline for the licensee's written response. */
  responseDeadline: string
  baseStatus: EnforcementBaseStatus
}

export type CalendarEventType =
  | "renewal"
  | "fee"
  | "enforcement-deadline"
  | "hearing"
  | "inspection"

export type EventUrgency = "overdue" | "urgent" | "soon" | "upcoming"

export interface CalendarEvent {
  id: string
  type: CalendarEventType
  title: string
  /** ISO date (all-day). */
  date: string
  description: string
  licenceId?: string
  establishmentName?: string
  /** Deep link into the relevant portal area. */
  href: string
}

export type EnforcementStatus =
  | "awaiting-response"
  | "response-submitted"
  | "hearing-requested"
  | "hearing-scheduled"
  | "resolved"
