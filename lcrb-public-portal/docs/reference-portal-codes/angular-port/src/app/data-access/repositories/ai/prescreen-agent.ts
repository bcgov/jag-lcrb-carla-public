"use client"

/** Pre-screening agent orchestrator (client-side). */

import type { AgentCheck, AgentLog, DynamicsQueue } from "@domain/models/staff"
import type { ApplicationAssessment } from "../applications/submissions"
import type { FitAndProperResult } from "@domain/models/screening"
import { fpRecommendationLabel, fpProgramLabel } from "@domain/screening/fit-and-proper"
import { computeFitAndProper } from "../screening/fit-and-proper-engine"
import { computeLicenceCap, type CapEngineInput, type LicenceCapResult } from "../licences/licence-cap-engine"
import { evaluateSection119 } from "@domain/compliance/section-119-evaluator"
import type { Section119Result } from "@domain/models/section-119"
import type { FirstNationProfile } from "@domain/models/section-119"

/** The shared linkage context both the sync (deterministic) and async (AI) agent paths need */
export interface PreScreenContextInput extends CapEngineInput {
  title: string
  /** The kind of application, if the caller already knows it. */
  applicationKind?: "new-licence" | "transfer" | "other"
  /** Business number (BN) - seeds the deterministic fit-and-proper screen. */
  businessNumber?: string
  /** First Nations profile on the business, if any. */
  firstNation?: FirstNationProfile
  /** Applicant self-declaration of Indigenous nation, if captured on the form. */
  selfDeclaredIndigenousNation?: boolean
}

export interface PreScreenAgentInput extends PreScreenContextInput {
  flowId: string
  applicant: string
  summary: { label: string; value: string }[]
  documentChecks?: { label: string; status: string; summary: string }[]
}

export interface PreScreenAgentResult {
  agentLog: AgentLog
  /** The staff-facing assessment (base fields), reused by the Dynamics sync. */
  assessment: ApplicationAssessment | null
  cap: LicenceCapResult
  section119: Section119Result
}


function capCheck(cap: LicenceCapResult): AgentCheck {
  // Only cannabis is capped. Liquor is reported for context.
  const worst = cap.cannabis.status

  const outcome = worst === "over" || worst === "at" ? "flag" : worst === "near" ? "attention" : "clear"
  // Confidence only degrades for ambiguity in the capped domain.
  const cannabisLinks = cap.contributingLicences.filter((c) => c.domain === "cannabis")
  const hasTiedHouse = cannabisLinks.some((c) => c.source === "tied-house")
  const hasLinked = cannabisLinks.some((c) => c.source !== "direct")
  const confidence = hasTiedHouse ? 60 : hasLinked ? 82 : 94

  return {
    kind: "licence-cap",
    label: "Cannabis licence cap determination",
    outcome,
    confidence,
    finding: cap.rationale,
    observations: [
      `Cannabis: ${cap.cannabis.total}/${cap.cannabis.cap} (${cap.cannabis.direct} direct, ${cap.cannabis.linked} linked)`,
      `Liquor: ${cap.liquor.total} in group (${cap.liquor.direct} direct, ${cap.liquor.linked} linked) — no cap applies`,
      `Effective group: ${cap.groupedParties.map((p) => p.name).join(", ")}`,
    ],
  }
}

function fitAndProperCheck(fp: FitAndProperResult): AgentCheck {
  // The check outcome mirrors the recommendation: an escalation is a flag, an in-portal item is attention, a full clear is clear.
  const outcome =
    fp.recommendation === "refer-to-spd"
      ? "flag"
      : fp.recommendation === "monitor-in-portal"
        ? "attention"
        : "clear"
  const coverageNote =
    fp.program === "cannabis"
      ? `Tier-1 in-portal coverage: ${fp.tierCoverage.tier1}% of checks.`
      : `All checks resolved in-portal from applicant-provided and registry data.`
  return {
    kind: "fit-and-proper",
    label: `Fit & proper (${fpProgramLabel(fp.program)})`,
    outcome,
    confidence: fp.overallConfidence,
    finding: `${fpRecommendationLabel(fp.recommendation, fp.program)}: ${fp.recommendationRationale}`,
    observations: [
      `Screened ${fp.people.length} associated individual(s) and the business across ${fp.findings.length} checks.`,
      ...fp.escalationCriteria.filter((c) => c.met).map((c) => `Escalation trigger: ${c.label} — ${c.detail}`),
      coverageNote,
    ],
  }
}

