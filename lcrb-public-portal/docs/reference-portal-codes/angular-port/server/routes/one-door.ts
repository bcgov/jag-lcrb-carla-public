import { streamText, convertToModelMessages, tool, stepCountIs, type UIMessage } from "ai"
import { z } from "zod"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"
import { AI_MAX_RETRIES, friendlyStreamError, logAiError } from "@domain/ai/ai-errors"
import { BRANCHES, type BranchId } from "@domain/one-door/branches"

export const maxDuration = 60

/** "One door into government": a shallow simulated BC Government assistant that hands LCRB requests to the portal. Other branches only show not-in-prototype cards. */
const SYSTEM_PROMPT = `You are the Government of British Columbia's front-door assistant — the single place a citizen starts when they need something from the Province. You are warm, plain-spoken and brief (2-4 short sentences), and you write in Canadian English.

YOUR JOB
1. Understand what the person needs.
2. Answer simple wayfinding questions yourself when you can (office hours, "where do I start", what a service is).
3. When the request belongs to a specific branch of government, call the handoffToBranch tool to transfer them. Do NOT try to complete branch business yourself.

BRANCH ROUTING
${BRANCHES.map((b) => `- ${b.id}: ${b.name}. Route here for: ${b.intents.join(", ")}.`).join("\n")}

RULES FOR HAND-OFF
- Liquor or cannabis licensing of ANY kind (new licence, renewal, transfer, patio, hours, endorsement, special event permit, manufacturer/winery/brewery/distillery, cannabis retail store, serving it right / selling it right, catering liquor) → handoffToBranch with branch "lcrb". This is the one destination that is fully connected, so be confident about it.
- Before handing off, ask AT MOST one clarifying question if the request is ambiguous between branches. If it clearly matches a branch, hand off immediately — don't make them repeat themselves.
- In the handoff "summary" field, write one sentence in the second person restating what they need, so the receiving branch (and the citizen) can see the context travelled with them.
- After calling handoffToBranch, write ONE or TWO short friendly sentences confirming the transfer and that their request is going with them, then stop. Do not describe the destination's features, do not answer branch questions (documents, fees, timelines — the branch owns those), and do not ask "what next?". Then call suggestReplies with options like "I have a different question" or "Start over".
- If the request is not government-related, say so kindly and offer the things you can help with.
- Never invent phone numbers, fees, addresses or wait times. If asked, say the branch will have the current details.

QUICK REPLIES
- Call suggestReplies at the END of every turn with 2-4 short options (2-6 words each) the person is likely to tap next. Don't repeat options they've already chosen.
- NEVER write the options as a list inside your message — they are rendered as buttons from the tool call. Your message text should contain no bullet lists of choices.

STYLE
- Greet by name if they give one. Use "you" and "your". No emojis. No bullet lists longer than three items.
- You are a prototype: if asked whether you are real, say you are a simulation of a future service.`

export async function POST(req: Request) {
  const { messages } = (await req.json()) as { messages: UIMessage[] }

  const result = streamText({
    model: AI_MODEL,
    providerOptions: AI_PROVIDER_OPTIONS,
    maxRetries: AI_MAX_RETRIES,
    system: SYSTEM_PROMPT,
    messages: await convertToModelMessages(messages),
    stopWhen: stepCountIs(4),
    // Once a hand-off has fired only suggestions remain; once suggestions have fired the model must finish in plain text, which prevents tool loops and the empty bubble with buttons.
    prepareStep: ({ steps }) => {
      const called = (steps as { toolCalls?: { toolName?: string }[] }[]).flatMap((s) =>
        (s.toolCalls ?? []).map((c) => c.toolName),
      )
      if (called.includes("suggestReplies")) return { toolChoice: "none" as const }
      if (called.includes("handoffToBranch")) return { activeTools: ["suggestReplies"] as const }
      return {}
    },
    tools: {
      handoffToBranch: tool({
        description:
          "Transfer the citizen to the branch of government that owns their request. Call this as soon as the branch is clear. The UI renders a transfer card from your input.",
        inputSchema: z.object({
          branch: z
            .enum(BRANCHES.map((b) => b.id) as [BranchId, ...BranchId[]])
            .describe("Which branch to hand off to"),
          summary: z
            .string()
            .describe("One second-person sentence restating the citizen's need, e.g. 'You want to add a patio to your restaurant's liquor licence.'"),
          keywords: z
            .array(z.string())
            .max(5)
            .describe("The 2-5 words or phrases in the conversation that triggered this routing"),
        }),
        // Server-executed so the agent loop continues into a text step.
        execute: async ({ branch, summary, keywords }) => {
          const meta = BRANCHES.find((b) => b.id === branch)
          return {
            branch,
            summary,
            keywords,
            connected: Boolean(meta?.href),
            destination: meta?.href ?? null,
          }
        },
      }),
      suggestReplies: tool({
        description: "Offer 2-4 short tappable quick replies for what the citizen is likely to say next.",
        inputSchema: z.object({
          replies: z.array(z.string()).min(1).max(4),
        }),
        execute: async ({ replies }) => ({ replies }),
      }),
    },
  })

  return result.toUIMessageStreamResponse({
    onError: (e) => {
      logAiError("one-door", e)
      return friendlyStreamError(e)
    },
  })
}
