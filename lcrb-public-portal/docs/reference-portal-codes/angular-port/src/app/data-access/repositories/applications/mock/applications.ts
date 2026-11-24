import type { FlowConfig } from "@domain/forms/flow-config"
import { EXTENDED_CATALOG } from "./application-catalog-extended"
import type { CatalogEntry, LicenceType } from "@domain/models/applications"
import { GLOSSARY, helpFor } from "./glossary"

/** Catalog of licensee applications and authorizations, organized by licence type. */

const YES_NO = [
  { value: "yes", label: "Yes" },
  { value: "no", label: "No" },
]


const contactSection = {
  title: "Contact Information",
  description: "The person we should contact about this request",
  columns: 2 as const,
  fields: [
    { name: "contactName", label: "Contact Name", type: "text" as const, required: true },
    { name: "contactTitle", label: "Title / Position", type: "text" as const },
    { name: "contactPhone", label: "Phone Number", type: "tel" as const, required: true },
    { name: "contactEmail", label: "Email", type: "email" as const, required: true },
  ],
}


const representative: CatalogEntry = {
  slug: "licensee-representative",
  title: "Add, Change or Remove a Licensee Representative",
  description: "Authorize someone to act on behalf of the licensee, or remove an existing representative.",
  licenceType: "common",
  kind: "authorization",
  config: {
    id: "licensee-representative",
    title: "Licensee Representative",
    subtitle: "Add, change, or remove an authorized representative",
    returnTo: "/applications",
    steps: [
      {
        id: "action",
        title: "Request Type",
        sections: [
          {
            title: "What would you like to do?",
            fields: [
              {
                name: "repAction",
                label: "Select an action",
                type: "radio",
                required: true,
                options: [
                  { value: "add", label: "Add a new representative" },
                  { value: "change", label: "Change an existing representative" },
                  { value: "remove", label: "Remove a representative" },
                ],
              },
              {
                name: "licenceNumber",
                label: "Licence number this applies to",
                type: "text",
                required: true,
                placeholder: "e.g., 326739",
              },
            ],
          },
        ],
      },
      {
        id: "rep",
        title: "Representative Details",
        sections: [
          {
            title: "Representative Information",
            columns: 2,
            fields: [
              { name: "repFirstName", label: "First Name", type: "text", required: true },
              { name: "repLastName", label: "Last Name", type: "text", required: true },
              { name: "repPhone", label: "Phone Number", type: "tel", required: true },
              { name: "repEmail", label: "Email", type: "email", required: true },
              {
                name: "repAuthority",
                label: "Level of authority",
                type: "select",
                required: true,
                options: [
                  { value: "full", label: "Full authority" },
                  { value: "limited", label: "Limited (specified matters only)" },
                ],
              },
            ],
          },
          {
            title: "Consent",
            fields: [
              {
                name: "repConsent",
                label: "The representative has consented to act on behalf of the licensee.",
                type: "checkbox",
                required: true,
              },
            ],
          },
        ],
      },
    ],
  },
}

const dormancy: CatalogEntry = {
  slug: "dormancy",
  title: "Request Dormancy",
  description: "Temporarily suspend operation of a licence while retaining it.",
  licenceType: "common",
  kind: "authorization",
  config: {
    id: "dormancy",
    title: "Dormancy Request",
    subtitle: "Temporarily suspend a licence",
    returnTo: "/applications",
    steps: [
      {
        id: "details",
        title: "Dormancy Details",
        sections: [
          {
            title: "Licence & Period",
            info: {
              variant: "info",
              body: "A licence may be placed in dormancy for up to 24 months. You may not sell or serve under a dormant licence.",
            },
            fields: [
              { name: "licenceNumber", label: "Licence number", type: "text", required: true },
              { name: "dormancyStart", label: "Dormancy start date", type: "date", required: true },
              { name: "dormancyEnd", label: "Expected reactivation date", type: "date", required: true },
              {
                name: "dormancyReason",
                label: "Reason for dormancy",
                type: "textarea",
                required: true,
                placeholder: "Describe why you are requesting dormancy (e.g., renovations, seasonal closure)",
              },
            ],
          },
        ],
      },
    ],
  },
}

const indigenousDeclaration: CatalogEntry = {
  slug: "indigenous-nation-declaration",
  title: "Indigenous Nation Declaration",
  description: "Declare that the applicant is, or is owned by, an Indigenous Nation.",
  licenceType: "common",
  kind: "authorization",
  config: {
    id: "indigenous-nation-declaration",
    title: "Indigenous Nation Declaration",
    subtitle: "Declaration for licence applications",
    returnTo: "/applications",
    steps: [
      {
        id: "declaration",
        title: "Declaration",
        sections: [
          {
            title: "Indigenous Nation",
            fields: [
              { name: "nationName", label: "Name of Indigenous Nation", type: "text", required: true },
              {
                name: "relationship",
                label: "Relationship to the applicant",
                type: "select",
                required: true,
                options: [
                  { value: "is", label: "The applicant is the Indigenous Nation" },
                  { value: "owned", label: "The applicant is owned by the Indigenous Nation" },
                  { value: "partnership", label: "The applicant is in partnership with the Indigenous Nation" },
                ],
              },
              {
                name: "declarationConfirm",
                label: "I declare the above information is true and accurate.",
                type: "checkbox",
                required: true,
              },
            ],
          },
        ],
      },
    ],
  },
}

