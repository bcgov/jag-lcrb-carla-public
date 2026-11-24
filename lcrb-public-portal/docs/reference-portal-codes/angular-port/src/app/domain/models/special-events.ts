/** Shared SEP data store for all roles. TODO: in-memory mock data; replace the helpers with API calls. */

export type SepStatus =
  | "submitted"
  | "under-review"
  | "changes-requested"
  | "approved"
  | "denied"
  | "auto-approved"
  | "cancelled"

export interface SepHistoryEntry {
  date: string
  actor: string
  action: string
  note?: string
}

export interface SepCondition {
  id: string
  text: string
  /** Standard conditions are pre-set; custom ones are added by the reviewer */
  standard: boolean
}

export interface SepRecord {
  id: string
  eventName: string
  eventType: string
  /** Applicant (organizer) details */
  applicantName: string
  applicantOrg?: string
  applicantPhone: string
  applicantEmail: string
  /** Event logistics */
  eventStart: string // ISO date
  eventEnd: string // ISO date
  startTime: string
  endTime: string
  venueName: string
  address: string
  city: string
  jurisdiction: string
  /** Approximate venue coordinates for the events map */
  lat: number
  lng: number
  /** Attendance drives the auto-approval threshold (500) */
  expectedAttendance: number
  minorsExpected: boolean
  /** Liquor service */
  serviceType: string
  /** Workflow */
  status: SepStatus
  submittedDate: string
  assignedOfficer?: string
  conditions: SepCondition[]
  history: SepHistoryEntry[]
  /** Free-text description for context */
  description: string
  siteplanProvided: boolean
  /** Additional service details from the application form; optional so older seeded fixtures still typecheck. */
  liquorTypes?: string[]
  estimatedDrinks?: number
  hasSecurity?: boolean
  securityStaffCount?: number
  /** Why this permit was referred instead of auto-approved; empty for auto-approved permits and seeded fixtures. */
  referralReasons?: string[]
}

export interface ReferralInput {
  attendance: number
  endTime?: string
  startTime?: string
  estimatedDrinks?: number
  eventType?: string
  hasSecurity?: boolean
  isPublicProperty?: boolean
}

export interface ReferralAssessment {
  /** True when no rule fired and the permit can be auto-approved. */
  autoApprove: boolean
  /** Human-readable reasons, one per rule that fired. Empty when autoApprove. */
  reasons: string[]
}
