/** Config-driven application flow: renders a `FlowConfig` as steps, fields, navigation and a review step. Flow types live in `domain/forms/flow-config.ts`. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from "@angular/core"
import { Router } from "@angular/router"
import { BCGovHeader } from "@core/layout/header"
import { BCGovFooter } from "@core/layout/footer"
import { injectSession } from "@core/session/session-provider"
import { injectApplicationMode } from "./application-mode"
import { actorName } from "@domain/roles"
import { gatherLinkContext } from "@core/navigation/link-context"
import { ACCOUNT_REPOSITORY } from "@data-access/repositories/account/account.repository"
import { LICENCES_REPOSITORY } from "@data-access/repositories/licences/licences.repository"
import { ApplicationModeToggle } from "./mode-toggle"
import { ModeExplainer } from "./mode-explainer"
import { FocusFlow } from "./focus-flow"
import {
  ApplicationProvider,
  ApplicationLayout,
  ApplicationHeaderAction,
  ApplicationSection,
  ApplicationNavigation,
  InfoBox,
  injectApplicationContext,
  type ApplicationStep,
} from "./index"
import { ConfigField, CheckboxFieldF, isFieldVisible } from "./fields"
import { AssistantFormBridge, type BridgeFieldMeta } from "./assistant-bridge"

import type { FlowConfig, FlowSection } from "@domain/forms/flow-config"
import { APPLICATIONS_REPOSITORY } from "@data-access/repositories/applications/applications.repository"
import { AI_REPOSITORY } from "@data-access/repositories/ai/ai.repository"
import { path } from "@/app/route-registry"

/** Flatten a flow config into the per-field metadata the AI assistant reads, explains, highlights and fills. */
function buildFieldMeta(config: FlowConfig): BridgeFieldMeta[] {
  const meta: BridgeFieldMeta[] = []
  config.steps.forEach((step, stepIndex) => {
    step.sections.forEach((section) => {
      section.fields.forEach((f) => {
        meta.push({
          name: f.name,
          label: f.label,
          type: f.type,
          required: f.required,
          help: f.help,
          options: f.options,
          step: stepIndex,
        })
      })
    })
  })
  const reviewStep = config.steps.length
  meta.push(
    {
      name: "__confirmAccuracy",
      label: "I confirm that all information provided is accurate and complete.",
      type: "checkbox",
      required: true,
      step: reviewStep,
    },
    {
      name: "__acknowledgeFalseInfo",
      label: "I understand that providing false information may result in refusal or revocation.",
      type: "checkbox",
      required: true,
      step: reviewStep,
    },
  )
  // Expose the running document-review note as read-only context for the chatbot; no step renders it.
  meta.push({
    name: "__documentReview",
    label: "AI document review findings",
    type: "text",
    help: "Automated findings from uploaded documents: floor-plan occupancy stamp reading/validity/scale, BC Registries director confirmation and fit-and-proper screening, lease address/term checks, local-government approval decisions, responsible-service certificate validity, and completeness. Advisory only — never blocks submission.",
    step: reviewStep,
  })
  return meta
}

function colClass(columns?: 1 | 2 | 3) {
  if (columns === 3) return "grid grid-cols-1 md:grid-cols-3 gap-4"
  if (columns === 2) return "grid grid-cols-1 md:grid-cols-2 gap-4"
  return "space-y-6"
}

export function buildSummaryRows(config: FlowConfig, formData: Record<string, unknown>) {
  const rows: { label: string; value: string }[] = []
  for (const step of config.steps) {
    for (const section of step.sections) {
      for (const field of section.fields) {
        if (!isFieldVisible(field, formData)) continue
        const raw = formData[field.name]
        if (raw === undefined || raw === "" || raw === false) continue
        // Choice fields store machine values ("current", "yes"), so show the option label the user clicked.
        const labelFor = (v: unknown) => field.options?.find((o) => o.value === String(v))?.label ?? String(v)
        let value: string
        if (Array.isArray(raw)) value = raw.map(labelFor).join(", ")
        else if (raw === true) value = "Yes"
        else value = labelFor(raw)
        rows.push({ label: field.label, value })
      }
    }
  }
  return rows
}