const offsiteStorage: CatalogEntry = {
  slug: "offsite-storage",
  title: "Manage Offsite Storage",
  description: "Add or update an approved offsite storage location for liquor.",
  licenceType: "common",
  kind: "authorization",
  config: {
    id: "offsite-storage",
    title: "Offsite Storage",
    subtitle: "Add or update an offsite storage location",
    returnTo: "/applications",
    steps: [
      {
        id: "location",
        title: "Storage Location",
        sections: [
          {
            title: "Storage Location Details",
            fields: [
              { name: "licenceNumber", label: "Licence number", type: "text", required: true },
              { name: "storageAddress", label: "Storage address", type: "text", required: true },
              { name: "storageCity", label: "City", type: "text", required: true },
              { name: "storagePostal", label: "Postal Code", type: "text", required: true },
              {
                name: "storageType",
                label: "Type of storage",
                type: "select",
                required: true,
                options: [
                  { value: "warehouse", label: "Warehouse" },
                  { value: "secured-room", label: "Secured room" },
                  { value: "third-party", label: "Third-party facility" },
                ],
              },
            ],
          },
        ],
      },
    ],
  },
}

const tiedHouse: CatalogEntry = {
  slug: "tied-house-exemption",
  title: "Tied House Exemption",
  description: "Request an exemption from tied-house restrictions for a financial association.",
  licenceType: "common",
  kind: "authorization",
  config: {
    id: "tied-house-exemption",
    title: "Tied House Exemption",
    subtitle: "Request a tied-house exemption",
    returnTo: "/applications",
    steps: [
      {
        id: "association",
        title: "Association",
        sections: [
          {
            title: "Association Details",
            info: {
              variant: "warning",
              body: "Tied-house rules restrict financial associations between manufacturers and retailers/hospitality. Provide full details of the association you are seeking an exemption for.",
            },
            fields: [
              { name: "licenceNumber", label: "Your licence number", type: "text", required: true },
              { name: "associatedParty", label: "Associated party (business name)", type: "text", required: true },
              {
                name: "associationType",
                label: "Nature of association",
                type: "textarea",
                required: true,
                placeholder: "Describe the financial or ownership association",
              },
            ],
          },
        ],
      },
    ],
  },
}

const licenceRenewal: CatalogEntry = {
  slug: "licence-renewal",
  title: "Renew a Licence",
  description: "Renew an existing liquor or cannabis licence for the next term.",
  licenceType: "common",
  kind: "application",
  config: {
    id: "licence-renewal",
    title: "Licence Renewal",
    subtitle: "Renew an existing licence for the next term",
    returnTo: "/licences",
    steps: [
      {
        id: "licence",
        title: "Licence",
        sections: [
          {
            title: "Licence to Renew",
            info: {
              variant: "info",
              body: "Confirm the licence you are renewing. Renewal applications should be submitted before the current expiry date to avoid a lapse in your licence.",
            },
            fields: [
              { name: "licenceNumber", label: "Licence number", type: "text", required: true, placeholder: "e.g., 326739" },
              {
                name: "contactEmail",
                label: "Confirmation email",
                type: "email",
                required: true,
                placeholder: "Where we should send your renewal confirmation",
              },
            ],
          },
        ],
      },
      {
        id: "confirm",
        title: "Confirm Details",
        sections: [
          {
            title: "Confirm Licence Information",
            description:
              "Review the business name, address, and ownership shown on this licence, then tell us whether anything has changed.",
            fields: [
              /* A two-way choice replaces the old checkbox, which blocked licensees whose details had changed. */
              {
                name: "infoStatus",
                label: "Are the business name, address, and ownership on this licence still accurate?",
                type: "radio",
                required: true,
                options: [
                  { value: "current", label: "Yes — everything shown is current and correct" },
                  { value: "changed", label: "No — some details have changed since the last term" },
                ],
              },
              {
                name: "changesNote",
                label: "Describe what has changed",
                type: "textarea",
                required: true,
                visibleWhenEquals: { field: "infoStatus", value: "changed" },
                placeholder: "e.g., new business address, a director has left, updated hours",
                hint: "Some changes — such as a transfer of ownership or a change of directors — also need their own application. Listing them here lets a reviewer tell you which ones.",
              },
              {
                name: "complianceConfirm",
                label: "The establishment continues to meet all terms and conditions of the licence.",
                type: "checkbox",
                required: true,
              },
            ],
          },
        ],
      },
      {
        id: "declaration",
        title: "Declaration",
        sections: [
          {
            title: "Declaration",
            fields: [
              {
                name: "renewalDeclaration",
                label: "I declare the information provided is true and complete, and I am authorized to renew this licence.",
                type: "checkbox",
                required: true,
              },
            ],
          },
        ],
      },
    ],
  },
}


