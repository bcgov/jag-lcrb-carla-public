/** Client-side field validation; production mirrors these rules in the server-side App Validation service, so never trust them alone. */

import type { FieldValidation, FieldValue } from "@domain/forms/field-validation"

const BC_POSTAL = /^[A-Za-z]\d[A-Za-z][ ]?\d[A-Za-z]\d$/
const EMAIL = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
const PHONE = /^[\d\s\-+()]{10,}$/

function isEmpty(value: FieldValue): boolean {
  if (value === undefined || value === null) return true
  if (typeof value === "string") return value.trim() === ""
  if (Array.isArray(value)) return value.length === 0
  if (typeof value === "boolean") return value === false
  return false
}

function startOfToday(): Date {
  const d = new Date()
  d.setHours(0, 0, 0, 0)
  return d
}

/** A non-blocking warning for the field, or undefined; separate from `validateValue` so a warning is never mistaken for an error. */
export function warnValue(value: FieldValue, rules: FieldValidation | undefined): string | undefined {
  if (!rules?.warnIfWithinDays) return undefined
  const str = typeof value === "string" ? value : ""
  if (!str) return undefined
  const date = new Date(str)
  if (Number.isNaN(date.getTime())) return undefined
  const min = startOfToday()
  min.setDate(min.getDate() + rules.warnIfWithinDays.days)
  return date < min ? rules.warnIfWithinDays.message : undefined
}

/** Returns an error message for the field, or undefined if valid. */
export function validateValue(
  value: FieldValue,
  rules: FieldValidation | undefined,
  form: Record<string, unknown>,
): string | undefined {
  if (!rules) return undefined

  // Required is the only rule that fires on empty values.
  if (rules.required && isEmpty(value)) {
    return typeof rules.required === "string" ? rules.required : "This field is required."
  }

  // All other rules are skipped for empty optional fields.
  if (isEmpty(value)) return undefined

  const str = typeof value === "string" ? value : String(value)

  if (rules.minLength !== undefined && str.length < rules.minLength) {
    return `Must be at least ${rules.minLength} characters.`
  }
  if (rules.maxLength !== undefined && str.length > rules.maxLength) {
    return `Must be ${rules.maxLength} characters or fewer.`
  }
  if (rules.email && !EMAIL.test(str)) {
    return "Enter a valid email address."
  }
  if (rules.postalCode && !BC_POSTAL.test(str)) {
    return "Enter a valid postal code (e.g. V8W 9V1)."
  }
  if (rules.phone && !PHONE.test(str)) {
    return "Enter a valid phone number (at least 10 digits)."
  }
  if (rules.min !== undefined && Number(value) < rules.min) {
    return `Must be ${rules.min} or greater.`
  }
  if (rules.max !== undefined && Number(value) > rules.max) {
    return `Must be ${rules.max} or less.`
  }
  if ((rules.notPast || rules.minDaysFromNow !== undefined) && str) {
    const date = new Date(str)
    if (!Number.isNaN(date.getTime())) {
      const today = startOfToday()
      if (rules.notPast && date < today) {
        return "Date cannot be in the past."
      }
      if (rules.minDaysFromNow !== undefined) {
        const min = new Date(today)
        min.setDate(min.getDate() + rules.minDaysFromNow)
        if (date < min) {
          return `Date must be at least ${rules.minDaysFromNow} days from today.`
        }
      }
    }
  }
  if (rules.afterField) {
    const other = form[rules.afterField.field]
    if (other !== undefined && other !== null && String(other) !== "") {
      if (str < String(other)) {
        return rules.afterField.message ?? "Must be on or after the start date."
      }
    }
  }
  if (rules.pattern && !rules.pattern.re.test(str)) {
    return rules.pattern.message
  }
  if (rules.validate) {
    return rules.validate(value, form)
  }
  return undefined
}