function section119Check(s119: Section119Result): AgentCheck {
  const outcome =
    s119.status === "not-applicable" || s119.status === "applies"
      ? s119.status === "applies"
        ? "attention"
        : "clear"
      : s119.status === "needs-review"
        ? "flag"
        : "attention"

  return {
    kind: "section-119",
    label: "Section 119 (First Nations cannabis)",
    outcome,
    confidence: s119.confidence,
    finding: s119.rationale,
    observations: s119.signals.map((s) => `${s.label}: ${s.present ? "yes" : "no"} — ${s.detail}`),
  }
}


function routeQueue(
  cap: LicenceCapResult,
  s119: Section119Result,
  involvesCannabis: boolean,
  narrativeChecks: AgentCheck[],
  scope: { capApplies: boolean; s119Applies: boolean } = { capApplies: true, s119Applies: true },
): { queue: DynamicsQueue; rationale: string } {
  // Only the cannabis domain is capped; liquor status is always "none".
  const overCap = scope.capApplies && (cap.cannabis.status === "over" || cap.cannabis.status === "at")
  if (overCap) {
    return {
      queue: "beyond-licence-cap",
      rationale: "Effective business group is at or over the 8-licence cannabis retail cap.",
    }
  }
  if (scope.s119Applies && involvesCannabis && s119.status !== "not-applicable") {
    return {
      queue: "section-119-review",
      rationale: `First Nations cannabis nexus detected (${s119.status}). Confirm s.119 agreement coverage.`,
    }
  }
  const capConcern = scope.capApplies && (cap.cannabis.status === "near" || cap.requiresStaffReview)
  const hasConcern =
    capConcern || narrativeChecks.some((c) => c.outcome === "flag" || c.outcome === "attention")
  if (hasConcern) {
    return {
      queue: "enhanced-review",
      rationale: "Elevated signals (near-cap, tied-house links, or consistency/completeness concerns) warrant a closer look.",
    }
  }
  return { queue: "standard-processing", rationale: "No elevated signals detected — route through the normal workflow." }
}



interface PreScreenCore {
  cap: LicenceCapResult
  section119: Section119Result
  involvesCannabis: boolean
  fitAndProper: FitAndProperResult
  /** Whether the licence cap check is in scope for this application type. */
  capApplies: boolean
  /** Whether the Section 119 check is in scope for this application type. */
  s119Applies: boolean
}

/** Infer the application kind for fit-and-proper applicability. */
export function detectApplicationKind(
  title: string,
  explicit?: "new-licence" | "transfer" | "other",
): "new-licence" | "transfer" | "other" {
  if (explicit) return explicit
  const t = title.toLowerCase()
  if (/transfer|change of ownership|change in ownership/.test(t)) return "transfer"

  // Check "not a new licence" keywords FIRST.
  if (
    /endorsement|amendment|amend|renewal|renew|reinstat|change (to|in|of) |relocat|structural change|floor plan|hours of (liquor )?service|name change|add(ing)? (a )?(lounge|patio|area)|patio|temporary (use|change)|special occasion|permanent change|representative|manager|third party|due diligence/.test(
      t,
    )
  ) {
    return "other"
  }

  // "New ... licence", "Apply for a ... licence", "... licence application"
  if (/\bnew\b|apply for|application for a|licence application|license application/.test(t)) {
    return "new-licence"
  }
  return "other"
}

/** Does the 8-licence cannabis cap apply to this application? */
export function licenceCapApplies(kind: "new-licence" | "transfer" | "other", involvesCannabis: boolean): boolean {
  if (!involvesCannabis) return false
  return kind === "new-licence" || kind === "transfer"
}

