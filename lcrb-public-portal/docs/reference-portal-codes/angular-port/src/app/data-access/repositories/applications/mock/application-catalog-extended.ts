import { buildEntry, customSteps, endorsementSteps, hoursChangeSteps, nameChangeSteps, patioSteps, relocationSteps, structuralChangeSteps, temporaryExtensionSteps, temporarySuspensionSteps, thirdPartyOperatorSteps, tiedHouseRemovalSteps, termsChangeSteps, transferSteps, afterDate, YES_NO } from "./application-factory"
import { type CatalogEntry, type LicenceType } from "@domain/models/applications"

/** Extended application catalog by licence type, built from the shared flow factory; bespoke flows stay in `applications.ts`. */

/* A few shared document sets reused across applications. */
const floorPlanDoc = {
  name: "floorPlan",
  label: "Floor Plan",
  required: true,
  docType: "floor-plan" as const,
  // Read the occupancy stamp and auto-populate a companion occupancy-load field.
  occupancyField: true,
  hint: "A scaled floor plan showing the affected area, including entrances, service areas and capacity.",
  criteria: [
    "Is a floor plan / architectural drawing of the establishment (not a photo, menu, or unrelated document).",
    "Is drawn to scale, or clearly notes the scale or key dimensions.",
    "Shows the boundaries of the licensed/affected area, including any patio or service area.",
    "Identifies entrances, exits and the location of bars or service areas.",
    "Indicates seating layout or occupant/seating capacity.",
    "Includes a current occupancy load stamp (dated within the last 12 months) showing the occupant load for each space.",
  ],
}
/** Director / corporate registry document - extracted + confirmed against BC Registries. */
const corporateRegistryDoc = {
  name: "corporateRegistry",
  label: "Directors / Central Securities Register",
  required: true,
  docType: "director-info" as const,
  hint: "A current directors list or Central Securities Register for the BC-registered company applying for or receiving the licence.",
  criteria: [
    "Identifies the company legal name and its BC incorporation/registration number.",
    "Lists the current directors (and officers, if applicable) of the company.",
    "Is current/recent and legible.",
  ],
}
const sitePlanDoc = {
  name: "sitePlan",
  label: "Site Plan",
  required: true,
  hint: "A site plan showing the property boundaries, building footprint and the proposed licensed area.",
  criteria: [
    "Shows the overall property/parcel boundaries and the building footprint.",
    "Identifies the proposed licensed area within the site.",
    "Shows adjacent streets, parking and property lines for context.",
    "Includes a north arrow or scale/dimensions.",
  ],
}
const letterOfIntentDoc = {
  name: "letterOfIntent",
  label: "Letter of Intent",
  required: true,
  hint: "A signed letter describing the proposed business, its operation and the reason for this application.",
  criteria: [
    "Is a written letter (not a form, plan, or image).",
    "Describes the proposed business and how it will operate.",
    "States the reason for, or purpose of, this application.",
    "Is dated and signed by the applicant or an authorized representative.",
  ],
}
/** Lease / tenure - premises address is matched against the application; term is checked for expiry. */
const leaseDoc = {
  name: "leaseAgreement",
  label: "Lease or Proof of Tenure",
  required: true,
  docType: "lease" as const,
  hint: "A lease, sublease, or other proof you have tenure of the premises for the licensed area.",
  criteria: [
    "Identifies the premises/civic address of the licensed location.",
    "Names the landlord and the tenant (the applicant or their company).",
    "States the lease term (start and end dates) or that it is month-to-month.",
    "Is signed by both parties.",
  ],
}
/** Local / Indigenous government approval - decision, date and address are extracted; RAG adds policy guidance. */
const localGovApprovalDoc = {
  name: "localGovApproval",
  label: "Local or Indigenous Government Approval",
  required: true,
  docType: "lg-approval" as const,
  hint: "A resolution, letter, or recommendation from the local government or Indigenous nation regarding this application.",
  criteria: [
    "Identifies the local government or Indigenous nation providing the input.",
    "States a clear position (support, opposition, resolution, or no comment).",
    "References the establishment name and/or address for this application.",
    "Is dated and issued by the government body (e.g. council resolution or signed letter).",
  ],
}
/** Corporate incorporation / business summary - used to confirm registration status via BC Registries. */
const incorporationDoc = {
  name: "incorporationDocument",
  label: "Incorporation / Business Summary",
  required: true,
  docType: "incorporation" as const,
  hint: "A certificate of incorporation or current BC Registries company summary for the applicant company.",
  criteria: [
    "Shows the company legal name and BC incorporation/registration number.",
    "Indicates the company's current registration status (e.g. active / in good standing).",
    "Is recent and legible.",
  ],
}
/** Serving It Right / Selling It Right - certificate number + expiry verified against the program registry. */
const servingItRightDoc = {
  name: "servingItRight",
  label: "Serving It Right / Selling It Right Certificate",
  required: true,
  docType: "certificate" as const,
  hint: "A valid Serving It Right (liquor) or Selling It Right (cannabis) certificate for the applicant or manager.",
  criteria: [
    "Shows the certificate holder's name.",
    "Shows the certificate number.",
    "Shows the issue and/or expiry date and is not expired.",
  ],
}


