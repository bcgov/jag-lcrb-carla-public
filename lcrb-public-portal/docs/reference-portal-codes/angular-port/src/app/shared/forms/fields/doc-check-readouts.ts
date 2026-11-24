/** Feedback for the AI document completeness check: DocCheckResult types, DocCheckFeedback and per-document readouts (attribute hosts, so `space-y-*` parents behave as in the prototype). */
import { ChangeDetectionStrategy, Component, computed, input } from "@angular/core"
import { LucideIcon } from "@shared/ui/primitives/lucide-icon"
import { cn } from "@shared/util/utils"

export interface OccupancyResult {
  stampFound: boolean
  spaces: { label: string; load: number }[]
  totalLoad: number | null
  stampDate: string | null
  validUntil: string | null
  monthsOld: number | null
  stampExpired: boolean | null
  floorArea: { value: number; unit: "sqft" | "sqm" } | null
  scaleNoted: boolean
}

export interface RegistryDirector {
  name: string
  matched: boolean
  role?: string
}

export interface RegistryResult {
  status: "verified" | "partial" | "not-found" | "unavailable"
  simulated: boolean
  registryName?: string
  incorporationNumber?: string
  registrationState?: string
  directors: RegistryDirector[]
  unlistedOnDocument?: string[]
  summary: string
}

export interface FitAndProperResult {
  simulated: boolean
  people: { name: string; found: boolean; status: "cleared" | "in-progress" | "not-started" | "flagged"; note: string }[]
  outstanding: number
  flagged: number
  summary: string
}

export interface CorporateResult {
  found: boolean
  companyName: string | null
  incorporationNumber: string | null
  directors: string[]
  companyStatus?: string | null
  registry?: RegistryResult
  fitAndProper?: FitAndProperResult
}

export interface LeaseResult {
  premisesAddress: string | null
  landlord: string | null
  tenant: string | null
  termStart: string | null
  termEnd: string | null
  signed: boolean
  termExpired: boolean | null
  addressMatchesApplication: boolean | null
}

export interface LgApprovalResult {
  governmentBody: string | null
  establishmentName: string | null
  establishmentAddress: string | null
  decision: "support" | "oppose" | "no-comment" | "resolution-passed" | "unclear"
  decisionDate: string | null
  addressMatchesApplication: boolean | null
  policyGuidance?: { note: string; source?: string }
}

export interface CertificateResult {
  status: "valid" | "expired" | "not-found" | "unavailable"
  simulated: boolean
  program: "serving-it-right" | "selling-it-right" | "unknown"
  certificateNumber?: string
  holderName?: string
  expiryDate?: string
  monthsToExpiry?: number | null
  summary: string
}

export interface DocCheckResult {
  status: "complete" | "incomplete" | "unclear"
  summary: string
  documentType?: string
  present?: string[]
  missing?: string[]
  occupancy?: OccupancyResult
  corporate?: CorporateResult
  lease?: LeaseResult
  lgApproval?: LgApprovalResult
  certificate?: CertificateResult
  /** Unified plain-language cross-check findings across all document types. */
  advisories?: string[]
}

const READOUT_CLASS = "mt-1 rounded border border-border bg-background/60 px-2.5 py-2"

@Component({
  selector: "div[occupancyReadout]",
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: READOUT_CLASS },
  template: `
    @if (!occupancy().stampFound && occupancy().totalLoad == null) {
      <p class="font-medium">Occupancy load</p>
      <p class="leading-relaxed">No occupancy load stamp was detected on this plan. If your establishment has one, please ensure it is legible. This won't stop you from submitting.</p>
    } @else {
      <p class="font-medium">Occupancy load read from stamp{{ occupancy().totalLoad != null ? ": " + occupancy().totalLoad + " persons" : "" }}</p>
      @if (occupancy().spaces.length > 0) {
        <ul class="ml-4 list-disc leading-relaxed">
          @for (s of occupancy().spaces; track $index) {
            <li>{{ s.label }}: {{ s.load }}</li>
          }
        </ul>
      }
      @if (occupancy().totalLoad != null) {
        <p class="leading-relaxed">We've auto-filled the occupancy load field for you — please confirm it.</p>
      }
      @if (occupancy().stampExpired === true) {
        <p class="mt-1 font-medium text-[#8A6100]">This occupancy stamp is dated {{ occupancy().stampDate }} and is more than 12 months old, so it may be out of date. You can still submit, but LCRB may ask for a current stamp.</p>
      }
      @if (occupancy().stampExpired === false && occupancy().validUntil) {
        <p class="leading-relaxed">Stamp dated {{ occupancy().stampDate }} — considered current (valid ~12 months).</p>
      }
      @if (occupancy().floorArea; as area) {
        <p class="mt-1 leading-relaxed">Floor area read from the plan: {{ area.value }} {{ area.unit }}.</p>
      }
      @if (!occupancy().scaleNoted) {
        <p class="mt-1 leading-relaxed text-[#8A6100]">No scale or labelled dimensions were found on this plan — LCRB may not be able to verify measurements.</p>
      }
    }
  `,
})
export class OccupancyReadout {
  readonly occupancy = input.required<OccupancyResult>()
}