/* Food Primary - the main new-licence application. */
const foodPrimaryLicence: CatalogEntry = {
  slug: "food-primary",
  title: "Food Primary Licence Application",
  description: "Apply for a new liquor licence for an establishment whose primary focus is food service.",
  licenceType: "food-primary",
  kind: "application",
  fee: "$950.00",
  config: {
    id: "food-primary",
    title: "Food Primary Licence Application",
    subtitle: "$950 application fee and first-year licensing fee",
    fee: "$950.00",
    returnTo: "/applications",
    steps: [
      {
        id: "before-starting",
        title: "Before Starting",
        description: "Important information about this application",
        sections: [
          {
            title: "Eligibility",
            info: {
              variant: "info",
              title: "Please read carefully",
              body: "Food primary licences are for establishments where the primary focus is the service of food, such as restaurants, bistros, and cafes. Take-out restaurants, mall food fairs, food trucks, and banquet halls without fully equipped kitchens are not eligible.",
            },
            fields: [
              {
                name: "__requirementsAck",
                label: "I have read and understand the eligibility requirements above.",
                type: "checkbox",
                required: true,
                /* This guidance lives on the FIELD, not in a second section's `info` callout. */
                help:
                  'The term "local government" is a defined term in our Act and may also include municipal government, city hall, regional district, local trust, etc. ' +
                  "If you wish to apply for a Patron Participation Entertainment endorsement (including but not limited to dancing, karaoke, or other entertainment involving patron participation in the service area), the local or Indigenous government must be given an opportunity to consider the impact on the community — this takes approximately 90 days.",
              },
            ],
          },
        ],
      },
      {
        id: "applicant",
        title: "Applicant Information",
        description: "Who is applying for this licence",
        sections: [
          {
            title: "Applicant Information",
            columns: 2,
            fields: [
              { name: "firstName", label: "First Name", type: "text", required: true, placeholder: "Enter first name" },
              { name: "lastName", label: "Last Name", type: "text", required: true, placeholder: "Enter last name" },
              {
                name: "email",
                label: "Email",
                type: "email",
                required: true,
                placeholder: "email@example.com",
                validation: { email: true },
              },
              { name: "phone", label: "Phone Number", type: "tel", required: true, placeholder: "(250) 555-1234" },
              { name: "title", label: "Title / Position", type: "text", placeholder: "e.g., Owner, Manager" },
            ],
          },
          {
            title: "Corporation Details",
            description:
              "These come from your BCeID business account and BC Registries record and cannot be changed here. Contact BCeID to correct them.",
            fields: [
              // Auto-filled and locked by ConfigField.
              { name: "legalEntityName", label: "Corporation - Legal Name", type: "text", required: true },
              {
                name: "businessNumber",
                label: "Business Number (9 digit number from the Canada Revenue Agency)",
                type: "text",
                required: true,
              },
              {
                name: "businessType",
                label: "Type of Legal Entity",
                type: "select",
                required: true,
                /* Option values match the rest of the catalog so the canonical BC Registries entity type resolves against them. */
                options: [
                  { value: "corporation", label: "Corporation" },
                  { value: "sole-proprietor", label: "Sole Proprietor" },
                  { value: "partnership", label: "Partnership" },
                  { value: "society", label: "Society" },
                  { value: "cooperative", label: "Cooperative" },
                ],
              },
              { name: "incorporationNumber", label: "Incorporation Number", type: "text" },
            ],
          },
        ],
      },
      {
        id: "establishment",
        title: "Establishment Details",
        description: "About the establishment being licensed",
        sections: [
          {
            title: "Establishment Details",
            fields: [
              {
                name: "establishmentName",
                label: "Establishment Name",
                type: "text",
                required: true,
                placeholder: "e.g., The Blue Ocean Restaurant",
                hint: "The proposed establishment name and signage must be approved by the Branch",
              },
              {
                name: "foodServiceType",
                label: "Type of Food Service",
                type: "select",
                required: true,
                options: [
                  { value: "restaurant", label: "Restaurant" },
                  { value: "cafe", label: "Cafe / Bistro" },
                  { value: "pub", label: "Pub with Food Service" },
                  { value: "hotel-dining", label: "Hotel Dining Room" },
                ],
              },
              {
                name: "businessDescription",
                label: "Describe the nature of your business",
                type: "textarea",
                required: true,
                placeholder:
                  "Describe the type of food you will serve, your target customers, and hours of operation...",
              },
            ],
          },
          {
            title: "Endorsements (optional)",
            fields: [
              {
                name: "ppeEndorsement",
                label: "Patron Participation Entertainment (PPE)",
                type: "checkbox",
                hint: "Allows dancing, karaoke, or other patron participation entertainment. Requires a local government referral of roughly 90 days.",
              },
              {
                name: "cateringEndorsement",
                label: "Catering Endorsement",
                type: "checkbox",
                hint: "Allows you to cater events at other locations",
              },
            ],
          },
          {
            title: "Seating Capacity",
            columns: 2,
            fields: [
              {
                name: "occupantLoad",
                label: "Total Occupant Load",
                type: "number",
                required: true,
                placeholder: "e.g., 100",
                hint: "As per building code. Auto-filled from your floor plan's occupancy stamp when we can read it — please confirm or correct it.",
              },
              {
                name: "serviceAreaCapacity",
                label: "Service Area Capacity",
                type: "number",
                required: true,
                placeholder: "e.g., 75",
                hint: "Maximum patrons in the liquor service area",
              },
            ],
          },
        ],
      },
      {
        id: "location",
        title: "Location",
        description: "Where the establishment is located",
        sections: [
          {
            title: "Establishment Address",
            columns: 2,
            fields: [
              { name: "streetAddress", label: "Street Address", type: "text", required: true, placeholder: "123 Main Street" },
              { name: "city", label: "City", type: "text", required: true, placeholder: "Victoria" },
              {
                name: "postalCode",
                label: "Postal Code",
                type: "text",
                required: true,
                placeholder: "V8W 1A1",
                validation: { postalCode: true },
              },
            ],
          },
        ],
      },
      {
        id: "documents",
        title: "Required Documents",
        description: "Upload the documents required for this application",
        sections: [
          {
            title: "Required Documents",
            info: {
              variant: "info",
              body: "All documents must be current and legible. Files must be PDF or image format and no larger than 25MB each. Each upload is automatically checked against the requirements — the floor plan is read for its occupancy load stamp and corporate documents are confirmed against BC Registries — so you can fix gaps before submitting. These checks are advisory and never block your submission.",
            },
            fields: [
              {
                name: "floorPlan",
                label: "Floor Plan",
                type: "file",
                required: true,
                docType: "floor-plan",
                // Feeds the occupant-load answer above rather than emitting a second occupancy field that would contradict it.
                autoPopulateField: "occupantLoad",
                hint: "Show the proposed service area(s) where liquor will be served, with dimensions, entrances/exits, and seating layout.",
                criteria: [
                  "Shows the proposed liquor service area(s)",
                  "Indicates dimensions and a scale",
                  "Marks entrances and exits",
                  "Shows the seating layout",
                ],
              },
              {
                name: "securitiesRegister",
                label: "Central Securities Register (shareholder register)",
                type: "file",
                required: true,
                docType: "director-info",
                hint: "List all shareholders, the class of shares, and the number of voting and non-voting shares per shareholder.",
              },
              {
                name: "certificateOfIncorporation",
                label: "Certificate of Incorporation",
                type: "file",
                required: true,
                docType: "incorporation",
                hint: "A copy of your BC incorporation certificate",
              },
              {
                name: "noticeOfArticles",
                label: "Notice of Articles",
                type: "file",
                required: true,
                docType: "incorporation",
                hint: "Current Notice of Articles from BC Registry Services",
              },
              {
                name: "proofOfInterest",
                label: "Proof of Valid Interest",
                type: "file",
                required: true,
                docType: "lease",
                hint: "Lease agreement, purchase agreement, or proof of ownership",
              },
              {
                name: "zoningConfirmation",
                label: "Zoning Confirmation",
                type: "file",
                docType: "lg-approval",
                hint: "Letter from local government confirming the property is properly zoned",
              },
            ],
          },
        ],
      },
    ],
  },
}

