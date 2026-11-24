export type LicenseType = "liquor" | "cannabis" | "special-event"

export type LicenseStatus = "licensed" | "application" | "special-event"

export type LicenseActiveStatus = "active" | "dormant" | "suspended"

export type ApplicationPhase =
  | "application-assessment"
  | "incomplete-info-requested"
  | "pending-lg-in-approval"
  | "under-review"
  | "approved-in-principle"

export type LiquorApplicationType =
  | "agent"
  | "catering"
  | "catering-endorsement"
  | "catering-license-relocation"
  | "family-food-service"
  | "food-primary"
  | "food-primary-relocation"
  | "liquor-establishment-name-change"
  | "liquor-license-transfer"
  | "permanent-change-to-licensee"
  | "liquor-primary"
  | "third-party-operator"
  | "manufacturer"

export interface Contravention {
  id: string
  date: string
  description: string
  severity: "minor" | "major" | "critical"
  status: "resolved" | "pending" | "appealed"
}

export interface Complaint {
  id: string
  date: string
  description: string
  category: "noise" | "overcrowding" | "underage" | "hours" | "other"
  status: "investigating" | "resolved" | "unfounded"
}

export interface InspectionRecord {
  id: string
  date: string
  inspector: string
  result: "pass" | "fail" | "warning"
  notes: string
}

export interface Inspection {
  id: string
  date: string
  inspector: string
  result: "pass" | "fail" | "warning"
  notes: string
}

export interface MonthlySales {
  month: string // Format: "YYYY-MM"
  amount: number // Dollar amount of wholesale purchases
}

export interface License {
  id: string
  businessName: string
  name?: string
  address: string
  city: string
  licenseNumber: string
  licenseType: LicenseType
  status: LicenseStatus
  activeStatus?: LicenseActiveStatus
  applicationPhase?: ApplicationPhase
  applicationType?: LiquorApplicationType // Added application type field
  applicationReceivedDate?: string // Added application received date for calculating age
  applicationIntelligence?: ApplicationIntelligence // Added application intelligence field
  lat: number
  lng: number
  phone: string
  email: string
  website: string
  contraventions: number
  complaints: number
  lastInspection: string
  assignedInspector: string
  region: string
  contraventionHistory?: Contravention[]
  complaintHistory?: Complaint[]
  inspectionHistory?: InspectionRecord[]
  establishmentSize?: "small" | "medium" | "large"
  yearEstablished?: number
  eventDate?: string
  expectedAttendees?: number
  monthlySales?: MonthlySales[]
}

// START MERGE FOR ONLINE RESEARCH FINDINGS
export interface OnlineResearchFinding {
  type: "news" | "social-media" | "public-records" | "corporate-registry" | "court-records" | "business-history"
  source: string
  title: string
  summary: string
  date?: string
  url?: string
  sentiment: "positive" | "neutral" | "negative" | "concerning"
  relevance: "high" | "medium" | "low"
}

// Application Intelligence Interfaces
export interface DirectorOfficer {
  name: string
  role: "director" | "officer" | "shareholder"
  sharePercentage?: number // For shareholders
  securityScreeningStatus: "pending" | "cleared" | "flagged" | "expired"
  financialIntegrityStatus?: "pending" | "cleared" | "flagged" // Cannabis only
  otherLicenseConnections?: string[] // Other licenses this person is connected to
  notes?: string
}

export interface TiedHouseConnection {
  type: "manufacturer" | "agent" | "producer" // producer for cannabis
  entityName: string
  relationshipType: "financial" | "family" | "business" | "employment"
  riskLevel: "low" | "medium" | "high"
  details?: string
}

export interface FitAndProperAssessment {
  status: "pending" | "in-progress" | "cleared" | "concerns" | "failed"
  securityScreeningComplete: boolean
  financialIntegrityComplete: boolean
  associatesIdentified: number
  associatesCleared: number
  illicitOperationHistory: boolean
  criminalRecordFlags: boolean
  estimatedCompletionDays?: number
  notes?: string
}

export interface ApplicationIntelligence {
  directorsOfficers: DirectorOfficer[]
  tiedHouseConnections: TiedHouseConnection[]
  fitAndProperAssessment?: FitAndProperAssessment // Cannabis applications only
  corporateStructureComplexity: "simple" | "moderate" | "complex"
  similarApplicationsInArea: number
  aiRiskScore: number // 0-100
  aiFindings: ApplicationAIFinding[]
  onlineResearchFindings: OnlineResearchFinding[]
}

export interface ApplicationAIFinding {
  type: "tied-house" | "fit-proper" | "director" | "location" | "history" | "corporate"
  severity: "info" | "low" | "medium" | "high"
  title: string
  description: string
  source?: string
  recommendation?: string
}

/** A single generic marker placed on the map. */
export interface MapMarkerData {
  id: string
  layerId: string
  lat: number
  lng: number
  /** Marker pin colour (hex). */
  color: string
  /** Pre-rendered, escaped popup HTML. */
  popupHtml: string
}

/** A toggleable group of markers with its own legend. */
export interface MapLayerConfig {
  id: string
  label: string
  /** Colour of the toggle chip / layer accent. */
  accent: string
  markers: MapMarkerData[]
  legend?: { label: string; color: string }[]
}
