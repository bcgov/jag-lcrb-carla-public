/** Turns a `FlowConfig` into the flat one-question-at-a-time list Guided Mode walks; type-only FlowConfig import avoids a runtime cycle. */

// Imported from ./field-schema rather than ./fields so the queue stays usable outside client components and in tests.
import { isFieldVisible, type FieldConfig } from "@domain/forms/field-schema"
import type { FlowConfig } from "@domain/forms/flow-config"
import type { FieldValidation } from "@domain/forms/field-validation"

export interface Question {
  field: FieldConfig
  /** Index into `config.steps` — used to keep the provider's currentStep in sync. */
  stepIndex: number
  stepTitle: string
  sectionTitle?: string
}

/** Every currently visible field in flow order; recompute when formData changes, since gating answers add or remove questions. */
export function buildQuestionQueue(config: FlowConfig, formData: Record<string, unknown>): Question[] {
  const queue: Question[] = []
  config.steps.forEach((step, stepIndex) => {
    for (const section of step.sections) {
      for (const field of section.fields) {
        if (!isFieldVisible(field, formData)) continue
        queue.push({ field, stepIndex, stepTitle: step.title, sectionTitle: section.title })
      }
    }
  })
  return queue
}

/** Locate the cursor by field name, since an index would jump when the queue changes shape; -1 when the field is no longer visible. */
export function indexOfQuestion(queue: Question[], fieldName: string | null): number {
  if (!fieldName) return -1
  return queue.findIndex((q) => q.field.name === fieldName)
}

/** First question of `stepIndex`, or of the next populated step; -1 when none is visible (go to review). */
export function firstQuestionOfStep(queue: Question[], stepIndex: number): number {
  const exact = queue.findIndex((q) => q.stepIndex === stepIndex)
  if (exact !== -1) return exact
  // The step has no visible fields, so skip it.
  return queue.findIndex((q) => q.stepIndex > stepIndex)
}

/** The validation spec a field enforces, composing `required` like `useField`; needed because Guided Mode validates fields that are not mounted. */
export function effectiveRules(field: FieldConfig): FieldValidation | undefined {
  // CheckboxFieldF hard-codes its own required message; match it so both modes show the same words.
  if (field.type === "checkbox") {
    return field.required ? { required: "You must check this box to continue." } : undefined
  }
  const rules = field.validation
  if (!rules && !field.required) return undefined
  return { ...(rules ?? {}), required: field.required ?? rules?.required }
}
