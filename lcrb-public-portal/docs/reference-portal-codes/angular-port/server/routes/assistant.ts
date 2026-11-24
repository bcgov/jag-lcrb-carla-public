import {
  streamText,
  convertToModelMessages,
  createUIMessageStream,
  createUIMessageStreamResponse,
  tool,
  stepCountIs,
  type UIMessage,
} from "ai"
import { EASTER_EGG_REPLY, isEasterEggTrigger } from "@server-lib/ai/easter-egg"
import { z } from "zod"
import {
  NAV_DESTINATIONS,
  PORTAL_FAQ,
  workflowsForRole,
  type PortalRole,
} from "@domain/ai/portal-knowledge"
import {
  retrieveKnowledge,
  relevantLinks,
  tagFromContext,
  type RetrievedChunk,
} from "@server-lib/ai/knowledge-base"
import { buildApplicationCatalogPrompt } from "@data-access/repositories/ai/application-actions"
import { matchOfficialResources } from "@server-lib/ai/official-resources"
import { AI_MAX_RETRIES, friendlyStreamError, logAiError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"
import { isPathAllowedForRole } from "@domain/ai/navigation-intent"
import { turnHasSuggestionsButNoText } from "@domain/ai/auto-continue"
import { formatPortalSnapshot, type PortalSnapshot } from "@domain/ai/portal-snapshot"

export const maxDuration = 60

/** Customer front door agent (GPT-5-mini via the AI gateway); tools are declared without `execute` so they run client-side against the live form bridge to fill fields, highlight them and navigate. */

interface AssistantContext {
  role: string
  pathname: string
  flowTitle?: string
  stepTitle?: string
  currentStep?: number
  totalSteps?: number
  fields?: {
    name: string
    label: string
    type: string
    required?: boolean
    help?: string
    options?: { value: string; label: string }[]
    value?: unknown
  }[]
  allFields?: {
    name: string
    label: string
    type: string
    required?: boolean
    options?: { value: string; label: string }[]
    value?: unknown
    step?: number
  }[]
  /** Snapshot of the signed-in user's own records (applications, licences, etc.). */
  snapshot?: PortalSnapshot | null
}

/** Whether a destination fits the role. Licensees may see applicant destinations, but an SEP organiser is not a licensee. */
function destinationMatchesRole(destRoles: PortalRole[] | undefined, role: string, href?: string): boolean {
  if (role === "applicant") {
    // Restrict applicants to their own surface regardless of how the destination is tagged.
    return href ? isPathAllowedForRole(role, href) : false
  }
  if (!destRoles || destRoles.length === 0) return true
  if (destRoles.includes(role as PortalRole)) return true
  // A licensee can also organise a special event, so applicant pages are fine.
  if (role === "licensee" && destRoles.includes("applicant")) return true
  return false
}

function buildSystemPrompt(
  ctx: AssistantContext,
  retrieved: RetrievedChunk[],
  links: { label: string; url: string }[],
): string {
  const navList = NAV_DESTINATIONS.filter((d) => destinationMatchesRole(d.roles, ctx.role, d.href))
    .map((d) => `- "${d.label}" (${d.href}): ${d.description}`)
    .join("\n")

  // The signed-in user's own records, so the assistant can answer about their applications, licences, drafts and business.
  const recordsBlock = formatPortalSnapshot(ctx.snapshot)

  const faq = PORTAL_FAQ.map((f) => `Q: ${f.q}\nA: ${f.a}`).join("\n\n")

  // Step-by-step "how do I..." workflows for this user's role.
  const workflows = workflowsForRole(ctx.role)
    .map((w) => {
      const stepLines = w.steps.map((s, i) => `   ${i + 1}. ${s}`).join("\n")
      const link = w.href ? ` (primary page: ${w.href})` : ""
      return `• ${w.task}${link}\n${stepLines}`
    })
    .join("\n\n")

  // Retrieved excerpts from the official LCRB policy documents (RAG).
  const excerpts = retrieved.length
    ? retrieved
        .map((c, i) => {
          const head = c.heading ? ` — ${c.heading}` : ""
          return `[${i + 1}] Source: ${c.doc}${head}\n${c.text}`
        })
        .join("\n\n")
    : "No specific policy excerpts matched this question."

  const linkList = links.length
    ? links.map((l) => `- ${l.label}: ${l.url}`).join("\n")
    : "(none)"

  let formContext = "The user is not currently filling out an application."
  if (ctx.flowTitle && ctx.fields?.length) {
    const fieldLines = ctx.fields
      .map((f) => {
        const opts = f.options?.length
          ? ` Options: ${f.options.map((o) => `${o.label} (value="${o.value}")`).join(", ")}.`
          : ""
        const val =
          f.value !== undefined && f.value !== "" && f.value !== false
            ? ` Current value: ${JSON.stringify(f.value)}.`
            : " Currently empty."
        const help = f.help ? ` Guidance: ${f.help}` : ""
        return `- name="${f.name}" | "${f.label}" | type=${f.type}${f.required ? " | REQUIRED" : ""}.${opts}${val}${help}`
      })
      .join("\n")
    const catalog = ctx.allFields?.length
      ? ctx.allFields
          .map((f) => {
            const filled =
              f.value !== undefined && f.value !== "" && f.value !== false ? " [filled]" : " [empty]"
            return `- name="${f.name}" ("${f.label}") is on step ${(f.step ?? 0) + 1}${filled}`
          })
          .join("\n")
      : ""
    formContext = `The user is filling out: "${ctx.flowTitle}".
Current step ${(ctx.currentStep ?? 0) + 1} of ${ctx.totalSteps}: "${ctx.stepTitle}".
Fields on the current step:
${fieldLines}

All fields across every step (use these exact names; setField and highlightField will auto-navigate to the correct step for you):
${catalog}`
  }

  return `You are the BC Liquor & Cannabis Licensing Portal assistant — a friendly, concise guide for the BC government licensing portal. You are the prototype of an Azure AI Foundry "Customer Front Door Agent".

Your job is to help the user with SIX things:
1. NAVIGATION — help them reach the right page, WITH THEIR CONSENT. Only call navigateTo when the user has actually asked to be moved ("take me to…", "open my licences", "go to the map"). If they asked a QUESTION — anything starting with can/where/what/how/is/do, or ending in a question mark — ANSWER IT IN TEXT and offer the page as a button instead. Never yank someone to a different page as a side effect of answering them: testers reported this as losing control of the browser, and it is the single worst thing you can do. When a page would genuinely help, either call suggestActions to offer a button, or say "I can take you there — want me to?" and let them confirm. IMPORTANT EXCEPTION: if the user is asking about the STATUS or DETAILS of one of their own records ("check my agent application", "what's the status of…", "how far along is…", "when does … expire"), that is job #5 — answer with the actual details in text FIRST; do not treat it as a navigation request at all.
2. RECOGNISING INTENT & STARTING APPLICATIONS — when someone describes what they want to do in plain language ("I want to open a bar", "I'm starting a cannabis shop", "I need to serve drinks at my wedding"), figure out which application fits using the application catalog below, then offer it as a tappable call-to-action button using the suggestActions tool (e.g. a button labelled "Start a Liquor Primary Application"). Prefer offering an action BUTTON over a wall of text. If they clearly confirm or there is only one obvious match, you may navigateTo the application directly. If the need is ambiguous, offer 2-3 action buttons for the most likely options, or suggest the Licence Finder.
3. ANSWERING QUESTIONS — explain policy, requirements, and how the portal works using the knowledge below.
4. STEP-BY-STEP APPLICATION GUIDANCE — when the user is filling out an application, walk them through it field by field. You can:
   - explain what a field is asking with explainField/your own words,
   - HIGHLIGHT the field you are talking about with the highlightField tool so it scrolls into view,
   - FILL IN fields on their behalf with the setField tool when they tell you the answer,
   - move between steps with goToStep.
5. ANSWERING ABOUT THE USER'S OWN RECORDS — when the user asks about THEIR specific things ("check my agent application", "what's the status of my liquor licence", "when does my cannabis licence expire", "what have I submitted", "what drafts do I have", "what's my business number"), you MUST write a real text answer that states the concrete details from the "Your records" section below — the actual status, percent complete, next section to finish, submitted/expected dates, licence number, expiry, etc. Writing the text answer is REQUIRED and comes FIRST. Only AFTER writing that answer may you optionally call navigateTo or suggestActions to open the record. NEVER respond to a "check my ___" question with only a tool call and no text — a navigation with no explanation is exactly what NOT to do. Example good answer: "Your Agent Licence Application (AGT-126082) is 60% complete and not yet submitted. You've finished Applicant details, Represented manufacturers, and Agent responsibilities; the next section to complete is Supporting documents. Want me to open it?" If they ask about something that isn't in their records, say you don't see it on their account and offer to help them start it.
6. COMPLIANCE & ENFORCEMENT — "do I have any compliance issues?", "any outstanding enforcement?", "what happened with my last inspection?", "am I in trouble?" are answerable from the "Your records" section below, which lists open enforcement actions and closed inspection/contravention history. Answer with the specifics: the file reference, the allegation and section, the proposed action, and above all the RESPONSE DEADLINE for anything still open — a missed deadline can cost the licensee their right to a hearing, so lead with open items. If the history is clean, say so plainly (e.g. "no contraventions on record; your last inspection on <date> found no issues"). NEVER say you cannot see compliance or enforcement information when the records below contain it, and never answer these questions by only pointing at the Compliance page.

The user's current role is: ${ctx.role}.
The user is currently on the page: ${ctx.pathname}.

## Consent before movement (CRITICAL)
The portal enforces this, so work with it rather than against it:
- navigateTo only actually moves the user when they explicitly asked to be moved. If they asked a question, the tool will NOT navigate — it shows them a button instead and tells you so. When that happens, answer the question in text; do not call navigateTo again.
- goToStep will REFUSE to move forward when required fields on the current step are incomplete, and will refuse entirely if the user merely asked whether they can continue. Read the tool result: if it says BLOCKED, tell the user exactly which fields need attention and offer to fill them in. Never claim you advanced the form when the tool reported it was blocked.
- "Can I go to the next page?" means: check and TELL them. It does not mean move them.
- Never describe an action you did not actually perform. If the tool result says nothing was moved, do not say "I've taken you to…".

## Stay inside the user's own area
The "Navigation destinations" list below has already been filtered to what THIS user can access. Never send someone to, or promise them, a page that is not on that list.
${
  ctx.role === "applicant"
    ? `This user is a Special Event Permit organiser. They are NOT a licensee: they have no liquor/cannabis licence, no compliance or enforcement history, and no security screening file. If they ask about licensee-only topics (licence renewals, compliance flags, endorsements, legal entity, tied house), explain plainly that those apply to licence holders and are not part of their special event permit, and point them to the LCRB contact details if they need more. Do NOT offer or open the licensee area, and do not imply they have a licence.`
    : `If a question falls outside this role's area, say so plainly and offer the LCRB contact details rather than sending them somewhere they cannot use.`
}

## Form context
${formContext}

## Your records (the signed-in user's own applications, licences, and business)
Use this to answer questions about the user's OWN things with real, specific detail (status, dates, percent complete, next step, licence numbers, expiry, business identity). This is authoritative for their account. When they refer to "my <thing>" (e.g. "my agent application", "my liquor licence"), match it here by type/name/number and answer directly, then offer to open it. Never invent records, numbers, or dates that are not listed here.
${recordsBlock || "No personal records are available for this user (they may be a staff/reviewer role, or not signed in as a licensee)."}

## How to guide an application
- When helping with a form, work ONE field at a time. Highlight the field with highlightField BEFORE explaining it, so the user sees what you mean.
- When the user gives you an answer (e.g. "the event is called Summer BBQ"), call setField to fill it in. Use the exact field "name" and, for select/radio/checkbox fields, the option "value" (not the label).
- IMPORTANT: Do NOT call goToStep before setField or highlightField. Those tools automatically move the wizard to the field's correct step using the "All fields" catalog above. Only use goToStep when the user explicitly asks to jump to a step and you are not setting/highlighting a specific field.
- For yes/no fields the values are "yes" and "no". For checkbox groups, set an array of values.
- After filling a field, briefly confirm what you set and move to the next one.
- The user can talk entirely to you without touching the form — you do the typing. Be proactive: offer to fill the next field.
- Never invent certificate numbers, addresses, dates, or other personal facts. Ask the user for them.
- Keep responses short (2-4 sentences) and plain-language. This is a government service used by the general public.

## Navigation destinations
${navList}

## Portal how-to workflows ("How do I…")
These are the EXACT steps for common tasks in THIS portal, tailored to the user's role. When the user asks "How do I…", "Where do I…", "How can I…", or otherwise asks how to accomplish a task, ANSWER USING THESE STEPS — do not invent a different process. Summarise the steps concisely (a short numbered list is good), reference the real page names, and when the workflow has a primary page, ALSO call navigateTo to take them there (or offer it via suggestActions if they haven't committed yet). If no workflow matches, fall back to the navigation destinations and policy excerpts, and say if you're not certain.

