/** Application framework state: an injectable `ApplicationFlowState` provided by `<application-provider>`; React state becomes signals and callbacks are arrow members. */
import {
  afterNextRender,
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  Injectable,
  input,
  type OnInit,
  signal,
  untracked,
  type Signal,
} from "@angular/core"
import { toast } from "@spartan-ng/brain/sonner"
import { ALERT_DIALOG } from "@shared/ui/primitives/alert-dialog"
import { pulseProgress } from "@core/progress/progress-bus"
import { validateValue } from "./validation"
import { type FieldValidation } from "@domain/forms/field-validation"
import { type ApplicationDraft } from "@data-access/repositories/applications/drafts"
import { APPLICATIONS_REPOSITORY } from "@data-access/repositories/applications/applications.repository"

export interface ApplicationStep {
  id: string
  title: string
  description?: string
  isOptional?: boolean
  /** Optional completeness check for flows whose fields don't register validators; decides the green check and whether the user may advance. */
  validate?: (formData: ApplicationFormData) => boolean
}

export interface ApplicationFormData {
  [key: string]: unknown
}

/** Metadata used to persist/resume a save-and-return draft. */
export interface DraftMeta {
  flowId: string
  title: string
  resumeHref: string
}

export interface FieldBlocker {
  name: string
  label: string
  error: string
}

/** The inputs of `<application-provider>`, as signals, read by the state. */
interface ProviderSource {
  steps: Signal<ApplicationStep[]>
  initialData: Signal<ApplicationFormData | undefined>
  draftId: Signal<string | undefined>
  draftMeta: Signal<DraftMeta | undefined>
}

