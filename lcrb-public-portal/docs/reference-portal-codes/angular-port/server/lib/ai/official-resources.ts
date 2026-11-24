/** Catalog of official gov.bc.ca LCRB pages so the assistant can cite them beyond RESOURCE_LINKS. Prototype stand-in for the Knowledge Retrieval layer. */

const BASE =
  "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing"

export type ResourceCategory =
  | "apply-liquor"
  | "apply-cannabis"
  | "amend"
  | "special-event"
  | "endorsement"
  | "resource"
  | "cannabis-resource"
  | "enforcement"
  | "law-policy"
  | "local-government"
  | "certification"
  | "bulletin"
  | "about"
  | "locations"

export interface OfficialResource {
  label: string
  /** Path relative to the LCRB content base (joined with BASE). */
  path: string
  category: ResourceCategory
  /** Short plain-language description of what the page covers. */
  summary: string
  /** Lower-cased keywords/phrases used for matching user questions. */
  keywords: string[]
}

/** The catalog as relative paths; `url()` joins with BASE. */
export const OFFICIAL_RESOURCES: OfficialResource[] = [
  {
    label: "Apply for a Liquor Primary Licence",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-a-liquor-primary-licence",
    category: "apply-liquor",
    summary: "Licence for bars, pubs, nightclubs, lounges and other venues whose primary focus is serving liquor.",
    keywords: ["liquor primary", "bar", "pub", "nightclub", "lounge", "tasting room", "open a bar", "lp licence"],
  },
  {
    label: "Apply for a Food Primary Licence",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-a-food-primary-licence",
    category: "apply-liquor",
    summary: "Licence for restaurants where food service is the primary focus and liquor is served alongside meals.",
    keywords: ["food primary", "restaurant", "cafe", "serve liquor with food", "fp licence", "diner"],
  },
  {
    label: "Apply for a Manufacturer Licence",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-a-manufacturer-licence",
    category: "apply-liquor",
    summary: "Licence for breweries, wineries, distilleries and cideries that manufacture liquor in BC.",
    keywords: ["manufacturer", "brewery", "winery", "distillery", "cidery", "make beer", "make wine", "make spirits", "produce liquor"],
  },
  {
    label: "Apply for a Catering Licence",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-a-catering-licence",
    category: "apply-liquor",
    summary: "Licence for caterers to serve liquor at events they cater at various locations.",
    keywords: ["catering", "caterer", "cater an event", "off-site catering"],
  },
  {
    label: "Apply for an Agent Licence",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-an-agent-licence",
    category: "apply-liquor",
    summary: "Licence for agents who market and represent liquor products to licensees on behalf of manufacturers.",
    keywords: ["agent licence", "represent products", "marketing agent", "liquor agent"],
  },
  {
    label: "Apply for a Rural Licensee Retail Store Licence",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-a-rural-licensee-retail-store-licence",
    category: "apply-liquor",
    summary: "Licence for a retail liquor store in a rural area without adequate existing service.",
    keywords: ["rural licensee retail store", "rural store", "lrs", "rural liquor store"],
  },
  {
    label: "Apply for a UBrew or UVin Licence",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-a-ubrew-or-uvin-licence",
    category: "apply-liquor",
    summary: "Licence for businesses that provide facilities for customers to ferment their own beer or wine.",
    keywords: ["ubrew", "uvin", "u-brew", "u-vin", "ferment on premises", "brew your own", "make your own wine"],
  },
  {
    label: "Apply for an Ethyl Alcohol Permit",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-an-ethyl-alcohol-permit",
    category: "apply-liquor",
    summary: "Permit to purchase and use ethyl alcohol (pure alcohol) for approved industrial or commercial purposes.",
    keywords: ["ethyl alcohol", "pure alcohol", "ethanol permit", "industrial alcohol"],
  },
  {
    label: "Non-Beverage Alcohol Exemption",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/non-beverage-alcohol-exemption",
    category: "apply-liquor",
    summary: "Exemption for products containing alcohol that are not intended to be consumed as a beverage.",
    keywords: ["non-beverage alcohol", "exemption", "not a beverage", "flavouring", "extract"],
  },
  {
    label: "Liquor Charitable Auction Permit",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/liquor-charitable-auction-permit",
    category: "apply-liquor",
    summary: "Permit allowing registered charities to auction liquor to raise funds.",
    keywords: ["charitable auction", "charity auction", "auction liquor", "fundraiser auction"],
  },
  {
    label: "Market Liquor Sales (Manufacturer)",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/market-liquor-sales",
    category: "apply-liquor",
    summary: "Authorization for manufacturers to sell their liquor at approved public markets such as farmers' markets.",
    keywords: ["market liquor sales", "farmers market", "public market", "sell at market", "market authorization"],
  },
  {
    label: "Wine Store Licence",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/wine-store",
    category: "apply-liquor",
    summary: "Retail licence for stores that sell BC and imported wine.",
    keywords: ["wine store", "vqa store", "sell wine retail"],
  },
  {
    label: "Special Wine Store Licence",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/special-wine-store",
    category: "apply-liquor",
    summary: "Retail licence for a special wine store, typically for BC VQA wines.",
    keywords: ["special wine store", "vqa wine store"],
  },
  {
    label: "Licensee Retail Store Licence",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/licensee-retail-store",
    category: "apply-liquor",
    summary: "Retail licence for a private liquor store (LRS).",
    keywords: ["licensee retail store", "private liquor store", "lrs licence", "cold beer and wine store"],
  },
  {
    label: "Apply for a Liquor Licence or Permit (overview)",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit",
    category: "apply-liquor",
    summary: "Overview of all liquor licence and permit types and how to apply.",
    keywords: ["apply for liquor licence", "types of liquor licence", "which liquor licence", "liquor permit"],
  },

  {
    label: "Apply for a Liquor Special Event Permit (SEP)",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-liquor-special-event-permit",
    category: "special-event",
    summary: "Permit to serve liquor at a one-time event such as a wedding, fundraiser or festival.",
    keywords: ["special event permit", "sep", "serve liquor at event", "wedding", "festival", "fundraiser", "one-time event"],
  },
  {
    label: "Determine if you need a Special Event Permit",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-liquor-special-event-permit/determine-if-you-need-a-liquor-special-event-permit",
    category: "special-event",
    summary: "Guidance on whether your event requires a Special Event Permit.",
    keywords: ["do i need a sep", "need a special event permit", "sep required", "private residence event"],
  },
  {
    label: "Prepare and apply for a Special Event Permit",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-liquor-special-event-permit/prepare-and-apply-for-a-special-event-permit",
    category: "special-event",
    summary: "How to prepare and submit a Special Event Permit application.",
    keywords: ["prepare sep", "apply for special event permit", "sep application steps"],
  },
  {
    label: "Special Event Permit — Before the event",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-liquor-special-event-permit/before-event",
    category: "special-event",
    summary: "What permit holders must do before their event.",
    keywords: ["before the event", "sep before event", "event preparation"],
  },
  {
    label: "Special Event Permit — During the event",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-liquor-special-event-permit/during-the-event",
    category: "special-event",
    summary: "Responsibilities of the permit holder while the event is running.",
    keywords: ["during the event", "sep during event", "event day rules"],
  },
  {
    label: "Special Event Permit — After your event",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-liquor-special-event-permit/after-your-event",
    category: "special-event",
    summary: "What to do after your event, including reporting requirements.",
    keywords: ["after the event", "sep after event", "post-event"],
  },
  {
    label: "Withdraw a Special Event Permit",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-liquor-special-event-permit/withdraw-permit",
    category: "special-event",
    summary: "How to withdraw or cancel a Special Event Permit application or permit.",
    keywords: ["withdraw permit", "cancel sep", "cancel special event permit"],
  },
  {
    label: "Special Event Permit resources",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-liquor-special-event-permit/special-event-permit-resources",
    category: "special-event",
    summary: "Reference materials and resources for Special Event Permit holders.",
    keywords: ["sep resources", "special event permit resources"],
  },
  {
    label: "Special Event Permit — Get help",
    path: "/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-liquor-special-event-permit/get-help",
    category: "special-event",
    summary: "Where to get help with a Special Event Permit.",
    keywords: ["sep help", "special event permit help", "sep contact"],
  },

  {
    label: "Amend a Liquor Licence (overview)",
    path: "/liquor-licence-permits/amend-liquor-licence",
    category: "amend",
    summary: "Overview of how to change, renew, relocate, or transfer an existing liquor licence.",
    keywords: ["amend liquor licence", "change my licence", "modify licence"],
  },
  {
    label: "Change a Liquor Licence",
    path: "/liquor-licence-permits/amend-liquor-licence/change-a-liquor-licence",
    category: "amend",
    summary: "Make changes to an existing liquor licence, such as hours, capacity, or structural changes.",
    keywords: ["change a liquor licence", "change hours", "change capacity", "structural change", "amend hours"],
  },
  {
    label: "Renew a Liquor Licence",
    path: "/liquor-licence-permits/amend-liquor-licence/renew-a-liquor-licence",
    category: "amend",
    summary: "How and when to renew an existing liquor licence and pay renewal fees.",
    keywords: ["renew liquor licence", "renewal", "renew my licence", "renewal fee", "expiry"],
  },
  {
    label: "Relocate a Liquor Licence",
    path: "/liquor-licence-permits/amend-liquor-licence/relocate-a-liquor-licence",
    category: "amend",
    summary: "Move a liquor licence to a new physical location.",
    keywords: ["relocate liquor licence", "move licence", "new location", "change address"],
  },
  {
    label: "Transfer a Liquor Licence",
    path: "/liquor-licence-permits/amend-liquor-licence/transfer-a-liquor-licence",
    category: "amend",
    summary: "Transfer a liquor licence to a new owner or business entity.",
    keywords: ["transfer liquor licence", "sell business", "new owner", "change ownership", "transfer ownership"],
  },
  {
    label: "TESA Authorization (Temporary Expanded Service Area)",
    path: "/liquor-licence-permits/amend-liquor-licence/change-a-liquor-licence/tesa-authorization",
    category: "amend",
    summary: "Authorize a temporary expanded service area, such as an expanded patio.",
    keywords: ["tesa", "temporary expanded service area", "expand patio", "expanded service area", "temporary patio", "patio", "expand my patio", "outdoor service area"],
  },
  {
    label: "Appoint a Liquor Licence Representative",
    path: "/liquor-licence-permits/amend-liquor-licence/appoint-a-liquor-licence-representative",
    category: "amend",
    summary: "Appoint a representative authorized to act on behalf of the licensee.",
    keywords: ["appoint representative", "licence representative", "authorized representative", "delegate authority"],
  },

  {
    label: "Lounge Endorsement",
    path: "/liquor-licence-permits/liquor-resources-information/lounge-endorsement",
    category: "endorsement",
    summary: "Adds a lounge or special event area to a manufacturer licence for on-site liquor service.",
    keywords: ["lounge endorsement", "manufacturer lounge", "tasting lounge"],
  },
  {
    label: "Special Event Area Endorsement",
    path: "/liquor-licence-permits/liquor-resources-information/special-event-area-endorsement",
    category: "endorsement",
    summary: "Adds a special event area to a manufacturer licence for hosting events.",
    keywords: ["special event area endorsement", "manufacturer event area", "sea endorsement"],
  },
  {
    label: "Picnic Area Endorsement",
    path: "/liquor-licence-permits/liquor-resources-information/picnic-area-endorsement",
    category: "endorsement",
    summary: "Adds an outdoor picnic area to a manufacturer site for sampling and sales.",
    keywords: ["picnic area endorsement", "outdoor picnic area", "manufacturer picnic"],
  },
  {
    label: "On-Site Store Endorsement",
    path: "/liquor-licence-permits/liquor-resources-information/on-site-store-endorsement",
    category: "endorsement",
    summary: "Allows a manufacturer to operate an on-site store selling its own and other products.",
    keywords: ["on-site store endorsement", "manufacturer store", "on site store", "sell on site"],
  },

  {
    label: "Liquor Resources & Information (overview)",
    path: "/liquor-licence-permits/liquor-resources-information",
    category: "resource",
    summary: "Hub of resources, guidance and tools for liquor licensees.",
    keywords: ["liquor resources", "licensee resources", "liquor information"],
  },
  {
    label: "BC Liquor Licensing Forms",
    path: "/liquor-licence-permits/liquor-resources-information/bc-liquor-licensing-forms",
    category: "resource",
    summary: "Downloadable forms for liquor licence applications and changes.",
    keywords: ["forms", "application form", "pdf form", "download form", "licensing forms"],
  },
  {
    label: "LCRB FAQ",
    path: "/liquor-licence-permits/liquor-resources-information/lcrb-faq",
    category: "resource",
    summary: "Frequently asked questions about liquor licensing.",
    keywords: ["faq", "frequently asked questions", "common questions"],
  },
  {
    label: "Liquor Licensee and Applicant Screening",
    path: "/liquor-licence-permits/liquor-resources-information/liquor-licensee-and-applicant-screening",
    category: "resource",
    summary: "How financial integrity and background screening of applicants and associates works.",
    keywords: ["screening", "background check", "financial integrity", "associate", "due diligence", "applicant screening"],
  },
  {
    label: "Liquor Advertising & Online Promotions",
    path: "/liquor-licence-permits/liquor-resources-information/liquor-advertising-online-promotions",
    category: "resource",
    summary: "Rules for advertising liquor and running promotions, including online.",
    keywords: ["advertising", "promotion", "online promotion", "marketing rules", "advertise liquor", "happy hour"],
  },
  {
    label: "Incident Log (Liquor)",
    path: "/liquor-licence-permits/liquor-resources-information/incident-log-liquor",
    category: "resource",
    summary: "Requirements for keeping an incident log at a liquor establishment.",
    keywords: ["incident log", "log incidents", "record incidents", "liquor incident"],
  },
  {
    label: "Patron Participation Entertainment",
    path: "/liquor-licence-permits/liquor-resources-information/patron-participation-entertainment",
    category: "resource",
    summary: "Rules for patron participation entertainment such as karaoke and contests.",
    keywords: ["patron participation", "entertainment", "karaoke", "contest", "wet t-shirt", "live entertainment"],
  },
  {
    label: "Cannabis Information for Liquor Licensees",
    path: "/liquor-licence-permits/liquor-resources-information/cannabis-information-liquor-licensees",
    category: "resource",
    summary: "How cannabis rules affect liquor licensees.",
    keywords: ["cannabis for liquor licensees", "cannabis and liquor", "cannabis in bar"],
  },
  {
    label: "Liquor Licensee Signs & Posters",
    path: "/liquor-licence-permits/liquor-resources-information/liquor-licensee-signs-posters",
    category: "resource",
    summary: "Required and available signage and posters for liquor establishments.",
    keywords: ["signs", "posters", "signage", "required signs", "display poster"],
  },
  {
    label: "BC Liquor Licensed Establishment Locations",
    path: "/liquor-licence-permits/liquor-resources-information/bc-liquor-licensed-establishment-locations",
    category: "locations",
    summary: "Directory/map of licensed liquor establishments in BC.",
    keywords: ["licensed establishment locations", "find a licensed establishment", "liquor location list"],
  },

  {
    label: "Serving It Right (SIR)",
    path: "/liquor-licence-permits/liquor-resources-information/serving-it-right",
    category: "certification",
    summary: "Mandatory responsible beverage service certification for licensees, managers and servers.",
    keywords: ["serving it right", "sir", "server certificate", "responsible service", "server certification", "bartender certificate"],
  },
  {
    label: "Special Event Server (SES) Certification",
    path: "/liquor-licence-permits/liquor-resources-information/special-event-serve",
    category: "certification",
    summary: "Certification for people serving liquor at special events.",
    keywords: ["special event server", "ses", "special event serve", "event server certificate"],
  },
  {
    label: "Selling It Right (Cannabis)",
    path: "/cannabis-licences/cannabis-resources-information/selling-it-right",
    category: "certification",
    summary: "Mandatory certification for cannabis retail store workers.",
    keywords: ["selling it right", "cannabis certification", "cannabis worker certificate", "cannabis retail training"],
  },

  {
    label: "Liquor Law & Policy (overview)",
    path: "/liquor-licence-permits/liquor-law-policy",
    category: "law-policy",
    summary: "Overview of liquor legislation, regulations, policy directives and manuals.",
    keywords: ["liquor law", "liquor policy", "legislation overview"],
  },
  {
    label: "Liquor Licence Terms & Conditions Manuals",
    path: "/liquor-licence-permits/liquor-law-policy/liquor-licence-terms-conditions-manuals",
    category: "law-policy",
    summary: "The terms and conditions handbooks for each type of liquor licence.",
    keywords: ["terms and conditions", "terms & conditions", "handbook", "manual", "licence handbook"],
  },
  {
    label: "Liquor Legislation & Regulations",
    path: "/liquor-licence-permits/liquor-law-policy/liquor-legislation-regulations",
    category: "law-policy",
    summary: "The Liquor Control and Licensing Act and its regulations.",
    keywords: ["legislation", "regulation", "act", "liquor control and licensing act", "lcla", "the law"],
  },
  {
    label: "Liquor Policy Directives",
    path: "/liquor-licence-permits/liquor-law-policy/liquor-policy-directives",
    category: "law-policy",
    summary: "Official policy directives that clarify how liquor rules are applied.",
    keywords: ["policy directive", "directives", "policy change"],
  },

  {
    label: "Inspections and Enforcement (overview)",
    path: "/inspections-and-enforcement",
    category: "enforcement",
    summary: "How the LCRB inspects establishments and enforces liquor and cannabis rules.",
    keywords: ["inspections", "enforcement", "compliance", "inspector", "contravention"],
  },
  {
    label: "Preventing Contraventions",
    path: "/inspections-and-enforcement/preventing-contraventions",
    category: "enforcement",
    summary: "Guidance to help licensees avoid contraventions.",
    keywords: ["preventing contraventions", "avoid violation", "stay compliant", "compliance tips"],
  },
  {
    label: "Preparing for Inspections",
    path: "/inspections-and-enforcement/preparing-for-inspections",
    category: "enforcement",
    summary: "How to prepare for an LCRB inspection.",
    keywords: ["preparing for inspection", "inspection checklist", "get ready for inspection"],
  },
  {
    label: "Minors as Agents Program",
    path: "/inspections-and-enforcement/minors-as-agents-program",
    category: "enforcement",
    summary: "The program that uses minors to test whether establishments check ID.",
    keywords: ["minors as agents", "map program", "minor agent", "id check test", "sting"],
  },
  {
    label: "Enforcement Action",
    path: "/inspections-and-enforcement/enforcement-action",
    category: "enforcement",
    summary: "What happens when a contravention leads to enforcement action.",
    keywords: ["enforcement action", "penalty", "fine", "suspension", "notice of enforcement", "monetary penalty"],
  },
  {
    label: "Liquor & Cannabis Seizures",
    path: "/inspections-and-enforcement/enforcement-action/liquor-cannabis-seizures",
    category: "enforcement",
    summary: "When and how liquor or cannabis can be seized.",
    keywords: ["seizure", "seized", "confiscate", "seize product"],
  },
  {
    label: "Hearings & Decisions",
    path: "/inspections-and-enforcement/hearings-decisions",
    category: "enforcement",
    summary: "Overview of the enforcement hearing process and published decisions.",
    keywords: ["hearing", "hearings", "decisions", "adjudication"],
  },
  {
    label: "Enforcement Hearing Process",
    path: "/inspections-and-enforcement/hearings-decisions/enforcement-hearing-process",
    category: "enforcement",
    summary: "Step-by-step explanation of how an enforcement hearing works.",
    keywords: ["hearing process", "how a hearing works", "enforcement hearing"],
  },
  {
    label: "Hearing Decisions and Penalties",
    path: "/inspections-and-enforcement/hearings-decisions/hearing-decisions-and-penalties",
    category: "enforcement",
    summary: "How penalties are decided and the range of possible penalties.",
    keywords: ["penalties", "penalty schedule", "decision", "fine amount", "suspension length"],
  },
  {
    label: "Enforcement Order Reconsideration",
    path: "/inspections-and-enforcement/hearings-decisions/enforcement-order-reconsideration",
    category: "enforcement",
    summary: "How to request reconsideration of an enforcement order.",
    keywords: ["reconsideration", "appeal", "reconsider order", "challenge decision"],
  },
  {
    label: "BC Liquor Enforcement Hearing Search",
    path: "/inspections-and-enforcement/hearings-decisions/bc-liquor-enforcement-hearing-search",
    category: "enforcement",
    summary: "Search past enforcement hearing decisions.",
    keywords: ["hearing search", "search decisions", "past decisions", "decision database"],
  },
  {
    label: "Submit a Complaint to LCRB",
    path: "/inspections-and-enforcement/preparing-for-inspections/submit-complaint-to-lcrb",
    category: "enforcement",
    summary: "How members of the public or licensees can submit a complaint.",
    keywords: ["submit complaint", "make a complaint", "report a problem", "report establishment", "complaint"],
  },

  {
    label: "Cannabis Licences (overview)",
    path: "/cannabis-licences",
    category: "apply-cannabis",
    summary: "Overview of cannabis retail, marketing and producer retail licensing in BC.",
    keywords: ["cannabis licence", "cannabis licensing", "cannabis overview"],
  },
  {
    label: "Apply for a Cannabis Retail Store Licence",
    path: "/cannabis-licences/apply-cannabis-licence/apply-for-a-cannabis-retail-store-licence",
    category: "apply-cannabis",
    summary: "Licence to operate a non-medical cannabis retail store.",
    keywords: ["cannabis retail store", "cannabis shop", "open a cannabis store", "crs", "dispensary", "weed store"],
  },
  {
    label: "Apply for a Producer Retail Store Licence",
    path: "/cannabis-licences/apply-cannabis-licence/apply-for-a-producer-retail-store-licence",
    category: "apply-cannabis",
    summary: "Licence for a federally licensed cannabis producer to operate a retail store at its production site.",
    keywords: ["producer retail store", "prs", "producer store", "on-site cannabis store"],
  },
  {
    label: "Apply for a Cannabis Marketing Licence",
    path: "/cannabis-licences/apply-cannabis-licence/apply-for-a-cannabis-marketing-licence",
    category: "apply-cannabis",
    summary: "Licence for cannabis marketers who represent producers to retailers.",
    keywords: ["cannabis marketing licence", "cannabis marketer", "market cannabis"],
  },
  {
    label: "Apply for a Cannabis Licence (overview)",
    path: "/cannabis-licences/apply-cannabis-licence",
    category: "apply-cannabis",
    summary: "Overview of how to apply for the different cannabis licence types.",
    keywords: ["apply for cannabis licence", "which cannabis licence", "cannabis application"],
  },
  {
    label: "Amend a Cannabis Licence Authorization (overview)",
    path: "/cannabis-licences/amend-cannabis-licence-authorization",
    category: "amend",
    summary: "Overview of changing, renewing or transferring a cannabis licence.",
    keywords: ["amend cannabis licence", "change cannabis licence"],
  },
  {
    label: "Renew a Cannabis Licence",
    path: "/cannabis-licences/amend-cannabis-licence-authorization/renew-cannabis-licence",
    category: "amend",
    summary: "How and when to renew a cannabis licence.",
    keywords: ["renew cannabis licence", "cannabis renewal", "cannabis renew"],
  },
  {
    label: "Change a Cannabis Marketing Licence",
    path: "/cannabis-licences/amend-cannabis-licence-authorization/change-a-cannabis-marketing-licence",
    category: "amend",
    summary: "Make changes to a cannabis marketing licence.",
    keywords: ["change cannabis marketing licence", "amend marketing licence"],
  },
  {
    label: "Transfer a Cannabis Retail Store Licence",
    path: "/cannabis-licences/amend-cannabis-licence-authorization/transfer-a-cannabis-retail-store-licence",
    category: "amend",
    summary: "Transfer a cannabis retail store licence to a new owner.",
    keywords: ["transfer cannabis licence", "sell cannabis store", "cannabis new owner", "cannabis ownership change"],
  },

  {
    label: "Cannabis Resources & Information (overview)",
    path: "/cannabis-licences/cannabis-resources-information",
    category: "cannabis-resource",
    summary: "Hub of resources and guidance for cannabis licensees.",
    keywords: ["cannabis resources", "cannabis information", "cannabis guidance"],
  },
  {
    label: "Cannabis Associate Forms",
    path: "/cannabis-licences/cannabis-resources-information/associate-forms",
    category: "cannabis-resource",
    summary: "Forms for declaring and screening cannabis licence associates.",
    keywords: ["associate forms", "cannabis associate", "cannabis forms", "associate declaration"],
  },
  {
    label: "Cannabis Advertising & Promotion",
    path: "/cannabis-licences/cannabis-resources-information/cannabis-advertising-promotion",
    category: "cannabis-resource",
    summary: "Rules for advertising and promoting cannabis and cannabis stores.",
    keywords: ["cannabis advertising", "cannabis promotion", "advertise cannabis", "cannabis marketing rules"],
  },
  {
    label: "Cannabis at a Special Event",
    path: "/cannabis-licences/cannabis-resources-information/cannabis-at-special-event",
    category: "cannabis-resource",
    summary: "Rules for cannabis at special events.",
    keywords: ["cannabis at special event", "cannabis event", "cannabis festival"],
  },
  {
    label: "Incident Record (Cannabis Licensees)",
    path: "/cannabis-licences/cannabis-resources-information/incident-record-cannabis-licensees",
    category: "cannabis-resource",
    summary: "Incident recording requirements for cannabis retailers.",
    keywords: ["cannabis incident record", "cannabis incident log", "record cannabis incident"],
  },
  {
    label: "Delivering Cannabis",
    path: "/cannabis-licences/cannabis-resources-information/delivering-cannabis",
    category: "cannabis-resource",
    summary: "Rules for delivering cannabis to customers.",
    keywords: ["delivering cannabis", "cannabis delivery", "deliver cannabis", "online cannabis order"],
  },
  {
    label: "Cannabis Store Record Keeping",
    path: "/cannabis-licences/cannabis-resources-information/cannabis-store-record-keeping",
    category: "cannabis-resource",
    summary: "Record-keeping requirements for cannabis retail stores.",
    keywords: ["cannabis record keeping", "cannabis records", "store records", "cannabis bookkeeping"],
  },
  {
    label: "Cannabis Signs & Posters",
    path: "/cannabis-licences/cannabis-resources-information/cannabis-signs-posters",
    category: "cannabis-resource",
    summary: "Required signage and posters for cannabis retail stores.",
    keywords: ["cannabis signs", "cannabis posters", "cannabis signage"],
  },
  {
    label: "Cannabis Store Locations & Buy Legal",
    path: "/cannabis-licences/cannabis-store-locations-and-buy-legal-information",
    category: "locations",
    summary: "Find legal cannabis stores and information about buying legal cannabis.",
    keywords: ["cannabis store locations", "find cannabis store", "buy legal cannabis", "legal cannabis"],
  },

  {
    label: "Cannabis Law & Policy (overview)",
    path: "/cannabis-licences/law-policy-cannabis-licence-authorization",
    category: "law-policy",
    summary: "Overview of cannabis legislation, regulations and terms & conditions.",
    keywords: ["cannabis law", "cannabis policy", "cannabis legislation overview"],
  },
  {
    label: "Cannabis Terms & Conditions",
    path: "/cannabis-licences/law-policy-cannabis-licence-authorization/cannabis-terms-conditions",
    category: "law-policy",
    summary: "The terms and conditions handbook for cannabis retail licences.",
    keywords: ["cannabis terms and conditions", "cannabis handbook", "cannabis manual"],
  },
  {
    label: "Cannabis Legislation & Regulations",
    path: "/cannabis-licences/law-policy-cannabis-licence-authorization/cannabis-legislation-regulations",
    category: "law-policy",
    summary: "The Cannabis Control and Licensing Act and its regulations.",
    keywords: ["cannabis legislation", "cannabis regulation", "cannabis control and licensing act", "cannabis act"],
  },

  {
    label: "Local & Indigenous Government (overview)",
    path: "/local-gov-indigenous-nations",
    category: "local-government",
    summary: "How local and Indigenous governments participate in liquor and cannabis licensing.",
    keywords: ["local government", "indigenous nations", "first nations", "municipal", "local input"],
  },
  {
    label: "Role of Local Governments & First Nations in Liquor Licensing",
    path: "/local-gov-indigenous-nations/role-of-local-governments-and-first-nations-in-liquor-licensing",
    category: "local-government",
    summary: "The role local governments and First Nations play in reviewing liquor applications.",
    keywords: ["local government liquor", "first nations liquor", "local government comment", "municipal recommendation"],
  },
  {
    label: "Role of Local Governments & Indigenous Nations in Cannabis Retail Licensing",
    path: "/local-gov-indigenous-nations/role-of-local-governments-and-indigenous-nations-in-cannabis-retail-licensing",
    category: "local-government",
    summary: "The role local and Indigenous governments play in cannabis retail licensing.",
    keywords: ["local government cannabis", "indigenous cannabis", "cannabis local input", "cannabis recommendation"],
  },

  {
    label: "About the LCRB",
    path: "/about-lcrb",
    category: "about",
    summary: "About the Liquor and Cannabis Regulation Branch.",
    keywords: ["about lcrb", "who is lcrb", "what is lcrb", "about the branch"],
  },
  {
    label: "Contact the LCRB",
    path: "/about-lcrb/contact-lcrb",
    category: "about",
    summary: "Phone, email and mailing details for the LCRB.",
    keywords: ["contact", "phone", "email", "get in touch", "reach lcrb", "contact lcrb"],
  },
  {
    label: "LCRB Bulletins",
    path: "/about-lcrb/bulletins",
    category: "about",
    summary: "Official bulletins announcing policy and operational changes.",
    keywords: ["bulletins", "bulletin", "announcements", "policy updates", "news"],
  },
  {
    label: "Liquor Licensing is Moving Online",
    path: "/about-lcrb/liquor-licensing-is-moving-online",
    category: "about",
    summary: "Information about the transition of liquor licensing services online.",
    keywords: ["moving online", "online portal", "digital services", "new system"],
  },
  {
    label: "Liquor Appeal / Liquor Panel",
    path: "/about-lcrb/liquor-panel",
    category: "about",
    summary: "Information about the liquor appeal panel.",
    keywords: ["liquor panel", "appeal panel", "panel"],
  },
  {
    label: "LCRB Social Responsibility",
    path: "/about-lcrb/lcrb-social-responsibility",
    category: "about",
    summary: "The LCRB's social responsibility mandate and programs.",
    keywords: ["social responsibility", "responsible", "public safety mandate"],
  },
]

