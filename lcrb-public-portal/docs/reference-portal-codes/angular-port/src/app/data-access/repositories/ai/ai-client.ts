/** Shared client-side helper for the portal's JSON AI endpoints. */

/** Max simultaneous AI JSON requests. Extra calls queue until a slot frees up. */
const MAX_CONCURRENT = 2

// Module-level state (shared across every component that imports this helper).
const inflight = new Map<string, Promise<unknown>>()
let active = 0
const waiters: Array<() => void> = []

/** Acquire a concurrency slot, queueing if all slots are busy. */
function acquireSlot(): Promise<void> {
  if (active < MAX_CONCURRENT) {
    active += 1
    return Promise.resolve()
  }
  return new Promise<void>((resolve) => waiters.push(resolve))
}

/** Release a concurrency slot and wake the next queued request, if any. */
function releaseSlot(): void {
  const next = waiters.shift()
  if (next) {
    // Hand the slot straight to the next waiter (active count stays the same).
    next()
  } else {
    active = Math.max(0, active - 1)
  }
}

function readCache<T>(key: string): T | null {
  try {
    const raw = sessionStorage.getItem(key)
    return raw ? (JSON.parse(raw) as T) : null
  } catch {
    return null
  }
}

function writeCache(key: string, value: unknown): void {
  try {
    sessionStorage.setItem(key, JSON.stringify(value))
  } catch {
    // Storage may be full or unavailable (private mode) - non-fatal.
  }
}

/** Read a previously cached AI result without issuing a request. */
export function readAiJsonCache<T>(key: string): T | null {
  return readCache<T>(key)
}

/** Small, stable string hash for building compact cache keys from payloads. */
export function hashKey(input: string): string {
  let hash = 5381
  for (let i = 0; i < input.length; i++) {
    hash = (hash * 33) ^ input.charCodeAt(i)
  }
  // Unsigned, base-36 for a short slug.
  return (hash >>> 0).toString(36)
}

export interface RequestAiJsonOptions {
  /** Endpoint to POST to, e.g. "/api/nudges". */
  url: string
  /** Request body (object is JSON-stringified; a string is sent as-is). */
  body: unknown
  /** Stable identity for this request. Callers with the same key share an in-flight request and a cached result. */
  key: string
  /** Persist successful results in sessionStorage under `key`. */
  cache?: boolean
}

/** POST JSON to an AI endpoint with in-flight dedup, session caching, and a global concurrency ceiling. */
export async function requestAiJson<T>(opts: RequestAiJsonOptions): Promise<T> {
  const { url, body, key, cache = true } = opts

  if (cache) {
    const cached = readCache<T>(key)
    if (cached !== null) return cached
  }

  // Share an already-running identical request instead of issuing another.
  const existing = inflight.get(key)
  if (existing) return existing as Promise<T>

  const promise = (async () => {
    await acquireSlot()
    try {
      const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: typeof body === "string" ? body : JSON.stringify(body),
      })
      if (!res.ok) throw new Error(`AI request to ${url} failed: ${res.status}`)
      const data = (await res.json()) as T
      if (cache) writeCache(key, data)
      return data
    } finally {
      releaseSlot()
      inflight.delete(key)
    }
  })()

  inflight.set(key, promise)
  return promise as Promise<T>
}
