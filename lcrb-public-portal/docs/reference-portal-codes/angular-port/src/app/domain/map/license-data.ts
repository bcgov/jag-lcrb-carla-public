import { type ApplicationPhase, type LiquorApplicationType, type MonthlySales } from "@domain/models/map"

export function getApplicationTypeDisplayName(type: LiquorApplicationType): string {
  const displayNames: Record<LiquorApplicationType, string> = {
    agent: "Agent",
    catering: "Catering",
    "catering-endorsement": "Catering Endorsement",
    "catering-license-relocation": "Catering License Relocation",
    "family-food-service": "Family Food Service",
    "food-primary": "Food Primary",
    "food-primary-relocation": "Food Primary Relocation",
    "liquor-establishment-name-change": "Liquor Establishment Name Change",
    "liquor-license-transfer": "Liquor License Transfer",
    "permanent-change-to-licensee": "Permanent Change to a Licensee",
    "liquor-primary": "Liquor Primary",
    "third-party-operator": "Third Party Operator",
    manufacturer: "Manufacturer",
  }
  return displayNames[type]
}

export function getApplicationPhaseDisplayName(phase: ApplicationPhase): string {
  const displayNames: Record<ApplicationPhase, string> = {
    "application-assessment": "Application Assessment",
    "incomplete-info-requested": "Incomplete/Info Requested",
    "pending-lg-in-approval": "Pending LG/IN Approval",
    "under-review": "Under Review",
    "approved-in-principle": "Approved in Principle",
  }
  return displayNames[phase]
}

export function calculateRiskScore(contraventions: number, complaints: number): "high" | "medium" | "low" {
  const totalIssues = contraventions + complaints
  if (contraventions >= 3 || complaints >= 5 || totalIssues >= 6) {
    return "high"
  }
  if (contraventions >= 1 || complaints >= 2 || totalIssues >= 2) {
    return "medium"
  }
  return "low"
}

export function calculateSalesTrend(sales: MonthlySales[]): { trend: "up" | "down" | "stable"; percentage: number } {
  if (!sales || sales.length < 6) return { trend: "stable", percentage: 0 }

  const recent3 = sales.slice(-3).reduce((sum, s) => sum + s.amount, 0) / 3
  const previous3 = sales.slice(-6, -3).reduce((sum, s) => sum + s.amount, 0) / 3

  if (previous3 === 0) return { trend: "stable", percentage: 0 }

  const percentChange = ((recent3 - previous3) / previous3) * 100

  if (percentChange > 5) return { trend: "up", percentage: Math.round(percentChange) }
  if (percentChange < -5) return { trend: "down", percentage: Math.round(Math.abs(percentChange)) }
  return { trend: "stable", percentage: Math.round(Math.abs(percentChange)) }
}

export function getCurrentMonthSales(sales: MonthlySales[]): number {
  if (!sales || sales.length === 0) return 0
  return sales[sales.length - 1].amount
}

export function getAnnualSales(sales: MonthlySales[] | undefined): number {
  if (!sales || sales.length === 0) return 0
  return sales.reduce((sum, s) => sum + s.amount, 0)
}

export function getApplicationAgeDays(receivedDate: string | undefined): number {
  if (!receivedDate) return 0
  const received = new Date(receivedDate)
  const today = new Date()
  const diffTime = today.getTime() - received.getTime()
  return Math.floor(diffTime / (1000 * 60 * 60 * 24))
}

export function formatApplicationAge(receivedDate: string | undefined): string {
  const days = getApplicationAgeDays(receivedDate)
  if (days === 0) return "Today"
  if (days === 1) return "1 day"
  if (days < 30) return `${days} days`
  if (days < 60) return "1 month"
  const months = Math.floor(days / 30)
  if (months < 12) return `${months} months`
  const years = Math.floor(months / 12)
  const remainingMonths = months % 12
  if (remainingMonths === 0) return years === 1 ? "1 year" : `${years} years`
  return `${years} year${years > 1 ? "s" : ""}, ${remainingMonths} month${remainingMonths > 1 ? "s" : ""}`
}
