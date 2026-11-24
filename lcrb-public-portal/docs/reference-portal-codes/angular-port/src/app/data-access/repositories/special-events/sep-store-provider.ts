/** Shared SEP records store. */
import { afterNextRender, effect, inject, Injectable, signal } from "@angular/core"
import { SEP_RECORDS } from "./mock/seps"
import { STANDARD_CONDITIONS, assessSepReferral } from "@domain/special-events/seps"
import { type SepRecord, type SepStatus, type SepCondition } from "@domain/models/special-events"
import { notify } from "../notifications/notification-service"
import { recordAudit } from "../activity/audit-service"
import { evaluateAutoApprovalGate, getAutoApprovalPolicy } from "./sep-auto-approval"

/** Payload an applicant submits from the SEP apply wizard. */
export interface SepSubmission {
  eventName: string
  eventType: string
  applicantName: string
  organization?: string
  venueName: string
  address: string
  city: string
  jurisdiction: string
  lat: number
  lng: number
  startDate: string
  endDate: string
  /** Daily liquor-service window ("HH:MM"). */
  startTime: string
  endTime: string
  attendance: number
  serviceType: string
  description?: string
  liquorTypes?: string[]
  estimatedDrinks?: number
  hasSecurity?: boolean
  securityStaffCount?: number
  /** Events on public property need external approval - a referral trigger. */
  isPublicProperty?: boolean
}

const STORAGE_KEY = "lcrb.sep.records"

function todayIso(): string {
  return new Date().toISOString().slice(0, 10)
}

const DECISION_ACTION: Record<string, string> = {
  approved: "Approved",
  denied: "Denied",
  "changes-requested": "Requested changes",
}

function cloneSeed(r: SepRecord): SepRecord {
  return { ...r, conditions: r.conditions.map((c) => ({ ...c })), history: r.history.map((h) => ({ ...h })) }
}

@Injectable({ providedIn: "root" })
export class SepStoreProvider {
  // Deep-clone the seed so edits don't mutate the module-level array.
  private readonly recordsState = signal<SepRecord[]>(SEP_RECORDS.map(cloneSeed))
  private readonly hydratedState = signal(false)
  readonly records = this.recordsState.asReadonly()