const fpPatio: CatalogEntry = {
  slug: "food-primary-patio",
  title: "Patio / Service Area Change",
  description: "Add or change an outdoor patio or liquor service area on a Food Primary licence.",
  licenceType: "food-primary",
  kind: "authorization",
  fee: "$330.00",
  config: {
    id: "food-primary-patio",
    title: "Patio / Service Area Change",
    subtitle: "Food Primary licence authorization",
    fee: "$330.00",
    returnTo: "/applications",
    steps: [
      {
        id: "area",
        title: "Service Area",
        sections: [
          {
            title: "Proposed Service Area",
            fields: [
              { name: "licenceNumber", label: "Licence number", type: "text", required: true },
              {
                name: "areaType",
                label: "Service area type",
                type: "select",
                required: true,
                options: [
                  { value: "patio", label: "Outdoor patio" },
                  { value: "indoor", label: "Additional indoor area" },
                  { value: "rooftop", label: "Rooftop" },
                ],
              },
              { name: "areaCapacity", label: "Service area capacity (persons)", type: "number", required: true },
              {
                name: "areaDescription",
                label: "Describe the proposed area",
                type: "textarea",
                required: true,
                placeholder: "Boundaries, fencing, hours of use, etc.",
              },
              {
                name: "localGovApproval",
                label: "Do you have local government approval for this area?",
                type: "radio",
                required: true,
                options: YES_NO,
              },
            ],
          },
        ],
      },
    ],
  },
}

