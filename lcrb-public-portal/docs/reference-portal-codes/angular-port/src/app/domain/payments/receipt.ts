import { type Receipt } from "@domain/models/payments"

export function formatReceiptCurrency(amount: number, currency = "CAD"): string {
  return new Intl.NumberFormat("en-CA", { style: "currency", currency }).format(amount)
}

export function formatReceiptDate(iso: string): string {
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return iso
  return d.toLocaleString("en-CA", {
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "numeric",
    minute: "2-digit",
  })
}

export function receiptFilename(receipt: Receipt): string {
  return `LCRB-Receipt-${receipt.receiptNumber}.pdf`
}
