"use client"

/** MS Dynamics 365 case-management boundary (prototype). */

import { createCollection, makeId } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { recordAudit } from "./audit-service"
import type { AgentLog, DynamicsQueue } from "@domain/models/staff"
import type { FitAndProperResult } from "@domain/models/screening"

export type SyncState = "pending" | "synced" | "failed"

export interface CaseSyncRecord extends Entity {
  /** Portal-side reference (job/permit number). */
  reference: string
  caseType: string
  applicant: string
  /** Dynamics case id once created (mock). */
  dynamicsCaseId?: string
  state: SyncState
  lastAttemptAt: string
  attempts: number
  /** Free-form payload digest for debugging. */
  payloadSummary: string
  error?: string
  /** Staff queue the AI pre-screen agent routed this case to. */
  queue?: DynamicsQueue
  /** The pre-screen agent log synced alongside the case (staff-only). */
  agentLog?: AgentLog | null
}

const collection = createCollection<CaseSyncRecord>("dynamics-sync")

export interface PushCaseInput {
  reference: string
  caseType: string
  applicant: string
  payloadSummary: string
  actor?: string
  /** Queue the pre-screen agent recommends for this case. */
  queue?: DynamicsQueue
  /** The pre-screen agent log to sync into the Dynamics case field. */
  agentLog?: AgentLog | null
}

/** Simulate pushing a case to Dynamics. */
export function pushCase(input: PushCaseInput): CaseSyncRecord {
  const ok = Math.random() > 0.08
  const record: CaseSyncRecord = {
    id: makeId("case"),
    reference: input.reference,
    caseType: input.caseType,
    applicant: input.applicant,
    dynamicsCaseId: ok ? `LCRB-${Math.floor(100000 + Math.random() * 899999)}` : undefined,
    state: ok ? "synced" : "failed",
    lastAttemptAt: new Date().toISOString(),
    attempts: 1,
    payloadSummary: input.payloadSummary,
    error: ok ? undefined : "Dataverse request timed out (simulated). Retry available.",
    queue: input.queue,
    agentLog: input.agentLog ?? null,
  }
  collection.upsert(record)
  recordAudit({
    category: "integration",
    action: ok ? "dynamics.synced" : "dynamics.sync_failed",
    summary: ok
      ? `Case ${input.reference} synced to Dynamics (${record.dynamicsCaseId})`
      : `Dynamics sync failed for case ${input.reference}`,
    actor: input.actor ?? "System",
    subjectId: input.reference,
  })
  return record
}

/** Retry a previously failed sync. */
export function retrySync(id: string, actor = "System"): CaseSyncRecord | undefined {
  const rec = collection.get(id)
  if (!rec) return undefined
  const ok = Math.random() > 0.05
  const updated = collection.update(id, {
    state: ok ? "synced" : "failed",
    dynamicsCaseId: ok
      ? rec.dynamicsCaseId ?? `LCRB-${Math.floor(100000 + Math.random() * 899999)}`
      : undefined,
    attempts: rec.attempts + 1,
    lastAttemptAt: new Date().toISOString(),
    error: ok ? undefined : "Dataverse request timed out (simulated). Retry available.",
  })
  recordAudit({
    category: "integration",
    action: ok ? "dynamics.synced" : "dynamics.sync_failed",
    summary: ok
      ? `Case ${rec.reference} re-synced to Dynamics`
      : `Dynamics retry failed for case ${rec.reference}`,
    actor,
    subjectId: rec.reference,
  })
  return updated
}

export function getSyncByReference(reference: string): CaseSyncRecord | undefined {
  return collection.list().find((c) => c.reference === reference)
}

export function listSyncRecords(): CaseSyncRecord[] {
  return collection.list().sort((a, b) => (a.lastAttemptAt < b.lastAttemptAt ? 1 : -1))
}


/** All cases routed to a given queue, newest first. */
export function listCasesByQueue(queue: DynamicsQueue): CaseSyncRecord[] {
  return listSyncRecords().filter((c) => (c.queue ?? "standard-processing") === queue)
}