const fpStructural: CatalogEntry = {
  slug: "food-primary-structural-change",
  title: "Structural Change",
  description: "Request approval for structural changes to a Food Primary establishment.",
  licenceType: "food-primary",
  kind: "authorization",
  config: {
    id: "food-primary-structural-change",
    title: "Structural Change",
    subtitle: "Food Primary licence authorization",
    returnTo: "/applications",
    steps: [
      {
        id: "change",
        title: "Proposed Change",
        sections: [
          {
            title: "Structural Change Details",
            fields: [
              { name: "licenceNumber", label: "Licence number", type: "text", required: true },
              {
                name: "changeDescription",
                label: "Describe the structural change",
                type: "textarea",
                required: true,
                placeholder: "e.g., relocating the bar, expanding the dining room",
              },
              { name: "newOccupantLoad", label: "New total occupant load", type: "number", required: true },
              {
                name: "changeImpact",
                label: "Does this change affect the liquor service area?",
                type: "radio",
                required: true,
                options: YES_NO,
              },
            ],
          },
        ],
      },
    ],
  },
}


const lpCateredEvent: CatalogEntry = {
  slug: "liquor-primary-catered-event",
  title: "Catered Event Authorization",
  description: "Authorize liquor service at a catered event under a Liquor Primary (catering) licence.",
  licenceType: "liquor-primary",
  kind: "authorization",
  config: {
    id: "liquor-primary-catered-event",
    title: "Catered Event Authorization",
    subtitle: "Liquor Primary licence authorization",
    returnTo: "/applications",
    steps: [
      {
        id: "event",
        title: "Event Details",
        sections: [
          {
            title: "Event Information",
            fields: [
              { name: "licenceNumber", label: "Catering licence number", type: "text", required: true },
              { name: "eventName", label: "Event name", type: "text", required: true },
              { name: "eventDate", label: "Event date", type: "date", required: true },
              { name: "eventStart", label: "Service start time", type: "time", required: true },
              { name: "eventEnd", label: "Service end time", type: "time", required: true },
              { name: "eventAddress", label: "Event location address", type: "text", required: true },
              { name: "expectedGuests", label: "Expected number of guests", type: "number", required: true },
            ],
            columns: 2,
          },
        ],
      },
      {
        id: "service",
        title: "Liquor Service",
        sections: [
          {
            title: "Service Details",
            fields: [
              {
                name: "liquorTypes",
                label: "Types of liquor to be served",
                type: "checkbox-group",
                options: [
                  { value: "beer", label: "Beer" },
                  { value: "wine", label: "Wine" },
                  { value: "spirits", label: "Spirits" },
                  { value: "coolers", label: "Coolers / Cider" },
                ],
              },
              {
                name: "minorsPresent",
                label: "Will minors be present at the event?",
                type: "radio",
                required: true,
                options: YES_NO,
              },
            ],
          },
        ],
      },
    ],
  },
}

const lpTempUseArea: CatalogEntry = {
  slug: "liquor-primary-temporary-use-area",
  title: "Temporary Use Area",
  description: "Request a temporary extension of the liquor service area (e.g., for a one-time event).",
  licenceType: "liquor-primary",
  kind: "authorization",
  config: {
    id: "liquor-primary-temporary-use-area",
    title: "Temporary Use Area",
    subtitle: "Liquor Primary licence authorization",
    returnTo: "/applications",
    steps: [
      {
        id: "tua",
        title: "Temporary Area",
        sections: [
          {
            title: "Temporary Use Area Details",
            fields: [
              { name: "licenceNumber", label: "Licence number", type: "text", required: true },
              { name: "tuaStart", label: "Start date", type: "date", required: true },
              { name: "tuaEnd", label: "End date", type: "date", required: true },
              { name: "tuaCapacity", label: "Additional capacity (persons)", type: "number", required: true },
              {
                name: "tuaDescription",
                label: "Describe the temporary area and its boundaries",
                type: "textarea",
                required: true,
              },
            ],
          },
        ],
      },
    ],
  },
}


