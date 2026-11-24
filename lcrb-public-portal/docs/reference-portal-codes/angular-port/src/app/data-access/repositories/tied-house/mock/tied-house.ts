/** Tied House - data model. */

import { MOCK_LICENCES, MOCK_AFFILIATE_LICENCES } from "../../licences/mock/licences"
import { type Licence } from "@domain/models/licences"
import { type TiedHouseDomain, type AssociableLicence, type TiedHouseDeclaration, type ExistingRelationship, type FeeLine, type FeeBreakdown } from "@domain/models/tied-house"
import { domainForLicenceType } from "@domain/tied-house/tied-house"
import { FEE_RATE, CHANGE_TYPE_LABEL } from "@domain/tied-house/tied-house"

function toAssociable(l: Licence): AssociableLicence {
  return {
    number: l.id,
    establishmentName: l.name,
    type: l.type,
    domain: domainForLicenceType(l.type),
    operatedBy: l.operatedBy,
  }
}

/** Other B.C. liquor/cannabis licences a declaration can be associated with. */
export const ASSOCIABLE_LICENCES: AssociableLicence[] = [
  ...MOCK_AFFILIATE_LICENCES.map(toAssociable),
  {
    number: "512004",
    establishmentName: "Pacific Coast Distillery",
    type: "Manufacturer",
    domain: "liquor",
    operatedBy: "Pacific Coast Spirits Ltd.",
  },
  {
    number: "512900",
    establishmentName: "Summit Rooftop Lounge",
    type: "Liquor Primary",
    domain: "liquor",
    operatedBy: "Summit Hospitality Group Ltd.",
  },
  {
    number: "608114",
    establishmentName: "Harbour Cannabis Co.",
    type: "Cannabis Retail Store",
    domain: "cannabis",
    operatedBy: "Harbourside Hospitality Group Ltd.",
  },
  {
    number: "609552",
    establishmentName: "Westside Green Room",
    type: "Cannabis Retail Store",
    domain: "cannabis",
    operatedBy: "Westcoast Beverage Partners",
  },
  {
    number: "441120",
    establishmentName: "Legacy Public House",
    type: "Liquor Primary",
    domain: "liquor",
    operatedBy: "Legacy Pubs Holdings Ltd.",
  },
]

/** Search the associable-licence directory by number or establishment name. */
export function searchAssociableLicences(query: string, limit = 20): AssociableLicence[] {
  const q = query.trim().toLowerCase()
  const results = q
    ? ASSOCIABLE_LICENCES.filter(
        (l) =>
          l.number.toLowerCase().includes(q) ||
          l.establishmentName.toLowerCase().includes(q) ||
          l.operatedBy.toLowerCase().includes(q),
      )
    : ASSOCIABLE_LICENCES
  return [...results]
    .sort((a, b) => a.establishmentName.localeCompare(b.establishmentName))
    .slice(0, limit)
}

export function getAssociableLicence(number: string): AssociableLicence | undefined {
  return ASSOCIABLE_LICENCES.find((l) => l.number === number)
}


export function sampleExistingRelationships(): ExistingRelationship[] {
  return [
    {
      id: "ex_1",
      type: "individual",
      displayName: "Morgan Blake – 14/03/1979",
      relationshipLabel: "Spouse",
      associatedLicenceNumber: "441882",
      associatedEstablishmentName: "Harbourside Taproom",
      domain: "liquor",
      dateTied: "2024-09-12",
    },
    {
      id: "ex_2",
      type: "legal-entity",
      displayName: "Island Craft Beverages Inc.",
      relationshipLabel: "Shareholder",
      associatedLicenceNumber: "441883",
      associatedEstablishmentName: "Island Craft Brewery",
      domain: "liquor",
      dateTied: "2023-06-01",
    },
  ]
}

/** Number of the declarant's OWN licences impacted, per domain. */
export function impactedLicenceCount(domain: TiedHouseDomain): number {
  return MOCK_LICENCES.filter((l) => domainForLicenceType(l.type) === domain).length
}

/** Compute the split liquor/cannabis fee breakdown for saved declarations. */
export function computeFees(declarations: TiedHouseDeclaration[], jobNumber: string): FeeBreakdown {
  const saved = declarations.filter((d) => d.status === "saved")
  const liquorLines: FeeLine[] = []
  const cannabisLines: FeeLine[] = []

  for (const d of saved) {
    const rate = FEE_RATE[d.changeType]
    const licences = impactedLicenceCount(d.domain)
    const amount = rate * licences
    if (amount <= 0) continue
    const line: FeeLine = {
      declarationId: d.id,
      label: `${CHANGE_TYPE_LABEL[d.changeType]} — ${jobNumber}`,
      licencesImpacted: licences,
      rate,
      amount,
      domain: d.domain,
    }
    if (d.domain === "cannabis") cannabisLines.push(line)
    else liquorLines.push(line)
  }

  const liquorTotal = liquorLines.reduce((a, l) => a + l.amount, 0)
  const cannabisTotal = cannabisLines.reduce((a, l) => a + l.amount, 0)

  return {
    liquor: { lines: liquorLines, total: liquorTotal },
    cannabis: { lines: cannabisLines, total: cannabisTotal },
    grandTotal: liquorTotal + cannabisTotal,
  }
}
