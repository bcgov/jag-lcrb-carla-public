/** Statuses an applicant may still withdraw from. */
export const CANCELLABLE_STATUSES = new Set(["under_review", "pending_documents"])

export function isCancellable(status: string): boolean {
  return CANCELLABLE_STATUSES.has(status)
}

export function formatCancelledDate(iso: string): string {
  const d = new Date(iso)
  return Number.isNaN(d.getTime())
    ? iso
    : d.toLocaleDateString("en-CA", { year: "numeric", month: "long", day: "numeric" })
}