/** Count of cases in each queue (for the queue tabs / badges). */
export function queueCounts(): Record<DynamicsQueue, number> {
  const counts: Record<DynamicsQueue, number> = {
    "beyond-licence-cap": 0,
    "section-119-review": 0,
    "enhanced-review": 0,
    "standard-processing": 0,
  }
  for (const c of collection.list()) counts[c.queue ?? "standard-processing"]++
  return counts
}

export function getCaseById(id: string): CaseSyncRecord | undefined {
  return collection.get(id)
}

/** Seed a few representative cases (one per queue) the first time the staff triage view is opened */
export function seedSampleCasesIfEmpty(): void {
  if (collection.list().length > 0) return
  for (const sample of SAMPLE_CASES) {
    collection.upsert({
      id: makeId("case"),
      lastAttemptAt: new Date(Date.now() - Math.floor(Math.random() * 6) * 3600_000).toISOString(),
      attempts: 1,
      state: "synced",
      dynamicsCaseId: `LCRB-${Math.floor(100000 + Math.random() * 899999)}`,
      ...sample,
    })
  }
}

function agentLogFor(
  queue: DynamicsQueue,
  overallConfidence: number,
  summary: string,
  checks: AgentLog["checks"],
  queueRationale: string,
  requiresHumanReview: boolean,
  fitAndProper?: FitAndProperResult,
): AgentLog {
  return {
    model: "openai/gpt-5-mini",
    generatedAt: new Date().toISOString(),
    overallConfidence,
    summary,
    checks,
    recommendedQueue: queue,
    queueRationale,
    requiresHumanReview,
    fitAndProper,
  }
}

/** Demo fit-and-proper results for the seeded cannabis cases. */
const FP_CLEAR_HARBOURSIDE: FitAndProperResult = {
  applicable: true,
  program: "cannabis",
  escalationTarget: "the Security Programs Division (SPD)",
  overallConfidence: 86,
  overallOutcome: "clear",
  summary:
    "Corporate structure, ownership and LCRB history are fully resolvable from Tier-1 sources with no adverse signal. No restricted-data question arises — recommend clearing in-portal.",
  findings: [
    {
      id: "fp-hs-1",
      dimension: "business",
      label: "Corporate structure & registration",
      outcome: "clear",
      confidence: 92,
      tier: 1,
      finding: "Active, in-good-standing BC company with a fully mapped ownership chain.",
      evidence: [
        { source: "BC Registry / OrgBook", detail: "BC1180245 — active, annual report current." },
        { source: "Legal Entity build", detail: "3-tier structure resolved; no undisclosed intermediary entities." },
      ],
    },
    {
      id: "fp-hs-2",
      dimension: "business",
      label: "LCRB licensing & enforcement history",
      outcome: "clear",
      confidence: 88,
      tier: 1,
      finding: "Existing liquor licences in good standing; no contraventions in the last 5 years.",
      evidence: [
        { source: "LCRB enforcement records", detail: "0 contraventions on associated licences." },
      ],
    },
    {
      id: "fp-hs-fin",
      dimension: "business",
      label: "Financial integrity & source of funding",
      outcome: "clear",
      confidence: 85,
      tier: 1,
      finding: "Declared source of all investment money reconciles with the disclosed ownership; no undisclosed financing or illicit-proceeds indicator detected.",
      evidence: [
        { source: "Cannabis Screening Form / applicant declarations", detail: "Funding sources and debts declared; reconcile with ownership." },
      ],
    },
    {
      id: "fp-hs-3",
      dimension: "people",
      label: "Beneficial ownership & control",
      outcome: "clear",
      confidence: 84,
      tier: 1,
      finding: "All directors and >10% shareholders disclosed and consistent with BC Registry.",
      evidence: [
        { source: "Applicant declarations", detail: "3 directors, 2 controlling shareholders declared." },
        { source: "BC Registry / OrgBook", detail: "Declared officers match registry filings." },
      ],
    },
    {
      id: "fp-hs-4",
      dimension: "people",
      label: "Open-source footprint (advisory)",
      outcome: "clear",
      confidence: 71,
      tier: 1,
      finding: "No adverse open-source media associated with the named principals.",
      evidence: [{ source: "Open-source review", detail: "No adverse matches on principal names." }],
    },
  ],
  people: [
    { name: "David Chen", roles: ["Director", "Shareholder"], entity: "Harbourside Hospitality Group Ltd.", outcome: "clear", note: "Disclosed; consistent with registry." },
    { name: "Priya Anand", roles: ["Director"], entity: "Harbourside Hospitality Group Ltd.", outcome: "clear", note: "Disclosed; consistent with registry." },
    { name: "Marcus Webb", roles: ["Shareholder (22%)"], entity: "Harbour Cannabis Co.", outcome: "clear", note: "Disclosed; no adverse signal." },
  ],
  recommendation: "clear-in-portal",
  recommendationRationale:
    "All findings resolved at Tier 1 with no adverse signal and no question requiring restricted justice/police data. An SPD referral would add delay without adding information.",
  escalationCriteria: [
    { label: "Undisclosed / unresolvable ownership", met: false, detail: "Ownership chain fully mapped from registry." },
    { label: "Adverse LCRB enforcement history", met: false, detail: "No contraventions on associated licences." },
    { label: "Adverse open-source / criminal-nexus signal", met: false, detail: "No adverse open-source matches." },
    { label: "Declaration inconsistency", met: false, detail: "Declarations match Tier-1 records." },
    { label: "Financial integrity / source-of-funding concern", met: false, detail: "Source of all investment money reconciles with ownership." },
    { label: "Unlicensed / illicit cannabis-market history", met: false, detail: "No illicit-market history matched to the applicant or associates." },
  ],
  tierCoverage: { tier1: 100, tier2: 0, tier3: 0 },
  generatedAt: new Date().toISOString(),
}

