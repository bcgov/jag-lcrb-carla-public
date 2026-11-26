// Mock append-only audit trail on the KV store; in production write to an immutable audit store, keeping `record()`.

import { createCollection, makeId } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"

export type AuditCategory =
  | "auth"
  | "application"
  | "payment"
  | "review"
  | "document"
  | "notification"
  | "integration"
  | "system"

export interface AuditEntry extends Entity {
  /** ISO timestamp. */
  at: string
  category: AuditCategory
  /** Short machine-ish action code, e.g. "application.submitted". */
  action: string
  /** Human-readable description for the activity feed. */
  summary: string
  /** Who performed it (display name) + role. */
  actor: string
  actorRole?: string
  /** Optional subject this action targeted (job number, permit id, etc.). */
  subjectId?: string
  subjectType?: string
  /** Arbitrary structured context for drill-down. */
  meta?: Record<string, string | number | boolean | null>
}

const collection = createCollection<AuditEntry>("audit")

export interface RecordAuditInput {
  category: AuditCategory
  action: string
  summary: string
  actor: string
  actorRole?: string
  subjectId?: string
  subjectType?: string
  meta?: AuditEntry["meta"]
}

/** Append an entry to the audit trail. Returns the stored entry. */
export function recordAudit(input: RecordAuditInput): AuditEntry {
  const entry: AuditEntry = {
    id: makeId("aud"),
    at: new Date().toISOString(),
    ...input,
  }
  collection.upsert(entry)
  return entry
}

/** All audit entries, newest first. */
export function listAudit(): AuditEntry[] {
  return collection
    .list()
    .sort((a, b) => (a.at < b.at ? 1 : a.at > b.at ? -1 : 0))
}

/** Audit entries for a specific subject (e.g. one application), newest first. */
export function listAuditForSubject(subjectId: string): AuditEntry[] {
  return listAudit().filter((e) => e.subjectId === subjectId)
}

/** Filter helper for the audit explorer UI. */
export function filterAudit(opts: {
  category?: AuditCategory | "all"
  actor?: string
  search?: string
}): AuditEntry[] {
  const search = opts.search?.trim().toLowerCase()
  return listAudit().filter((e) => {
    if (opts.category && opts.category !== "all" && e.category !== opts.category) return false
    if (opts.actor && e.actor !== opts.actor) return false
    if (search) {
      const hay = `${e.summary} ${e.action} ${e.actor} ${e.subjectId ?? ""}`.toLowerCase()
      if (!hay.includes(search)) return false
    }
    return true
  })
}

export const auditCollection = collection
