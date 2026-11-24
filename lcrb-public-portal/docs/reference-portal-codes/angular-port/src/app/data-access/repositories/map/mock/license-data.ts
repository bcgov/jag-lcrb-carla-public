import { type LicenseActiveStatus, type ApplicationPhase, type LiquorApplicationType, type Contravention, type Complaint, type InspectionRecord, type Inspection, type MonthlySales, type License, type OnlineResearchFinding, type DirectorOfficer, type TiedHouseConnection, type FitAndProperAssessment, type ApplicationIntelligence, type ApplicationAIFinding } from "@domain/models/map"

// BC Regions with geographic areas
export const bcRegions = [
  {
    id: "region-1",
    name: "Region 1 - Lower Mainland",
    description: "Vancouver, Burnaby, Richmond, North Vancouver, New Westminster, Coquitlam, Port Coquitlam",
  },
  {
    id: "region-2",
    name: "Region 2 - Fraser Valley",
    description: "Surrey, Langley, Abbotsford, Chilliwack, Maple Ridge, White Rock",
  },
  {
    id: "region-3",
    name: "Region 3 - Vancouver Island",
    description: "Victoria, Nanaimo, Courtenay, Campbell River, Powell River",
  },
  {
    id: "region-4",
    name: "Region 4 - Interior & Kootenays",
    description: "Kelowna, Kamloops, Vernon, Penticton, Cranbrook, Nelson",
  },
  {
    id: "region-5",
    name: "Region 5 - Northern BC",
    description: "Prince George, Fort St. John, Terrace, Squamish, Whistler",
  },
]

export const regions = bcRegions

const cityToRegion: { [key: string]: string } = {
  Vancouver: "region-1",
  Burnaby: "region-1",
  Richmond: "region-1",
  "North Vancouver": "region-1",
  "New Westminster": "region-1",
  Coquitlam: "region-1",
  "Port Coquitlam": "region-1",
  Surrey: "region-2",
  Langley: "region-2",
  Abbotsford: "region-2",
  Chilliwack: "region-2",
  "Maple Ridge": "region-2",
  "White Rock": "region-2",
  Victoria: "region-3",
  Nanaimo: "region-3",
  Courtenay: "region-3",
  "Campbell River": "region-3",
  "Powell River": "region-3",
  Kelowna: "region-4",
  Kamloops: "region-4",
  Vernon: "region-4",
  Penticton: "region-4",
  Cranbrook: "region-4",
  Nelson: "region-4",
  "Prince George": "region-5",
  "Fort St. John": "region-5",
  Terrace: "region-5",
  Squamish: "region-5",
  Whistler: "region-5",
}

// BC Cities with coordinates
const bcCities = [
  { city: "Vancouver", lat: 49.2827, lng: -123.1207 },
  { city: "Victoria", lat: 48.4284, lng: -123.3656 },
  { city: "Surrey", lat: 49.1913, lng: -122.849 },
  { city: "Burnaby", lat: 49.2488, lng: -122.9805 },
  { city: "Richmond", lat: 49.1666, lng: -123.1336 },
  { city: "Kelowna", lat: 49.888, lng: -119.496 },
  { city: "Kamloops", lat: 50.6745, lng: -120.3273 },
  { city: "Nanaimo", lat: 49.1659, lng: -123.9401 },
  { city: "Prince George", lat: 53.9171, lng: -122.7497 },
  { city: "Chilliwack", lat: 49.1579, lng: -121.9514 },
  { city: "Abbotsford", lat: 49.0504, lng: -122.3045 },
  { city: "Coquitlam", lat: 49.2838, lng: -122.7932 },
  { city: "Langley", lat: 49.1044, lng: -122.6609 },
  { city: "Maple Ridge", lat: 49.2193, lng: -122.5984 },
  { city: "North Vancouver", lat: 49.3165, lng: -123.0688 },
  { city: "New Westminster", lat: 49.2057, lng: -122.911 },
  { city: "Vernon", lat: 50.267, lng: -119.272 },
  { city: "Penticton", lat: 49.4991, lng: -119.5937 },
  { city: "Courtenay", lat: 49.6878, lng: -124.9944 },
  { city: "Campbell River", lat: 50.0244, lng: -125.2475 },
  { city: "Cranbrook", lat: 49.5097, lng: -115.7688 },
  { city: "Fort St. John", lat: 56.2465, lng: -120.8476 },
  { city: "Port Coquitlam", lat: 49.2628, lng: -122.7811 },
  { city: "Squamish", lat: 49.7016, lng: -123.1558 },
  { city: "Whistler", lat: 50.1163, lng: -122.9574 },
  { city: "White Rock", lat: 49.0253, lng: -122.8029 },
  { city: "Nelson", lat: 49.4928, lng: -117.2948 },
  { city: "Terrace", lat: 54.5182, lng: -128.5926 },
  { city: "Powell River", lat: 49.8353, lng: -124.5247 },
  { city: "Tofino", lat: 49.153, lng: -125.9066 },
]

const liquorNames = [
  "The Rusty Anchor Pub",
  "Gastown Brewing Co.",
  "Pacific Rim Bar & Grill",
  "Cedar Creek Winery",
  "The Keg Steakhouse",
  "Steamworks Brewing",
  "The Flying Pig",
  "Yaletown Brewing Company",
  "Mahony & Sons",
  "The Irish Heather",
  "Rogue Kitchen & Wetbar",
  "Craft Beer Market",
  "Tap & Barrel",
  "Browns Socialhouse",
  "Cactus Club Cafe",
  "Earls Kitchen + Bar",
  "Joey Restaurant",
  "Moxies Grill & Bar",
  "Original Joe's",
  "Boston Pizza",
  "Red Robin",
  "Milestones",
  "The Keg Manor",
  "White Spot",
  "Denny's Restaurant",
  "IHOP Vancouver",
  "The Old Spaghetti Factory",
  "The Boathouse",
  "Cardero's Restaurant",
  "Lift Bar Grill View",
  "Coal Harbour Bar & Grill",
  "Provence Marinaside",
  "Blue Water Cafe",
  "Coast Restaurant",
  "Glowbal Grill",
  "Italian Kitchen",
  "Tableau Bar Bistro",
  "Hawksworth Restaurant",
  "Boulevard Kitchen",
  "Botanist",
  "Chambar Restaurant",
  "L'Abattoir",
  "Bao Bei Chinese Brasserie",
  "Kissa Tanto",
  "Published on Main",
  "Farmer's Apprentice",
  "Savio Volpe",
  "Ask for Luigi",
  "Nightingale",
  "Pourhouse Vancouver",
  "The Diamond",
  "Guilt & Co",
  "The Alibi Room",
  "The Railway Club",
  "Blarney Stone",
  "Doolin's Irish Pub",
  "Johnnie Fox's",
  "The Cambie",
  "The Granville Room",
  "Celebrities Nightclub",
  "Republic Nightclub",
  "Venue Nightclub",
  "The Roxy",
  "Caprice Nightclub",
  "BC Liquor Store Downtown",
  "Everything Wine",
  "Marquis Wine Cellars",
  "Liberty Wine Merchants",
  "Taylorwood Wines",
  "Kitsilano Wine Cellar",
  "Broadway International Wine",
  "Dunbar Liquor Store",
  "West Point Grey Liquor",
  "Kerrisdale Liquor Store",
  "South Granville Liquor",
  "Commercial Drive Liquor",
  "Main Street Liquor",
  "Hastings Liquor Store",
  "East Van Liquor",
  "Strathcona Beer Company",
  "Storm Brewing",
  "Powell Brewery",
  "Callister Brewing",
  "Luppolo Brewing",
  "Parallel 49 Brewing",
  "Brassneck Brewery",
  "33 Acres Brewing",
  "Faculty Brewing",
  "Strange Fellows Brewing",
  "Superflux Beer Company",
  "Container Brewing",
  "East Van Brewing",
  "Red Truck Beer Company",
  "Stanley Park Brewing",
  "Granville Island Brewing",
  "Bridge Brewing",
  "Dageraad Brewing",
  "Four Winds Brewing",
  "Backcountry Brewing",
  "Yellow Dog Brewing",
  "Twin Sails Brewing",
  "Steel & Oak Brewing",
  "Moody Ales",
  "Parkside Brewery",
  "Trading Post Brewing",
  "Cannery Brewing",
  "Tin Whistle Brewing",
  "Firehall Brewery",
]

