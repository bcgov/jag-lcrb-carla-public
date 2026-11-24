/** Help "?" beside a field label: a popover with `FieldConfig.help` and a hand-off that opens the assistant with the question typed in. */
import { ChangeDetectionStrategy, Component, computed, input } from "@angular/core"
import { injectAssistant } from "@core/assistant/assistant-provider"
import { Button } from "@shared/ui/primitives/button"
import { LucideIcon } from "@shared/ui/primitives/lucide-icon"
import { POPOVER } from "@shared/ui/primitives/popover"

@Component({
  selector: "field-help",
  imports: [Button, LucideIcon, POPOVER],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <ui-popover align="start">
      <button
        uiPopoverTrigger
        type="button"
        class="inline-flex h-5 w-5 items-center justify-center rounded-full text-muted-foreground transition-colors hover:bg-muted hover:text-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
        [attr.aria-label]="ariaLabel()"
      >
        <svg lucide="CircleHelp" class="h-4 w-4" aria-hidden="true"></svg>
      </button>
      <div uiPopoverContent *uiPopoverPortal class="w-80 text-sm leading-relaxed">
        <p class="mb-1 font-semibold">{{ label() }}</p>
        <p class="text-muted-foreground">{{ help() }}</p>
        <button
          uiButton
          type="button"
          variant="link"
          size="sm"
          class="mt-2 h-auto p-0 text-[#38598A]"
          (click)="ask()"
        >
          <svg lucide="Sparkles" class="mr-1.5 h-3.5 w-3.5" aria-hidden="true"></svg>
          Ask the assistant about this
        </button>
      </div>
    </ui-popover>
  `,
})
export class FieldHelp {
  readonly label = input.required<string>()
  readonly help = input.required<string>()
  private readonly assistant = injectAssistant()
  protected readonly ariaLabel = computed(() => `What does "${this.label()}" mean?`)

  protected ask(): void {
    this.assistant.askAssistant(
      `Can you explain "${this.label()}" in the context of my application, and what I should enter here?`,
    )
  }
}
