"use client"

/** Applicant-initiated cancellation of liquor / cannabis licence applications. */

import { createCollection } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { recordAudit } from "../activity/audit-service"
import { notify } from "../notifications/notification-service"

export interface ApplicationCancellation extends Entity {
  /** `id` is the job number so lookups are O(1). */
  id: string
  /** Display label, e.g. "Liquor Primary Licence". */
  applicationType: string
  /** Applicant / business the file belongs to. */
  applicant: string
  cancelledAt: string
  cancelledBy: string
  reason: string
  /** Status the file was in when the applicant cancelled (for the record). */
  priorStatus: string
  /** Which partner(s) had been referred the file. */
  referredTo: Array<"local-government" | "lcrb">
}

export const cancellationCollection = createCollection<ApplicationCancellation>("application-cancellations")

export interface CancelApplicationInput {
  jobNumber: string
  applicationType: string
  applicant: string
  actor: string
  reason: string
  priorStatus: string
  referredTo?: Array<"local-government" | "lcrb">
}

export function getCancellation(jobNumber: string): ApplicationCancellation | undefined {
  return cancellationCollection.get(jobNumber)
}

export function listCancellations(): ApplicationCancellation[] {
  return cancellationCollection.list()
}

export function cancelApplication(input: CancelApplicationInput): ApplicationCancellation {
  const referredTo = input.referredTo ?? ["lcrb"]
  const record: ApplicationCancellation = {
    id: input.jobNumber,
    applicationType: input.applicationType,
    applicant: input.applicant,
    cancelledAt: new Date().toISOString(),
    cancelledBy: input.actor,
    reason: input.reason.trim(),
    priorStatus: input.priorStatus,
    referredTo,
  }
  cancellationCollection.upsert(record)

  recordAudit({
    category: "application",
    action: "application.cancelled",
    summary: `Applicant cancelled ${input.applicationType} (${input.jobNumber})`,
    actor: input.actor,
    subjectId: input.jobNumber,
    meta: { reason: record.reason, priorStatus: input.priorStatus },
  })

  const body = `${input.applicant} withdrew ${input.applicationType} (job ${input.jobNumber}).${
    record.reason ? ` Reason: ${record.reason}` : ""
  } Any comment you recorded stays on file; no further action is needed.`

  // LCRB staff always own the file; partners only hear about it if they were referred the application.
  notify({
    recipient: "lcrb",
    category: "application",
    title: "Application cancelled by applicant",
    body,
    href: "/lcrb/triage",
    subjectId: input.jobNumber,
  })
  if (referredTo.includes("local-government")) {
    notify({
      recipient: "local-government",
      category: "application",
      title: "Referral cancelled by applicant",
      body,
      href: `/local-government/review/${input.jobNumber}`,
      subjectId: input.jobNumber,
      channels: ["in-portal", "email"],
    })
  }

  // Confirmation back to the applicant.
  notify({
    recipient: "licensee",
    category: "application",
    title: "Your application has been cancelled",
    body: `${input.applicationType} (job ${input.jobNumber}) has been withdrawn. Fees already paid are not refunded. Start a new application at any time.`,
    href: "/applications",
    subjectId: input.jobNumber,
    channels: ["in-portal", "email"],
  })

  return record
}
