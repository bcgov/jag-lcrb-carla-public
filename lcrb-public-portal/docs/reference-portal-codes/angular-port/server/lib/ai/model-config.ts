/** Shared model config. Reasoning is "low" because "medium" was slow (~14s) and rate-limited the gateway. A free-tier quota error means upgrade the account. */

/** The primary model string used across the portal (AI Gateway provider/model form). */
export const AI_MODEL = "openai/gpt-5-mini"

/** Fallback chain, since the gateway rate-limits per model. List only reachable models; `max_tokens` must be >= 16 or the gateway returns 400. */
export const AI_FALLBACK_MODELS = [
  "google/gemini-2.5-flash",
  "anthropic/claude-haiku-4.5",
  "openai/gpt-5-nano",
  "google/gemini-2.5-flash-lite",
] as const

/** Provider options for all calls: `openai.reasoningEffort: "low"` (ignored by non-OpenAI fallbacks) and `gateway.models`, the fallback chain above. */
export const AI_PROVIDER_OPTIONS = {
  openai: {
    reasoningEffort: "low",
  },
  gateway: {
    // Mutable string[] (not a readonly tuple) so it satisfies the AI SDK's providerOptions JSON-compatible type.
    models: [...AI_FALLBACK_MODELS] as string[],
  },
}
