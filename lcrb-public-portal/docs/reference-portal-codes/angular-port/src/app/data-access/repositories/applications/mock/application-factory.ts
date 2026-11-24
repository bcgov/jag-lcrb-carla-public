import type { FlowConfig, FlowStep } from "@domain/forms/flow-config"
import type { FieldConfig } from "@domain/forms/field-schema"

/** Flow factory. */

import type { CatalogEntry } from "@domain/models/applications"
import { type DocumentSpec, type FlowSpec } from "@domain/models/applications"

export const YES_NO = [
  { value: "yes", label: "Yes" },
  { value: "no", label: "No" },
]

/** Validation: this date must be on or after the value of another date field. */
export function afterDate(otherField: string, message = "Must be on or after the start date") {
  return { afterField: { field: otherField, message } }
}


/** "Before you start" - eligibility / context acknowledgement. */
export function beforeStartingStep(body: string): FlowStep {
  return {
    id: "before-starting",
    title: "Before Starting",
    description: "Important information about this application",
    sections: [
      {
        title: "Before you begin",
        info: {
          variant: "info",
          title: "Please read carefully",
          body,
        },
        fields: [
          {
            name: "__requirementsAck",
            label: "I have read and understand the requirements above.",
            type: "checkbox",
            required: true,
          },
        ],
      },
    ],
  }
}

/** Establishment identification + physical location address. */
export function establishmentStep(): FlowStep {
  return {
    id: "establishment",
    title: "Establishment",
    description: "Identify the licence and establishment this request applies to",
    sections: [
      {
        title: "Licence & Establishment Details",
        description: "Your business details are pre-filled from your BCeID account. Select the licence this request applies to.",
        columns: 2,
        fields: [
          { name: "legalEntityName", label: "Legal Entity / Business Name (BCeID)", type: "text", required: true },
          { name: "bceidBusinessNumber", label: "BCeID Business Number", type: "text", required: true },
          { name: "licenceNumber", label: "Licence Number", type: "text", required: true },
          { name: "establishmentName", label: "Establishment Name", type: "text", required: true },
        ],
      },
      {
        title: "Establishment Location",
        description: "The physical address of the licensed establishment",
        columns: 2,
        fields: [
          { name: "addressLine1", label: "Street Address", type: "text", required: true },
          { name: "addressLine2", label: "Unit / Suite (optional)", type: "text" },
          { name: "city", label: "City / Town", type: "text", required: true },
          {
            name: "province",
            label: "Province",
            type: "select",
            required: true,
            options: [
              { value: "BC", label: "British Columbia" },
              { value: "AB", label: "Alberta" },
              { value: "other", label: "Other" },
            ],
          },
          {
            name: "postalCode",
            label: "Postal Code",
            type: "text",
            required: true,
            validation: { postalCode: true },
          },
        ],
      },
    ],
  }
}

/** Supporting documents upload step (only emitted when documents provided). */
export function documentsStep(docs: DocumentSpec[]): FlowStep {
  const fields: FieldConfig[] = []
  for (const d of docs) {
    const occupancyTarget = d.occupancyField ? `${d.name}OccupancyLoad` : undefined
    fields.push({
      name: d.name,
      label: d.label,
      type: "file",
      required: d.required,
      hint: d.hint,
      criteria: d.criteria,
      docType: d.docType,
      autoPopulateField: occupancyTarget,
    })
    // Companion occupancy field, auto-filled from the floor-plan stamp but editable so the applicant can correct a misread.
    if (occupancyTarget) {
      fields.push({
        name: occupancyTarget,
        label: "Occupancy load (persons)",
        type: "number",
        hint: "Auto-filled from your floor plan's occupancy stamp when detected. Please confirm or correct it.",
      })
    }
  }
  return {
    id: "documents",
    title: "Supporting Documents",
    description: "Upload the documents required for this application",
    sections: [
      {
        title: "Required Documents",
        info: {
          variant: "info",
          body: "Files must be PDF or image format and no larger than 25MB each. Each upload is automatically checked against the requirements — floor plans are read for their occupancy load stamp and director documents are confirmed against BC Registries — so you can fix gaps before submitting. These checks are advisory and never block your submission.",
        },
        fields,
      },
    ],
  }
}

