/** Establishment feasibility engine (deterministic, pure). */

import { APPLICATION_CATALOG, getCatalogEntry, getEntriesByType } from "./mock/applications"
import { type CatalogEntry, type LicenceType } from "@domain/models/applications"
import type { Licence } from "@domain/models/licences"
import { computeLicenceCap, type LicenceCapResult } from "../licences/licence-cap-engine"
import { evaluateSection119 } from "@domain/compliance/section-119-evaluator"
import type { Section119Result } from "@domain/models/section-119"
import { type EstablishmentDomain, type EstablishmentTypeDef, type EstablishmentTypeId, type AddOnId } from "@domain/models/applications"
import { ADD_ONS, getEstablishmentType } from "@domain/applications/establishment-feasibility"


export interface PlannerAnswers {
  establishmentType?: EstablishmentTypeId
  applicantName?: string
  establishmentName?: string
  city?: string
  address?: string
  /** Optional add-ons the applicant is interested in. */
  addOns?: AddOnId[]
  /** Existing BC liquor licences the applicant/related parties already hold. */
  existingLiquorLicences?: number
  /** Existing BC cannabis licences the applicant/related parties already hold. */
  existingCannabisLicences?: number
  /** First Nations affiliation (drives Section 119 for cannabis). */
  firstNationAffiliated?: boolean
  firstNationName?: string
  /** Whether the premises sit on the nation's lands (cannabis + s.119). */
  onFirstNationLands?: boolean
}


/** Parse a catalog fee string like "$7,500.00" into integer cents. */
export function parseFeeToCents(fee: string | undefined): number {
  if (!fee) return 0
  const digits = fee.replace(/[^0-9.]/g, "")
  if (!digits) return 0
  return Math.round(Number.parseFloat(digits) * 100)
}

/** Format integer cents into a "$1,100.00" display string. */
export function formatCents(cents: number): string {
  return new Intl.NumberFormat("en-CA", { style: "currency", currency: "CAD" }).format(cents / 100)
}


export type FlagTone = "positive" | "caution" | "danger" | "info"

export interface FeasibilityFlag {
  tone: FlagTone
  title: string
  detail: string
}

export interface RecommendedApplication {
  /** Catalog slug when this maps to a real flow; undefined for the estimated primary. */
  slug?: string
  title: string
  feeCents: number
  feeDisplay: string
  kind: "primary" | "add-on"
  feeIsEstimate: boolean
}

export type ChecklistCategory = "business" | "location" | "premises" | "people" | "compliance"

export interface ChecklistItem {
  id: string
  label: string
  detail: string
  category: ChecklistCategory
  required: boolean
}

export interface FeasibilitySummary {
  type: EstablishmentTypeDef
  domain: EstablishmentDomain
  applications: RecommendedApplication[]
  feeSubtotalCents: number
  feeSubtotalDisplay: string
  timeline: { minWeeks: number; maxWeeks: number; label: string }
  cap: LicenceCapResult
  section119?: Section119Result
  flags: FeasibilityFlag[]
  checklist: ChecklistItem[]
}

/** Build a minimal but valid Licence record for the cap engine. */
function syntheticLicence(domain: EstablishmentDomain, index: number, name: string): Licence {
  const type = domain === "cannabis" ? "Cannabis Retail Store" : "Liquor Primary"
  return {
    id: `planner-${domain}-${index}`,
    type,
    name,
    status: "Active",
    expiryDate: "—",
    address: "—",
    phone: "—",
    email: "—",
    operatedBy: name,
    licencedTo: name,
    flags: [],
    category: "owned",
  }
}

/** Resolve the catalog entry for an add-on within a licence type, if present. */
function findAddOnEntry(licenceType: LicenceType, addOn: AddOnId): CatalogEntry | undefined {
  const def = ADD_ONS[addOn]
  const inType = getEntriesByType(licenceType).find((e) => def.match.test(e.title))
  if (inType) return inType
  // Fall back to any catalog entry that matches (some add-ons live under a shared licence type in the prototype catalog).
  return APPLICATION_CATALOG.find((e) => e.licenceType === licenceType && def.match.test(e.title))
}

function timelineLabel(minWeeks: number, maxWeeks: number): string {
  if (maxWeeks >= 20) {
    const minM = Math.round(minWeeks / 4)
    const maxM = Math.round(maxWeeks / 4)
    return `${minM}–${maxM} months`
  }
  return `${minWeeks}–${maxWeeks} weeks`
}

