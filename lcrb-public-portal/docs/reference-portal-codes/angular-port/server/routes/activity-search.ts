import { generateText, Output } from "ai"
import { z } from "zod"
import { logAiError, AI_MAX_RETRIES, friendlyJsonError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

export const maxDuration = 30

/** Natural-language search over the client-side audit trail: the model picks matching entry ids from a capped list and writes a short answer; ids are returned so the UI renders the real entries. */

const EntrySchema = z.object({
  id: z.string(),
  at: z.string(),
  category: z.string(),
  action: z.string(),
  summary: z.string(),
  actor: z.string(),
  subjectId: z.string().nullable(),
})

export async function POST(req: Request) {
  try {
    const { query, entries, today } = (await req.json()) as {
      query: string
      entries: z.infer<typeof EntrySchema>[]
      today?: string
    }

    if (!query?.trim()) {
      return Response.json({ error: "Empty query" }, { status: 400 })
    }
    if (!Array.isArray(entries) || entries.length === 0) {
      return Response.json({ answer: "There is no activity recorded yet to search.", matchIds: [] })
    }

    // Cap the payload to keep the prompt bounded.
    const capped = entries.slice(0, 200)

    const result = await generateText({
      model: AI_MODEL,
      providerOptions: AI_PROVIDER_OPTIONS,
      maxRetries: AI_MAX_RETRIES,
      experimental_output: Output.object({
        schema: z.object({
          answer: z
            .string()
            .describe("A concise, plain-language answer to the question, summarizing what was found (1-3 sentences)."),
          matchIds: z.array(z.string()).describe("The ids of the entries that match the query, most relevant first."),
          interpretation: z
            .string()
            .describe("One short line restating how you interpreted the query (filters applied)."),
        }),
      }),
      system: [
        "You are a search assistant over a government licensing portal's audit/activity trail.",
        "You are given a JSON array of audit entries and a user question.",
        "Select ONLY the entries that genuinely match the question's intent (action type, actor, subject, category, and any time window).",
        `Interpret relative dates against today's date: ${today ?? new Date().toISOString().slice(0, 10)}.`,
        "Match ids EXACTLY as given. Never invent ids or facts. If nothing matches, return an empty matchIds array and say so plainly.",
        "Keep the answer factual and grounded only in the provided entries.",
      ].join("\n"),
      prompt: [
        `Question: ${query}`,
        "",
        "Audit entries (JSON):",
        JSON.stringify(capped),
      ].join("\n"),
    })

    const out = result.experimental_output
    // Defensive: only return ids that actually exist in the input.
    const validIds = new Set(capped.map((e) => e.id))
    const matchIds = out.matchIds.filter((id) => validIds.has(id))

    return Response.json({
      answer: out.answer,
      interpretation: out.interpretation,
      matchIds,
    })
  } catch (err) {
    console.error("[v0] activity-search error:", err instanceof Error ? err.message : err)
    logAiError("activity-search", err)
    const { body, status } = friendlyJsonError(err, "Search failed.")
    return Response.json(body, { status })
  }
}