const cannabisNames = [
  "Evergreen Cannabis Society",
  "BC Cannabis Store",
  "Hobo Recreational Cannabis",
  "Dutch Love Cannabis",
  "City Cannabis Co.",
  "Kiaro Cannabis",
  "Spiritleaf",
  "Fire & Flower",
  "Tokyo Smoke",
  "Tweed",
  "FOUR20 Premium Market",
  "Canna Cabana",
  "Meta Cannabis Supply Co.",
  "Value Buds",
  "Sessions Cannabis",
  "Greenlight Cannabis",
  "Seed & Stone",
  "Burb Cannabis",
  "Clarity Cannabis",
  "Eden Cannabis",
  "Kush Cannabis",
  "Lotus Land Cannabis",
  "Mountain High Cannabis",
  "Northern Lights Cannabis",
  "Pacific Cannabis Company",
  "Rainforest Cannabis",
  "Sacred Herb",
  "Sanctuary Cannabis",
  "Sunrise Cannabis",
  "The Green Room",
  "The Joint Cannabis",
  "True North Cannabis",
  "Vancouver Cannabis Club",
  "West Coast Cannabis",
  "Wild Flower Cannabis",
  "Zen Leaf Cannabis",
  "Budding Cannabis",
  "Canopy Cannabis",
  "Coastal Cannabis",
  "Emerald Cannabis",
  "Herbal Wellness Cannabis",
  "Island Cannabis",
  "Jade Cannabis",
  "Karma Cannabis",
  "Leaf & Vine Cannabis",
  "Mellow Cannabis",
  "Natural Cannabis Co.",
  "Oasis Cannabis",
  "Pure Cannabis",
  "Quantum Cannabis",
]

const cannabisApplicationNames = [
  "Evergreen Cannabis Society (Pending)",
  "BC Cannabis Store (Pending)",
  "Hobo Recreational Cannabis (Pending)",
  "Dutch Love Cannabis (Pending)",
  "City Cannabis Co. (Pending)",
  "Kiaro Cannabis (Pending)",
  "Spiritleaf (Pending)",
  "Fire & Flower (Pending)",
  "Tokyo Smoke (Pending)",
  "Tweed (Pending)",
  "FOUR20 Premium Market (Pending)",
  "Canna Cabana (Pending)",
  "Meta Cannabis Supply Co. (Pending)",
  "Value Buds (Pending)",
  "Sessions Cannabis (Pending)",
  "Greenlight Cannabis (Pending)",
  "Seed & Stone (Pending)",
  "Burb Cannabis (Pending)",
  "Clarity Cannabis (Pending)",
  "Eden Cannabis (Pending)",
  "Kush Cannabis (Pending)",
  "Lotus Land Cannabis (Pending)",
  "Mountain High Cannabis (Pending)",
  "Northern Lights Cannabis (Pending)",
  "Pacific Cannabis Company (Pending)",
  "Rainforest Cannabis (Pending)",
  "Sacred Herb (Pending)",
  "Sanctuary Cannabis (Pending)",
  "Sunrise Cannabis (Pending)",
  "The Green Room (Pending)",
  "The Joint Cannabis (Pending)",
  "True North Cannabis (Pending)",
  "Vancouver Cannabis Club (Pending)",
  "West Coast Cannabis (Pending)",
  "Wild Flower Cannabis (Pending)",
  "Zen Leaf Cannabis (Pending)",
  "Budding Cannabis (Pending)",
  "Canopy Cannabis (Pending)",
  "Coastal Cannabis (Pending)",
  "Emerald Cannabis (Pending)",
  "Herbal Wellness Cannabis (Pending)",
  "Island Cannabis (Pending)",
  "Jade Cannabis (Pending)",
  "Karma Cannabis (Pending)",
  "Leaf & Vine Cannabis (Pending)",
  "Mellow Cannabis (Pending)",
  "Natural Cannabis Co. (Pending)",
  "Oasis Cannabis (Pending)",
  "Pure Cannabis (Pending)",
  "Quantum Cannabis (Pending)",
]

const specialEventNames = [
  "Vancouver Food Festival",
  "BC Wine Festival",
  "Victoria Jazz Fest",
  "Kelowna Summer Concert",
  "Surrey Music Festival",
  "Burnaby Arts Gala",
  "Richmond Night Market",
  "Whistler Après Ski Party",
  "Tofino Beach Bash",
  "Nanaimo Harbour Festival",
  "Kamloops Rodeo",
  "Prince George Winter Carnival",
  "Chilliwack Corn Festival",
  "Abbotsford Air Show",
  "Langley Farm Festival",
  "Maple Ridge Country Fair",
  "North Van Film Festival",
  "New West Pride Festival",
  "Vernon Winter Festival",
  "Penticton Wine Country",
  "Courtenay Seafood Fest",
  "Campbell River Salmon Fest",
  "Cranbrook Fall Fair",
  "Fort St. John Oil & Gas",
  "Port Coquitlam Car Show",
  "Squamish Adventure Fest",
  "White Rock Sea Festival",
  "Nelson Arts Festival",
  "Terrace Riverboat Days",
  "Powell River Logger Sports",
  "Vancouver Pride Festival",
  "Victoria Fringe Festival",
  "Surrey Fusion Festival",
  "Burnaby Blues Festival",
  "Richmond Dragon Boat",
  "Kelowna Canada Day",
  "Kamloops Music in Park",
  "Nanaimo Bathtub Race",
  "PG Studio Tour",
  "Chilliwack Tulip Festival",
  "Abbotsford Berry Festival",
  "Langley Winery Tour",
  "Maple Ridge Craft Market",
  "North Van Shipyards Fest",
  "New West Hyack Festival",
  "Vernon Creative Chaos",
  "Penticton Peach Festival",
  "Courtenay Filberg Fest",
  "Campbell River Pier Party",
  "Whistler Film Festival",
]