const crsApplication: CatalogEntry = {
  slug: "cannabis-retail-store-application",
  title: "Cannabis Retail Store Licence",
  description: "Apply for a new Cannabis Retail Store licence.",
  licenceType: "cannabis-retail-store",
  kind: "application",
  fee: "$7,500.00",
  config: {
    id: "cannabis-retail-store-application",
    title: "Cannabis Retail Store Licence Application",
    subtitle: "Apply to operate a non-medical cannabis retail store",
    fee: "$7,500.00",
    returnTo: "/applications",
    steps: [
      {
        id: "applicant",
        title: "Applicant",
        sections: [
          {
            title: "Business Information",
            fields: [
              { name: "legalName", label: "Corporation legal name", type: "text", required: true },
              { name: "businessNumber", label: "Business Number (CRA)", type: "text", required: true },
              {
                name: "businessType",
                label: "Business type",
                type: "select",
                required: true,
                options: [
                  { value: "corporation", label: "Corporation" },
                  { value: "sole-prop", label: "Sole Proprietorship" },
                  { value: "partnership", label: "Partnership" },
                  { value: "indigenous-nation", label: "Indigenous Nation" },
                ],
              },
            ],
          },
          contactSection,
        ],
      },
      {
        id: "store",
        title: "Store",
        sections: [
          {
            title: "Proposed Store",
            fields: [
              { name: "storeName", label: "Proposed store name", type: "text", required: true },
              { name: "storeAddress", label: "Street address", type: "text", required: true },
              { name: "storeCity", label: "City", type: "text", required: true },
              { name: "storePostal", label: "Postal Code", type: "text", required: true },
              {
                name: "localGovSupport",
                label: "Have you received local government / Indigenous Nation support?",
                type: "radio",
                required: true,
                options: YES_NO,
                hint: "Cannabis retail applications require local government or Indigenous Nation input.",
              },
            ],
            columns: 2,
          },
        ],
      },
      {
        id: "security",
        title: "Security",
        sections: [
          {
            title: "Security Screening",
            info: {
              variant: "info",
              body: "All associates of a cannabis retail applicant must complete security screening. You can manage screening requests from the Security Screening section.",
            },
            fields: [
              {
                name: "associatesIdentified",
                label: "Have all associates been identified for security screening?",
                type: "radio",
                required: true,
                options: YES_NO,
                // "associate" and "security screening" tripped testers up.
                help: `${helpFor("associate")} ${GLOSSARY.securityScreening.definition}`,
              },
              {
                name: "numAssociates",
                label: "Number of associates",
                type: "number",
                required: true,
                help: helpFor("associate"),
              },
            ],
          },
          {
            title: "Financial Integrity",
            fields: [
              {
                name: "fundingSourcesDisclosed",
                label: "Can you document the source of all start-up funding?",
                type: "radio",
                required: true,
                options: YES_NO,
                help: helpFor("financialIntegrity"),
                hint: "You will be asked for supporting records (bank statements, loan agreements) during review.",
              },
            ],
          },
        ],
      },
    ],
  },
}

const crsFederalReporting: CatalogEntry = {
  slug: "cannabis-federal-reporting",
  title: "Federal Cannabis Reporting",
  description: "Submit the monthly federal cannabis tracking report for a retail store.",
  licenceType: "cannabis-retail-store",
  kind: "authorization",
  config: {
    id: "cannabis-federal-reporting",
    title: "Federal Cannabis Reporting",
    subtitle: "Monthly federal tracking report",
    returnTo: "/applications",
    steps: [
      {
        id: "period",
        title: "Reporting Period",
        sections: [
          {
            title: "Report Details",
            info: {
              variant: "info",
              body: "Cannabis retailers must report monthly to Health Canada's Cannabis Tracking and Licensing System. Enter the figures for the reporting period.",
            },
            fields: [
              { name: "licenceNumber", label: "Licence number", type: "text", required: true },
              {
                name: "reportMonth",
                label: "Reporting month",
                type: "select",
                required: true,
                options: [
                  { value: "2026-01", label: "January 2026" },
                  { value: "2026-02", label: "February 2026" },
                  { value: "2026-03", label: "March 2026" },
                  { value: "2026-04", label: "April 2026" },
                  { value: "2026-05", label: "May 2026" },
                ],
              },
              { name: "openingInventory", label: "Opening inventory (kg)", type: "number", required: true },
              { name: "unitsSold", label: "Units sold", type: "number", required: true },
              { name: "closingInventory", label: "Closing inventory (kg)", type: "number", required: true },
            ],
            columns: 2,
          },
        ],
      },
    ],
  },
}


