import { type MyApplication, type MyApplications } from "@domain/models/applications"

/** Partners each submitted application was referred to. */
export const APPLICATION_REFERRALS: Record<string, Array<"local-government" | "lcrb">> = {
  "LP-125998": ["lcrb", "local-government"],
  "CM-125876": ["lcrb"],
}

export function referralsFor(id: string): Array<"local-government" | "lcrb"> {
  return APPLICATION_REFERRALS[id] ?? ["lcrb"]
}

export const MY_APPLICATIONS: MyApplications = {
  inProgress: [
    {
      id: "FP-126117",
      type: "Food Primary Licence Application",
      reference: "FP - LCRBTrainingBusiness5",
      status: "not_submitted",
      lastModified: "2026-05-20",
      completionPercent: 45,
      sections: [
        { name: "Business & applicant details", status: "complete" },
        { name: "Establishment information", status: "complete" },
        { name: "Service area & occupancy", status: "in-progress" },
        { name: "Supporting documents", status: "not-started" },
        { name: "Review & declaration", status: "not-started" },
      ],
      captured: [
        { label: "Establishment name", value: "LCRB Training Business 5" },
        { label: "Establishment type", value: "Restaurant" },
        { label: "Service area", value: "Indoor dining — 42 seats" },
      ],
    },
    {
      id: "CRS-126141",
      type: "Cannabis Retail Store",
      reference: "CRS001 - LCRBTrainingBusiness5",
      status: "not_submitted",
      lastModified: "2026-05-18",
      completionPercent: 30,
      sections: [
        { name: "Business & applicant details", status: "complete" },
        { name: "Store location", status: "in-progress" },
        { name: "Local government context", status: "not-started" },
        { name: "Security & floor plan", status: "not-started" },
        { name: "Review & declaration", status: "not-started" },
      ],
      captured: [
        { label: "Proposed store name", value: "CRS001" },
        { label: "Location", value: "Not yet confirmed" },
      ],
    },
    {
      id: "AGT-126082",
      type: "Agent Licence Application",
      reference: "AGT - LCRBTrainingBusiness5",
      status: "not_submitted",
      lastModified: "2026-05-15",
      completionPercent: 60,
      sections: [
        { name: "Applicant details", status: "complete" },
        { name: "Represented manufacturers", status: "complete" },
        { name: "Agent responsibilities", status: "complete" },
        { name: "Supporting documents", status: "in-progress" },
        { name: "Review & declaration", status: "not-started" },
      ],
      captured: [
        { label: "Agent name", value: "LCRB Training Business 5" },
        { label: "Represented manufacturers", value: "3 listed" },
      ],
    },
    {
      id: "UBUV-126080",
      type: "UBrew and UVin Application",
      reference: "UBUV - LCRBTrainingBusiness5 - 126080",
      status: "not_submitted",
      lastModified: "2026-05-12",
      completionPercent: 20,
      sections: [
        { name: "Business & applicant details", status: "complete" },
        { name: "Premises information", status: "in-progress" },
        { name: "Equipment & capacity", status: "not-started" },
        { name: "Supporting documents", status: "not-started" },
        { name: "Review & declaration", status: "not-started" },
      ],
      captured: [{ label: "Business name", value: "LCRB Training Business 5" }],
    },
  ],
  submitted: [
    {
      id: "LP-125998",
      type: "Liquor Primary Licence",
      reference: "LP - LCRBTrainingBusiness5",
      status: "under_review",
      submittedDate: "2026-04-28",
      expectedCompletion: "2026-06-15",
    },
    {
      id: "CM-125876",
      type: "Cannabis Marketing Licence",
      reference: "CM - LCRBTrainingBusiness5",
      status: "pending_documents",
      submittedDate: "2026-04-15",
      expectedCompletion: "2026-05-30",
    },
  ],
  completed: [
    {
      id: "CAT-125654",
      type: "Catering Licence",
      reference: "CAT - LCRBTrainingBusiness5",
      status: "approved",
      completedDate: "2026-03-20",
    },
    {
      id: "FP-125432",
      type: "Food Primary Licence",
      reference: "FP - LCRBTrainingBusiness5",
      status: "approved",
      completedDate: "2026-02-15",
    },
  ],
}

/** Every application across all three stages, flattened. */
export function getAllMyApplications(): (MyApplication & {
  stage: "in_progress" | "submitted" | "completed"
})[] {
  return [
    ...MY_APPLICATIONS.inProgress.map((a) => ({ ...a, stage: "in_progress" as const })),
    ...MY_APPLICATIONS.submitted.map((a) => ({ ...a, stage: "submitted" as const })),
    ...MY_APPLICATIONS.completed.map((a) => ({ ...a, stage: "completed" as const })),
  ]
}
