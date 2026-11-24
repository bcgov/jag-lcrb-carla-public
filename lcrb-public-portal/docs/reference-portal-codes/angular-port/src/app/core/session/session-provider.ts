/** Mock session provider. TODO: replace with the real authenticated session (BCeID/IDIR via the IAM broker) and drop the localStorage logic; `Session` stays minimal so it maps from token claims. */

import { afterNextRender, inject, Injectable, signal } from "@angular/core"
import type { UserRole } from "@domain/roles"

export interface Session {
  role: UserRole
  firstName: string
  lastName: string
  organization: string
  /** Account email; seeded from the demo identity here, from verified IAM/BCeID claims in production. Used for scheduled report delivery. */
  email?: string
  /** True for the BCeID account owner, false for delegated users; gates owner-only settings, treated as owner when undefined. TODO: map from the IAM account-owner claim and enforce server-side. */
  isAccountOwner?: boolean
}

const STORAGE_KEY = "lcrb.mock.session"

@Injectable({ providedIn: "root" })
export class SessionProvider {
  private readonly sessionState = signal<Session | null>(null)
  private readonly loadingState = signal(true)

  readonly session = this.sessionState.asReadonly()
  readonly loading = this.loadingState.asReadonly()

  constructor() {
    // The prototype reads storage in an effect, so the first render is "loading".
    afterNextRender(() => {
      try {
        const raw = localStorage.getItem(STORAGE_KEY)
        if (raw) this.sessionState.set(JSON.parse(raw) as Session)
      } catch {
        // ignore malformed storage
      }
      this.loadingState.set(false)
    })
  }

  readonly login = (next: Session): void => {
    this.sessionState.set(next)
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(next))
    } catch {
      // ignore storage write failures
    }
  }

  readonly logout = (): void => {
    this.sessionState.set(null)
    try {
      localStorage.removeItem(STORAGE_KEY)
    } catch {
      // ignore
    }
  }
}

/** The prototype's useSession(): `session` and `loading` are signals. */
export function injectSession(): SessionProvider {
  return inject(SessionProvider)
}
