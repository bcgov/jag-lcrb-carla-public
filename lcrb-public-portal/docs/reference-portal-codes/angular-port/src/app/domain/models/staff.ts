import type { FitAndProperResult } from "./screening"

/** The distinct checks the agent performs. */
export type AgentCheckKind =
  | "licence-cap"
  | "section-119"
  | "consistency"
  | "completeness"
  | "tied-house"
  | "fit-and-proper"

/** Outcome of a single check. */
export type CheckOutcome = "clear" | "attention" | "flag"

export interface AgentCheck {
  kind: AgentCheckKind
  /** Human label, e.g. "Licence cap determination". */
  label: string
  outcome: CheckOutcome
  /** 0-100 confidence the agent has in this check's finding. */
  confidence: number
  /** One-line finding for the staff log. */
  finding: string
  /** The concrete data points the agent inspected (audit trail). */
  observations: string[]
}

/** Staff queues a case can be routed to in Dynamics. */
export type DynamicsQueue =
  | "beyond-licence-cap"
  | "section-119-review"
  | "enhanced-review"
  | "standard-processing"

/** The full agent log attached to a submission + synced to Dynamics. */
export interface AgentLog {
  /** Model + version that produced the narrative, for auditability. */
  model: string
  generatedAt: string
  /** Overall confidence (0-100) across all checks. */
  overallConfidence: number
  /** Short triage summary for staff. */
  summary: string
  /** Individual checks with per-check confidence. */
  checks: AgentCheck[]
  /** Queue the agent recommends this case be routed to. */
  recommendedQueue: DynamicsQueue
  /** Why that queue was chosen. */
  queueRationale: string
  /** Whether a human MUST review (agent never auto-decides these). */
  requiresHumanReview: boolean
  /** Fit-and-proper pre-screen for cannabis applications: advisory Tier-1 screening of the business and associates with an SPD-referral recommendation; omitted for out-of-scope types. */
  fitAndProper?: FitAndProperResult
}
