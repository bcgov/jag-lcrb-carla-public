import { type CheckOutcome, type DynamicsQueue } from "@domain/models/staff"

export const QUEUE_META: Record<
  DynamicsQueue,
  { label: string; description: string; tone: "danger" | "caution" | "info" | "neutral" }
> = {
  "beyond-licence-cap": {
    label: "Applications Beyond Licence Cap",
    description:
      "The connected business appears at or over the 8-licence cannabis retail cap. Cap determination required.",
    tone: "danger",
  },
  "section-119-review": {
    label: "Section 119 Review",
    description: "First Nations cannabis nexus detected. Confirm s.119 agreement coverage.",
    tone: "caution",
  },
  "enhanced-review": {
    label: "Enhanced Review",
    description: "Inconsistencies or risk signals warrant a closer look before processing.",
    tone: "caution",
  },
  "standard-processing": {
    label: "Standard Processing",
    description: "No elevated signals detected. Route through the normal workflow.",
    tone: "neutral",
  },
}

export const CHECK_OUTCOME_META: Record<
  CheckOutcome,
  { label: string; tone: "positive" | "caution" | "danger" }
> = {
  clear: { label: "Clear", tone: "positive" },
  attention: { label: "Attention", tone: "caution" },
  flag: { label: "Flag", tone: "danger" },
}

export function confidenceBand(confidence: number): "high" | "medium" | "low" {
  if (confidence >= 80) return "high"
  if (confidence >= 55) return "medium"
  return "low"
}
