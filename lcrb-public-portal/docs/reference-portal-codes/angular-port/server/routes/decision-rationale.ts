import { generateText } from "ai"
import { retrieveKnowledge } from "@server-lib/ai/knowledge-base"
import { logAiError, AI_MAX_RETRIES, friendlyJsonError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

export const maxDuration = 30

/** Drafts applicant-facing decision rationale text for the police and local-government decision panels; the reviewer edits it before sending. */
export async function POST(req: Request) {
  try {
    const body = await req.json()
    const {
      decision, // e.g. "Deny permit", "Request changes", "Recommend rejection"
      tone, // "deny" | "changes" | "approve" | "neutral"
      subjectType,
      title,
      reviewerRole,
      facts = [],
      signals = [],
      conditions = [],
      reviewerNotes = "",
    } = body as {
      decision: string
      tone?: string
      subjectType: string
      title: string
      reviewerRole: string
      facts?: { label: string; value: string }[]
      signals?: string[]
      conditions?: string[]
      reviewerNotes?: string
    }

    if (!decision || !subjectType) {
      return Response.json({ error: "Missing decision or subjectType" }, { status: 400 })
    }

    // Ground the language in real policy so the rationale cites the right reasons.
    const kb = retrieveKnowledge(`${decision} ${subjectType} ${signals.join(" ")}`, { limit: 4 })
    const policyContext = kb
      .map((k) => `- ${k.doc}${k.heading ? ` — ${k.heading}` : ""}: ${k.text}`)
      .join("\n")

    const factLines = facts.map((f) => `- ${f.label}: ${f.value}`).join("\n")
    const signalLines = signals.length ? signals.map((s) => `- ${s}`).join("\n") : "- (none flagged)"
    const conditionLines = conditions.length ? conditions.map((c) => `- ${c}`).join("\n") : "- (none)"

    const result = await generateText({
      model: AI_MODEL,
      providerOptions: AI_PROVIDER_OPTIONS,
      maxRetries: AI_MAX_RETRIES,
      system: [
        `You draft applicant-facing decision rationale text for a ${reviewerRole} reviewer in British Columbia's Liquor and Cannabis licensing system.`,
        "The reviewer will read and edit your draft before it is sent, so produce a clear, complete starting point.",
        "Write in plain, respectful, professional language addressed to the applicant.",
        "Be specific: reference the actual facts and any policy basis. Do NOT invent facts, dates, names, or statute numbers that are not provided.",
        "Output 2-4 short paragraphs of prose only. No salutation, no signature, no markdown headings, no bullet characters.",
        tone === "deny"
          ? "This is a DENIAL. State the decision plainly, give the specific reasons, and note any avenue to reapply if applicable."
          : tone === "changes"
            ? "This is a REQUEST FOR CHANGES (not a denial). Clearly list what the applicant must provide or fix, and that the file remains open pending their response."
            : tone === "approve"
              ? "This is an APPROVAL. Confirm the decision and clearly restate any conditions the applicant must follow."
              : "Summarize the recommendation and its reasoning.",
      ].join("\n"),
      prompt: [
        `Decision: ${decision}`,
        `Subject: ${subjectType} — "${title}"`,
        "",
        "Key facts:",
        factLines || "- (none provided)",
        "",
        "Flagged signals:",
        signalLines,
        "",
        "Conditions applied:",
        conditionLines,
        "",
        reviewerNotes ? `Reviewer's own notes to incorporate: ${reviewerNotes}` : "Reviewer provided no extra notes.",
        "",
        "Relevant policy context (use only what is relevant; do not quote verbatim):",
        policyContext || "- (none found)",
        "",
        "Write the applicant-facing rationale now.",
      ].join("\n"),
    })

    return Response.json({ text: result.text.trim() })
  } catch (err) {
    console.error("[v0] decision-rationale error:", err instanceof Error ? err.message : err)
    logAiError("decision-rationale", err)
    const { body, status } = friendlyJsonError(err, "Failed to draft rationale.")
    return Response.json(body, { status })
  }
}
