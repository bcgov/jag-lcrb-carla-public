/** Turns raw AI Gateway / model errors into user-facing messages. Quota exhaustion is a hard cap and must not be retried; transient 429/529 bursts should be, with a short backoff. */

export type AiFailureKind = "quota" | "transient"

export interface AiFailure {
  kind: AiFailureKind
  /** Suggested wait before another attempt could plausibly succeed. */
  retryAfterMs: number
  /** True only when retrying stands a real chance of working. */
  retryable: boolean
  /** Raw upstream text, for server logs only, never shown to users. */
  detail: string
}

/** Unwrap the shapes errors arrive in (streamText passes `{ error }`). */
function unwrap(error: unknown): unknown {
  if (error && typeof error === "object" && "error" in error) {
    return (error as { error?: unknown }).error ?? error
  }
  return error
}

function messageOf(error: unknown): string {
  const err = unwrap(error)
  if (err instanceof Error) return err.message
  if (err && typeof err === "object") {
    const o = err as { message?: unknown; responseBody?: unknown }
    return String(o.message ?? o.responseBody ?? "")
  }
  return String(err ?? "")
}

function statusOf(error: unknown): number | undefined {
  const err = unwrap(error) as { statusCode?: number; status?: number } | null
  return err?.statusCode ?? err?.status
}

/** Honours an upstream `retry-after` (seconds or HTTP date), capped so a garbled value can't freeze the UI. */
function retryAfterFromHeaders(error: unknown): number | null {
  const err = unwrap(error) as { responseHeaders?: Record<string, string> } | null
  const raw = err?.responseHeaders?.["retry-after"]
  if (!raw) return null
  const seconds = Number(raw)
  if (Number.isFinite(seconds) && seconds >= 0) return Math.min(seconds * 1000, 120_000)
  const asDate = Date.parse(raw)
  if (!Number.isNaN(asDate)) return Math.min(Math.max(asDate - Date.now(), 0), 120_000)
  return null
}

/** Classifies a failure as "wait and retry" or "stop and explain"; quota signals are checked first because the quota message also contains "rate-limited". */
export function classifyAiError(error: unknown): AiFailure {
  const detail = messageOf(error)
  const text = detail.toLowerCase()
  const status = statusOf(error)

  const isQuota =
    text.includes("free tier") ||
    text.includes("paid credits") ||
    text.includes("upgrade") ||
    text.includes("quota") ||
    text.includes("billing") ||
    text.includes("insufficient funds") ||
    text.includes("payment required") ||
    status === 402

  if (isQuota) {
    return { kind: "quota", retryAfterMs: 0, retryable: false, detail }
  }

  return {
    kind: "transient",
    retryAfterMs: retryAfterFromHeaders(error) ?? 4000,
    retryable: true,
    detail,
  }
}

/** True for a transient throttle worth retrying; excludes quota exhaustion. */
export function isRateLimited(error: unknown): boolean {
  if (!error) return false
  const failure = classifyAiError(error)
  if (failure.kind === "quota") return false
  const text = failure.detail.toLowerCase()
  const status = statusOf(error)
  return (
    status === 429 ||
    status === 529 ||
    text.includes("rate limit") ||
    text.includes("rate-limit") ||
    text.includes("rate_limit") ||
    text.includes("429") ||
    text.includes("529") ||
    text.includes("overloaded") ||
    text.includes("too many requests")
  )
}

/** True when the AI budget is exhausted and no amount of retrying will help. */
export function isQuotaExhausted(error: unknown): boolean {
  return !!error && classifyAiError(error).kind === "quota"
}

/** Logs the real upstream cause server-side, since the user-facing copy is deliberately vague. */
export function logAiError(route: string, error: unknown): void {
  const { kind, detail } = classifyAiError(error)
  console.log(`[v0] AI failure route=${route} kind=${kind} status=${statusOf(error) ?? "n/a"} detail=${detail.slice(0, 400)}`)
}

/** AI SDK retries for transient failures. Kept at 1 (2 attempts): streamText retries before the stream opens, so more can exceed `maxDuration` and leave the client hanging. */
export const AI_MAX_RETRIES = 1

/** Browser re-requests after a transient error; quota failures are never auto-retried. */
export const CLIENT_AUTO_RETRIES = 1

/** Base delay (ms) between transparent client retries; grows per attempt. */
export const CLIENT_RETRY_DELAY_MS = 4000

const QUOTA_MESSAGE =
  "The AI features have reached their usage limit, so the assistant is unavailable right now. " +
  "This is a service limit rather than a problem with your request — please let the LCRB team know if you're seeing this during testing."

const TRANSIENT_MESSAGE =
  "The assistant is briefly unavailable because it's handling too many requests at once. Please wait a few seconds and try again."

const GENERIC_MESSAGE = "Sorry — I ran into a problem reaching the assistant. Please try again in a moment."

/** Friendly text for a streamed response's `onError` handler. */
export function friendlyStreamError(error: unknown): string {
  const { kind } = classifyAiError(error)
  if (kind === "quota") return QUOTA_MESSAGE
  return isRateLimited(error) ? TRANSIENT_MESSAGE : GENERIC_MESSAGE
}

/** User-facing copy for a classified failure, for client-side panels. */
export function aiErrorMessage(error: unknown): string {
  return friendlyStreamError(error)
}

/** Friendly { error, retryable } payload plus HTTP status for JSON AI routes. */
export function friendlyJsonError(
  error: unknown,
  fallback?: string,
): {
  body: { error: string; retryable: boolean; kind: AiFailureKind }
  status: number
} {
  const { kind } = classifyAiError(error)

  if (kind === "quota") {
    // 503 (capability unavailable), not 429: a client that retries a 429 would burn more of an exhausted budget.
    return { body: { error: QUOTA_MESSAGE, retryable: false, kind }, status: 503 }
  }
  if (isRateLimited(error)) {
    return { body: { error: TRANSIENT_MESSAGE, retryable: true, kind }, status: 429 }
  }
  return { body: { error: fallback ?? GENERIC_MESSAGE, retryable: false, kind: "transient" }, status: 500 }
}