@Component({
  selector: "div[corporateReadout]",
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: READOUT_CLASS },
  template: `
    <p class="font-medium">Director information</p>
    @if (corporate().companyName) {
      <p class="leading-relaxed">{{ corporate().companyName }}{{ corporate().incorporationNumber ? " (" + corporate().incorporationNumber + ")" : "" }}</p>
    }
    @if (corporate().companyStatus) {
      <p class="leading-relaxed">Registration status on document: {{ corporate().companyStatus }}.</p>
    }
    @if (corporate().directors.length > 0) {
      <p class="leading-relaxed">Directors: {{ corporate().directors.join(", ") }}</p>
    }
    @if (corporate().registry; as r) {
      <p class="mt-1 leading-relaxed">{{ r.simulated ? "Simulated BC Registries check — " : "BC Registries — " }}{{ r.summary }}</p>
    }
    @if (corporate().fitAndProper; as fp) {
      <div class="mt-1">
        <p class="leading-relaxed">{{ fp.simulated ? "Simulated fit-and-proper / screening check — " : "Fit-and-proper / screening — " }}{{ fp.summary }}</p>
        @if (uncleared().length > 0) {
          <ul class="ml-4 list-disc leading-relaxed">
            @for (p of uncleared(); track $index) {
              <li><span class="font-medium">{{ p.name }}</span>{{ p.status === "flagged" ? " (needs review)" : p.status === "not-started" ? " (no screening on file)" : " (screening in progress)" }}: {{ p.note }}</li>
            }
          </ul>
        }
      </div>
    }
    <p class="mt-1 text-xs opacity-80">These registry &amp; screening checks are advisory and do not stop your submission.</p>
  `,
})
export class CorporateReadout {
  readonly corporate = input.required<CorporateResult>()
  protected readonly uncleared = computed(() =>
    (this.corporate().fitAndProper?.people ?? []).filter((p) => p.status !== "cleared"),
  )
}

@Component({
  selector: "div[leaseReadout]",
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: READOUT_CLASS },
  template: `
    <p class="font-medium">Lease / tenure</p>
    @if (lease().premisesAddress) {
      <p class="leading-relaxed">Premises: {{ lease().premisesAddress }}</p>
    }
    @if (lease().landlord || lease().tenant) {
      <p class="leading-relaxed">{{ lease().landlord ? "Landlord: " + lease().landlord : "" }}{{ lease().landlord && lease().tenant ? " · " : "" }}{{ lease().tenant ? "Tenant: " + lease().tenant : "" }}</p>
    }
    @if (lease().termStart || lease().termEnd) {
      <p class="leading-relaxed">Term: {{ lease().termStart ?? "?" }} to {{ lease().termEnd ?? "?" }}</p>
    }
    @if (lease().addressMatchesApplication === true) {
      <p class="leading-relaxed">The lease address matches your application address.</p>
    }
    @if (lease().addressMatchesApplication === false) {
      <p class="mt-1 leading-relaxed text-[#8A6100]">The lease premises address does not appear to match your application address — please confirm they are the same location.</p>
    }
    @if (lease().termExpired === true) {
      <p class="mt-1 leading-relaxed text-[#8A6100]">This lease term appears to have ended ({{ lease().termEnd }}). LCRB may ask for proof of current tenure.</p>
    }
    @if (!lease().signed) {
      <p class="mt-1 leading-relaxed">The lease does not appear to be signed by both parties.</p>
    }
    <p class="mt-1 text-xs opacity-80">This check is advisory and does not stop your submission.</p>
  `,
})
export class LeaseReadout {
  readonly lease = input.required<LeaseResult>()
}

const DECISION_TEXT: Record<LgApprovalResult["decision"], string> = {
  support: "supports the application",
  oppose: "opposes the application",
  "no-comment": "provided no comment",
  "resolution-passed": "passed a resolution",
  unclear: "recorded an unclear position",
}

