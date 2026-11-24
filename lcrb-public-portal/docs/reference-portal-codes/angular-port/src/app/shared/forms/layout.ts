/** Application framework presentation; components styled by a parent (`space-y-*`) are attribute hosts on a real element, e.g. `<div infoBox>`. */
import {
  booleanAttribute,
  ChangeDetectionStrategy,
  Component,
  computed,
  contentChild,
  Directive,
  inject,
  InjectionToken,
  input,
  signal,
} from "@angular/core"
import { toast } from "@spartan-ng/brain/sonner"
import { PageHero } from "@shared/ui/bc-gov/page-hero"
import { Button } from "@shared/ui/primitives/button"
import { LucideIcon } from "@shared/ui/primitives/lucide-icon"
import { Progress } from "@shared/ui/primitives/progress"
import { cn } from "@shared/util/utils"
import { FieldHelp } from "./field-help"
import { injectApplicationContext } from "./provider"

@Component({
  selector: "step-indicator",
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    @if (variant() === "vertical") {
      <div [attr.class]="cn('space-y-4', userClass())">
        @for (step of ctx.steps(); track step.id; let index = $index) {
          @let isCompleted = ctx.completedSteps().includes(index);
          @let isCurrent = index === ctx.currentStep();
          @let isClickable = index !== ctx.currentStep();
          <button
            type="button"
            [disabled]="!isClickable"
            (click)="ctx.attemptGoToStep(index)"
            [attr.aria-current]="isCurrent ? 'step' : null"
            [attr.class]="
              cn(
                'flex w-full items-start gap-3 text-left rounded-md -mx-1 px-1 py-0.5',
                isClickable && 'cursor-pointer hover:bg-muted/60 transition-colors'
              )
            "
          >
            <div
              [attr.class]="
                cn(
                  'flex items-center justify-center w-8 h-8 rounded-full border-2 shrink-0 transition-colors',
                  isCompleted
                    ? 'bg-[#2E8540] border-[#2E8540] text-white'
                    : isCurrent
                    ? 'bg-[#013366] border-[#013366] text-white'
                    : 'bg-white border-gray-300 text-gray-500'
                )
              "
            >
              @if (isCompleted) {
                <svg lucide="Check" class="h-4 w-4"></svg>
              } @else {
                <span class="text-sm font-medium">{{ index + 1 }}</span>
              }
            </div>
            <div class="pt-1">
              <div [attr.class]="cn('font-medium text-sm', isCurrent ? 'text-[#013366]' : 'text-muted-foreground')">{{ step.title }}</div>
              @if (step.description) {
                <div class="text-xs text-muted-foreground mt-0.5">{{ step.description }}</div>
              }
            </div>
          </button>
        }
      </div>
    } @else {
      <div [attr.class]="cn('w-full', userClass())">
        <div class="flex items-center justify-between">
          @for (step of ctx.steps(); track step.id; let index = $index) {
            @let isCompleted = ctx.completedSteps().includes(index);
            @let isCurrent = index === ctx.currentStep();
            @let isClickable = index !== ctx.currentStep();
            <div class="flex items-center flex-1 last:flex-none">
              <button
                type="button"
                [disabled]="!isClickable"
                (click)="ctx.attemptGoToStep(index)"
                [attr.aria-current]="isCurrent ? 'step' : null"
                [attr.class]="cn('flex flex-col items-center', isClickable && 'cursor-pointer')"
              >
                <div
                  [attr.class]="
                    cn(
                      'flex items-center justify-center w-10 h-10 rounded-full border-2 transition-colors',
                      isCompleted
                        ? 'bg-[#2E8540] border-[#2E8540] text-white'
                        : isCurrent
                        ? 'bg-[#013366] border-[#013366] text-white'
                        : 'bg-white border-gray-300 text-gray-500'
                    )
                  "
                >
                  @if (isCompleted) {
                    <svg lucide="Check" class="h-5 w-5"></svg>
                  } @else {
                    <span class="text-sm font-semibold">{{ index + 1 }}</span>
                  }
                </div>
                <div
                  [attr.class]="
                    cn('mt-2 text-xs font-medium text-center max-w-[100px]', isCurrent ? 'text-[#013366]' : 'text-muted-foreground')
                  "
                >{{ step.title }}</div>
              </button>
              @if (index < ctx.steps().length - 1) {
                <div [attr.class]="cn('flex-1 h-0.5 mx-2 mt-[-1.5rem]', isCompleted ? 'bg-[#2E8540]' : 'bg-gray-200')"></div>
              }
            </div>
          }
        </div>
      </div>
    }
  `,
})
export class StepIndicator {
  readonly userClass = input<string | undefined>(undefined, { alias: "class" })
  readonly variant = input<"horizontal" | "vertical">("horizontal")
  protected readonly ctx = injectApplicationContext()
  protected readonly cn = cn
}

/** Panel after a failed Next/Submit listing each invalid field; clicking a row scrolls to and focuses it. */
@Component({
  selector: "validation-summary",
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    @if (ctx.submitAttempted() && ctx.invalidFields().length > 0) {
      <div role="alert" aria-live="assertive" class="mb-6 rounded-lg border border-[#CE3E39] bg-[#F4E1E2] p-4">
        <div class="flex items-start gap-3">
          <svg lucide="AlertCircle" class="mt-0.5 h-5 w-5 shrink-0 text-[#CE3E39]" aria-hidden="true"></svg>
          <div class="flex-1">
            <h3 class="text-sm font-semibold text-[#8B1E1E]">
              {{
                ctx.invalidFields().length === 1
                  ? "1 field needs your attention before you can continue"
                  : ctx.invalidFields().length + " fields need your attention before you can continue"
              }}
            </h3>
            <ul class="mt-2 space-y-1">
              @for (f of ctx.invalidFields(); track f.name) {
                <li>
                  <button
                    type="button"
                    (click)="focusField(f.name)"
                    class="text-left text-sm font-medium text-[#8B1E1E] underline underline-offset-2 hover:text-[#CE3E39] focus:outline-none focus-visible:ring-2 focus-visible:ring-[#CE3E39] rounded"
                  >
                    {{ f.label }}
                  </button>
                  <span class="text-xs text-[#8B1E1E]/80"> — {{ f.error }}</span>
                </li>
              }
            </ul>
          </div>
        </div>
      </div>
    }
  `,
})
export class ValidationSummary {
  protected readonly ctx = injectApplicationContext()

