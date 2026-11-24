import { generateText, Output } from "ai"
import { z } from "zod"
import { logAiError, AI_MAX_RETRIES, friendlyJsonError } from "@domain/ai/ai-errors"

export const maxDuration = 30

/** Generates the single templated AI assessment report for a submitted application; it is pushed to Dynamics via the sync service and only staff see it, never the applicant. */

const AssessmentSchema = z.object({
  confidence: z
    .number()
    .min(0)
    .max(100)
    .describe("Overall confidence (0-100) that the application is complete and approvable as submitted."),
  recommendation: z
    .string()
    .describe('One short triage line, e.g. "Low-touch — appears complete and approvable" or "Needs review — missing documents".'),
  report: z
    .string()
    .describe(
      "The templated assessment report body as plain text with the section headers described in the system prompt. This maps into a single Dynamics data field.",
    ),
  highlights: z
    .array(z.string())
    .describe("3-5 short bullet points summarising what was assessed and what looks good."),
  flags: z
    .array(z.string())
    .describe("Items a reviewer should pay attention to (missing info, inconsistencies, risk factors). Empty array if none."),
})

/** Pre-screen narrative checks (internal consistency, completeness, tied-house associations) on top of the deterministic engines, with a per-check confidence score. */
const NarrativeCheckSchema = z.object({
  kind: z.enum(["consistency", "completeness", "tied-house"]),
  label: z.string().describe("Short check label, e.g. 'Internal consistency'."),
  outcome: z.enum(["clear", "attention", "flag"]),
  confidence: z.number().min(0).max(100).describe("Confidence (0-100) in this check's finding."),
  finding: z.string().describe("One-line finding for the staff log."),
  observations: z.array(z.string()).describe("The concrete data points inspected (audit trail)."),
})

const AgentSchema = AssessmentSchema.extend({
  agentSummary: z
    .string()
    .describe("2-3 sentence triage narrative that also comments on the licence-cap and Section 119 findings supplied."),
  narrativeChecks: z
    .array(NarrativeCheckSchema)
    .describe("Judgement-based checks: internal consistency, completeness, and tied-house associations."),
})

interface PreScreenContext {
  involvesCannabis: boolean
  /** Which deterministic engines apply to this application type; out-of-scope engines arrive as null (an endorsement or amendment skips the licence cap, Section 119 is cannabis-only). */
  applicability?: {
    licenceCap: boolean
    section119: boolean
    fitAndProper: boolean
  }
  cap?: {
    rationale: string
    liquor: { total: number; cap: number; status: string }
    cannabis: { total: number; cap: number; status: string }
    groupedParties: string[]
    requiresStaffReview: boolean
  } | null
  section119?: {
    status: string
    confidence: number
    rationale: string
    signals: { label: string; present: boolean }[]
  } | null
  /** Fit-and-proper pre-screen (every cannabis app; liquor new-licence/transfer). */
  fitAndProper?: {
    program: "cannabis" | "liquor"
    escalationTarget: string
    recommendation: string
    overallConfidence: number
    rationale: string
    peopleScreened: number
    metCriteria: string[]
    findings: { label: string; outcome: string; tier: number; finding: string }[]
  } | null
}

interface AssessmentRequest {
  flowId: string
  title: string
  applicant: string
  summary: { label: string; value: string }[]
  /** Optional per-document completeness results gathered during the flow. */
  documentChecks?: { label: string; status: string; summary: string }[]
  /** When present, the route also returns agent pre-screen narrative + checks. */
  preScreen?: PreScreenContext
}

const SYSTEM_PROMPT = `You are the LCRB (BC Liquor and Cannabis Regulation Branch) application assessment assistant.

You assess a SUBMITTED licensing application and produce ONE templated report for LCRB staff inside Microsoft Dynamics. The applicant never sees this. Your goal is to help staff quickly understand what was assessed and how much time, if any, they need to spend.

Ground your assessment in BC's Liquor Control and Licensing Act & Regulation and the Cannabis Control and Licensing Act: completeness of required information, eligibility signals, consistency of the answers, presence of required supporting documents, and any risk/compliance factors that warrant human review. Be objective and concise. Do not invent facts that are not supported by the provided data; if something is unknown, say so.

The DOCUMENT INTELLIGENCE & COMPLETENESS CHECKS may include automated findings from uploaded documents: the occupancy load read from a floor plan's stamp, whether that stamp is older than 12 months (expired), and a BC Registries confirmation of declared company directors. Treat these as advisory signals — an expired occupancy stamp, a missing stamp, or an unverified/partial director match should be surfaced under RISK & ATTENTION ITEMS (and in "flags") for staff follow-up, but NONE of them block the application. Note when a registry check was simulated.

Produce the "report" field as plain text using EXACTLY these section headers, each on its own line followed by its content:

ASSESSMENT SUMMARY
<2-3 sentence overview of the application and overall readiness.>

COMPLETENESS
<What required information/documents are present vs. missing or unclear.>

CONSISTENCY & ELIGIBILITY
<Note any inconsistencies, eligibility signals, or statutory considerations.>

RISK & ATTENTION ITEMS
<Anything a reviewer should look at closely; write "None identified" if nothing.>

RECOMMENDED ACTION
<Triage recommendation and a rough sense of reviewer effort (e.g. low/medium/high touch).>

CONFIDENCE: <n>%
<One sentence explaining the score.>

Keep the whole report under ~250 words. Be specific to the data provided.`

