"use client"

/** Payment service (prototype) - mock boundary for PayBC / Bambora. */

import { createCollection, makeId } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { recordAudit } from "../activity/audit-service"

export type PaymentStatus = "pending" | "paid" | "refunded" | "partially-refunded" | "failed"

export interface PaymentLineItem {
  label: string
  amount: number
  /** Optional fee code mapped to the LCRB fee schedule. */
  code?: string
}

export interface Refund {
  id: string
  amount: number
  reason: string
  at: string
  actor: string
}

export interface Transaction extends Entity {
  /** Human-facing receipt number, e.g. "RCPT-2026-4F9C2A". */
  receiptNumber: string
  /** What this payment is for. */
  description: string
  /** Related application/permit reference. */
  subjectId?: string
  payerName: string
  lineItems: PaymentLineItem[]
  subtotal: number
  /** Service/processing fee if any. */
  serviceFee: number
  total: number
  currency: string
  status: PaymentStatus
  /** Masked instrument, e.g. "Visa ending in 4242". */
  method: string
  /** Processor reference id (mock). */
  processorRef: string
  createdAt: string
  paidAt?: string
  refunds: Refund[]
}

const collection = createCollection<Transaction>("transactions")

export function makeReceiptNumber(): string {
  const rand = Math.random().toString(16).slice(2, 8).toUpperCase()
  return `RCPT-${new Date().getFullYear()}-${rand}`
}

function sum(items: PaymentLineItem[]): number {
  return items.reduce((acc, i) => acc + i.amount, 0)
}

export interface CreatePaymentInput {
  description: string
  subjectId?: string
  payerName: string
  lineItems: PaymentLineItem[]
  /** Defaults to 0. Card service fees can be modeled here. */
  serviceFee?: number
  currency?: string
}

/** Create a pending transaction (a "payment intent"). */
export function createPayment(input: CreatePaymentInput): Transaction {
  const subtotal = sum(input.lineItems)
  const serviceFee = input.serviceFee ?? 0
  const tx: Transaction = {
    id: makeId("txn"),
    receiptNumber: makeReceiptNumber(),
    description: input.description,
    subjectId: input.subjectId,
    payerName: input.payerName,
    lineItems: input.lineItems,
    subtotal,
    serviceFee,
    total: subtotal + serviceFee,
    currency: input.currency ?? "CAD",
    status: subtotal + serviceFee === 0 ? "paid" : "pending",
    method: "",
    processorRef: "",
    createdAt: new Date().toISOString(),
    paidAt: subtotal + serviceFee === 0 ? new Date().toISOString() : undefined,
    refunds: [],
  }
  collection.upsert(tx)
  return tx
}

export interface ProcessPaymentInput {
  transactionId: string
  /** Masked card label captured from the (mock) pay page. */
  method: string
  actor: string
}

/** Simulate processing a payment at PayBC/Bambora. */
export function processPayment(input: ProcessPaymentInput): Transaction | undefined {
  const tx = collection.get(input.transactionId)
  if (!tx) return undefined
  const paid = collection.update(tx.id, {
    status: "paid",
    method: input.method,
    processorRef: `PAYBC-${makeId("ref").toUpperCase()}`,
    paidAt: new Date().toISOString(),
  })
  recordAudit({
    category: "payment",
    action: "payment.captured",
    summary: `Payment of $${tx.total.toLocaleString()} captured for ${tx.description}`,
    actor: input.actor,
    subjectId: tx.subjectId,
    meta: { receiptNumber: tx.receiptNumber, total: tx.total },
  })
  return paid
}

export interface RefundInput {
  transactionId: string
  amount: number
  reason: string
  actor: string
}

export function refundPayment(input: RefundInput): Transaction | undefined {
  const tx = collection.get(input.transactionId)
  if (!tx || tx.status === "pending" || tx.status === "failed") return undefined
  const refund: Refund = {
    id: makeId("rfnd"),
    amount: input.amount,
    reason: input.reason,
    at: new Date().toISOString(),
    actor: input.actor,
  }
  const refundedTotal = tx.refunds.reduce((a, r) => a + r.amount, 0) + input.amount
  const status: PaymentStatus = refundedTotal >= tx.total ? "refunded" : "partially-refunded"
  const updated = collection.update(tx.id, {
    refunds: [...tx.refunds, refund],
    status,
  })
  recordAudit({
    category: "payment",
    action: "payment.refunded",
    summary: `Refund of $${input.amount.toLocaleString()} issued for ${tx.description} (${input.reason})`,
    actor: input.actor,
    subjectId: tx.subjectId,
    meta: { receiptNumber: tx.receiptNumber, amount: input.amount },
  })
  return updated
}

export function getTransaction(id: string): Transaction | undefined {
  return collection.get(id)
}

export function getTransactionByReceipt(receiptNumber: string): Transaction | undefined {
  return collection.list().find((t) => t.receiptNumber === receiptNumber)
}

export function listTransactions(): Transaction[] {
  return collection.list().sort((a, b) => (a.createdAt < b.createdAt ? 1 : -1))
}

export function listTransactionsForSubject(subjectId: string): Transaction[] {
  return listTransactions().filter((t) => t.subjectId === subjectId)
}

export const transactionCollection = collection
