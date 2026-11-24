import type { CatalogEntry } from "@domain/models/applications"
import { LICENCE_TYPE_LABELS } from "@domain/models/applications"

/** Search metadata for the "Start a new application" catalog. */
const KEYWORDS_BY_TITLE: Record<string, string[]> = {
  // Common authorizations
  "Add, Change or Remove a Licensee Representative": ["rep", "agent", "delegate", "authorize someone", "act on my behalf", "third party", "consultant", "lawyer"],
  "Request Dormancy": ["dormant", "pause licence", "close temporarily", "not operating", "inactive", "hold licence", "mothball"],
  "Indigenous Nation Declaration": ["first nation", "indigenous", "band", "reserve", "aboriginal", "treaty"],
  "Manage Offsite Storage": ["storage", "warehouse", "store stock offsite", "off-site", "inventory location"],
  "Apply to Remove a Tied House Exemption": ["tied house", "manufacturer relationship", "exemption", "brewery ownership"],
  "Report a Corporate or Personnel Change": ["director", "officer", "shareholder", "ownership change", "pcl", "personnel", "corporate", "new partner", "share transfer", "change directors", "add director"],
  "Renew a Licence": ["renew", "renewal", "expiring", "expired", "extend licence", "annual fee", "licence term", "overdue"],
  "Licence Cancellation and Suspension": ["cancel", "cancellation", "give up licence", "surrender", "close business", "suspend"],
  "Temporary Suspension Request": ["suspend", "temporary close", "renovation closure", "pause"],
  "Request of Change in Terms and Conditions / Request for Discretion": ["terms and conditions", "t&c", "discretion", "exception", "special permission", "variance", "waiver", "conditions"],
  "Request for Change to Event Driven Terms and Conditions": ["event terms", "event conditions", "festival conditions"],
  "Temporary Change to a Liquor Licence - Other": ["temporary change", "one-time change", "short term"],
  "Add or Change a Third Party Operator": ["operator", "management company", "run my establishment", "contract operator", "third party"],

  // Patios / areas
  "New Outdoor Patio": ["patio", "deck", "outdoor seating", "sidewalk", "terrace", "outside", "rooftop", "garden", "outdoor area", "expand outside"],
  "New Outdoor Patio (Manufacturer) Application": ["patio", "deck", "outdoor seating", "terrace", "outside", "tasting patio"],
  "Patio / Service Area Change": ["patio", "deck", "outdoor", "service area", "expand", "seating", "capacity"],
  "Temporary Expanded Service Area Application": ["tesa", "temporary patio", "expand service", "temporary area", "extra seating", "event area"],
  "Temporary Extension of Licensed Area": ["extend area", "temporary area", "extra space", "event space", "tent", "parking lot"],
  "Temporary Use Area Endorsement at a Ski Hill or Golf Course": ["ski", "golf", "course", "hill", "tua", "temporary use area", "resort", "outdoor event"],
  "Change to an Approved Picnic Area Application": ["picnic", "outdoor consumption", "winery picnic", "grounds"],

  // Structural
  "Structural Change Application (Capacity Increase)": ["renovation", "renovate", "structural", "floor plan", "capacity", "more seats", "expand", "increase capacity", "construction", "remodel", "layout"],
  "Structural Change Application (No Capacity Increase)": ["renovation", "renovate", "structural", "floor plan", "construction", "remodel", "layout", "same capacity", "no capacity change"],
  "Structural Alterations to an Approved Lounge or Special Event Area (Increase to Capacity)": ["lounge renovation", "structural", "capacity", "floor plan", "expand lounge"],
  "Structural Alterations to an Approved Lounge or Special Event Area (No Increase to Capacity)": ["lounge renovation", "structural", "floor plan", "remodel lounge"],
  "Structural Change to a Production/Storage Area, Sampling Area, or On Site Store": ["structural", "production area", "sampling room", "tasting room", "on site store", "floor plan", "renovation"],
  "Cannabis Retail Store Structural Change Application": ["renovation", "structural", "floor plan", "layout", "remodel", "construction"],
  "Licensee Retail Store Structural Change Application": ["renovation", "structural", "floor plan", "layout", "remodel", "construction"],

  // Hours
  "Change to Hours of Liquor Service (After Midnight)": ["hours", "late", "after midnight", "stay open later", "close later", "extend hours", "2am", "late night"],
  "Change to Hours of Liquor Service (Inside Service Hours)": ["hours", "service hours", "change hours", "opening time", "closing time"],
  "Change to Hours of Liquor Service (Lounge Area)": ["hours", "lounge hours", "lounge"],
  "Change to Hours of Liquor Service (Outside Service Hours)": ["hours", "early", "open earlier", "brunch", "morning service", "before 9"],
  "Change to Hours of Liquor Service (Special Event Area)": ["hours", "event area hours", "special event"],
  "Change to Hours of Sale": ["hours", "store hours", "sale hours", "open later", "open earlier", "retail hours"],
  "Temporary Change to Hours of Liquor Service (NM to Midnight)": ["hours", "temporary hours", "one night", "event hours", "later for an event", "new year", "special occasion"],
  "Temporary Change to Hours of Sale (After Midnight)": ["hours", "temporary hours", "store late", "midnight sale"],
  "Temporary Change to Hours of Sale (Before Midnight)": ["hours", "temporary hours", "extended sale"],
  "Temporary Change to Hours of Sale": ["hours", "temporary hours", "store hours", "one-time"],

  // Endorsements
  "Catering Endorsement Application": ["endorsement", "cater", "catering", "off-site events", "serve at events", "weddings", "banquet"],
  "Lounge Area Endorsement Application": ["endorsement", "lounge", "tasting lounge", "bar area", "serve by the glass", "sit down"],
  "Special Event Area Endorsement Application": ["endorsement", "event area", "festival", "concert", "special event", "sea"],
  "Manufacturer On Site Store Endorsement Application": ["endorsement", "on site store", "retail store", "sell at the winery", "sell at the brewery", "tasting room sales", "shop"],
  "Manufacturer Picnic Area Endorsement Application": ["endorsement", "picnic", "outdoor consumption", "grounds", "lawn"],
  "Patron Participation Entertainment Endorsement Application": ["endorsement", "dancing", "dance floor", "karaoke", "live music", "entertainment", "dj", "patron participation", "trivia"],
  "Temporary Patron Participation Entertainment Endorsement": ["endorsement", "dancing", "karaoke", "one-time entertainment", "temporary entertainment", "party"],

  // Family / minors / venues
  "Application to Allow Family Food Service": ["family", "minors", "kids", "children", "under 19", "family dining", "all ages dining"],
  "Application to Allow Minors in Recreation Facilities": ["minors", "kids", "children", "under 19", "arena", "rec centre", "curling", "bowling", "recreation"],
  "All Ages Liquor Event": ["all ages", "minors", "family event", "concert", "festival", "kids at event"],
  "Live Theatres Requesting Liquor Service": ["theatre", "theater", "performance", "show", "arts", "concert hall", "serve in the auditorium"],
  "Temporary Use Beer Event": ["beer event", "beer festival", "tasting event", "brew fest", "temporary event"],

  // Transfers / relocation / name change
  "Transfer Your Food Primary Licence or Authorization": ["transfer", "sell my business", "sell my restaurant", "new owner", "buy a restaurant", "change of ownership", "assign licence"],
  "Transfer Your Liquor Primary Licence or Authorization": ["transfer", "sell my bar", "sell my pub", "new owner", "buy a bar", "change of ownership", "assign licence"],
  "Transfer Your Cannabis Retail Store Licence": ["transfer", "sell my store", "new owner", "buy a store", "change of ownership"],
  "Transfer Your Licensee Retail Store Licence": ["transfer", "sell my store", "new owner", "buy a store", "change of ownership"],
  "Transfer Your Manufacturer Licence or Authorization": ["transfer", "sell my winery", "sell my brewery", "sell my distillery", "new owner", "change of ownership"],
  "Application to Request Relocation": ["relocate", "relocation", "move", "moving", "new address", "new location", "new premises"],
  "Liquor Primary and Liquor Primary Club Relocation Application": ["relocate", "relocation", "move", "moving", "new address", "new location"],
  "Cannabis Retail Store Relocation Application": ["relocate", "relocation", "move", "moving", "new address", "new location"],
  "Licensee Retail Store Relocation Application": ["relocate", "relocation", "move", "moving", "new address", "new location"],
  "Manufacturer Relocation Application": ["relocate", "relocation", "move", "moving", "new address", "new facility"],
  "Liquor Establishment Name Change Application": ["name change", "rename", "rebrand", "new name", "trade name", "doing business as", "dba", "signage"],
  "Cannabis Establishment Name Change Application": ["name change", "rename", "rebrand", "new name", "trade name", "dba"],

  // New licences
  "Food Primary Licence Application": ["new licence", "restaurant", "cafe", "bistro", "serve liquor with food", "open a restaurant", "food primary", "fp", "start serving alcohol"],
  "Liquor Primary Licence Application": ["new licence", "bar", "pub", "nightclub", "lounge", "tavern", "open a bar", "liquor primary", "lp", "stadium", "hotel bar"],
  "Liquor Primary Club Licence Application": ["club", "legion", "private club", "members only", "society", "liquor primary club"],
  "Cannabis Retail Store Licence Application": ["cannabis", "weed", "marijuana", "dispensary", "pot shop", "open a cannabis store", "crs", "non-medical cannabis"],
  "Licensee Retail Store Application": ["liquor store", "private liquor store", "lrs", "off-sale", "packaged liquor", "sell bottles", "retail liquor", "cold beer and wine"],
  "Rural Licensee Retail Store Application": ["rural", "small town", "rural liquor store", "rural agency", "general store liquor"],
  "Manufacturer Licence Application": ["winery", "brewery", "distillery", "cidery", "meadery", "make wine", "make beer", "make spirits", "produce liquor", "manufacture", "craft"],
  "Deemed Manufacturer Authorization": ["contract brewing", "contract winemaking", "use another facility", "virtual brewery", "deemed", "co-pack", "custom crush"],
  "Agent Licence Application": ["agent", "import", "distributor", "sales rep", "represent a brand", "liquor agent", "wholesale", "marketing"],
  "Catering Licence Application": ["cater", "catering", "caterer", "serve at events", "weddings", "off-site events", "event bar service"],
  "UBrew and UVin (Ferment-on-Premises) Licence Application": ["ubrew", "uvin", "u-brew", "u-vin", "ferment on premises", "make your own wine", "home brew shop", "fop"],

  // Hand-built entries in applications.ts use shorter titles than the factory ones
  "Tied House Exemption": ["tied house", "manufacturer relationship", "exemption", "brewery ownership"],
  "Structural Change": ["renovation", "renovate", "structural", "floor plan", "capacity", "construction", "remodel", "layout"],
  "Catered Event Authorization": ["cater", "catering", "catered event", "off-site event", "wedding", "banquet", "serve at an event"],
  "Temporary Use Area": ["tua", "temporary use area", "ski", "golf", "outdoor event", "temporary area"],
  "Cannabis Retail Store Licence": ["cannabis", "weed", "marijuana", "dispensary", "pot shop", "open a cannabis store", "crs", "non-medical cannabis", "new licence"],
  "Federal Cannabis Reporting": ["federal", "health canada", "ctls", "cannabis tracking", "monthly report", "inventory report", "compliance report"],
  "Licensee Retail Store Licence": ["liquor store", "private liquor store", "lrs", "off-sale", "packaged liquor", "sell bottles", "retail liquor", "cold beer and wine", "new licence"],
  "Manufacturer Licence": ["winery", "brewery", "distillery", "cidery", "meadery", "make wine", "make beer", "make spirits", "produce liquor", "manufacture", "craft", "new licence"],

  // Reporting
  "Monthly federal tracking report": ["federal", "health canada", "ctls", "cannabis tracking", "monthly report", "inventory report", "compliance report"],
}

