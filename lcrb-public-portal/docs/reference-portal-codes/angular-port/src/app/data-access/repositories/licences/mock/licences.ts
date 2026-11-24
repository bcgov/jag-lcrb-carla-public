/** Mock licence records shared by the Licences list page and the per-licence detail page. */

import { type Licence } from "@domain/models/licences"

export const MOCK_LICENCES: Licence[] = [
  {
    id: "326739",
    type: "Liquor Primary",
    name: "OneStop Test 1",
    status: "Active",
    expiryDate: "2027-04-30",
    address: "1125 Douglas St, Victoria, BC V8W1H2",
    phone: "(123) 123-5555",
    email: "test@example.com",
    operatedBy: "LCRBTrainingBusiness5",
    licencedTo: "",
    flags: ["Transfer Requested", "Licence Operated"],
    category: "owned",
    endorsements: [
      {
        name: "Patron Participation Entertainment",
        kind: "permanent",
        granted: "2023-11-20",
        summary: "Patrons may dance, sing karaoke and take part in entertainment while liquor is served.",
      },
    ],
  },
  {
    id: "326740",
    type: "Food Primary",
    name: "Restaurant ABC",
    status: "Active",
    expiryDate: "2026-12-15",
    address: "456 Government St, Victoria, BC V8V2L4",
    phone: "(250) 555-1234",
    email: "restaurant@example.com",
    operatedBy: "LCRBTrainingBusiness5",
    licencedTo: "",
    flags: [],
    category: "owned",
    endorsements: [
      {
        name: "Family Food Service",
        kind: "permanent",
        granted: "2024-03-12",
        summary: "Minors may be present with a parent or guardian while liquor is served.",
      },
      {
        name: "Temporary Use Area — Patio",
        kind: "temporary",
        granted: "2026-05-01",
        expires: "2026-10-15",
        summary: "Adds the rear patio (32 seats) to the licensed area for the summer season.",
      },
    ],
  },
  {
    id: "326741",
    type: "Cannabis Retail Store",
    name: "Green Leaf Victoria",
    status: "Pending Renewal",
    expiryDate: "2026-06-30",
    address: "789 Yates St, Victoria, BC V8W1L6",
    phone: "(250) 555-9876",
    email: "greenleaf@example.com",
    operatedBy: "LCRBTrainingBusiness5",
    licencedTo: "",
    flags: ["Renewal Due"],
    category: "owned",
  },
  {
    id: "326742",
    type: "Manufacturer",
    name: "Saanich Ridge Winery",
    status: "Active",
    // Expired without renewal - exercises the overdue-renewal highlighting on the licences list and the calendar.
    expiryDate: "2026-07-31",
    address: "5210 Old West Saanich Rd, Saanichton, BC V8M1W8",
    phone: "(250) 555-6620",
    email: "cellar@saanichridge.example.com",
    operatedBy: "LCRBTrainingBusiness5",
    licencedTo: "",
    flags: ["Renewal Overdue"],
    category: "owned",
    endorsements: [
      {
        name: "On-Site Store",
        kind: "permanent",
        granted: "2022-06-01",
        summary: "Sell the winery's own products by the bottle from the on-site retail store.",
      },
      {
        name: "Lounge Area",
        kind: "permanent",
        granted: "2023-04-14",
        summary: "Serve the winery's products by the glass in the 40-seat tasting lounge.",
      },
      {
        name: "Picnic Area",
        kind: "permanent",
        granted: "2024-05-30",
        summary: "Patrons may consume purchased products in the designated outdoor picnic area.",
      },
    ],
  },
]

export const MOCK_OPERATED_LICENCES: Licence[] = [
  {
    id: "326739",
    type: "Liquor Primary",
    name: "OneStop Test 1",
    status: "Active",
    expiryDate: "2027-04-30",
    address: "1125 Douglas St, Victoria, BC V8W1H2",
    phone: "(123) 123-5555",
    email: "test@example.com",
    operatedBy: "LCRBTrainingBusiness5",
    licencedTo: "LCRBTrainingBusiness5",
    flags: ["Transfer Requested"],
    category: "operated",
  },
]

/** Licences held by OTHER businesses the same licensee account is authorized on (beyond their primary business). */
export const MOCK_AFFILIATE_LICENCES: Licence[] = [
  {
    id: "441882",
    type: "Liquor Primary",
    name: "Harbourside Taproom",
    status: "Active",
    expiryDate: "2027-09-30",
    address: "12 Wharf St, Victoria, BC V8W1T4",
    phone: "(250) 555-4410",
    email: "taproom@harbourside.example.com",
    operatedBy: "Harbourside Hospitality Group Ltd.",
    licencedTo: "",
    flags: [],
    category: "owned",
  },
  {
    id: "441883",
    type: "Manufacturer",
    name: "Island Craft Brewery",
    status: "Active",
    expiryDate: "2027-03-31",
    address: "3400 Bridge St, Victoria, BC V8T4Z9",
    phone: "(250) 555-7720",
    email: "brewery@islandcraft.example.com",
    operatedBy: "Island Craft Beverages Inc.",
    licencedTo: "",
    flags: [],
    category: "owned",
  },
  {
    id: "441884",
    type: "Liquor Primary",
    name: "Island Craft Tasting Room",
    status: "Pending Renewal",
    expiryDate: "2026-08-31",
    address: "3402 Bridge St, Victoria, BC V8T4Z9",
    phone: "(250) 555-7721",
    email: "tasting@islandcraft.example.com",
    operatedBy: "Island Craft Beverages Inc.",
    licencedTo: "",
    flags: ["Renewal Due"],
    category: "owned",
  },
]

/** Look up a licence by id across owned, operated, and affiliate lists. */
export function getLicenceById(id: string): Licence | undefined {
  return [...MOCK_LICENCES, ...MOCK_OPERATED_LICENCES, ...MOCK_AFFILIATE_LICENCES].find(
    (l) => l.id === id,
  )
}

/** All ids for static generation of detail routes. */
export function getAllLicenceIds(): string[] {
  const ids = new Set<string>()
  for (const l of [...MOCK_LICENCES, ...MOCK_OPERATED_LICENCES]) ids.add(l.id)
  return [...ids]
}
