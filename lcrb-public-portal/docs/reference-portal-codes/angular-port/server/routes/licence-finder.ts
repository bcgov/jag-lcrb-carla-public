import {
  streamText,
  convertToModelMessages,
  tool,
  stepCountIs,
  type UIMessage,
} from "ai"
import { z } from "zod"
import {
  retrieveKnowledge,
  relevantLinks,
  type RetrievedChunk,
} from "@server-lib/ai/knowledge-base"
import { buildApplicationCatalogPrompt } from "@data-access/repositories/ai/application-actions"
import { logAiError, AI_MAX_RETRIES, friendlyStreamError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

export const maxDuration = 60

/** Eligibility triage agent: a short interview, then recommended applications as CTA cards. Both tools resolve server-side so the question text is still written. */

interface FinderContext {
  role: string
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

function buildSystemPrompt(retrieved: RetrievedChunk[], links: { label: string; url: string }[]): string {
  const excerpts = retrieved.length
    ? retrieved
        .map((c, i) => `[${i + 1}] Source: ${c.doc}${c.heading ? ` — ${c.heading}` : ""}\n${c.text}`)
        .join("\n\n")
    : "No specific policy excerpts matched yet."

  const linkList = links.length ? links.map((l) => `- ${l.label}: ${l.url}`).join("\n") : "(none)"

  return `You are the BC Liquor & Cannabis Licensing Portal's "Licence Finder" — a friendly eligibility pre-check that helps a member of the public figure out WHICH licence or permit they need before they start an application. You are a focused triage agent in the portal's family of AI assistants.

Your goal: through a SHORT, plain-language conversation, understand what the person wants to do, then recommend the right application(s) and let them launch it with one tap.

## How to run the interview
- Ask ONE simple question at a time. Keep the whole interview to about 2-4 questions before recommending — do not interrogate.
- The key things you usually need to know:
  1. What is the business or activity? (restaurant, bar/pub, liquor/cannabis store, manufacturer/brewery, one-time event, etc.)
  2. Will they serve/sell liquor, cannabis, or both? Is serving liquor the PRIMARY purpose (a bar) or secondary to food (a restaurant)?
  3. Is this a permanent business or a one-time/temporary event?
  4. Anything notable (patio, manufacturing on site, existing licence) — only if relevant.
- If the user's first message already makes the answer obvious (e.g. "I want to open a brewery"), skip ahead and recommend right away. Don't ask questions you already know the answer to.
- After EVERY question you ask, call suggestReplies with 2-4 short tappable answer options so the user can respond with one tap.

## Making the recommendation
- Once you have enough to decide, call the recommendApplications tool with 1-3 options, best match first. For each: the exact catalog slug, a friendly title, a ONE-sentence plain-language reason it fits, and a confidence ("high" | "medium" | "low").
- If the user told you a business or legal name, pass it as establishmentName / legalName so the application is pre-filled.
- Prefer recommending the single best match with high confidence. Offer 2-3 only when the situation is genuinely ambiguous (e.g. could be Food Primary OR Liquor Primary).
- ALWAYS pair the recommendation with a one or two sentence explanation in your text reply of why, citing policy when the excerpts support it, e.g. "(Source: Food Primary Licence Handbook)".
- After recommending, call suggestReplies with follow-ups like "What documents will I need?", "What does it cost?", "Start over".

## Rules
- This Licence Finder is for BUSINESS liquor and cannabis licences only. NEVER recommend a Special Event Permit (SEP). If someone describes a one-time private event (wedding, party, fundraiser, festival) rather than an ongoing business, briefly explain that Special Event Permits are handled separately and point them to the "Special Event Permits" area of the portal — do not call recommendApplications for it.
- Base any policy/eligibility statements on the retrieved excerpts below; if they don't cover it, say you're not certain and point to the official link rather than inventing fees, timelines, or rules.
- Keep every message short (2-4 sentences) and in friendly plain language for the general public.
- Never collect more personal information than you need to make a recommendation.

## Application catalog (use these exact slugs)
${buildApplicationCatalogPrompt({ includeSpecialEvent: false })}

## Retrieved policy excerpts
${excerpts}

## Official links you may share
${linkList}`
}

export async function POST(req: Request) {
  const { messages, context }: { messages: UIMessage[]; context?: FinderContext } = await req.json()
  void context

  const query = latestUserQuery(messages)
  const retrieved = query ? retrieveKnowledge(query, { limit: 6 }) : []
  const links = query ? relevantLinks(query) : []

  const result = streamText({
    model: AI_MODEL,
    providerOptions: AI_PROVIDER_OPTIONS,
    maxRetries: AI_MAX_RETRIES,
    system: buildSystemPrompt(retrieved, links),
    messages: await convertToModelMessages(messages),
    stopWhen: stepCountIs(5),
    prepareStep: ({ steps }) => {
      // Both tools resolve server-side. Only ban the same tool; once quick replies exist ban all, so the turn ends in text, else buttons come out as raw JSON.
      const calls = (steps as { toolCalls?: { toolName?: string }[] }[]).flatMap((s) =>
        (s.toolCalls ?? []).map((c) => c.toolName),
      )
      if (calls.includes("suggestReplies")) return { toolChoice: "none" as const }
      if (calls.includes("recommendApplications")) {
        return { activeTools: ["suggestReplies" as const] }
      }
      return {}
    },
    tools: {
      recommendApplications: tool({
        description:
          "Recommend the licence/permit application(s) that fit the user's needs. Renders as prominent call-to-action cards the user can tap to start the application. Call this once you have enough information to decide.",
        inputSchema: z.object({
          recommendations: z
            .array(
              z.object({
                slug: z
                  .string()
                  .describe("The exact application catalog slug, e.g. 'liquor-primary-licence' or 'food-primary'"),
                title: z.string().describe("Friendly application title, e.g. 'Liquor Primary Licence'"),
                reason: z
                  .string()
                  .describe("One short plain-language sentence on why this fits the user's situation"),
                confidence: z.enum(["high", "medium", "low"]).describe("How confident this is the right match"),
                establishmentName: z
                  .string()
                  .nullable()
                  .describe("Business/establishment name the user mentioned, to pre-fill the application, or null"),
                legalName: z
                  .string()
                  .nullable()
                  .describe("Legal entity name the user mentioned, to pre-fill the application, or null"),
              }),
            )
            .min(1)
            .max(3)
            .describe("1-3 recommended applications, best match first"),
        }),
        // Server-resolved so the agent loop keeps going; the CTA cards render from the tool input, and a client-only tool ended the turn with bare cards and no explanation.
        execute: async ({ recommendations }) => ({ recommendations }),
      }),
      suggestReplies: tool({
        description:
          "Offer the user 2-4 short, tappable quick-reply buttons — either answers to the question you just asked, or sensible follow-ups. Call this at the END of every turn.",
        inputSchema: z.object({
          replies: z
            .array(z.string())
            .min(1)
            .max(4)
            .describe("Short quick-reply button labels (about 2-6 words each)"),
        }),
        // Server-resolved: a client-resolved call ends the turn before the question is written, leaving bare buttons. The UI renders the buttons from the tool input.
        execute: async ({ replies }) => ({ replies }),
      }),
    },
  })

  return result.toUIMessageStreamResponse({
    onError: (e) => {
      logAiError("licence-finder", e)
      return friendlyStreamError(e)
    },
  })
}
