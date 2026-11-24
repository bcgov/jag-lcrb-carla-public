import { streamText, convertToModelMessages, tool, stepCountIs, type UIMessage } from "ai"
import { z } from "zod"
import { retrieveKnowledge, relevantLinks, type RetrievedChunk } from "@server-lib/ai/knowledge-base"
import { logAiError, AI_MAX_RETRIES, friendlyStreamError } from "@domain/ai/ai-errors"
import { turnHasSuggestionsButNoText } from "@domain/ai/auto-continue"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

export const maxDuration = 60

/** Establishment Planner advisor: read-only agent that explains the current plan using a plan summary and the knowledge base. It does not pick a licence. */

interface AdvisorContext {
  /** Human-readable summary of the applicant's current plan + feasibility. */
  planSummary?: string
}

function latestUserQuery(messages: UIMessage[]): string {
  for (let i = messages.length - 1; i >= 0; i--) {
    const m = messages[i]
    if (m.role !== "user") continue
    const text = (m.parts ?? [])
      .filter((p) => p.type === "text")
      .map((p) => (p as { text: string }).text)
      .join(" ")
      .trim()
    if (text) return text
  }
  return ""
}

function buildSystemPrompt(
  planSummary: string,
  retrieved: RetrievedChunk[],
  links: { label: string; url: string }[],
): string {
  const excerpts = retrieved.length
    ? retrieved.map((c, i) => `[${i + 1}] Source: ${c.doc}${c.heading ? ` — ${c.heading}` : ""}\n${c.text}`).join("\n\n")
    : "No specific policy excerpts matched yet."

  const linkList = links.length ? links.map((l) => `- ${l.label}: ${l.url}`).join("\n") : "(none)"

  return `You are the BC Liquor & Cannabis Licensing Portal's "Establishment Planner" advisor — a friendly, plain-language guide for someone planning to open a new licensed establishment (liquor or cannabis). You are part of the portal's family of AI assistants.

The applicant is looking at a feasibility plan the portal has already generated for them (below). Your job is to HELP THEM UNDERSTAND AND ACT ON IT:
- Explain what a flag, fee, timeline, or checklist item means in simple terms.
- Explain WHY something is required and how to get it (e.g. "how do I get local government approval?").
- Answer "what if" questions (e.g. "what if I also want a patio?", "what if I already hold 7 liquor licences?").
- Suggest a sensible next step.

## Rules
- The plan below is authoritative for THIS applicant's numbers (licence cap, fees, timeline, flags). Refer to it directly; do not contradict it or invent different figures.
- For policy/eligibility beyond the plan, rely on the retrieved excerpts; if they don't cover it, say you're not certain and point to the official link rather than inventing fees, timelines, or rules.
- You do NOT change the plan or pick a different licence — if they want to reconsider the establishment type, tell them to adjust their answers in the planner.
- Keep every message short (2-4 sentences) and in warm, plain language.

## ANSWER THE QUESTION (CRITICAL)
Testing found this advisor replying to questions with more questions. Never do that.
- ALWAYS write a real, substantive answer in plain text FIRST. Explain the thing the user asked about using the plan and the excerpts below.
- Do NOT reply with a clarifying question unless the request is genuinely impossible to answer — and even then, give your best partial answer first, then ask ONE specific question.
- Do NOT reply with only quick replies. suggestReplies is a follow-up to an answer, never a replacement for one: a turn with buttons but no text renders as an empty bubble.
- If the user taps one of your own suggested prompts, that is a direct question — answer it with information, not another prompt.
- AFTER your written answer, call suggestReplies with 2-4 short, tappable follow-ups relevant to their plan.

## The applicant's current plan
${planSummary || "No plan details were provided yet. Answer the user's question from the policy excerpts and general BC licensing knowledge, and mention that completing the planner will give them figures specific to their establishment."}

## Retrieved policy excerpts
${excerpts}

## Official links you may share
${linkList}`
}

export async function POST(req: Request) {
  const { messages, context }: { messages: UIMessage[]; context?: AdvisorContext } = await req.json()

  const query = latestUserQuery(messages)
  const planSummary = context?.planSummary ?? ""
  // Ground retrieval in both the question and the plan so early questions still surface relevant policy.
  const retrievalQuery = [query, planSummary].filter(Boolean).join(" \n ")
  const retrieved = retrievalQuery ? retrieveKnowledge(retrievalQuery, { limit: 6 }) : []
  const links = query ? relevantLinks(query) : []

  const result = streamText({
    model: AI_MODEL,
    providerOptions: AI_PROVIDER_OPTIONS,
    maxRetries: AI_MAX_RETRIES,
    system: buildSystemPrompt(planSummary, retrieved, links),
    messages: await convertToModelMessages(messages),
    stopWhen: stepCountIs(5),
    prepareStep: ({ steps }) => {
      // Owed an answer from a previous turn → text only.
      if (turnHasSuggestionsButNoText(messages)) return { toolChoice: "none" as const }
      // Suggestions already emitted this request: force the next step to plain text so the user gets a real answer.
      const suggested = (steps as { toolCalls?: { toolName?: string }[] }[]).some((s) =>
        (s.toolCalls ?? []).some((c) => c.toolName === "suggestReplies"),
      )
      return suggested ? { toolChoice: "none" as const } : {}
    },
    tools: {
      suggestReplies: tool({
        description:
          "Offer the user 2-4 short, tappable quick-reply buttons with sensible follow-up questions about their plan. Call this only AFTER you have written a substantive text answer — never as a substitute for answering.",
        inputSchema: z.object({
          replies: z.array(z.string()).min(1).max(4).describe("Short quick-reply button labels (about 2-6 words each)"),
        }),
        // Resolved on the server so a suggestion call cannot end the turn before the answer is written; the UI renders from the tool input.
        execute: async ({ replies }) => ({ replies }),
      }),
    },
  })

  return result.toUIMessageStreamResponse({
    onError: (e) => {
      logAiError("planner-advisor", e)
      return friendlyStreamError(e)
    },
  })
}
