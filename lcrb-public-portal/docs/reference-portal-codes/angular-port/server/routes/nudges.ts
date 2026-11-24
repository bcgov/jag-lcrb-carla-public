import { generateText, Output } from "ai"
import { z } from "zod"
import { AI_MAX_RETRIES } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

export const maxDuration = 30

/** POST /api/nudges rewords and re-prioritizes the client's reminders; the client swaps it in only on success and caches. TODO: no auth; scope to the licensee. */
const nudgeSchema = z.object({
  nudges: z
    .array(
      z.object({
        id: z.string().describe("short stable slug, e.g. 'renew-326739'"),
        priority: z.enum(["high", "medium", "low"]).describe("high = action needed now / overdue"),
        title: z.string().describe("short, plain-language headline, max ~8 words"),
        body: z.string().describe("one or two friendly sentences explaining why this matters and what to do"),
        ctaLabel: z.string().describe("button text, e.g. 'Renew now'"),
        ctaHref: z.string().describe("relative portal path for the CTA, must be one of the provided hrefs"),
      }),
    )
    .max(5)
    .describe("0-5 reminders, most urgent first. Only include genuinely useful items."),
})

interface NudgeContextItem {
  kind: string
  label: string
  detail: string
  href: string
}

export async function POST(req: Request) {
  try {
    const { items, today } = (await req.json()) as { items: NudgeContextItem[]; today: string }

    if (!Array.isArray(items) || items.length === 0) {
      return Response.json({ nudges: [] })
    }

    const context = items
      .map((i) => `- [${i.kind}] ${i.label} — ${i.detail} (link: ${i.href})`)
      .join("\n")

    const { experimental_output } = await generateText({
      model: AI_MODEL,
      providerOptions: AI_PROVIDER_OPTIONS,
      maxRetries: AI_MAX_RETRIES,
      experimental_output: Output.object({ schema: nudgeSchema }),
      system:
        "You are a helpful assistant for a British Columbia liquor & cannabis licensing portal (LCRB). " +
        "You turn a licensee's account data into a short list of proactive, plain-language reminders. " +
        "Be warm, concise, and specific. Prioritize anything expired or due soon. " +
        "Only surface reminders that are genuinely actionable. If nothing needs attention, return an empty list. " +
        "Every ctaHref MUST be copied exactly from one of the provided links — never invent a URL. " +
        "Use Canadian spelling (licence).",
      prompt:
        `Today is ${today}. Here is the licensee's current account data:\n\n${context}\n\n` +
        "Generate the most useful reminders, most urgent first.",
    })

    return Response.json(experimental_output)
  } catch (err) {
    console.log("[v0] nudges error:", err instanceof Error ? err.message : String(err))
    return Response.json({ nudges: [] }, { status: 200 })
  }
}