const lrsApplication: CatalogEntry = {
  slug: "licensee-retail-store-application",
  title: "Licensee Retail Store Licence",
  description: "Apply for a Licensee Retail Store (LRS) to sell packaged liquor.",
  licenceType: "licensee-retail-store",
  kind: "application",
  fee: "$220.00",
  config: {
    id: "licensee-retail-store-application",
    title: "Licensee Retail Store Application",
    subtitle: "Apply to sell packaged liquor for off-site consumption",
    fee: "$220.00",
    returnTo: "/applications",
    steps: [
      {
        id: "applicant",
        title: "Applicant",
        sections: [
          {
            title: "Business Information",
            fields: [
              { name: "legalName", label: "Corporation legal name", type: "text", required: true },
              { name: "businessNumber", label: "Business Number (CRA)", type: "text", required: true },
            ],
          },
          contactSection,
        ],
      },
      {
        id: "store",
        title: "Store",
        sections: [
          {
            title: "Store Location",
            fields: [
              { name: "storeName", label: "Store name", type: "text", required: true },
              { name: "storeAddress", label: "Street address", type: "text", required: true },
              { name: "storeFloorArea", label: "Retail floor area (sq. ft.)", type: "number", required: true },
            ],
            columns: 2,
          },
        ],
      },
    ],
  },
}

const manufacturerApplication: CatalogEntry = {
  slug: "manufacturer-application",
  title: "Manufacturer Licence",
  description: "Apply for a liquor Manufacturer licence (brewery, winery, distillery).",
  licenceType: "manufacturer",
  kind: "application",
  fee: "$550.00",
  config: {
    id: "manufacturer-application",
    title: "Manufacturer Licence Application",
    subtitle: "Apply to manufacture liquor in British Columbia",
    fee: "$550.00",
    returnTo: "/applications",
    steps: [
      {
        id: "applicant",
        title: "Applicant",
        sections: [
          {
            title: "Business Information",
            fields: [
              { name: "legalName", label: "Corporation legal name", type: "text", required: true },
              { name: "businessNumber", label: "Business Number (CRA)", type: "text", required: true },
              {
                name: "manufacturerType",
                label: "Type of manufacturer",
                type: "select",
                required: true,
                options: [
                  { value: "brewery", label: "Brewery" },
                  { value: "winery", label: "Winery" },
                  { value: "distillery", label: "Distillery" },
                  { value: "cidery", label: "Cidery" },
                ],
              },
            ],
          },
          contactSection,
        ],
      },
      {
        id: "facility",
        title: "Facility",
        sections: [
          {
            title: "Manufacturing Facility",
            fields: [
              { name: "facilityAddress", label: "Facility address", type: "text", required: true },
              { name: "annualProduction", label: "Estimated annual production (litres)", type: "number", required: true },
              {
                name: "onSiteStore",
                label: "Will you operate an on-site store?",
                type: "radio",
                required: true,
                options: YES_NO,
              },
              {
                name: "lounge",
                label: "Will you operate a lounge or special event area?",
                type: "radio",
                required: true,
                options: YES_NO,
              },
            ],
          },
        ],
      },
    ],
  },
}

const deemedManufacturer: CatalogEntry = {
  slug: "deemed-manufacturer-authorization",
  title: "Deemed Manufacturer Authorization",
  description: "Request deemed-manufacturer status to produce under another manufacturer's facility.",
  licenceType: "deemed-manufacturer",
  kind: "authorization",
  config: {
    id: "deemed-manufacturer-authorization",
    title: "Deemed Manufacturer Authorization",
    subtitle: "Produce liquor under an existing manufacturer's facility",
    returnTo: "/applications",
    steps: [
      {
        id: "arrangement",
        title: "Arrangement",
        sections: [
          {
            title: "Production Arrangement",
            info: {
              variant: "info",
              body: "A deemed manufacturer produces liquor at a licensed manufacturer's facility. Identify the host facility and the products to be produced.",
            },
            fields: [
              { name: "hostManufacturer", label: "Host manufacturer (business name)", type: "text", required: true },
              { name: "hostLicenceNumber", label: "Host manufacturer licence number", type: "text", required: true },
              {
                name: "productTypes",
                label: "Products to be produced",
                type: "checkbox-group",
                options: [
                  { value: "beer", label: "Beer" },
                  { value: "wine", label: "Wine" },
                  { value: "spirits", label: "Spirits" },
                  { value: "cider", label: "Cider" },
                ],
              },
              { name: "estimatedVolume", label: "Estimated annual volume (litres)", type: "number", required: true },
            ],
          },
        ],
      },
    ],
  },
}


