/** Easter egg: typing "vijaivir" gets a fixed reply from the route before any model call, so it costs no tokens. */

/** The exact reply. Kept as a single source of truth for tests. */
export const EASTER_EGG_REPLY =
  "The man of the hour, the man with the power, the man who's too sweet to be sour."

/** Licensee-only: staff roles see a regulatory tool and an applicant is a member of the public mid-application. */
const EGG_ROLES = new Set(["licensee"])

/** Matches "vijaivir" as its own word, case-insensitively; explicit letter/digit lookarounds instead of \b so a longer word containing the name does not match. */
const EGG_PATTERN = /(?<![\p{L}\p{N}])vijaivir(?![\p{L}\p{N}])/iu

/** True when this turn should return the fixed easter-egg reply. */
export function isEasterEggTrigger(text: string, role: string | undefined): boolean {
  if (!text || !role) return false
  if (!EGG_ROLES.has(role)) return false
  return EGG_PATTERN.test(text)
}
