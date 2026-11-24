import { generateText, Output } from "ai"
import { z } from "zod"
import { logAiError, AI_MAX_RETRIES, friendlyJsonError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"

export const maxDuration = 30

/** Guided Mode helper: maps a plain-language answer to one typed field value. Output is bounds-checked here, so a bad option or date is rejected as low confidence. */

const AnswerSchema = z.object({
  value: z
    .string()
    .describe(
      "The single value for the field, formatted exactly as specified for its type. Empty string if you cannot tell.",
    ),
  values: z
    .array(z.string())
    .describe("For multi-select fields only: the chosen option values. Empty array for every other field type."),
  confidence: z
    .number()
    .min(0)
    .max(1)
    .describe("0-1. Use below 0.5 when the text is ambiguous, off-topic, or does not answer the question."),
  note: z
    .string()
    .describe("Optional very short note to the user about an assumption you made. Empty string when not needed."),
})

type FieldType =
  | "text"
  | "email"
  | "tel"
  | "number"
  | "date"
  | "time"
  | "textarea"
  | "select"
  | "radio"
  | "checkbox"
  | "checkbox-group"
  | "file"

interface InterpretRequest {
  field: {
    label: string
    type: FieldType
    hint?: string
    help?: string
    options?: { value: string; label: string }[]
  }
  text: string
}

const DATE_RE = /^\d{4}-\d{2}-\d{2}$/
const TIME_RE = /^([01]\d|2[0-3]):[0-5]\d$/

/** Resolve a model-supplied option to a REAL option value, by value or label. */
function matchOption(raw: string, options: { value: string; label: string }[]): string | null {
  const needle = raw.trim().toLowerCase()
  if (!needle) return null
  const hit =
    options.find((o) => o.value.toLowerCase() === needle) ??
    options.find((o) => o.label.toLowerCase() === needle) ??
    // Last resort: a label that clearly contains the answer (e.g. "yes" → "Yes, all of them").
    options.find((o) => o.label.toLowerCase().startsWith(needle))
  return hit ? hit.value : null
}

/** Force the model's answer into something the field's control can hold; null when unusable, so the caller leaves the control untouched. */
function coerce(
  out: z.infer<typeof AnswerSchema>,
  field: InterpretRequest["field"],
): string | string[] | boolean | null {
  const raw = out.value.trim()
  const options = field.options ?? []

  switch (field.type) {
    case "select":
    case "radio":
      return options.length ? matchOption(raw, options) : raw || null

    case "checkbox-group": {
      const picked = (out.values.length ? out.values : raw ? [raw] : [])
        .map((v) => matchOption(v, options))
        .filter((v): v is string => Boolean(v))
      // De-duplicate: the model sometimes repeats a value it also put in `value`.
      const unique = Array.from(new Set(picked))
      return unique.length ? unique : null
    }

    case "checkbox": {
      const t = raw.toLowerCase()
      if (["true", "yes", "y", "agree", "confirmed", "checked", "1"].includes(t)) return true
      if (["false", "no", "n", "decline", "unchecked", "0"].includes(t)) return false
      return null
    }

    case "date": {
      if (DATE_RE.test(raw) && !Number.isNaN(new Date(raw).getTime())) return raw
      return null
    }

    case "time":
      return TIME_RE.test(raw) ? raw : null

    case "number": {
      const n = Number.parseFloat(raw.replace(/[^0-9.\-]/g, ""))
      return Number.isFinite(n) ? String(n) : null
    }

    default:
      return raw || null
  }
}

/** Tell the model exactly what shape the value must take for this field type. */
function formatRule(field: InterpretRequest["field"]): string {
  const opts = (field.options ?? []).map((o) => `"${o.value}" (${o.label})`).join(", ")
  switch (field.type) {
    case "select":
    case "radio":
      return `Set "value" to EXACTLY one of these option values: ${opts}. Never invent a value outside that list.`
    case "checkbox-group":
      return `Set "values" to EVERY applicable option value from: ${opts}. Leave "value" empty. Never output a value outside that list, but DO resolve everyday collective terms onto the options they plainly cover (e.g. "weekends" covers the Saturday and Sunday options; "every day" covers all of them).`
    case "checkbox":
      return `Set "value" to "true" if the person is agreeing/confirming, or "false" if they are not.`
    case "date":
      return `Set "value" to a calendar date as YYYY-MM-DD. Resolve relative dates ("next Monday") against today's date.`
    case "time":
      return `Set "value" to a 24-hour time as HH:MM (e.g. "20:30" for half eight in the evening).`
    case "number":
      return `Set "value" to digits only, no units or commas.`
    case "email":
      return `Set "value" to just the email address.`
    case "tel":
      return `Set "value" to just the phone number.`
    default:
      return `Set "value" to the cleaned-up answer text, correcting obvious typos and capitalisation but never adding facts the person did not state.`
  }
}

export async function POST(req: Request) {
  let body: InterpretRequest
  try {
    body = (await req.json()) as InterpretRequest
  } catch {
    return Response.json({ error: "Invalid request body." }, { status: 400 })
  }

  const { field, text } = body
  if (!field?.label || !field?.type || typeof text !== "string" || !text.trim()) {
    return Response.json({ error: "Missing field or answer text." }, { status: 400 })
  }
  // Documents can only be uploaded, never described.
  if (field.type === "file") {
    return Response.json({ value: null, confidence: 0, note: "Please upload the document." })
  }

  const today = new Date().toISOString().slice(0, 10)

  const systemPrompt = `You convert a person's plain-language answer into a single value for one field on a British Columbia liquor & cannabis licensing application form.

Today's date is ${today}.

Rules:
- ${formatRule(field)}
- Base the answer ONLY on what the person actually said. Never fill in plausible-sounding details they did not state, and never answer the question yourself.
- Interpreting their WORDING is your job, and is not guessing: normalise phrasing, units and colloquialisms onto the required format ("half eight in the evening" → 20:30, "a dozen" → 12, "weekends" → the Saturday and Sunday options). Only inventing unstated FACTS is forbidden.
- If the text is ambiguous, off-topic, refuses, or asks a question back, set "confidence" below 0.5 and leave "value" empty.
- Use "note" ONLY when you made an assumption worth flagging (e.g. "Read 'next Friday' as ${today}"). Keep it under 12 words. Otherwise return an empty string.`

  try {
    const { experimental_output } = await generateText({
      model: AI_MODEL,
      providerOptions: AI_PROVIDER_OPTIONS,
      maxRetries: AI_MAX_RETRIES,
      system: systemPrompt,
      experimental_output: Output.object({ schema: AnswerSchema }),
      prompt: `FIELD LABEL: ${field.label}
FIELD TYPE: ${field.type}
${field.hint ? `FIELD HINT: ${field.hint}\n` : ""}${field.help ? `WHAT THIS FIELD MEANS: ${field.help}\n` : ""}
THE PERSON WROTE:
"""
${text.slice(0, 1000)}
"""`,
    })

    const out = experimental_output
    const value = coerce(out, field)

    // A value the field cannot hold is treated as "not understood", regardless
    // of how confident the model claimed to be.
    if (value === null) {
      return Response.json({ value: null, confidence: 0, note: out.note?.trim() || "" })
    }

    return Response.json({
      value,
      confidence: out.confidence,
      note: out.note?.trim() || "",
    })
  } catch (err) {
    console.error("[v0] interpret-answer error:", err instanceof Error ? err.message : err)
    logAiError("interpret-answer", err)
    const { body: errBody, status } = friendlyJsonError(err, "Could not read that answer.")
    return Response.json(errBody, { status })
  }
}