/** Contact step. */
export function contactStep(): FlowStep {
  return {
    id: "contact",
    title: "Contact",
    description: "Who should we contact about this application?",
    sections: [
      {
        title: "Contact Information",
        columns: 2,
        fields: [
          { name: "contactName", label: "Contact Name", type: "text", required: true },
          { name: "contactTitle", label: "Title / Position", type: "text" },
          { name: "contactPhone", label: "Phone Number", type: "tel", required: true },
          { name: "contactEmail", label: "Email", type: "email", required: true },
        ],
      },
    ],
  }
}

export function buildEntry(spec: FlowSpec): CatalogEntry {
  const steps: FlowStep[] = []
  if (spec.intro) steps.push(beforeStartingStep(spec.intro))
  if (spec.establishment !== false) steps.push(establishmentStep())
  steps.push(...spec.steps)
  if (spec.documents && spec.documents.length) steps.push(documentsStep(spec.documents))
  if (spec.contact !== false) steps.push(contactStep())

  const config: FlowConfig = {
    id: spec.slug,
    title: spec.title,
    subtitle: spec.description,
    fee: spec.fee,
    returnTo: spec.returnTo ?? "/applications",
    currentNav: spec.currentNav ?? "/applications",
    steps,
  }

  return {
    slug: spec.slug,
    title: spec.title,
    description: spec.description,
    licenceType: spec.licenceType,
    kind: spec.kind ?? "authorization",
    fee: spec.fee,
    config,
  }
}


/** Single section made of fields. */
function step(id: string, title: string, fields: FieldConfig[], description?: string, columns?: 1 | 2 | 3): FlowStep {
  return { id, title, description, sections: [{ title, columns, fields }] }
}

/** Temporary or permanent change to hours of liquor service / sale. */
export function hoursChangeSteps(opts: { temporary?: boolean; label?: string }): FlowStep[] {
  const f: FieldConfig[] = [
    {
      name: "serviceArea",
      label: "Which service area does this apply to?",
      type: "select",
      required: true,
      options: [
        { value: "entire", label: "Entire establishment" },
        { value: "lounge", label: "Lounge area" },
        { value: "patio", label: "Patio / outdoor area" },
        { value: "special-event", label: "Special event area" },
      ],
    },
    { name: "currentHours", label: "Current hours of service", type: "text", required: true, placeholder: "e.g. 11:00 AM – 11:00 PM" },
    { name: "requestedOpen", label: "Requested opening time", type: "time", required: true },
    { name: "requestedClose", label: "Requested closing time", type: "time", required: true },
  ]
  if (opts.temporary) {
    f.push(
      { name: "startDate", label: "Start date", type: "date", required: true, validation: { notPast: true } },
      { name: "endDate", label: "End date", type: "date", required: true, validation: afterDate("startDate") },
    )
  }
  f.push({ name: "reason", label: "Reason for the change", type: "textarea", required: true })
  return [step("details", opts.label ?? "Hours of Service", f, "Tell us about the change to your hours", 2)]
}

/** Structural change (with or without capacity increase). */
export function structuralChangeSteps(capacityIncrease: boolean): FlowStep[] {
  const f: FieldConfig[] = [
    { name: "changeDescription", label: "Describe the structural changes", type: "textarea", required: true },
    { name: "areaAffected", label: "Area(s) affected", type: "text", required: true },
    { name: "currentCapacity", label: "Current approved capacity (persons)", type: "number", required: true },
  ]
  if (capacityIncrease) {
    f.push({ name: "proposedCapacity", label: "Proposed capacity (persons)", type: "number", required: true })
  }
  f.push(
    { name: "constructionStart", label: "Estimated construction start", type: "date" },
    { name: "constructionComplete", label: "Estimated completion", type: "date" },
  )
  return [step("details", "Structural Change Details", f, undefined, 2)]
}

