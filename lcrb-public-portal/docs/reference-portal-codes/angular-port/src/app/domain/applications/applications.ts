import type { LicenceType } from "@domain/models/applications"
import { type LicenceTypeAccent } from "@domain/models/applications"

export const LICENCE_TYPE_ACCENTS: Record<LicenceType, LicenceTypeAccent> = {
  common: { bar: "border-l-slate-400", chip: "bg-slate-100 text-slate-700", dot: "bg-slate-400" },
  "food-primary": { bar: "border-l-blue-500", chip: "bg-blue-100 text-blue-700", dot: "bg-blue-500" },
  "liquor-primary": { bar: "border-l-sky-500", chip: "bg-sky-100 text-sky-700", dot: "bg-sky-500" },
  "cannabis-retail-store": {
    bar: "border-l-emerald-500",
    chip: "bg-emerald-100 text-emerald-700",
    dot: "bg-emerald-500",
  },
  "licensee-retail-store": {
    bar: "border-l-indigo-500",
    chip: "bg-indigo-100 text-indigo-700",
    dot: "bg-indigo-500",
  },
  manufacturer: { bar: "border-l-orange-500", chip: "bg-orange-100 text-orange-700", dot: "bg-orange-500" },
  "deemed-manufacturer": { bar: "border-l-amber-500", chip: "bg-amber-100 text-amber-700", dot: "bg-amber-500" },
  agent: { bar: "border-l-cyan-500", chip: "bg-cyan-100 text-cyan-700", dot: "bg-cyan-500" },
  catering: { bar: "border-l-rose-500", chip: "bg-rose-100 text-rose-700", dot: "bg-rose-500" },
  "ubrew-uvin": { bar: "border-l-lime-600", chip: "bg-lime-100 text-lime-700", dot: "bg-lime-600" },
}