const streetNames = [
  "Main Street",
  "Granville Street",
  "Robson Street",
  "Davie Street",
  "Hastings Street",
  "Broadway",
  "Commercial Drive",
  "Cambie Street",
  "Fraser Street",
  "Knight Street",
  "Kingsway",
  "Victoria Drive",
  "Oak Street",
  "Arbutus Street",
  "Dunbar Street",
  "Marine Drive",
  "41st Avenue",
  "49th Avenue",
  "King George Boulevard",
  "Scott Road",
  "Lougheed Highway",
  "Canada Way",
  "Boundary Road",
  "Willingdon Avenue",
  "Brentwood Boulevard",
]

const contraventionTypes = [
  "Serving after hours",
  "Failure to check ID",
  "Over-serving intoxicated patron",
  "Noise violation",
  "Capacity exceeded",
  "Improper storage of cannabis",
  "Missing social responsibility signage",
  "Unauthorized entertainment",
]

const complaintCategories: Complaint["category"][] = ["noise", "overcrowding", "underage", "hours", "other"]

function randomDate(start: Date, end: Date): string {
  const date = new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime()))
  return date.toISOString().split("T")[0]
}

function generateContraventions(count: number): Contravention[] {
  const contraventions: Contravention[] = []
  const threeYearsAgo = new Date()
  threeYearsAgo.setFullYear(threeYearsAgo.getFullYear() - 3)

  for (let i = 0; i < count; i++) {
    contraventions.push({
      id: `cont-${Math.random().toString(36).substr(2, 9)}`,
      date: randomDate(threeYearsAgo, new Date()),
      description: contraventionTypes[Math.floor(Math.random() * contraventionTypes.length)],
      severity: ["minor", "major", "critical"][Math.floor(Math.random() * 3)] as Contravention["severity"],
      status: ["resolved", "pending", "appealed"][Math.floor(Math.random() * 3)] as Contravention["status"],
    })
  }
  return contraventions
}

function generateInspections(count: number): Inspection[] {
  const inspections: Inspection[] = []
  const twoYearsAgo = new Date()
  twoYearsAgo.setFullYear(twoYearsAgo.getFullYear() - 2)

  const inspectorsList = ["Sarah Chen", "Michael Park", "Jennifer Wong", "David Kumar", "Lisa Thompson"]

  for (let i = 0; i < count; i++) {
    inspections.push({
      id: `insp-${Math.random().toString(36).substr(2, 9)}`,
      date: randomDate(twoYearsAgo, new Date()),
      inspector: inspectorsList[Math.floor(Math.random() * inspectorsList.length)],
      result: ["pass", "fail", "warning"][Math.floor(Math.random() * 3)] as Inspection["result"],
      notes: "Standard compliance inspection completed.",
    })
  }
  return inspections.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
}

function generateOnlineIntelligence(): {
  id: string
  severity: "high" | "medium" | "low"
  source: string
  sourceType: "website" | "social-media" | "review"
  description: string
  detectedDate: string
}[] {
  const intelligence: {
    id: string
    severity: "high" | "medium" | "low"
    source: string
    sourceType: "website" | "social-media" | "review"
    description: string
    detectedDate: string
  }[] = []
  const sources = ["Google", "Facebook", "Yelp"]
  const severities = ["high", "medium", "low"]

  for (let i = 0; i < Math.floor(Math.random() * 3) + 1; i++) {
    intelligence.push({
      id: `int-${Math.random().toString(36).substr(2, 9)}`,
      severity: severities[Math.floor(Math.random() * 3)] as "high" | "medium" | "low",
      source: sources[Math.floor(Math.random() * sources.length)],
      sourceType: ["website", "social-media", "review"][Math.floor(Math.random() * 3)] as
        | "website"
        | "social-media"
        | "review",
      description: `Online intelligence ${i + 1}`,
      detectedDate: randomDate(new Date(2023, 0, 1), new Date()),
    })
  }
  return intelligence
}

const ONLINE_RESEARCH_TEMPLATES: {
  type: OnlineResearchFinding["type"]
  sources: string[]
  templates: { title: string; summary: string; sentiment: OnlineResearchFinding["sentiment"] }[]
}[] = [
  {
    type: "news",
    sources: ["Vancouver Sun", "BC Business", "Daily Hive", "CBC News", "Global News BC"],
    templates: [
      {
        title: "Local entrepreneur expands hospitality portfolio",
        summary:
          "Business owner behind the application has been featured positively for community involvement and successful restaurant operations.",
        sentiment: "positive",
      },
      {
        title: "Business owner named in civil lawsuit",
        summary:
          "One of the principals was defendant in a 2022 civil lawsuit regarding a previous business venture. Case was settled out of court.",
        sentiment: "concerning",
      },
      {
        title: "New cannabis retail store proposed for downtown location",
        summary: "Local news coverage of the application with mixed community reaction noted in comments.",
        sentiment: "neutral",
      },
      {
        title: "Former restaurant cited for liquor violations",
        summary:
          "A previous establishment operated by one of the directors received multiple violations before closure.",
        sentiment: "concerning",
      },
    ],
  },
  {
    type: "social-media",
    sources: ["LinkedIn", "Facebook Business", "Instagram", "X (Twitter)"],
    templates: [
      {
        title: "Active professional presence",
        summary:
          "Directors maintain professional LinkedIn profiles with verifiable employment history in hospitality industry.",
        sentiment: "positive",
      },
      {
        title: "Business social media established",
        summary:
          "The applicant business has established social media presence with consistent branding and community engagement.",
        sentiment: "positive",
      },
      {
        title: "Limited online presence",
        summary:
          "Minimal social media footprint for the business and principals. Unable to verify some background claims.",
        sentiment: "neutral",
      },
      {
        title: "Controversial social media posts",
        summary:
          "Historical social media posts from one principal include content that may raise reputational concerns.",
        sentiment: "concerning",
      },
    ],
  },
  {
    type: "public-records",
    sources: ["BC Registry Services", "Land Title Survey Authority", "WorkSafeBC", "Consumer Protection BC"],
    templates: [
      {
        title: "Clean business registry history",
        summary: "Corporate entity in good standing with no administrative actions or compliance orders on record.",
        sentiment: "positive",
      },
      {
        title: "Previous business dissolution",
        summary:
          "One principal was director of a company that was dissolved for failure to file annual reports in 2019.",
        sentiment: "neutral",
      },
      {
        title: "WorkSafeBC compliance orders",
        summary: "A previous business associated with the applicant had workplace safety orders issued in 2021.",
        sentiment: "concerning",
      },
    ],
  },
  {
    type: "corporate-registry",
    sources: ["BC Corporate Registry", "Canada Business Registries", "SEDAR+"],
    templates: [
      {
        title: "Straightforward corporate structure",
        summary:
          "Single BC-incorporated company with clearly identified beneficial owners matching application disclosure.",
        sentiment: "positive",
      },
      {
        title: "Multi-layered corporate structure",
        summary:
          "Application involves holding company with numbered subsidiaries. Beneficial ownership requires verification.",
        sentiment: "neutral",
      },
      {
        title: "Out-of-province parent company",
        summary: "BC entity is subsidiary of Alberta corporation. Full ownership chain documented but complex.",
        sentiment: "neutral",
      },
      {
        title: "Discrepancy in corporate records",
        summary: "Director listed on application does not appear in current corporate registry filings.",
        sentiment: "concerning",
      },
    ],
  },
  {
    type: "court-records",
    sources: ["BC Court Services", "Civil Resolution Tribunal", "Federal Court Records"],
    templates: [
      {
        title: "No court records found",
        summary: "Search of available court databases returned no matches for principals or associated businesses.",
        sentiment: "positive",
      },
      {
        title: "Minor civil matters resolved",
        summary:
          "Small claims matters from 5+ years ago involving one principal. All resolved with no pattern of concern.",
        sentiment: "neutral",
      },
      {
        title: "Active litigation",
        summary: "One principal is currently party to civil litigation regarding a business dispute.",
        sentiment: "concerning",
      },
    ],
  },
  {
    type: "business-history",
    sources: ["Google Reviews", "Yelp", "TripAdvisor", "Better Business Bureau"],
    templates: [
      {
        title: "Strong track record in hospitality",
        summary:
          "Previous establishments operated by principals show consistent 4+ star ratings and positive customer feedback.",
        sentiment: "positive",
      },
      {
        title: "Mixed reviews at previous venue",
        summary:
          "A previous restaurant operated by one director had mixed reviews, with some complaints about service quality.",
        sentiment: "neutral",
      },
      {
        title: "BBB complaints on file",
        summary:
          "Better Business Bureau shows 3 unresolved complaints against a previous business operated by the applicant.",
        sentiment: "concerning",
      },
      {
        title: "First-time operator",
        summary:
          "No previous hospitality or retail experience found for the principals. Limited track record to assess.",
        sentiment: "neutral",
      },
    ],
  },
]

