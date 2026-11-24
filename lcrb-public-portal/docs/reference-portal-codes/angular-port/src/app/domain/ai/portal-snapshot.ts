/** Compact serializable summary of the signed-in user's own records, sent with each assistant request; the client-side gatherer is in `gather-snapshot.ts`. */

export interface SnapshotApplication {
  id: string
  type: string
  reference: string
  status: string
  statusLabel: string
  stage: "in_progress" | "submitted" | "completed"
  completionPercent?: number
  lastModified?: string
  submittedDate?: string
  expectedCompletion?: string
  completedDate?: string
  /** Section-by-section progress (in-progress applications). */
  sections?: { name: string; status: string }[]
  /** Key values already captured on the application. */
  captured?: { label: string; value: string }[]
  /** Where to open/resume/view this application. */
  href: string
}

export interface SnapshotLicence {
  id: string
  type: string
  name: string
  status: string
  expiryDate: string
  address: string
  flags: string[]
  href: string
}

export interface SnapshotBusiness {
  legalName: string
  bceidBusinessNumber: string
  liquorLicenceCount: number
  cannabisLicenceCount: number
  firstNation?: string
}

export interface SnapshotSubmission {
  jobNumber: string
  title: string
  submittedAt: string
  href: string
}

export interface SnapshotDraft {
  title: string
  progress: number
  href: string
}

/** A closed inspection / contravention on the establishment's record. */
export interface SnapshotComplianceRecord {
  id: string
  date: string
  establishmentName: string
  licenceType: string
  kind: string
  section?: string
  description: string
  outcomeLabel: string
  penaltyAmount?: number
  suspensionDays?: number
  href: string
}

/** An ACTIVE Notice of Enforcement Action the licensee must respond to. */
export interface SnapshotEnforcementAction {
  referenceNumber: string
  establishmentName: string
  section: string
  allegation: string
  proposedAction: string
  issuedDate: string
  responseDeadline: string
  statusLabel: string
  href: string
}

export interface PortalSnapshot {
  role: string
  business?: SnapshotBusiness
  applications: SnapshotApplication[]
  licences: SnapshotLicence[]
  recentSubmissions: SnapshotSubmission[]
  drafts: SnapshotDraft[]
  /** Closed inspection & contravention history (licensees only). */
  complianceHistory?: SnapshotComplianceRecord[]
  /** Open Notices of Enforcement Action (licensees only). */
  enforcementActions?: SnapshotEnforcementAction[]
}

function formatDate(iso?: string): string {
  if (!iso) return "—"
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return iso
  return d.toLocaleDateString("en-CA", { year: "numeric", month: "short", day: "numeric" })
}

