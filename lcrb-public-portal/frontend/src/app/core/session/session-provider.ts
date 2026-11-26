// Mock session provider; replace the localStorage logic with the verified server session (IAM token claims).

import { afterNextRender, inject, Injectable, signal } from "@angular/core"
import type { UserRole } from "@domain/roles"

export interface Session {
  role: UserRole
  firstName: string
  lastName: string
  organization: string
  // Account email: demo identity in the mock, IAM/BCeID token claims in production.
  email?: string
  // True for the BCeID account owner (undefined counts as owner); enforce owner-only changes server-side.
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
    // Storage is read after the first render, so it starts as "loading".
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

/** The current session: `session` and `loading` are signals. */
export function injectSession(): SessionProvider {
  return inject(SessionProvider)
}