function generateOnlineResearchFindings(businessName: string, directors: DirectorOfficer[]): OnlineResearchFinding[] {
  const findings: OnlineResearchFinding[] = []
  const numFindings = Math.floor(Math.random() * 5) + 2 // 2-6 findings

  const usedTypes = new Set<string>()

  for (let i = 0; i < numFindings; i++) {
    const typeData = ONLINE_RESEARCH_TEMPLATES[Math.floor(Math.random() * ONLINE_RESEARCH_TEMPLATES.length)]

    // Avoid too many of the same type
    if (usedTypes.has(typeData.type) && Math.random() > 0.3) continue
    usedTypes.add(typeData.type)

    const template = typeData.templates[Math.floor(Math.random() * typeData.templates.length)]
    const source = typeData.sources[Math.floor(Math.random() * typeData.sources.length)]

    // Generate a realistic date within the last 2 years
    const daysAgo = Math.floor(Math.random() * 730)
    const date = new Date()
    date.setDate(date.getDate() - daysAgo)

    findings.push({
      type: typeData.type,
      source,
      title: template.title,
      summary: template.summary,
      date: date.toISOString().split("T")[0],
      sentiment: template.sentiment,
      relevance:
        template.sentiment === "concerning"
          ? "high"
          : template.sentiment === "positive"
            ? "medium"
            : Math.random() > 0.5
              ? "medium"
              : "low",
    })
  }

  // Sort by relevance (high first) then by date (newest first)
  return findings.sort((a, b) => {
    const relevanceOrder = { high: 0, medium: 1, low: 2 }
    if (relevanceOrder[a.relevance] !== relevanceOrder[b.relevance]) {
      return relevanceOrder[a.relevance] - relevanceOrder[b.relevance]
    }
    return new Date(b.date || 0).getTime() - new Date(a.date || 0).getTime()
  })
}
// END MERGE FOR ONLINE RESEARCH FINDINGS

const SAMPLE_DIRECTOR_NAMES = [
  "John Smith",
  "Sarah Johnson",
  "Michael Chen",
  "Emily Brown",
  "David Wilson",
  "Jennifer Lee",
  "Robert Taylor",
  "Lisa Anderson",
  "James Martin",
  "Maria Garcia",
  "William Thompson",
  "Patricia White",
  "Richard Harris",
  "Elizabeth Clark",
  "Thomas Lewis",
]

const MANUFACTURER_NAMES = [
  "Pacific Coast Brewing Co.",
  "Vancouver Island Distillery",
  "Okanagan Wine Estates",
  "Fraser Valley Craft Spirits",
  "Northern BC Brewing",
  "Coastal Cannabis Inc.",
  "Mountain View Producers",
  "Island Spirits Ltd.",
]

