/** Section 119 (First Nations cannabis) - data model + reference knowledge. */


import type { FirstNationProfile } from "@domain/models/section-119"


/** Demo First Nations affiliations keyed by business legal name. */
const FIRST_NATION_SEED: Record<string, FirstNationProfile> = {
  "Spirit Bear Cannabis Ltd.": {
    affiliated: true,
    nationName: "Tla-o-qui-aht First Nation",
    agreementStatus: "signed",
    agreementReference: "S119-2023-014",
    agreementDate: "2023-11-02",
    premisesOnNationLands: true,
  },
  "Cedar Coast Holdings Inc.": {
    affiliated: true,
    nationName: "Snuneymuxw First Nation",
    agreementStatus: "in-negotiation",
    premisesOnNationLands: true,
  },
  "Eagle Ridge Retail Group Ltd.": {
    affiliated: true,
    nationName: "Westbank First Nation",
    agreementStatus: "unknown",
    premisesOnNationLands: false,
  },
}

/** Resolve a First Nations profile for a business, if any is on file. */
export function getFirstNationProfile(businessLegalName?: string): FirstNationProfile | undefined {
  if (!businessLegalName) return undefined
  return FIRST_NATION_SEED[businessLegalName.trim()]
}

/** Business names that have a First Nations profile (for demo pickers). */
export function firstNationBusinessNames(): string[] {
  return Object.keys(FIRST_NATION_SEED)
}
