/** Client-side roll-up PDF of a whole application (sections, parties, documents, status history); production should render server-side so it is tamper-evident. */

import { jsPDF } from "jspdf"
import autoTable from "jspdf-autotable"
import { type Licence } from "@domain/models/licences"
import { formatLicenceDate, getLicenceTerms } from "@domain/licences/licences"

export interface PdfField {
  label: string
  value: string
}

export interface PdfSection {
  heading: string
  /** Simple key/value rows */
  fields?: PdfField[]
  /** Optional tabular data with column headers */
  table?: { columns: string[]; rows: string[][] }
  /** Optional free-text note rendered under the heading */
  note?: string
}

export interface RollupData {
  title: string
  applicationNumber: string
  applicantName: string
  licenceType: string
  status: string
  submittedDate?: string
  sections: PdfSection[]
  /** Optional footer label, e.g. the reviewing role */
  preparedFor?: string
}

const BC_BLUE: [number, number, number] = [1, 51, 102]
const BC_GOLD: [number, number, number] = [252, 186, 25]
const TEXT: [number, number, number] = [45, 45, 45]
const MUTED: [number, number, number] = [71, 69, 67]

export function generateApplicationPdf(data: RollupData): void {
  const doc = new jsPDF({ unit: "pt", format: "letter" })
  const pageWidth = doc.internal.pageSize.getWidth()
  const pageHeight = doc.internal.pageSize.getHeight()
  const margin = 48
  let cursorY = margin

  doc.setFillColor(...BC_BLUE)
  doc.rect(0, 0, pageWidth, 70, "F")
  doc.setFillColor(...BC_GOLD)
  doc.rect(0, 70, pageWidth, 4, "F")
  doc.setTextColor(255, 255, 255)
  doc.setFont("helvetica", "bold")
  doc.setFontSize(16)
  doc.text("Province of British Columbia", margin, 32)
  doc.setFontSize(11)
  doc.setFont("helvetica", "normal")
  doc.text("Liquor and Cannabis Regulation Branch", margin, 50)
  cursorY = 100

  doc.setTextColor(...TEXT)
  doc.setFont("helvetica", "bold")
  doc.setFontSize(18)
  const titleLines = doc.splitTextToSize(data.title, pageWidth - margin * 2)
  doc.text(titleLines, margin, cursorY)
  cursorY += titleLines.length * 22 + 6

  const meta: PdfField[] = [
    { label: "Application #", value: data.applicationNumber },
    { label: "Applicant", value: data.applicantName },
    { label: "Licence type", value: data.licenceType },
    { label: "Status", value: data.status },
  ]
  if (data.submittedDate) meta.push({ label: "Submitted", value: data.submittedDate })

  autoTable(doc, {
    startY: cursorY,
    theme: "plain",
    body: meta.map((m) => [m.label, m.value]),
    styles: { fontSize: 10, cellPadding: 4, textColor: TEXT },
    columnStyles: {
      0: { fontStyle: "bold", cellWidth: 120, textColor: MUTED },
      1: { cellWidth: "auto" },
    },
    margin: { left: margin, right: margin },
  })
  // @ts-expect-error lastAutoTable is augmented at runtime by jspdf-autotable
  cursorY = doc.lastAutoTable.finalY + 18

  for (const section of data.sections) {
    const blockEstimate = 60
    if (cursorY + blockEstimate > pageHeight - margin) {
      doc.addPage()
      cursorY = margin
    }

    doc.setFillColor(240, 243, 248)
    doc.rect(margin, cursorY - 12, pageWidth - margin * 2, 22, "F")
    doc.setTextColor(...BC_BLUE)
    doc.setFont("helvetica", "bold")
    doc.setFontSize(12)
    doc.text(section.heading, margin + 6, cursorY + 3)
    cursorY += 22

    if (section.note) {
      doc.setTextColor(...MUTED)
      doc.setFont("helvetica", "italic")
      doc.setFontSize(9)
      const noteLines = doc.splitTextToSize(section.note, pageWidth - margin * 2)
      doc.text(noteLines, margin, cursorY)
      cursorY += noteLines.length * 12 + 4
    }

    if (section.fields && section.fields.length > 0) {
      autoTable(doc, {
        startY: cursorY,
        theme: "grid",
        body: section.fields.map((f) => [f.label, f.value]),
        styles: { fontSize: 10, cellPadding: 5, textColor: TEXT, lineColor: [217, 217, 217] },
        columnStyles: {
          0: { fontStyle: "bold", cellWidth: 180, textColor: MUTED },
          1: { cellWidth: "auto" },
        },
        margin: { left: margin, right: margin },
      })
      // @ts-expect-error runtime augmentation
      cursorY = doc.lastAutoTable.finalY + 14
    }

    if (section.table) {
      autoTable(doc, {
        startY: cursorY,
        theme: "striped",
        head: [section.table.columns],
        body: section.table.rows,
        styles: { fontSize: 9, cellPadding: 5, textColor: TEXT },
        headStyles: { fillColor: BC_BLUE, textColor: [255, 255, 255] },
        margin: { left: margin, right: margin },
      })
      // @ts-expect-error runtime augmentation
      cursorY = doc.lastAutoTable.finalY + 14
    }
  }

  const pageCount = doc.getNumberOfPages()
  for (let i = 1; i <= pageCount; i++) {
    doc.setPage(i)
    doc.setDrawColor(217, 217, 217)
    doc.line(margin, pageHeight - 36, pageWidth - margin, pageHeight - 36)
    doc.setFontSize(8)
    doc.setTextColor(...MUTED)
    doc.setFont("helvetica", "normal")
    const generated = `Generated ${new Date().toLocaleString("en-CA")}`
    doc.text(generated, margin, pageHeight - 22)
    if (data.preparedFor) {
      doc.text(`Prepared for: ${data.preparedFor}`, margin, pageHeight - 12)
    }
    doc.text(`Page ${i} of ${pageCount}`, pageWidth - margin, pageHeight - 22, { align: "right" })
    doc.text(data.applicationNumber, pageWidth - margin, pageHeight - 12, { align: "right" })
  }

  doc.save(`${data.applicationNumber}-${data.licenceType.replace(/\s+/g, "-")}.pdf`)
}

/** Map a licence into the roll-up shape consumed by the PDF generator. */
export function licenceToRollup(licence: Licence): RollupData {
  return {
    title: `${licence.type} Licence`,
    applicationNumber: licence.id,
    applicantName: licence.operatedBy,
    licenceType: licence.type,
    status: licence.status,
    preparedFor: "Licensee",
    sections: [
      {
        heading: "Licence Details",
        fields: [
          { label: "Establishment", value: licence.name },
          { label: "Licence Number", value: licence.id },
          { label: "Type", value: licence.type },
          { label: "Status", value: licence.status },
          { label: "Expiry / Renewal Date", value: formatLicenceDate(licence.expiryDate) },
          { label: "Operated By", value: licence.operatedBy },
        ],
      },
      {
        heading: "Location & Contact",
        fields: [
          { label: "Address", value: licence.address },
          { label: "Phone", value: licence.phone },
          { label: "Email", value: licence.email },
        ],
      },
      {
        heading: "Terms & Conditions",
        fields: getLicenceTerms(licence.type).map((t) => ({ label: t.heading, value: t.body })),
      },
    ],
  }
}