/** Searchable text for an entry: title + description + licence-type label + keywords. */
const haystackCache = new WeakMap<CatalogEntry, string>()
function haystack(entry: CatalogEntry): string {
  let text = haystackCache.get(entry)
  if (!text) {
    text = [
      entry.title,
      entry.description,
      LICENCE_TYPE_LABELS[entry.licenceType],
      ...(KEYWORDS_BY_TITLE[entry.title] ?? []),
      ...(entry.keywords ?? []),
    ]
      .join(" • ")
      .toLowerCase()
    haystackCache.set(entry, text)
  }
  return text
}

/** Filler words that carry no meaning in a search ("add a patio" → "patio"). */
const STOP_WORDS = new Set([
  "a", "an", "the", "to", "my", "our", "i", "we", "want", "need", "for", "of", "on", "in", "and", "or",
  "apply", "application", "request", "form", "how", "do", "can", "get", "with", "at", "be", "is", "it",
])

function tokens(query: string): string[] {
  const all = query
    .toLowerCase()
    .split(/[^a-z0-9&]+/)
    .filter((t) => t.length > 1)
  const meaningful = all.filter((t) => !STOP_WORDS.has(t))
  // If the user typed ONLY stop words ("application"), fall back to them.
  return meaningful.length > 0 ? meaningful : all
}