${workflows}

## Starting an application (intent → action)
When the user describes a goal that maps to an application, offer it as a tappable button with suggestActions (label + href), rather than only describing it. Use these mappings:

${buildApplicationCatalogPrompt({ role: ctx.role })}

If you are unsure which licence someone needs, offer a "Find the right licence" button (/licence-finder) alongside your best guesses.

## Quick FAQ
${faq}

## Talking to a person (human escalation)
When the user asks to speak with a person/human/agent/representative, wants to "talk to someone", asks for a phone number or email, or is frustrated and wants direct help, SHARE the LCRB's official contact details below. Present them clearly (a short bulleted list is good) and pick the most relevant email for their topic (liquor, cannabis, complaints). Reassure them these reach real LCRB staff.

Liquor and Cannabis Regulation Branch (LCRB) contact information:
- Phone: 1-866-209-2111 (toll-free in Canada) or 250-952-5787 (Victoria office)
- Liquor licensing & general inquiries: LCRBLiquor@gov.bc.ca
- Cannabis licensing: LCRBCannabis@gov.bc.ca
- Cannabis policy inquiries: Cannabisregs@gov.bc.ca
- Complaints: LCRBComplaints@gov.bc.ca

Do not invent other phone numbers or emails. If their topic doesn't match a specific email, give the phone number and the general liquor inquiries email.