const foodPrimary: CatalogEntry[] = [
  buildEntry({
    slug: "fp-catering-endorsement",
    title: "Catering Endorsement Application",
    description: "Add a catering endorsement to your Food Primary licence",
    licenceType: "food-primary",
    fee: "$220.00",
    intro:
      "A catering endorsement allows you to serve liquor at catered events away from your licensed establishment. You must hold a valid Food Primary licence.",
    steps: customSteps(
      "Catering Endorsement",
      [
        { name: "cateringRegions", label: "Regions you intend to cater in", type: "textarea", required: true },
        { name: "expectedEvents", label: "Estimated number of catered events per year", type: "number", required: true },
        { name: "serviceDescription", label: "Describe your catering liquor service", type: "textarea", required: true },
      ],
      "Tell us about your proposed catering operations",
    ),
    documents: [letterOfIntentDoc],
  }),
  buildEntry({
    slug: "fp-third-party-operator",
    title: "Add or Change a Third Party Operator",
    description: "Add, change, or remove a third party operator on your Food Primary licence",
    licenceType: "food-primary",
    steps: thirdPartyOperatorSteps(),
    documents: [{ name: "operatorAgreement", label: "Third Party Operator Agreement", required: true }],
  }),
  buildEntry({
    slug: "fp-transfer",
    title: "Transfer Your Food Primary Licence or Authorization",
    description: "Transfer your Food Primary licence to a new owner",
    licenceType: "food-primary",
    fee: "$330.00",
    intro: "Use this application to transfer your licence to a new owner or following a change in ownership.",
    steps: transferSteps(),
    documents: [letterOfIntentDoc, corporateRegistryDoc, incorporationDoc, servingItRightDoc],
  }),
  buildEntry({
    slug: "fp-hours-after-midnight",
    title: "Change to Hours of Liquor Service (After Midnight)",
    description: "Permanently change your hours of liquor service to after midnight",
    licenceType: "food-primary",
    intro: "Local government or First Nation approval may be required for service after midnight.",
    steps: hoursChangeSteps({ label: "Hours of Liquor Service" }),
    documents: [localGovApprovalDoc],
  }),
  buildEntry({
    slug: "fp-temp-hours-nm-midnight",
    title: "Temporary Change to Hours of Liquor Service (NM to Midnight)",
    description: "Temporarily extend liquor service hours up to midnight",
    licenceType: "food-primary",
    steps: hoursChangeSteps({ temporary: true, label: "Temporary Hours" }),
  }),
  buildEntry({
    slug: "fp-temp-hours-after-midnight",
    title: "Temporary Change to Hours of Sale (After Midnight)",
    description: "Temporarily change your hours of sale to after midnight",
    licenceType: "food-primary",
    steps: hoursChangeSteps({ temporary: true, label: "Temporary Hours of Sale" }),
  }),
  buildEntry({
    slug: "fp-temp-hours-before-midnight",
    title: "Temporary Change to Hours of Sale (Before Midnight)",
    description: "Temporarily change your hours of sale before midnight",
    licenceType: "food-primary",
    steps: hoursChangeSteps({ temporary: true, label: "Temporary Hours of Sale" }),
  }),
  buildEntry({
    slug: "fp-temp-change-other",
    title: "Temporary Change to a Liquor Licence - Other",
    description: "Request another temporary change to your Food Primary licence",
    licenceType: "food-primary",
    steps: customSteps(
      "Temporary Change",
      [
        { name: "changeRequested", label: "Describe the temporary change requested", type: "textarea", required: true },
        { name: "startDate", label: "Start date", type: "date", required: true, validation: { notPast: true } },
        { name: "endDate", label: "End date", type: "date", required: true, validation: afterDate("startDate") },
        { name: "reason", label: "Reason", type: "textarea", required: true },
      ],
      undefined,
      2,
    ),
  }),
  buildEntry({
    slug: "fp-temp-expanded-service-area",
    title: "Temporary Expanded Service Area Application",
    description: "Temporarily expand your service area",
    licenceType: "food-primary",
    steps: temporaryExtensionSteps(),
    documents: [floorPlanDoc],
  }),
  buildEntry({
    slug: "fp-temp-extension-licensed-area",
    title: "Temporary Extension of Licensed Area",
    description: "Temporarily extend your licensed area",
    licenceType: "food-primary",
    steps: temporaryExtensionSteps(),
    documents: [floorPlanDoc],
  }),
  buildEntry({
    slug: "fp-temp-suspension",
    title: "Temporary Suspension Request",
    description: "Request a temporary suspension of your Food Primary licence",
    licenceType: "food-primary",
    steps: temporarySuspensionSteps(),
  }),
  buildEntry({
    slug: "fp-patron-participation-entertainment",
    title: "Patron Participation Entertainment Endorsement Application",
    description: "Add a patron participation entertainment endorsement",
    licenceType: "food-primary",
    intro: "This endorsement permits activities such as karaoke and dancing where patrons participate in entertainment.",
    steps: customSteps(
      "Patron Participation Entertainment",
      [
        { name: "entertainmentTypes", label: "Types of entertainment", type: "textarea", required: true },
        { name: "entertainmentArea", label: "Area where entertainment will occur", type: "text", required: true },
        { name: "permanent", label: "Is this a permanent endorsement?", type: "radio", options: YES_NO, required: true },
      ],
    ),
  }),
  buildEntry({
    slug: "fp-temp-patron-participation-entertainment",
    title: "Temporary Patron Participation Entertainment Endorsement",
    description: "Add a temporary patron participation entertainment endorsement",
    licenceType: "food-primary",
    steps: customSteps(
      "Temporary Entertainment",
      [
        { name: "entertainmentTypes", label: "Types of entertainment", type: "textarea", required: true },
        { name: "startDate", label: "Start date", type: "date", required: true, validation: { notPast: true } },
        { name: "endDate", label: "End date", type: "date", required: true, validation: afterDate("startDate") },
      ],
      undefined,
      2,
    ),
  }),
  buildEntry({
    slug: "fp-temp-use-area-ski-golf",
    title: "Temporary Use Area Endorsement at a Ski Hill or Golf Course",
    description: "Add a temporary use area endorsement at a ski hill or golf course",
    licenceType: "food-primary",
    steps: endorsementSteps("Temporary Use Area"),
    documents: [sitePlanDoc],
  }),
  buildEntry({
    slug: "fp-remove-tied-house",
    title: "Apply to Remove a Tied House Exemption",
    description: "Request removal of a tied house exemption",
    licenceType: "food-primary",
    steps: tiedHouseRemovalSteps(),
  }),
  buildEntry({
    slug: "fp-name-change",
    title: "Liquor Establishment Name Change Application",
    description: "Change the name of your licensed establishment",
    licenceType: "food-primary",
    steps: nameChangeSteps(),
  }),
  buildEntry({
    slug: "fp-new-outdoor-patio",
    title: "New Outdoor Patio",
    description: "Apply for a new outdoor patio at your Food Primary establishment",
    licenceType: "food-primary",
    intro: "Local government or First Nation approval is required before a patio can be added.",
    steps: patioSteps(),
    documents: [floorPlanDoc, sitePlanDoc],
  }),
  buildEntry({
    slug: "fp-relocation",
    title: "Application to Request Relocation",
    description: "Relocate your Food Primary establishment",
    licenceType: "food-primary",
    steps: relocationSteps(),
    documents: [floorPlanDoc, sitePlanDoc, leaseDoc, localGovApprovalDoc],
  }),
  buildEntry({
    slug: "fp-terms-change",
    title: "Request of Change in Terms and Conditions / Request for Discretion",
    description: "Request a change to the terms and conditions of your licence",
    licenceType: "food-primary",
    steps: termsChangeSteps(),
  }),
]


