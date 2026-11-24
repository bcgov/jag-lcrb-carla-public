import { LG_STATUS_LABELS, type LgApplication } from "@domain/models/lg-review"
import { formatLgDate } from "@domain/lg-review/due-dates"
import { generateApplicationPdf, type RollupData } from "./pdf-rollup"

/** Builds the single roll-up PDF for a Local Government application review and downloads it. */
export function downloadLgApplicationPdf(app: LgApplication, preparedFor: string): void {
  const data: RollupData = {
    title: `${app.establishmentName} — ${app.applicationType}`,
    applicationNumber: `Job #${app.jobNumber}`,
    applicantName: app.applicantName,
    licenceType: app.licenceType,
    status: LG_STATUS_LABELS[app.status],
    submittedDate: formatLgDate(app.submittedDate),
    preparedFor,
    sections: [
      {
        heading: "Application summary",
        fields: [
          { label: "Application type", value: app.applicationType },
          { label: "Licence type", value: app.licenceType },
          { label: "Submitted", value: formatLgDate(app.submittedDate) },
          { label: "Comment due", value: formatLgDate(app.commentDueDate) },
          { label: "Jurisdiction", value: app.jurisdiction },
          { label: "Current status", value: LG_STATUS_LABELS[app.status] },
        ],
        note: app.description,
      },
      {
        heading: "Establishment",
        fields: [
          { label: "Name", value: app.establishmentName },
          { label: "Address", value: `${app.address}, ${app.city} ${app.postalCode}` },
          { label: "Risk level", value: app.riskLevel },
          { label: "Contraventions on file", value: String(app.contraventions) },
          { label: "Incidents on file", value: String(app.incidents) },
        ],
      },
      {
        heading: "Business & applicant",
        fields: [
          { label: "Business", value: app.businessName },
          { label: "Applicant", value: app.applicantName },
        ],
      },
      {
        heading: "Hours of operation",
        table: {
          columns: ["Day", "Hours"],
          rows: app.hours.map((h) => [h.day, h.hours]),
        },
      },
      {
        heading: "Contacts",
        table: {
          columns: ["Name", "Role", "Phone", "Email"],
          rows: app.contacts.map((c) => [c.name, c.role, c.phone, c.email]),
        },
      },
      {
        heading: "Documents",
        table: {
          columns: ["Document", "Uploaded by", "Date"],
          rows: app.documents.map((d) => [d.name, d.uploadedBy, formatLgDate(d.uploadedDate)]),
        },
      },
      {
        heading: "Status history",
        table: {
          columns: ["Event", "By", "Date"],
          rows: app.history.map((h) => [h.label, h.actor, formatLgDate(h.date)]),
        },
      },
    ],
  }

  generateApplicationPdf(data)
}

/** Downloads one attached document as a PDF cover sheet; a production build would stream the original file. */
export function downloadLgDocumentPdf(
  app: LgApplication,
  doc: LgApplication["documents"][number],
  preparedFor: string,
): void {
  const data: RollupData = {
    title: doc.name,
    applicationNumber: `Job #${app.jobNumber}`,
    applicantName: app.applicantName,
    licenceType: app.licenceType,
    status: LG_STATUS_LABELS[app.status],
    submittedDate: formatLgDate(app.submittedDate),
    preparedFor,
    sections: [
      {
        heading: "Document details",
        fields: [
          { label: "Document", value: doc.name },
          { label: "Uploaded by", value: doc.uploadedBy },
          { label: "Uploaded", value: formatLgDate(doc.uploadedDate) },
          { label: "Establishment", value: app.establishmentName },
          { label: "Address", value: `${app.address}, ${app.city} ${app.postalCode}` },
        ],
        note:
          "This document is part of the application package submitted to the Liquor and Cannabis Regulation Branch. Retain for your records.",
      },
    ],
  }

  generateApplicationPdf(data)
}
