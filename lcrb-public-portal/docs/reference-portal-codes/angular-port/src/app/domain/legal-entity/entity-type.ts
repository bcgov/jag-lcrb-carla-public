/** Legal-entity type resolution; pure data logic with no client dependencies. */

/** Canonical legal-entity types, as stored on the business profile. */
export type CanonicalEntityType =
  | "corporation"
  | "sole-proprietor"
  | "partnership"
  | "society"
  | "cooperative"
  | "indigenous-nation"

/** Accepted spellings per canonical type, compared after normalisation. */
const SYNONYMS: Record<string, string[]> = {
  corporation: ["corporation", "corp", "incorporated", "company", "limitedcompany", "ltd"],
  "sole-proprietor": ["soleproprietor", "soleproprietorship", "soleprop", "proprietorship"],
  partnership: ["partnership", "generalpartnership", "limitedpartnership"],
  society: ["society", "nonprofitsociety", "nonprofit"],
  cooperative: ["cooperative", "coop"],
  "indigenous-nation": ["indigenousnation", "firstnation", "indigenousgovernment", "band"],
}

const normalise = (s: string) => s.toLowerCase().replace(/[^a-z]/g, "")

/** Maps a canonical `legalEntityType` onto a field's options; returns "" when nothing matches so the field stays editable. */
export function resolveEntityTypeOption(
  canonical: string,
  options: { value: string; label: string }[],
): string {
  if (!canonical) return ""
  const accepted = new Set([normalise(canonical), ...(SYNONYMS[canonical] ?? []).map(normalise)])
  const hit = options.find(
    (o) => accepted.has(normalise(o.value)) || accepted.has(normalise(o.label)),
  )
  return hit?.value ?? ""
}