@Injectable()
export class ApplicationFlowState {
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY)
  private readonly source = signal<ProviderSource | null>(null)

  readonly steps = computed(() => this.source()?.steps() ?? [])
  readonly draftId = computed(() => this.source()?.draftId())
  readonly draftMeta = computed(() => this.source()?.draftMeta())

  readonly currentStep = signal(0)
  readonly formData = signal<ApplicationFormData>({})
  // Furthest step reached; a step earns a green check only once the user has moved beyond it and its data is valid.
  readonly furthestStep = signal(0)
  readonly hasErrors = signal(false)
  private readonly touched = signal<Record<string, boolean>>({})
  readonly submitAttempted = signal(false)
  readonly invalidFields = signal<FieldBlocker[]>([])
  /** A saved draft held while the "Continue where you left off?" dialog is open. */
  readonly pendingDraft = signal<ApplicationDraft | null>(null)

  readonly isFirstStep = computed(() => this.currentStep() === 0)
  readonly isLastStep = computed(() => this.currentStep() === this.steps().length - 1)
  readonly hasDraftSupport = computed(() => Boolean(this.draftId() && this.draftMeta()))

  /** Set once the draft check has run on mount (the prototype's `hydrated` ref). */
  readonly hydrated = signal(false)

  // Validator registry: only mounted (visible) fields, so hidden conditional fields are never validated; entries carry the label for the summary.
  private readonly validators = new Map<string, { rules: FieldValidation; label: string }>()

  /** Called by `<application-provider>` once its inputs are set. */
  readonly bind = (source: ProviderSource): void => {
    this.source.set(source)
    this.formData.set(untracked(source.initialData) ?? {})
  }

  // Green-check steps: beyond the furthest-reached marker and valid; recomputed live, so clearing a required field removes the check.
  readonly completedSteps = computed(() => {
    const steps = this.steps()
    const furthest = this.furthestStep()
    const formData = this.formData()
    const done: number[] = []
    for (let i = 0; i < steps.length; i++) {
      if (i < furthest && this.isStepDataValid(i, formData)) done.push(i)
    }
    return done
  })

  // Step completeness: flows with a `validate` fn check live form data; flows of self-validating fields count as valid once passed.
  private isStepDataValid(index: number, formData: ApplicationFormData): boolean {
    const step = this.steps()[index]
    if (!step) return false
    return step.validate ? step.validate(formData) : true
  }

  readonly setFormData = (data: ApplicationFormData): void => this.formData.set(data)

  readonly updateFormData = (updates: Partial<ApplicationFormData>): void => {
    this.formData.update((prev) => ({ ...prev, ...updates }))
  }

  readonly setHasErrors = (hasErrors: boolean): void => this.hasErrors.set(hasErrors)

  readonly goToStep = (step: number): void => {
    if (step >= 0 && step < this.steps().length) this.currentStep.set(step)
  }

  readonly registerValidator = (name: string, rules: FieldValidation, label?: string): void => {
    // Re-inserting keeps the latest label; Map insertion order matches mount/DOM order for the summary.
    this.validators.set(name, { rules, label: label ?? name })
  }

  readonly unregisterValidator = (name: string): void => {
    this.validators.delete(name)
  }

  readonly setFieldTouched = (name: string): void => {
    this.touched.update((prev) => (prev[name] ? prev : { ...prev, [name]: true }))
  }

  /** Returns the error to display for a field (respects touched / submit-attempted). */
  readonly getFieldError = (name: string, rules?: FieldValidation): string | undefined => {
    if (!this.touched()[name] && !this.submitAttempted()) return undefined
    const formData = this.formData()
    const effective = rules ?? this.validators.get(name)?.rules
    return validateValue(formData[name], effective, formData)
  }

  /** Read-only `blockersOnCurrentStep`: reports what would block a forward move without flagging fields. */
  readonly peekBlockers = (): FieldBlocker[] => {
    const invalid: FieldBlocker[] = []
    const formData = untracked(this.formData)
    this.validators.forEach(({ rules, label }, name) => {
      const err = validateValue(formData[name], rules, formData)
      if (err) invalid.push({ name, label, error: err })
    })
    return invalid
  }

  /** Same checks as `validateVisible`, returning the blocking fields and showing the inline errors. */
  readonly blockersOnCurrentStep = (): FieldBlocker[] => {
    const invalid = this.peekBlockers()
    this.submitAttempted.set(true)
    this.hasErrors.set(invalid.length > 0)
    this.invalidFields.set(invalid)
    return invalid
  }

  /** Validates all currently-mounted (visible) fields; returns true if valid. */
  readonly validateVisible = (): boolean => this.blockersOnCurrentStep().length === 0

  readonly saveProgress = (opts?: { silent?: boolean }): void => {
    const draftId = untracked(this.draftId)
    const draftMeta = untracked(this.draftMeta)
    if (!draftId || !draftMeta) return
    const currentStep = untracked(this.currentStep)
    const totalSteps = untracked(this.steps).length
    this.applicationsRepository.saveDraft({
      draftId,
      flowId: draftMeta.flowId,
      title: draftMeta.title,
      resumeHref: draftMeta.resumeHref,
      formData: untracked(this.formData),
      currentStep,
      progress: Math.round(((currentStep + 1) / totalSteps) * 100),
      updatedAt: new Date().toISOString(),
    })
    if (!opts?.silent) {
      toast.success("Progress saved", {
        description: "You can resume this application from your dashboard.",
      })
    }
  }

  // Gate forward movement: registered validators and any `validate` fn both apply; shows the most helpful message.
  private canAdvanceFromCurrent(): boolean {
    const fieldsValid = this.validateVisible()
    const step = this.steps()[untracked(this.currentStep)]
    const dataValid = step?.validate ? step.validate(untracked(this.formData)) : true
    if (!fieldsValid) {
      toast.error("Some required fields need attention", {
        description: "See the highlighted list at the top of the step to jump to each one.",
      })
      return false
    }
    if (!dataValid) {
      toast.error("Please complete all required fields on this step before continuing.")
      return false
    }
    return true
  }

  readonly nextStep = (): void => {
    if (!this.canAdvanceFromCurrent()) return
    this.submitAttempted.set(false)
    this.touched.set({})
    if (untracked(this.currentStep) < this.steps().length - 1) {
      pulseProgress()
      this.currentStep.update((prev) => prev + 1)
      if (typeof window !== "undefined") window.scrollTo({ top: 0, behavior: "smooth" })
    }
  }

  readonly prevStep = (): void => {
    if (untracked(this.currentStep) > 0) {
      pulseProgress()
      this.submitAttempted.set(false)
      this.currentStep.update((prev) => prev - 1)
      if (typeof window !== "undefined") window.scrollTo({ top: 0, behavior: "smooth" })
    }
  }

  // Kept for API compatibility: nudges the furthest-reached marker.
  readonly markStepComplete = (step: number): void => {
    this.furthestStep.update((f) => Math.max(f, step + 1))
  }

  // Jump from the step indicator; backward is always allowed, forward needs the current step to pass.
  readonly attemptGoToStep = (target: number): void => {
    const currentStep = untracked(this.currentStep)
    if (target < 0 || target >= this.steps().length || target === currentStep) return
    if (target < currentStep) {
      pulseProgress()
      this.submitAttempted.set(false)
      this.currentStep.set(target)
      if (typeof window !== "undefined") window.scrollTo({ top: 0, behavior: "smooth" })
      return
    }
    if (!this.canAdvanceFromCurrent()) return
    pulseProgress()
    this.submitAttempted.set(false)
    this.touched.set({})
    this.currentStep.set(target)
    if (typeof window !== "undefined") window.scrollTo({ top: 0, behavior: "smooth" })
  }

  readonly resumeDraft = (): void => {
    const pending = this.pendingDraft()
    if (!pending) return
    // Keep prefilled identity fields (e.g. licence number) while saved draft values win.
    this.formData.set({ ...(untracked(this.source()!.initialData) ?? {}), ...pending.formData })
    const resumeStep = Math.min(pending.currentStep, this.steps().length - 1)
    this.currentStep.set(resumeStep)
    // Steps already moved past stay eligible for a green check.
    this.furthestStep.set(resumeStep)
    this.pendingDraft.set(null)
    toast.info("Draft restored", { description: "We picked up where you left off." })
  }

  readonly discardDraft = (): void => {
    const pending = this.pendingDraft()
    if (!pending) return
    this.applicationsRepository.deleteDraft(pending.draftId)
    this.pendingDraft.set(null)
  }
}

