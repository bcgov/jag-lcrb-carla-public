import { type BusinessProfile, type LicenceHoldings } from "@domain/models/account"

/** Splits held licences into liquor vs cannabis counts by type label (everything not cannabis counts as liquor); used to prefill the Establishment Planner. */
export function getLicenceHoldings(profile: BusinessProfile): LicenceHoldings {
  let liquor = 0
  let cannabis = 0
  for (const licence of profile.licences) {
    if (/cannabis/i.test(licence.type)) cannabis += 1
    else liquor += 1
  }
  return { liquor, cannabis }
}