You may also offer tappable action buttons with suggestActions for these — the portal supports "tel:" and "mailto:" hrefs, e.g. label "Call the LCRB" href "tel:1-866-209-2111", label "Email cannabis licensing" href "mailto:LCRBCannabis@gov.bc.ca", and a "Contact page" button href "/contact". Lead with these buttons plus a brief sentence.

## Retrieved policy excerpts (from official LCRB documents)
These excerpts were retrieved from the official BC liquor & cannabis policy manuals and licence handbooks based on the user's question. Treat them as your PRIMARY source of truth and ground your answer in them. They are ordered by relevance.

${excerpts}

## Official links you may share
${linkList}

## Answering rules
- For questions about the user's OWN records ("my application", "my licence", "my business", "what have I submitted"), answer from the "Your records" section with the concrete details, then offer to open the specific record. Do not answer these with policy excerpts or a generic "go check your applications page".
- Base policy/requirement answers on the retrieved excerpts above. When you use one, cite it inline by its document name, e.g. "(Source: Special Event Permit Policy Manual)".
- If the excerpts don't actually contain the answer, say you're not certain and point the user to the relevant official page or portal section instead of guessing. Never invent numbers, fees, deadlines, or policy.
- When a relevant official link is listed above, include it so the user can read the authoritative source.
- Keep answers concise (2-4 sentences) and in plain language for the general public. Use a short bulleted list only when it genuinely helps.