/** The prototype's useApplicationContext(). */
export function injectApplicationContext(): ApplicationFlowState {
  const context = inject(ApplicationFlowState, { optional: true })
  if (!context) {
    throw new Error("useApplicationContext must be used within ApplicationProvider")
  }
  return context
}

function formatDraftDate(iso: string): string {
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return "earlier"
  const days = Math.floor((Date.now() - d.getTime()) / 86_400_000)
  if (days === 0) return "today"
  if (days === 1) return "yesterday"
  if (days < 7) return `${days} days ago`
  return d.toLocaleDateString("en-CA", { month: "long", day: "numeric", year: "numeric" })
}

const AUTOSAVE_INTERVAL_MS = 30_000

@Component({
  selector: "application-provider",
  imports: [ALERT_DIALOG],
  providers: [ApplicationFlowState],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <ng-content />
    <!-- Explicit resume choice instead of a silent jump to the saved step. -->
    <ui-alert-dialog [state]="state.pendingDraft() !== null ? 'open' : 'closed'">
      <ui-alert-dialog-content *uiAlertDialogPortal>
        <div uiAlertDialogHeader>
          <h2 uiAlertDialogTitle>Continue where you left off?</h2>
          <p uiAlertDialogDescription>{{ description() }}</p>
        </div>
        <div uiAlertDialogFooter>
          <button uiAlertDialogCancel (click)="state.discardDraft()">Start fresh</button>
          <button uiAlertDialogAction (click)="state.resumeDraft()">Resume draft</button>
        </div>
      </ui-alert-dialog-content>
    </ui-alert-dialog>
  `,
})
export class ApplicationProvider implements OnInit {
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY)
  readonly steps = input<ApplicationStep[]>([])
  readonly initialData = input<ApplicationFormData | undefined>(undefined)
  /** Enables save-and-return persistence keyed by this id. */
  readonly draftId = input<string | undefined>(undefined)
  readonly draftMeta = input<DraftMeta | undefined>(undefined)

  protected readonly state = inject(ApplicationFlowState)

  protected readonly description = computed(() => {
    const pendingDraft = this.state.pendingDraft()
    const steps = this.state.steps()
    return (
      `You have a saved draft of this application from ${pendingDraft ? formatDraftDate(pendingDraft.updatedAt) : "earlier"}` +
      `${pendingDraft && pendingDraft.currentStep > 0 ? `, paused at step ${pendingDraft.currentStep + 1} of ${steps.length}` : ""}` +
      `. Resume it, or start a fresh application (this will delete the draft).`
    )
  })

  constructor() {
    const state = this.state

    // Effects run in the prototype's order: draft load, step-change reset, debounced autosave, interval autosave; each waits for first render.
    afterNextRender(() => {
      // Hydrate from a saved draft if one exists for this draftId.
      const draftId = untracked(state.draftId)
      if (untracked(state.hydrated) || !draftId) return
      state.hydrated.set(true)
      const draft = this.applicationsRepository.draft(draftId)
      // A draft that never got past the first step with nothing typed isn't worth interrupting for.
      if (draft && (draft.currentStep > 0 || Object.keys(draft.formData).length > 0)) {
        state.pendingDraft.set(draft)
      }
    })

    // Track the furthest step reached, so completion means moved past it.
    effect(() => {
      const currentStep = state.currentStep()
      untracked(() => {
        state.furthestStep.update((f) => Math.max(f, currentStep))
        // A fresh step starts with a clean validation slate.
        state.invalidFields.set([])
      })
    })

    // Debounced auto-save after any change, if drafts are enabled.
    effect((onCleanup) => {
      state.formData()
      state.currentStep()
      state.steps()
      const draftId = state.draftId()
      const draftMeta = state.draftMeta()
      if (!draftId || !draftMeta || !state.hydrated()) return
      const t = setTimeout(() => state.saveProgress({ silent: true }), 600)
      onCleanup(() => clearTimeout(t))
    })

    // Periodic auto-save on a fixed cadence, independent of the debounced save and manual save.
    effect((onCleanup) => {
      state.currentStep()
      state.steps()
      const draftId = state.draftId()
      const draftMeta = state.draftMeta()
      if (!draftId || !draftMeta) return
      const id = setInterval(() => {
        if (untracked(state.hydrated)) state.saveProgress({ silent: true })
      }, AUTOSAVE_INTERVAL_MS)
      onCleanup(() => clearInterval(id))
    })
  }

  ngOnInit(): void {
    this.state.bind({
      steps: this.steps,
      initialData: this.initialData,
      draftId: this.draftId,
      draftMeta: this.draftMeta,
    })
  }
}
