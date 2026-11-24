"use client"

/** Police-controlled SEP auto-approval policy. */

import { createCollection } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { recordAudit } from "../activity/audit-service"
import { MIN_NOTICE_DAYS } from "@domain/special-events/auto-approval"

export interface SepAutoApprovalPolicy extends Entity {
  id: "policy"
  enabled: boolean
  minNoticeDays: number
  updatedAt: string | null
  updatedBy: string | null
  agency: string | null
}

export const DEFAULT_POLICY: SepAutoApprovalPolicy = {
  id: "policy",
  enabled: false,
  minNoticeDays: MIN_NOTICE_DAYS,
  updatedAt: null,
  updatedBy: null,
  agency: null,
}

export const autoApprovalCollection = createCollection<SepAutoApprovalPolicy>("sep-auto-approval")

export function getAutoApprovalPolicy(): SepAutoApprovalPolicy {
  return autoApprovalCollection.get("policy") ?? DEFAULT_POLICY
}

export function setAutoApprovalEnabled(enabled: boolean, actor: string, agency: string): SepAutoApprovalPolicy {
  const next: SepAutoApprovalPolicy = {
    ...getAutoApprovalPolicy(),
    enabled,
    updatedAt: new Date().toISOString(),
    updatedBy: actor,
    agency,
  }
  autoApprovalCollection.upsert(next)
  recordAudit({
    category: "system",
    action: enabled ? "sep.auto-approval.enabled" : "sep.auto-approval.disabled",
    summary: `${enabled ? "Enabled" : "Disabled"} SEP auto-approval for ${agency} (minimum ${next.minNoticeDays} days' notice)`,
    actor,
    actorRole: "police",
    subjectId: "sep-auto-approval",
  })
  return next
}

/** Whole days from `fromIso` (default today) to the event start date. */
export function daysUntilEvent(eventStartIso: string, fromIso?: string): number {
  const start = new Date(`${eventStartIso}T00:00:00`).getTime()
  const from = fromIso ? new Date(`${fromIso}T00:00:00`).getTime() : new Date(new Date().toDateString()).getTime()
  return Math.floor((start - from) / 86_400_000)
}

export interface GateResult {
  /** True when the policy allows auto-approval for this event's timing. */
  allowed: boolean
  /** Reason shown to applicant and reviewer when not allowed. */
  reason?: string
  daysOut: number
}

/** The policy layer that sits in front of the standard referral rules. */
export function evaluateAutoApprovalGate(
  policy: SepAutoApprovalPolicy,
  eventStartIso: string,
  submittedIso?: string,
): GateResult {
  const daysOut = daysUntilEvent(eventStartIso, submittedIso)
  if (!policy.enabled) {
    return {
      allowed: false,
      daysOut,
      reason: "Auto-approval is not enabled by the police agency for this jurisdiction; all permits are reviewed by an officer.",
    }
  }
  if (daysOut < policy.minNoticeDays) {
    return {
      allowed: false,
      daysOut,
      reason: `Event is ${daysOut < 0 ? 0 : daysOut} day${daysOut === 1 ? "" : "s"} away; auto-approval requires at least ${policy.minNoticeDays} days' notice.`,
    }
  }
  return { allowed: true, daysOut }
}