/** Individual bulletins in compact form, expanded into full OfficialResource records so they take part in matching. */
const BULLETINS: { slug: string; title: string; keywords: string[] }[] = [
  { slug: "bulletin-25-01-liquor-primary-licences-may-offer-bottle-service", title: "Bulletin 25-01: Liquor Primary Licences May Offer Bottle Service", keywords: ["bottle service", "bulletin 25-01", "25-01"] },
  { slug: "bulletin-25-02-liquor-primary-licences-outdoor-event-venues", title: "Bulletin 25-02: Liquor Primary Licences at Outdoor Event Venues", keywords: ["outdoor event venue", "bulletin 25-02", "25-02"] },
  { slug: "bulletin-25-03-extension-direct-delivery-from-off-site-storage", title: "Bulletin 25-03: Extension of Direct Delivery from Off-Site Storage", keywords: ["direct delivery", "off-site storage", "bulletin 25-03", "25-03"] },
  { slug: "bulletin-24-01-promoting-cannabis-friendly-spaces-and-consuming-cannabis-on-public-patios", title: "Bulletin 24-01: Cannabis-Friendly Spaces & Cannabis on Public Patios", keywords: ["cannabis friendly", "cannabis patio", "bulletin 24-01", "24-01"] },
  { slug: "bulletin-24-02-sales-in-a-manufacturer-picnic-area", title: "Bulletin 24-02: Sales in a Manufacturer Picnic Area", keywords: ["manufacturer picnic area sales", "bulletin 24-02", "24-02"] },
  { slug: "bulletin-24-03-update-to-packaged-sales-from-a-manufacturer-on-site-store", title: "Bulletin 24-03: Packaged Sales from a Manufacturer On-Site Store", keywords: ["packaged sales", "on-site store sales", "bulletin 24-03", "24-03"] },
  { slug: "bulletin-24-04-update-to-guided-tours-and-sampling-at-a-manufacturer-site", title: "Bulletin 24-04: Guided Tours and Sampling at a Manufacturer Site", keywords: ["guided tours", "sampling", "bulletin 24-04", "24-04"] },
  { slug: "bulletin-24-05-agents-and-liquor-clubs", title: "Bulletin 24-05: Agents and Liquor Clubs", keywords: ["agents", "liquor clubs", "bulletin 24-05", "24-05"] },
  { slug: "bulletin-24-06-use-of-plastic-containers-in-stadiums", title: "Bulletin 24-06: Use of Plastic Containers in Stadiums", keywords: ["plastic containers", "stadiums", "bulletin 24-06", "24-06"] },
  { slug: "bulletin-24-07-update-to-cannabis-industry-sampling", title: "Bulletin 24-07: Cannabis Industry Sampling", keywords: ["cannabis sampling", "bulletin 24-07", "24-07"] },
  { slug: "bulletin-24-08-temporary-relocation-of-lrs-licence", title: "Bulletin 24-08: Temporary Relocation of an LRS Licence", keywords: ["temporary relocation", "lrs relocation", "bulletin 24-08", "24-08"] },
  { slug: "bulletin-24-09-eligible-market-requirements-for-market-authorizations", title: "Bulletin 24-09: Eligible Market Requirements for Market Authorizations", keywords: ["market authorization", "eligible market", "bulletin 24-09", "24-09"] },
  { slug: "bulletin-24-10-updates-to-liquor-delivery-requirements", title: "Bulletin 24-10: Updates to Liquor Delivery Requirements", keywords: ["liquor delivery", "delivery requirements", "bulletin 24-10", "24-10"] },
  { slug: "bulletin-23-01-extension-of-tesas-to-dec-31-2024", title: "Bulletin 23-01: Extension of TESAs", keywords: ["tesa extension", "bulletin 23-01", "23-01"] },
  { slug: "bulletin-23-02-change-to-possession-limit-for-cannabis-beverages", title: "Bulletin 23-02: Possession Limit for Cannabis Beverages", keywords: ["cannabis beverage possession", "possession limit", "bulletin 23-02", "23-02"] },
  { slug: "bulletin-23-03-structural-change-application-requirements", title: "Bulletin 23-03: Structural Change Application Requirements", keywords: ["structural change", "bulletin 23-03", "23-03"] },
  { slug: "bulletin-23-04-repeal-cannabis-visibility-rules", title: "Bulletin 23-04: Repeal of Cannabis Visibility Rules", keywords: ["cannabis visibility", "bulletin 23-04", "23-04"] },
  { slug: "bulletin-23-05-extension-direct-delivery-from-off-site-storage", title: "Bulletin 23-05: Extension of Direct Delivery from Off-Site Storage", keywords: ["direct delivery extension", "bulletin 23-05", "23-05"] },
  { slug: "bulletin-23-06-outdoor-sampling-areas-for-manufacturers", title: "Bulletin 23-06: Outdoor Sampling Areas for Manufacturers", keywords: ["outdoor sampling", "bulletin 23-06", "23-06"] },
  { slug: "bulletin-23-07-updates-to-hearing-rules", title: "Bulletin 23-07: Updates to Hearing Rules", keywords: ["hearing rules", "bulletin 23-07", "23-07"] },
  { slug: "bulletin-23-08-cannabis-industry-sampling", title: "Bulletin 23-08: Cannabis Industry Sampling", keywords: ["cannabis sampling", "bulletin 23-08", "23-08"] },
  { slug: "bulletin-23-09-physical-separation-between-liquor-retailer-and-other-businesses", title: "Bulletin 23-09: Physical Separation Between Liquor Retailer and Other Businesses", keywords: ["physical separation", "bulletin 23-09", "23-09"] },
  { slug: "bulletin-23-10-accompanying-minors-in-liquor-stores-and-liquor-primary-establishments", title: "Bulletin 23-10: Accompanying Minors in Liquor Stores and Liquor Primary Establishments", keywords: ["minors", "accompanying minors", "bulletin 23-10", "23-10"] },
  { slug: "bulletin-23-11-increase-in-liquor-transfers-between-licences", title: "Bulletin 23-11: Increase in Liquor Transfers Between Licences", keywords: ["liquor transfers", "transfer between licences", "bulletin 23-11", "23-11"] },
  { slug: "bulletin-23-12-clarification-of-renewal-fee-calculations", title: "Bulletin 23-12: Clarification of Renewal Fee Calculations", keywords: ["renewal fee calculation", "bulletin 23-12", "23-12"] },
]

