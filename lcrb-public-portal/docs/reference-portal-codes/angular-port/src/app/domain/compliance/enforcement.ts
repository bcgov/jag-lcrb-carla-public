import { type ContraventionOutcome } from "@domain/models/compliance"
import { type EnforcementStatus } from "@domain/models/compliance"


export const OUTCOME_META: Record<
  ContraventionOutcome,
  { label: string; badge: string }
> = {
  "no-contravention": {
    label: "No contravention",
    badge: "bg-green-100 text-green-800 border-green-200",
  },
  warning: { label: "Warning", badge: "bg-yellow-100 text-yellow-800 border-yellow-200" },
  "monetary-penalty": {
    label: "Monetary penalty",
    badge: "bg-orange-100 text-orange-800 border-orange-200",
  },
  suspension: { label: "Suspension", badge: "bg-red-100 text-red-800 border-red-200" },
  cancellation: { label: "Cancellation", badge: "bg-red-100 text-red-900 border-red-300" },
}

export function formatEnforcementDate(iso: string): string {
  return new Date(iso).toLocaleDateString("en-CA", {
    year: "numeric",
    month: "long",
    day: "numeric",
  })
}

export function daysUntil(iso: string): number {
  const now = new Date()
  now.setHours(0, 0, 0, 0)
  const due = new Date(iso)
  due.setHours(0, 0, 0, 0)
  return Math.round((due.getTime() - now.getTime()) / (24 * 60 * 60 * 1000))
}

export function formatCurrency(amount: number): string {
  return amount.toLocaleString("en-CA", { style: "currency", currency: "CAD", maximumFractionDigits: 0 })
}

export const STATUS_META: Record<
  EnforcementStatus,
  { label: string; badge: string; tone: "danger" | "caution" | "info" | "positive" }
> = {
  "awaiting-response": {
    label: "Awaiting your response",
    badge: "bg-red-100 text-red-800 border-red-200",
    tone: "danger",
  },
  "response-submitted": {
    label: "Response submitted",
    badge: "bg-blue-100 text-blue-800 border-blue-200",
    tone: "info",
  },
  "hearing-requested": {
    label: "Hearing requested",
    badge: "bg-orange-100 text-orange-800 border-orange-200",
    tone: "caution",
  },
  "hearing-scheduled": {
    label: "Hearing scheduled",
    badge: "bg-orange-100 text-orange-800 border-orange-200",
    tone: "caution",
  },
  resolved: {
    label: "Resolved",
    badge: "bg-green-100 text-green-800 border-green-200",
    tone: "positive",
  },
}
