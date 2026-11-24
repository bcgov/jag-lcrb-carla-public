import { lastAssistantMessageIsCompleteWithToolCalls, type UIMessage } from "ai"

/** Tools that end a turn: they render UI (quick replies, action buttons, cards) and the model never needs to react to their output. */
const TERMINAL_TOOLS = new Set(["suggestReplies", "suggestActions", "recommendApplications"])

/** Ceiling on automatic tool-result resends per user turn; backstop against a runaway loop. */
const MAX_AUTO_STEPS = 4

/** Guarded `lastAssistantMessageIsCompleteWithToolCalls`: auto-continues only after non-terminal tool calls, up to a step cap, to avoid a resend loop with suggestReplies. */
/** True when the last assistant turn had suggestion UI but no written text; callers force one more text-only step so the user gets an answer. */
export function turnHasSuggestionsButNoText(messages: UIMessage[]): boolean {
  const last = messages[messages.length - 1]
  if (!last || last.role !== "assistant") return false

  let hasText = false
  let hasSuggestionTool = false
  for (const p of last.parts) {
    const type = (p as { type?: string }).type
    if (type === "text") {
      const text = (p as { text?: string }).text
      if (typeof text === "string" && text.trim().length > 0) hasText = true
    } else if (typeof type === "string" && type.startsWith("tool-")) {
      if (TERMINAL_TOOLS.has(type.slice("tool-".length))) hasSuggestionTool = true
    }
  }
  return hasSuggestionTool && !hasText
}

export function shouldAutoResendToolResults(options: { messages: UIMessage[] }): boolean {
  if (!lastAssistantMessageIsCompleteWithToolCalls(options)) return false

  const { messages } = options
  const last = messages[messages.length - 1]
  if (!last || last.role !== "assistant") return false

  // Bail out if the assistant already auto-continued too many times since the last user message.
  let assistantTurns = 0
  for (let i = messages.length - 1; i >= 0; i--) {
    if (messages[i].role === "user") break
    if (messages[i].role === "assistant") assistantTurns++
  }
  if (assistantTurns >= MAX_AUTO_STEPS) return false

  // If the model already emitted a terminal display tool, the turn is complete.
  const calledTerminal = last.parts.some((p) => {
    const type = (p as { type?: string }).type
    if (typeof type !== "string" || !type.startsWith("tool-")) return false
    return TERMINAL_TOOLS.has(type.slice("tool-".length))
  })
  return !calledTerminal
}
