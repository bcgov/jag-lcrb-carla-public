/** Receipt model. A receipt is a read-only view over a captured payment. */

import { listTransactions, type Transaction } from "./payment-service"
import { listSubmittedApplications } from "../applications/mock/submitted-applications"
import { type SubmittedApplication } from "@domain/models/applications"
import { getAllSubmissions } from "../applications/submissions"
import { type Receipt } from "@domain/models/payments"

/** Deterministic receipt number for a seeded application, derived from its confirmation number. */
function seededReceiptNumber(app: SubmittedApplication): string {
  const suffix = app.confirmationNumber.split("-").pop() ?? app.id
  return `RCPT-${app.submittedDate.slice(0, 4)}-${suffix}`
}

export function receiptFromTransaction(tx: Transaction, extra: Partial<Receipt> = {}): Receipt | null {
  if (tx.status === "pending" || tx.status === "failed") return null
  if (tx.total <= 0) return null
  return {
    receiptNumber: tx.receiptNumber,
    jobNumber: tx.subjectId ?? "—",
    title: tx.description,
    payerName: tx.payerName,
    lineItems: tx.lineItems.map((l) => ({ label: l.label, amount: l.amount })),
    subtotal: tx.subtotal,
    serviceFee: tx.serviceFee,
    total: tx.total,
    currency: tx.currency,
    method: tx.method,
    processorRef: tx.processorRef || undefined,
    status: tx.status,
    paidAt: tx.paidAt ?? tx.createdAt,
    refundedAmount: tx.refunds.reduce((a, r) => a + r.amount, 0),
    ...extra,
  }
}

export function receiptFromSubmittedApplication(app: SubmittedApplication): Receipt | null {
  const total = app.fee.reduce((a, f) => a + f.amount, 0)
  if (total <= 0) return null
  return {
    receiptNumber: seededReceiptNumber(app),
    confirmationNumber: app.confirmationNumber,
    jobNumber: app.reference,
    applicationHref: `/applications/submitted/${app.id}`,
    title: `${app.type} Application`,
    payerName: app.applicant,
    lineItems: app.fee,
    subtotal: total,
    serviceFee: 0,
    total,
    currency: app.currency,
    method: app.paymentMethod,
    status: "paid",
    paidAt: `${app.submittedDate}T10:30:00`,
    refundedAmount: 0,
  }
}

/** All receipts the current user can see, newest first. */
export function listReceipts(): Receipt[] {
  const submissionsByJob = new Map(getAllSubmissions().map((s) => [s.jobNumber, s]))
  const live = listTransactions()
    .map((tx) => {
      const sub = submissionsByJob.get(tx.subjectId ?? "")
      return receiptFromTransaction(tx, {
        confirmationNumber: sub?.confirmationNumber,
        payerEmail: sub?.payerEmail,
        applicationHref: sub ? `/applications/confirmation/${sub.id}` : undefined,
      })
    })
    .filter((r): r is Receipt => r !== null)
  const liveJobs = new Set(live.map((r) => r.jobNumber))
  const seeded = listSubmittedApplications()
    .filter((app) => !liveJobs.has(app.reference))
    .map(receiptFromSubmittedApplication)
    .filter((r): r is Receipt => r !== null)
  return [...live, ...seeded].sort((a, b) => (a.paidAt < b.paidAt ? 1 : -1))
}

export function getReceipt(receiptNumber: string): Receipt | undefined {
  return listReceipts().find((r) => r.receiptNumber === receiptNumber)
}

export function getReceiptForJob(jobNumber: string): Receipt | undefined {
  return listReceipts().find((r) => r.jobNumber === jobNumber)
}
