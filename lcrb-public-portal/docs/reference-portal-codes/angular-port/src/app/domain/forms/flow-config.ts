/** Framework-free flow types (steps, sections, fields) for the metadata-driven application framework. */
import type { FieldConfig } from "./field-schema"

export interface FlowSection {
  title: string
  description?: string
  /** Optional informational callout shown above the fields */
  info?: { variant?: "info" | "warning" | "success" | "error"; title?: string; body: string }
  fields: FieldConfig[]
  /** Lay fields out in a responsive grid of this many columns (default 1) */
  columns?: 1 | 2 | 3
}

export interface FlowStep {
  id: string
  title: string
  description?: string
  sections: FlowSection[]
}

export interface FlowConfig {
  id: string
  title: string
  subtitle?: string
  /** e.g. "$25.00", shown on the review step */
  fee?: string
  /** route to return to on cancel/save and after submit */
  returnTo: string
  currentNav?: string
  steps: FlowStep[]
}
