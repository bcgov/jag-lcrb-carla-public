import type { LERelationshipType } from "@domain/models/legal-entity"
import { type TiedHouseType, type TiedHouseSource, type TiedHouseDomain, type TiedHouseChangeType, type Option, type RequiredDoc } from "@domain/models/tied-house"

export const SOURCE_META: Record<TiedHouseSource, { label: string; context: string }> = {
  "le-review": {
    label: "Legal Entity Review",
    context:
      "A Legal Entity Review of a permanent change requires you to declare the tied house connections identified in your review outcome.",
  },
  voluntary: {
    label: "Voluntary update",
    context:
      "You are updating your tied house declarations outside of another application. Add, change, or remove declarations as your associations change.",
  },
  "new-application": {
    label: "New application",
    context: "Declare any tied house connections as part of your new licence application.",
  },
}

/** Individual: "Relationship to Declared Licence" (immediate family + interests). */
export const INDIVIDUAL_RELATIONSHIPS: Option[] = [
  { value: "spouse", label: "Spouse" },
  { value: "parent", label: "Parent" },
  { value: "sibling", label: "Sibling" },
  { value: "child", label: "Child" },
  { value: "financial-interest", label: "Financial interest" },
  { value: "third-party-operator", label: "Third party operator" },
  { value: "other", label: "Other (describe)" },
]

/** Legal entity "Relationship Type"; maps onto the Legal Entity builder's `LERelationshipType` / role enums. */
export const LEGAL_ENTITY_RELATIONSHIPS: { value: LERelationshipType | "other"; label: string }[] = [
  { value: "shareholding", label: "Shareholder" },
  { value: "director", label: "Director" },
  { value: "partnership", label: "Partner" },
  { value: "trustee", label: "Trustee" },
  { value: "business-agreement", label: "Business agreement" },
  { value: "connection", label: "Other connection" },
  { value: "other", label: "Other (describe)" },
]

/** Business types for the legal-entity variant. */
export const BUSINESS_TYPES: Option[] = [
  { value: "private-corporation", label: "Private Corporation" },
  { value: "public-corporation", label: "Public Corporation" },
  { value: "partnership", label: "Partnership" },
  { value: "limited-partnership", label: "Limited Partnership" },
  { value: "trust", label: "Trust" },
  { value: "society", label: "Society" },
  { value: "sole-proprietor", label: "Sole Proprietor" },
]

export function relationshipLabel(type: TiedHouseType, value: string): string {
  const set = type === "individual" ? INDIVIDUAL_RELATIONSHIPS : LEGAL_ENTITY_RELATIONSHIPS
  return set.find((o) => o.value === value)?.label ?? value
}

export function businessTypeLabel(value: string): string {
  return BUSINESS_TYPES.find((o) => o.value === value)?.label ?? value
}

/** Liquor vs cannabis stream for a licence type. */
export function domainForLicenceType(type: string): TiedHouseDomain {
  return /cannabis/i.test(type) ? "cannabis" : "liquor"
}

/** Documents required when an association is added (liquor: PHS and criminal record search; cannabis: security screening). */
export function requiredDocsFor(type: TiedHouseType, domain: TiedHouseDomain): RequiredDoc[] {
  if (domain === "cannabis") {
    return [
      {
        id: "financial-integrity",
        label: "Financial Integrity disclosure",
        description: "Source-of-funds and financial integrity documentation for the cannabis associate.",
      },
      {
        id: "security-screening",
        label: "Security Screening form",
        description: "Security screening submission required for new cannabis associates.",
      },
    ]
  }
  if (type === "individual") {
    return [
      {
        id: "personal-history-summary",
        label: "Personal History Summary",
        description: "Personal History Summary form for the newly declared individual.",
      },
      {
        id: "criminal-record-search",
        label: "Criminal Record Search consent",
        description: "Consent authorizing a criminal record search for the individual.",
      },
    ]
  }
  // New legal-entity liquor associate: no personal screening docs by default.
  return []
}


/** Per-impacted-licence rate by change type. Removal carries no fee. */
export const FEE_RATE: Record<TiedHouseChangeType, number> = {
  "add-individual": 220,
  "add-legal-entity": 220,
  "change-details": 110,
  remove: 0,
}

export const CHANGE_TYPE_LABEL: Record<TiedHouseChangeType, string> = {
  "add-individual": "Add tied house declaration (individual)",
  "add-legal-entity": "Add tied house declaration (legal entity)",
  "change-details": "Change tied house declaration details",
  remove: "Remove tied house declaration",
}

export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat("en-CA", { style: "currency", currency: "CAD" }).format(amount)
}

export function formatDate(iso: string): string {
  try {
    return new Date(iso).toLocaleDateString("en-CA", { year: "numeric", month: "long", day: "numeric" })
  } catch {
    return iso
  }
}
