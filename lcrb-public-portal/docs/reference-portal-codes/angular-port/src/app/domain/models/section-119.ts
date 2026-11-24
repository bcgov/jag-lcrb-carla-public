/** Section 119 (First Nations cannabis) model: profile, status, signals, result and status labels. */

/** State of a nation's s.119 agreement with the Province. */
export type AgreementStatus = "signed" | "in-negotiation" | "none" | "unknown"

/** First Nations affiliation attached to a business account. */
export interface FirstNationProfile {
  /** Whether the business is First Nation owned / affiliated (self-declared). */
  affiliated: boolean
  /** Name of the Indigenous nation, if declared. */
  nationName?: string
  /** State of that nation's s.119 agreement with the Province. */
  agreementStatus: AgreementStatus
  /** Agreement reference on file, if signed. */
  agreementReference?: string
  /** ISO date the agreement was signed. */
  agreementDate?: string
  /** Whether cannabis premises sit on the nation's treaty/reserve lands. */
  premisesOnNationLands?: boolean
}


export type Section119Status =
  | "applies" // covered by a signed s.119 agreement
  | "likely-applies" // strong signals, agreement pending/unverified
  | "not-applicable" // no First Nations cannabis nexus
  | "insufficient-info" // can't tell from what was provided
  | "needs-review" // conflicting signals; staff must decide

export interface Section119Signal {
  /** Short label, e.g. "First Nation affiliation declared". */
  label: string
  /** Whether this signal points toward s.119 applying. */
  present: boolean
  /** One line explaining what was checked. */
  detail: string
}

export interface Section119Result {
  status: Section119Status
  /** 0-100 confidence in the provisional status. */
  confidence: number
  signals: Section119Signal[]
  /** Whether staff review is recommended before relying on this. */
  requiresStaffReview: boolean
  /** One-line rationale for the log. */
  rationale: string
}

export const SECTION_119_STATUS_META: Record<
  Section119Status,
  { label: string; tone: "positive" | "caution" | "neutral" | "info" }
> = {
  applies: { label: "s.119 applies", tone: "positive" },
  "likely-applies": { label: "s.119 likely applies", tone: "caution" },
  "not-applicable": { label: "s.119 not applicable", tone: "neutral" },
  "insufficient-info": { label: "Insufficient information", tone: "info" },
  "needs-review": { label: "Needs staff review", tone: "caution" },
}
