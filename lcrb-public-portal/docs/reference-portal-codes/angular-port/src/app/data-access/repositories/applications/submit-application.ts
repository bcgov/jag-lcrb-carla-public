import {
  saveSubmission,
  makeConfirmationNumber,
  makeJobNumber,
  type SubmissionRecord,
  type ApplicationAssessment,
} from "./submissions"
import { createPayment, processPayment } from "../payments/payment-service"
import { pushCase } from "../activity/dynamics-service"
import { notify } from "../notifications/notification-service"
import { recordAudit } from "../activity/audit-service"
import { receiptFromTransaction } from "../payments/receipt-model"
import { sendReceiptEmail } from "../payments/receipt-email"
import type { UserRole } from "@domain/roles"
import type { AgentLog } from "@domain/models/staff"

export interface SubmitApplicationInput {
  /** Catalog flow id used to derive the job number prefix. */
  flowId: string
  /** Human-facing application title. */
  title: string
  /** Applicant / business display name. */
  applicant: string
  /** Flattened review rows shown on the confirmation + staff record. */
  summary: { label: string; value: string }[]
  /** Total application fee in dollars (0 = no payment required). */
  fee: number
  /** Acting user's display name (from the session). */
  actor: string
  /** Acting user's role, used to route the in-portal notification. */
  role?: UserRole
  /** Applicant's account email - where the confirmation + receipt are sent. */
  email?: string
  /** Optional pre-generated staff assessment (Food Primary supplies none). */
  assessment?: ApplicationAssessment | null
  /** Optional AI pre-screening agent log (checks, confidence, recommended queue). */
  agentLog?: AgentLog | null
}

/** Single source of truth for submitting any application. */
export function submitApplication(input: SubmitApplicationInput): SubmissionRecord {
  const { flowId, title, applicant, summary, fee, actor, role, email, assessment = null, agentLog = null } = input
  const id = (globalThis.crypto?.randomUUID?.() ?? `sub-${Date.now()}`).slice(0, 36)
  const jobNumber = makeJobNumber(flowId)
  const recipient: UserRole = role ?? "licensee"
  const payerEmail = email || `${applicant.replace(/\s+/g, ".").toLowerCase()}@example.com`
  const lineItems = fee > 0 ? [{ label: `${title} — Application Fee`, amount: fee }] : []

  // 1) Payment via the PayBC/Bambora boundary (auto-captured in the prototype).
  const tx = createPayment({ description: title, subjectId: jobNumber, payerName: applicant, lineItems })
  const paidTx = fee > 0 ? processPayment({ transactionId: tx.id, method: "Visa ending in 4242", actor }) ?? tx : tx

  // 2) Push the case to MS Dynamics and capture the real sync state.
  const sync = pushCase({
    reference: jobNumber,
    caseType: title,
    applicant,
    payloadSummary: `${summary.length} fields; assessment ${assessment ? "included" : "unavailable"}${
      agentLog ? `; agent queue ${agentLog.recommendedQueue}` : ""
    }`,
    actor,
    queue: agentLog?.recommendedQueue,
    agentLog,
  })

  const record: SubmissionRecord = {
    id,
    confirmationNumber: makeConfirmationNumber(),
    jobNumber,
    flowId,
    title,
    applicant,
    lineItems,
    total: fee,
    currency: "CAD",
    paymentMethod: fee > 0 ? paidTx.method || "Visa ending in 4242" : "No payment required",
    receiptNumber: fee > 0 ? paidTx.receiptNumber : undefined,
    payerEmail,
    submittedAt: new Date().toISOString(),
    summary,
    syncStatus: sync.state,
    assessment,
    agentLog,
  }
  saveSubmission(record)

  // 3) Audit + applicant notification (in-portal + simulated email).
  recordAudit({
    category: "application",
    action: "application.submitted",
    summary: `${title} submitted (${jobNumber})`,
    actor,
    actorRole: role,
    subjectId: jobNumber,
    subjectType: "application",
    meta: { total: fee, receiptNumber: tx.receiptNumber },
  })
  notify({
    recipient,
    category: "application",
    title: "Application received",
    body: `We've received your ${title} (${jobNumber}). You can track its status from your dashboard.`,
    href: `/applications/confirmation/${id}`,
    subjectId: jobNumber,
    channels: ["in-portal", "email"],
    email: payerEmail,
  })

  // 4) Official receipt: emailed automatically with the PDF attached.
  if (fee > 0) {
    const receipt = receiptFromTransaction(paidTx, {
      confirmationNumber: record.confirmationNumber,
      payerEmail,
      applicationHref: `/applications/confirmation/${id}`,
    })
    if (receipt) {
      sendReceiptEmail({ receipt, to: payerEmail, recipientRole: recipient, actor })
    }
  }

  return record
}
