/** Fit & Proper screening engine (deterministic). */

import type { FitAndProperResult, FPFinding, FPPerson, FPRecommendation, FPEscalationCriterion, FPOutcome, FPProgram } from "@domain/models/screening"
import type { LegalEntityBuild } from "@domain/models/legal-entity"
import { ROLE_META } from "@domain/legal-entity/legal-entity"
import type { ExistingRelationship, TiedHouseDeclaration } from "@domain/models/tied-house"
import { ENFORCEMENT_ACTIONS, COMPLIANCE_HISTORY } from "../compliance/mock/enforcement"

export interface FitAndProperInput {
  title: string
  applicantName: string
  businessNumber: string
  involvesCannabis: boolean
  /** The kind of application. */
  applicationKind: "new-licence" | "transfer" | "other"
  leBuild: LegalEntityBuild | null
  existingRelationships: ExistingRelationship[]
  declarations: TiedHouseDeclaration[]
  ownedLicences: { id: string; type: string }[]
}


/** Stable non-negative hash - seeds the deterministic demo signals. */
function hashString(s: string): number {
  let h = 0
  for (let i = 0; i < s.length; i++) h = (h * 31 + s.charCodeAt(i)) | 0
  return Math.abs(h)
}

const worstOf = (a: FPOutcome, b: FPOutcome): FPOutcome => {
  const rank: Record<FPOutcome, number> = { clear: 0, attention: 1, flag: 2 }
  return rank[a] >= rank[b] ? a : b
}

/** Collapse the ownership/control structure into a de-duplicated people list. */
function extractPeople(build: LegalEntityBuild): FPPerson[] {
  const map = new Map<string, FPPerson>()
  for (const node of build.nodes) {
    if (node.external) continue // skip read-only tied-house overlay cards
    for (const p of node.people) {
      const key = p.name.toLowerCase()
      const role = ROLE_META[p.role]
      const existing = map.get(key)
      if (existing) {
        if (!existing.roles.includes(role)) existing.roles.push(role)
        if (!existing.entity.includes(node.legalName)) existing.entity += `, ${node.legalName}`
      } else {
        map.set(key, {
          name: p.name,
          roles: [role],
          entity: node.legalName,
          outcome: "clear",
          note: "Disclosed in the ownership/control structure; identity confirmed against records.",
        })
      }
    }
  }
  return [...map.values()]
}


