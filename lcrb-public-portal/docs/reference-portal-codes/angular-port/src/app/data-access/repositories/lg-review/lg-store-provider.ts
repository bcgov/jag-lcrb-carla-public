/** Local government referral store. */
import { afterNextRender, computed, effect, inject, Injectable, signal } from "@angular/core"
import { LG_APPLICATIONS } from "./mock/lg-applications"
import { type LgApplication, type LgDecision, type LgHistoryEntry, type LgReviewStatus } from "@domain/models/lg-review"
import { recordAudit } from "../activity/audit-service"
import { notify } from "../notifications/notification-service"
import { injectCancellations } from "../applications/applications.signals"

const DECISION_TO_STATUS: Record<LgDecision, LgReviewStatus> = {
  "no-comment": "no-comment",
  "community-review": "community-review",
  reject: "reject",
  "documents-requested": "documents-requested",
}

const DECISION_LABELS: Record<LgDecision, string> = {
  "no-comment": "Comment submitted: No comment",
  "community-review": "Sent to community review",
  reject: "Comment submitted: Reject",
  "documents-requested": "Documents requested",
}

const STORAGE_KEY = "lcrb.lg.applications"

function cloneSeed(a: LgApplication): LgApplication {
  return { ...a, history: a.history.map((h) => ({ ...h })) }
}

@Injectable({ providedIn: "root" })
export class LgStoreProvider {
  private readonly rawState = signal<LgApplication[]>(LG_APPLICATIONS.map(cloneSeed))
  private readonly hydratedState = signal(false)
  // Applicant-side cancellations arrive through the shared cancellation service, not through any LG action.
  private readonly cancellations = injectCancellations()

  readonly applications = computed<LgApplication[]>(() => {
    const rawApplications = this.rawState()
    const cancellations = this.cancellations()
    if (cancellations.length === 0) return rawApplications
    const byJob = new Map(cancellations.map((c) => [c.id, c]))
    return rawApplications.map((a) => {
      const c = byJob.get(a.jobNumber)
      if (!c || a.status === "cancelled") return a
      return {
        ...a,
        status: "cancelled",
        cancellation: {
          cancelledAt: c.cancelledAt,
          cancelledBy: c.cancelledBy,
          reason: c.reason,
          priorStatus: a.status,
        },
        history: [
          ...a.history,
          {
            label: c.reason ? `Cancelled by applicant — ${c.reason}` : "Cancelled by applicant",
            actor: c.cancelledBy,
            date: c.cancelledAt.slice(0, 10),
          },
        ],
      }
    })
  })

  constructor() {
    // Hydrate from localStorage so recorded comments and simulated referrals survive a page reload or opening a referral in a new tab.
    afterNextRender(() => {
      try {
        const raw = localStorage.getItem(STORAGE_KEY)
        if (raw) {
          const stored = JSON.parse(raw) as LgApplication[]
          // Merge in any seed referrals the stored snapshot is missing (e.g. new fixtures added since the tester first loaded the app)
          const storedJobs = new Set(stored.map((a) => a.jobNumber))
          const missingSeeds = LG_APPLICATIONS.filter((a) => !storedJobs.has(a.jobNumber)).map(cloneSeed)
          this.rawState.set([...stored, ...missingSeeds])
        }
      } catch {
        // ignore malformed storage
      }
      this.hydratedState.set(true)
    })

    effect(() => {
      const rawApplications = this.rawState()
      if (!this.hydratedState()) return
      try {
        // Persist the jurisdiction's own state only; the cancellation overlay is re-derived from its own collection on every read.
        localStorage.setItem(STORAGE_KEY, JSON.stringify(rawApplications))
      } catch {
        // ignore storage write failures
      }
    })
  }

  readonly getByJob = (jobNumber: string): LgApplication | undefined =>
    this.applications().find((a) => a.jobNumber === jobNumber)

  readonly getByJobNumber = this.getByJob

  readonly recordDecision = (jobNumber: string, decision: LgDecision, note: string, actor: string): void => {
    const today = new Date().toISOString().slice(0, 10)
    const entry: LgHistoryEntry = {
      label: note ? `${DECISION_LABELS[decision]} — ${note}` : DECISION_LABELS[decision],
      actor,
      date: today,
    }
    let decided: LgApplication | undefined
    this.rawState.update((prev) =>
      prev.map((a) => {
        if (a.jobNumber !== jobNumber) return a
        // A withdrawn file is closed to new comments; the UI disables the actions, this is the belt to those braces.
        if (a.status === "cancelled") return a
        decided = { ...a, status: DECISION_TO_STATUS[decision], history: [...a.history, entry] }
        return decided
      }),
    )
    if (!decided) return
      // Audit the local-government recommendation so it appears in the trail and flows back to the branch alongside the case.
      recordAudit({
        category: "review",
        action: `lg.${decision}`,
        summary: `${DECISION_LABELS[decision]} — ${decided.businessName} (${jobNumber})`,
        actor,
        actorRole: "local-government",
        subjectId: jobNumber,
        subjectType: "lg-referral",
        meta: note ? { note } : undefined,
      })

      // Notify the licensee that their referral progressed.
      notify({
        recipient: "licensee",
        category: "decision",
        title:
          decision === "documents-requested"
            ? "Local government requested documents"
            : "Local government response received",
        body:
          decision === "documents-requested"
            ? `${decided.businessName}: ${note || "Additional documents have been requested."}`
            : `${decided.businessName} (${jobNumber}): ${DECISION_LABELS[decision]}.${note ? ` ${note}` : ""}`,
        href: "/applications",
        subjectId: jobNumber,
        channels: ["in-portal", "email"],
      })
  }

  readonly requestDocuments = (jobNumber: string, message: string, actor: string): void => {
    this.recordDecision(jobNumber, "documents-requested", message, actor)
  }

  /** PROTOTYPE TESTING HELPER - adds a referral dated today so testers can see how a newly arriving application behaves */
  readonly simulateIncomingReferral = (actor: string): void => {
    const today = new Date()
    const iso = today.toISOString().slice(0, 10)
    const due = new Date(today)
    due.setDate(due.getDate() + 30)

    let created: LgApplication | undefined
    this.rawState.update((prev) => {
      // Keep job numbers unique across reloads, since simulated referrals now persist.
      let seq = prev.filter((a) => a.jobNumber.startsWith("SIM-")).length + 1
      while (prev.some((a) => a.jobNumber === `SIM-${String(seq).padStart(3, "0")}`)) seq += 1
      const next: LgApplication = {
        ...prev[0],
        jobNumber: `SIM-${String(seq).padStart(3, "0")}`,
        establishmentName: `New Referral ${seq} (simulated)`,
        businessName: `Simulated Applicant ${seq} Ltd.`,
        applicantName: "Simulated Applicant",
        status: "pending-review",
        submittedDate: iso,
        commentDueDate: due.toISOString().slice(0, 10),
        history: [{ label: "Referred to local government for comment", actor: "LCRB Intake", date: iso }],
      }
      created = next
      return [next, ...prev]
    })
    if (!created) return
    recordAudit({
      category: "system",
      action: "lg.referral.simulated",
      summary: `Simulated incoming referral ${created.jobNumber} for prototype testing`,
      actor,
      actorRole: "local-government",
      subjectId: created.jobNumber,
      subjectType: "lg-referral",
    })
  }
}

/** The prototype's useLgStore(): `applications` is a signal; methods as in the prototype. */
export function injectLgStore(): LgStoreProvider {
  return inject(LgStoreProvider)
}
