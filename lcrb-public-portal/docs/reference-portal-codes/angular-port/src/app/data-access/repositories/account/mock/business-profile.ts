"use client"

/** Business profile for the authenticated licensee. */

import { MOCK_LICENCES } from "../../licences/mock/licences"
import { getFirstNationProfile } from "../../compliance/mock/section-119"
import { type BusinessProfile } from "@domain/models/account"

/** Resolve the business profile for the given organization. */
export function getBusinessProfile(organization?: string): BusinessProfile {
  const legalName = organization?.trim() ? organization.trim() : "LCRB Training Business 5 Ltd."
  return {
    legalName,
    bceidBusinessNumber: "BC0987654",
    legalEntityType: "corporation",
    incorporationNumber: "BC1234567",
    licences: MOCK_LICENCES.map((l) => ({
      number: l.id,
      establishmentName: l.name,
      type: l.type,
      address: l.address,
    })),
    firstNation: getFirstNationProfile(legalName),
  }
}

/* `resolveEntityTypeOption` lives in ./entity-type - this module is "use client" (it exposes hooks) */
