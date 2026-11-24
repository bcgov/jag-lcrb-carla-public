/** SEP PDF in two modes: "full" application roll-up, or a condensed "field" copy for officers with selectable sections. */

import { generateApplicationPdf, type PdfSection } from "./pdf-rollup"
import { SEP_STATUS_LABELS, formatEventDate } from "@domain/special-events/seps"
import { type SepRecord } from "@domain/models/special-events"

export interface SepPdfOptions {
  /** Sections the officer chose to include in a field copy. */
  include?: {
    applicant?: boolean
    logistics?: boolean
    liquor?: boolean
    conditions?: boolean
    history?: boolean
  }
  preparedFor?: string
}

function applicantSection(record: SepRecord): PdfSection {
  return {
    heading: "Applicant & Organizer",
    fields: [
      { label: "Applicant", value: record.applicantName },
      { label: "Organization", value: record.applicantOrg || "—" },
      { label: "Phone", value: record.applicantPhone },
      { label: "Email", value: record.applicantEmail },
    ],
  }
}

function logisticsSection(record: SepRecord): PdfSection {
  return {
    heading: "Event Logistics",
    fields: [
      { label: "Event", value: record.eventName },
      { label: "Type", value: record.eventType },
      { label: "Date(s)", value: formatEventDate(record) },
      { label: "Time", value: `${record.startTime} – ${record.endTime}` },
      { label: "Venue", value: record.venueName },
      { label: "Address", value: `${record.address}, ${record.city}` },
      { label: "Jurisdiction", value: record.jurisdiction },
      { label: "Expected attendance", value: String(record.expectedAttendance) },
      { label: "Minors expected", value: record.minorsExpected ? "Yes" : "No" },
      { label: "Site plan provided", value: record.siteplanProvided ? "Yes" : "No" },
    ],
  }
}

function liquorSection(record: SepRecord): PdfSection {
  return {
    heading: "Liquor Service",
    fields: [{ label: "Service type", value: record.serviceType }],
    note: record.description,
  }
}

function conditionsSection(record: SepRecord): PdfSection {
  return {
    heading: "Permit Conditions",
    table: {
      columns: ["#", "Condition", "Type"],
      rows: record.conditions.length
        ? record.conditions.map((c, i) => [
            String(i + 1),
            c.text,
            c.standard ? "Standard" : "Custom",
          ])
        : [["—", "No conditions recorded", "—"]],
    },
  }
}

function historySection(record: SepRecord): PdfSection {
  return {
    heading: "Review History",
    table: {
      columns: ["Date", "Actor", "Action", "Note"],
      rows: record.history.map((h) => [h.date, h.actor, h.action, h.note || ""]),
    },
  }
}

export function downloadSepPdf(
  record: SepRecord,
  modeOrOptions: "full" | "field" | "permit" | { variant?: "full" | "field" | "permit" } = "full",
  options: SepPdfOptions = {},
): void {
  // Accepts downloadSepPdf(record, "permit") or downloadSepPdf(record, { variant: "permit" }).
  const mode =
    typeof modeOrOptions === "string" ? modeOrOptions : modeOrOptions.variant ?? "full"

  const include = options.include ?? {
    applicant: true,
    logistics: true,
    liquor: true,
    conditions: true,
    history: mode === "full",
  }

  const sections: PdfSection[] = []

  // The issued permit leads with an official issuance/validity block.
  if (mode === "permit") {
    const issued = record.history.find((h) => /approv/i.test(h.action))
    sections.push({
      heading: "Permit Issuance",
      fields: [
        { label: "Permit number", value: record.id },
        { label: "Status", value: SEP_STATUS_LABELS[record.status] },
        { label: "Issued", value: issued?.date || record.submittedDate },
        { label: "Issued by", value: issued?.actor || "LCRB Special Events" },
        { label: "Valid for", value: formatEventDate(record) },
        { label: "Venue", value: `${record.venueName}, ${record.city}` },
      ],
      note: "This permit must be available on-site for the duration of the event and is subject to the conditions below.",
    })
  }

  if (include.applicant) sections.push(applicantSection(record))
  if (include.logistics) sections.push(logisticsSection(record))
  if (include.liquor) sections.push(liquorSection(record))
  if (include.conditions) sections.push(conditionsSection(record))
  if (include.history) sections.push(historySection(record))

  const title =
    mode === "field"
      ? `SEP Field Copy — ${record.eventName}`
      : mode === "permit"
        ? `Special Event Permit (Issued) — ${record.eventName}`
        : `Special Event Permit — ${record.eventName}`

  generateApplicationPdf({
    title,
    applicationNumber: record.id,
    applicantName: record.applicantName,
    licenceType: record.eventType,
    status: SEP_STATUS_LABELS[record.status],
    submittedDate: record.submittedDate,
    sections,
    preparedFor: options.preparedFor,
  })
}
