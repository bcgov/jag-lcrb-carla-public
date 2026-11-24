/** Payment receipt PDF sharing the roll-up header/footer; production should generate it server-side from the PayBC transaction. */

import { jsPDF } from "jspdf"
import autoTable from "jspdf-autotable"
import { formatReceiptCurrency, formatReceiptDate, receiptFilename } from "@domain/payments/receipt"
import { type Receipt } from "@domain/models/payments"

const BC_BLUE: [number, number, number] = [1, 51, 102]
const BC_GOLD: [number, number, number] = [252, 186, 25]
const TEXT: [number, number, number] = [45, 45, 45]
const MUTED: [number, number, number] = [71, 69, 67]
const GREEN: [number, number, number] = [46, 133, 64]

const STATUS_LABEL: Record<Receipt["status"], string> = {
  paid: "PAID",
  refunded: "REFUNDED",
  "partially-refunded": "PARTIALLY REFUNDED",
}

export function buildReceiptPdf(receipt: Receipt): jsPDF {
  const doc = new jsPDF({ unit: "pt", format: "letter" })
  const pageWidth = doc.internal.pageSize.getWidth()
  const pageHeight = doc.internal.pageSize.getHeight()
  const margin = 48
  const money = (n: number) => formatReceiptCurrency(n, receipt.currency)

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
  doc.setFont("helvetica", "bold")
  doc.setFontSize(12)
  doc.text("OFFICIAL RECEIPT", pageWidth - margin, 32, { align: "right" })
  doc.setFont("helvetica", "normal")
  doc.setFontSize(10)
  doc.text(receipt.receiptNumber, pageWidth - margin, 50, { align: "right" })

  let y = 104

  doc.setTextColor(...TEXT)
  doc.setFont("helvetica", "bold")
  doc.setFontSize(18)
  doc.text("Payment receipt", margin, y)
  const stamp = STATUS_LABEL[receipt.status]
  doc.setFontSize(10)
  const stampWidth = doc.getTextWidth(stamp) + 16
  doc.setDrawColor(...(receipt.status === "paid" ? GREEN : MUTED))
  doc.setTextColor(...(receipt.status === "paid" ? GREEN : MUTED))
  doc.setLineWidth(1.2)
  doc.roundedRect(pageWidth - margin - stampWidth, y - 13, stampWidth, 20, 3, 3, "S")
  doc.text(stamp, pageWidth - margin - stampWidth / 2, y + 1, { align: "center" })
  y += 14

  doc.setTextColor(...MUTED)
  doc.setFont("helvetica", "normal")
  doc.setFontSize(10)
  doc.text(`Issued ${formatReceiptDate(receipt.paidAt)}`, margin, y + 6)
  y += 24

  const refs: string[][] = [
    ["Receipt number", receipt.receiptNumber],
    ["Application reference", receipt.jobNumber],
  ]
  if (receipt.confirmationNumber) refs.push(["Confirmation number", receipt.confirmationNumber])
  refs.push(["Paid by", receipt.payerName])
  if (receipt.payerEmail) refs.push(["Email", receipt.payerEmail])
  refs.push(["Payment method", receipt.method || "—"])
  if (receipt.processorRef) refs.push(["Processor reference", receipt.processorRef])

  autoTable(doc, {
    startY: y,
    theme: "plain",
    body: refs,
    styles: { fontSize: 10, cellPadding: 4, textColor: TEXT },
    columnStyles: { 0: { fontStyle: "bold", cellWidth: 150, textColor: MUTED }, 1: { cellWidth: "auto" } },
    margin: { left: margin, right: margin },
  })
  // @ts-expect-error lastAutoTable is augmented at runtime by jspdf-autotable
  y = doc.lastAutoTable.finalY + 20

  doc.setFillColor(240, 243, 248)
  doc.rect(margin, y - 12, pageWidth - margin * 2, 22, "F")
  doc.setTextColor(...BC_BLUE)
  doc.setFont("helvetica", "bold")
  doc.setFontSize(12)
  doc.text(receipt.title, margin + 6, y + 3)
  y += 22

  const rows: string[][] = receipt.lineItems.map((l) => [l.label, money(l.amount)])
  const foot: string[][] = []
  if (receipt.serviceFee > 0) {
    foot.push(["Subtotal", money(receipt.subtotal)])
    foot.push(["Service fee", money(receipt.serviceFee)])
  }
  foot.push(["Total paid", money(receipt.total)])
  if (receipt.refundedAmount > 0) {
    foot.push(["Refunded", `-${money(receipt.refundedAmount)}`])
    foot.push(["Net", money(receipt.total - receipt.refundedAmount)])
  }

  autoTable(doc, {
    startY: y,
    theme: "grid",
    head: [["Description", "Amount"]],
    body: rows,
    foot,
    styles: { fontSize: 10, cellPadding: 6, textColor: TEXT, lineColor: [217, 217, 217] },
    headStyles: { fillColor: BC_BLUE, textColor: [255, 255, 255] },
    footStyles: { fillColor: [250, 250, 250], textColor: TEXT, fontStyle: "bold" },
    columnStyles: { 0: { cellWidth: "auto" }, 1: { cellWidth: 120, halign: "right" } },
    margin: { left: margin, right: margin },
  })
  // @ts-expect-error runtime augmentation
  y = doc.lastAutoTable.finalY + 24

  doc.setTextColor(...MUTED)
  doc.setFont("helvetica", "normal")
  doc.setFontSize(9)
  const notes = [
    "Keep this receipt for your records. Application fees are non-refundable once an application has been accepted for review.",
    "Questions about this payment? Contact the Liquor and Cannabis Regulation Branch at 1-866-209-2111 and quote the receipt number above.",
    "This receipt was generated by the LCRB Licensing Portal and is valid without a signature.",
  ]
  for (const note of notes) {
    const lines = doc.splitTextToSize(note, pageWidth - margin * 2)
    doc.text(lines, margin, y)
    y += lines.length * 12 + 6
  }

  doc.setDrawColor(217, 217, 217)
  doc.line(margin, pageHeight - 36, pageWidth - margin, pageHeight - 36)
  doc.setFontSize(8)
  doc.text(`Generated ${new Date().toLocaleString("en-CA")}`, margin, pageHeight - 22)
  doc.text("Liquor and Cannabis Regulation Branch · PO Box 9292 Stn Prov Govt, Victoria BC V8W 9J8", margin, pageHeight - 12)
  doc.text(receipt.receiptNumber, pageWidth - margin, pageHeight - 22, { align: "right" })
  doc.text("Page 1 of 1", pageWidth - margin, pageHeight - 12, { align: "right" })

  return doc
}

export function downloadReceiptPdf(receipt: Receipt): void {
  buildReceiptPdf(receipt).save(receiptFilename(receipt))
}

/** Byte size of the rendered PDF — used for the attachment chip in the email log. */
export function receiptPdfSizeBytes(receipt: Receipt): number {
  return buildReceiptPdf(receipt).output("arraybuffer").byteLength
}
