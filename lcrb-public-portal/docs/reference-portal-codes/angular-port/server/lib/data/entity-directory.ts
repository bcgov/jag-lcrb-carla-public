/** Privacy-limited simulated BC Registry and Dynamics directory for the Legal Entity Builder. TODO: replace `DIRECTORY` and `searchDirectory` with real API calls. */

import type { LEEntityType } from "@domain/models/legal-entity"

export type DirectorySource = "bc-registry" | "dynamics"

export const SOURCE_META: Record<DirectorySource, { label: string; short: string }> = {
  "bc-registry": { label: "BC Registry", short: "Registry" },
  dynamics: { label: "LCRB Dynamics", short: "Dynamics" },
}

export interface DirectoryBusinessResult {
  kind: "business"
  id: string
  sources: DirectorySource[]
  legalName: string
  entityType: LEEntityType
  /** Public identifier (incorporation/registration number or BN). */
  registrationNumber: string
  status: "Active" | "Historical" | "In Dissolution"
  jurisdiction: string
  /** Licence numbers LCRB associates with this business (may be empty). */
  licenceNumbers: string[]
}

export interface DirectoryPersonResult {
  kind: "person"
  id: string
  sources: DirectorySource[]
  name: string
  /** Neutral, non-identifying context — never personal data. */
  descriptor: string
}

export type DirectoryResult = DirectoryBusinessResult | DirectoryPersonResult

const BUSINESSES: DirectoryBusinessResult[] = [
  {
    kind: "business",
    id: "biz_avery_holdings",
    sources: ["bc-registry", "dynamics"],
    legalName: "Avery Holdings Ltd.",
    entityType: "private-corporation",
    registrationNumber: "BC0774521",
    status: "Active",
    jurisdiction: "British Columbia",
    licenceNumbers: [],
  },
  {
    kind: "business",
    id: "biz_blake_family_investments",
    sources: ["bc-registry"],
    legalName: "Blake Family Investments Inc.",
    entityType: "private-corporation",
    registrationNumber: "BC0912345",
    status: "Active",
    jurisdiction: "British Columbia",
    licenceNumbers: [],
  },
  {
    kind: "business",
    id: "biz_harbourside",
    sources: ["bc-registry", "dynamics"],
    legalName: "Harbourside Hospitality Group Ltd.",
    entityType: "private-corporation",
    registrationNumber: "BC1102394",
    status: "Active",
    jurisdiction: "British Columbia",
    licenceNumbers: ["441882"],
  },
  {
    kind: "business",
    id: "biz_island_craft",
    sources: ["bc-registry", "dynamics"],
    legalName: "Island Craft Beverages Inc.",
    entityType: "private-corporation",
    registrationNumber: "BC1155870",
    status: "Active",
    jurisdiction: "British Columbia",
    licenceNumbers: ["441883", "441884"],
  },
  {
    kind: "business",
    id: "biz_pacific_coast_spirits",
    sources: ["dynamics"],
    legalName: "Pacific Coast Spirits Ltd.",
    entityType: "private-corporation",
    registrationNumber: "BC1240981",
    status: "Active",
    jurisdiction: "British Columbia",
    licenceNumbers: ["512004"],
  },
  {
    kind: "business",
    id: "biz_mountain_view_cellars",
    sources: ["bc-registry"],
    legalName: "Mountain View Cellars Inc.",
    entityType: "private-corporation",
    registrationNumber: "BC0998217",
    status: "Active",
    jurisdiction: "British Columbia",
    licenceNumbers: [],
  },
  {
    kind: "business",
    id: "biz_blake_family_trust",
    sources: ["bc-registry"],
    legalName: "Blake Family Trust",
    entityType: "trust",
    registrationNumber: "TR-2214880",
    status: "Active",
    jurisdiction: "British Columbia",
    licenceNumbers: [],
  },
  {
    kind: "business",
    id: "biz_westcoast_partners",
    sources: ["bc-registry"],
    legalName: "Westcoast Beverage Partners",
    entityType: "limited-partnership",
    registrationNumber: "LP0551203",
    status: "Active",
    jurisdiction: "British Columbia",
    licenceNumbers: [],
  },
  {
    kind: "business",
    id: "biz_summit_group",
    sources: ["bc-registry", "dynamics"],
    legalName: "Summit Hospitality Group Ltd.",
    entityType: "private-corporation",
    registrationNumber: "BC1330074",
    status: "Active",
    jurisdiction: "British Columbia",
    licenceNumbers: ["512900"],
  },
  {
    kind: "business",
    id: "biz_legacy_pubs",
    sources: ["bc-registry"],
    legalName: "Legacy Pubs Holdings Ltd.",
    entityType: "private-corporation",
    registrationNumber: "BC0631190",
    status: "Historical",
    jurisdiction: "British Columbia",
    licenceNumbers: [],
  },
]

const PEOPLE: DirectoryPersonResult[] = [
  { kind: "person", id: "per_jordan_avery", sources: ["bc-registry", "dynamics"], name: "Jordan Avery", descriptor: "Named on registry filings" },
  { kind: "person", id: "per_morgan_blake", sources: ["bc-registry", "dynamics"], name: "Morgan Blake", descriptor: "Named on registry filings" },
  { kind: "person", id: "per_casey_blake", sources: ["bc-registry"], name: "Casey Blake", descriptor: "Named on registry filings" },
  { kind: "person", id: "per_priya_desai", sources: ["dynamics"], name: "Priya Desai", descriptor: "On LCRB record" },
  { kind: "person", id: "per_alex_ng", sources: ["bc-registry"], name: "Alex Ng", descriptor: "Named on registry filings" },
  { kind: "person", id: "per_sam_rivera", sources: ["bc-registry", "dynamics"], name: "Sam Rivera", descriptor: "Named on registry filings" },
  { kind: "person", id: "per_taylor_quinn", sources: ["bc-registry"], name: "Taylor Quinn", descriptor: "Named on registry filings" },
  { kind: "person", id: "per_dana_lee", sources: ["dynamics"], name: "Dana Lee", descriptor: "On LCRB record" },
]

export const DIRECTORY: DirectoryResult[] = [...BUSINESSES, ...PEOPLE]

export type DirectoryScope = "all" | "business" | "person"

export interface DirectorySearchParams {
  query: string
  scope?: DirectoryScope
  source?: DirectorySource | "all"
  limit?: number
}

/** Search by name and, for businesses, registration and licence numbers; returns only the privacy-limited fields. */
export function searchDirectory({
  query,
  scope = "all",
  source = "all",
  limit = 25,
}: DirectorySearchParams): DirectoryResult[] {
  const q = query.trim().toLowerCase()
  let results = DIRECTORY

  if (scope !== "all") {
    results = results.filter((r) => r.kind === scope)
  }
  if (source !== "all") {
    results = results.filter((r) => r.sources.includes(source))
  }
  if (q) {
    results = results.filter((r) => {
      if (r.kind === "business") {
        return (
          r.legalName.toLowerCase().includes(q) ||
          r.registrationNumber.toLowerCase().includes(q) ||
          r.licenceNumbers.some((l) => l.toLowerCase().includes(q))
        )
      }
      return r.name.toLowerCase().includes(q)
    })
  }

  // Businesses first, then people; alphabetical within each group.
  results = [...results].sort((a, b) => {
    if (a.kind !== b.kind) return a.kind === "business" ? -1 : 1
    const an = a.kind === "business" ? a.legalName : a.name
    const bn = b.kind === "business" ? b.legalName : b.name
    return an.localeCompare(bn)
  })

  return results.slice(0, limit)
}