const FP_REFER_SPIRITBEAR: FitAndProperResult = {
  applicable: true,
  program: "cannabis",
  escalationTarget: "the Security Programs Division (SPD)",
  overallConfidence: 61,
  overallOutcome: "flag",
  summary:
    "Tier-1 screening is mostly clean, but an open-source signal on one principal points to a question only restricted Tier-3 data (CPIC/PRIME BC) can resolve. Recommend an SPD referral scoped to that individual.",
  findings: [
    {
      id: "fp-sb-1",
      dimension: "business",
      label: "Corporate structure & registration",
      outcome: "clear",
      confidence: 89,
      tier: 1,
      finding: "Active BC company; ownership chain resolved from registry.",
      evidence: [{ source: "BC Registry / OrgBook", detail: "BC1211882 — active, in good standing." }],
    },
    {
      id: "fp-sb-fin",
      dimension: "business",
      label: "Financial integrity & source of funding",
      outcome: "clear",
      confidence: 80,
      tier: 1,
      finding: "Declared source of all investment money reconciles with the disclosed ownership; no undisclosed financing or illicit-proceeds indicator detected.",
      evidence: [
        { source: "Cannabis Screening Form / applicant declarations", detail: "Funding sources and debts declared; reconcile with ownership." },
      ],
    },
    {
      id: "fp-sb-2",
      dimension: "people",
      label: "Beneficial ownership & control",
      outcome: "attention",
      confidence: 66,
      tier: 1,
      finding: "One controlling shareholder was added by amendment after initial filing; confirm timing with applicant.",
      evidence: [
        { source: "BC Registry / OrgBook", detail: "Shareholder register amended 2 weeks post-incorporation." },
      ],
    },
    {
      id: "fp-sb-3",
      dimension: "people",
      label: "Open-source footprint (advisory)",
      outcome: "flag",
      confidence: 58,
      tier: 1,
      finding:
        "Adverse open-source media associates a named principal with a prior enforcement matter in another jurisdiction — cannot be confirmed or cleared with in-portal data.",
      evidence: [
        { source: "Open-source review", detail: "News match on principal name tied to a prior regulatory action (unverified)." },
      ],
      escalates: true,
    },
  ],
  people: [
    { name: "Raymond Osei", roles: ["Director", "Shareholder (51%)"], entity: "Spirit Bear Cannabis Ltd.", outcome: "flag", note: "Open-source signal requires Tier-3 confirmation." },
    { name: "Lena Whitefeather", roles: ["Director"], entity: "Spirit Bear Cannabis Ltd.", outcome: "clear", note: "Disclosed; no adverse signal." },
  ],
  recommendation: "refer-to-spd",
  recommendationRationale:
    "An adverse open-source signal on a controlling principal cannot be verified or cleared with Tier-1/Tier-2 data. Confirming or dismissing it requires restricted CPIC/PRIME BC records — a scoped SPD referral is warranted for that individual only.",
  escalationCriteria: [
    { label: "Undisclosed / unresolvable ownership", met: false, detail: "Ownership resolved; one amendment to confirm." },
    { label: "Adverse LCRB enforcement history", met: false, detail: "No LCRB contraventions on file." },
    { label: "Adverse open-source / criminal-nexus signal", met: true, detail: "Open-source match to a prior regulatory action on a controlling principal." },
    { label: "Declaration inconsistency", met: false, detail: "Declarations otherwise consistent." },
    { label: "Financial integrity / source-of-funding concern", met: false, detail: "Source of all investment money reconciles with ownership." },
    { label: "Unlicensed / illicit cannabis-market history", met: false, detail: "No illicit-market history matched to the applicant or associates." },
  ],
  tierCoverage: { tier1: 67, tier2: 0, tier3: 33 },
  generatedAt: new Date().toISOString(),
}

