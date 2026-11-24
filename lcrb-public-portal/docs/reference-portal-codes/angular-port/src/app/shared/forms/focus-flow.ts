/** Guided Mode: a second renderer over the same `ApplicationProvider`, one question at a time. Leaving a step re-validates its visible fields. */
import {
  afterNextRender,
  afterRenderEffect,
  ChangeDetectionStrategy,
  Component,
  computed,
  DestroyRef,
  ElementRef,
  inject,
  input,
  output,
  signal,
  type TemplateRef,
  untracked,
  viewChild,
} from "@angular/core"
import { AI_REPOSITORY } from "@data-access/repositories/ai/ai.repository"
import { NgTemplateOutlet } from "@angular/common"
import { Router } from "@angular/router"
import { toast } from "@spartan-ng/brain/sonner"
import { Button } from "@shared/ui/primitives/button"
import { LucideIcon } from "@shared/ui/primitives/lucide-icon"
import { Progress } from "@shared/ui/primitives/progress"
import { POPOVER } from "@shared/ui/primitives/popover"
import { Textarea } from "@shared/ui/primitives/textarea"
import { cn } from "@shared/util/utils"
import { validateValue } from "./validation"
import { ConfigField, proseHelpsFor } from "./fields"
import { injectApplicationContext } from "./index"
import type { FlowConfig } from "@domain/forms/flow-config"
import {
  buildQuestionQueue,
  effectiveRules,
  firstQuestionOfStep,
  indexOfQuestion,
  type Question,
} from "./question-queue"

interface InterpretState {
  status: "idle" | "loading" | "error"
  message?: string
}

interface StepBlocker {
  name: string
  label: string
  error: string
}