/** Renders the snapshot as a compact text block for the system prompt; empty string when there is nothing useful (e.g. staff roles). */
export function formatPortalSnapshot(snapshot?: PortalSnapshot | null): string {
  if (!snapshot) return ""

  // Defensive: the snapshot arrives over the wire, so any array may be missing or malformed; formatting must never crash the route.
  const applications = Array.isArray(snapshot.applications) ? snapshot.applications : []
  const licences = Array.isArray(snapshot.licences) ? snapshot.licences : []
  const recentSubmissions = Array.isArray(snapshot.recentSubmissions) ? snapshot.recentSubmissions : []
  const drafts = Array.isArray(snapshot.drafts) ? snapshot.drafts : []
  const complianceHistory = Array.isArray(snapshot.complianceHistory) ? snapshot.complianceHistory : []
  const enforcementActions = Array.isArray(snapshot.enforcementActions) ? snapshot.enforcementActions : []

  const hasAnything =
    snapshot.business ||
    applications.length > 0 ||
    licences.length > 0 ||
    recentSubmissions.length > 0 ||
    drafts.length > 0 ||
    complianceHistory.length > 0 ||
    enforcementActions.length > 0
  if (!hasAnything) return ""

  const parts: string[] = []

  if (snapshot.business) {
    const b = snapshot.business
    const fn = b.firstNation ? `; First Nation on file: ${b.firstNation}` : ""
    parts.push(
      `Business: ${b.legalName} (BCeID business #${b.bceidBusinessNumber}). Holds ${b.liquorLicenceCount} liquor and ${b.cannabisLicenceCount} cannabis licence(s)${fn}.`,
    )
  }

  if (applications.length > 0) {
    const lines = applications.map((a) => {
      const bits: string[] = [`${a.type} (${a.id}) — ${a.statusLabel}`]
      if (a.stage === "in_progress") {
        if (typeof a.completionPercent === "number") bits.push(`${a.completionPercent}% complete`)
        if (a.lastModified) bits.push(`last modified ${formatDate(a.lastModified)}`)
        const nextSection = a.sections?.find((s) => s.status !== "complete")
        if (nextSection) bits.push(`next: "${nextSection.name}" (${nextSection.status})`)
      } else if (a.stage === "submitted") {
        if (a.submittedDate) bits.push(`submitted ${formatDate(a.submittedDate)}`)
        if (a.expectedCompletion) bits.push(`expected decision ${formatDate(a.expectedCompletion)}`)
      } else {
        if (a.completedDate) bits.push(`completed ${formatDate(a.completedDate)}`)
      }
      const captured = a.captured?.length
        ? ` Captured so far: ${a.captured.map((c) => `${c.label}: ${c.value}`).join("; ")}.`
        : ""
      return `- ${bits.join("; ")}. Open: ${a.href}.${captured}`
    })
    parts.push(`Applications (${applications.length}):\n${lines.join("\n")}`)
  }

  if (licences.length > 0) {
    const lines = licences.map((l) => {
      const flags = Array.isArray(l.flags) && l.flags.length ? ` Flags: ${l.flags.join(", ")}.` : ""
      return `- ${l.type} "${l.name}" (#${l.id}) — ${l.status}; expires ${formatDate(l.expiryDate)}; ${l.address}. View: ${l.href}.${flags}`
    })
    parts.push(`Licences (${licences.length}):\n${lines.join("\n")}`)
  }

  if (recentSubmissions.length > 0) {
    const lines = recentSubmissions.map(
      (s) => `- ${s.title} (${s.jobNumber}) — submitted ${formatDate(s.submittedAt)}. Receipt: ${s.href}.`,
    )
    parts.push(`Recently submitted in this session (${recentSubmissions.length}):\n${lines.join("\n")}`)
  }

  if (drafts.length > 0) {
    const lines = drafts.map((d) => `- ${d.title} — ${d.progress}% complete. Resume: ${d.href}.`)
    parts.push(`Saved drafts (${drafts.length}):\n${lines.join("\n")}`)
  }

  // Open enforcement first: it carries response deadlines, so it is the most urgent.
  if (enforcementActions.length > 0) {
    const lines = enforcementActions.map(
      (e) =>
        `- ${e.referenceNumber} — ${e.allegation} (${e.section}) at ${e.establishmentName}. ` +
        `Status: ${e.statusLabel}; issued ${formatDate(e.issuedDate)}; RESPONSE DEADLINE ${formatDate(e.responseDeadline)}. ` +
        `Proposed action: ${e.proposedAction}. Open: ${e.href}.`,
    )
    parts.push(
      `OPEN enforcement actions (${enforcementActions.length}) — these need the licensee's attention:\n${lines.join("\n")}`,
    )
  }

  if (complianceHistory.length > 0) {
    const lines = complianceHistory.map((c) => {
      const extra: string[] = []
      if (typeof c.penaltyAmount === "number") extra.push(`$${c.penaltyAmount.toLocaleString("en-CA")} penalty`)
      if (typeof c.suspensionDays === "number") extra.push(`${c.suspensionDays}-day suspension`)
      const sec = c.section ? ` ${c.section};` : ""
      return (
        `- ${formatDate(c.date)} — ${c.kind} at ${c.establishmentName} (${c.licenceType});${sec} ` +
        `outcome: ${c.outcomeLabel}${extra.length ? ` (${extra.join(", ")})` : ""}. ${c.description} View: ${c.href}.`
      )
    })
    parts.push(
      `Compliance history — closed inspections & contraventions (${complianceHistory.length}):\n${lines.join("\n")}`,
    )
  }

  return parts.join("\n\n")
}