/* Stop-gap channel for PCL changes while the Legal Entity Builder is still being built out. */
const pclChange: CatalogEntry = {
  slug: "pcl-change",
  title: "Report a Corporate or Personnel Change",
  description:
    "Report a change of directors, shareholders, officers, or a tied-house relationship to Personnel & Corporate Licensing.",
  licenceType: "common",
  kind: "application",
  config: {
    id: "pcl-change",
    title: "Corporate or Personnel Change",
    subtitle: "Personnel & Corporate Licensing (PCL) request",
    returnTo: "/applications",
    steps: [
      {
        id: "request",
        title: "Request",
        sections: [
          {
            title: "What are you reporting?",
            info: {
              variant: "info",
              body: "Licensees must notify the LCRB when the people or companies behind a licence change. Report the change here; if you also want to map your full ownership structure visually, use the Legal Entity Builder under Business Structure.",
            },
            fields: [
              {
                name: "requestType",
                label: "Type of change",
                type: "radio",
                required: true,
                options: [
                  { value: "directors", label: "Change of directors" },
                  { value: "shareholders", label: "Change of shareholders or ownership interest" },
                  { value: "officers", label: "Change of officers or senior management" },
                  { value: "tied-house", label: "New or changed tied-house relationship" },
                  { value: "other", label: "Another corporate or personnel change" },
                ],
              },
              {
                name: "effectiveDate",
                label: "Date the change took effect",
                type: "date",
                required: true,
                hint: "If the change has not happened yet, enter the date it is expected to take effect.",
              },
              {
                name: "contactEmail",
                label: "Confirmation email",
                type: "email",
                required: true,
                placeholder: "Where we should send updates about this request",
              },
            ],
          },
        ],
      },
      {
        id: "details",
        title: "Details",
        sections: [
          {
            title: "Change Details",
            description: "Tell us who is affected so PCL can screen the people involved and update the licence record.",
            fields: [
              {
                name: "directorChanges",
                label: "Directors appointed and/or resigned",
                type: "textarea",
                required: true,
                visibleWhenEquals: { field: "requestType", value: "directors" },
                placeholder:
                  "e.g., Appointed: Priya Raman (2026-03-01). Resigned: J. Okonjo (2026-02-15).",
                hint: "Include the full legal name of each person and whether they were appointed or resigned.",
              },
              {
                name: "shareholderChanges",
                label: "Shareholders and ownership percentages after the change",
                type: "textarea",
                required: true,
                visibleWhenEquals: { field: "requestType", value: "shareholders" },
                placeholder: "e.g., Harbour Holdings Ltd. 60%, Priya Raman 40%",
                hint: "List every shareholder with an interest of 10% or more, including any that did not change.",
              },
              {
                name: "officerChanges",
                label: "Officers or senior management affected",
                type: "textarea",
                required: true,
                visibleWhenEquals: { field: "requestType", value: "officers" },
                placeholder: "e.g., New General Manager: Dana Whitmore, effective 2026-03-01",
              },
              {
                name: "tiedHouseParties",
                label: "Parties to the tied-house relationship and its nature",
                type: "textarea",
                required: true,
                visibleWhenEquals: { field: "requestType", value: "tied-house" },
                placeholder:
                  "e.g., Harbour Hospitality Ltd. holds a 15% interest in Coastal Brewing Co., a licensed manufacturer.",
                hint: "Reporting the relationship is not the same as being permitted to hold it. If you need an exemption from the tied-house restrictions, also submit a Tied House Exemption application.",
              },
              {
                name: "otherRequest",
                label: "Describe the change",
                type: "textarea",
                required: true,
                visibleWhenEquals: { field: "requestType", value: "other" },
                placeholder: "Tell us what changed and who is affected.",
              },
              /* Newly appointed people must consent to security screening, so this only applies to the request types that add individuals. */
              {
                name: "screeningConsent",
                label:
                  "Each newly appointed individual has been advised they must complete a personal history and consent to security screening.",
                type: "checkbox",
                required: true,
                visibleWhenEquals: { field: "requestType", value: ["directors", "shareholders", "officers"] },
              },
              {
                name: "supportingDocuments",
                label: "Supporting documents (optional)",
                type: "file",
                hint: "For example a central securities register, notice of change of directors, or share transfer agreement.",
              },
            ],
          },
        ],
      },
      {
        id: "declaration",
        title: "Declaration",
        sections: [
          {
            title: "Declaration",
            fields: [
              {
                name: "pclDeclaration",
                label:
                  "I declare the information provided is true and complete, and I am authorized to report this change on behalf of the licensee.",
                type: "checkbox",
                required: true,
              },
            ],
          },
        ],
      },
    ],
  },
}


export const APPLICATION_CATALOG: CatalogEntry[] = [
  representative,
  dormancy,
  indigenousDeclaration,
  offsiteStorage,
  tiedHouse,
  pclChange,
  licenceRenewal,
  fpPatio,
  fpStructural,
  lpCateredEvent,
  lpTempUseArea,
  crsApplication,
  crsFederalReporting,
  lrsApplication,
  manufacturerApplication,
  deemedManufacturer,
  foodPrimaryLicence,
  // Full factory-built catalog (per-licence-type applications & authorizations)
  ...EXTENDED_CATALOG,
]

export function getCatalogEntry(slug: string): CatalogEntry | undefined {
  return APPLICATION_CATALOG.find((e) => e.slug === slug)
}

export function getEntriesByType(type: LicenceType): CatalogEntry[] {
  return APPLICATION_CATALOG.filter((e) => e.licenceType === type)
}
