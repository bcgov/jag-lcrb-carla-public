import { generateText, Output } from "ai"
import { z } from "zod"
import { retrieveKnowledge } from "@server-lib/ai/knowledge-base"
import { logAiError, AI_MAX_RETRIES, friendlyJsonError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

export const maxDuration = 30

/** "What does this status mean?" explainer: plain-language explanation of an application/permit/licence status and what happens next, grounded in the policy knowledge base. Read-only. */

const ExplanationSchema = z.object({
  meaning: z
    .string()
    .describe("2-3 plain-language sentences explaining what the current status means for the applicant."),
  whatsNext: z
    .array(z.string())
    .describe("2-4 short bullet points describing what happens next or what the applicant should do."),
  actionRequired: z
    .boolean()
    .describe("True if the applicant must take an action now (e.g. resubmit, provide documents); false if they just wait."),
})

interface HistoryItem {
  action: string
  actor?: string
  date?: string
  note?: string
}

interface ExplainRequest {
  subjectType: string // e.g. "Special Event Permit", "Food Primary application", "licence"
  title: string
  status: string
  statusLabel?: string
  history?: HistoryItem[]
  extra?: string
}

export async function POST(req: Request) {
  let body: ExplainRequest
  try {
    body = (await req.json()) as ExplainRequest
  } catch {
    return Response.json({ error: "Invalid request body." }, { status: 400 })
  }

  const { subjectType, title, status, statusLabel, history, extra } = body
  if (!subjectType || !status) {
    return Response.json({ error: "Missing subject or status." }, { status: 400 })
  }

  const historyText =
    history && history.length
      ? history
          .map((h) => `- ${h.action}${h.actor ? ` by ${h.actor}` : ""}${h.date ? ` (${h.date})` : ""}${h.note ? `: ${h.note}` : ""}`)
          .join("\n")
      : "(no history recorded)"

  // Ground the explanation in policy material relevant to this subject + status.
  const retrieved = retrieveKnowledge(`${subjectType} ${statusLabel ?? status} what happens next review process`, {
    limit: 4,
  })
  const excerpts = retrieved.length
    ? retrieved.map((c, i) => `[${i + 1}] ${c.doc}${c.heading ? ` — ${c.heading}` : ""}\n${c.text}`).join("\n\n")
    : "No specific policy excerpts matched."

  const systemPrompt = `You explain the status of a BC liquor/cannabis licensing item to the member of the public who submitted it. Be warm, clear, and concise — write for the general public, not lawyers.

Rules:
- Explain what the CURRENT status means in 2-3 plain sentences.
- In "whatsNext", give 2-4 short, concrete bullets: what the branch/reviewers will do next and/or what the applicant should do.
- Set "actionRequired" true ONLY if the applicant must act now (e.g. status indicates changes requested, documents requested, payment needed, or denial they can respond to). Otherwise false.
- Base process/timeline statements on the retrieved policy excerpts when available; do NOT invent specific fees, statutory deadlines, or numbers that aren't supported. If unknown, keep it general ("a reviewer will assess your application").
- Never restate personal data unnecessarily. Do not be alarming.`

  try {
    const { experimental_output } = await generateText({
      model: AI_MODEL,
      providerOptions: AI_PROVIDER_OPTIONS,
      maxRetries: AI_MAX_RETRIES,
      system: systemPrompt,
      experimental_output: Output.object({ schema: ExplanationSchema }),
      prompt: `Explain this status to the applicant.

ITEM TYPE: ${subjectType}
TITLE: ${title}
CURRENT STATUS: ${statusLabel ?? status} (code: ${status})
${extra ? `ADDITIONAL CONTEXT: ${extra}\n` : ""}
HISTORY (most recent context):
${historyText}

RELEVANT POLICY EXCERPTS:
${excerpts}`,
    })

    const out = experimental_output
    return Response.json({
      meaning: out.meaning.replace(/\s+/g, " ").trim(),
      whatsNext: out.whatsNext.map((s) => s.replace(/\s+/g, " ").trim()).filter(Boolean),
      actionRequired: out.actionRequired,
    })
  } catch (err) {
    console.error("[v0] status-explainer error:", err instanceof Error ? err.message : err)
    logAiError("status-explainer", err)
    const { body, status } = friendlyJsonError(err, "Could not generate an explanation.")
    return Response.json(body, { status })
  }
}
