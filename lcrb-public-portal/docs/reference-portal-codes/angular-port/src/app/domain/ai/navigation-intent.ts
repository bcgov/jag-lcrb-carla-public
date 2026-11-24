/** Separates "take me there" (command) from "can I go there?" (question): the assistant offers to navigate and waits for consent. Shared by the API route and the panel. */

/** Imperatives that request movement even when phrased as a polite question. */
const MOVE_ME_PATTERNS = [
  /\btake me\b/,
  /\bbring me\b/,
  /\bsend me\b/,
  /\bnavigate me\b/,
  /\bgo ahead and (open|go|take)\b/,
]

/** Sentence-initial imperatives: "open my licences", "show me the queue". */
const IMPERATIVE_START =
  /^\s*(please\s+)?(take|bring|send|go|open|show|navigate|jump|switch|visit|launch|start|view)\b/

/** Question openers that signal the user wants information, not movement. */
const QUESTION_START =
  /^\s*(can|could|would|will|should|may|is|are|was|were|do|does|did|have|has|am|what|where|which|how|why|when|who|whose|whom)\b/

/** True when the user explicitly asked to be moved; questions are information requests, except an explicit "can you take me to X?". */
export function isExplicitNavigationRequest(text: string): boolean {
  if (!text) return false
  const t = text.trim().toLowerCase()
  if (!t) return false

  // "Can you take me to the map?" is a question in form but a command.
  if (MOVE_ME_PATTERNS.some((re) => re.test(t))) return true

  // "Where is the checklist?" / "Can I go to the next page?" are informational; a trailing question mark alone makes it a question.
  if (QUESTION_START.test(t) || t.endsWith("?")) return false

  return IMPERATIVE_START.test(t)
}

/** Pages an SEP applicant may be sent to; the licensee area is a dead end for them. Mirrors the applicant header navigation plus universally reachable pages. */
export const APPLICANT_ALLOWED_PATHS = [
  "/special-events",
  "/special-events/apply",
  "/announcements",
  "/account",
  "/contact",
  "/map",
]

/** Whether a role may be navigated to a path; only applicants are restricted, other roles' destinations are already scoped by the nav catalog. */
export function isPathAllowedForRole(role: string, href: string): boolean {
  if (role !== "applicant") return true
  // External schemes (tel:, mailto:) are always fine; they aren't portal areas.
  if (/^(tel:|mailto:|https?:\/\/)/i.test(href.trim())) return true
  const path = href.split("?")[0].split("#")[0].replace(/\/+$/, "") || "/"
  return APPLICANT_ALLOWED_PATHS.some((p) => path === p || path.startsWith(`${p}/`))
}

/** Confirmation prompt wording for when the assistant wants to move the user without being asked. */
export function navigationConsentPrompt(label: string): string {
  return `I can take you to ${label}. Want me to open it?`
}
