"use client"

/** Reactive store for the licensee's response to an active Enforcement Action. */

import { computed, type Signal } from "@angular/core"
import { injectExternalStore, injectHydrated } from "@shared/util/external-store"
import { createCollection, makeId } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { recordAudit } from "../activity/audit-service"
import { notify } from "../notifications/notification-service"
import { ENFORCEMENT_ACTIONS } from "./mock/enforcement"
import { formatEnforcementDate } from "@domain/compliance/enforcement"
import { type EnforcementAction } from "@domain/models/compliance"
import { type EnforcementStatus } from "@domain/models/compliance"

/** How the licensee elected to respond to the NEA. */
export type ResponseElection = "accept" | "dispute" | "request-hearing"

export interface EnforcementResponse extends Entity {
  /** Matches EnforcementAction.id. */
  id: string
  status: EnforcementStatus
  election?: ResponseElection
  /** The licensee's written statement. */
  statement?: string
  submittedAt?: string
  submittedBy?: string
  /** ISO hearing date once scheduled (simulated). */
  hearingDate?: string
}

const collection = createCollection<EnforcementResponse>("enforcement-responses")

/** The current status for an action (defaults to its base status if untouched). */
export function statusFor(action: EnforcementAction): EnforcementStatus {
  return collection.get(action.id)?.status ?? action.baseStatus
}

export function responseFor(actionId: string): EnforcementResponse | undefined {
  return collection.get(actionId)
}

export interface SubmitResponseInput {
  action: EnforcementAction
  election: ResponseElection
  statement: string
  actor: string
  email?: string
}

/** Submit the licensee's response to an NEA. */
export function submitResponse(input: SubmitResponseInput): EnforcementResponse {
  const { action, election, statement, actor, email } = input
  const requestsHearing = election === "request-hearing"

  let hearingDate: string | undefined
  if (requestsHearing) {
    const d = new Date()
    d.setDate(d.getDate() + 21)
    hearingDate = d.toISOString()
  }

  const record: EnforcementResponse = {
    id: action.id,
    status: requestsHearing ? "hearing-scheduled" : "response-submitted",
    election,
    statement,
    submittedAt: new Date().toISOString(),
    submittedBy: actor,
    hearingDate,
  }
  collection.upsert(record)

  recordAudit({
    category: "review",
    action: "enforcement.response_submitted",
    summary: `Response submitted for enforcement action ${action.referenceNumber} (${election})`,
    actor,
    subjectId: action.referenceNumber,
    subjectType: "enforcement-action",
    meta: { election },
  })

  notify({
    recipient: "licensee",
    category: "decision",
    title: `Response received — ${action.referenceNumber}`,
    body: requestsHearing
      ? `Your request for an enforcement hearing on ${action.allegation} has been received. A hearing has been scheduled for ${formatEnforcementDate(hearingDate as string)}.`
      : `Your response to the enforcement action regarding ${action.allegation} has been received and will be reviewed by LCRB compliance staff.`,
    href: "/compliance",
    subjectId: action.referenceNumber,
    channels: email ? ["in-portal", "email"] : ["in-portal"],
    email,
  })

  return record
}

/** Withdraw a not-yet-resolved response (returns to awaiting-response). */
export function withdrawResponse(actionId: string, actor: string): void {
  const existing = collection.get(actionId)
  if (!existing || existing.status === "resolved") return
  collection.remove(actionId)
  recordAudit({
    category: "review",
    action: "enforcement.response_withdrawn",
    summary: `Response withdrawn for enforcement action ${actionId}`,
    actor,
    subjectId: actionId,
    subjectType: "enforcement-action",
  })
}

export interface EnforcementView extends EnforcementAction {
  status: EnforcementStatus
  response?: EnforcementResponse
}

function buildViews(): EnforcementView[] {
  return ENFORCEMENT_ACTIONS.map((a) => {
    const response = collection.get(a.id)
    return { ...a, status: response?.status ?? a.baseStatus, response }
  })
}

/** Reactive read of all enforcement actions with their live status. */
export function injectEnforcementActions(): Signal<{ actions: EnforcementView[]; hydrated: boolean }> {
  const version = injectExternalStore(collection.subscribe, collection.snapshot)
  const hydrated = injectHydrated()
  return computed(() => (version(), { actions: buildViews(), hydrated: hydrated() }))
}

/** Count of actions still awaiting a licensee response. */
export function injectOpenEnforcementCount(): Signal<number> {
  const feed = injectEnforcementActions()
  return computed(() => {
    const { actions, hydrated } = feed()
    if (!hydrated) return ENFORCEMENT_ACTIONS.filter((a) => a.baseStatus === "awaiting-response").length
    return actions.filter((a) => a.status === "awaiting-response").length
  })
}

export const enforcementCollection = collection