  constructor() {
    // Hydrate from localStorage so applicant submissions survive the reload when switching roles.
    afterNextRender(() => {
      try {
        const raw = localStorage.getItem(STORAGE_KEY)
        if (raw) {
          const stored = JSON.parse(raw) as SepRecord[]
          // Merge in any seed records the stored snapshot is missing (e.g. new demo events added after the user first loaded the app)
          const storedIds = new Set(stored.map((r) => r.id))
          const missingSeeds = SEP_RECORDS.filter((r) => !storedIds.has(r.id)).map(cloneSeed)
          this.recordsState.set([...stored, ...missingSeeds])
        }
      } catch {
        // ignore malformed storage
      }
      this.hydratedState.set(true)
    })

    effect(() => {
      const records = this.recordsState()
      if (!this.hydratedState()) return
      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(records))
      } catch {
        // ignore storage write failures
      }
    })
  }

  readonly getById = (id: string): SepRecord | undefined => this.recordsState().find((r) => r.id === id)

  readonly assignOfficer = (id: string, officer: string): void => {
    this.recordsState.update((prev) =>
      prev.map((r) =>
        r.id === id
          ? {
              ...r,
              assignedOfficer: officer,
              status: r.status === "submitted" ? "under-review" : r.status,
              history: [...r.history, { date: todayIso(), actor: officer, action: "Assigned to self" }],
            }
          : r,
      ),
    )
  }

  readonly setConditions = (id: string, conditions: SepCondition[]): void => {
    this.recordsState.update((prev) => prev.map((r) => (r.id === id ? { ...r, conditions } : r)))
  }

  readonly decide = (
    id: string,
    decision: Extract<SepStatus, "approved" | "denied" | "changes-requested">,
    actor: string,
    note: string,
  ): void => {
    let decided: SepRecord | undefined
    this.recordsState.update((prev) =>
      prev.map((r) => {
        if (r.id !== id) return r
        decided = {
          ...r,
          status: decision,
          history: [
            ...r.history,
            {
              date: todayIso(),
              actor,
              action: DECISION_ACTION[decision],
              note: note || undefined,
            },
          ],
        }
        return decided
      }),
    )

    if (!decided) return

      // Audit the reviewer decision.
      recordAudit({
        category: "application",
        action: `sep.${decision}`,
        summary: `${DECISION_ACTION[decision]} Special Event Permit ${decided.eventName} (${decided.id})`,
        actor,
        subjectId: decided.id,
        meta: note ? { note } : undefined,
      })

      // Notify the applicant. SEP applicants use the "applicant" role inbox.
      const titles: Record<typeof decision, string> = {
        approved: "Your Special Event Permit was approved",
        denied: "Your Special Event Permit was denied",
        "changes-requested": "Changes requested on your Special Event Permit",
      }
      const bodies: Record<typeof decision, string> = {
        approved: `${decided.eventName} has been approved. You can download your permit from the Special Event Permits area.`,
        denied: `${decided.eventName} was denied.${note ? ` Reason: ${note}` : ""}`,
        "changes-requested": `${decided.eventName} needs changes before it can be approved.${note ? ` ${note}` : ""}`,
      }
      notify({
        recipient: "applicant",
        category: "decision",
        title: titles[decision],
        body: bodies[decision],
        href: "/special-events",
        subjectId: decided.id,
        channels: decided.applicantEmail ? ["in-portal", "email"] : ["in-portal"],
        email: decided.applicantEmail || undefined,
      })
  }

  /** Adds a freshly submitted applicant SEP to the shared queue. Returns the new id. */
  readonly submitApplication = (submission: SepSubmission): string => {
    const id = `SEP-${new Date().getFullYear()}-${Math.floor(10000 + Math.random() * 89999)}`
    // The decision used to be `attendance < 500` and nothing else
    const referral = assessSepReferral({
      attendance: submission.attendance,
      startTime: submission.startTime,
      endTime: submission.endTime,
      estimatedDrinks: submission.estimatedDrinks,
      eventType: submission.eventType,
      hasSecurity: submission.hasSecurity,
      isPublicProperty: submission.isPublicProperty,
    })
    // Policy layer: the police agency must have turned auto-approval on, and the event must be far enough out.
    const gate = evaluateAutoApprovalGate(getAutoApprovalPolicy(), submission.startDate, todayIso())
    const autoApproved = referral.autoApprove && gate.allowed
    const referralReasons = [...referral.reasons, ...(gate.allowed ? [] : [gate.reason!])]
    const status: SepStatus = autoApproved ? "auto-approved" : "submitted"
    const conditions: SepCondition[] = autoApproved
      ? STANDARD_CONDITIONS.map((text, i) => ({ id: `c-${id}-${i}`, text, standard: true }))
      : []
    const record: SepRecord = {
      id,
      eventName: submission.eventName,
      eventType: submission.eventType,
      applicantName: submission.applicantName,
      applicantOrg: submission.organization,
      applicantPhone: "",
      applicantEmail: "",
      eventStart: submission.startDate,
      eventEnd: submission.endDate || submission.startDate,
      startTime: submission.startTime,
      endTime: submission.endTime,
      venueName: submission.venueName,
      address: submission.address,
      city: submission.city,
      jurisdiction: submission.jurisdiction,
      lat: submission.lat,
      lng: submission.lng,
      expectedAttendance: submission.attendance,
      minorsExpected: false,
      serviceType: submission.serviceType,
      status,
      submittedDate: todayIso(),
      conditions,
      history: [
        { date: todayIso(), actor: submission.applicantName, action: "Submitted application" },
        autoApproved
          ? {
              date: todayIso(),
              actor: "System",
              action: "Auto-approved",
              note: "No referral triggers; standard conditions applied automatically.",
            }
          : {
              date: todayIso(),
              actor: "System",
              action: "Referred for review",
              note: referralReasons.join(" "),
            },
      ],
      referralReasons,
      description: submission.description ?? "",
      siteplanProvided: false,
      liquorTypes: submission.liquorTypes,
      estimatedDrinks: submission.estimatedDrinks,
      hasSecurity: submission.hasSecurity,
      securityStaffCount: submission.securityStaffCount,
    }
    this.recordsState.update((prev) => [record, ...prev])
    return id
  }

  /** Applicant-initiated cancellation of a pending permit. */
  readonly cancel = (id: string, actor: string, reason?: string): void => {
    let cancelled: SepRecord | undefined
    let priorStatus: SepStatus | undefined
    this.recordsState.update((prev) =>
      prev.map((r) => {
        if (r.id !== id || r.status === "cancelled") return r
        priorStatus = r.status
        cancelled = {
          ...r,
          status: "cancelled",
          history: [
            ...r.history,
            { date: todayIso(), actor, action: "Cancelled by applicant", note: reason?.trim() || undefined },
          ],
        }
        return cancelled
      }),
    )
    if (!cancelled) return
    recordAudit({
      category: "application",
      action: "sep.cancelled",
      summary: `Cancelled Special Event Permit ${cancelled.eventName} (${cancelled.id})`,
      actor,
      subjectId: cancelled.id,
      meta: { reason: reason?.trim() || null, priorStatus: priorStatus ?? null },
    })

    const detail = `${cancelled.applicantName} cancelled ${cancelled.eventName} (${cancelled.id}), ${cancelled.eventStart} at ${cancelled.venueName}.${
      reason?.trim() ? ` Reason: ${reason.trim()}` : ""
    }`
    // Police only need to hear about permits that were in their hands (referred, assigned or with conditions).
    const policeHadIt =
      priorStatus === "under-review" ||
      priorStatus === "changes-requested" ||
      priorStatus === "submitted" ||
      Boolean(cancelled.assignedOfficer)
    if (policeHadIt) {
      const worked = cancelled.assignedOfficer
        ? ` It was assigned to ${cancelled.assignedOfficer}${
            cancelled.conditions.length ? ` with ${cancelled.conditions.length} condition(s) drafted` : ""
          }; that work is kept on the record.`
        : ""
      notify({
        recipient: "police",
        category: "application",
        title: "Special Event Permit cancelled by applicant",
        body: `${detail}${worked} No further review is needed.`,
        href: `/police/review/${cancelled.id}`,
        subjectId: cancelled.id,
      })
    }
    notify({
      recipient: "lcrb",
      category: "application",
      title: "Special Event Permit cancelled",
      body: detail,
      href: "/lcrb/conversations",
      subjectId: cancelled.id,
    })
  }
}

/** The prototype's useSepStore(): `records` is a signal; methods as in the prototype. */
export function injectSepStore(): SepStoreProvider {
  return inject(SepStoreProvider)
}
