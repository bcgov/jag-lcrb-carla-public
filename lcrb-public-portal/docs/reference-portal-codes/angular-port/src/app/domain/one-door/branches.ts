/** Branches the "One door" assistant can route to. Only LCRB has a real `href`; the others are simulated, and their hand-off cards say the destination is not part of the prototype. */
export type BranchId = "lcrb" | "icbc" | "health" | "servicebc" | "revenue" | "housing"

export interface Branch {
  id: BranchId
  name: string
  short: string
  /** Where the hand-off lands. Only set when the destination exists in the prototype. */
  href?: string
  /** Plain-language phrases the model should route on. Also shown to the citizen in the card. */
  intents: string[]
  /** One line describing what the branch does, shown on the transfer card. */
  blurb: string
}

export const BRANCHES: Branch[] = [
  {
    id: "lcrb",
    name: "Liquor and Cannabis Regulation Branch",
    short: "LCRB",
    href: "/login?from=one-door",
    intents: [
      "liquor licence",
      "cannabis retail store",
      "special event permit",
      "serve alcohol at an event",
      "restaurant or bar licence",
      "winery, brewery or distillery",
      "patio or hours change",
      "licence renewal or transfer",
      "Serving It Right",
    ],
    blurb: "Licenses and regulates the sale and service of liquor and non-medical cannabis in B.C.",
  },
  {
    id: "icbc",
    name: "ICBC — Driver Licensing and Vehicle Insurance",
    short: "ICBC",
    intents: ["driver's licence", "road test", "vehicle insurance", "Autoplan", "claim after a crash"],
    blurb: "Driver licensing, vehicle registration and basic auto insurance.",
  },
  {
    id: "health",
    name: "Health Insurance BC (MSP)",
    short: "MSP",
    intents: ["Medical Services Plan", "MSP enrolment", "BC Services Card", "PharmaCare"],
    blurb: "Medical Services Plan enrolment and health coverage.",
  },
  {
    id: "servicebc",
    name: "Service BC",
    short: "Service BC",
    intents: ["birth certificate", "marriage certificate", "name change", "general in-person services"],
    blurb: "In-person and phone help for most provincial programs and vital statistics.",
  },
  {
    id: "revenue",
    name: "Ministry of Finance — Revenue Services",
    short: "Revenue",
    intents: ["PST", "property tax deferment", "speculation tax", "employer health tax"],
    blurb: "Provincial taxes, credits and payments.",
  },
  {
    id: "housing",
    name: "BC Housing and the Residential Tenancy Branch",
    short: "Housing",
    intents: ["tenancy dispute", "rent increase", "subsidized housing", "renter's tax credit"],
    blurb: "Rental housing rules, disputes and housing programs.",
  },
]

export function getBranch(id: string): Branch | undefined {
  return BRANCHES.find((b) => b.id === id)
}
