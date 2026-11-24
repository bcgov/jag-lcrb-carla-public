/** Receipt email. */

import { notify, type DeliveryLogEntry } from "../notifications/notification-service"
import { recordAudit } from "../activity/audit-service"
import type { UserRole } from "@domain/roles"
import { formatReceiptCurrency, formatReceiptDate, receiptFilename } from "@domain/payments/receipt"
import { type Receipt } from "@domain/models/payments"
import { receiptPdfSizeBytes } from "@shared/util/pdf/receipt-pdf"

export interface ReceiptEmail {
  subject: string
  body: string
}

export function buildReceiptEmail(receipt: Receipt, portalOrigin = ""): ReceiptEmail {
  const money = (n: number) => formatReceiptCurrency(n, receipt.currency)
  const lines = receipt.lineItems.map((l) => `  ${l.label.padEnd(52)} ${money(l.amount)}`)
  const refs = [
    `Receipt number:        ${receipt.receiptNumber}`,
    receipt.confirmationNumber ? `Confirmation number:   ${receipt.confirmationNumber}` : null,
    `Application reference: ${receipt.jobNumber}`,
    `Date:                  ${formatReceiptDate(receipt.paidAt)}`,
    `Payment method:        ${receipt.method || "—"}`,
  ].filter(Boolean)

  const body = [
    `Hello ${receipt.payerName},`,
    "",
    `Thank you. Your payment of ${money(receipt.total)} for ${receipt.title} has been received by the Liquor and Cannabis Regulation Branch.`,
    "",
    ...refs,
    "",
    "What you paid for",
    ...lines,
    `  ${"Total paid".padEnd(52)} ${money(receipt.total)}`,
    "",
    `Your official receipt is attached as ${receiptFilename(receipt)}. You can also download it any time from the Receipts page in the portal${portalOrigin ? ` (${portalOrigin}/receipts)` : ""}.`,
    "",
    "What happens next",
    "A licensing analyst will review your application. We will email you if we need anything else, and you can track progress under My Applications.",
    "",
    "Application fees are non-refundable once an application has been accepted for review.",
    "",
    "Liquor and Cannabis Regulation Branch",
    "Province of British Columbia",
    "1-866-209-2111",
    "",
    "This is an automated message; replies to this address are not monitored.",
  ].join("\n")

  return { subject: `Your LCRB payment receipt ${receipt.receiptNumber}`, body }
}

export interface SendReceiptEmailInput {
  receipt: Receipt
  to: string
  /** Role used to drop a matching card in the in-portal inbox. */
  recipientRole?: UserRole
  actor?: string
  /** Whether this is a user-initiated resend (changes the audit summary only). */
  resend?: boolean
}

export function sendReceiptEmail(input: SendReceiptEmailInput): void {
  const { receipt, to, recipientRole, actor = "System", resend = false } = input
  const origin = typeof window !== "undefined" ? window.location.origin : ""
  const email = buildReceiptEmail(receipt, origin)
  const attachment = {
    filename: receiptFilename(receipt),
    contentType: "application/pdf",
    sizeBytes: receiptPdfSizeBytes(receipt),
  }
  notify({
    recipient: recipientRole ?? "licensee",
    category: "payment",
    title: email.subject,
    body: `Receipt ${receipt.receiptNumber} for ${receipt.title} (${formatReceiptCurrency(receipt.total, receipt.currency)}) was emailed to ${to}.`,
    href: `/receipts?receipt=${encodeURIComponent(receipt.receiptNumber)}`,
    subjectId: receipt.receiptNumber,
    channels: resend ? ["email"] : ["in-portal", "email"],
    email: to,
    emailBody: email.body,
    attachments: [attachment],
  })
  recordAudit({
    category: "payment",
    action: resend ? "receipt.resent" : "receipt.emailed",
    summary: `${resend ? "Receipt re-sent" : "Receipt emailed"} to ${to} (${receipt.receiptNumber})`,
    actor,
    subjectId: receipt.jobNumber,
    meta: { receiptNumber: receipt.receiptNumber, to },
  })
}

export function latestReceiptEmail(entries: DeliveryLogEntry[]): DeliveryLogEntry | undefined {
  return entries.find((e) => e.channel === "email")
}