function generateApplicationIntelligence(
  licenseType: "liquor" | "cannabis",
  businessName: string,
): ApplicationIntelligence {
  const numDirectors = Math.floor(Math.random() * 4) + 2 // 2-5 directors/officers
  const directorsOfficers: DirectorOfficer[] = []

  const usedNames = new Set<string>()
  for (let i = 0; i < numDirectors; i++) {
    let name = SAMPLE_DIRECTOR_NAMES[Math.floor(Math.random() * SAMPLE_DIRECTOR_NAMES.length)]
    while (usedNames.has(name)) {
      name = SAMPLE_DIRECTOR_NAMES[Math.floor(Math.random() * SAMPLE_DIRECTOR_NAMES.length)]
    }
    usedNames.add(name)

    const role = i === 0 ? "director" : i === 1 ? "officer" : Math.random() > 0.5 ? "shareholder" : "director"

    const screeningStatus = Math.random() > 0.85 ? "flagged" : Math.random() > 0.7 ? "pending" : "cleared"

    const director: DirectorOfficer = {
      name,
      role,
      sharePercentage: role === "shareholder" ? Math.floor(Math.random() * 40) + 10 : undefined,
      securityScreeningStatus: screeningStatus,
      financialIntegrityStatus:
        licenseType === "cannabis"
          ? Math.random() > 0.8
            ? "flagged"
            : Math.random() > 0.6
              ? "pending"
              : "cleared"
          : undefined,
      otherLicenseConnections: Math.random() > 0.7 ? [`LQ-${Math.floor(Math.random() * 900000) + 100000}`] : undefined,
      notes: screeningStatus === "flagged" ? "Requires additional review" : undefined,
    }
    directorsOfficers.push(director)
  }

  // Generate tied house connections (more likely for certain application types)
  const tiedHouseConnections: TiedHouseConnection[] = []
  if (Math.random() > 0.6) {
    const numConnections = Math.floor(Math.random() * 2) + 1
    for (let i = 0; i < numConnections; i++) {
      tiedHouseConnections.push({
        type: licenseType === "cannabis" ? "producer" : Math.random() > 0.5 ? "manufacturer" : "agent",
        entityName: MANUFACTURER_NAMES[Math.floor(Math.random() * MANUFACTURER_NAMES.length)],
        relationshipType: ["financial", "family", "business", "employment"][Math.floor(Math.random() * 4)] as any,
        riskLevel: Math.random() > 0.7 ? "high" : Math.random() > 0.4 ? "medium" : "low",
        details: "Potential connection identified through corporate registry search",
      })
    }
  }

  // Generate fit and proper assessment for cannabis applications
  let fitAndProperAssessment: FitAndProperAssessment | undefined
  if (licenseType === "cannabis") {
    const associatesIdentified = Math.floor(Math.random() * 5) + numDirectors
    const associatesCleared = Math.floor(Math.random() * associatesIdentified)
    const hasIssues = Math.random() > 0.75

    fitAndProperAssessment = {
      status: hasIssues
        ? "concerns"
        : associatesCleared === associatesIdentified
          ? "cleared"
          : associatesCleared > 0
            ? "in-progress"
            : "pending",
      securityScreeningComplete: Math.random() > 0.4,
      financialIntegrityComplete: Math.random() > 0.5,
      associatesIdentified,
      associatesCleared,
      illicitOperationHistory: Math.random() > 0.85,
      criminalRecordFlags: Math.random() > 0.9,
      estimatedCompletionDays: Math.floor(Math.random() * 60) + 14,
      notes: hasIssues ? "Additional documentation requested" : undefined,
    }
  }

  // Generate AI findings
  const aiFindings: ApplicationAIFinding[] = []

  // Check for tied house findings
  if (tiedHouseConnections.length > 0) {
    const highRiskConnection = tiedHouseConnections.find((c) => c.riskLevel === "high")
    aiFindings.push({
      type: "tied-house",
      severity: highRiskConnection ? "high" : "medium",
      title: `Potential Tied House ${highRiskConnection ? "Violation" : "Connection"}`,
      description: `AI detected ${tiedHouseConnections.length} potential connection(s) to ${licenseType === "cannabis" ? "federal producers" : "manufacturers/agents"} that may require review.`,
      source: "Corporate Registry & Business Database",
      recommendation: highRiskConnection
        ? "Immediate review required - applicant must demonstrate no prohibited relationship exists"
        : "Request clarification from applicant regarding business relationships",
    })
  }

  // Check for director/officer findings
  const flaggedDirectors = directorsOfficers.filter((d) => d.securityScreeningStatus === "flagged")
  if (flaggedDirectors.length > 0) {
    aiFindings.push({
      type: "director",
      severity: "high",
      title: "Security Screening Concerns",
      description: `${flaggedDirectors.length} director(s)/officer(s) have been flagged during security screening and require additional review.`,
      source: "Security Programs Division",
      recommendation: "Review flagged individuals before proceeding with application",
    })
  }

  // Check for cross-license connections
  const connectionsFound = directorsOfficers.filter(
    (d) => d.otherLicenseConnections && d.otherLicenseConnections.length > 0,
  )
  if (connectionsFound.length > 0) {
    aiFindings.push({
      type: "director",
      severity: "low",
      title: "Cross-License Connections Identified",
      description: `${connectionsFound.length} individual(s) connected to this application hold interests in other licensed establishments.`,
      source: "LCRB License Database",
      recommendation: "Verify no conflicts of interest or tied house implications",
    })
  }

  // Cannabis-specific findings
  if (licenseType === "cannabis" && fitAndProperAssessment) {
    if (fitAndProperAssessment.illicitOperationHistory) {
      aiFindings.push({
        type: "fit-proper",
        severity: "high",
        title: "Prior Illicit Cannabis Operation",
        description: "Records indicate potential connection to previously unlicensed cannabis retail operation.",
        source: "Community Safety Unit & LCRB Records",
        recommendation:
          "Applicant disclosure required - does not automatically disqualify but requires thorough assessment",
      })
    }

    if (fitAndProperAssessment.criminalRecordFlags) {
      aiFindings.push({
        type: "fit-proper",
        severity: "high",
        title: "Criminal Record Flag",
        description: "Security screening has identified criminal record entries requiring case-by-case review.",
        source: "Criminal Record Check",
        recommendation: "Review with all available information - consider nature of offenses and rehabilitation",
      })
    }

    if (!fitAndProperAssessment.financialIntegrityComplete) {
      aiFindings.push({
        type: "fit-proper",
        severity: "medium",
        title: "Financial Integrity Check Pending",
        description: "Financial integrity documentation incomplete - source of investment funds not fully verified.",
        source: "Financial Integrity Assessment",
        recommendation: "Request completed financial integrity forms and supporting documentation",
      })
    }
  }

  // Location-based findings
  if (Math.random() > 0.7) {
    aiFindings.push({
      type: "location",
      severity: "info",
      title: "High Density Area",
      description: "Proposed location is in an area with multiple existing licensed establishments.",
      source: "LCRB Geographic Analysis",
      recommendation: "Consider local government input and community impact",
    })
  }

  // Corporate structure findings
  const corporateComplexity =
    directorsOfficers.length > 4 || tiedHouseConnections.length > 1
      ? "complex"
      : directorsOfficers.length > 2
        ? "moderate"
        : "simple"

  if (corporateComplexity === "complex") {
    aiFindings.push({
      type: "corporate",
      severity: "medium",
      title: "Complex Corporate Structure",
      description:
        "Application involves multiple shareholders and/or layered corporate entities requiring detailed review.",
      source: "Corporate Registry Analysis",
      recommendation: "Request complete organizational chart and beneficial ownership disclosure",
    })
  }

  const onlineResearchFindings = generateOnlineResearchFindings(businessName, directorsOfficers)

  return {
    directorsOfficers,
    tiedHouseConnections,
    fitAndProperAssessment,
    corporateStructureComplexity: corporateComplexity,
    similarApplicationsInArea: Math.floor(Math.random() * 5),
    aiRiskScore: Math.min(
      100,
      aiFindings.reduce(
        (score, f) =>
          score + (f.severity === "high" ? 25 : f.severity === "medium" ? 15 : f.severity === "low" ? 5 : 0),
        Math.floor(Math.random() * 20),
      ),
    ),
    aiFindings,
    onlineResearchFindings,
  }
}

export const inspectors = [
  // Region 1 - Lower Mainland
  { id: "insp-1", name: "Sarah Chen", email: "sarah.chen@gov.bc.ca", region: "region-1" },
  { id: "insp-2", name: "Michael Park", email: "michael.park@gov.bc.ca", region: "region-1" },
  { id: "insp-3", name: "Emily Zhang", email: "emily.zhang@gov.bc.ca", region: "region-1" },
  // Region 2 - Fraser Valley
  { id: "insp-4", name: "Lisa Thompson", email: "lisa.thompson@gov.bc.ca", region: "region-2" },
  { id: "insp-5", name: "James Wilson", email: "james.wilson@gov.bc.ca", region: "region-2" },
  { id: "insp-6", name: "Amanda Brown", email: "amanda.brown@gov.bc.ca", region: "region-2" },
  // Region 3 - Vancouver Island
  { id: "insp-7", name: "Robert Lee", email: "robert.lee@gov.bc.ca", region: "region-3" },
  { id: "insp-8", name: "Jennifer Wong", email: "jennifer.wong@gov.bc.ca", region: "region-3" },
  { id: "insp-9", name: "Christopher Davis", email: "christopher.davis@gov.bc.ca", region: "region-3" },
  // Region 4 - Interior & Kootenays
  { id: "insp-10", name: "Michelle Scott", email: "michelle.scott@gov.bc.ca", region: "region-4" },
  { id: "insp-11", name: "Daniel Miller", email: "daniel.miller@gov.bc.ca", region: "region-4" },
  { id: "insp-12", name: "Karen Taylor", email: "karen.taylor@gov.bc.ca", region: "region-4" },
  // Region 5 - Northern BC
  { id: "insp-13", name: "David Kumar", email: "david.kumar@gov.bc.ca", region: "region-5" },
  { id: "insp-14", name: "Patricia Anderson", email: "patricia.anderson@gov.bc.ca", region: "region-5" },
  { id: "insp-15", name: "Steven Martinez", email: "steven.martinez@gov.bc.ca", region: "region-5" },
]