const liquorPrimary: CatalogEntry[] = [
  buildEntry({
    slug: "lp-temp-use-beer-event",
    title: "Temporary Use Beer Event",
    description: "Apply to hold a temporary use beer event",
    licenceType: "liquor-primary",
    steps: customSteps(
      "Beer Event",
      [
        { name: "eventName", label: "Event name", type: "text", required: true },
        { name: "eventDate", label: "Event date", type: "date", required: true, validation: { notPast: true } },
        { name: "expectedAttendance", label: "Expected attendance", type: "number", required: true },
        { name: "eventDescription", label: "Describe the event", type: "textarea", required: true },
      ],
      undefined,
      2,
    ),
  }),
  buildEntry({
    slug: "lp-new-outdoor-patio",
    title: "New Outdoor Patio",
    description: "Apply for a new outdoor patio at your Liquor Primary establishment",
    licenceType: "liquor-primary",
    steps: patioSteps(),
    documents: [floorPlanDoc, sitePlanDoc],
  }),
  buildEntry({
    slug: "lp-event-driven-terms",
    title: "Request for Change to Event Driven Terms and Conditions",
    description: "Request a change to event-driven terms and conditions",
    licenceType: "liquor-primary",
    steps: termsChangeSteps(),
  }),
  buildEntry({
    slug: "lp-terms-change",
    title: "Request of Change in Terms and Conditions / Request for Discretion",
    description: "Request a change to the terms and conditions of your licence",
    licenceType: "liquor-primary",
    steps: termsChangeSteps(),
  }),
  buildEntry({
    slug: "lp-relocation",
    title: "Liquor Primary and Liquor Primary Club Relocation Application",
    description: "Relocate your Liquor Primary or Liquor Primary Club establishment",
    licenceType: "liquor-primary",
    steps: relocationSteps(),
    documents: [floorPlanDoc, sitePlanDoc, leaseDoc, localGovApprovalDoc],
  }),
  buildEntry({
    slug: "lp-live-theatre-liquor-service",
    title: "Live Theatres Requesting Liquor Service",
    description: "Request liquor service in conjunction with films or broadcasts at a live theatre",
    licenceType: "liquor-primary",
    steps: customSteps(
      "Live Theatre Liquor Service",
      [
        { name: "theatreName", label: "Theatre name", type: "text", required: true },
        { name: "screeningTypes", label: "Types of films / broadcasts", type: "textarea", required: true },
        { name: "serviceAreas", label: "Areas where liquor will be served", type: "textarea", required: true },
      ],
    ),
  }),
  buildEntry({
    slug: "lp-all-ages-liquor-event",
    title: "All Ages Liquor Event",
    description: "Apply to hold an all ages liquor event",
    licenceType: "liquor-primary",
    steps: customSteps(
      "All Ages Event",
      [
        { name: "eventName", label: "Event name", type: "text", required: true },
        { name: "eventDate", label: "Event date", type: "date", required: true, validation: { notPast: true } },
        { name: "minorsControls", label: "Describe how minors will be separated from liquor service", type: "textarea", required: true },
      ],
      undefined,
      2,
    ),
  }),
  buildEntry({
    slug: "lp-third-party-operator",
    title: "Add or Change a Third Party Operator",
    description: "Add, change, or remove a third party operator on your Liquor Primary licence",
    licenceType: "liquor-primary",
    steps: thirdPartyOperatorSteps(),
    documents: [{ name: "operatorAgreement", label: "Third Party Operator Agreement", required: true }],
  }),
  buildEntry({
    slug: "lp-structural-change-capacity",
    title: "Structural Change Application (Capacity Increase)",
    description: "Structural changes that increase your approved capacity",
    licenceType: "liquor-primary",
    steps: structuralChangeSteps(true),
    documents: [floorPlanDoc],
  }),
  buildEntry({
    slug: "lp-structural-change-no-capacity",
    title: "Structural Change Application (No Capacity Increase)",
    description: "Structural changes that do not increase your approved capacity",
    licenceType: "liquor-primary",
    steps: structuralChangeSteps(false),
    documents: [floorPlanDoc],
  }),
  buildEntry({
    slug: "lp-temp-change-other",
    title: "Temporary Change to a Liquor Licence - Other",
    description: "Request another temporary change to your Liquor Primary licence",
    licenceType: "liquor-primary",
    steps: customSteps(
      "Temporary Change",
      [
        { name: "changeRequested", label: "Describe the temporary change requested", type: "textarea", required: true },
        { name: "startDate", label: "Start date", type: "date", required: true, validation: { notPast: true } },
        { name: "endDate", label: "End date", type: "date", required: true, validation: afterDate("startDate") },
      ],
      undefined,
      2,
    ),
  }),
  buildEntry({
    slug: "lp-temp-hours-sale",
    title: "Temporary Change to Hours of Sale",
    description: "Temporarily change your hours of sale",
    licenceType: "liquor-primary",
    steps: hoursChangeSteps({ temporary: true, label: "Temporary Hours of Sale" }),
  }),
  buildEntry({
    slug: "lp-temp-extension-licensed-area",
    title: "Temporary Extension of Licensed Area",
    description: "Temporarily extend your licensed area",
    licenceType: "liquor-primary",
    steps: temporaryExtensionSteps(),
    documents: [floorPlanDoc],
  }),
  buildEntry({
    slug: "lp-temp-suspension",
    title: "Temporary Suspension Request",
    description: "Request a temporary suspension of your Liquor Primary licence",
    licenceType: "liquor-primary",
    steps: temporarySuspensionSteps(),
  }),
  buildEntry({
    slug: "lp-temp-use-area-ski-golf",
    title: "Temporary Use Area Endorsement at a Ski Hill or Golf Course",
    description: "Add a temporary use area endorsement at a ski hill or golf course",
    licenceType: "liquor-primary",
    steps: endorsementSteps("Temporary Use Area"),
    documents: [sitePlanDoc],
  }),
  buildEntry({
    slug: "lp-remove-tied-house",
    title: "Apply to Remove a Tied House Exemption",
    description: "Request removal of a tied house exemption",
    licenceType: "liquor-primary",
    steps: tiedHouseRemovalSteps(),
  }),
  buildEntry({
    slug: "lp-family-food-service",
    title: "Application to Allow Family Food Service",
    description: "Apply to allow family food service in your establishment",
    licenceType: "liquor-primary",
    steps: customSteps(
      "Family Food Service",
      [
        { name: "foodServiceArea", label: "Area for family food service", type: "text", required: true },
        { name: "foodServiceHours", label: "Hours of family food service", type: "text", required: true },
        { name: "minorControls", label: "How will minors be supervised?", type: "textarea", required: true },
      ],
    ),
  }),
  buildEntry({
    slug: "lp-minors-recreation",
    title: "Application to Allow Minors in Recreation Facilities",
    description: "Apply to allow minors in recreation facilities",
    licenceType: "liquor-primary",
    steps: customSteps(
      "Minors in Recreation Facilities",
      [
        { name: "facilityType", label: "Type of recreation facility", type: "text", required: true },
        { name: "minorAreas", label: "Areas minors will be permitted", type: "textarea", required: true },
        { name: "minorHours", label: "Hours minors will be permitted", type: "text", required: true },
      ],
    ),
  }),
  buildEntry({
    slug: "lp-cancellation-suspension",
    title: "Licence Cancellation and Suspension",
    description: "Request cancellation or suspension of your Liquor Primary licence",
    licenceType: "liquor-primary",
    steps: customSteps(
      "Cancellation / Suspension",
      [
        {
          name: "requestType",
          label: "Request type",
          type: "select",
          required: true,
          options: [
            { value: "cancel", label: "Cancel the licence" },
            { value: "suspend", label: "Suspend the licence" },
          ],
        },
        { name: "effectiveDate", label: "Effective date", type: "date", required: true, validation: { notPast: true } },
        { name: "reason", label: "Reason", type: "textarea", required: true },
      ],
      undefined,
      2,
    ),
  }),
  buildEntry({
    slug: "lp-catering-endorsement",
    title: "Catering Endorsement Application",
    description: "Add a catering endorsement to your Liquor Primary licence",
    licenceType: "liquor-primary",
    fee: "$220.00",
    steps: customSteps(
      "Catering Endorsement",
      [
        { name: "cateringRegions", label: "Regions you intend to cater in", type: "textarea", required: true },
        { name: "expectedEvents", label: "Estimated number of catered events per year", type: "number", required: true },
      ],
    ),
  }),
  buildEntry({
    slug: "lp-hours-inside",
    title: "Change to Hours of Liquor Service (Inside Service Hours)",
    description: "Permanently change hours of liquor service within standard service hours",
    licenceType: "liquor-primary",
    steps: hoursChangeSteps({ label: "Hours of Liquor Service" }),
  }),
  buildEntry({
    slug: "lp-hours-outside",
    title: "Change to Hours of Liquor Service (Outside Service Hours)",
    description: "Permanently change hours of liquor service outside standard service hours",
    licenceType: "liquor-primary",
    intro: "Local government or First Nation approval is required for service outside standard hours.",
    steps: hoursChangeSteps({ label: "Hours of Liquor Service" }),
  }),
  buildEntry({
    slug: "lp-name-change",
    title: "Liquor Establishment Name Change Application",
    description: "Change the name of your licensed establishment",
    licenceType: "liquor-primary",
    steps: nameChangeSteps(),
  }),
  buildEntry({
    slug: "lp-transfer",
    title: "Transfer Your Liquor Primary Licence or Authorization",
    description: "Transfer your Liquor Primary licence to a new owner",
    licenceType: "liquor-primary",
    fee: "$330.00",
    steps: transferSteps(),
    documents: [letterOfIntentDoc, corporateRegistryDoc, incorporationDoc, servingItRightDoc],
  }),
]


