/** Licence-prerequisite eligibility and ordering for the application catalog; pure and synchronous so the catalog page and per-slug gate share the rules. */

import { LICENCE_TYPE_LABELS, LICENCE_TYPE_ORDER } from "@domain/models/applications"
import type { CatalogEntry, LicenceType } from "@domain/models/applications"

/** Applications that create a licence rather than modify one; they stay open to everyone, or new applicants would be locked out. */
const NEW_LICENCE_SLUGS = new Set([
  // Applying for a first Food Primary licence must never require already holding one.
  "food-primary",
  "cannabis-retail-store-application",
  "licensee-retail-store-application",
  "rural-licensee-retail-store-licence",
  "manufacturer-application",
  "deemed-manufacturer-authorization",
  "agent-licence",
  "catering-licence",
  "liquor-primary-licence",
  "liquor-primary-club-licence",
  "ubrew-uvin-licence",
])

/** Licence-type label (as stored on a held licence) -> catalog licence type. */
const TYPE_BY_LABEL = new Map<string, LicenceType>(
  LICENCE_TYPE_ORDER.map((type) => [LICENCE_TYPE_LABELS[type].toLowerCase(), type]),
)

/** Resolve a held licence's display label (e.g. "Liquor Primary") to its catalog type. */
export function licenceTypeFromLabel(label: string): LicenceType | null {
  return TYPE_BY_LABEL.get(label.trim().toLowerCase()) ?? null
}

/** Catalog licence types the business holds; `common` entries are always treated as held. */
export function heldLicenceTypes(licences: { type: string }[]): Set<LicenceType> {
  const held = new Set<LicenceType>(["common"])
  for (const licence of licences) {
    const type = licenceTypeFromLabel(licence.type)
    if (type) held.add(type)
  }
  return held
}

/** How many licences of each type the business holds; drives relevance ordering. */
export function heldLicenceCounts(licences: { type: string }[]): Map<LicenceType, number> {
  const counts = new Map<LicenceType, number>()
  for (const licence of licences) {
    const type = licenceTypeFromLabel(licence.type)
    if (type) counts.set(type, (counts.get(type) ?? 0) + 1)
  }
  return counts
}

/** Safety net for later entries: "Apply for a new <something> licence" counts as creating a licence ("licence" is required so a patio change isn't). Fails open on purpose. */
const NEW_LICENCE_COPY = /^apply for a (?:new|liquor)\b[^.]*\blicen[cs]e/i

/** True when this entry creates a new licence and so needs no prerequisite. */
export function isNewLicenceApplication(entry: Pick<CatalogEntry, "slug" | "description">): boolean {
  return NEW_LICENCE_SLUGS.has(entry.slug) || NEW_LICENCE_COPY.test(entry.description ?? "")
}

/** Whether the business may start this application; modifying or endorsing a licence requires holding that licence type. */
export function isEntryEligible(
  entry: Pick<CatalogEntry, "slug" | "licenceType" | "description">,
  held: Set<LicenceType>,
): boolean {
  if (isNewLicenceApplication(entry)) return true
  return held.has(entry.licenceType)
}

/** Plain-language reason an entry is unavailable, for the blocked-state UI. */
export function ineligibilityReason(entry: Pick<CatalogEntry, "licenceType">): string {
  return `Requires an active ${LICENCE_TYPE_LABELS[entry.licenceType]} licence`
}

/** Relevance score for catalog ordering (higher first): applications outrank authorizations, and entries tied to a held licence type outrank generic ones. */
export function relevanceScore(entry: CatalogEntry, counts: Map<LicenceType, number>): number {
  /* Opening a brand-new licence is always allowed but rarely what an existing licensee wants, so these sort below changes to held licences. */
  if (isNewLicenceApplication(entry)) return 20

  // Everything below acts on a licence the business holds.
  let score = 100
  // Applications before authorizations.
  if (entry.kind === "application") score += 30
  // Weight by held licences of this type, capped so many of one type don't bury universally useful items.
  score += Math.min(counts.get(entry.licenceType) ?? 0, 3) * 10
  // Available on every licence, so broadly useful.
  if (entry.licenceType === "common") score += 10
  return score
}

/** A catalog entry deduplicated by title: one card with a variant per applicable licence type. */
export interface CatalogGroup {
  title: string
  description: string
  kind: CatalogEntry["kind"]
  fee?: string
  /** One option per licence type this application is available for. */
  variants: { slug: string; licenceType: LicenceType; label: string }[]
  score: number
}

/** Collapses entries sharing a title into one group, ordered by relevance; per-licence-type flows are kept as `variants`. */
export function groupByTitle(
  entries: CatalogEntry[],
  counts: Map<LicenceType, number>,
  /** Optional per-entry search score; takes precedence over held-licence relevance. */
  searchScore?: (entry: CatalogEntry) => number,
): CatalogGroup[] {
  const groups = new Map<string, CatalogGroup>()
  for (const entry of entries) {
    const existing = groups.get(entry.title)
    const variant = {
      slug: entry.slug,
      licenceType: entry.licenceType,
      label: LICENCE_TYPE_LABELS[entry.licenceType],
    }
    const score = searchScore
      ? searchScore(entry) * 1000 + relevanceScore(entry, counts)
      : relevanceScore(entry, counts)
    if (existing) {
      existing.variants.push(variant)
      existing.score = Math.max(existing.score, score)
    } else {
      groups.set(entry.title, {
        title: entry.title,
        // Per-type descriptions are suffixed with "(Liquor Primary)" etc., which reads oddly once the card covers several types.
        description: entry.description.replace(/\s*\([^()]*\)\s*$/, ""),
        kind: entry.kind,
        fee: entry.fee,
        variants: [variant],
        score,
      })
    }
  }
  const ordered = [...groups.values()]
  for (const group of ordered) {
    group.variants.sort((a, b) => LICENCE_TYPE_ORDER.indexOf(a.licenceType) - LICENCE_TYPE_ORDER.indexOf(b.licenceType))
    /* Per-type copy often names its licence inline ("...on your Food Primary licence"); generalize it once the card covers several types. */
    if (group.variants.length > 1) {
      const names = Object.values(LICENCE_TYPE_LABELS)
        .map((l) => l.replace(/[.*+?^${}()|[\]\\]/g, "\\$&"))
        .join("|")
      group.description = group.description.replace(
        new RegExp(`\\b(your|a|an|the)\\s+(${names})\\s+licence`, "gi"),
        "the licence you select",
      )
    }
  }
  ordered.sort((a, b) => b.score - a.score || a.title.localeCompare(b.title))
  return ordered
}

/** Licence types the business does not hold, in canonical display order. */
export function unheldLicenceTypes(held: Set<LicenceType>): LicenceType[] {
  return LICENCE_TYPE_ORDER.filter((type) => type !== "common" && !held.has(type))
}
