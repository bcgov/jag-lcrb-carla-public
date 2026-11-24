/** PDF of applications awaiting LG/Indigenous Nation comment with due dates; jsPDF is imported dynamically, and "save" downloads while "blob" returns the Blob. */

import { daysUntilDue } from "@domain/lg-review/due-dates"
import { LG_STATUS_LABELS, type LgApplication } from "@domain/models/lg-review"
import { formatLgDate } from "@domain/lg-review/due-dates"

export interface LgReviewReportOptions {
  /** Applications already filtered to what the report should contain. */
  applications: LgApplication[]
  /** Human-readable description of the active filters, shown in the header. */
  filterSummary: string
  /** "save" (default) downloads; "blob" resolves with the PDF Blob. */
  output?: "save" | "blob"
  /** Override the download / attachment filename (without extension). */
  fileBaseName?: string
}

export interface LgReviewReportResult {
  filename: string
  blob?: Blob
  count: number
}

function dueText(iso: string): string {
  const days = daysUntilDue(iso)
  if (days < 0) return `${Math.abs(days)} day${Math.abs(days) === 1 ? "" : "s"} overdue`
  if (days === 0) return "Due today"
  return `${days} day${days === 1 ? "" : "s"} left`
}

export async function generateLgReviewReportPdf(opts: LgReviewReportOptions): Promise<LgReviewReportResult> {
  const { applications, filterSummary, output = "save", fileBaseName } = opts

  const { default: jsPDF } = await import("jspdf")
  const { default: autoTable } = await import("jspdf-autotable")

  const doc = new jsPDF({ orientation: "landscape", unit: "pt", format: "letter" })
  const pageWidth = doc.internal.pageSize.getWidth()
  const marginX = 40
  const navy = [1, 51, 102] as const

  const rows = [...applications].sort((a, b) => a.commentDueDate.localeCompare(b.commentDueDate))

  doc.setFillColor(navy[0], navy[1], navy[2])
  doc.rect(0, 0, pageWidth, 74, "F")
  doc.setTextColor(255, 255, 255)
  doc.setFont("helvetica", "bold")
  doc.setFontSize(16)
  doc.text("Applications Awaiting Review — Due Dates", marginX, 32)
  doc.setFont("helvetica", "normal")
  doc.setFontSize(11)
  doc.text("Local Government / Indigenous Nation comment deadlines", marginX, 52)

  doc.setTextColor(90, 90, 90)
  doc.setFontSize(9)
  const generated = new Date().toLocaleString("en-CA", { dateStyle: "medium", timeStyle: "short" })
  doc.text(`Filters: ${filterSummary}`, marginX, 92)
  doc.text(
    `${rows.length} application${rows.length === 1 ? "" : "s"}  ·  Generated ${generated}`,
    marginX,
    106,
  )

  if (rows.length === 0) {
    doc.setTextColor(120, 120, 120)
    doc.setFontSize(12)
    doc.text("No applications match the selected filters.", marginX, 150)
  } else {
    autoTable(doc, {
      startY: 122,
      margin: { left: marginX, right: marginX },
      head: [["Comment Due", "Remaining", "Job #", "Application", "Establishment", "Applicant / Business", "Status"]],
      body: rows.map((a) => [
        formatLgDate(a.commentDueDate),
        dueText(a.commentDueDate),
        a.jobNumber,
        a.applicationType,
        `${a.establishmentName}\n${a.address}, ${a.city}`,
        `${a.applicantName}\n${a.businessName}`,
        LG_STATUS_LABELS[a.status],
      ]),
      styles: { fontSize: 9, cellPadding: 5, valign: "top", overflow: "linebreak" },
      headStyles: { fillColor: [1, 51, 102], textColor: 255, fontStyle: "bold" },
      alternateRowStyles: { fillColor: [244, 247, 250] },
      columnStyles: {
        0: { cellWidth: 88 },
        1: { cellWidth: 76 },
        2: { cellWidth: 56 },
        3: { cellWidth: 140 },
        4: { cellWidth: 170 },
        5: { cellWidth: 140 },
        6: { cellWidth: 92 },
      },
    })
  }

  const pageCount = doc.getNumberOfPages()
  for (let i = 1; i <= pageCount; i++) {
    doc.setPage(i)
    const h = doc.internal.pageSize.getHeight()
    doc.setTextColor(140, 140, 140)
    doc.setFontSize(8)
    doc.text(
      "Generated from the LCRB portal for local-government review planning. Not an official record.",
      marginX,
      h - 20,
    )
    doc.text(`Page ${i} of ${pageCount}`, pageWidth - marginX, h - 20, { align: "right" })
  }

  const filename = (fileBaseName ?? `LG-review-due-dates-${new Date().toISOString().slice(0, 10)}`) + ".pdf"

  if (output === "blob") {
    return { filename, blob: doc.output("blob"), count: rows.length }
  }
  doc.save(filename)
  return { filename, count: rows.length }
}
