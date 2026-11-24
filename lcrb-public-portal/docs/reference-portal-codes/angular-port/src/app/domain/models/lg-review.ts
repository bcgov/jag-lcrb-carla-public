/** Local Government / Indigenous Nation review model: statuses, decisions, contacts, documents, history and the referred application. */

export type LgReviewStatus =
  | "pending-review"
  | "documents-requested"
  | "community-review"
  | "no-comment"
  | "reject"
  | "completed"
  /** Applicant withdrew the file. Terminal; any recorded comment stays on file. */
  | "cancelled"

export const LG_STATUS_LABELS: Record<LgReviewStatus, string> = {
  "pending-review": "Pending Review",
  "documents-requested": "Documents Requested",
  "community-review": "Community Review",
  "no-comment": "No Comment",
  reject: "Reject",
  completed: "Comment Submitted",
  cancelled: "Cancelled by applicant",
}

/** Statuses in which the referral no longer needs anything from the jurisdiction. */
export const LG_TERMINAL_STATUSES: LgReviewStatus[] = ["no-comment", "reject", "completed", "cancelled"]

/** Set when the applicant withdraws a referred application; overlaid from the shared cancellation service, never by a local-government action. */
export interface LgCancellation {
  cancelledAt: string
  cancelledBy: string
  reason: string
  /** The jurisdiction's status at the moment of cancellation (work is kept). */
  priorStatus: Exclude<LgReviewStatus, "cancelled">
}

/** The decisions a local government can record on an application. */
export type LgDecision = "no-comment" | "community-review" | "reject" | "documents-requested"

export interface LgContact {
  name: string
  role: string
  phone: string
  email: string
}

export interface LgDocument {
  name: string
  uploadedBy: string
  uploadedDate: string
}

export interface LgHistoryEntry {
  label: string
  actor: string
  date: string
}

export interface LgApplication {
  jobNumber: string
  establishmentName: string
  address: string
  city: string
  postalCode: string
  applicantName: string
  businessName: string
  licenceType: string
  applicationType: string
  submittedDate: string
  /** The 90-day clock replacement: when comment is due back to LCRB */
  commentDueDate: string
  status: LgReviewStatus
  jurisdiction: string
  riskLevel: "Low" | "Medium" | "High"
  contraventions: number
  incidents: number
  description: string
  hours: { day: string; hours: string }[]
  contacts: LgContact[]
  documents: LgDocument[]
  history: LgHistoryEntry[]
  /** Coordinates for the regional licence map */
  lat: number
  lng: number
  /** Present only once the applicant has withdrawn the file. */
  cancellation?: LgCancellation
}

export interface LgCommentingPreference {
  /** True when the jurisdiction has unsubscribed from commenting. */
  optedOut: boolean
  /** ISO timestamp of the last change (audit display). */
  updatedAt: string | null
  /** Display name of the account owner who made the change (audit display). */
  updatedBy: string | null
  /** ISO date (yyyy-mm-dd) the current opt-out took effect; later referrals skip the comment hold, earlier ones keep their deadline. */
  effectiveFrom: string | null
}

export type ReportCadence = "daily" | "weekly"

export interface LgReportFilters {
  /** LG review status or "all". */
  status: LgReviewStatus | "all"
  /** Free-text search, or empty string. */
  query: string
}