const AGENT_PROMPT_ADDON = `PRE-SCREEN AGENT MODE.

You are also acting as an automated pre-screening agent that triages the case BEFORE a human reviewer. You are given deterministic engine results for the licence cap and Section 119 — treat those numbers and statuses as authoritative; never recompute or contradict the math. Your job is to (a) write "agentSummary": a 2-3 sentence plain-language triage narrative that comments on whichever engine findings were SUPPLIED below, and (b) produce "narrativeChecks": your own judgement-based checks that the deterministic engines do NOT cover.

SCOPE (CRITICAL): only reason about checks that apply to THIS application type. Any engine marked "NOT APPLICABLE" below must be omitted entirely — do not mention it, do not speculate about it, and never produce a check for it. Staff reported an endorsement application carrying licence-cap and security-screening checks that had no bearing on it; irrelevant checks bury the findings that matter. Every check you return must be justifiable for this specific application type.

For narrativeChecks, return up to three checks with kinds "consistency" (do the submitted answers agree with each other and with known holdings?), "completeness" (is required info/documentation present?), and "tied-house" (do declared associations raise concerns?). For each, set outcome to "clear", "attention", or "flag", give a calibrated confidence 0-100 (lower when the data is thin or ambiguous), a one-line finding, and list the specific observations you inspected. Do not duplicate the licence-cap or Section 119 checks here. Never assert a final legal determination — you advise; LCRB staff decide.

When a FIT & PROPER PRE-SCREEN block is supplied, treat its recommendation as authoritative and DO NOT recompute it or produce your own fit-and-proper check in narrativeChecks. Instead, weave it into the report: reference the fit-and-proper recommendation in ASSESSMENT SUMMARY, and surface any met escalation triggers under RISK & ATTENTION ITEMS. Be explicit about whether the case can be cleared in-portal or needs escalation, and use the program's escalation target (given in the block) as the destination.
- For CANNABIS, escalation is a referral to the Security Programs Division (SPD); the portal only screens Tier-1 data and restricted Tier-3 records (CPIC/PRIME BC/intelligence) are resolved by SPD, not in-portal.
- For LIQUOR, fit-and-proper applies only to new-licence and transfer (change of ownership) applications; there is no organized-crime/SPD nexus. Screening rests on the applicant's Personal History Summary, police information check, compliance history, and financial integrity — all resolvable in-portal — and any escalation is an internal ENHANCED SUITABILITY REVIEW, never an SPD referral. Do not mention SPD or Tier-3 for liquor.`