function generateLicenses(): License[] {
  const licenses: License[] = []
  const today = new Date()

  const applicationPhases: ApplicationPhase[] = [
    "application-assessment",
    "incomplete-info-requested",
    "pending-lg-in-approval",
    "under-review",
    "approved-in-principle",
  ]

  const applicationTypes: LiquorApplicationType[] = [
    "agent",
    "catering",
    "catering-endorsement",
    "catering-license-relocation",
    "family-food-service",
    "food-primary",
    "food-primary-relocation",
    "liquor-establishment-name-change",
    "liquor-license-transfer",
    "permanent-change-to-licensee",
    "liquor-primary",
    "third-party-operator",
    "manufacturer",
  ]

  // Vancouver-specific liquor applications (10 locations)
  const vancouverCoords = bcCities.find((c) => c.city === "Vancouver") || { lat: 49.2827, lng: -123.1207 }
  const vancouverStreets = [
    "Robson St",
    "Granville St",
    "Davie St",
    "Denman St",
    "Commercial Dr",
    "Main St",
    "Hastings St",
    "Cambie St",
    "Broadway",
    "Burrard St",
  ]
  const vancouverLiquorApplicationNames = [
    "Gastown Spirits Lounge",
    "Yaletown Cocktail Bar",
    "Kitsilano Wine & Dine",
    "Mount Pleasant Taphouse",
    "Coal Harbour Whiskey Bar",
    "Chinatown Brewing Co.",
    "Commercial Drive Cantina",
    "Granville Island Distillery",
    "East Van Craft Beer Hall",
    "Downtown Rooftop Lounge",
  ]

  for (let i = 0; i < 10; i++) {
    const applicationPhase = applicationPhases[Math.floor(Math.random() * applicationPhases.length)]
    const applicationType = applicationTypes[Math.floor(Math.random() * applicationTypes.length)]
    const applicationReceivedDate = generateApplicationReceivedDate()
    const name = vancouverLiquorApplicationNames[i]
    licenses.push({
      id: `vancouver-liquor-app-${i + 1}`,
      businessName: name,
      address: `${100 + i * 25} ${vancouverStreets[i]}`,
      city: "Vancouver",
      licenseNumber: `LQ-APP-VAN-${2024}-${String(i + 1).padStart(4, "0")}`,
      licenseType: "liquor",
      status: "application",
      applicationPhase,
      applicationType,
      applicationReceivedDate,
      applicationIntelligence: generateApplicationIntelligence("liquor", name),
      lat: vancouverCoords.lat + (Math.random() - 0.5) * 0.03,
      lng: vancouverCoords.lng + (Math.random() - 0.5) * 0.03,
      phone: `604-${String(Math.floor(Math.random() * 900) + 100).padStart(3, "0")}-${String(Math.floor(Math.random() * 9000) + 1000).padStart(4, "0")}`,
      email: `apply@${name.toLowerCase().replace(/[^a-z0-9]/g, "")}.ca`,
      website: "",
      contraventions: 0,
      complaints: 0,
      lastInspection: "",
      assignedInspector: "",
      region: "region-1",
    })
  }

  // Vancouver-specific cannabis applications (5 locations)
  const vancouverCannabisApplicationNames = [
    "Hastings Green Dispensary",
    "Main Street Cannabis Collective",
    "Cambie Wellness Centre",
    "Broadway Cannabis Boutique",
    "Davie Village Green Shop",
  ]

  for (let i = 0; i < 5; i++) {
    const applicationPhase = applicationPhases[Math.floor(Math.random() * applicationPhases.length)]
    const applicationReceivedDate = generateApplicationReceivedDate()
    const name = vancouverCannabisApplicationNames[i]
    licenses.push({
      id: `vancouver-cannabis-app-${i + 1}`,
      businessName: name,
      address: `${200 + i * 50} ${vancouverStreets[i + 5]}`,
      city: "Vancouver",
      licenseNumber: `CN-APP-VAN-${2024}-${String(i + 1).padStart(4, "0")}`,
      licenseType: "cannabis",
      status: "application",
      applicationPhase,
      applicationReceivedDate,
      applicationIntelligence: generateApplicationIntelligence("cannabis", name),
      // Cannabis applications don't have liquor application types
      lat: vancouverCoords.lat + (Math.random() - 0.5) * 0.03,
      lng: vancouverCoords.lng + (Math.random() - 0.5) * 0.03,
      phone: `778-${String(Math.floor(Math.random() * 900) + 100).padStart(3, "0")}-${String(Math.floor(Math.random() * 9000) + 1000).padStart(4, "0")}`,
      email: `apply@${name.toLowerCase().replace(/[^a-z0-9]/g, "")}.ca`,
      website: "",
      contraventions: 0,
      complaints: 0,
      lastInspection: "",
      assignedInspector: "",
      region: "region-1",
    })
  }

  // Generate Liquor Licenses
  for (let i = 0; i < 100; i++) {
    const cityData = bcCities[i % bcCities.length]
    const isApplication = i >= 85
    const streetNum = Math.floor(Math.random() * 9000) + 1000
    const streetNames = ["Main St", "Oak St", "Maple Ave", "Cedar Rd", "Pine Blvd", "Government St", "Victoria Ave"]
    const streetName = streetNames[Math.floor(Math.random() * streetNames.length)]
    const size: "small" | "medium" | "large" = i % 3 === 0 ? "large" : i % 2 === 0 ? "medium" : "small"
    const activeStatus: LicenseActiveStatus = Math.random() > 0.1 ? "active" : "suspended"
    const applicationReceivedDate = isApplication ? generateApplicationReceivedDate() : undefined

    const liquorEstablishmentNames = [
      "The Rusty Anchor Pub",
      "Gastown Brewing Co.",
      "Pacific Rim Bar & Grill",
      "Cedar Creek Winery",
      "The Keg Steakhouse",
      "Steamworks Brewing",
      "The Flying Pig",
      "Yaletown Brewing Company",
      "Mahony & Sons",
      "The Irish Heather",
      "Rogue Kitchen & Wetbar",
      "Craft Beer Market",
      "Tap & Barrel",
      "Browns Socialhouse",
      "Cactus Club Cafe",
      "Earls Kitchen + Bar",
      "Joey Restaurant",
      "Moxies Grill & Bar",
      "Original Joe's",
      "Boston Pizza",
      "Red Robin",
      "Milestones",
      "The Keg Manor",
      "White Spot",
      "Denny's Restaurant",
      "IHOP Vancouver",
      "The Old Spaghetti Factory",
      "The Boathouse",
      "Cardero's Restaurant",
      "Lift Bar Grill View",
      "Coal Harbour Bar & Grill",
      "Provence Marinaside",
      "Blue Water Cafe",
      "Coast Restaurant",
      "Glowbal Grill",
      "Italian Kitchen",
      "Tableau Bar Bistro",
      "Hawksworth Restaurant",
      "Boulevard Kitchen",
      "Botanist",
      "Chambar Restaurant",
      "L'Abattoir",
      "Bao Bei Chinese Brasserie",
      "Kissa Tanto",
      "Published on Main",
      "Farmer's Apprentice",
      "Savio Volpe",
      "Ask for Luigi",
      "Nightingale",
      "Pourhouse Vancouver",
      "The Diamond",
      "Guilt & Co",
      "The Alibi Room",
      "The Railway Club",
      "Blarney Stone",
      "Doolin's Irish Pub",
      "Johnnie Fox's",
      "The Cambie",
      "The Granville Room",
      "Celebrities Nightclub",
      "Republic Nightclub",
      "Venue Nightclub",
      "The Roxy",
      "Caprice Nightclub",
      "BC Liquor Store Downtown",
      "Everything Wine",
      "Marquis Wine Cellars",
      "Liberty Wine Merchants",
      "Taylorwood Wines",
      "Kitsilano Wine Cellar",
      "Broadway International Wine",
      "Dunbar Liquor Store",
      "West Point Grey Liquor",
      "Kerrisdale Liquor Store",
      "South Granville Liquor",
      "Commercial Drive Liquor",
      "Main Street Liquor",
      "Hastings Liquor Store",
      "East Van Liquor",
      "Strathcona Beer Company",
      "Storm Brewing",
      "Powell Brewery",
      "Callister Brewing",
      "Luppolo Brewing",
      "Parallel 49 Brewing",
      "Brassneck Brewery",
      "33 Acres Brewing",
      "Faculty Brewing",
      "Strange Fellows Brewing",
      "Superflux Beer Company",
      "Container Brewing",
      "East Van Brewing",
      "Red Truck Beer Company",
      "Stanley Park Brewing",
      "Granville Island Brewing",
      "Bridge Brewing",
      "Dageraad Brewing",
      "Four Winds Brewing",
      "Backcountry Brewing",
      "Yellow Dog Brewing",
      "Twin Sails Brewing",
      "Steel & Oak Brewing",
      "Moody Ales",
      "Parkside Brewery",
      "Trading Post Brewing",
      "Cannery Brewing",
      "Tin Whistle Brewing",
      "Firehall Brewery",
    ]

    const phases = applicationPhases
    const applicationTypes: LiquorApplicationType[] = [
      "agent",
      "catering",
      "catering-endorsement",
      "catering-license-relocation",
      "family-food-service",
      "food-primary",
      "food-primary-relocation",
      "liquor-establishment-name-change",
      "liquor-license-transfer",
      "permanent-change-to-licensee",
      "liquor-primary",
      "third-party-operator",
      "manufacturer",
    ]
    const name = liquorEstablishmentNames[i % liquorEstablishmentNames.length].replace("#", `${i + 1}`)
    const baseLat = cityData.lat

    licenses.push({
      id: `liquor-${i + 1}`,
      businessName: name,
      address: `${streetNum} ${streetName}`,
      city: cityData.city,
      licenseNumber: `LQ-${String(Math.floor(Math.random() * 900000) + 100000)}`,
      licenseType: "liquor",
      status: isApplication ? "application" : "licensed",
      activeStatus: isApplication ? undefined : activeStatus,
      applicationPhase: isApplication ? phases[Math.floor(Math.random() * phases.length)] : undefined,
      applicationType: isApplication
        ? applicationTypes[Math.floor(Math.random() * applicationTypes.length)]
        : undefined,
      applicationReceivedDate,
      applicationIntelligence: generateApplicationIntelligence("liquor", name),
      lat: baseLat + (Math.random() - 0.5) * 0.15,
      lng: cityData.lng + (Math.random() - 0.5) * 0.02,
      phone: `${["604", "250", "778"][Math.floor(Math.random() * 3)]}-${Math.floor(Math.random() * 900) + 100}-${Math.floor(Math.random() * 9000) + 1000}`,
      email: `info@${name.toLowerCase().replace(/[^a-z0-9]/g, "")}.ca`,
      website: Math.random() > 0.3 ? `https://www.${name.toLowerCase().replace(/[^a-z0-9]/g, "")}.ca` : "",
      contraventions: isApplication ? 0 : Math.floor(Math.random() * 5),
      complaints: isApplication ? 0 : Math.floor(Math.random() * 8),
      lastInspection: isApplication
        ? ""
        : `${2024 - Math.floor(Math.random() * 2)}-${String(Math.floor(Math.random() * 12) + 1).padStart(2, "0")}-${String(Math.floor(Math.random() * 28) + 1).padStart(2, "0")}`,
      assignedInspector: isApplication ? "" : inspectors[Math.floor(Math.random() * inspectors.length)].name,
      region: cityToRegion[cityData.city] || "region-1",
      establishmentSize: size,
      yearEstablished: 2000 + Math.floor(Math.random() * 24),
      monthlySales: isApplication ? undefined : generateMonthlySales("liquor", size),
    })
  }

  // Generate Cannabis Licenses
  for (let i = 0; i < 50; i++) {
    const cityData = bcCities[i % bcCities.length]
    const isApplication = i >= 45
    const streetNum = Math.floor(Math.random() * 9000) + 1000
    const streetNames = ["Main St", "Oak St", "Maple Ave", "Cedar Rd", "Pine Blvd", "Government St", "Victoria Ave"]
    const streetName = streetNames[Math.floor(Math.random() * streetNames.length)]
    const size: "small" | "medium" | "large" = i % 3 === 0 ? "large" : i % 2 === 0 ? "medium" : "small"
    const activeStatus: LicenseActiveStatus = Math.random() > 0.1 ? "active" : "suspended"
    const applicationReceivedDate = isApplication ? generateApplicationReceivedDate() : undefined

    const cannabisEstablishmentNames = [
      "Evergreen Cannabis Society",
      "BC Cannabis Store",
      "Hobo Recreational Cannabis",
      "Dutch Love Cannabis",
      "City Cannabis Co.",
      "Kiaro Cannabis",
      "Spiritleaf",
      "Fire & Flower",
      "Tokyo Smoke",
      "Tweed",
      "FOUR20 Premium Market",
      "Canna Cabana",
      "Meta Cannabis Supply Co.",
      "Value Buds",
      "Sessions Cannabis",
      "Greenlight Cannabis",
      "Seed & Stone",
      "Burb Cannabis",
      "Clarity Cannabis",
      "Eden Cannabis",
      "Kush Cannabis",
      "Lotus Land Cannabis",
      "Mountain High Cannabis",
      "Northern Lights Cannabis",
      "Pacific Cannabis Company",
      "Rainforest Cannabis",
      "Sacred Herb",
      "Sanctuary Cannabis",
      "Sunrise Cannabis",
      "The Green Room",
      "The Joint Cannabis",
      "True North Cannabis",
      "Vancouver Cannabis Club",
      "West Coast Cannabis",
      "Wild Flower Cannabis",
      "Zen Leaf Cannabis",
      "Budding Cannabis",
      "Canopy Cannabis",
      "Coastal Cannabis",
      "Emerald Cannabis",
      "Herbal Wellness Cannabis",
      "Island Cannabis",
      "Jade Cannabis",
      "Karma Cannabis",
      "Leaf & Vine Cannabis",
      "Mellow Cannabis",
      "Natural Cannabis Co.",
      "Oasis Cannabis",
      "Pure Cannabis",
      "Quantum Cannabis",
    ]

    const applicationPhase = applicationPhases[Math.floor(Math.random() * applicationPhases.length)]
    const name = cannabisEstablishmentNames[i % cannabisEstablishmentNames.length].replace("#", `${i + 1}`)
    const baseLat = cityData.lat

    licenses.push({
      id: `cannabis-${i + 1}`,
      businessName: name,
      address: `${streetNum} ${streetName}`,
      city: cityData.city,
      licenseNumber: `CN-${String(Math.floor(Math.random() * 900000) + 100000)}`,
      licenseType: "cannabis",
      status: isApplication ? "application" : "licensed",
      activeStatus: isApplication ? undefined : activeStatus,
      applicationPhase: isApplication ? applicationPhase : undefined,
      applicationReceivedDate,
      applicationIntelligence: generateApplicationIntelligence("cannabis", name),
      lat: baseLat + (Math.random() - 0.5) * 0.15,
      lng: cityData.lng + (Math.random() - 0.5) * 0.02,
      phone: `778-${String(Math.floor(Math.random() * 900) + 100).padStart(3, "0")}-${String(Math.floor(Math.random() * 9000) + 1000).padStart(4, "0")}`,
      email: `info@${name.toLowerCase().replace(/[^a-z0-9]/g, "")}.ca`,
      website: Math.random() > 0.3 ? `https://www.${name.toLowerCase().replace(/[^a-z0-9]/g, "")}.ca` : "",
      contraventions: isApplication ? 0 : Math.floor(Math.random() * 3),
      complaints: isApplication ? 0 : Math.floor(Math.random() * 5),
      lastInspection: isApplication
        ? ""
        : `${2024 - Math.floor(Math.random() * 2)}-${String(Math.floor(Math.random() * 12) + 1).padStart(2, "0")}-${String(Math.floor(Math.random() * 28) + 1).padStart(2, "0")}`,
      assignedInspector: isApplication ? "" : inspectors[Math.floor(Math.random() * inspectors.length)].name,
      region: cityToRegion[cityData.city] || "region-1",
      establishmentSize: size,
      yearEstablished: 2000 + Math.floor(Math.random() * 24),
      monthlySales: isApplication ? undefined : generateMonthlySales("cannabis", size),
    })
  }

  // Generate Special Event Permits
  for (let i = 0; i < 50; i++) {
    const cityData = bcCities[i % bcCities.length]
    const region = cityToRegion[cityData.city] || "region-1"
    const eventDate = randomDate(new Date(2024, 0, 1), new Date(2025, 0, 1))
    const expectedAttendees = Math.floor(Math.random() * 4950) + 50 // Random number between 50 and 5000
    licenses.push({
      id: `sep-${i + 1}`,
      businessName: specialEventNames[i],
      address: `${500 + i * 10} ${["Event Plaza", "Festival Way", "Celebration Ave", "Party Lane", "Gala Rd"][i % 5]}`,
      city: cityData.city,
      licenseNumber: `SEP-${2024}-${String(i + 1).padStart(4, "0")}`,
      licenseType: "special-event",
      status: "special-event",
      lat: cityData.lat + (Math.random() - 0.5) * 0.05,
      lng: cityData.lng + (Math.random() - 0.5) * 0.05,
      phone: `604-${String(Math.floor(Math.random() * 900) + 100).padStart(3, "0")}-${String(Math.floor(Math.random() * 9000) + 1000).padStart(4, "0")}`,
      email: `events@${specialEventNames[i].toLowerCase().replace(/[^a-z0-9]/g, "")}.ca`,
      website: "",
      contraventions: 0,
      complaints: 0,
      lastInspection: "",
      assignedInspector: "",
      region,
      eventDate,
      expectedAttendees,
    })
  }

  return licenses
}

