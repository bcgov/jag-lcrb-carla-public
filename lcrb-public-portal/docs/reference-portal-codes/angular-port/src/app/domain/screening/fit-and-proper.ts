import { type FPProgram, type FPTier, type FPOutcome, type FPRecommendation } from "@domain/models/screening"


export const FP_TIER_META: Record<
  FPTier,
  { label: string; short: string; description: string; tone: "positive" | "caution" | "danger" }
> = {
  1: {
    label: "Tier 1 — In-portal",
    short: "Tier 1",
    description:
      "Available to LCRB now: applicant declarations, BC Registry / OrgBook, LCRB licensing & enforcement records, and open-source review.",
    tone: "positive",
  },
  2: {
    label: "Tier 2 — Access request",
    short: "Tier 2",
    description: "Available to LCRB via an access request: JUSTIN, CORNET.",
    tone: "caution",
  },
  3: {
    label: "Tier 3 — SPD only",
    short: "Tier 3",
    description:
      "Restricted justice/police & organized-crime intelligence (CPIC, PRIME BC, CFSEU, CSU). Accessible only through an SPD referral — never assessed in-portal.",
    tone: "danger",
  },
}

export const FP_OUTCOME_META: Record<
  FPOutcome,
  { label: string; tone: "positive" | "caution" | "danger" }
> = {
  clear: { label: "Clear", tone: "positive" },
  attention: { label: "Attention", tone: "caution" },
  flag: { label: "Flag", tone: "danger" },
}

export const FP_RECOMMENDATION_META: Record<
  FPRecommendation,
  { label: string; description: string; tone: "positive" | "caution" | "danger" }
> = {
  "clear-in-portal": {
    label: "Clear in-portal",
    description:
      "Tier-1 screening surfaced no signal requiring restricted data. Recommend proceeding without an SPD referral, subject to analyst review.",
    tone: "positive",
  },
  "monitor-in-portal": {
    label: "Resolve in-portal",
    description:
      "Minor items to confirm, but none require restricted data. Recommend resolving with the applicant in-portal rather than referring to SPD.",
    tone: "caution",
  },
  "refer-to-spd": {
    label: "Refer to SPD",
    description:
      "A signal was found that only restricted Tier-3 data can resolve. An SPD referral is warranted.",
    tone: "danger",
  },
}

export function fpConfidenceBand(confidence: number): "high" | "medium" | "low" {
  if (confidence >= 80) return "high"
  if (confidence >= 55) return "medium"
  return "low"
}


/** Program-aware label for a recommendation; `refer-to-spd` means an SPD referral for cannabis and an enhanced suitability review for liquor. */
export function fpRecommendationLabel(rec: FPRecommendation, program: FPProgram): string {
  if (rec === "refer-to-spd") {
    return program === "cannabis" ? "Refer to SPD" : "Refer for enhanced review"
  }
  return FP_RECOMMENDATION_META[rec].label
}

/** Program-aware heading for the escalation ruleset. */
export function fpEscalationHeading(program: FPProgram): string {
  return program === "cannabis" ? "SPD escalation criteria" : "Enhanced-review escalation criteria"
}

/** Short program label for chips/headers, e.g. "Fit & proper (liquor)". */
export function fpProgramLabel(program: FPProgram): string {
  return program === "cannabis" ? "cannabis" : "liquor"
}