/** Collect the `__doccheck:<field>` results recorded by `FileFieldF` into one status + summary line each for the staff AI assessment report. */
function gatherDocumentChecks(
  formData: Record<string, unknown>,
): { label: string; status: string; summary: string }[] {
  const out: { label: string; status: string; summary: string }[] = []
  for (const [key, raw] of Object.entries(formData)) {
    if (!key.startsWith("__doccheck:") || typeof raw !== "string") continue
    try {
      const rec = JSON.parse(raw) as {
        label: string
        status: string
        summary: string
        /** Server-produced cross-check findings for every document type; the single source for the report. */
        advisories?: string[]
      }
      const extras = Array.isArray(rec.advisories) ? rec.advisories.filter(Boolean) : []
      out.push({
        label: rec.label,
        status: rec.status,
        summary: [rec.summary, ...extras].filter(Boolean).join(" "),
      })
    } catch {
      /* ignore malformed entries */
    }
  }
  return out
}

function parseFee(fee?: string): number {
  if (!fee) return 0
  const n = Number.parseFloat(fee.replace(/[^0-9.]/g, ""))
  return Number.isFinite(n) ? n : 0
}

function deriveApplicant(formData: Record<string, unknown>): string {
  for (const key of ["establishmentName", "legalEntityName", "legalName", "businessName", "applicantName"]) {
    const v = formData[key]
    if (typeof v === "string" && v.trim()) return v.trim()
  }
  return "Applicant"
}

/** A section card; its root is the card itself because the parent's `space-y-6` styles it. */
@Component({
  selector: "div[sectionBlock]",
  imports: [InfoBox, ConfigField],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: "bg-white rounded-lg border" },
  template: `
    <div class="bg-[#38598A] text-white px-6 py-3 rounded-t-lg">
      <h2 class="font-semibold">{{ section().title }}</h2>
      @if (section().description) {
        <p class="text-white/80 text-sm mt-1">{{ section().description }}</p>
      }
    </div>
    <div class="p-6">
      <div class="space-y-6">
        @if (section().info; as info) {
          <div infoBox [variant]="info.variant" [title]="info.title">{{ info.body }}</div>
        }
        <div [attr.class]="colClass()">
          @for (f of section().fields; track f.name) {
            <div configField [field]="f"></div>
          }
        </div>
      </div>
    </div>
  `,
})
export class SectionBlock {
  readonly section = input.required<FlowSection>()
  protected readonly colClass = computed(() => colClass(this.section().columns))
}

