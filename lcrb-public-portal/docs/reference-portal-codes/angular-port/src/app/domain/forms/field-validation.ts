/** Framework-free validation rule types for the application framework. */

export type FieldValue = unknown

export interface FieldValidation {
  /** Pass `true` for the default message or a string for a custom one. */
  required?: boolean | string
  minLength?: number
  maxLength?: number
  /** Numeric bounds (value is coerced with Number()). */
  min?: number
  max?: number
  /** Built-in formats. */
  email?: boolean
  postalCode?: boolean
  phone?: boolean
  /** Date must be today or later. */
  notPast?: boolean
  /** Date must be at least N days from today. */
  minDaysFromNow?: number
  /** Soft counterpart to `minDaysFromNow`: dates in the window are accepted but show the message as a warning. Evaluated by `warnValue()` only, so it never blocks progression. */
  warnIfWithinDays?: { days: number; message: string }
  /** Date must be on or after another field's date; declarative so configs stay serializable. */
  afterField?: { field: string; message?: string }
  /** Arbitrary pattern. */
  pattern?: { re: RegExp; message: string }
  /** Custom validator; return an error message or undefined when valid. */
  validate?: (value: FieldValue, form: Record<string, unknown>) => string | undefined
}
