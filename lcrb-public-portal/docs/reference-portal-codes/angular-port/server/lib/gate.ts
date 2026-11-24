/** Access gate: one shared passcode (ACCESS_CODE, server-side) keeps the prototype private. The cookie holds a SHA-256 token, so this is not per-user auth. */

export const GATE_COOKIE = "lcrb_access"

/** How long a successful unlock lasts before testers must re-enter the code. */
export const GATE_MAX_AGE_SECONDS = 60 * 60 * 5 // 5 hours

/** The shared passcode, read from the server environment only. */
export function getAccessCode(): string | undefined {
  const code = process.env.ACCESS_CODE
  return code && code.length > 0 ? code : undefined
}

/** Gate is enforced only when ACCESS_CODE is set; unset disables it so nobody gets locked out. */
export function isGateEnabled(): boolean {
  return getAccessCode() !== undefined
}

/** Opaque cookie token derived from the passcode; same input gives the same output in every runtime. */
export async function computeGateToken(code: string): Promise<string> {
  const data = new TextEncoder().encode(`lcrb-gate:v1:${code}`)
  const digest = await crypto.subtle.digest("SHA-256", data)
  return hex(new Uint8Array(digest))
}

/** The token value a valid cookie must contain for the current passcode. */
export async function expectedGateToken(): Promise<string | null> {
  const code = getAccessCode()
  if (!code) return null
  return computeGateToken(code)
}

/** Constant-time-ish string comparison to avoid leaking length/prefix timing. */
export function safeEqual(a: string, b: string): boolean {
  if (a.length !== b.length) return false
  let mismatch = 0
  for (let i = 0; i < a.length; i++) {
    mismatch |= a.charCodeAt(i) ^ b.charCodeAt(i)
  }
  return mismatch === 0
}

function hex(bytes: Uint8Array): string {
  let out = ""
  for (const b of bytes) out += b.toString(16).padStart(2, "0")
  return out
}
