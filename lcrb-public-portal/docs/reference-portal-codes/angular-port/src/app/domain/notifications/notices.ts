import { type Notice } from "@domain/models/notifications"

/** Format an ISO date as a long-form date, e.g. "May 25, 2026". */
export function formatNoticeDate(iso: string): string {
  return new Date(iso).toLocaleDateString("en-CA", {
    year: "numeric",
    month: "long",
    day: "numeric",
  })
}

/** Flattens a notice document into plain text for copy and text-only consumers. */
export function noticeToPlainText(notice: Notice): string {
  const d = notice.document
  const lines: string[] = []
  lines.push("LIQUOR AND CANNABIS REGULATION BRANCH")
  lines.push("Province of British Columbia")
  lines.push("")
  lines.push(formatNoticeDate(notice.date))
  lines.push(`File: ${d.documentNumber}`)
  lines.push("")
  lines.push(d.recipientName)
  lines.push(d.recipientBusiness)
  lines.push(...d.recipientAddress)
  lines.push("")
  lines.push(`RE: ${d.subject}`)
  lines.push("")
  lines.push(d.salutation)
  lines.push("")
  for (const p of d.body) {
    lines.push(p)
    lines.push("")
  }
  lines.push(d.closing)
  lines.push("")
  lines.push(d.signatoryName)
  lines.push(d.signatoryTitle)
  lines.push(d.signatoryBranch)
  if (d.enclosures?.length) {
    lines.push("")
    lines.push(`Enclosures: ${d.enclosures.join("; ")}`)
  }
  if (d.references?.length) {
    lines.push("")
    lines.push(`References: ${d.references.join("; ")}`)
  }
  return lines.join("\n")
}