export function computeFitAndProper(input: FitAndProperInput): FitAndProperResult {
  const now = new Date().toISOString()

  // Program + scope.
  const program: FPProgram = input.involvesCannabis ? "cannabis" : "liquor"
  const isNewOrTransfer =
    input.applicationKind === "new-licence" || input.applicationKind === "transfer"
  const applicable = program === "cannabis" ? true : isNewOrTransfer
  const escalationTarget =
    program === "cannabis" ? "the Security Programs Division (SPD)" : "an enhanced suitability review"

  if (!applicable) {
    return {
      applicable: false,
      program,
      escalationTarget,
      overallConfidence: 0,
      overallOutcome: "clear",
      summary: "Fit-and-proper pre-screen not applicable to this application type.",
      findings: [],
      people: [],
      recommendation: "clear-in-portal",
      recommendationRationale:
        "Out of scope — fit-and-proper screening runs on every cannabis application and on liquor new-licence and transfer applications.",
      escalationCriteria: [],
      tierCoverage: { tier1: 0, tier2: 0, tier3: 0 },
      generatedAt: now,
    }
  }

  const seed = hashString(`${input.businessNumber}|${input.applicantName}`)
  const findings: FPFinding[] = []

  const people = input.leBuild ? extractPeople(input.leBuild) : []


  // 1. Corporate registration & structure (BC Registry / OrgBook).
  if (input.leBuild) {
    const recordNodes = input.leBuild.nodes.filter((n) => !n.external && n.source === "record")
    const withReg = recordNodes.filter((n) => n.incorporationNumber || n.businessNumber)
    findings.push({
      id: "fp-structure",
      dimension: "business",
      label: "Corporate registration & structure",
      outcome: "clear",
      confidence: 93,
      tier: 1,
      finding: `Corporate structure resolved against BC Registry / OrgBook: ${recordNodes.length} registered ${
        recordNodes.length === 1 ? "entity" : "entities"
      } in the group, ${withReg.length} with a confirmed incorporation/business number.`,
      evidence: [
        {
          source: "BC Registry / OrgBook",
          detail: `${withReg.length} of ${recordNodes.length} entities carry a locked registration identifier.`,
        },
        {
          source: "Legal Entity build",
          detail: `${input.leBuild.nodes.length} nodes / ${input.leBuild.edges.length} relationships in the effective business group.`,
        },
      ],
    })
  } else {
    findings.push({
      id: "fp-structure",
      dimension: "business",
      label: "Corporate registration & structure",
      outcome: "attention",
      confidence: 62,
      tier: 1,
      finding:
        "No Legal Entity build is on file for this business yet, so the corporate structure could not be fully resolved in-portal.",
      evidence: [{ source: "Legal Entity build", detail: "No build found for this business number." }],
    })
  }

  // 2. Beneficial ownership & control.
  const hasTrust = input.leBuild?.nodes.some((n) => !n.external && n.type === "trust") ?? false
  const shareholdingEdges = input.leBuild?.edges.filter((e) => !e.external && e.type === "shareholding") ?? []
  const layered = shareholdingEdges.length > 2
  const ownershipComplex = hasTrust || layered
  findings.push({
    id: "fp-ownership",
    dimension: "business",
    label: "Beneficial ownership & control",
    outcome: ownershipComplex ? "attention" : "clear",
    confidence: ownershipComplex ? 78 : 90,
    tier: 1,
    finding: ownershipComplex
      ? `Multi-layer ownership detected${hasTrust ? " including a trust" : ""}. Beneficial owners are disclosed but the control chain warrants an analyst confirmation.`
      : "Ownership chain is disclosed and traces cleanly to the beneficial owners.",
    evidence: [
      {
        source: "Legal Entity build",
        detail: `${shareholdingEdges.length} shareholding relationship${shareholdingEdges.length === 1 ? "" : "s"} mapped${
          hasTrust ? "; a trust is present in the control chain" : ""
        }.`,
      },
    ],
  })

  // 3. Associated & linked entities (Tied House register + internal links).
  const associationCount = input.existingRelationships.length + input.declarations.length
  findings.push({
    id: "fp-associations",
    dimension: "business",
    label: "Associated & linked entities",
    outcome: associationCount > 0 ? "attention" : "clear",
    confidence: associationCount > 0 ? 80 : 88,
    tier: 1,
    finding:
      associationCount > 0
        ? `${associationCount} cross-tier / associated relationship${associationCount === 1 ? "" : "s"} declared. Confirm none constitute an undisclosed controlling interest.`
        : "No associated or cross-tier relationships declared or detected across LCRB records.",
    evidence: [
      {
        source: "Tied House register",
        detail: `${input.existingRelationships.length} existing relationship(s), ${input.declarations.length} active declaration(s).`,
      },
    ],
  })

  // 4. Regulatory & enforcement history (LCRB records).
  const ownedIds = new Set(input.ownedLicences.map((l) => l.id))
  const activeEnforcement = ENFORCEMENT_ACTIONS.filter((a) => ownedIds.has(a.licenceId))
  const contraventions = COMPLIANCE_HISTORY.filter(
    (c) => ownedIds.has(c.licenceId) && c.kind === "contravention",
  )
  const enforcementOutcome: FPOutcome =
    activeEnforcement.length > 0 ? "flag" : contraventions.length > 0 ? "attention" : "clear"
  findings.push({
    id: "fp-enforcement",
    dimension: "business",
    label: "Regulatory & enforcement history",
    outcome: enforcementOutcome,
    confidence: 91,
    tier: 1,
    finding:
      activeEnforcement.length > 0
        ? `${activeEnforcement.length} active enforcement action(s) against the group's licences — review before determining fit and proper.`
        : contraventions.length > 0
          ? `${contraventions.length} historical contravention(s) on file; all previously closed. No active enforcement.`
          : "No enforcement actions or contraventions on file for the group's licences.",
    evidence: [
      {
        source: "LCRB enforcement records",
        detail:
          activeEnforcement.length > 0
            ? activeEnforcement.map((a) => `${a.referenceNumber}: ${a.allegation}`).join("; ")
            : contraventions.length > 0
              ? contraventions.map((c) => c.description).slice(0, 3).join("; ")
              : "No records matched the group's licences.",
      },
    ],
    escalates: activeEnforcement.length > 0,
  })


  // 5. Declaration consistency.
  const consistencyGap = seed % 6 === 0 && people.length > 0
  findings.push({
    id: "fp-consistency",
    dimension: "people",
    label: "Declaration consistency",
    outcome: consistencyGap ? "attention" : "clear",
    confidence: consistencyGap ? 74 : 88,
    tier: 1,
    finding: consistencyGap
      ? `One disclosed individual (${people[seed % people.length].name}) appears in the structure without a matching personal history submission. Resolve with the applicant.`
      : `All ${people.length} disclosed individual${people.length === 1 ? "" : "s"} reconcile between the application, declarations, and the ownership structure.`,
    evidence: [
      {
        source: "Applicant declarations",
        detail: `${people.length} individual(s) cross-checked against the Legal Entity build and application forms.`,
      },
    ],
  })

  // 6. Personal history & screening records (LCRB / declarations).
  findings.push({
    id: "fp-personal-history",
    dimension: "people",
    label: "Personal history & screening records",
    outcome: "clear",
    confidence: 82,
    tier: 1,
    finding: `Personal history declarations reviewed for ${people.length} associated individual${
      people.length === 1 ? "" : "s"
    }; no adverse self-disclosures or prior LCRB screening failures on file.`,
    evidence: [
      { source: "Applicant declarations", detail: "Self-declared personal history reviewed; no disclosed adverse items." },
      { source: "LCRB records", detail: "No prior fit-and-proper refusal linked to these individuals." },
    ],
  })

  // 7. Open-source / adverse-media footprint (preliminary indicator).
  const openSourceHit = seed % 4 === 0
  const openSourcePerson = people.length > 0 ? people[(seed >> 3) % people.length].name : input.applicantName
  findings.push({
    id: "fp-open-source",
    dimension: "people",
    label: "Adverse-media / open-source footprint",
    outcome: openSourceHit ? "attention" : "clear",
    confidence: openSourceHit ? 66 : 79,
    tier: 1,
    finding: openSourceHit
      ? `A preliminary open-source reference possibly matching ${openSourcePerson} was detected. Run the adverse-media scan below to retrieve current media and verify identity before drawing any conclusion.`
      : "No preliminary adverse-media indicator for the disclosed individuals. Run the adverse-media scan below for a live open-source search of the business and associated people.",
    evidence: [
      {
        source: "Adverse-media scan (on-demand RAG)",
        detail: openSourceHit
          ? "Preliminary low-confidence indicator; run the live scan to retrieve sources and confirm identity."
          : "No standing indicator; a live open-source scan can be run on demand for current media.",
      },
    ],
  })

  // Deterministic demo signal, rare. Source of funding and undisclosed third-party control are named suitability factors for liquor.
  const financialConcern = seed % 7 === 0
  findings.push({
    id: "fp-financial-integrity",
    dimension: "business",
    label: "Financial integrity & source of funding",
    outcome: financialConcern ? "attention" : "clear",
    confidence: financialConcern ? 70 : 83,
    tier: 1,
    finding: financialConcern
      ? program === "cannabis"
        ? "The declared source of investment money could not be fully reconciled in-portal. Confirm the funding chain and that no undisclosed party or illicit proceeds fund the business, consistent with the Act's organized-crime-prevention purpose."
        : "The declared source of funding or a third-party financing arrangement could not be fully reconciled in-portal. Confirm the funding chain and that no undisclosed party controls the business."
      : program === "cannabis"
        ? "Declared source of all investment money reconciles with the ownership structure; no undisclosed financing or illicit-proceeds indicator detected."
        : "Declared source of funding reconciles with the ownership structure; no undisclosed financing or third-party control detected.",
    evidence: [
      {
        source:
          program === "cannabis"
            ? "Cannabis Screening Form / applicant declarations / BC Registry"
            : "Applicant declarations / BC Registry",
        detail: financialConcern
          ? "Funding declaration does not fully reconcile — confirm with the applicant."
          : "Funding declaration reconciles with the disclosed ownership.",
      },
    ],
    escalates: false,
  })

  // 9. Criminal-record dimension.
  const spdIntelSignal = program === "cannabis" && seed % 11 === 0
  const illicitStoreHistory = program === "cannabis" && seed % 13 === 0
  const adverseCriminalRecord = program === "liquor" && seed % 9 === 0

  if (program === "cannabis") {
    findings.push({
      id: "fp-restricted",
      dimension: "people",
      label: "Criminal record & police intelligence",
      outcome: spdIntelSignal ? "flag" : "clear",
      confidence: spdIntelSignal ? 58 : 70,
      tier: 3,
      finding: spdIntelSignal
        ? "An in-portal signal suggests a possible association that only restricted Tier-3 data (CPIC / PRIME BC / organized-crime intelligence) can confirm or clear. This cannot be resolved in-portal."
        : "No in-portal signal indicates a restricted-records check is required. Tier-3 systems (CPIC, PRIME BC, intelligence) are not accessed in-portal.",
      evidence: [
        {
          source: "SPD (Tier 3, restricted)",
          detail: spdIntelSignal
            ? "Escalation trigger met — restricted verification recommended."
            : "Not accessed. Referral only if an escalation criterion is met.",
        },
      ],
      escalates: spdIntelSignal,
    })

    // 9b (cannabis only) - unlicensed / illicit-market history.
    findings.push({
      id: "fp-illicit-market",
      dimension: "business",
      label: "Unlicensed / illicit-market history",
      outcome: illicitStoreHistory ? "flag" : "clear",
      confidence: illicitStoreHistory ? 60 : 81,
      tier: illicitStoreHistory ? 3 : 1,
      finding: illicitStoreHistory
        ? "An in-portal signal suggests a possible link to a previously unlicensed/illicit cannabis retail operation. This is a named enhanced-scrutiny trigger and warrants restricted verification before a determination."
        : "No indicator that the applicant or an associate operated an unlicensed/illicit cannabis store.",
      evidence: [
        {
          source: illicitStoreHistory ? "SPD (Tier 3, restricted)" : "LCRB records / open source",
          detail: illicitStoreHistory
            ? "Possible illicit-market history — enhanced scrutiny recommended."
            : "No illicit-market history matched to the applicant or associates.",
        },
      ],
      escalates: illicitStoreHistory,
    })
  } else {
    // Liquor - criminal record check & personal history (applicant-provided).
    findings.push({
      id: "fp-criminal-record",
      dimension: "people",
      label: "Criminal record check & personal history",
      outcome: adverseCriminalRecord ? "flag" : "clear",
      confidence: adverseCriminalRecord ? 72 : 84,
      tier: 1,
      finding: adverseCriminalRecord
        ? `A police information check for ${
            people.length > 0 ? people[(seed >> 2) % people.length].name : input.applicantName
          } returned an item that may bear on suitability. An enhanced suitability review should weigh it against the fit-and-proper standard before any determination.`
        : "Personal History Summaries and police information checks for the disclosed director(s)/officer(s) show no items bearing on suitability.",
      evidence: [
        {
          source: "Personal History Summary / police information check",
          detail: adverseCriminalRecord
            ? "A returned criminal-record item requires an enhanced suitability review."
            : "Applicant-provided checks reviewed; no adverse items relevant to the fit-and-proper standard.",
        },
      ],
      escalates: adverseCriminalRecord,
    })
  }

  const sharedCriteria: FPEscalationCriterion[] = [
    {
      label: "Active enforcement / serious contravention history",
      met: activeEnforcement.length > 0,
      detail:
        activeEnforcement.length > 0
          ? `${activeEnforcement.length} active enforcement action(s) on the group's licences.`
          : "No active enforcement against the group.",
    },
    {
      label: "Unresolved ownership or beneficial-owner gap",
      met: false, // disclosed multi-layer ownership is "attention", not a gap; only undisclosed control escalates
      detail: ownershipComplex
        ? "Ownership is multi-layer but disclosed — resolve in-portal, no gap."
        : "Beneficial ownership traces cleanly.",
    },
    {
      label: "Adverse open-source / media finding requiring verification",
      met: openSourceHit,
      detail: openSourceHit
        ? "A low-confidence open-source match should be identity-verified."
        : "No adverse open-source finding.",
    },
    {
      label: "Unresolvable declaration inconsistency",
      met: false, // demo consistency gaps are resolvable in-portal with the applicant
      detail: consistencyGap
        ? "A declaration gap was found but is resolvable in-portal with the applicant."
        : "Declarations reconcile.",
    },
    {
      label: "Financial integrity / source-of-funding concern",
      met: false, // a routine reconciliation gap is resolved in-portal with the applicant
      detail: financialConcern
        ? "A source-of-funding item was found but is resolvable in-portal with the applicant."
        : "Source of funding reconciles with the ownership structure.",
    },
  ]
  const escalationCriteria: FPEscalationCriterion[] =
    program === "cannabis"
      ? [
          ...sharedCriteria,
          {
            label: "Possible restricted-records / organized-crime concern (Tier 3)",
            met: spdIntelSignal,
            detail: spdIntelSignal
              ? "A signal was found that only SPD's restricted data can resolve."
              : "No signal requiring restricted Tier-3 data.",
          },
          {
            label: "Unlicensed / illicit cannabis-market history",
            met: illicitStoreHistory,
            detail: illicitStoreHistory
              ? "A possible link to an unlicensed/illicit cannabis operation warrants enhanced scrutiny."
              : "No unlicensed/illicit-market history matched to the applicant or associates.",
          },
        ]
      : [
          ...sharedCriteria,
          {
            label: "Adverse criminal record relevant to suitability",
            met: adverseCriminalRecord,
            detail: adverseCriminalRecord
              ? "A police information check returned an item that an enhanced suitability review should weigh."
              : "No criminal-record item bearing on suitability.",
          },
        ]

  // `refer-to-spd` is the shared "escalate" outcome; for liquor it reads as an enhanced suitability review.
  let recommendation: FPRecommendation
  let recommendationRationale: string
  const escalate =
    program === "cannabis"
      ? spdIntelSignal || illicitStoreHistory || activeEnforcement.length > 0
      : adverseCriminalRecord || activeEnforcement.length > 0
  if (escalate) {
    recommendation = "refer-to-spd"
    recommendationRationale =
      program === "cannabis"
        ? spdIntelSignal
          ? "A Tier-3 signal was detected that cannot be resolved with the data available in-portal. An SPD referral is warranted for restricted verification."
          : illicitStoreHistory
            ? "A possible link to an unlicensed/illicit cannabis operation was detected — a named enhanced-scrutiny trigger. An SPD referral is warranted for restricted verification."
            : "Active enforcement history combined with the fit-and-proper standard warrants restricted verification via SPD."
        : adverseCriminalRecord
          ? "A criminal-record item relevant to suitability was found. An enhanced suitability review should weigh it against the fit-and-proper standard before any determination."
          : "Active enforcement history combined with the fit-and-proper standard warrants an enhanced suitability review."
  } else if (openSourceHit || ownershipComplex || consistencyGap || associationCount > 0 || financialConcern) {
    recommendation = "monitor-in-portal"
    recommendationRationale =
      program === "cannabis"
        ? "Minor items to confirm were found, but all are resolvable with Tier-1 data in-portal. An SPD referral is not warranted at this stage."
        : "Minor items to confirm were found, but all are resolvable in-portal. An enhanced suitability review is not warranted at this stage."
  } else {
    recommendation = "clear-in-portal"
    recommendationRationale =
      program === "cannabis"
        ? "Tier-1 screening surfaced no signal requiring restricted data. Recommend proceeding without an SPD referral, subject to analyst review."
        : "Screening surfaced no signal bearing on suitability. Recommend proceeding without an enhanced suitability review, subject to analyst review."
  }

  /* Roll-ups */
  const tier1Findings = findings.filter((f) => f.tier === 1)
  const overallConfidence = Math.round(
    tier1Findings.reduce((a, f) => a + f.confidence, 0) / (tier1Findings.length || 1),
  )
  // Overall outcome ignores the informational Tier-3 boundary unless it fired.
  const overallOutcome = findings
    .filter((f) => f.tier !== 3 || f.escalates)
    .reduce<FPOutcome>((acc, f) => worstOf(acc, f.outcome), "clear")

  const total = findings.length || 1
  const tierCoverage = {
    tier1: Math.round((findings.filter((f) => f.tier === 1).length / total) * 100),
    tier2: Math.round((findings.filter((f) => f.tier === 2).length / total) * 100),
    tier3: Math.round((findings.filter((f) => f.tier === 3).length / total) * 100),
  }

  // Annotate the person tied to any people-level attention/flag.
  if (consistencyGap && people.length > 0) {
    const target = people[seed % people.length]
    target.outcome = worstOf(target.outcome, "attention")
    target.note = "Personal history submission missing — resolve with the applicant."
  }
  if (spdIntelSignal && people.length > 0) {
    const target = people[(seed >> 5) % people.length]
    target.outcome = worstOf(target.outcome, "flag")
    target.note = "Possible restricted-records concern — SPD verification recommended."
  }
  if (adverseCriminalRecord && people.length > 0) {
    const target = people[(seed >> 2) % people.length]
    target.outcome = worstOf(target.outcome, "flag")
    target.note = "Criminal-record item relevant to suitability — enhanced suitability review recommended."
  }

  const clearedCount = escalationCriteria.filter((c) => !c.met).length
  const escalationNoun = program === "cannabis" ? "an SPD referral" : "an enhanced suitability review"
  const escalationNounCap = `${escalationNoun.charAt(0).toUpperCase()}${escalationNoun.slice(1)}`
  const summary =
    recommendation === "refer-to-spd"
      ? `Fit-and-proper pre-screen found a signal that warrants ${escalationTarget}. ${clearedCount} of ${escalationCriteria.length} escalation criteria are clear; ${escalationNoun} is recommended for the remaining concern.`
      : recommendation === "monitor-in-portal"
        ? `Fit-and-proper pre-screen cleared ${clearedCount} of ${escalationCriteria.length} escalation criteria. Remaining items are resolvable in-portal — ${escalationNoun} is not needed.`
        : `Fit-and-proper pre-screen cleared all ${escalationCriteria.length} escalation criteria in-portal. ${escalationNounCap} is not needed, subject to analyst review.`

  return {
    applicable: true,
    program,
    escalationTarget,
    overallConfidence,
    overallOutcome,
    summary,
    findings,
    people,
    recommendation,
    recommendationRationale,
    escalationCriteria,
    tierCoverage,
    generatedAt: now,
  }
}
