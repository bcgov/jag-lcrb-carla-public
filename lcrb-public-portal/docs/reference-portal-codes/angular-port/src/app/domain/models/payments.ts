export type ReceiptStatus = "paid" | "refunded" | "partially-refunded"

export interface ReceiptLine {
  label: string
  amount: number
}

export interface Receipt {
  receiptNumber: string
  /** Portal confirmation number (PAY-…), when the payment came from a submission. */
  confirmationNumber?: string
  /** Application / permit reference the payment relates to. */
  jobNumber: string
  /** Link to the application record, when we have one. */
  applicationHref?: string
  /** What was paid for, e.g. "Liquor Primary Licence Application". */
  title: string
  payerName: string
  payerEmail?: string
  lineItems: ReceiptLine[]
  subtotal: number
  serviceFee: number
  total: number
  currency: string
  method: string
  processorRef?: string
  status: ReceiptStatus
  paidAt: string
  refundedAmount: number
}