/** Does Section 119 (First Nations cannabis agreements) apply? */
export function section119Applies(involvesCannabis: boolean): boolean {
  return involvesCannabis
}

/** Run the deterministic engines (licence cap + Section 119 + fit & proper). */
function computeCore(input: PreScreenContextInput): PreScreenCore {
  const cap = computeLicenceCap(input)
  const involvesCannabis =
    cap.cannabis.total > 0 ||
    input.ownedLicences.some((l) => /cannabis/i.test(l.type)) ||
    /cannabis/i.test(input.title)
  const section119 = evaluateSection119({
    firstNation: input.firstNation,
    involvesCannabis,
    selfDeclaredIndigenousNation: input.selfDeclaredIndigenousNation,
  })
  const applicationKind = detectApplicationKind(input.title, input.applicationKind)
  const fitAndProper = computeFitAndProper({
    title: input.title,
    applicantName: input.applicantName,
    businessNumber: input.businessNumber ?? input.leBuild?.businessNumber ?? input.applicantName,
    involvesCannabis,
    applicationKind,
    leBuild: input.leBuild ?? null,
    existingRelationships: input.existingRelationships ?? [],
    declarations: input.declarations ?? [],
    ownedLicences: input.ownedLicences,
  })
  // `involvesCannabis` is broad (the group merely HOLDING a cannabis licence engages s.119 / fit-and-proper).
  const addsCannabisLicence = /cannabis/i.test(input.title)
  return {
    cap,
    section119,
    involvesCannabis,
    fitAndProper,
    capApplies: licenceCapApplies(applicationKind, addsCannabisLicence),
    s119Applies: section119Applies(involvesCannabis),
  }
}

/** Assemble the final AgentLog from the deterministic core + any AI narrative. */
function assembleLog(
  core: PreScreenCore,
  opts: { narrativeChecks?: AgentCheck[]; agentSummary?: string; model?: string } = {},
): AgentLog {
  const { cap, section119, involvesCannabis, fitAndProper, capApplies, s119Applies } = core
  const narrativeChecks = opts.narrativeChecks ?? []
  // Only include checks that are actually in scope for this application type.
  const fpCheck = fitAndProper.applicable ? [fitAndProperCheck(fitAndProper)] : []
  const checks: AgentCheck[] = [
    ...(capApplies ? [capCheck(cap)] : []),
    ...(s119Applies ? [section119Check(section119)] : []),
    ...fpCheck,
    ...narrativeChecks,
  ]
  const overallConfidence = Math.round(checks.reduce((a, c) => a + c.confidence, 0) / (checks.length || 1))
  const { queue, rationale } = routeQueue(cap, section119, involvesCannabis, narrativeChecks, {
    capApplies,
    s119Applies,
  })
  // A fit-and-proper SPD referral always needs a human; so does any flag.
  const fpNeedsReview = fitAndProper.applicable && fitAndProper.recommendation === "refer-to-spd"
  const requiresHumanReview =
    (capApplies && cap.requiresStaffReview) ||
    (s119Applies && section119.requiresStaffReview) ||
    fpNeedsReview ||
    checks.some((c) => c.outcome === "flag")
  const fpSentence = fitAndProper.applicable
    ? ` Fit & proper (${fpProgramLabel(fitAndProper.program)}): ${fpRecommendationLabel(
        fitAndProper.recommendation,
        fitAndProper.program,
      ).toLowerCase()}.`
    : ""
  // Only mention engines that actually ran, so staff aren't told about a cap or s.119 assessment that doesn't apply.
  const capSentence = capApplies ? ` ${cap.rationale}` : ""
  const s119Sentence = s119Applies ? ` Section 119: ${section119.rationale}` : ""
  const scopeSentence =
    !capApplies && !s119Applies && !fitAndProper.applicable
      ? " Licence cap, Section 119, and fit & proper screening are not applicable to this application type."
      : ""
  const summary =
    opts.agentSummary ||
    `Pre-screen complete.${capSentence}${s119Sentence}${fpSentence}${scopeSentence} Recommended queue: ${queue.replace(/-/g, " ")}.`

  return {
    model: opts.model ?? "deterministic-rules",
    generatedAt: new Date().toISOString(),
    overallConfidence,
    summary,
    checks,
    recommendedQueue: queue,
    queueRationale: rationale,
    requiresHumanReview,
    fitAndProper: fitAndProper.applicable ? fitAndProper : undefined,
  }
}

