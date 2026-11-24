/** Fit & Proper screening model: an advisory Tier-1 pre-screen that clears routine cases in-portal; Tier-3 (SPD-only) systems are never simulated and an LCRB analyst decides. */

/** Screening program: cannabis Tier-3 signals go to the SPD; liquor is resolved in-portal with an internal enhanced review. */
export type FPProgram = "cannabis" | "liquor"

/** Data-accessibility tier a finding was drawn from (see study). */
export type FPTier = 1 | 2 | 3

/** The two screening dimensions: the business itself, and the people behind it. */
export type FPDimension = "business" | "people"

/** Outcome of a single fit-and-proper finding. */
export type FPOutcome = "clear" | "attention" | "flag"

/** A cited source the finding rests on (audit trail). */
export interface FPEvidence {
  /** Where the data came from, e.g. "BC Registry / OrgBook". */
  source: string
  /** What was observed there. */
  detail: string
}

/** A single fit-and-proper finding within a dimension. */
export interface FPFinding {
  id: string
  dimension: FPDimension
  /** Human label, e.g. "Beneficial ownership & control". */
  label: string
  outcome: FPOutcome
  /** 0-100 confidence in this finding, given the data available in-portal. */
  confidence: number
  /** The accessibility tier of the data this finding used. */
  tier: FPTier
  /** One-line finding for the staff log. */
  finding: string
  /** The concrete data points inspected. */
  evidence: FPEvidence[]
  /** True when this signal alone is a reason to consider an SPD referral (only Tier-3 data can resolve it). */
  escalates?: boolean
}

/** A person extracted from the ownership/control structure and screened. */
export interface FPPerson {
  name: string
  /** Roles held across the structure, e.g. ["Director", "Shareholder"]. */
  roles: string[]
  /** The entity they are attached to. */
  entity: string
  outcome: FPOutcome
  note: string
}

/** The portal's recommendation on SPD referral. */
export type FPRecommendation = "clear-in-portal" | "monitor-in-portal" | "refer-to-spd"

/** A defined criterion that governs whether a case should escalate to SPD. */
export interface FPEscalationCriterion {
  label: string
  /** Whether this criterion is triggered by the current application. */
  met: boolean
  detail: string
}

/** The full fit-and-proper screening result attached to the agent log. */
export interface FitAndProperResult {
  /** Screening runs only for in-scope applications: every cannabis application and liquor new-licence / transfer applications. */
  applicable: boolean
  /** Which regulatory program this screen ran under. */
  program: FPProgram
  /** Where a genuine escalation goes: an SPD referral for cannabis, an internal enhanced suitability review for liquor. */
  escalationTarget: string
  /** Overall confidence (0-100) across in-portal findings. */
  overallConfidence: number
  overallOutcome: FPOutcome
  /** Plain-language triage summary for staff. */
  summary: string
  findings: FPFinding[]
  /** People screened, derived from the ownership/control structure. */
  people: FPPerson[]
  recommendation: FPRecommendation
  recommendationRationale: string
  /** The escalation ruleset and which criteria the case triggered. */
  escalationCriteria: FPEscalationCriterion[]
  /** Share of findings resolvable at each tier (coverage snapshot). */
  tierCoverage: { tier1: number; tier2: number; tier3: number }
  generatedAt: string
}
