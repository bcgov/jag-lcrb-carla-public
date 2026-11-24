import { generateText, Output } from "ai"
import { z } from "zod"
import { logAiError, AI_MAX_RETRIES, friendlyJsonError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

// Web search plus structured reasoning across several subjects takes longer than a single generation, so this route gets more headroom than the 30s default.
export const maxDuration = 60

/** Advisory adverse-media scan: a web-search model finds recent news per subject, then each is classified from that material only. Matches need human verification. */

type Program = "cannabis" | "liquor"

interface ScanSubject {
  name: string
  type: "person" | "business"
  context?: string
}

interface ScanRequest {
  program: Program
  applicantName: string
  businessName?: string
  people?: { name: string; roles?: string[]; entity?: string }[]
}

/** Max subjects scanned per request — bounds latency and cost. */
const MAX_SUBJECTS = 6

const ArticleSchema = z.object({
  title: z.string().describe("Headline or title of the media item."),
  publisher: z.string().nullable().describe("Publication / outlet name, or null if unclear."),
  date: z.string().nullable().describe("Publication date as reported (e.g. '2023-08' or 'Aug 2023'), or null."),
  url: z.string().describe("Source URL. MUST be one of the URLs present in the retrieved material — never invented."),
  relevance: z.string().describe("One sentence: what the item alleges/reports and why it bears on suitability."),
})

const SubjectFindingSchema = z.object({
  name: z.string(),
  type: z.enum(["person", "business"]),
  outcome: z
    .enum(["clear", "attention", "flag"])
    .describe("clear = nothing adverse; attention = minor/unconfirmed; flag = serious credible adverse media."),
  matchConfidence: z
    .enum(["confirmed", "possible", "unclear"])
    .describe("How confident the retrieved material is about being the SAME entity (guard against name collisions)."),
  summary: z.string().describe("One-line plain-language summary of what the scan found for this subject."),
  articles: z.array(ArticleSchema).describe("The specific adverse items found. Empty array if none."),
})

const AdverseMediaSchema = z.object({
  overall: z.object({
    outcome: z.enum(["clear", "attention", "flag"]).describe("Worst outcome across all subjects."),
    escalates: z
      .boolean()
      .describe("true only if serious, credible, plausibly-matched adverse media warrants escalation."),
    summary: z.string().describe("2-3 sentence overall summary for the analyst."),
  }),
  subjects: z.array(SubjectFindingSchema),
})

/** Build the retrieval query context for a subject. */
function subjectQuery(subject: ScanSubject, program: Program): string {
  const licence = program === "cannabis" ? "cannabis retail" : "liquor"
  const who =
    subject.type === "business"
      ? `the business "${subject.name}", which is applying for a ${licence} licence in British Columbia, Canada`
      : `${subject.name}${subject.context ? `, ${subject.context}` : ""} (British Columbia, Canada)`
  return `Search the web for adverse or negative media about ${who}.
Focus on: criminal charges or convictions, fraud, money laundering, organized-crime links, regulatory sanctions or licence refusals, tax/financial misconduct, bankruptcy, significant lawsuits, and serious public controversy.
For each item you find, give the outlet name, the publication date, a direct source URL, and one line on what it reports.
Be careful about name ambiguity: if you cannot reasonably confirm the item is about this same ${subject.type}, say so explicitly.
If you find nothing adverse, reply exactly: "No adverse media identified."`
}

/** Retrieve real media for one subject via the web-search model. */
async function retrieveForSubject(
  subject: ScanSubject,
  program: Program,
): Promise<{ subject: ScanSubject; text: string; sources: string[] }> {
  try {
    const result = await generateText({
      // Web-search-grounded model. Does NOT accept reasoningEffort/temperature,
      // so no shared provider options here.
      model: "openai/gpt-4o-mini-search-preview",
      maxRetries: AI_MAX_RETRIES,
      prompt: subjectQuery(subject, program),
    })
    const sources = Array.isArray(result.sources)
      ? (result.sources
          .map((s) => (s && typeof s === "object" && "url" in s ? (s as { url?: string }).url : undefined))
          .filter(Boolean) as string[])
      : []
    return { subject, text: result.text ?? "", sources }
  } catch (err) {
    console.error("[v0] adverse-media retrieval error for", subject.name, err instanceof Error ? err.message : err)
    return { subject, text: "(retrieval failed for this subject)", sources: [] }
  }
}

export async function POST(req: Request) {
  let body: ScanRequest
  try {
    body = (await req.json()) as ScanRequest
  } catch {
    return Response.json({ error: "Invalid request body." }, { status: 400 })
  }

  const { program, applicantName, businessName, people } = body
  if (program !== "cannabis" && program !== "liquor") {
    return Response.json({ error: "Missing or invalid program." }, { status: 400 })
  }

  // Assemble subjects: the business first, then associated individuals (deduped).
  const business = (businessName || applicantName || "").trim()
  const subjects: ScanSubject[] = []
  if (business) subjects.push({ name: business, type: "business" })
  const seen = new Set<string>()
  for (const p of people ?? []) {
    const name = (p?.name || "").trim()
    if (!name || seen.has(name.toLowerCase())) continue
    seen.add(name.toLowerCase())
    const ctx = [p.roles?.length ? p.roles.join(", ") : null, p.entity ? `of ${p.entity}` : null]
      .filter(Boolean)
      .join(" ")
    subjects.push({ name, type: "person", context: ctx || undefined })
  }

  if (subjects.length === 0) {
    return Response.json({ error: "No subjects to scan." }, { status: 400 })
  }

  const scanList = subjects.slice(0, MAX_SUBJECTS)

  const escalationTarget =
    program === "cannabis" ? "the Security Programs Division (SPD)" : "an enhanced suitability review"

  try {
    // Step 1 — retrieve real media in parallel (bounded by MAX_SUBJECTS).
    const retrieved = await Promise.all(scanList.map((s) => retrieveForSubject(s, program)))

    const dossier = retrieved
      .map((r, i) => {
        const sourceList = r.sources.length ? `\nSOURCE URLS:\n${r.sources.map((u) => `- ${u}`).join("\n")}` : ""
        return `SUBJECT ${i + 1}: ${r.subject.name} (${r.subject.type})${
          r.subject.context ? `\nCONTEXT: ${r.subject.context}` : ""
        }\nRETRIEVED MEDIA:\n${r.text}${sourceList}`
      })
      .join("\n\n---\n\n")

    // Step 2 — reason over ONLY the retrieved material into a structured result.
    const systemPrompt = `You are an LCRB fit-and-proper analyst assistant assessing ADVERSE MEDIA for a ${program} licence application in British Columbia, Canada. You are given web-search results already retrieved for each subject (the business and its associated individuals). Your job is to classify what was found — NOT to search yourself and NOT to make a licensing decision.

Rules:
- Use ONLY the retrieved material below. Never invent articles, outlets, dates, or URLs. Every "url" you output MUST appear in the retrieved material for that subject.
- Only treat an item as adverse if it plausibly reports criminal, financial-integrity, regulatory, organized-crime, or serious-reputational concerns.
- Guard against name collisions. If the material cannot confirm it is the same ${program === "cannabis" ? "person/business" : "person/business"}, set matchConfidence to "possible" or "unclear" and lower the outcome accordingly (usually "attention", not "flag").
- outcome per subject: "clear" = nothing adverse found; "attention" = minor, dated, or unconfirmed items worth a look; "flag" = serious, credible, plausibly-matched adverse media.
- "escalates" is true ONLY when a "flag" with confirmed/possible match would justify ${escalationTarget}.
- Be concise and factual. This is advisory; a human analyst verifies identity and decides.`

    const { experimental_output } = await generateText({
      model: AI_MODEL,
      providerOptions: AI_PROVIDER_OPTIONS,
      maxRetries: AI_MAX_RETRIES,
      system: systemPrompt,
      experimental_output: Output.object({ schema: AdverseMediaSchema }),
      prompt: `Assess the adverse-media scan results below.\n\n${dossier}`,
    })

    return Response.json({
      program,
      escalationTarget,
      scannedSubjects: scanList.map((s) => ({ name: s.name, type: s.type })),
      ...experimental_output,
    })
  } catch (err) {
    console.error("[v0] adverse-media error:", err instanceof Error ? err.message : err)
    logAiError("adverse-media", err)
    const { body: errBody, status } = friendlyJsonError(err, "Could not complete the adverse-media scan.")
    return Response.json(errBody, { status })
  }
}