function manufacturerSet(licenceType: LicenceType, prefix: string, label: string): CatalogEntry[] {
  return [
    buildEntry({
      slug: `${prefix}-remove-tied-house`,
      title: "Apply to Remove a Tied House Exemption",
      description: `Request removal of a tied house exemption (${label})`,
      licenceType,
      steps: tiedHouseRemovalSteps(),
    }),
    buildEntry({
      slug: `${prefix}-third-party-operator`,
      title: "Add or Change a Third Party Operator",
      description: `Add, change, or remove a third party operator (${label})`,
      licenceType,
      steps: thirdPartyOperatorSteps(),
      documents: [{ name: "operatorAgreement", label: "Third Party Operator Agreement", required: true }],
    }),
    buildEntry({
      slug: `${prefix}-lounge-endorsement`,
      title: "Lounge Area Endorsement Application",
      description: `Add a lounge area endorsement (${label})`,
      licenceType,
      steps: endorsementSteps("Lounge Area"),
      documents: [floorPlanDoc],
    }),
    buildEntry({
      slug: `${prefix}-new-patio`,
      title: "New Outdoor Patio (Manufacturer) Application",
      description: `Apply for a new outdoor patio (${label})`,
      licenceType,
      steps: patioSteps(),
      documents: [floorPlanDoc, sitePlanDoc],
    }),
    buildEntry({
      slug: `${prefix}-onsite-store-endorsement`,
      title: "Manufacturer On Site Store Endorsement Application",
      description: `Add an on-site store endorsement (${label})`,
      licenceType,
      steps: endorsementSteps("On Site Store"),
      documents: [floorPlanDoc],
    }),
    buildEntry({
      slug: `${prefix}-picnic-area-endorsement`,
      title: "Manufacturer Picnic Area Endorsement Application",
      description: `Add a picnic area endorsement (${label})`,
      licenceType,
      steps: endorsementSteps("Picnic Area"),
      documents: [sitePlanDoc],
    }),
    buildEntry({
      slug: `${prefix}-terms-change`,
      title: "Request of Change in Terms and Conditions / Request for Discretion",
      description: `Request a change to the terms and conditions of your licence (${label})`,
      licenceType,
      steps: termsChangeSteps(),
    }),
    buildEntry({
      slug: `${prefix}-relocation`,
      title: "Manufacturer Relocation Application",
      description: `Relocate your manufacturing establishment (${label})`,
      licenceType,
      steps: relocationSteps(),
      documents: [floorPlanDoc, sitePlanDoc, leaseDoc, localGovApprovalDoc],
    }),
    buildEntry({
      slug: `${prefix}-special-event-area-endorsement`,
      title: "Special Event Area Endorsement Application",
      description: `Add a special event area endorsement (${label})`,
      licenceType,
      steps: endorsementSteps("Special Event Area"),
      documents: [floorPlanDoc],
    }),
    buildEntry({
      slug: `${prefix}-structural-lounge-capacity`,
      title: "Structural Alterations to an Approved Lounge or Special Event Area (Increase to Capacity)",
      description: `Structural alterations increasing capacity of a lounge or special event area (${label})`,
      licenceType,
      steps: structuralChangeSteps(true),
      documents: [floorPlanDoc],
    }),
    buildEntry({
      slug: `${prefix}-structural-lounge-no-capacity`,
      title: "Structural Alterations to an Approved Lounge or Special Event Area (No Increase to Capacity)",
      description: `Structural alterations to a lounge or special event area without capacity increase (${label})`,
      licenceType,
      steps: structuralChangeSteps(false),
      documents: [floorPlanDoc],
    }),
    buildEntry({
      slug: `${prefix}-structural-production`,
      title: "Structural Change to a Production/Storage Area, Sampling Area, or On Site Store",
      description: `Structural change to a production, storage, sampling, or on-site store area (${label})`,
      licenceType,
      steps: structuralChangeSteps(false),
      documents: [floorPlanDoc],
    }),
    buildEntry({
      slug: `${prefix}-temp-change-other`,
      title: "Temporary Change to a Liquor Licence - Other",
      description: `Request another temporary change to your licence (${label})`,
      licenceType,
      steps: customSteps(
        "Temporary Change",
        [
          { name: "changeRequested", label: "Describe the temporary change requested", type: "textarea", required: true },
          { name: "startDate", label: "Start date", type: "date", required: true, validation: { notPast: true } },
          { name: "endDate", label: "End date", type: "date", required: true, validation: afterDate("startDate") },
        ],
        undefined,
        2,
      ),
    }),
    buildEntry({
      slug: `${prefix}-temp-hours-sale`,
      title: "Temporary Change to Hours of Sale",
      description: `Temporarily change your hours of sale (${label})`,
      licenceType,
      steps: hoursChangeSteps({ temporary: true, label: "Temporary Hours of Sale" }),
    }),
    buildEntry({
      slug: `${prefix}-temp-extension`,
      title: "Temporary Extension of Licensed Area",
      description: `Temporarily extend your licensed area (${label})`,
      licenceType,
      steps: temporaryExtensionSteps(),
      documents: [floorPlanDoc],
    }),
    buildEntry({
      slug: `${prefix}-temp-suspension`,
      title: "Temporary Suspension Request",
      description: `Request a temporary suspension of your licence (${label})`,
      licenceType,
      steps: temporarySuspensionSteps(),
    }),
    buildEntry({
      slug: `${prefix}-cancellation-suspension`,
      title: "Licence Cancellation and Suspension",
      description: `Request cancellation or suspension of your licence (${label})`,
      licenceType,
      steps: customSteps(
        "Cancellation / Suspension",
        [
          {
            name: "requestType",
            label: "Request type",
            type: "select",
            required: true,
            options: [
              { value: "cancel", label: "Cancel the licence" },
              { value: "suspend", label: "Suspend the licence" },
            ],
          },
          { name: "effectiveDate", label: "Effective date", type: "date", required: true, validation: { notPast: true } },
          { name: "reason", label: "Reason", type: "textarea", required: true },
        ],
        undefined,
        2,
      ),
    }),
    buildEntry({
      slug: `${prefix}-hours-lounge`,
      title: "Change to Hours of Liquor Service (Lounge Area)",
      description: `Permanently change hours of liquor service in a lounge area (${label})`,
      licenceType,
      steps: hoursChangeSteps({ label: "Lounge Hours of Service" }),
    }),
    buildEntry({
      slug: `${prefix}-hours-special-event`,
      title: "Change to Hours of Liquor Service (Special Event Area)",
      description: `Permanently change hours of liquor service in a special event area (${label})`,
      licenceType,
      steps: hoursChangeSteps({ label: "Special Event Area Hours" }),
    }),
    buildEntry({
      slug: `${prefix}-name-change`,
      title: "Liquor Establishment Name Change Application",
      description: `Change the name of your establishment (${label})`,
      licenceType,
      steps: nameChangeSteps(),
    }),
    buildEntry({
      slug: `${prefix}-transfer`,
      title: "Transfer Your Manufacturer Licence or Authorization",
      description: `Transfer your manufacturer licence to a new owner (${label})`,
      licenceType,
      fee: "$330.00",
      steps: transferSteps(),
      documents: [letterOfIntentDoc, corporateRegistryDoc, incorporationDoc, servingItRightDoc],
    }),
  ]
}

