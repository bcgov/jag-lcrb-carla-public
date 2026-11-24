import { type GlossaryEntry } from "@domain/models/applications"


export const GLOSSARY = {
  associate: {
    term: "Associate",
    definition:
      "Anyone who has, or will have, a say in how the business is run or shares in its profits — including directors, officers, shareholders with a significant stake, partners, and anyone financing the business. Associates must be disclosed and may be security screened.",
    example:
      "A silent investor who put up 30% of the start-up money is an associate even if they never set foot in the store.",
  },
  securityScreening: {
    term: "Security screening",
    definition:
      "A background check the LCRB runs on the applicant and each associate. It covers criminal record, past licensing history and links to organised crime, and is required for cannabis retail licences.",
    example: "Each associate submits consent and identification; the Branch requests the checks — you do not arrange them yourself.",
  },
  financialIntegrity: {
    term: "Financial integrity",
    definition:
      "Evidence that the money behind the business comes from legitimate, traceable sources and that the business can meet its obligations. The Branch asks for this to keep illegal cash out of the regulated market.",
    example: "Bank statements, loan agreements and a source-of-funds summary showing where the start-up capital originated.",
  },
  tiedHouse: {
    term: "Tied house",
    definition:
      "A financial or ownership link between a liquor manufacturer and a licensed retailer or bar that could let the manufacturer influence what the venue sells. Such arrangements are restricted.",
  },
  primaryPurpose: {
    term: "Primary purpose",
    definition:
      "What the establishment mainly exists to do — serving food, providing entertainment, selling liquor to take home. The licence class you need depends on it, and the Branch checks that the business actually operates that way.",
  },
} as const satisfies Record<string, GlossaryEntry>

export type GlossaryKey = keyof typeof GLOSSARY

/** Renders a glossary entry as a single help string for `FieldConfig.help`. */
export function helpFor(key: GlossaryKey): string {
  const g: GlossaryEntry = GLOSSARY[key]
  return g.example ? `${g.definition} For example: ${g.example}` : g.definition
}
