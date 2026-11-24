import { type SubmittedApplication } from "@domain/models/applications"


const SUBMITTED_APPLICATIONS: Record<string, SubmittedApplication> = {
  "LP-125998": {
    id: "LP-125998",
    type: "Liquor Primary Licence",
    reference: "LP - LCRBTrainingBusiness5",
    status: "under_review",
    applicant: "LCRB Training Business 5 Ltd.",
    submittedDate: "2026-04-28",
    expectedCompletion: "2026-06-15",
    confirmationNumber: "PAY-2026-9F3A11",
    currency: "CAD",
    paymentMethod: "Visa ending in 4242",
    fee: [
      { label: "Liquor Primary application fee", amount: 950 },
      { label: "Annual licence fee (prorated)", amount: 480 },
    ],
    summary: [
      {
        heading: "Establishment",
        fields: [
          { label: "Establishment name", value: "The Cedar Room" },
          { label: "Establishment type", value: "Restaurant / Lounge" },
          { label: "Service area", value: "Indoor seating + patio" },
          { label: "Maximum occupancy", value: "120 persons" },
        ],
      },
      {
        heading: "Location",
        fields: [
          { label: "Address", value: "1450 Government St, Victoria, BC" },
          { label: "Local government", value: "City of Victoria" },
          { label: "Zoning confirmed", value: "Yes" },
        ],
      },
      {
        heading: "Hours of liquor service",
        fields: [
          { label: "Monday – Thursday", value: "11:00 AM – 12:00 AM" },
          { label: "Friday – Saturday", value: "11:00 AM – 1:00 AM" },
          { label: "Sunday", value: "11:00 AM – 11:00 PM" },
        ],
      },
    ],
    timeline: [
      { label: "Submitted", date: "2026-04-28", state: "done" },
      { label: "Payment received", date: "2026-04-28", state: "done" },
      { label: "Under review", date: "2026-05-02", state: "current" },
      { label: "Decision", date: "Expected 2026-06-15", state: "upcoming" },
    ],
    requiredDocuments: [],
  },
  "CM-125876": {
    id: "CM-125876",
    type: "Cannabis Marketing Licence",
    reference: "CM - LCRBTrainingBusiness5",
    status: "pending_documents",
    applicant: "LCRB Training Business 5 Ltd.",
    submittedDate: "2026-04-15",
    expectedCompletion: "2026-05-30",
    confirmationNumber: "PAY-2026-2B77C4",
    currency: "CAD",
    paymentMethod: "Mastercard ending in 8810",
    reviewNote:
      "Your application is on hold pending additional information. Please upload the documents listed below so we can continue our review. Review will resume once all required documents are received.",
    fee: [{ label: "Cannabis Marketing application fee", amount: 7500 }],
    summary: [
      {
        heading: "Business",
        fields: [
          { label: "Legal business name", value: "LCRB Training Business 5 Ltd." },
          { label: "Business number", value: "10001 5555 BC0001" },
          { label: "Marketing category", value: "Cannabis accessories + promotion" },
        ],
      },
      {
        heading: "Primary contact",
        fields: [
          { label: "Name", value: "Dana Lee" },
          { label: "Email", value: "dana.lee@example.com" },
          { label: "Phone", value: "250-555-0182" },
        ],
      },
    ],
    timeline: [
      { label: "Submitted", date: "2026-04-15", state: "done" },
      { label: "Payment received", date: "2026-04-15", state: "done" },
      { label: "Information requested", date: "2026-05-06", state: "current" },
      { label: "Decision", date: "Expected 2026-05-30", state: "upcoming" },
    ],
    requiredDocuments: [
      {
        id: "floor-plan",
        name: "Signed floor plan",
        reason:
          "The floor plan submitted with your application was unsigned. Please upload a copy signed and dated by the business owner.",
        required: true,
      },
      {
        id: "financial-integrity",
        name: "Financial integrity declaration",
        reason:
          "We need a completed financial integrity declaration for the numbered company listed as a shareholder.",
        required: true,
      },
      {
        id: "lease-agreement",
        name: "Lease agreement (pages 4–6)",
        reason:
          "Pages 4 to 6 were missing from the lease agreement you uploaded. Please provide the complete document.",
        required: true,
      },
    ],
  },
}

// Completed (approved) applications.
const COMPLETED_APPLICATIONS: Record<string, SubmittedApplication> = {
  "CAT-125654": {
    id: "CAT-125654",
    type: "Catering Licence",
    reference: "CAT - LCRBTrainingBusiness5",
    status: "approved",
    applicant: "LCRB Training Business 5 Ltd.",
    submittedDate: "2026-02-10",
    expectedCompletion: "2026-03-20",
    confirmationNumber: "PAY-2026-4C81D2",
    currency: "CAD",
    paymentMethod: "Visa ending in 4242",
    fee: [{ label: "Catering application fee", amount: 220 }],
    summary: [
      {
        heading: "Business",
        fields: [
          { label: "Legal business name", value: "LCRB Training Business 5 Ltd." },
          { label: "Licence category", value: "Catering" },
          { label: "Service area", value: "Off-site event catering" },
        ],
      },
      {
        heading: "Primary contact",
        fields: [
          { label: "Name", value: "Dana Lee" },
          { label: "Email", value: "dana.lee@example.com" },
          { label: "Phone", value: "250-555-0182" },
        ],
      },
    ],
    timeline: [
      { label: "Submitted", date: "2026-02-10", state: "done" },
      { label: "Payment received", date: "2026-02-10", state: "done" },
      { label: "Under review", date: "2026-02-18", state: "done" },
      { label: "Approved", date: "2026-03-20", state: "done" },
    ],
    requiredDocuments: [],
  },
  "FP-125432": {
    id: "FP-125432",
    type: "Food Primary Licence",
    reference: "FP - LCRBTrainingBusiness5",
    status: "approved",
    applicant: "LCRB Training Business 5 Ltd.",
    submittedDate: "2026-01-05",
    expectedCompletion: "2026-02-15",
    confirmationNumber: "PAY-2026-1A55E9",
    currency: "CAD",
    paymentMethod: "Mastercard ending in 8810",
    fee: [
      { label: "Food Primary application fee", amount: 475 },
      { label: "Annual licence fee (prorated)", amount: 240 },
    ],
    summary: [
      {
        heading: "Establishment",
        fields: [
          { label: "Establishment name", value: "Downtown Bistro" },
          { label: "Establishment type", value: "Restaurant" },
          { label: "Maximum occupancy", value: "80 persons" },
        ],
      },
      {
        heading: "Location",
        fields: [
          { label: "Address", value: "820 Fort St, Victoria, BC" },
          { label: "Local government", value: "City of Victoria" },
        ],
      },
    ],
    timeline: [
      { label: "Submitted", date: "2026-01-05", state: "done" },
      { label: "Payment received", date: "2026-01-05", state: "done" },
      { label: "Under review", date: "2026-01-14", state: "done" },
      { label: "Approved", date: "2026-02-15", state: "done" },
    ],
    requiredDocuments: [],
  },
}

/** Every seeded submitted application (under review and completed), newest first. */
export function listSubmittedApplications(): SubmittedApplication[] {
  return [...Object.values(SUBMITTED_APPLICATIONS), ...Object.values(COMPLETED_APPLICATIONS)].sort((a, b) =>
    b.submittedDate.localeCompare(a.submittedDate),
  )
}

export function getSubmittedApplication(id: string): SubmittedApplication | undefined {
  return SUBMITTED_APPLICATIONS[id] ?? COMPLETED_APPLICATIONS[id]
}