/** Demo fit-and-proper results for the seeded LIQUOR cases. */
const FP_CLEAR_MAPLE_TRANSFER: FitAndProperResult = {
  applicable: true,
  program: "liquor",
  escalationTarget: "an enhanced suitability review",
  overallConfidence: 88,
  overallOutcome: "clear",
  summary:
    "Change-of-ownership transfer. The incoming owners' Personal History Summaries, police information checks, and source of funding all reconcile in-portal with no adverse signal. Clear in-portal — no enhanced suitability review needed.",
  findings: [
    {
      id: "fp-mt-1",
      dimension: "business",
      label: "Corporate structure & registration",
      outcome: "clear",
      confidence: 91,
      tier: 1,
      finding: "Incoming licensee is an active BC company with a fully disclosed ownership chain.",
      evidence: [{ source: "BC Registry / OrgBook", detail: "BC1099432 — active, in good standing." }],
    },
    {
      id: "fp-mt-2",
      dimension: "business",
      label: "Financial integrity & source of funding",
      outcome: "clear",
      confidence: 86,
      tier: 1,
      finding: "Declared purchase financing reconciles with the disclosed ownership; no undisclosed third-party control.",
      evidence: [{ source: "Applicant declarations / BC Registry", detail: "Funding reconciles with ownership." }],
    },
    {
      id: "fp-mt-3",
      dimension: "people",
      label: "Criminal record check & personal history",
      outcome: "clear",
      confidence: 87,
      tier: 1,
      finding: "Personal History Summaries and police information checks for both incoming directors show no items bearing on suitability.",
      evidence: [{ source: "Personal History Summary / police information check", detail: "No adverse items." }],
    },
  ],
  people: [
    { name: "Sarah Nguyen", roles: ["Director", "Shareholder (60%)"], entity: "Maple & Oak Dining Ltd.", outcome: "clear", note: "Incoming owner; checks clear." },
    { name: "Tom Reyes", roles: ["Director", "Shareholder (40%)"], entity: "Maple & Oak Dining Ltd.", outcome: "clear", note: "Incoming owner; checks clear." },
  ],
  recommendation: "clear-in-portal",
  recommendationRationale:
    "All suitability factors — criminal record, compliance history, and financial integrity — resolved in-portal with no adverse signal. Recommend proceeding without an enhanced suitability review, subject to analyst review.",
  escalationCriteria: [
    { label: "Active enforcement / serious contravention history", met: false, detail: "No active enforcement against the group." },
    { label: "Unresolved ownership or beneficial-owner gap", met: false, detail: "Ownership traces cleanly." },
    { label: "Adverse open-source / media finding requiring verification", met: false, detail: "No adverse open-source finding." },
    { label: "Unresolvable declaration inconsistency", met: false, detail: "Declarations reconcile." },
    { label: "Adverse criminal record relevant to suitability", met: false, detail: "No criminal-record item bearing on suitability." },
    { label: "Financial integrity / source-of-funding concern", met: false, detail: "Source of funding reconciles with the ownership structure." },
  ],
  tierCoverage: { tier1: 100, tier2: 0, tier3: 0 },
  generatedAt: new Date().toISOString(),
}

