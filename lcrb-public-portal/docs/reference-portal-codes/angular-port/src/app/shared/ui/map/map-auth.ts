/** The map's own user, from the portal session or the in-map demo login. TODO: replace `demoUsers` and `login()` with the verified IAM session (`seedFromSession`). */
import { computed, inject, Injectable, signal } from "@angular/core"
import { injectSession } from "@core/session/session-provider"
import type { UserRole as PortalRole } from "@domain/roles"

type MapRole = "staff" | "inspector" | "admin" | "police" | "localgovernment"

interface MapUser {
  id: string
  name: string
  email: string
  role: MapRole
}

const demoUsers: Record<string, MapUser> = {
  "staff@gov.bc.ca": { id: "1", name: "Jane Doe", email: "staff@gov.bc.ca", role: "staff" },
  "inspector@gov.bc.ca": { id: "2", name: "John Smith", email: "inspector@gov.bc.ca", role: "inspector" },
  "admin@gov.bc.ca": { id: "3", name: "Admin User", email: "admin@gov.bc.ca", role: "admin" },
  "police@gov.bc.ca": { id: "4", name: "Constable Brown", email: "police@gov.bc.ca", role: "police" },
  "lg@gov.bc.ca": { id: "5", name: "Local Gov User", email: "lg@gov.bc.ca", role: "localgovernment" },
}

/** Map the portal session role onto a map user (or null for public view). */
function seedFromSession(
  session: { role: PortalRole; firstName: string; lastName: string; organization: string } | null,
): MapUser | null {
  if (!session) return null
  const name = `${session.firstName} ${session.lastName}`.trim() || session.organization
  if (session.role === "police") {
    return { id: "session", name: `Cst. ${name}`, email: "police@gov.bc.ca", role: "police" }
  }
  if (session.role === "local-government") {
    return { id: "session", name, email: "lg@gov.bc.ca", role: "localgovernment" }
  }
  if (session.role === "lcrb") {
    // LCRB licensing staff get the full staff map (inspection planner, insights, application pipeline, and all-status data incl. in-progress applications).
    return { id: "session", name, email: "staff@gov.bc.ca", role: "staff" }
  }
  // Licensee / SEP applicant see the public-facing map by default.
  return null
}

@Injectable()
export class AuthProvider {
  private readonly session = injectSession().session
  // `override` lets the in-map demo login switch roles; `undefined` follows the portal session, a value (incl. null) overrides it.
  private readonly override = signal<MapUser | null | undefined>(undefined)

  readonly user = computed(() => {
    const override = this.override()
    return override === undefined ? seedFromSession(this.session()) : override
  })
  readonly isAdmin = computed(() => !!this.user() && this.user()!.role === "admin")
  readonly isStaff = computed(() => {
    const user = this.user()
    return !!user && (user.role === "staff" || user.role === "inspector" || user.role === "admin")
  })
  readonly isPolice = computed(() => !!this.user() && this.user()!.role === "police")
  readonly isLocalGovernment = computed(() => !!this.user() && this.user()!.role === "localgovernment")
  readonly isAuthenticated = computed(() => !!this.user())
  readonly isLoggedIn = this.isAuthenticated
  readonly canViewSpecialEvents = computed(() => this.isStaff() || this.isPolice() || this.isLocalGovernment())

  readonly login = async (email: string, password: string): Promise<boolean> => {
    await new Promise((resolve) => setTimeout(resolve, 400))
    const demoUser = demoUsers[email.toLowerCase()]
    if (demoUser && password === "lcrb2024") {
      this.override.set(demoUser)
      return true
    }
    return false
  }

  readonly logout = (): void => {
    // Drop back to the public view for the map without touching the portal session.
    this.override.set(null)
  }
}

export function injectAuth(): AuthProvider {
  return inject(AuthProvider)
}