const manufacturer: CatalogEntry[] = manufacturerSet("manufacturer", "mfr", "Manufacturer")


const deemedManufacturer: CatalogEntry[] = [
  ...manufacturerSet("deemed-manufacturer", "dmfr", "Deemed Manufacturer"),
  buildEntry({
    slug: "dmfr-change-picnic-area",
    title: "Change to an Approved Picnic Area Application",
    description: "Request a change to an approved picnic area (Deemed Manufacturer)",
    licenceType: "deemed-manufacturer",
    steps: endorsementSteps("Picnic Area"),
    documents: [sitePlanDoc],
  }),
]


const cannabisRetail: CatalogEntry[] = [
  buildEntry({
    slug: "crs-relocation",
    title: "Cannabis Retail Store Relocation Application",
    description: "Relocate your cannabis retail store",
    licenceType: "cannabis-retail-store",
    steps: relocationSteps(),
    documents: [floorPlanDoc, sitePlanDoc, leaseDoc, localGovApprovalDoc],
  }),
  buildEntry({
    slug: "crs-transfer",
    title: "Transfer Your Cannabis Retail Store Licence",
    description: "Transfer your cannabis retail store licence to a new owner",
    licenceType: "cannabis-retail-store",
    fee: "$330.00",
    steps: transferSteps(),
    documents: [letterOfIntentDoc, corporateRegistryDoc, incorporationDoc, servingItRightDoc],
  }),
  buildEntry({
    slug: "crs-name-change",
    title: "Cannabis Establishment Name Change Application",
    description: "Change the name of your cannabis retail store",
    licenceType: "cannabis-retail-store",
    steps: nameChangeSteps(),
  }),
  buildEntry({
    slug: "crs-structural-change",
    title: "Cannabis Retail Store Structural Change Application",
    description: "Structural changes to your cannabis retail store",
    licenceType: "cannabis-retail-store",
    steps: structuralChangeSteps(false),
    documents: [floorPlanDoc],
  }),
  buildEntry({
    slug: "crs-hours-change",
    title: "Change to Hours of Sale",
    description: "Change your cannabis retail store hours of sale",
    licenceType: "cannabis-retail-store",
    steps: hoursChangeSteps({ label: "Hours of Sale" }),
  }),
  buildEntry({
    slug: "crs-third-party-operator",
    title: "Add or Change a Third Party Operator",
    description: "Add, change, or remove a third party operator on your cannabis retail store licence",
    licenceType: "cannabis-retail-store",
    steps: thirdPartyOperatorSteps(),
    documents: [{ name: "operatorAgreement", label: "Third Party Operator Agreement", required: true }],
  }),
  buildEntry({
    slug: "crs-terms-change",
    title: "Request of Change in Terms and Conditions / Request for Discretion",
    description: "Request a change to the terms and conditions of your licence",
    licenceType: "cannabis-retail-store",
    steps: termsChangeSteps(),
  }),
  buildEntry({
    slug: "crs-temp-suspension",
    title: "Temporary Suspension Request",
    description: "Request a temporary suspension of your cannabis retail store licence",
    licenceType: "cannabis-retail-store",
    steps: temporarySuspensionSteps(),
  }),
]