/** Word-prefix match: `t` must start a word in `text`. */
function hasWord(text: string, t: string): boolean {
  let i = text.indexOf(t)
  while (i !== -1) {
    if (i === 0 || !/[a-z0-9]/.test(text[i - 1])) return true
    i = text.indexOf(t, i + 1)
  }
  return false
}

/** Score an entry against a query. 0 = no match. */
export function scoreEntry(entry: CatalogEntry, query: string): number {
  const q = query.trim().toLowerCase()
  if (!q) return 1
  const title = entry.title.toLowerCase()
  const keywords = [...(KEYWORDS_BY_TITLE[entry.title] ?? []), ...(entry.keywords ?? [])].map((k) =>
    k.toLowerCase(),
  )
  const description = entry.description.toLowerCase()
  const hay = haystack(entry)

  const terms = tokens(q)
  if (terms.length === 0) return hasWord(hay, q) ? 1 : 0

  let score = 0
  for (const t of terms) {
    if (!hasWord(hay, t)) return 0 // AND: every token must appear somewhere
    if (hasWord(title, t)) score += 10
    else if (keywords.some((k) => hasWord(k, t))) score += 6
    else if (hasWord(description, t)) score += 2
    else score += 1 // licence-type label
  }
  const phrase = terms.join(" ")
  if (hasWord(title, phrase)) score += 30
  else if (keywords.some((k) => k === phrase)) score += 20
  else if (keywords.some((k) => hasWord(k, phrase))) score += 12
  if (title.startsWith(phrase)) score += 10
  return score
}

/** Filter + rank a catalog list against a query (empty query = passthrough). */
export function searchCatalog<T extends CatalogEntry>(list: T[], query: string): T[] {
  const q = query.trim()
  if (!q) return list
  return list
    .map((entry) => ({ entry, score: scoreEntry(entry, q) }))
    .filter((r) => r.score > 0)
    .sort((a, b) => b.score - a.score)
    .map((r) => r.entry)
}

/** The applications licensees start most often, in order. */
export const MOST_USED_TITLES: string[] = [
  "Renew a Licence",
  "Report a Corporate or Personnel Change",
  "New Outdoor Patio",
  "Change to Hours of Liquor Service (After Midnight)",
  "Structural Change Application (No Capacity Increase)",
  "Transfer Your Food Primary Licence or Authorization",
  // Fallbacks when the business doesn't hold the licence type above
  "Transfer Your Liquor Primary Licence or Authorization",
  "Temporary Expanded Service Area Application",
  "Liquor Establishment Name Change Application",
  "Add, Change or Remove a Licensee Representative",
  "Temporary Change to Hours of Liquor Service (NM to Midnight)",
  "Change to Hours of Sale",
]