/** Produce the full provisional feasibility summary for a set of answers. */
export function computeFeasibility(answers: PlannerAnswers): FeasibilitySummary | null {
  const type = getEstablishmentType(answers.establishmentType)
  if (!type) return null

  const domain = type.domain
  const applicantName = answers.applicantName?.trim() || "Your business"
  const establishmentName = answers.establishmentName?.trim() || applicantName

  /* Applications: primary + selected add-ons. */
  const applications: RecommendedApplication[] = []
  const primaryEntry = type.primarySlug ? getCatalogEntry(type.primarySlug) : undefined
  const primaryFeeCents = primaryEntry?.fee ? parseFeeToCents(primaryEntry.fee) : parseFeeToCents(type.baseFee)
  applications.push({
    slug: primaryEntry?.slug,
    title: primaryEntry?.title ?? `New ${labelForLicenceType(type.licenceType)} licence`,
    feeCents: primaryFeeCents,
    feeDisplay: formatCents(primaryFeeCents),
    kind: "primary",
    feeIsEstimate: !primaryEntry?.fee,
  })

  const selectedAddOns = (answers.addOns ?? []).filter((a) => type.relevantAddOns.includes(a))
  for (const addOn of selectedAddOns) {
    const entry = findAddOnEntry(type.licenceType, addOn)
    if (!entry) continue
    const feeCents = parseFeeToCents(entry.fee)
    applications.push({
      slug: entry.slug,
      title: entry.title,
      feeCents,
      feeDisplay: feeCents ? formatCents(feeCents) : "No fee",
      kind: "add-on",
      feeIsEstimate: false,
    })
  }

  const feeSubtotalCents = applications.reduce((sum, a) => sum + a.feeCents, 0)

  /* Licence cap - count existing holdings + the proposed new licence. */
  const owned: Licence[] = []
  const liquorCount = Math.max(0, answers.existingLiquorLicences ?? 0)
  const cannabisCount = Math.max(0, answers.existingCannabisLicences ?? 0)
  for (let i = 0; i < liquorCount; i++) owned.push(syntheticLicence("liquor", i, `${applicantName} (existing liquor ${i + 1})`))
  for (let i = 0; i < cannabisCount; i++)
    owned.push(syntheticLicence("cannabis", i, `${applicantName} (existing cannabis ${i + 1})`))
  // The licence being planned also counts toward the cap.
  owned.push(syntheticLicence(domain, 999, establishmentName))
  const cap = computeLicenceCap({ applicantName, ownedLicences: owned })

  /* Section 119 - cannabis only. */
  let section119: Section119Result | undefined
  if (domain === "cannabis") {
    section119 = evaluateSection119({
      involvesCannabis: true,
      selfDeclaredIndigenousNation: answers.firstNationAffiliated,
      firstNation: answers.firstNationAffiliated
        ? {
            affiliated: true,
            nationName: answers.firstNationName,
            agreementStatus: "none",
            premisesOnNationLands: answers.onFirstNationLands,
          }
        : undefined,
    })
  }

  const flags = buildFlags(type, cap, section119)
  const checklist = buildChecklist(type, answers)

  return {
    type,
    domain,
    applications,
    feeSubtotalCents,
    feeSubtotalDisplay: formatCents(feeSubtotalCents),
    timeline: { minWeeks: type.minWeeks, maxWeeks: type.maxWeeks, label: timelineLabel(type.minWeeks, type.maxWeeks) },
    cap,
    section119,
    flags,
    checklist,
  }
}

function labelForLicenceType(licenceType: LicenceType): string {
  const map: Partial<Record<LicenceType, string>> = {
    "food-primary": "Food Primary",
    "liquor-primary": "Liquor Primary",
    manufacturer: "Manufacturer",
    catering: "Catering",
    "licensee-retail-store": "Licensee Retail Store",
    "cannabis-retail-store": "Cannabis Retail Store",
  }
  return map[licenceType] ?? "licence"
}

function buildFlags(
  type: EstablishmentTypeDef,
  cap: LicenceCapResult,
  section119: Section119Result | undefined,
): FeasibilityFlag[] {
  const flags: FeasibilityFlag[] = []
  const domainResult = type.domain === "cannabis" ? cap.cannabis : cap.liquor

  if (domainResult.status === "none") {
    // liquor licences are not capped.
    const existing = domainResult.total - 1 // minus the one being planned
    flags.push({
      tone: "info",
      title: "No licence cap for liquor",
      detail:
        existing > 0
          ? `Your business group already holds ${existing} liquor licence${existing === 1 ? "" : "s"}. There is no limit on how many you may hold.`
          : "There is no limit on the number of liquor licences a business may hold.",
    })
  } else if (domainResult.status === "over") {
    flags.push({
      tone: "danger",
      title: "Over the licence cap",
      detail: `This would be licence ${domainResult.total} of ${domainResult.cap} allowed ${type.domain} licences for your business group. A cap determination is required before you can proceed.`,
    })
  } else if (domainResult.status === "at") {
    flags.push({
      tone: "caution",
      title: "At the licence cap",
      detail: `This reaches the maximum of ${domainResult.cap} ${type.domain} licences. Staff must confirm before a further licence is possible.`,
    })
  } else if (domainResult.status === "near") {
    flags.push({
      tone: "caution",
      title: "Approaching the licence cap",
      detail: `You would hold ${domainResult.total} of ${domainResult.cap} allowed ${type.domain} licences.`,
    })
  } else {
    flags.push({
      tone: "positive",
      title: "Within the licence cap",
      detail: `You would hold ${domainResult.total} of ${domainResult.cap} allowed ${type.domain} licences.`,
    })
  }

  if (section119) {
    const tone: FlagTone =
      section119.status === "applies" || section119.status === "likely-applies"
        ? "info"
        : section119.status === "needs-review"
          ? "caution"
          : "info"
    flags.push({
      tone,
      title: "Section 119 (First Nations)",
      detail: section119.rationale,
    })
  }

  if (type.licenceType === "manufacturer") {
    flags.push({
      tone: "info",
      title: "Tied-house rules apply",
      detail:
        "As a manufacturer, tied-house rules restrict financial and operational associations with hospitality and retail licences. Declare any associations early.",
    })
  }

  return flags
}