  protected focusField(name: string): void {
    if (typeof document === "undefined") return
    const shell = document.getElementById(`field-${name}`)
    if (!shell) return
    shell.scrollIntoView({ behavior: "smooth", block: "center" })
    const control = shell.querySelector<HTMLElement>(
      "input, textarea, select, [role='radiogroup'] button, [role='checkbox'], button",
    )
    // Delay focus so it lands after the smooth scroll starts.
    window.setTimeout(() => control?.focus({ preventScroll: true }), 150)
  }
}

/** Marks the optional right-hand sidebar content of <application-layout>. */
@Directive({ selector: "[appSidebar]" })
export class ApplicationSidebar {}
/** Optional right-aligned control above the step indicator, e.g. the Guided/Full-form toggle. */
@Directive({ selector: "[appHeaderAction]" })
export class ApplicationHeaderAction {}

@Component({
  selector: "application-layout",
  imports: [PageHero, Progress, StepIndicator, ValidationSummary],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <div class="min-h-screen bg-muted/30">
      <div class="sticky top-0 z-10 bg-white border-b">
        <div uiProgress [value]="progress()" class="h-1 rounded-none"></div>
      </div>

      <page-hero [title]="title()" [description]="applicationName()" />

      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        @if (headerActionSlot()) {
          <div class="mb-6 flex justify-end"><ng-content select="[appHeaderAction]" /></div>
        }

        <div class="mb-8">
          <step-indicator />
        </div>

        <div class="flex gap-8">
          <div class="flex-1">
            <validation-summary />
            <ng-content />
          </div>
          @if (sidebarSlot()) {
            <aside class="hidden lg:block w-80 shrink-0"><ng-content select="[appSidebar]" /></aside>
          }
        </div>
      </div>
    </div>
  `,
})
export class ApplicationLayout {
  readonly title = input.required<string>()
  readonly applicationName = input<string | undefined>(undefined)
  protected readonly sidebarSlot = contentChild(ApplicationSidebar)
  protected readonly headerActionSlot = contentChild(ApplicationHeaderAction)
  private readonly ctx = injectApplicationContext()
  protected readonly progress = computed(() => ((this.ctx.currentStep() + 1) / this.ctx.steps().length) * 100)
}

@Component({
  selector: "application-navigation",
  imports: [Button, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <div class="flex items-center justify-between pt-6 mt-6 border-t">
      <div>
        @if (!ctx.isFirstStep()) {
          <button uiButton variant="outline" (click)="ctx.prevStep()" [disabled]="isSubmitting()">Back</button>
        }
      </div>
      <div class="flex items-center gap-3">
        @if (showSave() && (onSave() || ctx.hasDraftSupport())) {
          <button uiButton variant="outline" (click)="handleSave()" [disabled]="isSubmitting()">{{ saveLabel() }}</button>
        }
        @if (ctx.isLastStep()) {
          <button uiButton (click)="handleSubmit()" [disabled]="isSubmitting()" class="bg-[#013366] hover:bg-[#38598A]">
            @if (isSubmitting()) {
              <svg lucide="Loader2" class="mr-2 h-4 w-4 animate-spin"></svg>
            }
            {{ isSubmitting() ? submittingLabel() : submitLabel() }}
          </button>
        } @else {
          <button uiButton (click)="ctx.nextStep()" class="bg-[#013366] hover:bg-[#38598A]">Next</button>
        }
      </div>
    </div>
  `,
})
export class ApplicationNavigation {
  /** Callback props stay function inputs: `onSubmit` is awaited, and `onSave` being set decides whether the save button shows. */
  readonly onSave = input<(() => void) | undefined>(undefined)
  readonly onSubmit = input<(() => void | Promise<void>) | undefined>(undefined)
  readonly saveLabel = input("Save & Continue Later")
  readonly submitLabel = input("Submit Application")
  readonly submittingLabel = input("Submitting & assessing…")
  readonly showSave = input(true, { transform: booleanAttribute })

