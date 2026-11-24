/** Hand-off from the Servings Calculator to the SEP application: the estimate is written to sessionStorage only when the user clicks Apply, then read once and cleared. */

export const SERVINGS_PREFILL_KEY = "sep:servings-prefill"

export interface ServingsPrefill {
  /** Estimated total standard drinks → SEP field `estimatedDrinks`. */
  estimatedDrinks: string
  /** Expected guests → SEP field `maxGuests`. */
  maxGuests: string
  /** Liquor categories with a non-zero share of the mix → SEP `liquorTypes`. */
  liquorTypes: string[]
}

/** Persist a calculator estimate for the SEP application to pick up. */
export function writeServingsPrefill(prefill: ServingsPrefill): void {
  if (typeof window === "undefined") return
  try {
    window.sessionStorage.setItem(SERVINGS_PREFILL_KEY, JSON.stringify(prefill))
  } catch {
    /* sessionStorage unavailable: the calculator still navigates, without prefill. */
  }
}

/** Read and remove any pending calculator estimate. Returns null when none. */
export function consumeServingsPrefill(): ServingsPrefill | null {
  if (typeof window === "undefined") return null
  try {
    const raw = window.sessionStorage.getItem(SERVINGS_PREFILL_KEY)
    if (!raw) return null
    window.sessionStorage.removeItem(SERVINGS_PREFILL_KEY)
    const parsed = JSON.parse(raw) as Partial<ServingsPrefill>
    return {
      estimatedDrinks: typeof parsed.estimatedDrinks === "string" ? parsed.estimatedDrinks : "",
      maxGuests: typeof parsed.maxGuests === "string" ? parsed.maxGuests : "",
      liquorTypes: Array.isArray(parsed.liquorTypes) ? parsed.liquorTypes.filter((v) => typeof v === "string") : [],
    }
  } catch {
    return null
  }
}