for (const b of BULLETINS) {
  OFFICIAL_RESOURCES.push({
    label: b.title,
    path: `/about-lcrb/bulletins/${b.slug}`,
    category: "bulletin",
    summary: "Official LCRB policy bulletin.",
    keywords: [...b.keywords, "bulletin"],
  })
}

/** Resolve a resource's full gov.bc.ca URL. */
export function url(resource: OfficialResource): string {
  return `${BASE}${resource.path}`
}


const CATEGORY_HINTS: { re: RegExp; category: ResourceCategory; weight: number }[] = [
  { re: /\bbulletin\b|\b2[35]-\d\d\b/, category: "bulletin", weight: 2 },
  { re: /transfer|relocat|renew|amend|change.*licence|licence.*change/, category: "amend", weight: 1.5 },
  { re: /enforce|contravention|inspect|penalt|hearing|seizure|complaint/, category: "enforcement", weight: 1.5 },
  { re: /special event|\bsep\b/, category: "special-event", weight: 1.5 },
  { re: /cannabis/, category: "apply-cannabis", weight: 1 },
  { re: /endorsement|lounge|picnic|on-site store/, category: "endorsement", weight: 1.5 },
  { re: /serving it right|selling it right|certif|\bsir\b|\bses\b/, category: "certification", weight: 2 },
]