  protected readonly ctx = injectApplicationContext()
  protected readonly isSubmitting = signal(false)

  protected async handleSubmit(): Promise<void> {
    if (this.isSubmitting()) return
    if (!this.ctx.validateVisible()) {
      toast.error("Some required fields need attention", {
        description: "See the highlighted list at the top of the step to jump to each one.",
      })
      return
    }
    try {
      this.isSubmitting.set(true)
      await this.onSubmit()?.()
    } finally {
      this.isSubmitting.set(false)
    }
  }

  protected handleSave(): void {
    if (this.ctx.hasDraftSupport()) this.ctx.saveProgress()
    this.onSave()?.()
  }
}

@Component({
  selector: "div[applicationSection]",
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { "[attr.class]": "hostClass()" },
  template: `
    <div class="bg-[#38598A] text-white px-6 py-3 rounded-t-lg">
      <h2 class="font-semibold">{{ title() }}</h2>
      @if (description()) {
        <p class="text-white/80 text-sm mt-1">{{ description() }}</p>
      }
    </div>
    <div class="p-6"><ng-content /></div>
  `,
})
export class ApplicationSection {
  readonly title = input.required<string>()
  readonly description = input<string | undefined>(undefined)
  readonly userClass = input<string | undefined>(undefined, { alias: "class" })
  protected readonly hostClass = computed(() => cn("bg-white rounded-lg border", this.userClass()))
}

