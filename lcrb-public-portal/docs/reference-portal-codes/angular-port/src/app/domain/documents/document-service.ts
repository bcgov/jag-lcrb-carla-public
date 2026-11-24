import { type UploadValidationError, type UploadValidationOk } from "@domain/models/documents"

export const ALLOWED_TYPES = [
  "application/pdf",
  "image/png",
  "image/jpeg",
  "image/jpg",
  "application/msword",
  "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
]

export const MAX_SIZE_BYTES = 10 * 1024 * 1024

/** Validate a file against type/size rules before "uploading". */
export function validateFile(file: { type: string; size: number }): UploadValidationError | UploadValidationOk {
  if (!ALLOWED_TYPES.includes(file.type)) {
    return { ok: false, reason: "Unsupported file type. Upload a PDF, image, or Word document." }
  }
  if (file.size > MAX_SIZE_BYTES) {
    return { ok: false, reason: "File exceeds the 10 MB limit." }
  }
  return { ok: true }
}

/** Human-readable file size. */
export function formatBytes(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(0)} KB`
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}
