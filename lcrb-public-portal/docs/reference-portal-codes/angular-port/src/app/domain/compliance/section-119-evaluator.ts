/** Advisory check of whether a First Nations s.119 agreement covers a cannabis application. TODO: resolve `firstNation` from real registers. */

import type {
  FirstNationProfile,
  Section119Result,
  Section119Signal,
  Section119Status,
} from "@domain/models/section-119"

export interface Section119Input {
  /** First Nations profile on the business, if any. */
  firstNation?: FirstNationProfile
  /** Whether the application/holdings involve cannabis at all. */
  involvesCannabis: boolean
  /** Optional applicant self-declaration captured on the form. */
  selfDeclaredIndigenousNation?: boolean
}

export function evaluateSection119(input: Section119Input): Section119Result {
  const { firstNation, involvesCannabis, selfDeclaredIndigenousNation } = input

  const affiliated = Boolean(firstNation?.affiliated || selfDeclaredIndigenousNation)
  const onLands = Boolean(firstNation?.premisesOnNationLands)
  const agreementStatus = firstNation?.agreementStatus ?? (affiliated ? "unknown" : "none")

  const signals: Section119Signal[] = [
    {
      label: "Cannabis application",
      present: involvesCannabis,
      detail: involvesCannabis
        ? "Application involves cannabis — s.119 can apply."
        : "No cannabis nexus — s.119 does not apply (liquor only).",
    },
    {
      label: "First Nation affiliation",
      present: affiliated,
      detail: affiliated
        ? `Declared/known affiliation${firstNation?.nationName ? `: ${firstNation.nationName}` : ""}.`
        : "No First Nation affiliation on file.",
    },
    {
      label: "Premises on nation lands",
      present: onLands,
      detail: onLands
        ? "Premises located on the nation's treaty/reserve lands."
        : "Premises not identified as on nation lands (or unknown).",
    },
    {
      label: "Signed s.119 agreement on file",
      present: agreementStatus === "signed",
      detail:
        agreementStatus === "signed"
          ? `Agreement ${firstNation?.agreementReference ?? "on file"}${firstNation?.agreementDate ? ` (${firstNation.agreementDate})` : ""}.`
          : agreementStatus === "in-negotiation"
            ? "Agreement in negotiation — not yet signed."
            : agreementStatus === "unknown"
              ? "Agreement status unknown — not verified."
              : "No s.119 agreement on file.",
    },
  ]

  let status: Section119Status
  let confidence: number
  let rationale: string

  if (!involvesCannabis) {
    status = "not-applicable"
    confidence = 96
    rationale = "No cannabis nexus; s.119 is a cannabis-only provision."
  } else if (!affiliated) {
    status = "not-applicable"
    confidence = 82
    rationale = "Cannabis application with no First Nation affiliation on file."
  } else if (affiliated && onLands && agreementStatus === "signed") {
    status = "applies"
    confidence = 90
    rationale = "First Nation cannabis on nation lands with a signed s.119 agreement on file."
  } else if (affiliated && (onLands || agreementStatus === "in-negotiation")) {
    status = "likely-applies"
    confidence = 62
    rationale =
      agreementStatus === "in-negotiation"
        ? "First Nation cannabis nexus with an agreement in negotiation — confirm coverage."
        : "First Nation cannabis on nation lands but agreement not verified — confirm coverage."
  } else if (affiliated && agreementStatus === "unknown") {
    status = "needs-review"
    confidence = 45
    rationale = "First Nation cannabis nexus but agreement and land status are unverified."
  } else {
    status = "insufficient-info"
    confidence = 40
    rationale = "Some First Nation signals present but not enough to determine s.119 coverage."
  }

  const requiresStaffReview = status !== "not-applicable"

  return { status, confidence, signals, requiresStaffReview, rationale }
}
