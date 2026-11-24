import { generateText, Output } from "ai"
import { z } from "zod"
import { retrieveKnowledge } from "@server-lib/ai/knowledge-base"
import { logAiError, AI_MAX_RETRIES, friendlyJsonError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

export const maxDuration = 30

/** Reviewer Brief: staff-only AI triage summary (risk, items to check, recommended decision the reviewer can override), grounded in the policy knowledge base. */

const BriefSchema = z.object({
  summary: z
    .string()
    .describe("A 2-3 sentence reviewer brief: what this is, overall readiness, and the headline consideration."),
  riskLevel: z.enum(["low", "medium", "high"]).describe("Overall risk / attention level for the reviewer."),
  attentionItems: z
    .array(z.string())
    .describe("Specific things the reviewer should check: missing documents, risk factors, inconsistencies. Empty if none."),
  recommendedDecision: z
    .string()
    .describe("The single recommended action, chosen EXACTLY from the provided decision options."),
  rationale: z.string().describe("One short sentence explaining why this decision is recommended."),
})

interface Fact {
  label: string
  value: string
}

interface BriefRequest {
  subjectType: string // e.g. "Special Event Permit", "Liquor Primary application referral"
  title: string
  reviewerRole: string // "police" | "local government" | ...
  facts: Fact[]
  signals?: string[] // pre-computed risk flags, e.g. "No site plan provided"
  decisionOptions: string[] // the allowed decisions on this screen
  history?: { action: string; actor?: string; date?: string; note?: string }[]
}

export async function POST(req: Request) {
  let body: BriefRequest
  try {
    body = (await req.json()) as BriefRequest
  } catch {
    return Response.json({ error: "Invalid request body." }, { status: 400 })
  }

  const { subjectType, title, reviewerRole, facts, signals, decisionOptions, history } = body
  if (!subjectType || !Array.isArray(facts) || !Array.isArray(decisionOptions) || decisionOptions.length === 0) {
    return Response.json({ error: "Missing brief data." }, { status: 400 })
  }

  const factsText = facts.length ? facts.map((f) => `- ${f.label}: ${f.value}`).join("\n") : "(none provided)"
  const signalsText = signals && signals.length ? signals.map((s) => `- ${s}`).join("\n") : "(none auto-detected)"
  const historyText =
    history && history.length
      ? history.map((h) => `- ${h.action}${h.note ? `: ${h.note}` : ""}`).join("\n")
      : "(no history)"

  const retrieved = retrieveKnowledge(`${subjectType} review assessment risk conditions ${reviewerRole}`, {
    limit: 4,
  })
  const excerpts = retrieved.length
    ? retrieved.map((c, i) => `[${i + 1}] ${c.doc}${c.heading ? ` — ${c.heading}` : ""}\n${c.text}`).join("\n\n")
    : "No specific policy excerpts matched."

  const systemPrompt = `You are an assessment assistant for the BC Liquor & Cannabis Regulation Branch (LCRB). You prepare a concise REVIEWER BRIEF for a ${reviewerRole} reviewer assessing a "${subjectType}". This is seen only by staff, never the applicant.

Your brief helps the reviewer decide quickly and consistently:
- "summary": 2-3 sentences — what this is and the overall picture.
- "riskLevel": low / medium / high, based on the facts, signals, and policy.
- "attentionItems": concrete things to check (missing docs, public-safety factors, inconsistencies, proximity/sensitivity issues). Empty array if genuinely none.
- "recommendedDecision": MUST be chosen verbatim from this exact list of allowed options: ${decisionOptions.map((d) => `"${d}"`).join(", ")}.
- "rationale": one sentence justifying the recommendation.

Rules:
- Ground risk/process judgments in the retrieved policy excerpts where relevant. Do not invent facts, statutory thresholds, or numbers not supported by the data.
- Be objective and proportionate. A clean, low-attendance event with no flags is low risk; do not manufacture concerns.
- The recommendation is advisory — the human reviewer decides. Be helpful, not prescriptive.`

  try {
    const { experimental_output } = await generateText({
      model: AI_MODEL,
      providerOptions: AI_PROVIDER_OPTIONS,
      maxRetries: AI_MAX_RETRIES,
      system: systemPrompt,
      experimental_output: Output.object({ schema: BriefSchema }),
      prompt: `Prepare the reviewer brief.

ITEM: ${title} (${subjectType})

FACTS:
${factsText}

AUTO-DETECTED SIGNALS:
${signalsText}

HISTORY:
${historyText}

RELEVANT POLICY EXCERPTS:
${excerpts}`,
    })

    const out = experimental_output
    // Snap the recommendation to an allowed option if the model drifted.
    const match =
      decisionOptions.find((d) => d.toLowerCase() === out.recommendedDecision.trim().toLowerCase()) ??
      decisionOptions.find((d) => out.recommendedDecision.toLowerCase().includes(d.toLowerCase())) ??
      out.recommendedDecision.trim()

    return Response.json({
      summary: out.summary.replace(/\s+/g, " ").trim(),
      riskLevel: out.riskLevel,
      attentionItems: out.attentionItems.map((s) => s.replace(/\s+/g, " ").trim()).filter(Boolean),
      recommendedDecision: match,
      rationale: out.rationale.replace(/\s+/g, " ").trim(),
    })
  } catch (err) {
    console.error("[v0] reviewer-brief error:", err instanceof Error ? err.message : err)
    logAiError("reviewer-brief", err)
    const { body, status } = friendlyJsonError(err, "Could not generate the reviewer brief.")
    return Response.json(body, { status })
  }
}
