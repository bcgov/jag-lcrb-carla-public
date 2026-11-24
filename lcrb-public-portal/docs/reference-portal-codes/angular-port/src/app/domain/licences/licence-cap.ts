import { type CapStatus } from "@domain/models/licences"

/** Maximum cannabis retail store licences per business group. */
export const CANNABIS_LICENCE_CAP = 8

export const CAP_STATUS_META: Record<CapStatus, { label: string; tone: "positive" | "caution" | "danger" | "neutral" }> = {
  none: { label: "No cap", tone: "neutral" },
  under: { label: "Under cap", tone: "positive" },
  near: { label: "Near cap", tone: "caution" },
  at: { label: "At cap", tone: "danger" },
  over: { label: "Over cap", tone: "danger" },
}