const FP_REVIEW_STONEGATE_NEW: FitAndProperResult = {
  applicable: true,
  program: "liquor",
  escalationTarget: "an enhanced suitability review",
  overallConfidence: 64,
  overallOutcome: "flag",
  summary:
    "New liquor primary application. A police information check on a controlling director returned an item that may bear on suitability. Recommend an enhanced suitability review to weigh it against the fit-and-proper standard before any determination.",
  findings: [
    {
      id: "fp-sg-1",
      dimension: "business",
      label: "Corporate structure & registration",
      outcome: "clear",
      confidence: 88,
      tier: 1,
      finding: "Active BC company; ownership chain disclosed and resolved.",
      evidence: [{ source: "BC Registry / OrgBook", detail: "BC1240771 — active, in good standing." }],
    },
    {
      id: "fp-sg-2",
      dimension: "business",
      label: "Financial integrity & source of funding",
      outcome: "attention",
      confidence: 68,
      tier: 1,
      finding: "A portion of startup funding is attributed to a private lender not named in the ownership disclosure — confirm the arrangement and that no undisclosed party controls the business.",
      evidence: [{ source: "Applicant declarations / BC Registry", detail: "Funding declaration does not fully reconcile — confirm with the applicant." }],
    },
    {
      id: "fp-sg-3",
      dimension: "people",
      label: "Criminal record check & personal history",
      outcome: "flag",
      confidence: 66,
      tier: 1,
      finding: "A police information check for the controlling director returned an item that an enhanced suitability review should weigh against the fit-and-proper standard before any determination.",
      evidence: [{ source: "Personal History Summary / police information check", detail: "A returned criminal-record item requires an enhanced suitability review." }],
      escalates: true,
    },
  ],
  people: [
    { name: "Derek Malone", roles: ["Director", "Shareholder (70%)"], entity: "Stonegate Public House Ltd.", outcome: "flag", note: "Criminal-record item relevant to suitability — enhanced suitability review recommended." },
    { name: "Aisha Karim", roles: ["Director", "Shareholder (30%)"], entity: "Stonegate Public House Ltd.", outcome: "clear", note: "Checks clear." },
  ],
  recommendation: "refer-to-spd",
  recommendationRationale:
    "A criminal-record item relevant to suitability was found on a controlling director. An enhanced suitability review should weigh it against the fit-and-proper standard before any determination.",
  escalationCriteria: [
    { label: "Active enforcement / serious contravention history", met: false, detail: "No active enforcement against the group." },
    { label: "Unresolved ownership or beneficial-owner gap", met: false, detail: "Ownership traces cleanly." },
    { label: "Adverse open-source / media finding requiring verification", met: false, detail: "No adverse open-source finding." },
    { label: "Unresolvable declaration inconsistency", met: false, detail: "Declarations reconcile." },
    { label: "Adverse criminal record relevant to suitability", met: true, detail: "A police information check returned an item that an enhanced suitability review should weigh." },
    { label: "Financial integrity / source-of-funding concern", met: false, detail: "Funding attention item is resolvable in-portal with the applicant." },
  ],
  tierCoverage: { tier1: 100, tier2: 0, tier3: 0 },
  generatedAt: new Date().toISOString(),
}

