/** Switches between the classic form and Guided Mode; both read the same provider state, so no answer is lost. */
import { ChangeDetectionStrategy, Component, input, output } from "@angular/core"
import { LucideIcon } from "@shared/ui/primitives/lucide-icon"
import type { IconName } from "@shared/ui/primitives/icons"
import { cn } from "@shared/util/utils"
import type { ApplicationMode } from "./application-mode"

const OPTIONS: { value: ApplicationMode; label: string; icon: IconName; hint: string }[] = [
  { value: "form", label: "Full form", icon: "LayoutList", hint: "See every question on a step at once" },
  { value: "guided", label: "Guided", icon: "Sparkles", hint: "Answer one question at a time" },
]

@Component({
  selector: "application-mode-toggle",
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <div
      role="radiogroup"
      aria-label="How would you like to fill in this application?"
      [attr.class]="cn('inline-flex items-center gap-1 rounded-full border bg-card p-1 shadow-sm', userClass())"
    >
      @for (opt of options; track opt.value) {
        @let active = mode() === opt.value;
        <button
          type="button"
          role="radio"
          [attr.aria-checked]="active"
          [attr.title]="opt.hint"
          (click)="modeChange.emit(opt.value)"
          [attr.class]="
            cn(
              'inline-flex items-center gap-2 rounded-full px-3 py-1.5 text-sm font-medium transition-colors',
              'focus:outline-none focus-visible:ring-2 focus-visible:ring-[#013366] focus-visible:ring-offset-1',
              active
                ? 'bg-[#013366] text-white shadow-sm'
                : 'text-muted-foreground hover:bg-muted hover:text-foreground'
            )
          "
        >
          <svg [lucide]="opt.icon" class="h-4 w-4" aria-hidden="true"></svg>{{ opt.label }}
        </button>
      }
    </div>
  `,
})
export class ApplicationModeToggle {
  readonly mode = input.required<ApplicationMode>()
  /** `onChange` in the prototype. */
  readonly modeChange = output<ApplicationMode>()
  readonly userClass = input<string | undefined>(undefined, { alias: "class" })
  protected readonly options = OPTIONS
  protected readonly cn = cn
}