export interface OfficialResourceMatch {
  label: string
  url: string
  summary: string
  category: ResourceCategory
  score: number
}

/** Score official resources against a free-text query and return the best matches for citation. */
export function matchOfficialResources(query: string, max = 4): OfficialResourceMatch[] {
  const q = ` ${query.toLowerCase()} `
  if (q.trim().length === 0) return []

  // Category boosts inferred from the query.
  const catBoost = new Map<ResourceCategory, number>()
  for (const hint of CATEGORY_HINTS) {
    if (hint.re.test(q)) catBoost.set(hint.category, (catBoost.get(hint.category) ?? 0) + hint.weight)
  }

  const scored: OfficialResourceMatch[] = []
  for (const r of OFFICIAL_RESOURCES) {
    let score = 0
    for (const kw of r.keywords) {
      // Phrase match (word-boundary-ish) scores higher for longer phrases.
      if (q.includes(` ${kw} `) || q.includes(`${kw} `) || q.includes(` ${kw}`)) {
        score += 1 + kw.length / 12
      }
    }
    // Also reward matches on words in the label.
    const labelWords = r.label.toLowerCase().split(/[^a-z0-9]+/).filter((w) => w.length > 3)
    for (const w of labelWords) {
      if (q.includes(` ${w} `)) score += 0.5
    }
    if (score <= 0) continue
    score += catBoost.get(r.category) ?? 0
    scored.push({ label: r.label, url: url(r), summary: r.summary, category: r.category, score })
  }

  scored.sort((a, b) => b.score - a.score)
  return scored.slice(0, max)
}
