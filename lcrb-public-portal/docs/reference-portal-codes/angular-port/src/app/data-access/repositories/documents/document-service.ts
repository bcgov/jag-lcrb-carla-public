"use client"

/** Document management boundary (prototype). */

import { createCollection, makeId } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { recordAudit } from "../activity/audit-service"

export type ScanState = "pending" | "clean" | "infected" | "error"

export interface StoredDocument extends Entity {
  subjectId: string
  name: string
  /** MIME type. */
  contentType: string
  /** Size in bytes. */
  size: number
  uploadedBy: string
  uploadedAt: string
  scanState: ScanState
  /** Logical category, e.g. "floor-plan", "lease", "id". */
  category?: string
  /** Optional inline data URL for small prototype files (not used in prod). */
  dataUrl?: string
  /** Vercel Blob pathname/URL when the file is stored in real Blob storage. */
  blobUrl?: string
}

const collection = createCollection<StoredDocument>("documents")

export interface UploadInput {
  subjectId: string
  name: string
  contentType: string
  size: number
  uploadedBy: string
  category?: string
  dataUrl?: string
  blobUrl?: string
}

/** Register an uploaded document. */
export function uploadDocument(input: UploadInput): StoredDocument {
  const doc: StoredDocument = {
    id: makeId("doc"),
    subjectId: input.subjectId,
    name: input.name,
    contentType: input.contentType,
    size: input.size,
    uploadedBy: input.uploadedBy,
    uploadedAt: new Date().toISOString(),
    scanState: "pending",
    category: input.category,
    dataUrl: input.dataUrl,
    blobUrl: input.blobUrl,
  }
  collection.upsert(doc)
  recordAudit({
    category: "document",
    action: "document.uploaded",
    summary: `Document "${input.name}" uploaded`,
    actor: input.uploadedBy,
    subjectId: input.subjectId,
    meta: { size: input.size, contentType: input.contentType },
  })
  return doc
}

/** Resolve the simulated virus scan (defaults to clean). */
export function completeScan(id: string, result: ScanState = "clean"): StoredDocument | undefined {
  return collection.update(id, { scanState: result })
}

/** Record that a document was downloaded (for the audit trail). */
export function recordDownload(id: string, actor: string): void {
  const doc = collection.get(id)
  if (!doc) return
  recordAudit({
    category: "document",
    action: "document.downloaded",
    summary: `Document "${doc.name}" downloaded`,
    actor,
    subjectId: doc.subjectId,
  })
}

export function listDocuments(subjectId: string): StoredDocument[] {
  return collection
    .list()
    .filter((d) => d.subjectId === subjectId)
    .sort((a, b) => (a.uploadedAt < b.uploadedAt ? 1 : -1))
}

export function removeDocument(id: string, actor: string): void {
  const doc = collection.get(id)
  if (!doc) return
  collection.remove(id)
  recordAudit({
    category: "document",
    action: "document.removed",
    summary: `Document "${doc.name}" removed`,
    actor,
    subjectId: doc.subjectId,
  })
}

export const documentCollection = collection
