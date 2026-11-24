"use client"

/** Client-side gatherer for the assistant's portal snapshot. */

import { MY_APPLICATIONS } from "../applications/mock/my-applications"
import { MY_APPLICATION_STATUS_LABELS, hrefForApplication } from "@domain/applications/my-applications"
import { type MyApplication } from "@domain/models/applications"
import { MOCK_LICENCES } from "../licences/mock/licences"
import { getBusinessProfile } from "../account/mock/business-profile"
import { getLicenceHoldings } from "@domain/account/business-profile"
import { getAllSubmissions } from "../applications/submissions"
import { getAllDrafts } from "../applications/drafts"
import { COMPLIANCE_HISTORY, ENFORCEMENT_ACTIONS } from "../compliance/mock/enforcement"
import { OUTCOME_META } from "@domain/compliance/enforcement"
import { statusFor } from "../compliance/enforcement-store"
import { STATUS_META } from "@domain/compliance/enforcement"
import type {
  PortalSnapshot,
  SnapshotApplication,
  SnapshotComplianceRecord,
  SnapshotEnforcementAction,
  SnapshotLicence,
} from "@domain/ai/portal-snapshot"

function toSnapshotApplication(
  a: MyApplication,
  stage: SnapshotApplication["stage"],
): SnapshotApplication {
  return {
    id: a.id,
    type: a.type,
    reference: a.reference,
    status: a.status,
    statusLabel: MY_APPLICATION_STATUS_LABELS[a.status] ?? a.status,
    stage,
    completionPercent: "completionPercent" in a ? a.completionPercent : undefined,
    lastModified: "lastModified" in a ? a.lastModified : undefined,
    submittedDate: "submittedDate" in a ? a.submittedDate : undefined,
    expectedCompletion: "expectedCompletion" in a ? a.expectedCompletion : undefined,
    completedDate: "completedDate" in a ? a.completedDate : undefined,
    sections: "sections" in a ? a.sections : undefined,
    captured: "captured" in a ? a.captured : undefined,
    href: hrefForApplication(a.id),
  }
}

/** Build the snapshot of the signed-in user's own records for the assistant. */
export function gatherPortalSnapshot(role: string, organization?: string): PortalSnapshot {
  const empty: PortalSnapshot = {
    role,
    applications: [],
    licences: [],
    recentSubmissions: [],
    drafts: [],
  }

  // Only customers (licensees / applicants) have personal records here.
  const isCustomer = role === "licensee" || role === "applicant"
  if (!isCustomer) return empty

  const profile = getBusinessProfile(organization)
  const holdings = getLicenceHoldings(profile)

  const applications: SnapshotApplication[] = [
    ...MY_APPLICATIONS.inProgress.map((a) => toSnapshotApplication(a, "in_progress")),
    ...MY_APPLICATIONS.submitted.map((a) => toSnapshotApplication(a, "submitted")),
    ...MY_APPLICATIONS.completed.map((a) => toSnapshotApplication(a, "completed")),
  ]

  const licences: SnapshotLicence[] = MOCK_LICENCES.map((l) => ({
    id: l.id,
    type: l.type,
    name: l.name,
    status: l.status,
    expiryDate: l.expiryDate,
    address: l.address,
    flags: l.flags,
    href: `/licences/${l.id}`,
  }))

  // Dynamic records created during this browser session.
  let recentSubmissions: PortalSnapshot["recentSubmissions"] = []
  let drafts: PortalSnapshot["drafts"] = []
  try {
    recentSubmissions = getAllSubmissions()
      .sort((a, b) => (a.submittedAt < b.submittedAt ? 1 : -1))
      .slice(0, 8)
      .map((s) => ({
        jobNumber: s.jobNumber,
        title: s.title,
        submittedAt: s.submittedAt,
        href: `/applications/confirmation/${s.id}`,
      }))
  } catch {
    recentSubmissions = []
  }
  try {
    drafts = getAllDrafts()
      .slice(0, 8)
      .map((d) => ({ title: d.title, progress: d.progress, href: d.resumeHref }))
  } catch {
    drafts = []
  }

  // Compliance & enforcement belong to licence holders.
  let complianceHistory: SnapshotComplianceRecord[] = []
  let enforcementActions: SnapshotEnforcementAction[] = []
  if (role === "licensee") {
    complianceHistory = COMPLIANCE_HISTORY.map((c) => ({
      id: c.id,
      date: c.date,
      establishmentName: c.establishmentName,
      licenceType: c.licenceType,
      kind: c.kind,
      section: c.section,
      description: c.description,
      outcomeLabel: OUTCOME_META[c.outcome]?.label ?? c.outcome,
      penaltyAmount: c.penaltyAmount,
      suspensionDays: c.suspensionDays,
      href: "/compliance",
    }))
    try {
      enforcementActions = ENFORCEMENT_ACTIONS.map((a) => {
        const status = statusFor(a)
        return {
          referenceNumber: a.referenceNumber,
          establishmentName: a.establishmentName,
          section: a.section,
          allegation: a.allegation,
          proposedAction: a.proposedAction,
          issuedDate: a.issuedDate,
          responseDeadline: a.responseDeadline,
          statusLabel: STATUS_META[status]?.label ?? status,
          // Compliance & enforcement is a single hub page; there is no per-action route to deep-link to.
          href: "/compliance",
        }
      })
    } catch {
      enforcementActions = []
    }
  }

  return {
    role,
    complianceHistory,
    enforcementActions,
    business: {
      legalName: profile.legalName,
      bceidBusinessNumber: profile.bceidBusinessNumber,
      liquorLicenceCount: holdings.liquor,
      cannabisLicenceCount: holdings.cannabis,
      firstNation:
        profile.firstNation?.affiliated && profile.firstNation.nationName
          ? profile.firstNation.nationName
          : undefined,
    },
    applications,
    licences,
    recentSubmissions,
    drafts,
  }
}