/** Synchronous, deterministic-only pre-screen. */
export function buildDeterministicAgentLog(
  input: PreScreenContextInput,
): { agentLog: AgentLog; cap: LicenceCapResult; section119: Section119Result } {
  const core = computeCore(input)
  return { agentLog: assembleLog(core), cap: core.cap, section119: core.section119 }
}

export async function runPreScreenAgent(input: PreScreenAgentInput): Promise<PreScreenAgentResult> {
  const core = computeCore(input)
  const { cap, section119, involvesCannabis } = core

  let narrativeChecks: AgentCheck[] = []
  let agentSummary = ""
  let assessment: ApplicationAssessment | null = null
  let model = "deterministic-rules"

  try {
    const res = await fetch("/api/assessment", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        flowId: input.flowId,
        title: input.title,
        applicant: input.applicant,
        summary: input.summary,
        documentChecks: input.documentChecks,
        preScreen: {
          involvesCannabis,
          // Tell the narrative pass which engines are in scope so it doesn't discuss (or invent checks about) ones that don't apply.
          applicability: {
            licenceCap: core.capApplies,
            section119: core.s119Applies,
            fitAndProper: core.fitAndProper.applicable,
          },
          cap: core.capApplies
            ? {
                rationale: cap.rationale,
                liquor: { total: cap.liquor.total, cap: cap.liquor.cap, status: cap.liquor.status },
                cannabis: { total: cap.cannabis.total, cap: cap.cannabis.cap, status: cap.cannabis.status },
                groupedParties: cap.groupedParties.map((p) => p.name),
                requiresStaffReview: cap.requiresStaffReview,
              }
            : null,
          section119: core.s119Applies
            ? {
                status: section119.status,
                confidence: section119.confidence,
                rationale: section119.rationale,
                signals: section119.signals.map((s) => ({ label: s.label, present: s.present })),
              }
            : null,
          fitAndProper: core.fitAndProper.applicable
            ? {
                program: core.fitAndProper.program,
                escalationTarget: core.fitAndProper.escalationTarget,
                recommendation: core.fitAndProper.recommendation,
                overallConfidence: core.fitAndProper.overallConfidence,
                rationale: core.fitAndProper.recommendationRationale,
                peopleScreened: core.fitAndProper.people.length,
                metCriteria: core.fitAndProper.escalationCriteria
                  .filter((c) => c.met)
                  .map((c) => c.label),
                findings: core.fitAndProper.findings.map((f) => ({
                  label: f.label,
                  outcome: f.outcome,
                  tier: f.tier,
                  finding: f.finding,
                })),
              }
            : null,
        },
      }),
    })

    if (res.ok) {
      const data = await res.json()
      model = data.model ?? model
      agentSummary = data.agentSummary ?? ""
      narrativeChecks = Array.isArray(data.narrativeChecks)
        ? data.narrativeChecks.map((c: AgentCheck) => ({
            kind: c.kind,
            label: c.label,
            outcome: c.outcome,
            confidence: c.confidence,
            finding: c.finding,
            observations: c.observations ?? [],
          }))
        : []
      assessment = {
        report: data.report,
        confidence: data.confidence,
        recommendation: data.recommendation,
        highlights: data.highlights ?? [],
        flags: data.flags ?? [],
        model: data.model,
        generatedAt: data.generatedAt,
      }
    }
  } catch (err) {
    console.log("[v0] pre-screen agent AI pass failed, using deterministic checks:", err instanceof Error ? err.message : err)
  }

  const agentLog = assembleLog(core, { narrativeChecks, agentSummary, model })
  return { agentLog, assessment, cap, section119 }
}