@Component({
  selector: "div[lgApprovalReadout]",
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: READOUT_CLASS },
  template: `
    <p class="font-medium">Government approval</p>
    <p class="leading-relaxed">{{ approval().governmentBody ?? "The government body" }} {{ decisionText() }}{{ approval().decisionDate ? " (dated " + approval().decisionDate + ")" : "" }}.</p>
    @if (approval().establishmentName) {
      <p class="leading-relaxed">Establishment: {{ approval().establishmentName }}</p>
    }
    @if (approval().decision === "oppose") {
      <p class="mt-1 leading-relaxed text-[#8A6100]">This government opposes the application. You can still submit — the general manager weighs government input in the decision.</p>
    }
    @if (approval().addressMatchesApplication === false) {
      <p class="mt-1 leading-relaxed text-[#8A6100]">The address in the approval does not appear to match your application address — please confirm.</p>
    }
    @if (approval().policyGuidance; as g) {
      <p class="mt-1 text-xs opacity-80">{{ g.note }}{{ g.source ? " (" + g.source + ")" : "" }}</p>
    }
  `,
})
export class LgApprovalReadout {
  readonly approval = input.required<LgApprovalResult>()
  protected readonly decisionText = computed(() => DECISION_TEXT[this.approval().decision])
}

@Component({
  selector: "div[certificateReadout]",
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: READOUT_CLASS },
  template: `
    <p class="font-medium">{{ programLabel() }} certificate</p>
    @if (certificate().holderName) {
      <p class="leading-relaxed">Holder: {{ certificate().holderName }}</p>
    }
    @if (certificate().certificateNumber) {
      <p class="leading-relaxed">Certificate no.: {{ certificate().certificateNumber }}</p>
    }
    <p class="mt-1 leading-relaxed">{{ certificate().simulated ? "Simulated registry check — " : "Registry check — " }}{{ certificate().summary }}</p>
    @if (certificate().status === "expired") {
      <p class="mt-1 leading-relaxed text-[#8A6100]">This certificate has expired. Please provide a current certificate — the holder can renew online.</p>
    }
    <p class="mt-1 text-xs opacity-80">This check is advisory and does not stop your submission.</p>
  `,
})
export class CertificateReadout {
  readonly certificate = input.required<CertificateResult>()
  protected readonly programLabel = computed(() => {
    const program = this.certificate().program
    return program === "serving-it-right"
      ? "Serving It Right"
      : program === "selling-it-right"
        ? "Selling It Right"
        : "Responsible service"
  })
}

const FEEDBACK_CONFIG = {
  complete: {
    icon: "CheckCircle2",
    wrap: "border-[#2E8540]/40 bg-[#2E8540]/10 text-[#1E5631]",
    title: "Looks complete",
  },
  incomplete: {
    icon: "AlertTriangle",
    wrap: "border-[#F2A900]/50 bg-[#F2A900]/10 text-[#8A6100]",
    title: "May be missing something",
  },
  unclear: {
    icon: "HelpCircle",
    wrap: "border-border bg-muted/50 text-foreground",
    title: "Couldn't fully read this file",
  },
} as const

@Component({
  selector: "div[docCheckFeedback]",
  imports: [LucideIcon, OccupancyReadout, CorporateReadout, LeaseReadout, LgApprovalReadout, CertificateReadout],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { "[attr.class]": "wrapClass()", role: "status", "aria-live": "polite" },
  template: `
    <div class="flex items-start gap-2">
      <svg [lucide]="config().icon" class="mt-0.5 h-4 w-4 shrink-0"></svg>
      <div class="space-y-1.5">
        <p class="font-semibold leading-snug">{{ config().title }}</p>
        <p class="leading-relaxed">{{ result().summary }}</p>
        @if (result().missing; as missing) {
          @if (missing.length > 0) {
            <div>
              <p class="font-medium">Please check that your document includes:</p>
              <ul class="ml-4 list-disc leading-relaxed">
                @for (m of missing; track $index) {
                  <li>{{ m }}</li>
                }
              </ul>
            </div>
          }
        }
        @if (result().occupancy; as occupancy) {
          <div occupancyReadout [occupancy]="occupancy"></div>
        }
        @if (result().corporate; as corporate) {
          <div corporateReadout [corporate]="corporate"></div>
        }
        @if (result().lease; as lease) {
          <div leaseReadout [lease]="lease"></div>
        }
        @if (result().lgApproval; as approval) {
          <div lgApprovalReadout [approval]="approval"></div>
        }
        @if (result().certificate; as certificate) {
          <div certificateReadout [certificate]="certificate"></div>
        }
      </div>
    </div>
  `,
})
export class DocCheckFeedback {
  readonly result = input.required<DocCheckResult>()
  protected readonly config = computed(() => FEEDBACK_CONFIG[this.result().status])
  protected readonly wrapClass = computed(() => cn("mt-2 rounded-md border px-3 py-2.5 text-sm", this.config().wrap))
}
