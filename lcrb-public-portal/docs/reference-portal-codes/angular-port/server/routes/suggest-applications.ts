import { generateText, Output } from "ai"
import { z } from "zod"
import { retrieveKnowledge } from "@server-lib/ai/knowledge-base"
import { logAiError, AI_MAX_RETRIES, friendlyJsonError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

export const maxDuration = 30

/** "AI Suggest Applications" for the new-application catalog: prioritized picks with rationale, urgency and exact slug. Advisory and read-only. */

const SuggestionSchema = z.object({
  suggestions: z
    .array(
      z.object({
        title: z
          .string()
          .describe("A short, action-oriented suggestion naming the application (max ~9 words)."),
        reason: z
          .string()
          .describe(
            "One plain-language sentence explaining why this application is relevant to THIS business right now, referencing their specific licences where possible.",
          ),
        urgency: z
          .enum(["urgent", "soon", "routine"])
          .describe("urgent = time-sensitive; soon = plan in the coming weeks; routine = worth considering."),
        appSlug: z
          .string()
          .describe(
            "The slug of the catalog application that fulfils this suggestion, chosen EXACTLY from the provided available applications.",
          ),
      }),
    )
    .describe("2-4 suggested applications, most important first."),
})

interface CatalogItem {
  slug: string
  title: string
  description: string
  licenceType: string
  kind: string
}

interface HeldLicence {
  type: string
  name: string
  status: string
  expiryDate: string
  daysToExpiry: number
  flags: string[]
}

interface SuggestRequest {
  legalName: string
  licences: HeldLicence[]
  availableApplications: CatalogItem[]
}

export async function POST(req: Request) {
  let body: SuggestRequest
  try {
    body = (await req.json()) as SuggestRequest
  } catch {
    return Response.json({ error: "Invalid request body." }, { status: 400 })
  }

  const { legalName, licences, availableApplications } = body

  if (!Array.isArray(licences) || !Array.isArray(availableApplications)) {
    return Response.json({ error: "Missing business or catalog data." }, { status: 400 })
  }

  const allowedSlugs = availableApplications.map((a) => a.slug)

  const licencesText = licences.length
    ? licences
        .map(
          (l) =>
            `- ${l.name} (${l.type}) — status ${l.status}; expires ${l.expiryDate} (${
              l.daysToExpiry < 0 ? `expired ${Math.abs(l.daysToExpiry)} days ago` : `${l.daysToExpiry} days away`
            })${l.flags.length ? `; flags: ${l.flags.join(", ")}` : ""}`,
        )
        .join("\n")
    : "(this business currently holds no licences)"

  const catalogText = availableApplications.length
    ? availableApplications
        .map((a) => `- slug "${a.slug}" [${a.licenceType} · ${a.kind}]: ${a.title} — ${a.description}`)
        .join("\n")
    : "(no applications available)"

  // Ground suggestions in policy material relevant to the business's holdings.
  const licenceTypes = Array.from(new Set(licences.map((l) => l.type))).join(", ")
  const retrieved = retrieveKnowledge(
    `${licenceTypes} licence renewal structural change patio endorsement application obligations`,
    { limit: 4 },
  )
  const excerpts = retrieved.length
    ? retrieved.map((c, i) => `[${i + 1}] ${c.doc}${c.heading ? ` — ${c.heading}` : ""}\n${c.text}`).join("\n\n")
    : "No specific policy excerpts matched."

  const systemPrompt = `You help a BC liquor/cannabis business owner decide which application or authorization to start next, on the "Start a new application" page. Write for a busy business owner — practical, specific, and encouraging.

Return 2-4 suggestions, most important first:
- "title": short and action-oriented, naming the application.
- "reason": ONE sentence tied to THIS business's actual holdings (their licence types, statuses, expiry timing, and flags).
- "urgency": "urgent" (time-sensitive, e.g. a renewal for a licence expiring soon or already flagged for renewal), "soon" (worth planning in the coming weeks), or "routine" (a sensible option to consider).
- "appSlug": the EXACT slug of a listed available application that fulfils the suggestion.

Rules:
- Prioritise by real signals: a licence that is "Pending Renewal", flagged "Renewal Due", or expiring soon makes a renewal urgent; a "Transfer Requested" flag means suggest actions related to that transfer; match the suggested application's licence type to a licence the business actually holds.
- ONLY recommend applications from the provided available list, and set appSlug to one of their exact slugs. Never invent a slug.
- Do NOT recommend the same application twice.
- Do NOT invent fees, statutory deadlines, or numbers not supported by the excerpts. Keep timeline claims general when unsure.
- If the business holds a licence of a given type, prefer authorizations/changes for that type over unrelated new-licence applications. Be concise.`

  try {
    const { experimental_output } = await generateText({
      model: AI_MODEL,
      providerOptions: AI_PROVIDER_OPTIONS,
      maxRetries: AI_MAX_RETRIES,
      system: systemPrompt,
      experimental_output: Output.object({ schema: SuggestionSchema }),
      prompt: `Suggest which applications this business should consider starting.

BUSINESS: ${legalName || "the licensee"}

LICENCES HELD:
${licencesText}

AVAILABLE APPLICATIONS (use these slugs for appSlug):
${catalogText}

RELEVANT POLICY EXCERPTS:
${excerpts}`,
    })

    const out = experimental_output
    // Snap each appSlug to an allowed slug; drop anything the model invented.
    const suggestions = out.suggestions
      .map((s) => {
        const raw = (s.appSlug ?? "").trim()
        const matched = allowedSlugs.find((slug) => slug.toLowerCase() === raw.toLowerCase()) ?? ""
        return {
          title: s.title.replace(/\s+/g, " ").trim(),
          reason: s.reason.replace(/\s+/g, " ").trim(),
          urgency: s.urgency,
          appSlug: matched,
        }
      })
      .filter((s) => s.appSlug)
      .slice(0, 4)

    return Response.json({ suggestions })
  } catch (err) {
    console.error("[v0] suggest-applications error:", err instanceof Error ? err.message : err)
    logAiError("suggest-applications", err)
    const { body, status } = friendlyJsonError(err, "Could not generate suggestions.")
    return Response.json(body, { status })
  }
}