const SAMPLE_CASES: Omit<CaseSyncRecord, "id" | "lastAttemptAt" | "attempts" | "state" | "dynamicsCaseId">[] = [
  {
    reference: "CRS-126540",
    caseType: "Cannabis Retail Store — New Application",
    applicant: "Harbourside Hospitality Group Ltd.",
    payloadSummary: "18 fields; assessment included",
    queue: "beyond-licence-cap",
    agentLog: agentLogFor(
      "beyond-licence-cap",
      71,
      "Applicant's effective business group reaches the cannabis licence cap once tied-house associations are collapsed. Cap determination required before processing.",
      [
        {
          kind: "licence-cap",
          label: "Licence cap determination",
          outcome: "flag",
          confidence: 63,
          finding: "Cannabis 8/8 (6 direct, +2 linked); at cap in the cannabis domain.",
          observations: [
            "Cannabis: 8/8 (6 direct, 2 linked)",
            "Liquor: 3/8 (3 direct, 0 linked)",
            "Effective group: Harbourside Hospitality Group Ltd., Harbour Cannabis Co., Westcoast Beverage Partners",
          ],
        },
        {
          kind: "section-119",
          label: "Section 119 (First Nations cannabis)",
          outcome: "clear",
          confidence: 82,
          finding: "Cannabis application with no First Nation affiliation on file.",
          observations: ["First Nation affiliation: no", "Signed s.119 agreement on file: no"],
        },
        {
          kind: "tied-house",
          label: "Tied-house associations",
          outcome: "attention",
          confidence: 66,
          finding: "Two associated cannabis licences may make the businesses one-and-the-same.",
          observations: ["Association: Harbour Cannabis Co. (608114)", "Association: Westside Green Room (609552)"],
        },
        {
          kind: "fit-and-proper",
          label: "Fit & proper (cannabis)",
          outcome: "clear",
          confidence: 86,
          finding: "Clear in-portal: Tier-1 screening resolved ownership and history with no adverse signal.",
          observations: [
            "Screened 3 associated individual(s) across 4 Tier-1 checks.",
            "Tier-1 in-portal coverage: 100% of checks.",
            "No escalation triggers met — SPD referral not required.",
          ],
        },
      ],
      "Effective business group is at the 8-licence cannabis cap. Route to cap review.",
      true,
      FP_CLEAR_HARBOURSIDE,
    ),
  },
  {
    reference: "CRS-126603",
    caseType: "Cannabis Retail Store — New Application",
    applicant: "Spirit Bear Cannabis Ltd.",
    payloadSummary: "20 fields; assessment included",
    queue: "section-119-review",
    agentLog: agentLogFor(
      "section-119-review",
      84,
      "First Nation cannabis application on nation lands with a signed s.119 agreement on file. Confirm agreement coverage, then process.",
      [
        {
          kind: "section-119",
          label: "Section 119 (First Nations cannabis)",
          outcome: "attention",
          confidence: 90,
          finding: "First Nation cannabis on nation lands with a signed s.119 agreement on file.",
          observations: [
            "Cannabis application: yes",
            "First Nation affiliation: yes — Tla-o-qui-aht First Nation",
            "Premises on nation lands: yes",
            "Signed s.119 agreement on file: yes — S119-2023-014 (2023-11-02)",
          ],
        },
        {
          kind: "licence-cap",
          label: "Licence cap determination",
          outcome: "clear",
          confidence: 94,
          finding: "Cannabis 1/8 (1 direct); under cap in both domains.",
          observations: ["Cannabis: 1/8 (1 direct, 0 linked)", "Liquor: 0/8"],
        },
        {
          kind: "completeness",
          label: "Completeness",
          outcome: "clear",
          confidence: 88,
          finding: "Required documents present, including the s.119 agreement reference.",
          observations: ["Security screening: submitted", "Financial integrity: submitted"],
        },
        {
          kind: "fit-and-proper",
          label: "Fit & proper (cannabis)",
          outcome: "flag",
          confidence: 61,
          finding: "Refer to SPD: an open-source signal on a controlling principal needs restricted Tier-3 confirmation.",
          observations: [
            "Screened 2 associated individual(s) across 3 Tier-1 checks.",
            "Escalation trigger: Adverse open-source / criminal-nexus signal — open-source match to a prior regulatory action on a controlling principal.",
            "Tier-1 in-portal coverage: 67% of checks; 33% requires SPD.",
          ],
        },
      ],
      "First Nations cannabis nexus detected (applies). Confirm s.119 agreement coverage.",
      true,
      FP_REFER_SPIRITBEAR,
    ),
  },
  {
    reference: "LP-126588",
    caseType: "Liquor Primary — Structural Change",
    applicant: "Cedar Coast Holdings Inc.",
    payloadSummary: "15 fields; assessment included",
    queue: "enhanced-review",
    agentLog: agentLogFor(
      "enhanced-review",
      58,
      "Answers about the operating entity are inconsistent with the legal entity build, and a tied-house link is undeclared. A closer look is recommended before processing.",
      [
        {
          kind: "consistency",
          label: "Internal consistency",
          outcome: "flag",
          confidence: 52,
          finding: "Declared operator name does not match the applicant legal entity on the build.",
          observations: ["Form operator: Cedar Coast Retail", "LE applicant: Cedar Coast Holdings Inc."],
        },
        {
          kind: "tied-house",
          label: "Tied-house associations",
          outcome: "attention",
          confidence: 55,
          finding: "A manufacturer association appears undeclared on this application.",
          observations: ["Possible association: Island Craft Brewery (441883)"],
        },
      ],
      "Elevated signals (undeclared tied-house link and consistency concerns) warrant a closer look.",
      true,
    ),
  },
  {
    reference: "LT-126612",
    caseType: "Liquor Primary — Transfer of Ownership",
    applicant: "Maple & Oak Dining Ltd.",
    payloadSummary: "14 fields; assessment included",
    queue: "standard-processing",
    agentLog: agentLogFor(
      "standard-processing",
      88,
      "Change-of-ownership transfer of a liquor primary licence. Fit-and-proper screening of the incoming owners clears in-portal with no adverse signal. Route through the normal workflow.",
      [
        {
          kind: "fit-and-proper",
          label: "Fit & proper (liquor)",
          outcome: "clear",
          confidence: 88,
          finding: "Clear in-portal: incoming owners' criminal record, compliance history, and financial integrity all reconcile with no adverse signal.",
          observations: [
            "Screened 2 associated individual(s) and the business across 3 checks.",
            "All checks resolved in-portal from applicant-provided and registry data.",
            "No escalation triggers met — enhanced suitability review not required.",
          ],
        },
        {
          kind: "completeness",
          label: "Completeness",
          outcome: "clear",
          confidence: 90,
          finding: "Transfer package complete, including Personal History Summaries for the incoming owners.",
          observations: ["Personal History Summary: provided", "Financial integrity: submitted"],
        },
      ],
      "Transfer with a clean fit-and-proper pre-screen — route through the normal workflow.",
      false,
      FP_CLEAR_MAPLE_TRANSFER,
    ),
  },
  {
    reference: "LP-126620",
    caseType: "Liquor Primary — New Application",
    applicant: "Stonegate Public House Ltd.",
    payloadSummary: "17 fields; assessment included",
    queue: "enhanced-review",
    agentLog: agentLogFor(
      "enhanced-review",
      64,
      "New liquor primary application. A police information check on a controlling director returned an item bearing on suitability, and a source-of-funding item needs confirmation. Recommend an enhanced suitability review before processing.",
      [
        {
          kind: "fit-and-proper",
          label: "Fit & proper (liquor)",
          outcome: "flag",
          confidence: 64,
          finding: "Refer for enhanced review: a criminal-record item on a controlling director should be weighed against the fit-and-proper standard.",
          observations: [
            "Screened 2 associated individual(s) and the business across 3 checks.",
            "Escalation trigger: Adverse criminal record relevant to suitability — a police information check returned an item that an enhanced suitability review should weigh.",
            "All checks resolved in-portal from applicant-provided and registry data.",
          ],
        },
        {
          kind: "completeness",
          label: "Completeness",
          outcome: "clear",
          confidence: 87,
          finding: "Application package complete, including Personal History Summaries and financing declarations.",
          observations: ["Personal History Summary: provided", "Source-of-funding declaration: submitted"],
        },
      ],
      "Criminal-record item on a controlling director warrants an enhanced suitability review.",
      true,
      FP_REVIEW_STONEGATE_NEW,
    ),
  },
  {
    reference: "FP-126571",
    caseType: "Food Primary — New Application",
    applicant: "Riverside Eatery Ltd.",
    payloadSummary: "12 fields; assessment included",
    queue: "standard-processing",
    agentLog: agentLogFor(
      "standard-processing",
      93,
      "Straightforward food-primary application. Fit-and-proper screening clears in-portal and the package is complete. Licence cap and Section 119 do not apply to liquor applications. Route through the normal workflow.",
      [
        {
          kind: "completeness",
          label: "Completeness",
          outcome: "clear",
          confidence: 90,
          finding: "All required fields and declarations complete.",
          observations: ["Floor plan: provided", "Declarations: signed"],
        },
      ],
      "No elevated signals detected — route through the normal workflow.",
      false,
    ),
  },
]

export const dynamicsCollection = collection