/** Lets <ConfigField> pass a field's `help` to its <FormField> without threading a prop; an explicit `help` on FormField still wins. */
export const FieldHelpContext = new InjectionToken<() => string | undefined>("FieldHelpContext")

@Component({
  selector: "div[formField]",
  imports: [FieldHelp, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { "[attr.class]": "hostClass()" },
  template: `
    <!-- data-field-label lets Guided Mode hide this label visually (shown as a heading instead) while keeping it in the accessibility tree; the help trigger sits outside the label so clicking it does not focus the control. -->
    <div class="flex items-center gap-1.5">
      <label data-field-label [attr.for]="htmlFor()" class="block text-sm font-medium text-foreground">{{ label() }}@if (required()) {<span class="text-destructive ml-1">*</span>}</label>
      @if (help()) {
        <span data-field-help>
          <field-help [label]="label()" [help]="help()!" />
        </span>
      }
    </div>
    <ng-content />
    @if (hint() && !error()) {
      <p class="text-xs text-muted-foreground">{{ hint() }}</p>
    }
    @if (warning() && !error()) {
      <p role="status" class="flex items-start gap-1.5 rounded-md bg-amber-50 px-2.5 py-2 text-xs text-amber-900">
        <svg lucide="AlertCircle" class="mt-0.5 h-3.5 w-3.5 shrink-0" aria-hidden="true"></svg>
        {{ warning() }}
      </p>
    }
    @if (error()) {
      <p class="text-xs text-destructive flex items-center gap-1">
        <svg lucide="AlertCircle" class="h-3 w-3"></svg>
        {{ error() }}
      </p>
    }
  `,
})
export class FormField {
  readonly label = input.required<string>()
  readonly required = input(false, { transform: booleanAttribute })
  readonly error = input<string | undefined>(undefined)
  /** Non-blocking advice shown beneath the hint; never gates navigation. */
  readonly warning = input<string | undefined>(undefined)
  readonly hint = input<string | undefined>(undefined)
  readonly userClass = input<string | undefined>(undefined, { alias: "class" })
  readonly htmlFor = input<string | undefined>(undefined)
  /** Plain-language explanation of the label term, shown as a "?" popover with an assistant hand-off. */
  readonly helpProp = input<string | undefined>(undefined, { alias: "help" })

  private readonly contextHelp = inject(FieldHelpContext, { optional: true })
  protected readonly help = computed(() => this.helpProp() ?? this.contextHelp?.())
  protected readonly hostClass = computed(() => cn("space-y-2", this.userClass()))
}

const INFO_BOX_VARIANTS = {
  info: "bg-[#F7F9FC] border-[#255A90] text-[#013366]",
  warning: "bg-[#FEF1D8] border-[#F5A623] text-[#8B5A00]",
  success: "bg-[#F6FFF8] border-[#2E8540] text-[#1E5631]",
  error: "bg-[#F4E1E2] border-[#CE3E39] text-[#8B1E1E]",
}

@Component({
  selector: "div[infoBox]",
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { "[attr.class]": "hostClass()" },
  template: `
    @if (title()) {
      <div class="font-semibold mb-1">{{ title() }}</div>
    }
    <div class="text-sm"><ng-content /></div>
  `,
})
export class InfoBox {
  readonly variant = input<"info" | "warning" | "success" | "error" | undefined>("info")
  readonly title = input<string | undefined>(undefined)
  readonly userClass = input<string | undefined>(undefined, { alias: "class" })
  protected readonly hostClass = computed(() =>
    cn("border-l-4 p-4 rounded-r", INFO_BOX_VARIANTS[this.variant() ?? "info"], this.userClass()),
  )
}
