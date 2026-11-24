import { type ComplianceRecord, type EnforcementAction } from "@domain/models/compliance"
"use client"

/** Compliance & Enforcement data (prototype). */

/** Days from today (negative = past) as an ISO date string. */
function isoFromNow(days: number): string {
  const d = new Date()
  d.setDate(d.getDate() + days)
  return d.toISOString()
}

export const COMPLIANCE_HISTORY: ComplianceRecord[] = [
  {
    id: "CMP-2026-0412",
    date: isoFromNow(-38),
    licenceId: "326739",
    establishmentName: "OneStop Test 1",
    licenceType: "Liquor Primary",
    kind: "inspection",
    description:
      "Routine compliance inspection during evening service. Occupancy, service practices, and signage reviewed.",
    outcome: "no-contravention",
    officer: "Insp. D. Marsh",
    status: "closed",
  },
  {
    id: "CMP-2025-0987",
    date: isoFromNow(-146),
    licenceId: "326739",
    establishmentName: "OneStop Test 1",
    licenceType: "Liquor Primary",
    kind: "contravention",
    section: "s.75 LCLA",
    description:
      "Liquor served to a patron after authorized hours of liquor service. First contravention of this type.",
    outcome: "monetary-penalty",
    penaltyAmount: 5000,
    officer: "Insp. D. Marsh",
    status: "closed",
  },
  {
    id: "CMP-2025-0631",
    date: isoFromNow(-233),
    licenceId: "326741",
    establishmentName: "Green Leaf Victoria",
    licenceType: "Cannabis Retail Store",
    kind: "warning",
    section: "s.72 CCLA",
    description:
      "Product display visible from outside the store contrary to promotion rules. Corrected on site; written warning issued.",
    outcome: "warning",
    officer: "Insp. R. Okafor",
    status: "closed",
  },
  {
    id: "CMP-2024-1502",
    date: isoFromNow(-402),
    licenceId: "326739",
    establishmentName: "OneStop Test 1",
    licenceType: "Liquor Primary",
    kind: "contravention",
    section: "s.77 LCLA",
    description:
      "Permitted intoxicated patron to remain in the service area. Contravention established following inspection.",
    outcome: "suspension",
    suspensionDays: 4,
    officer: "Insp. D. Marsh",
    status: "closed",
  },
  {
    id: "CMP-2024-0774",
    date: isoFromNow(-503),
    licenceId: "326740",
    establishmentName: "Restaurant ABC",
    licenceType: "Food Primary",
    kind: "inspection",
    description: "Follow-up inspection after a change to the approved floor plan. No issues identified.",
    outcome: "no-contravention",
    officer: "Insp. R. Okafor",
    status: "closed",
  },
]

export const ENFORCEMENT_ACTIONS: EnforcementAction[] = [
  {
    id: "EA-2026-00142",
    referenceNumber: "EA-2026-00142",
    licenceId: "326739",
    establishmentName: "OneStop Test 1",
    licenceType: "Liquor Primary",
    issuedDate: isoFromNow(-9),
    section: "s.76 LCLA",
    allegation: "Sold or supplied liquor to a minor",
    details:
      "During a compliance inspection with a minor agent, a server supplied liquor to a person under 19 years of age without requesting identification. This is alleged to contravene section 76 of the Liquor Control and Licensing Act.",
    proposedAction: "Monetary penalty of $7,000, or a 10-day suspension of the licence",
    responseDeadline: isoFromNow(12),
    baseStatus: "awaiting-response",
  },
  {
    id: "EA-2026-00119",
    referenceNumber: "EA-2026-00119",
    licenceId: "326741",
    establishmentName: "Green Leaf Victoria",
    licenceType: "Cannabis Retail Store",
    issuedDate: isoFromNow(-3),
    section: "s.71 CCLA",
    allegation: "Failure to verify age of a customer",
    details:
      "A compliance inspection observed a sale of cannabis completed without verification of the customer's age. This is alleged to contravene section 71 of the Cannabis Control and Licensing Act.",
    proposedAction: "Monetary penalty of $5,000",
    responseDeadline: isoFromNow(19),
    baseStatus: "awaiting-response",
  },
]