export const allLicenses = generateLicenses()

function generateContraventionHistory(count: number): Contravention[] {
  const history: Contravention[] = []
  for (let i = 0; i < count; i++) {
    const date = new Date()
    date.setDate(date.getDate() - Math.floor(Math.random() * 1000))
    history.push({
      id: `cont-${i}`,
      date: date.toISOString().split("T")[0],
      description: contraventionTypes[Math.floor(Math.random() * contraventionTypes.length)],
      severity: ["minor", "major", "critical"][Math.floor(Math.random() * 3)] as Contravention["severity"],
      status: ["resolved", "pending", "appealed"][Math.floor(Math.random() * 3)] as Contravention["status"],
    })
  }
  return history.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
}

function generateComplaintHistory(count: number): Complaint[] {
  const descriptions = [
    "Loud music late at night",
    "Too many people in venue",
    "Suspected underage drinking",
    "Open past licensed hours",
    "Intoxicated patrons causing disturbance",
    "Smoking in prohibited area",
  ]
  const history: Complaint[] = []
  for (let i = 0; i < count; i++) {
    const date = new Date()
    date.setDate(date.getDate() - Math.floor(Math.random() * 500))
    history.push({
      id: `comp-${i}`,
      date: date.toISOString().split("T")[0],
      description: descriptions[Math.floor(Math.random() * descriptions.length)],
      category: complaintCategories[Math.floor(Math.random() * complaintCategories.length)],
      status: ["investigating", "resolved", "unfounded"][Math.floor(Math.random() * 3)] as Complaint["status"],
    })
  }
  return history.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
}