@Component({
  selector: "focus-shell",
  imports: [Button, LucideIcon, NgTemplateOutlet, Progress],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <ng-template #body><ng-content /></ng-template>
    <!-- Full-screen: an opaque layer above all host chrome, reduced to the progress line, step label and an "Exit full screen" control. -->
    @if (fullscreen()) {
      <div
        class="fixed inset-0 z-[60] flex flex-col overflow-y-auto bg-background"
        role="region"
        [attr.aria-label]="config().title + ' — full screen'"
      >
        <div uiProgress [value]="progressValue()" class="h-1 rounded-none"></div>
        <div class="mx-auto flex w-full max-w-4xl items-center justify-between gap-3 px-4 py-4 sm:px-6">
          <p class="truncate text-xs text-muted-foreground">{{ stepLabel() }}</p>
          <button uiButton type="button" variant="ghost" size="sm" (click)="toggleFullscreen.emit()">
            <svg lucide="Minimize2" class="mr-2 h-4 w-4" aria-hidden="true"></svg>
            Exit full screen
          </button>
        </div>
        <div class="flex flex-1 items-start justify-center px-4 py-12 sm:px-6 sm:py-20 lg:py-28">
          <ng-container [ngTemplateOutlet]="body" />
        </div>
      </div>
    } @else {
      <div class="flex min-h-[calc(100vh-4rem)] flex-col bg-background">
        <div uiProgress [value]="progressValue()" class="h-1 rounded-none"></div>

        <div class="border-b bg-card">
          <div class="mx-auto flex max-w-4xl flex-wrap items-center justify-between gap-3 px-4 py-3 sm:px-6">
            <div class="min-w-0">
              <p class="truncate text-sm font-semibold">{{ config().title }}</p>
              <p class="truncate text-xs text-muted-foreground">{{ eyebrow() }} · {{ stepLabel() }}</p>
            </div>
            <div class="flex items-center gap-2">
              @if (jumpTo(); as jump) {
                <ng-container [ngTemplateOutlet]="jump" />
              }
              <button uiButton type="button" variant="outline" size="sm" (click)="saveExit.emit()">
                <svg lucide="Save" class="mr-2 h-4 w-4" aria-hidden="true"></svg>
                {{ hasDraftSupport() ? "Save & exit" : "Exit" }}
              </button>
              <button
                uiButton
                type="button"
                variant="ghost"
                size="sm"
                (click)="toggleFullscreen.emit()"
                aria-label="Enter full screen"
              >
                <svg lucide="Maximize2" class="h-4 w-4 sm:mr-2" aria-hidden="true"></svg>
                <span class="hidden sm:inline">Full screen</span>
              </button>
              <button uiButton type="button" variant="ghost" size="sm" (click)="exitMode.emit()">
                <svg lucide="X" class="mr-2 h-4 w-4" aria-hidden="true"></svg>
                Full form
              </button>
            </div>
          </div>
        </div>

        <div class="flex flex-1 items-start justify-center px-4 py-12 sm:px-6 sm:py-16">
          <ng-container [ngTemplateOutlet]="body" />
        </div>
      </div>
    }
  `,
})
export class FocusShell {
  readonly config = input.required<FlowConfig>()
  readonly hasDraftSupport = input.required<boolean>()
  readonly progressValue = input.required<number>()
  readonly eyebrow = input.required<string>()
  readonly stepLabel = input.required<string>()
  readonly jumpTo = input<TemplateRef<unknown> | undefined>(undefined)
  readonly fullscreen = input.required<boolean>()
  readonly exitMode = output<void>()
  readonly saveExit = output<void>()
  readonly toggleFullscreen = output<void>()
}

@Component({
  selector: "step-jump",
  imports: [Button, ...POPOVER],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <ui-popover align="end" [state]="open() ? 'open' : 'closed'" (stateChanged)="open.set($event === 'open')">
      <button uiButton uiPopoverTrigger type="button" variant="outline" size="sm">All steps</button>
      <div uiPopoverContent *uiPopoverPortal class="w-72 p-2">
        <p class="px-2 py-1.5 text-xs font-medium uppercase tracking-wide text-muted-foreground">Jump to a step</p>
        <ul>
          @for (step of config().steps; track step.id; let i = $index) {
            <li>
              <button
                type="button"
                [disabled]="count(i) === 0"
                (click)="pick(i)"
                [attr.class]="itemClass(i)"
              >
                <span class="truncate">{{ i + 1 }}. {{ step.title }}</span>
                <span [attr.class]="countClass(i)">{{ count(i) === 0 ? "n/a" : count(i) }}</span>
              </button>
            </li>
          }
        </ul>
      </div>
    </ui-popover>
  `,
})
export class StepJump {
  readonly config = input.required<FlowConfig>()
  readonly queue = input.required<Question[]>()
  readonly activeStep = input.required<number>()
  readonly jump = output<number>()

  protected readonly open = signal(false)

  protected count(i: number): number {
    return this.queue().filter((q) => q.stepIndex === i).length
  }

  protected itemClass(i: number): string {
    return cn(
      "flex w-full items-center justify-between gap-2 rounded-md px-2 py-2 text-left text-sm transition-colors",
      i === this.activeStep() ? "bg-[#013366] text-white" : "hover:bg-muted",
      this.count(i) === 0 && "cursor-not-allowed opacity-40",
    )
  }

  protected countClass(i: number): string {
    return cn("shrink-0 text-xs", i === this.activeStep() ? "text-white/80" : "text-muted-foreground")
  }

  protected pick(i: number): void {
    this.jump.emit(i)
    this.open.set(false)
  }
}