/** Licence/authorization transfer. */
export function transferSteps(): FlowStep[] {
  return [
    step(
      "transfer",
      "Transfer Details",
      [
        {
          name: "transferType",
          label: "Type of transfer",
          type: "select",
          required: true,
          options: [
            { value: "new-owner", label: "Transfer to a new owner" },
            { value: "share-transfer", label: "Share transfer (change in ownership)" },
            { value: "name-change", label: "Change of legal entity name only" },
          ],
        },
        { name: "proposedTransfereeName", label: "Proposed transferee (legal name)", type: "text", required: true },
        { name: "transfereeContactName", label: "Transferee contact name", type: "text", required: true },
        { name: "transfereeEmail", label: "Transferee email", type: "email", required: true },
        { name: "proposedDate", label: "Proposed transfer date", type: "date", required: true, validation: { notPast: true } },
        { name: "purchasePrice", label: "Purchase price (optional)", type: "number" },
      ],
      "Provide details about the proposed transfer",
      2,
    ),
  ]
}

/** Relocation of a licensed establishment. */
export function relocationSteps(): FlowStep[] {
  return [
    step(
      "relocation",
      "Proposed New Location",
      [
        { name: "newAddressLine1", label: "New street address", type: "text", required: true },
        { name: "newCity", label: "New city / town", type: "text", required: true },
        { name: "newPostalCode", label: "New postal code", type: "text", required: true, validation: { postalCode: true } },
        { name: "relocationReason", label: "Reason for relocation", type: "textarea", required: true },
        { name: "proposedMoveDate", label: "Proposed move date", type: "date", required: true, validation: { notPast: true } },
      ],
      "Where do you intend to relocate the establishment?",
      2,
    ),
  ]
}

/** Establishment name change. */
export function nameChangeSteps(): FlowStep[] {
  return [
    step(
      "name-change",
      "Name Change",
      [
        { name: "currentName", label: "Current establishment name", type: "text", required: true },
        { name: "proposedName", label: "Proposed establishment name", type: "text", required: true },
        { name: "effectiveDate", label: "Requested effective date", type: "date", validation: { notPast: true } },
        { name: "nameReason", label: "Reason for the name change", type: "textarea" },
      ],
      "Tell us the new trading name for your establishment",
      2,
    ),
  ]
}

/** Outdoor patio / expanded service area. */
export function patioSteps(): FlowStep[] {
  return [
    step(
      "patio",
      "Patio / Outdoor Area",
      [
        {
          name: "patioType",
          label: "Type of area",
          type: "select",
          required: true,
          options: [
            { value: "new-patio", label: "New outdoor patio" },
            { value: "expand", label: "Expansion of existing patio" },
            { value: "temporary", label: "Temporary expanded service area" },
          ],
        },
        { name: "patioCapacity", label: "Proposed patio capacity (persons)", type: "number", required: true },
        { name: "patioArea", label: "Approximate area (sq. metres)", type: "number" },
        { name: "patioAdjacent", label: "Is the patio adjacent to the establishment?", type: "radio", options: YES_NO, required: true },
        { name: "patioDescription", label: "Describe the proposed area and boundaries", type: "textarea", required: true },
      ],
      "Describe the outdoor service area",
      2,
    ),
  ]
}