const licenseeRetail: CatalogEntry[] = [
  buildEntry({
    slug: "lrs-relocation",
    title: "Licensee Retail Store Relocation Application",
    description: "Relocate your licensee retail store",
    licenceType: "licensee-retail-store",
    steps: relocationSteps(),
    documents: [floorPlanDoc, sitePlanDoc, leaseDoc, localGovApprovalDoc],
  }),
  buildEntry({
    slug: "lrs-transfer",
    title: "Transfer Your Licensee Retail Store Licence",
    description: "Transfer your licensee retail store licence to a new owner",
    licenceType: "licensee-retail-store",
    fee: "$330.00",
    steps: transferSteps(),
    documents: [letterOfIntentDoc, corporateRegistryDoc, incorporationDoc, servingItRightDoc],
  }),
  buildEntry({
    slug: "lrs-name-change",
    title: "Liquor Establishment Name Change Application",
    description: "Change the name of your licensee retail store",
    licenceType: "licensee-retail-store",
    steps: nameChangeSteps(),
  }),
  buildEntry({
    slug: "lrs-structural-change",
    title: "Licensee Retail Store Structural Change Application",
    description: "Structural changes to your licensee retail store",
    licenceType: "licensee-retail-store",
    steps: structuralChangeSteps(false),
    documents: [floorPlanDoc],
  }),
  buildEntry({
    slug: "lrs-hours-change",
    title: "Change to Hours of Sale",
    description: "Change your licensee retail store hours of sale",
    licenceType: "licensee-retail-store",
    steps: hoursChangeSteps({ label: "Hours of Sale" }),
  }),
  buildEntry({
    slug: "lrs-third-party-operator",
    title: "Add or Change a Third Party Operator",
    description: "Add, change, or remove a third party operator on your licensee retail store licence",
    licenceType: "licensee-retail-store",
    steps: thirdPartyOperatorSteps(),
    documents: [{ name: "operatorAgreement", label: "Third Party Operator Agreement", required: true }],
  }),
  buildEntry({
    slug: "lrs-terms-change",
    title: "Request of Change in Terms and Conditions / Request for Discretion",
    description: "Request a change to the terms and conditions of your licence",
    licenceType: "licensee-retail-store",
    steps: termsChangeSteps(),
  }),
  buildEntry({
    slug: "lrs-temp-suspension",
    title: "Temporary Suspension Request",
    description: "Request a temporary suspension of your licensee retail store licence",
    licenceType: "licensee-retail-store",
    steps: temporarySuspensionSteps(),
  }),
]