// The remaining selectors roughly double the answer text (16px to 30px) and grow the control; file inputs keep their native size.
const FIELD_WRAP_CLASS = cn(
  "mt-8 [&_[data-field-label]]:sr-only [&_[data-field-help]]:hidden",
  "[&_input]:h-auto [&_input]:py-3 [&_input]:text-3xl",
  "[&_textarea]:min-h-40 [&_textarea]:py-3 [&_textarea]:text-3xl",
  "[&_button[role=combobox]]:h-auto [&_button[role=combobox]]:py-3 [&_button[role=combobox]]:text-3xl",
  "[&_[role=radiogroup]]:gap-4 [&_[role=radiogroup]_label]:text-2xl",
  "[&_[role=radio]]:size-6 [&_[role=checkbox]]:size-6",
  "[&_[role=checkbox]~label]:text-2xl",
  // File inputs keep their native size: a 30px file picker is unusable.
  "[&_input[type=file]]:text-base [&_input[type=file]]:py-2",
)

@Component({
  selector: "focus-flow",
  imports: [Button, ConfigField, FocusShell, LucideIcon, NgTemplateOutlet, StepJump, Textarea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    @if (onReview()) {
      <focus-shell
        [config]="config()"
        (exitMode)="exitMode.emit()"
        (saveExit)="handleSaveExit()"
        [hasDraftSupport]="ctx.hasDraftSupport()"
        [progressValue]="100"
        eyebrow="Final step"
        stepLabel="Review & submit"
        [fullscreen]="fullscreen()"
        (toggleFullscreen)="toggleFullscreen()"
      >
        <div class="mx-auto w-full max-w-2xl">
          <h1 #heading tabindex="-1" class="text-3xl font-semibold tracking-tight outline-none sm:text-4xl">
            Review your application
          </h1>
          <p class="mt-3 text-base leading-relaxed text-muted-foreground">
            Check your answers before submitting. You can go back to change anything.
          </p>
          <div class="mt-8"><ng-container [ngTemplateOutlet]="reviewSlot() ?? null" /></div>
          @if (stepBlockers().length > 0) {
            <div role="alert" class="mt-6 rounded-lg border border-destructive/30 bg-destructive/5 p-4">
              <p class="text-sm font-semibold text-destructive">Before submitting, these need attention:</p>
              <ul class="mt-2 space-y-1">
                @for (b of stepBlockers(); track b.name) {
                  <li class="text-sm text-destructive">{{ b.label }} — {{ b.error }}</li>
                }
              </ul>
            </div>
          }
          <div class="mt-10 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <button uiButton type="button" variant="ghost" (click)="handleBack()" class="sm:order-1">
              <svg lucide="ArrowLeft" class="mr-2 h-4 w-4" aria-hidden="true"></svg>
              Back
            </button>
            <button
              uiButton
              type="button"
              size="lg"
              (click)="handleReviewSubmit()"
              [disabled]="submitting()"
              class="bg-[#013366] hover:bg-[#01254a] sm:order-2"
            >
              @if (submitting()) {
                <svg lucide="Loader2" class="mr-2 h-4 w-4 animate-spin" aria-hidden="true"></svg>
                Submitting…
              } @else {
                Submit application
                <svg lucide="Check" class="ml-2 h-4 w-4" aria-hidden="true"></svg>
              }
            </button>
          </div>
        </div>
      </focus-shell>
    } @else if (!question()) {
      <focus-shell
        [config]="config()"
        (exitMode)="exitMode.emit()"
        (saveExit)="handleSaveExit()"
        [hasDraftSupport]="ctx.hasDraftSupport()"
        [progressValue]="100"
        [eyebrow]="config().title"
        stepLabel="Ready to review"
        [fullscreen]="fullscreen()"
        (toggleFullscreen)="toggleFullscreen()"
      >
        <div class="mx-auto w-full max-w-2xl text-center">
          <h1 class="text-3xl font-semibold tracking-tight">Nothing left to answer</h1>
          <p class="mt-3 text-muted-foreground">Based on your answers, there are no further questions.</p>
          <button
            uiButton
            type="button"
            size="lg"
            (click)="onReview.set(true)"
            class="mt-8 bg-[#013366] hover:bg-[#01254a]"
          >
            Go to review
            <svg lucide="ArrowRight" class="ml-2 h-4 w-4" aria-hidden="true"></svg>
          </button>
        </div>
      </focus-shell>
    } @else {
      @let q = question()!;
      <ng-template #jump>
        <step-jump [config]="config()" [queue]="queue()" [activeStep]="q.stepIndex" (jump)="onJump($event)" />
      </ng-template>
      <focus-shell
        [config]="config()"
        (exitMode)="exitMode.emit()"
        (saveExit)="handleSaveExit()"
        [hasDraftSupport]="ctx.hasDraftSupport()"
        [progressValue]="progressValue()"
        [eyebrow]="q.stepTitle"
        [stepLabel]="'Question ' + (index() + 1) + ' of ' + queue().length"
        [fullscreen]="fullscreen()"
        (toggleFullscreen)="toggleFullscreen()"
        [jumpTo]="jump"
      >
        <div class="mx-auto w-full max-w-2xl">
          <p class="text-sm font-medium uppercase tracking-wide text-[#1A5A96]">{{ q.sectionTitle ?? q.stepTitle }}</p>

          <!-- The question as the page heading; the field's own label is visually hidden but still names the control. -->
          <h1
            #heading
            tabindex="-1"
            class="mt-2 text-pretty text-3xl font-semibold leading-tight tracking-tight outline-none sm:text-4xl"
          >
            {{ q.field.label }}@if (q.field.required) {<span class="ml-2 align-middle text-base font-normal text-muted-foreground">(required)</span>}
          </h1>

          @if (q.field.help) {
            <p class="mt-4 text-base leading-relaxed text-muted-foreground">{{ q.field.help }}</p>
          }

          <!-- Terms and declarations the applicant is about to agree to, shown in full. -->
          @if (q.field.readBefore; as rb) {
            <section
              [attr.aria-label]="rb.title"
              class="mt-6 max-h-72 overflow-y-auto rounded-lg border bg-card p-5 text-sm leading-relaxed"
            >
              <h2 class="mb-3 text-base font-semibold">{{ rb.title }}</h2>
              <div class="flex flex-col gap-3">
                @for (p of rb.paragraphs; track $index) {
                  <p>
                    @if (p.heading) {<strong>{{ p.heading }}&ngsp;</strong>}{{ p.text }}
                  </p>
                }
              </div>
            </section>
          }

          <!-- The real control from the classic form; the wrapper hides the duplicate label and "?" trigger (help prints as body copy) and enlarges the control. -->
          <div [attr.class]="fieldWrapClass">
            <div configField [field]="q.field"></div>
          </div>

          @if (aiNote()) {
            <p class="mt-3 flex items-start gap-2 text-sm text-[#1A5A96]">
              <svg lucide="Sparkles" class="mt-0.5 h-4 w-4 shrink-0" aria-hidden="true"></svg>
              {{ aiNote() }}
            </p>
          }

          @if (error()) {
            <p role="alert" class="mt-3 flex items-start gap-2 text-sm font-medium text-destructive">
              <svg lucide="CircleAlert" class="mt-0.5 h-4 w-4 shrink-0" aria-hidden="true"></svg>
              {{ error() }}
            </p>
          }

          @if (stepBlockers().length > 0) {
            <div role="alert" class="mt-6 rounded-lg border border-destructive/30 bg-destructive/5 p-4">
              <p class="text-sm font-semibold text-destructive">
                Before moving on, these answers on this step need attention:
              </p>
              <ul class="mt-2 space-y-1">
                @for (b of stepBlockers(); track b.name) {
                  <li>
                    <button
                      type="button"
                      (click)="jumpToBlocker(b.name)"
                      class="text-left text-sm text-destructive underline underline-offset-2 hover:no-underline"
                    >
                      {{ b.label }} — {{ b.error }}
                    </button>
                  </li>
                }
              </ul>
            </div>
          }

          @if (allowProse()) {
            <div class="mt-8 rounded-xl border bg-muted/30 p-4">
              <label for="guided-prose" class="flex items-center gap-2 text-sm font-medium">
                <svg lucide="Sparkles" class="h-4 w-4 text-[#1A5A96]" aria-hidden="true"></svg>
                Prefer to describe it in your own words?
              </label>
              <textarea
                uiTextarea
                id="guided-prose"
                [value]="prose()"
                (input)="onProseInput($event)"
                rows="2"
                placeholder="Type your answer however you like and we'll fill in the field above."
                class="mt-2 resize-none bg-background"
              ></textarea>
              <div class="mt-2 flex items-center gap-3">
                <button
                  uiButton
                  type="button"
                  variant="outline"
                  size="sm"
                  (click)="handleInterpret()"
                  [disabled]="!prose().trim() || interpret().status === 'loading'"
                >
                  @if (interpret().status === "loading") {
                    <svg lucide="Loader2" class="mr-2 h-4 w-4 animate-spin" aria-hidden="true"></svg>
                    Reading…
                  } @else {
                    Fill in for me
                  }
                </button>
                <p class="text-xs text-muted-foreground">You'll always see the result and can change it.</p>
              </div>
              @if (interpret().status === "error" && interpret().message) {
                <p role="alert" class="mt-2 text-sm text-destructive">{{ interpret().message }}</p>
              }
            </div>
          }

          <div class="mt-10 flex items-center justify-between gap-3">
            <button uiButton type="button" variant="ghost" (click)="handleBack()" [disabled]="index() === 0">
              <svg lucide="ArrowLeft" class="mr-2 h-4 w-4" aria-hidden="true"></svg>
              Back
            </button>
            <button
              uiButton
              type="button"
              size="lg"
              (click)="handleContinue()"
              class="bg-[#013366] px-8 hover:bg-[#01254a]"
            >
              {{ index() === queue().length - 1 ? "Review" : "Continue" }}
              <svg lucide="ArrowRight" class="ml-2 h-4 w-4" aria-hidden="true"></svg>
            </button>
          </div>
        </div>
      </focus-shell>
    }
  `,
})
export class FocusFlow {
  private readonly ai = inject(AI_REPOSITORY)
  readonly config = input.required<FlowConfig>()
  readonly submitting = input(false)
  readonly reviewSlot = input<TemplateRef<unknown> | undefined>(undefined)
  readonly exitMode = output<void>()
  readonly submitFlow = output<void>()

  private readonly router = inject(Router)
  protected readonly ctx = injectApplicationContext()
  protected readonly fieldWrapClass = FIELD_WRAP_CLASS

  protected readonly queue = computed(() => buildQuestionQueue(this.config(), this.ctx.formData()))

  protected readonly cursorName = signal<string | null>(null)
  protected readonly onReview = signal(false)
  protected readonly error = signal<string | null>(null)
  protected readonly stepBlockers = signal<StepBlocker[]>([])

  protected readonly prose = signal("")
  protected readonly interpret = signal<InterpretState>({ status: "idle" })
  protected readonly aiNote = signal<string | null>(null)

  // Immersive full-screen state lives here, not in the shell, so it survives moving between the review, no-questions and question screens.
  protected readonly fullscreen = signal(false)

  private readonly heading = viewChild<ElementRef<HTMLElement>>("heading")

  // Resolve the cursor in the live queue; if the anchored field was just hidden, fall back to the first question of the current step.
  private readonly resolved = computed(() => indexOfQuestion(this.queue(), this.cursorName()))
  protected readonly index = computed(() => {
    const resolved = this.resolved()
    return resolved !== -1 ? resolved : Math.max(0, firstQuestionOfStep(this.queue(), this.ctx.currentStep()))
  })
  protected readonly question = computed<Question | undefined>(() => this.queue()[this.index()])
  private readonly questionStep = computed(() => this.question()?.stepIndex)

  protected readonly progressValue = computed(() => {
    const queue = this.queue()
    const formData = this.ctx.formData()
    const answered = queue.filter((q) => {
      const v = formData[q.field.name]
      return v !== undefined && v !== "" && v !== false && !(Array.isArray(v) && v.length === 0)
    }).length
    return queue.length ? (answered / queue.length) * 100 : 0
  })

  // Prose only where it genuinely saves effort (see proseHelpsFor).
  protected readonly allowProse = computed(() => {
    const q = this.question()
    return q ? proseHelpsFor(q.field) : false
  })

  constructor() {
    const fullscreen = this.fullscreen
    const destroyRef = inject(DestroyRef)

    // Full-screen: prevent a second scrollbar behind the overlay and let Escape leave it.
    afterRenderEffect((onCleanup) => {
      if (!fullscreen()) return
      const previousOverflow = document.body.style.overflow
      document.body.style.overflow = "hidden"
      const onKeyDown = (e: KeyboardEvent) => {
        if (e.key === "Escape") fullscreen.set(false)
      }
      window.addEventListener("keydown", onKeyDown)
      onCleanup(() => {
        document.body.style.overflow = previousOverflow
        window.removeEventListener("keydown", onKeyDown)
      })
    })

    // Flag <body> while Guided Mode is mounted so global CSS can enlarge portalled select options; removed on destroy.
    afterNextRender(() => {
      document.body.dataset["guidedMode"] = "true"
    })
    destroyRef.onDestroy(() => {
      delete document.body.dataset["guidedMode"]
    })

    // Adopt the resolved question as the anchor after a conditional change.
    afterRenderEffect(() => {
      const question = this.question()
      const cursorName = this.cursorName()
      if (question && question.field.name !== cursorName) this.cursorName.set(question.field.name)
    })

    // Keep the provider step aligned with the question on screen, so drafts save the right step; runs only when the step changes.
    afterRenderEffect(() => {
      this.questionStep()
      untracked(() => {
        const question = this.question()
        if (question && question.stepIndex !== this.ctx.currentStep()) this.ctx.goToStep(question.stepIndex)
      })
    })

    // Reset per-question state and focus the new heading so screen-reader users hear the change.
    afterRenderEffect(() => {
      this.cursorName()
      this.onReview()
      untracked(() => {
        this.prose.set("")
        this.interpret.set({ status: "idle" })
        this.aiNote.set(null)
        this.error.set(null)
        this.stepBlockers.set([])
        this.heading()?.nativeElement.focus()
      })
    })
  }

  protected toggleFullscreen(): void {
    this.fullscreen.update((v) => !v)
  }

  /** Validate every visible field of `stepIndex`, returning the blocking fields in flow order. */
  private blockersOnStep(stepIndex: number): StepBlocker[] {
    const formData = this.ctx.formData()
    return this.queue()
      .filter((q) => q.stepIndex === stepIndex)
      .map((q) => {
        const err = validateValue(formData[q.field.name], effectiveRules(q.field), formData)
        return err ? { name: q.field.name, label: q.field.label, error: err } : null
      })
      .filter((b): b is StepBlocker => b !== null)
  }

  private goToIndex(next: number): void {
    const target = this.queue()[next]
    if (target) this.cursorName.set(target.field.name)
  }

  protected handleBack(): void {
    if (this.onReview()) {
      this.onReview.set(false)
      const last = this.queue().length - 1
      if (last >= 0) this.goToIndex(last)
      return
    }
    if (this.index() > 0) this.goToIndex(this.index() - 1)
  }

  protected handleContinue(): void {
    const question = this.question()
    if (!question) {
      this.onReview.set(true)
      return
    }
    const formData = this.ctx.formData()
    const queue = this.queue()
    const index = this.index()

    // Validate the on-screen field first; its own message is the most relevant.
    this.ctx.setFieldTouched(question.field.name)
    const ownError = validateValue(formData[question.field.name], effectiveRules(question.field), formData)
    if (ownError) {
      this.error.set(ownError)
      return
    }

    const next = index + 1
    const leavingStep = !queue[next] || queue[next].stepIndex !== question.stepIndex

    // Re-check the whole step only when crossing a boundary, so moving inside a step never flags unreached questions.
    if (leavingStep) {
      const blockers = this.blockersOnStep(question.stepIndex).filter((b) => b.name !== question.field.name)
      if (blockers.length > 0) {
        this.stepBlockers.set(blockers)
        toast.error("Some earlier answers need attention", {
          description: "We've listed what's missing on this step below.",
        })
        return
      }
    }

    this.error.set(null)
    this.stepBlockers.set([])
    if (!queue[next]) {
      this.onReview.set(true)
      return
    }
    this.goToIndex(next)
  }

  protected onProseInput(event: Event): void {
    this.prose.set((event.target as HTMLTextAreaElement).value)
    if (this.interpret().status === "error") this.interpret.set({ status: "idle" })
  }

  protected async handleInterpret(): Promise<void> {
    const question = this.question()
    const prose = this.prose()
    if (!question || !prose.trim()) return
    this.interpret.set({ status: "loading" })
    this.aiNote.set(null)
    try {
      const res = await this.ai.fetchApi("/api/interpret-answer", {
        method: "POST",
        headers: { "content-type": "application/json" },
        body: JSON.stringify({
          field: {
            label: question.field.label,
            type: question.field.type,
            hint: question.field.hint,
            help: question.field.help,
            options: question.field.options,
          },
          text: prose,
        }),
      })
      const data = (await res.json()) as {
        value?: string | string[] | boolean | null
        confidence?: number
        note?: string
        error?: string
      }
      if (!res.ok) {
        this.interpret.set({ status: "error", message: data.error ?? "Could not read that answer." })
        return
      }
      // A low-confidence or unusable reading is never written; the user is asked to answer directly.
      if (data.value === null || data.value === undefined || (data.confidence ?? 0) < 0.5) {
        this.interpret.set({
          status: "error",
          message:
            data.note?.trim() || "I couldn't tell from that. Try rephrasing, or answer using the field above.",
        })
        return
      }
      this.ctx.updateFormData({ [question.field.name]: data.value })
      this.ctx.setFieldTouched(question.field.name)
      this.error.set(null)
      this.prose.set("")
      this.interpret.set({ status: "idle" })
      this.aiNote.set(data.note?.trim() ? data.note.trim() : "Filled in from what you wrote — check it's right.")
    } catch {
      this.interpret.set({ status: "error", message: "Something went wrong reading that. Please answer directly." })
    }
  }

  protected handleSaveExit(): void {
    if (this.ctx.hasDraftSupport()) this.ctx.saveProgress()
    void this.router.navigateByUrl(this.config().returnTo)
  }

  // Submit from review, gated like the classic form; the review has required declaration checkboxes outside the queue, and `blockersOnCurrentStep()` flags them red inline.
  protected handleReviewSubmit(): void {
    const blocking = this.ctx.blockersOnCurrentStep()
    if (blocking.length > 0) {
      this.stepBlockers.set(blocking)
      return
    }
    this.stepBlockers.set([])
    this.submitFlow.emit()
  }

  protected jumpToBlocker(name: string): void {
    const target = indexOfQuestion(this.queue(), name)
    if (target !== -1) this.goToIndex(target)
  }

  protected onJump(stepIndex: number): void {
    const question = this.question()
    if (!question) return
    // Backward jumps are free; forward jumps must pass the current step, as in the classic form.
    if (stepIndex > question.stepIndex) {
      const blockers = this.blockersOnStep(question.stepIndex)
      if (blockers.length > 0) {
        this.stepBlockers.set(blockers)
        toast.error("Finish this step first", {
          description: "We've listed what's missing below.",
        })
        return
      }
    }
    const target = firstQuestionOfStep(this.queue(), stepIndex)
    if (target !== -1) this.goToIndex(target)
  }
}