@Component({
  selector: "review-step",
  imports: [ApplicationSection, InfoBox, CheckboxFieldF],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <div class="space-y-6">
      <div infoBox variant="info" [title]="'Review your application'">
        Please confirm the details below are correct before submitting. You can go back to any step to make changes.
      </div>

      <div applicationSection [title]="'Summary'" [description]="'Information you have entered'">
        @if (rows().length === 0) {
          <p class="text-sm text-muted-foreground">No information entered yet. Go back to complete the form.</p>
        } @else {
          <dl class="divide-y">
            @for (r of rows(); track $index) {
              <div class="grid grid-cols-1 sm:grid-cols-3 gap-1 py-2 text-sm">
                <dt class="text-muted-foreground sm:col-span-1">{{ r.label }}</dt>
                <dd class="font-medium sm:col-span-2">{{ r.value }}</dd>
              </div>
            }
          </dl>
        }
      </div>

      @if (config().fee) {
        <div applicationSection [title]="'Fees'">
          <dl class="space-y-2 text-sm">
            <div class="flex justify-between">
              <dt class="text-muted-foreground">Application Fee</dt>
              <dd class="font-medium">{{ config().fee }}</dd>
            </div>
            <div class="flex justify-between border-t pt-2">
              <dt class="font-semibold">Total</dt>
              <dd class="font-semibold">{{ config().fee }}</dd>
            </div>
          </dl>
        </div>
      }

      <div applicationSection [title]="'Declaration'">
        <div class="space-y-4">
          <div
            checkboxFieldF
            [name]="'__confirmAccuracy'"
            [label]="'I confirm that all information provided is accurate and complete.'"
            [required]="true"
          ></div>
          <div
            checkboxFieldF
            [name]="'__acknowledgeFalseInfo'"
            [label]="'I understand that providing false information may result in refusal or revocation.'"
            [required]="true"
          ></div>
        </div>
      </div>
    </div>
  `,
})
export class ReviewStep {
  readonly config = input.required<FlowConfig>()
  private readonly ctx = injectApplicationContext()
  protected readonly rows = computed(() => buildSummaryRows(this.config(), this.ctx.formData()))
}

@Component({
  selector: "config-flow-inner",
  imports: [
    ApplicationLayout,
    ApplicationHeaderAction,
    ApplicationNavigation,
    ApplicationModeToggle,
    ModeExplainer,
    FocusFlow,
    ReviewStep,
    SectionBlock,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <!-- Render the classic form until the stored preference is read, so first paint matches before hydration. -->
    @if (hydrated() && mode() === "guided") {
      <!-- The review screen is the same component as the classic form, so both modes submit the same thing. -->
      <ng-template #reviewSlot><review-step [config]="config()" /></ng-template>
      <focus-flow
        [config]="config()"
        (exitMode)="setMode('form')"
        (submitFlow)="handleGuidedSubmit()"
        [submitting]="submitting()"
        [reviewSlot]="reviewSlot"
      />
    } @else {
      <application-layout [title]="config().title" [applicationName]="config().subtitle">
        @if (hydrated()) {
          <application-mode-toggle appHeaderAction [mode]="mode()" (modeChange)="setMode($event)" />
        }
        <!-- First-visit explainer, only on the opening step. -->
        @if (hydrated() && ctx.currentStep() === 0 && !isReview()) {
          <div class="mb-6">
            <mode-explainer [mode]="mode()" (modeChange)="setMode($event)" />
          </div>
        }
        @if (isReview()) {
          <review-step [config]="config()" />
        } @else {
          <div class="space-y-6">
            @if (step()?.description) {
              <p class="text-muted-foreground">{{ step()!.description }}</p>
            }
            @for (s of step()!.sections; track $index) {
              <div sectionBlock [section]="s"></div>
            }
          </div>
        }
        <application-navigation [onSave]="goToReturnTo" [onSubmit]="handleSubmit" />
      </application-layout>
    }
  `,
})
export class ConfigFlowInner {
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY)
  private readonly aiRepository = inject(AI_REPOSITORY)
  private readonly account = inject(ACCOUNT_REPOSITORY)
  private readonly licencesRepository = inject(LICENCES_REPOSITORY)
  readonly config = input.required<FlowConfig>()

  private readonly router = inject(Router)
  private readonly sessionProvider = injectSession()
  protected readonly ctx = injectApplicationContext()
  private readonly applicationMode = injectApplicationMode()
  protected readonly mode = this.applicationMode.mode
  protected readonly setMode = this.applicationMode.setMode
  protected readonly hydrated = this.applicationMode.hydrated
  // Guided Mode has its own submit button and needs a pending state; the classic navigation manages it internally.
  protected readonly submitting = signal(false)

  protected readonly isReview = computed(() => this.ctx.currentStep() === this.config().steps.length)
  protected readonly step = computed(() => this.config().steps[this.ctx.currentStep()])

  protected readonly goToReturnTo = (): void => {
    void this.router.navigateByUrl(this.config().returnTo)
  }

  protected readonly handleSubmit = async (): Promise<void> => {
    const config = this.config()
    const formData = this.ctx.formData()
    const session = this.sessionProvider.session()
    const summary = buildSummaryRows(config, formData)
    const applicant = deriveApplicant(formData)
    const fee = parseFee(config.fee)

    // Gather the per-document AI checks recorded during the flow for the staff assessment report; advisory only, never blocks.
    const documentChecks = gatherDocumentChecks(formData)

    // AI pre-screening: deterministic licence-cap and Section 119 engines plus an AI narrative, yielding the assessment and an agent log; a failure still yields a deterministic log.
    const profile = this.account.businessProfile(session?.organization)
    const selfDeclared =
      formData["indigenousNation"] === true ||
      formData["indigenousNationDeclaration"] === true ||
      formData["firstNationAffiliated"] === true
    // Include the shared Legal Entity build and Tied House register so the cap engine groups the whole "one and the same" business.
    const link = gatherLinkContext(profile.bceidBusinessNumber)
    const { agentLog, assessment } = await this.aiRepository.preScreen({
      flowId: config.id,
      title: config.title,
      applicant,
      summary,
      documentChecks,
      applicantName: profile.legalName,
      businessNumber: profile.bceidBusinessNumber,
      ownedLicences: this.licencesRepository.licences(),
      leBuild: link.leBuild,
      existingRelationships: link.existingRelationships,
      declarations: link.declarations,
      firstNation: profile.firstNation,
      selfDeclaredIndigenousNation: selfDeclared,
    })

    // Payment, Dynamics, audit and notification orchestration is shared.
    const record = this.applicationsRepository.submit({
      flowId: config.id,
      title: config.title,
      applicant,
      summary,
      fee,
      actor: actorName(session),
      role: session?.role,
      email: session?.email,
      assessment,
      agentLog,
    })

    this.applicationsRepository.deleteDraft(`draft:${config.id}`)
    void this.router.navigateByUrl(path('applications.confirmation', { id: record.id }))
  }

  /** Wraps handleSubmit with a pending flag for Guided Mode's submit button. */
  protected async handleGuidedSubmit(): Promise<void> {
    if (this.submitting()) return
    this.submitting.set(true)
    try {
      await this.handleSubmit()
    } finally {
      // The success path navigates away, so this only matters on failure.
      this.submitting.set(false)
    }
  }
}

