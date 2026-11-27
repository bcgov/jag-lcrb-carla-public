import type { UserRole } from "@domain/roles"

export type SearchKind = "licence" | "application" | "sep" | "lg-referral" | "catalog"

export interface SearchResult {
  id: string
  kind: SearchKind
  title: string
  subtitle: string
  badge?: string
  href: string
  /** Concatenated searchable text (lowercased). */
  haystack: string
}

export interface SearchOptions {
  role?: UserRole
  limit?: number
}