## Action buttons (IMPORTANT)
- When you recognise that the user wants to start/apply for something, call suggestActions with 1-3 call-to-action buttons that navigate straight into the right application (e.g. label "Start a Liquor Primary Application", href "/applications/new/liquor-primary-licence"). These are the primary way users launch applications — lead with a button, not a paragraph.
- Use suggestActions for navigational calls-to-action (start an application, open the Licence Finder, go to a key page). Use suggestReplies (below) for conversational follow-ups the user would TYPE/say.
- It is good to use BOTH in one turn: e.g. a "Start a Cannabis Retail Store Application" action button plus quick replies like "What documents do I need?".

## Quick replies (IMPORTANT)
- WRITE YOUR ANSWER FIRST. suggestReplies and suggestActions are follow-ups to an answer, never a substitute for one. A turn that contains only tool calls and no written text renders as an empty chat bubble with orphaned buttons — this was reported as a bug in testing. If you have nothing else to say, you still owe the user at least one sentence of substance before calling any suggestion tool.
- NEVER answer a factual question ("what is federal reporting?", "what's a tied house?") with suggestReplies alone. Explain the thing in 2-4 plain-language sentences, THEN offer follow-ups.
- After that written answer, call the suggestReplies tool with 2-4 short, tappable options for what the user is most likely to do or ask next. This lets people respond with a single tap instead of typing.
- Make them specific and contextual: relevant next pages to open, follow-up questions, or — during an application — the next field to fill or "Fill this in for me", "Explain this field", "Next step".
- Keep each label short (about 2-6 words) and phrase them as something the user would say or tap (e.g. "Open my licences", "What documents do I need?", "Start the application").
- Always provide quick replies, even after navigating or answering a question.`
}

/** True when the user asks about the status/details of their own records (not just navigation); forces a concrete text answer. */
function isPersonalRecordQuery(text: string): boolean {
  if (!text) return false
  const t = text.toLowerCase()
  const mine = /\b(my|mine|i've|i have|i submitted|i applied)\b/.test(t)
  const askish =
    /\b(check|status|how far|how's|hows|progress|where('?s| is| are)|when|expire|expiry|renew|update|what('?s| is| are)|show me|tell me|do i have|left to|remaining|outstanding|missing|complete)\b/.test(
      t,
    )
  const noun =
    /\b(application|licence|license|permit|draft|submission|renewal|business|number|agent|liquor|cannabis|food primary|catering|compliance|enforcement|inspection|contravention|penalty|suspension|violation|hearing)\b/.test(
      t,
    )
  // Compliance questions are often phrased without "my" ("any outstanding enforcement?") but are still about the user's record.
  const complianceish =
    /\b(compliance|enforcement|contravention|inspection|penalt(y|ies)|suspension|in trouble|violation)\b/.test(t)
  // "check my agent application", "status of my liquor licence", "what have I submitted"
  return (
    (mine && noun) ||
    (askish && mine) ||
    complianceish ||
    /\bwhat have i (submitted|applied)/.test(t)
  )
}

/** True if the latest assistant message already holds an executed tool result (the client ran e.g. navigateTo and is resending), so the model should now answer in text. */
function messagesContainToolResult(messages: UIMessage[]): boolean {
  for (let i = messages.length - 1; i >= 0; i--) {
    const m = messages[i]
    if (m.role === "user") return false // reached the current user turn; no tool acted yet
    if (m.role === "assistant") {
      const parts = (m.parts ?? []) as { type?: string; state?: string }[]
      if (parts.some((p) => p.type?.startsWith("tool-") && p.state === "output-available")) {
        return true
      }
    }
  }
  return false
}

/** Pull plain text out of the most recent user message for retrieval. */
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

export async function POST(req: Request) {
  const { messages, context }: { messages: UIMessage[]; context: AssistantContext } = await req.json()
  const ctx = context ?? { role: "applicant", pathname: "/" }

  // RAG: retrieve policy excerpts, boosting the licence type the user is working with.
  const query = latestUserQuery(messages)

  // Easter egg: short-circuit before any model call so the reply is verbatim and free.
  if (isEasterEggTrigger(query, ctx.role)) {
    return createUIMessageStreamResponse({
      stream: createUIMessageStream({
        execute: async ({ writer }) => {
          const id = "easter-egg"
          // `start`/`finish` are the envelope useChat needs to open and complete the message; text frames alone left the composer stuck streaming.
          writer.write({ type: "start" })
          writer.write({ type: "start-step" })
          writer.write({ type: "text-start", id })
          // Stream in word-sized chunks so it types out like a real answer.
          for (const chunk of EASTER_EGG_REPLY.split(/(?<=\s)/)) {
            writer.write({ type: "text-delta", id, delta: chunk })
            await new Promise((r) => setTimeout(r, 45))
          }
          writer.write({ type: "text-end", id })
          writer.write({ type: "finish-step" })
          writer.write({ type: "finish" })
        },
      }),
    })
  }
  const boostTag = tagFromContext(ctx.flowTitle, ctx.pathname)
  const retrieved = query ? retrieveKnowledge(query, { boostTag, limit: 6 }) : []

  // Merge the hand-picked resource links with the official gov.bc.ca page catalog, deduped by URL.
  const curated = query ? relevantLinks(query) : []
  const official = query ? matchOfficialResources(query, 4) : []
  const seenUrls = new Set<string>()
  const links: { label: string; url: string }[] = []
  for (const l of [...curated, ...official.map((o) => ({ label: o.label, url: o.url }))]) {
    if (seenUrls.has(l.url)) continue
    seenUrls.add(l.url)
    links.push(l)
  }

  // For questions about the user's own records, force a written answer: set toolChoice "none" once tool results exist or after step 0.
  const recordQuery = isPersonalRecordQuery(query) && !!ctx.snapshot
  const alreadyActedThisTurn = messagesContainToolResult(messages)

  // A previous turn offered quick replies but never wrote an answer (empty bubble); force text only.
  const owesTextAnswer = turnHasSuggestionsButNoText(messages)

  const prepareStep = ({ stepNumber, steps }: { stepNumber: number; steps: unknown[] }) => {
    // Owed an answer from a prior turn, or asking about own records after a navigation: text only.
    if (owesTextAnswer) return { toolChoice: "none" as const }
    if (recordQuery && (stepNumber >= 1 || alreadyActedThisTurn)) {
      return { toolChoice: "none" as const }
    }

    // Suggestions already emitted resolve server-side, so the loop continues; force plain text so the user gets a written answer and the model cannot keep calling suggestion tools.
    const suggested = (steps as { toolCalls?: { toolName?: string }[] }[]).some((s) =>
      (s.toolCalls ?? []).some((c) => c.toolName === "suggestReplies" || c.toolName === "suggestActions"),
    )
    if (suggested) return { toolChoice: "none" as const }

    return {}
  }

  const result = streamText({
    model: AI_MODEL,
    providerOptions: AI_PROVIDER_OPTIONS,
    // Retry transient upstream errors (rate limits, overloads) with exponential backoff before surfacing anything.
    maxRetries: AI_MAX_RETRIES,
    system: buildSystemPrompt(ctx, retrieved, links),
    messages: await convertToModelMessages(messages),
    stopWhen: stepCountIs(6),
    prepareStep,
    tools: {
      navigateTo: tool({
        description:
          "Take the user to a page — ONLY when they explicitly asked to be moved (e.g. 'take me to the map', 'open my licences'). If they asked a question, answer it and use suggestActions to offer the page as a button instead. When the user has not asked to be moved, this tool will decline to navigate and simply offer them a button, and it will tell you so in its result.",
        inputSchema: z.object({
          href: z.string().describe("The path to navigate to, e.g. /special-events/apply"),
          label: z.string().describe("Human-friendly name of the destination"),
        }),
      }),
      highlightField: tool({
        description:
          "Highlight a form field on the current application step so the user can see which one you're explaining. Scrolls it into view.",
        inputSchema: z.object({
          name: z.string().describe("The exact field name to highlight"),
        }),
      }),
      setField: tool({
        description:
          "Fill in a form field on the user's behalf. For select/radio fields use the option value; for checkbox groups pass an array of values; for yes/no use 'yes' or 'no'.",
        inputSchema: z.object({
          name: z.string().describe("The exact field name to set"),
          value: z
            .union([z.string(), z.boolean(), z.array(z.string())])
            .describe("The value to set"),
        }),
      }),
      goToStep: tool({
        description:
          "Move the application wizard to a specific step (0-based index). Forward moves are validated exactly like the user's own Next button and will be BLOCKED if required fields are incomplete — read the result and relay what needs fixing. If the user only asked WHETHER they can continue, this reports the answer without moving them.",
        inputSchema: z.object({
          step: z.number().int().min(0).describe("The 0-based step index to move to"),
        }),
      }),
      suggestActions: tool({
        description:
          "Offer the user prominent call-to-action BUTTONS that navigate somewhere when tapped — most importantly to START an application (e.g. 'Start a Liquor Primary Application'). Use this whenever you recognise an intent that maps to an application or a key portal page. Each action has a label and an href.",
        inputSchema: z.object({
          actions: z
            .array(
              z.object({
                label: z
                  .string()
                  .describe('Button text, e.g. "Start a Liquor Primary Application" or "Find the right licence"'),
                href: z.string().describe("The path the button navigates to, e.g. /applications/new/liquor-primary-licence"),
              }),
            )
            .min(1)
            .max(3)
            .describe("1-3 call-to-action buttons"),
        }),
        // Runs server-side: a client-side suggestion tool called before any text left an empty bubble with orphaned buttons.
        execute: async ({ actions }) => ({ actions }),
      }),
      suggestReplies: tool({
        description:
          "Offer the user 2-4 short, tappable quick-reply buttons for likely next actions or answers. Call this at the END of every turn so the user can respond with one tap instead of typing.",
        inputSchema: z.object({
          replies: z
            .array(z.string())
            .min(1)
            .max(4)
            .describe("Short quick-reply button labels (about 2-6 words each)"),
        }),
        // Server-executed for the same reason as suggestActions: it must not end the turn before a written answer.
        execute: async ({ replies }) => ({ replies }),
      }),
    },
  })

  return result.toUIMessageStreamResponse({
    onError: (e) => {
      /** Log the real upstream cause server-side before replacing it with friendly copy, so quota vs rate-limit failures stay distinguishable. */
      logAiError("assistant", e)
      return friendlyStreamError(e)
    },
  })
}
