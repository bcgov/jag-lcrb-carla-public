import { generateText, Output } from "ai"
import { z } from "zod"
import { retrieveKnowledge } from "@server-lib/ai/knowledge-base"
import { logAiError, AI_MAX_RETRIES, friendlyJsonError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

export const maxDuration = 30

/** Advisory "AI Suggest Actions" for a reviewer-queue item; reviewer-side twin of /api/licence-actions, kept separate because framing and grounding differ. */

type ReviewerRole = "police" | "local-government"

const SuggestionSchema = z.object({
  suggestions: z
    .array(
      z.object({
        title: z
          .string()
          .describe("A short, action-oriented next step for the reviewer (max ~8 words), e.g. 'Assign this file to yourself'."),
        reason: z
          .string()
          .describe("One plain-language sentence explaining why this matters now, specific to this item's facts."),
        urgency: z
          .enum(["urgent", "soon", "routine"])
          .describe("urgent = time-sensitive/overdue; soon = plan in coming days; routine = good practice."),
        opensReview: z
          .boolean()
          .describe(
            "true if completing this step means opening this item's review page (e.g. review, submit comment, request info). false for purely informational/coordination steps.",
          ),
      }),
    )
    .describe("2-4 suggested next actions for the reviewer, most important first."),
})

interface ReviewerFact {
  label: string
  value: string
}

interface ReviewerRequest {
  role: ReviewerRole
  itemTitle: string
  itemId: string
  status: string
  facts: ReviewerFact[]
}

const ROLE_CONFIG: Record<
  ReviewerRole,
  { persona: string; itemNoun: string; knowledgeQuery: string; guidance: string }
> = {
  police: {
    persona:
      "a BC police reviewer assessing a Special Event Permit (SEP) application for public-safety risk before the event happens",
    itemNoun: "Special Event Permit application",
    knowledgeQuery:
      "special event permit police review public safety security plan attendance approval conditions",
    guidance: `Prioritise by real signals:
- If the file is UNASSIGNED and needs review, the first action is to assign it to yourself.
- The event DATE is a hard deadline — an event only days away makes review urgent; a large event (attendance over the review threshold) warrants confirming the security/policing plan.
- "Changes requested" means the file is waiting on the applicant — suggest monitoring or following up rather than re-reviewing.
- For small events under the threshold, a quick confirmation review is routine.`,
  },
  "local-government": {
    persona:
      "a BC Local Government / Indigenous relations reviewer providing comment on a liquor or cannabis licence application routed to your jurisdiction",
    itemNoun: "licence application routed for local-government comment",
    knowledgeQuery:
      "local government comment period liquor cannabis licence application resolution community input Indigenous nation referral",
    guidance: `Prioritise by real signals:
- The COMMENT DUE DATE governs everything — if it is overdue or within ~2 weeks, submitting your comment/resolution is urgent.
- "Documents requested" means you are waiting on the applicant/LCRB — suggest following up or monitoring, not re-reviewing.
- "Community review" means gathering local input — suggest advancing or concluding that consultation before the deadline.
- If comment is already submitted, there is usually nothing left to do.`,
  },
}

export async function POST(req: Request) {
  let body: ReviewerRequest
  try {
    body = (await req.json()) as ReviewerRequest
  } catch {
    return Response.json({ error: "Invalid request body." }, { status: 400 })
  }

  const { role, itemTitle, itemId, status, facts } = body

  if (!role || !ROLE_CONFIG[role] || !status || !Array.isArray(facts)) {
    return Response.json({ error: "Missing or invalid reviewer item data." }, { status: 400 })
  }

  const cfg = ROLE_CONFIG[role]
  const factsText = facts.length
    ? facts.map((f) => `- ${f.label}: ${f.value}`).join("\n")
    : "(no additional facts provided)"

  const retrieved = retrieveKnowledge(`${cfg.knowledgeQuery} ${status}`, { limit: 4 })
  const excerpts = retrieved.length
    ? retrieved.map((c, i) => `[${i + 1}] ${c.doc}${c.heading ? ` — ${c.heading}` : ""}\n${c.text}`).join("\n\n")
    : "No specific policy excerpts matched."

  const systemPrompt = `You advise ${cfg.persona}. You are looking at ONE ${cfg.itemNoun} in this reviewer's work queue and recommending what they should do next on it. Write for a busy professional reviewer — practical and specific, not legalistic.

Return 2-4 suggestions, most important first:
- "title": short and action-oriented, addressed to the reviewer.
- "reason": ONE sentence, specific to THIS item's actual facts (its status, deadlines, assignment, size).
- "urgency": "urgent", "soon", or "routine".
- "opensReview": true when the step is done on the item's review page (review it, submit a comment, request information); false for coordination/monitoring steps.

${cfg.guidance}

Rules:
- Base everything on the provided facts. Do NOT invent deadlines, statutory timelines, or numbers not supported by the facts or excerpts.
- Do not repeat the same action twice. Be concise.
- You advise only; the reviewer makes the decision.`

  try {
    const { experimental_output } = await generateText({
      model: AI_MODEL,
      providerOptions: AI_PROVIDER_OPTIONS,
      maxRetries: AI_MAX_RETRIES,
      system: systemPrompt,
      experimental_output: Output.object({ schema: SuggestionSchema }),
      prompt: `Suggest next actions for this ${cfg.itemNoun}.

ITEM: ${itemTitle} (${itemId})
STATUS: ${status}
FACTS:
${factsText}

RELEVANT POLICY EXCERPTS:
${excerpts}`,
    })

    const suggestions = experimental_output.suggestions.slice(0, 4).map((s) => ({
      title: s.title.replace(/\s+/g, " ").trim(),
      reason: s.reason.replace(/\s+/g, " ").trim(),
      urgency: s.urgency,
      opensReview: Boolean(s.opensReview),
    }))

    return Response.json({ suggestions })
  } catch (err) {
    console.error("[v0] reviewer-actions error:", err instanceof Error ? err.message : err)
    logAiError("reviewer-actions", err)
    const { body, status } = friendlyJsonError(err, "Could not generate suggestions.")
    return Response.json(body, { status })
  }
}