function generateInspectionHistory(): InspectionRecord[] {
  const count = Math.floor(Math.random() * 5) + 1
  const history: InspectionRecord[] = []
  for (let i = 0; i < count; i++) {
    const date = new Date()
    date.setDate(date.getDate() - Math.floor(Math.random() * 730))
    history.push({
      id: `insp-${i}`,
      date: date.toISOString().split("T")[0],
      inspector: inspectors[Math.floor(Math.random() * inspectors.length)].name,
      result: ["pass", "fail", "warning"][Math.floor(Math.random() * 3)] as InspectionRecord["result"],
      notes: "Routine inspection completed.",
    })
  }
  return history.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
}

function generateMonthlySales(licenseType: "liquor" | "cannabis", size: "small" | "medium" | "large"): MonthlySales[] {
  const sales: MonthlySales[] = []
  const today = new Date()

  // Base monthly sales by license type and size (in dollars)
  const baseSales = {
    liquor: { small: 15000, medium: 45000, large: 120000 },
    cannabis: { small: 25000, medium: 75000, large: 180000 },
  }

  const base = baseSales[licenseType][size]

  // Generate last 12 months of sales data
  for (let i = 11; i >= 0; i--) {
    const date = new Date(today.getFullYear(), today.getMonth() - i, 1)
    const month = date.toISOString().slice(0, 7) // "YYYY-MM"

    // Add seasonal variation (higher in summer/holidays)
    const monthNum = date.getMonth()
    let seasonalMultiplier = 1
    if (monthNum >= 5 && monthNum <= 8) seasonalMultiplier = 1.2 // Summer
    if (monthNum === 11 || monthNum === 0) seasonalMultiplier = 1.35 // Holiday season

    // Add random variation (+/- 20%)
    const randomVariation = 0.8 + Math.random() * 0.4

    const amount = Math.round(base * seasonalMultiplier * randomVariation)

    sales.push({ month, amount })
  }

  return sales
}

function generateApplicationReceivedDate(): string {
  const today = new Date()
  const daysAgo = Math.floor(Math.random() * 365) + 1 // 1 to 365 days ago
  const receivedDate = new Date(today.getTime() - daysAgo * 24 * 60 * 60 * 1000)
  return receivedDate.toISOString().split("T")[0]
}