export async function POST(req: Request) {
  let body: AssessmentRequest
  try {
    body = (await req.json()) as AssessmentRequest
  } catch {
    return Response.json({ error: "Invalid request body." }, { status: 400 })
  }

  const { title, applicant, summary, documentChecks, preScreen } = body
  if (!title || !Array.isArray(summary)) {
    return Response.json({ error: "Missing application data." }, { status: 400 })
  }

  const answersText = summary.length
    ? summary.map((s) => `- ${s.label}: ${s.value}`).join("\n")
    : "(no answers captured)"

  const docsText =
    documentChecks && documentChecks.length
      ? documentChecks.map((d) => `- ${d.label}: ${d.status} — ${d.summary}`).join("\n")
      : "(no document completeness checks recorded)"

  const model = "openai/gpt-5-mini"

  // Build the optional pre-screen context block for the agent path.
  const preScreenText = preScreen
    ? `

PRE-SCREEN ENGINE RESULTS (deterministic — do NOT recompute these numbers; reason about them):
${
  preScreen.cap
    ? `LICENCE CAP (applies to CANNABIS retail only — there is NO cap on liquor licences; never imply one): ${preScreen.cap.rationale}
  - Cannabis: ${preScreen.cap.cannabis.total}/${preScreen.cap.cannabis.cap} (${preScreen.cap.cannabis.status})
  - Liquor in group (context only, uncapped): ${preScreen.cap.liquor.total}
  - Effective business group: ${preScreen.cap.groupedParties.join(", ") || "applicant only"}`
    : `LICENCE CAP: NOT APPLICABLE to this application type — it does not add a licence to the business group. Do not discuss the licence cap or produce a cap-related check.`
}
${
  preScreen.section119
    ? `SECTION 119: ${preScreen.section119.status} (engine confidence ${preScreen.section119.confidence}%) — ${preScreen.section119.rationale}
  - Signals: ${preScreen.section119.signals.map((s) => `${s.label}=${s.present ? "yes" : "no"}`).join(", ")}`
    : `SECTION 119: NOT APPLICABLE — this is not a cannabis application. Do not discuss Section 119 or First Nations cannabis agreements.`
}
INVOLVES CANNABIS: ${preScreen.involvesCannabis ? "yes" : "no"}${
        preScreen.fitAndProper
          ? `
FIT & PROPER PRE-SCREEN (${preScreen.fitAndProper.program.toUpperCase()} program, deterministic — do NOT recompute; reason about it):
  - Escalation target if needed: ${preScreen.fitAndProper.escalationTarget}
  - Recommendation: ${preScreen.fitAndProper.recommendation} (confidence ${preScreen.fitAndProper.overallConfidence}%) — ${preScreen.fitAndProper.rationale}
  - Individuals screened: ${preScreen.fitAndProper.peopleScreened}
  - Escalation triggers met: ${preScreen.fitAndProper.metCriteria.length ? preScreen.fitAndProper.metCriteria.join("; ") : "none"}
  - Findings: ${preScreen.fitAndProper.findings.map((f) => `${f.label} [${f.outcome}]: ${f.finding}`).join(" | ")}`
          : ""
      }`
    : ""

  try {
    const { experimental_output } = await generateText({
      model,
      maxRetries: AI_MAX_RETRIES,
      system: preScreen ? `${SYSTEM_PROMPT}\n\n${AGENT_PROMPT_ADDON}` : SYSTEM_PROMPT,
      experimental_output: Output.object({ schema: preScreen ? AgentSchema : AssessmentSchema }),
      prompt: `Assess this submitted application.

APPLICATION TYPE: ${title}
APPLICANT / BUSINESS: ${applicant || "Not provided"}

SUBMITTED ANSWERS:
${answersText}

DOCUMENT INTELLIGENCE & COMPLETENESS CHECKS:
${docsText}${preScreenText}`,
    })

    // Models occasionally pad string fields with runaway whitespace; tidy up.
    const out = experimental_output as z.infer<typeof AgentSchema>
    const cleaned = {
      confidence: Math.round(out.confidence),
      recommendation: out.recommendation.replace(/\s+/g, " ").trim(),
      report: out.report
        .split("\n")
        .map((line) => line.replace(/[ \t]+/g, " ").trimEnd())
        .join("\n")
        .replace(/\n{3,}/g, "\n\n")
        .trim(),
      highlights: out.highlights.map((h) => h.replace(/\s+/g, " ").trim()).filter(Boolean),
      flags: out.flags.map((f) => f.replace(/\s+/g, " ").trim()).filter(Boolean),
    }

    const agentFields = preScreen
      ? {
          agentSummary: (out.agentSummary ?? "").replace(/\s+/g, " ").trim(),
          narrativeChecks: (out.narrativeChecks ?? []).map((c) => ({
            kind: c.kind,
            label: c.label.replace(/\s+/g, " ").trim(),
            outcome: c.outcome,
            confidence: Math.round(c.confidence),
            finding: c.finding.replace(/\s+/g, " ").trim(),
            observations: c.observations.map((o) => o.replace(/\s+/g, " ").trim()).filter(Boolean),
          })),
        }
      : {}

    return Response.json({
      ...cleaned,
      ...agentFields,
      model,
      generatedAt: new Date().toISOString(),
    })
  } catch (err) {
    console.error("[v0] assessment error:", err instanceof Error ? err.message : err)
    logAiError("assessment", err)
    const { body, status } = friendlyJsonError(err, "Could not generate the assessment.")
    return Response.json(body, { status })
  }
}