/** Applicant / business details step shared by new-licence applications. */
const applicantStep = customSteps(
  "Applicant Details",
  [
    { name: "legalEntityName", label: "Legal entity / business name", type: "text", required: true },
    {
      name: "entityType",
      label: "Type of legal entity",
      type: "select",
      required: true,
      options: [
        { value: "sole-proprietor", label: "Sole proprietor" },
        { value: "partnership", label: "Partnership" },
        { value: "corporation", label: "Corporation" },
        { value: "society", label: "Society" },
        { value: "cooperative", label: "Co-operative" },
      ],
    },
    { name: "businessNumber", label: "Business number (BN)", type: "text" },
    // Auto-filled read-only from BC Registries, so the old "(if applicable)" hedge is no longer needed
    { name: "incorporationNumber", label: "Incorporation number", type: "text" },
  ],
  "Tell us about the applicant",
  2,
)

const proposedEstablishmentStep = customSteps(
  "Proposed Establishment",
  [
    { name: "establishmentName", label: "Proposed establishment name", type: "text", required: true },
    { name: "addressLine1", label: "Street address", type: "text", required: true },
    { name: "city", label: "City / town", type: "text", required: true },
    { name: "postalCode", label: "Postal code", type: "text", required: true, validation: { postalCode: true } },
    { name: "proposedCapacity", label: "Proposed occupant capacity", type: "number" },
    { name: "proposedOpenDate", label: "Proposed opening date", type: "date", validation: { notPast: true } },
  ],
  "Where will the licensed business operate?",
  2,
)

