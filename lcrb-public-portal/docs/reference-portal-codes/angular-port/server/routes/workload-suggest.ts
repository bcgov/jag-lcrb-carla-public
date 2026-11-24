import { generateText, Output } from "ai"
import { z } from "zod"
import { retrieveKnowledge } from "@server-lib/ai/knowledge-base"
import { logAiError, AI_MAX_RETRIES, friendlyJsonError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

export const maxDuration = 30

/** Portfolio-level "AI Suggest" for licensee, Police and LG dashboards. Suggestions may use only the provided links by exact label, so no invented URLs. */

type Scope = "compliance" | "police" | "local-government"

const SuggestionSchema = z.object({
  suggestions: z
    .array(
      z.object({
        title: z
          .string()
          .describe("A short, action-oriented next step (max ~9 words), e.g. 'Triage the 3 unassigned files'."),
        reason: z
          .string()
          .describe("One plain-language sentence explaining why this matters now, grounded in the provided stats/items."),
        urgency: z
          .enum(["urgent", "soon", "routine"])
          .describe("urgent = overdue/time-critical; soon = act within days; routine = good practice."),
        linkLabel: z
          .string()
          .nullable()
          .describe(
            "If this action maps to one of the provided navigation links, the EXACT label of that link. Use null if none applies.",
          ),
      }),
    )
    .describe("2-4 prioritized suggestions for the whole workload, most important first."),
})

interface WorkloadRequest {
  scope: Scope
  /** Headline metrics, e.g. { label: 'Unassigned', value: '3' }. */
  stats?: { label: string; value: string }[]
  /** The most relevant open items with a short signal line each. */
  items?: { label: string; detail: string }[]
  /** Navigation the user has available; suggestions may reference these by label. */
  links?: { label: string; href: string }[]
}

const SCOPE_CONFIG: Record<Scope, { persona: string; noun: string; knowledgeQuery: string; guidance: string }> = {
  compliance: {
    persona:
      "a BC liquor/cannabis licensee reviewing their own compliance and enforcement standing in the LCRB portal",
    noun: "compliance & enforcement portfolio",
    knowledgeQuery:
      "enforcement action response deadline hearing contravention monetary penalty compliance inspection licensee",
    guidance: `Prioritise by real signals:
- An enforcement action AWAITING RESPONSE with a deadline is the top priority — an overdue or soon-due response risks losing the right to a hearing.
- Recent contraventions or penalties suggest reviewing the compliance history and addressing root causes.
- If there are no open actions, keep suggestions light and routine (e.g. review inspection history).`,
  },
  police: {
    persona:
      "a BC police reviewer managing a queue of Special Event Permit (SEP) applications to assess for public-safety risk before events happen",
    noun: "SEP review queue",
    knowledgeQuery:
      "special event permit police review public safety security plan attendance approval unassigned deadline",
    guidance: `Prioritise by real signals:
- UNASSIGNED files that need review should be assigned/triaged first.
- Events happening SOON are hard deadlines — near-term or large events (high expected attendance) are urgent.
- "Changes requested" files are waiting on the applicant — monitor rather than re-review.
- Auto-approved events are for awareness only; flag only if something looks concerning.`,
  },
  "local-government": {
    persona:
      "a BC Local Government / Indigenous relations reviewer providing comment on liquor and cannabis licence applications routed to your jurisdiction",
    noun: "local-government comment queue",
    knowledgeQuery:
      "local government comment period liquor cannabis licence application resolution deadline referral community input",
    guidance: `Prioritise by real signals:
- The COMMENT DEADLINE governs everything — overdue or within ~2 weeks means submitting the comment/resolution is urgent.
- "Documents requested" means waiting on the applicant/LCRB — follow up or monitor.
- "Community review" means gathering local input — advance or conclude it before the deadline.
- If comment is already submitted, there is usually nothing left to do.`,
  },
}

export async function POST(req: Request) {
  let body: WorkloadRequest
  try {
    body = (await req.json()) as WorkloadRequest
  } catch {
    return Response.json({ error: "Invalid request body." }, { status: 400 })
  }

  const { scope, stats = [], items = [], links = [] } = body
  if (!scope || !SCOPE_CONFIG[scope]) {
    return Response.json({ error: "Missing or invalid workload scope." }, { status: 400 })
  }

  const cfg = SCOPE_CONFIG[scope]
  const statsText = stats.length ? stats.map((s) => `- ${s.label}: ${s.value}`).join("\n") : "(no stats provided)"
  const itemsText = items.length
    ? items.map((i) => `- ${i.label}: ${i.detail}`).join("\n")
    : "(no individual items provided)"
  const linksText = links.length
    ? links.map((l) => `- "${l.label}"`).join("\n")
    : "(no navigation links provided)"

  const retrieved = retrieveKnowledge(cfg.knowledgeQuery, { limit: 4 })
  const excerpts = retrieved.length
    ? retrieved.map((c, i) => `[${i + 1}] ${c.doc}${c.heading ? ` — ${c.heading}` : ""}\n${c.text}`).join("\n\n")
    : "No specific policy excerpts matched."

  const systemPrompt = `You advise ${cfg.persona}. You are looking at their whole ${cfg.noun} and recommending what to focus on next across it. Write for a busy professional — practical and specific, not legalistic.

Return 2-4 suggestions, most important first:
- "title": short and action-oriented.
- "reason": ONE sentence, grounded in the ACTUAL stats and items provided (counts, deadlines, sizes).
- "urgency": "urgent", "soon", or "routine".
- "linkLabel": if the action maps to one of the available navigation links, copy that link's EXACT label; otherwise omit it.

${cfg.guidance}

Rules:
- Base everything on the provided stats and items. Do NOT invent counts, deadlines, or numbers not present in the data.
- Only use a "linkLabel" that exactly matches one of the provided navigation links.
- Do not repeat the same action twice. Be concise.
- You advise only; the person makes the decision.`

  try {
    const { experimental_output } = await generateText({
      model: AI_MODEL,
      providerOptions: AI_PROVIDER_OPTIONS,
      maxRetries: AI_MAX_RETRIES,
      system: systemPrompt,
      experimental_output: Output.object({ schema: SuggestionSchema }),
      prompt: `Suggest what to focus on next across this ${cfg.noun}.

HEADLINE STATS:
${statsText}

MOST RELEVANT OPEN ITEMS:
${itemsText}

AVAILABLE NAVIGATION LINKS (reference by exact label if relevant):
${linksText}

RELEVANT POLICY EXCERPTS:
${excerpts}`,
    })

    const validLabels = new Set(links.map((l) => l.label))
    const suggestions = experimental_output.suggestions.slice(0, 4).map((s) => ({
      title: s.title.replace(/\s+/g, " ").trim(),
      reason: s.reason.replace(/\s+/g, " ").trim(),
      urgency: s.urgency,
      // Only keep a link the client actually offered; drop anything invented.
      linkLabel: s.linkLabel && validLabels.has(s.linkLabel) ? s.linkLabel : undefined,
    }))

    return Response.json({ suggestions })
  } catch (err) {
    console.error("[v0] workload-suggest error:", err instanceof Error ? err.message : err)
    logAiError("workload-suggest", err)
    const { body: errBody, status } = friendlyJsonError(err, "Could not generate suggestions.")
    return Response.json(errBody, { status })
  }
}
