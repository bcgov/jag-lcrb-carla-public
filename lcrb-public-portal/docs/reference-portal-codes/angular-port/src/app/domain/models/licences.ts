/** An endorsement attached to a licence; permanent ones stay with the licence, temporary ones (e.g. a Temporary Use Area) carry an `expires` date. */
export interface Endorsement {
  name: string
  kind: "permanent" | "temporary"
  /** ISO date the endorsement was granted. */
  granted: string
  /** ISO date it lapses; temporary endorsements only. */
  expires?: string
  /** One-line summary of what it authorizes. */
  summary: string
}

export interface Licence {
  id: string
  type: string
  name: string
  status: string
  expiryDate: string
  address: string
  phone: string
  email: string
  operatedBy: string
  licencedTo: string
  flags: string[]
  category: "owned" | "operated"
  endorsements?: Endorsement[]
}

/** The kind of inspection C&E has scheduled. */
export type InspectionKind =
  | "routine"
  | "follow-up"
  | "pre-opening"
  | "complaint"
  | "education"

/** Lifecycle of a scheduled inspection from the licensee's point of view. */
export type ScheduledInspectionStatus =
  | "scheduled"
  | "confirmed"
  | "rescheduled"

export interface ScheduledInspection {
  id: string
  licenceId: string
  establishmentName: string
  licenceType: string
  /** ISO datetime the inspection is scheduled to begin. */
  scheduledDate: string
  /** Human window/label for when to expect the inspector. */
  window: string
  kind: InspectionKind
  status: ScheduledInspectionStatus
  /** What the inspector intends to review. */
  focusAreas: string[]
  /** Inspecting officer, where disclosed. */
  officer?: string
  /** C&E regional office coordinating the visit. */
  office: string
  /** Any note to the licensee (preparation, follow-up context, etc.). */
  note?: string
  /** True when the licensee is asked to confirm attendance / a contact. */
  confirmationRequested?: boolean
}

/** `none` = this domain is not capped (liquor); it is tallied for context only. */
export type CapStatus = "none" | "under" | "near" | "at" | "over"