@Component({
  selector: "config-flow",
  imports: [BCGovHeader, BCGovFooter, ApplicationProvider, AssistantFormBridge, ConfigFlowInner],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <div class="min-h-screen flex flex-col">
      <bc-gov-header [currentPage]="config().currentNav ?? path('applications.list')" />
      <main id="main-content" class="flex-1">
        <application-provider
          [steps]="steps()"
          [initialData]="prefill()"
          [draftId]="'draft:' + config().id"
          [draftMeta]="draftMeta()"
        >
          <assistant-form-bridge [flowTitle]="config().title" [fieldMeta]="fieldMeta()" />
          <config-flow-inner [config]="config()" />
        </application-provider>
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class ConfigFlow {
  protected readonly path = path
  readonly config = input.required<FlowConfig>()
  /** Values to pre-populate the form with, e.g. licence number and business name when started from a licence. */
  readonly prefill = input<Record<string, string> | undefined>(undefined)

  // A synthetic final "Review" step is appended to the defined steps.
  protected readonly steps = computed<ApplicationStep[]>(() => [
    ...this.config().steps.map((s) => ({ id: s.id, title: s.title, description: s.description })),
    { id: "review", title: "Review & Submit", description: "Confirm and submit" },
  ])
  protected readonly draftMeta = computed(() => ({
    flowId: this.config().id,
    title: this.config().title,
    resumeHref: path('applications.flow', { slug: this.config().id }),
  }))
  protected readonly fieldMeta = computed(() => buildFieldMeta(this.config()))
}
