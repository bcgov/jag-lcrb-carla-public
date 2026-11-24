import { generateText, Output } from "ai"
import { z } from "zod"
import { retrieveKnowledge } from "@server-lib/ai/knowledge-base"
import { logAiError, AI_MAX_RETRIES, friendlyJsonError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

export const maxDuration = 30

/** "AI Suggest Actions" for one licence: short prioritized next actions with rationale, urgency and application slug. Advisory and read-only. */

const SuggestionSchema = z.object({
  suggestions: z
    .array(
      z.object({
        title: z.string().describe("A short, action-oriented suggestion (max ~8 words), e.g. 'Renew before expiry'."),
        reason: z
          .string()
          .describe("One plain-language sentence explaining why this is worth doing now, specific to this licence."),
        urgency: z
          .enum(["urgent", "soon", "routine"])
          .describe("urgent = act now / time-sensitive; soon = plan in the coming weeks; routine = good housekeeping."),
        actionSlug: z
          .string()
          .describe(
            "The slug of a portal request that fulfils this suggestion, chosen EXACTLY from the provided available actions. Use an empty string for purely informational suggestions with no matching request.",
          ),
      }),
    )
    .describe("2-4 suggested next actions, most important first."),
})

interface AvailableAction {
  slug: string
  title: string
  description: string
}

interface SuggestRequest {
  licenceType: string
  licenceName: string
  status: string
  expiryDate: string
  daysToExpiry: number
  flags: string[]
  isOperated: boolean
  hasScheduledInspection: boolean
  nextInspectionDate?: string
  availableActions: AvailableAction[]
}

export async function POST(req: Request) {
  let body: SuggestRequest
  try {
    body = (await req.json()) as SuggestRequest
  } catch {
    return Response.json({ error: "Invalid request body." }, { status: 400 })
  }

  const {
    licenceType,
    licenceName,
    status,
    expiryDate,
    daysToExpiry,
    flags,
    isOperated,
    hasScheduledInspection,
    nextInspectionDate,
    availableActions,
  } = body

  if (!licenceType || !status || !Array.isArray(availableActions)) {
    return Response.json({ error: "Missing licence data." }, { status: 400 })
  }

  const allowedSlugs = availableActions.map((a) => a.slug)
  const actionsText = availableActions.length
    ? availableActions.map((a) => `- slug "${a.slug}": ${a.title} — ${a.description}`).join("\n")
    : "(no self-serve request types available for this licence)"

  const flagsText = flags.length ? flags.join(", ") : "none"

  // Ground suggestions in policy material relevant to this licence + situation.
  const retrieved = retrieveKnowledge(
    `${licenceType} licence renewal obligations conditions inspection ${status}`,
    { limit: 4 },
  )
  const excerpts = retrieved.length
    ? retrieved.map((c, i) => `[${i + 1}] ${c.doc}${c.heading ? ` — ${c.heading}` : ""}\n${c.text}`).join("\n\n")
    : "No specific policy excerpts matched."

  const systemPrompt = `You advise a BC liquor/cannabis business owner on sensible next actions for ONE of their licences, shown on their Licences & Authorizations page. Be practical, specific, and encouraging — write for a busy business owner, not a lawyer.

Return 2-4 suggestions, most important first:
- "title": short and action-oriented.
- "reason": ONE sentence, specific to this licence's actual situation (its status, expiry timing, flags, inspection).
- "urgency": "urgent" (time-sensitive, e.g. expiry imminent, expired, or an action already requested that needs completion), "soon" (worth planning in coming weeks), or "routine" (good housekeeping).
- "actionSlug": if a listed available action fulfils the suggestion, use its EXACT slug; otherwise use an empty string for informational suggestions (e.g. "prepare for your inspection").

Rules:
- Prioritise by real signals: an imminent/passed expiry or "Pending Renewal" status makes renewal urgent; a "Transfer Requested" flag means suggest completing/monitoring that transfer; an upcoming inspection means suggest preparing for it.
- ${
    isOperated
      ? "This licence is OPERATED by a third party. Third-party operators CANNOT request changes to the licence, so do NOT recommend change requests — focus on informational/monitoring actions (leave actionSlug empty) such as coordinating with the licensee, preparing for inspections, or watching the expiry date."
      : "Only recommend actions the owner can actually take; use actionSlug empty for informational suggestions."
  }
- Do NOT invent fees, statutory deadlines, or numbers not supported by the excerpts. Keep timeline claims general when unsure.
- Never recommend the same action twice. Be concise.`

  try {
    const { experimental_output } = await generateText({
      model: AI_MODEL,
      providerOptions: AI_PROVIDER_OPTIONS,
      maxRetries: AI_MAX_RETRIES,
      system: systemPrompt,
      experimental_output: Output.object({ schema: SuggestionSchema }),
      prompt: `Suggest next actions for this licence.

LICENCE: ${licenceName} (${licenceType})
STATUS: ${status}
EXPIRY DATE: ${expiryDate} (${
        daysToExpiry < 0 ? `expired ${Math.abs(daysToExpiry)} days ago` : `${daysToExpiry} days away`
      })
FLAGS: ${flagsText}
OPERATED BY THIRD PARTY: ${isOperated ? "yes" : "no"}
UPCOMING INSPECTION: ${
        hasScheduledInspection ? `yes${nextInspectionDate ? ` on ${nextInspectionDate}` : ""}` : "none scheduled"
      }

AVAILABLE ACTIONS (use these slugs for actionSlug):
${actionsText}

RELEVANT POLICY EXCERPTS:
${excerpts}`,
    })

    const out = experimental_output
    // Snap each actionSlug to an allowed slug; blank out anything the model invented.
    const suggestions = out.suggestions.slice(0, 4).map((s) => {
      const raw = (s.actionSlug ?? "").trim()
      const matched = allowedSlugs.find((slug) => slug.toLowerCase() === raw.toLowerCase()) ?? ""
      return {
        title: s.title.replace(/\s+/g, " ").trim(),
        reason: s.reason.replace(/\s+/g, " ").trim(),
        urgency: s.urgency,
        actionSlug: matched,
      }
    })

    return Response.json({ suggestions })
  } catch (err) {
    console.error("[v0] licence-actions error:", err instanceof Error ? err.message : err)
    logAiError("licence-actions", err)
    const { body, status } = friendlyJsonError(err, "Could not generate suggestions.")
    return Response.json(body, { status })
  }
}