/** Add or change a third party operator. */
export function thirdPartyOperatorSteps(): FlowStep[] {
  return [
    step(
      "operator",
      "Third Party Operator",
      [
        {
          name: "operatorAction",
          label: "What would you like to do?",
          type: "select",
          required: true,
          options: [
            { value: "add", label: "Add a third party operator" },
            { value: "change", label: "Change the existing operator" },
            { value: "remove", label: "Remove the operator" },
          ],
        },
        { name: "operatorLegalName", label: "Operator legal name", type: "text", required: true },
        { name: "operatorContact", label: "Operator contact name", type: "text", required: true },
        { name: "operatorEmail", label: "Operator email", type: "email", required: true },
        { name: "agreementStart", label: "Agreement start date", type: "date", required: true },
      ],
      "Provide the third party operator details",
      2,
    ),
  ]
}

/** Temporary suspension request. */
export function temporarySuspensionSteps(): FlowStep[] {
  return [
    step(
      "suspension",
      "Suspension Request",
      [
        { name: "suspensionStart", label: "Suspension start date", type: "date", required: true, validation: { notPast: true } },
        { name: "suspensionEnd", label: "Expected resumption date", type: "date", required: true, validation: afterDate("suspensionStart") },
        { name: "suspensionReason", label: "Reason for suspension", type: "textarea", required: true },
      ],
      "Tell us about the temporary suspension of your licence",
      2,
    ),
  ]
}

/** Temporary extension of licensed area. */
export function temporaryExtensionSteps(): FlowStep[] {
  return [
    step(
      "extension",
      "Temporary Extension",
      [
        { name: "extensionArea", label: "Area to be temporarily added", type: "textarea", required: true },
        { name: "extensionCapacity", label: "Additional capacity (persons)", type: "number", required: true },
        { name: "extensionStart", label: "Start date", type: "date", required: true, validation: { notPast: true } },
        { name: "extensionEnd", label: "End date", type: "date", required: true, validation: afterDate("extensionStart") },
        { name: "extensionReason", label: "Reason / event", type: "textarea", required: true },
      ],
      "Describe the temporary extension of your licensed area",
      2,
    ),
  ]
}

/** Remove a tied house exemption. */
export function tiedHouseRemovalSteps(): FlowStep[] {
  return [
    step(
      "tied-house",
      "Tied House Exemption",
      [
        { name: "exemptionDetails", label: "Which exemption do you want to remove?", type: "textarea", required: true },
        { name: "associatedManufacturer", label: "Associated manufacturer (if any)", type: "text" },
        { name: "removalReason", label: "Reason for removal", type: "textarea", required: true },
      ],
      undefined,
      1,
    ),
  ]
}

/** Generic endorsement application. */
export function endorsementSteps(label: string): FlowStep[] {
  return [
    step(
      "endorsement",
      label,
      [
        { name: "endorsementCapacity", label: "Proposed capacity (persons)", type: "number", required: true },
        { name: "endorsementHours", label: "Proposed hours of service", type: "text", required: true },
        { name: "endorsementDescription", label: "Describe the proposed area and its use", type: "textarea", required: true },
        { name: "endorsementArea", label: "Approximate area (sq. metres)", type: "number" },
      ],
      `Tell us about the proposed ${label.toLowerCase()}`,
      2,
    ),
  ]
}

/** Request for change in terms and conditions / request for discretion. */
export function termsChangeSteps(): FlowStep[] {
  return [
    step(
      "terms",
      "Terms & Conditions",
      [
        { name: "currentTerm", label: "Current term or condition", type: "textarea", required: true },
        { name: "requestedChange", label: "Requested change", type: "textarea", required: true },
        { name: "rationale", label: "Rationale for the request", type: "textarea", required: true },
      ],
      "Describe the change you are requesting",
      1,
    ),
  ]
}

/** A single free-form details step (for one-off applications). */
export function customSteps(title: string, fields: FieldConfig[], description?: string, columns?: 1 | 2 | 3): FlowStep[] {
  const id =
    title
      .toLowerCase()
      .replace(/[^a-z0-9]+/g, "-")
      .replace(/^-+|-+$/g, "") || "details"
  return [step(id, title, fields, description, columns)]
}
