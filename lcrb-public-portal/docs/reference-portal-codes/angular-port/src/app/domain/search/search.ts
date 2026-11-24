import { type SearchKind } from "@domain/models/search"

const KIND_LABELS: Record<SearchKind, string> = {
  licence: "Licence",
  application: "Application",
  sep: "Special Event Permit",
  "lg-referral": "Government Referral",
  catalog: "Apply / Start",
}

export function kindLabel(kind: SearchKind): string {
  return KIND_LABELS[kind]
}
