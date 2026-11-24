import { type Licence } from "@domain/models/licences"

/** Overdue for renewal: expiry passed and renewal not completed (status still Active / Pending Renewal). `now` is injectable for tests. */
export function isOverdueForRenewal(licence: Pick<Licence, "expiryDate" | "status">, now = new Date()): boolean {
  const expiry = new Date(`${licence.expiryDate}T23:59:59`)
  if (Number.isNaN(expiry.getTime())) return false
  return expiry.getTime() < now.getTime() && licence.status !== "Cancelled" && licence.status !== "Suspended"
}

/** Whole days until expiry (negative when already past). */
export function daysUntilExpiry(licence: Pick<Licence, "expiryDate">, now = new Date()): number {
  const expiry = new Date(`${licence.expiryDate}T00:00:00`)
  const today = new Date(now.getFullYear(), now.getMonth(), now.getDate())
  return Math.round((expiry.getTime() - today.getTime()) / 86_400_000)
}

/** Standard terms and conditions by licence type; the authoritative T&Cs live in the licensing system of record. */
export function getLicenceTerms(licenceType: string): { heading: string; body: string }[] {
  const common = [
    {
      heading: "Hours of Liquor Service",
      body: "Liquor may only be sold or served during the hours authorized on the licence. Service outside approved hours requires a separate authorization.",
    },
    {
      heading: "Authorized Service Area",
      body: "Liquor may only be sold, served, and consumed within the service area approved on the licence and any endorsed patio or temporary use area.",
    },
    {
      heading: "Records & Compliance",
      body: "The licensee must keep purchase and sales records for the period required by the Liquor Control and Licensing Act and produce them on request by an inspector or police officer.",
    },
    {
      heading: "Display of Licence",
      body: "The licence must be displayed in a conspicuous place at the licensed establishment at all times.",
    },
  ]

  if (licenceType === "Cannabis Retail Store") {
    return [
      {
        heading: "Hours of Sale",
        body: "Cannabis may only be sold during the hours authorized on the licence and in accordance with local government hours of operation.",
      },
      {
        heading: "Authorized Products",
        body: "Only cannabis and cannabis accessories permitted under the Cannabis Control and Licensing Act may be sold. No liquor or tobacco may be sold on the premises.",
      },
      {
        heading: "Minors Prohibited",
        body: "Persons under 19 years of age are prohibited from entering the store at all times. Government-issued identification must be checked.",
      },
      {
        heading: "Records & Compliance",
        body: "The licensee must maintain inventory and sales records and report to the federal Cannabis Tracking and Licensing System as required.",
      },
    ]
  }

  return common
}

/** Format an ISO date for display (Canadian long form). */
export function formatLicenceDate(iso: string): string {
  return new Date(iso).toLocaleDateString("en-CA", { year: "numeric", month: "long", day: "numeric" })
}