const mainApplications: CatalogEntry[] = [
  buildEntry({
    slug: "agent-licence",
    title: "Agent Licence Application",
    description: "Apply for a new Agent licence to market liquor in B.C.",
    licenceType: "agent",
    kind: "application",
    fee: "$220.00",
    establishment: false,
    intro:
      "An Agent licence authorizes you to market liquor on behalf of a manufacturer to the Liquor Distribution Branch and licensees. If you have questions regarding the Agent licence application, contact the LDB.",
    steps: [
      ...applicantStep,
      ...customSteps(
        "Agent Details",
        [
          { name: "manufacturersRepresented", label: "Manufacturers you will represent", type: "textarea", required: true },
          { name: "productTypes", label: "Types of liquor products", type: "textarea", required: true },
        ],
      ),
    ],
    documents: [letterOfIntentDoc],
  }),
  buildEntry({
    slug: "catering-licence",
    title: "Catering Licence Application",
    description: "Apply for a new Catering licence to serve liquor at catered events",
    licenceType: "catering",
    kind: "application",
    fee: "$475.00",
    establishment: false,
    intro: "A Catering licence allows you to sell and serve liquor at events you cater across British Columbia.",
    steps: [
      ...applicantStep,
      ...customSteps(
        "Catering Operations",
        [
          { name: "cateringRegions", label: "Regions you intend to operate in", type: "textarea", required: true },
          { name: "commercialKitchen", label: "Do you operate a commercial kitchen?", type: "radio", options: YES_NO, required: true },
          { name: "expectedEvents", label: "Estimated catered events per year", type: "number", required: true },
        ],
        undefined,
        2,
      ),
    ],
    documents: [letterOfIntentDoc],
  }),
  buildEntry({
    slug: "liquor-primary-licence",
    title: "Liquor Primary Licence Application",
    description: "Apply for a new Liquor Primary licence (bar, pub, lounge, etc.)",
    licenceType: "liquor-primary",
    kind: "application",
    fee: "$950.00",
    establishment: false,
    intro:
      "A Liquor Primary licence is for businesses where liquor service is the primary focus, such as bars, pubs, lounges, and nightclubs. Local government or First Nation approval is required.",
    steps: [
      ...applicantStep,
      ...proposedEstablishmentStep,
      ...customSteps(
        "Establishment Type",
        [
          {
            name: "establishmentType",
            label: "Type of establishment",
            type: "select",
            required: true,
            options: [
              { value: "bar-pub", label: "Bar / Pub" },
              { value: "lounge", label: "Lounge" },
              { value: "nightclub", label: "Nightclub" },
              { value: "stadium", label: "Stadium / Concert venue" },
              { value: "other", label: "Other" },
            ],
          },
          { name: "proposedHours", label: "Proposed hours of liquor service", type: "text", required: true },
          { name: "patronCapacity", label: "Proposed patron capacity", type: "number", required: true },
        ],
        undefined,
        2,
      ),
    ],
    documents: [floorPlanDoc, sitePlanDoc, letterOfIntentDoc],
  }),
  buildEntry({
    slug: "liquor-primary-club-licence",
    title: "Liquor Primary Club Licence Application",
    description: "Apply for a new Liquor Primary Club licence",
    licenceType: "liquor-primary",
    kind: "application",
    fee: "$950.00",
    establishment: false,
    intro: "A Liquor Primary Club licence is for member-based clubs such as legions, golf clubs, and social clubs.",
    steps: [
      ...applicantStep,
      ...proposedEstablishmentStep,
      ...customSteps(
        "Club Details",
        [
          { name: "clubType", label: "Type of club", type: "text", required: true },
          { name: "membershipSize", label: "Approximate membership size", type: "number", required: true },
          { name: "membershipCriteria", label: "Membership criteria", type: "textarea", required: true },
        ],
      ),
    ],
    documents: [floorPlanDoc, letterOfIntentDoc],
  }),
  buildEntry({
    slug: "ubrew-uvin-licence",
    title: "UBrew and UVin (Ferment-on-Premises) Licence Application",
    description: "Apply for a new UBrew and UVin ferment-on-premises licence",
    licenceType: "ubrew-uvin",
    kind: "application",
    fee: "$475.00",
    establishment: false,
    intro:
      "A UBrew and UVin licence allows customers to make beer or wine on your premises for their personal use using your equipment.",
    steps: [
      ...applicantStep,
      ...proposedEstablishmentStep,
      ...customSteps(
        "Premises Details",
        [
          { name: "equipmentDescription", label: "Describe the brewing / fermenting equipment", type: "textarea", required: true },
          { name: "stationCount", label: "Number of customer stations", type: "number", required: true },
        ],
      ),
    ],
    documents: [floorPlanDoc],
  }),
  buildEntry({
    slug: "rural-licensee-retail-store-licence",
    title: "Rural Licensee Retail Store Application",
    description: "Apply for a new Rural Licensee Retail Store licence",
    licenceType: "licensee-retail-store",
    kind: "application",
    fee: "$950.00",
    establishment: false,
    intro:
      "A Rural Licensee Retail Store licence allows liquor sales in rural areas more than 10 km from an existing liquor store. Eligibility requirements apply.",
    steps: [
      ...applicantStep,
      ...proposedEstablishmentStep,
      ...customSteps(
        "Rural Eligibility",
        [
          { name: "distanceToNearestStore", label: "Distance to nearest liquor store (km)", type: "number", required: true },
          { name: "communityPopulation", label: "Population of the community served", type: "number", required: true },
        ],
        undefined,
        2,
      ),
    ],
    documents: [sitePlanDoc, letterOfIntentDoc],
  }),
]


export const EXTENDED_CATALOG: CatalogEntry[] = [
  ...mainApplications,
  ...foodPrimary,
  ...liquorPrimary,
  ...manufacturer,
  ...deemedManufacturer,
  ...cannabisRetail,
  ...licenseeRetail,
]