function buildChecklist(type: EstablishmentTypeDef, answers: PlannerAnswers): ChecklistItem[] {
  const items: ChecklistItem[] = [
    {
      id: "corp-registry",
      label: "BC business registration",
      detail: "Corporate registration / business number for the legal entity that will hold the licence.",
      category: "business",
      required: true,
    },
    {
      id: "right-to-occupy",
      label: "Proof of right to occupy",
      detail: "Signed lease, offer to lease, or proof of ownership for the premises.",
      category: "premises",
      required: true,
    },
    {
      id: "floor-plan",
      label: "Floor plan / site plan",
      detail: "Scaled floor plan showing the proposed licensed area and, if applicable, service areas.",
      category: "premises",
      required: true,
    },
    {
      id: "local-gov",
      label: "Local government / First Nation approval",
      detail:
        "Confirm zoning permits your use and obtain the required local government (or First Nation) resolution or comment. This is not checked automatically — contact your municipality early.",
      category: "location",
      required: true,
    },
    {
      id: "signing-id",
      label: "Government-issued photo ID",
      detail: "ID for each individual with signing authority.",
      category: "people",
      required: true,
    },
    {
      id: "screening",
      label: "Security screening (Personal History Summary)",
      detail: "Each director, owner (10%+ interest), and key personnel must complete security screening.",
      category: "people",
      required: true,
    },
  ]

  if (type.domain === "liquor") {
    if (type.licenceType === "food-primary") {
      items.push({
        id: "food-service-plan",
        label: "Food service plan / menu",
        detail: "Evidence that food service is the primary purpose (menu, kitchen capacity).",
        category: "compliance",
        required: true,
      })
    }
    if (type.licenceType === "liquor-primary" || type.id === "nightclub") {
      items.push({
        id: "security-plan",
        label: "Security & occupant load plan",
        detail: "Staffing, capacity, and safety plan appropriate to a liquor-primary venue.",
        category: "compliance",
        required: true,
      })
    }
  }

  if (type.domain === "cannabis") {
    items.push(
      {
        id: "financial-integrity",
        label: "Financial integrity documentation",
        detail: "Source-of-funds and financial integrity records for owners and investors.",
        category: "compliance",
        required: true,
      },
      {
        id: "cannabis-security-plan",
        label: "Store security plan",
        detail: "Cameras, secure storage, alarm, and inventory controls meeting cannabis retail standards.",
        category: "compliance",
        required: true,
      },
      {
        id: "cannabis-worker",
        label: "Cannabis worker qualification",
        detail: "Retail staff must hold a valid cannabis worker qualification (Selling It Right).",
        category: "people",
        required: true,
      },
    )
    if (answers.firstNationAffiliated && answers.onFirstNationLands) {
      items.push({
        id: "s119-agreement",
        label: "Section 119 agreement confirmation",
        detail: "Confirm the status of the First Nation's s.119 agreement covering the premises.",
        category: "compliance",
        required: true,
      })
    }
  }

  return items
}


/** Which interview answers are considered "complete" for readiness. */
export function interviewComplete(answers: PlannerAnswers): boolean {
  return Boolean(answers.establishmentType && answers.applicantName?.trim() && answers.city?.trim())
}

/** Readiness score (0–100): 25% for completing the interview, 65% for checklist progress */
export function computeReadiness(
  answers: PlannerAnswers,
  summary: FeasibilitySummary | null,
  checklistDone: Record<string, boolean>,
): number {
  let score = 0
  if (interviewComplete(answers)) score += 25
  if (summary) {
    const total = summary.checklist.length
    if (total > 0) {
      const done = summary.checklist.filter((i) => checklistDone[i.id]).length
      score += Math.round((done / total) * 65)
    }
    const hasBlocker = summary.flags.some((f) => f.tone === "danger")
    if (!hasBlocker) score += 10
  }
  return Math.min(100, score)
}
