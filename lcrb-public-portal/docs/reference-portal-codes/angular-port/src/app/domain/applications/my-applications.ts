import { type MyApplicationStatus } from "@domain/models/applications"

/** Human-readable labels for each application status. */
export const MY_APPLICATION_STATUS_LABELS: Record<MyApplicationStatus, string> = {
  not_submitted: "Not Submitted",
  under_review: "Under Review",
  pending_documents: "Pending Documents",
  approved: "Approved",
  rejected: "Rejected",
  cancelled: "Cancelled",
}

/** Where each application opens or resumes, keyed by job number. */
export const APP_HREFS: Record<string, string> = {
  "FP-126117": "/applications/food-primary",
  "CRS-126141": "/applications/new/cannabis-retail-store-application",
  "AGT-126082": "/applications/new?type=agent",
  "UBUV-126080": "/applications/new?type=ubrew-uvin",
  "LP-125998": "/applications/new?type=liquor-primary",
  "CM-125876": "/applications/new",
  "CAT-125654": "/applications/new?type=catering",
  "FP-125432": "/applications/food-primary",
}

/** Resolve the resume/open href for an application job number. */
export function hrefForApplication(id: string): string {
  return APP_HREFS[id] ?? "/applications/new"
}
