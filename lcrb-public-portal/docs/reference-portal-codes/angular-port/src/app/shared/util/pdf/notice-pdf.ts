/** PDF copy of an LCRB notice formatted as an official letter; jsPDF is imported dynamically. */

import { formatNoticeDate } from "@domain/notifications/notices"
import { type Notice } from "@domain/models/notifications"

// BC government brand navy (#013366).
const NAVY: [number, number, number] = [1, 51, 102]
const INK: [number, number, number] = [33, 33, 33]
const MUTED: [number, number, number] = [110, 110, 110]

export async function generateNoticePdf(notice: Notice): Promise<void> {
  const { jsPDF } = await import("jspdf")
  const doc = new jsPDF({ unit: "mm", format: "a4" })

  const pageW = doc.internal.pageSize.getWidth()
  const pageH = doc.internal.pageSize.getHeight()
  const margin = 20
  const contentW = pageW - margin * 2
  const bottom = pageH - margin
  const d = notice.document

  let y = margin

  const ensureSpace = (needed: number) => {
    if (y + needed > bottom) {
      drawFooter()
      doc.addPage()
      y = margin
    }
  }

  const drawFooter = () => {
    doc.setFont("helvetica", "normal")
    doc.setFontSize(7.5)
    doc.setTextColor(...MUTED)
    doc.text(
      "Liquor and Cannabis Regulation Branch  •  PO Box 9292 Stn Prov Govt, Victoria, BC  V8W 9J8  •  1-866-209-2111",
      pageW / 2,
      pageH - 12,
      { align: "center" },
    )
    doc.text(`${d.documentNumber}`, pageW / 2, pageH - 8, { align: "center" })
  }

  doc.setFont("helvetica", "bold")
  doc.setFontSize(15)
  doc.setTextColor(...NAVY)
  doc.text("BRITISH COLUMBIA", margin, y)
  y += 6
  doc.setFontSize(11)
  doc.text("Liquor and Cannabis Regulation Branch", margin, y)
  y += 5
  doc.setFont("helvetica", "normal")
  doc.setFontSize(8.5)
  doc.setTextColor(...MUTED)
  doc.text("Ministry of Public Safety and Solicitor General", margin, y)
  y += 3
  doc.setDrawColor(...NAVY)
  doc.setLineWidth(0.6)
  doc.line(margin, y, pageW - margin, y)
  y += 10

  doc.setFont("helvetica", "normal")
  doc.setFontSize(10)
  doc.setTextColor(...INK)
  doc.text(formatNoticeDate(notice.date), margin, y)
  doc.setTextColor(...MUTED)
  doc.text(`File: ${d.documentNumber}`, pageW - margin, y, { align: "right" })
  y += 10

  doc.setTextColor(...INK)
  const recipientLines = [d.recipientName, d.recipientBusiness, ...d.recipientAddress]
  for (const line of recipientLines) {
    doc.text(line, margin, y)
    y += 5
  }
  y += 6

  doc.setFont("helvetica", "bold")
  const subjectLines = doc.splitTextToSize(`RE: ${d.subject}`, contentW) as string[]
  for (const line of subjectLines) {
    ensureSpace(6)
    doc.text(line, margin, y)
    y += 5.5
  }
  y += 4

  doc.setFont("helvetica", "normal")
  ensureSpace(6)
  doc.text(d.salutation, margin, y)
  y += 8

  const lineH = 5.4
  for (const para of d.body) {
    const lines = doc.splitTextToSize(para, contentW) as string[]
    for (const line of lines) {
      ensureSpace(lineH)
      doc.text(line, margin, y)
      y += lineH
    }
    y += 4 // paragraph gap
  }

  y += 4
  ensureSpace(28)
  doc.text(d.closing, margin, y)
  y += 14 // space for a signature
  doc.setFont("helvetica", "bold")
  doc.text(d.signatoryName, margin, y)
  y += 5
  doc.setFont("helvetica", "normal")
  doc.setTextColor(...MUTED)
  doc.text(d.signatoryTitle, margin, y)
  y += 5
  doc.text(d.signatoryBranch, margin, y)
  y += 8
  doc.setTextColor(...INK)

  if (d.enclosures?.length) {
    ensureSpace(6 + d.enclosures.length * 5)
    doc.setFont("helvetica", "bold")
    doc.text("Enclosures", margin, y)
    y += 5
    doc.setFont("helvetica", "normal")
    for (const enc of d.enclosures) {
      ensureSpace(5)
      doc.text(`•  ${enc}`, margin, y)
      y += 5
    }
    y += 3
  }

  if (d.references?.length) {
    ensureSpace(6 + d.references.length * 5)
    doc.setFont("helvetica", "italic")
    doc.setFontSize(9)
    doc.setTextColor(...MUTED)
    doc.text("Authority:", margin, y)
    y += 5
    for (const ref of d.references) {
      ensureSpace(5)
      doc.text(ref, margin, y)
      y += 4.5
    }
  }

  drawFooter()

  const safeRef = notice.reference.replace(/[^a-z0-9-]/gi, "")
  doc.save(`LCRB-Notice-${safeRef}.pdf`)
}
