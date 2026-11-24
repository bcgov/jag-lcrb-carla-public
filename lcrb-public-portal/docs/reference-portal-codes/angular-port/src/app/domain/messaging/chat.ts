import type { UserRole } from "@domain/roles"
import { type ThreadKind } from "@domain/models/messaging"

/** Roles that may read and write each kind of thread. */
export const THREAD_PARTICIPANTS: Record<ThreadKind, UserRole[]> = {
  application: ["licensee", "applicant", "lcrb", "local-government"],
  sep: ["applicant", "licensee", "lcrb", "police"],
}

export const ROLE_LABELS: Record<UserRole, string> = {
  licensee: "Applicant",
  applicant: "Applicant",
  lcrb: "LCRB staff",
  "local-government": "Local government",
  police: "Police",
}

/** Statuses (per kind) during which the thread accepts new messages. */
const OPEN_STATUSES: Record<ThreadKind, Set<string>> = {
  application: new Set(["under_review", "pending_documents", "pending-review", "documents-requested", "community-review"]),
  sep: new Set(["submitted", "under-review", "changes-requested"]),
}

export function isThreadOpen(kind: ThreadKind, status: string): boolean {
  return OPEN_STATUSES[kind].has(status)
}

export function formatMessageTime(iso: string): string {
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return iso
  const now = new Date()
  const sameDay = d.toDateString() === now.toDateString()
  const time = d.toLocaleTimeString("en-CA", { hour: "numeric", minute: "2-digit" })
  if (sameDay) return `Today ${time}`
  return `${d.toLocaleDateString("en-CA", { month: "short", day: "numeric" })} ${time}`
}
