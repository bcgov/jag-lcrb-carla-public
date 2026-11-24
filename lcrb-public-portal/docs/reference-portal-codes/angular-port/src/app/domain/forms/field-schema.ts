/** Field schema and visibility rules: pure types and `isFieldVisible`, with no UI dependency. */

import type { FieldValidation } from "./field-validation"

export interface Option {
  value: string
  label: string
}

/** The standard yes/no option pair, plain data that flow configs need. */
export const YES_NO: Option[] = [
  { value: "yes", label: "Yes" },
  { value: "no", label: "No" },
]

export type FieldType =
  | "text"
  | "email"
  | "tel"
  | "number"
  | "date"
  | "time"
  | "textarea"
  | "select"
  | "radio"
  | "checkbox"
  | "checkbox-group"
  | "file"

export interface FieldConfig {
  name: string
  label: string
  type: FieldType
  required?: boolean
  placeholder?: string
  hint?: string
  options?: Option[]
  /** Declarative validation rules (see lib/validation). */
  validation?: FieldValidation
  /** Field is shown and validated only when this predicate passes; use `visibleWhenEquals` in serializable catalog data. */
  visibleWhen?: (form: Record<string, unknown>) => boolean
  /** Serializable form of `visibleWhen`: show only while `field` holds `value` (or any of `value` when an array). */
  visibleWhenEquals?: { field: string; value: string | string[] }
  /** Plain-language explanation surfaced by the AI assistant. */
  help?: string
  /** Text the applicant must read before answering, shown above the control in Guided Mode; one paragraph per entry. */
  readBefore?: {
    title: string
    paragraphs: { heading?: string; text: string }[]
  }
  /** For `type: "file"` fields: checkable requirements the upload must satisfy; the AI completeness check reads these (falling back to `hint`). */
  criteria?: string[]
  /** For `type: "file"` fields: document-intelligence extraction hint. */
  docType?: "floor-plan" | "director-info" | "incorporation" | "lease" | "lg-approval" | "certificate" | "generic"
  /** For floor-plan file fields: the field to auto-fill with the occupancy total. */
  autoPopulateField?: string
}

/** Whether a field renders and is validated, honouring both `visibleWhen` and `visibleWhenEquals`, so rendering and validation agree. */
export function isFieldVisible(field: FieldConfig, formData: Record<string, unknown>): boolean {
  if (field.visibleWhen && !field.visibleWhen(formData)) return false
  const rule = field.visibleWhenEquals
  if (rule) {
    const actual = formData[rule.field]
    const allowed = Array.isArray(rule.value) ? rule.value : [rule.value]
    if (!allowed.includes(String(actual))) return false
  }
  return true
}

/** Whether Guided Mode's "describe it in your own words" box is worth offering for this field type; useless for postal codes, dates, numbers and yes/no. */
export function proseHelpsForType(field: FieldConfig): boolean {
  switch (field.type) {
    case "textarea":
    case "select":
    case "checkbox-group":
      return true
    case "radio":
      // Yes/No (or any two-way) radios are one click; longer lists benefit.
      return (field.options?.length ?? 0) > 2
    default:
      // text, email, tel, number, date, time, checkbox, file
      return false
  }
}
