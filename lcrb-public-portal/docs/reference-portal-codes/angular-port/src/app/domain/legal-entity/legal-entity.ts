import { type LEEntityType, type LERole, type LERelationshipType, type EntityMeta } from "@domain/models/legal-entity"

export const ENTITY_META: Record<LEEntityType, EntityMeta> = {
  applicant: { label: "Applicant Legal Entity", short: "Applicant", color: "#2E7D57", hasPeople: true },
  "private-corporation": { label: "Private Corporation", short: "Private Corp", color: "#38598A", hasPeople: true },
  "public-corporation": { label: "Public Corporation", short: "Public Corp", color: "#1F6FA8", hasPeople: true },
  partnership: { label: "Partnership", short: "Partnership", color: "#0F766E", hasPeople: true },
  "limited-partnership": { label: "Limited Partnership", short: "LP", color: "#0D9488", hasPeople: true },
  trust: { label: "Trust", short: "Trust", color: "#B45309", hasPeople: true },
  society: { label: "Society", short: "Society", color: "#7C5AA6", hasPeople: true },
  "sole-proprietor": { label: "Sole Proprietor", short: "Sole Prop", color: "#475569", hasPeople: false },
  individual: { label: "Individual", short: "Individual", color: "#64748B", hasPeople: false },
}

export const ROLE_META: Record<LERole, string> = {
  director: "Director",
  shareholder: "Shareholder",
  "key-personnel": "Key Personnel",
  partner: "Partner",
  trustee: "Trustee",
  beneficiary: "Beneficiary",
  officer: "Officer",
}

export const RELATIONSHIP_META: Record<LERelationshipType, { label: string; color: string }> = {
  shareholding: { label: "Shareholding", color: "#38598A" },
  partnership: { label: "Partnership", color: "#0F766E" },
  trustee: { label: "Trustee", color: "#B45309" },
  director: { label: "Director", color: "#2E7D57" },
  "key-personnel": { label: "Key Personnel", color: "#64748B" },
  family: { label: "Family Connection", color: "#9A3412" },
  "business-agreement": { label: "Business Agreement", color: "#1F6FA8" },
  dba: { label: "Doing Business As", color: "#7C5AA6" },
  connection: { label: "Connection to other LE", color: "#475569" },
}

/** Entity types that a person-style (leaf) card represents. */
export const PERSON_TYPES: LEEntityType[] = ["individual", "sole-proprietor"]

export function isPersonType(t: LEEntityType): boolean {
  return PERSON_TYPES.includes(t)
}
