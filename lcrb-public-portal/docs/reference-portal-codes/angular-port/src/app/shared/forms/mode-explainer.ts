/** First-visit card explaining Guided versus Full form, with a switch; once dismissed it stays gone. */
import { ChangeDetectionStrategy, Component, computed, input, output } from "@angular/core"
import { Button } from "@shared/ui/primitives/button"
import { DismissibleNotice } from "@shared/ui/primitives/dismissible-notice"
import { LucideIcon } from "@shared/ui/primitives/lucide-icon"
import type { ApplicationMode } from "./application-mode"

@Component({
  selector: "mode-explainer",
  imports: [Button, DismissibleNotice, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <ui-dismissible-notice noticeId="application-mode-v1" title="Two ways to fill this in">
      <svg icon #icon lucide="Sparkles" class="h-5 w-5" aria-hidden="true"></svg>
      <ng-container ngProjectAs="[actions]" #actions>
        <button uiButton type="button" size="sm" variant="outline" (click)="modeChange.emit(other())">
          @if (other() === "guided") {
            <svg lucide="Sparkles" class="mr-2 h-4 w-4" aria-hidden="true"></svg>
            Try Guided Mode
          } @else {
            <svg lucide="LayoutList" class="mr-2 h-4 w-4" aria-hidden="true"></svg>
            Switch to the full form
          }
        </button>
        <span class="text-xs text-muted-foreground">
          You can switch any time with the toggle at the top of the page. Your answers are kept either way.
        </span>
      </ng-container>
      <ul class="flex flex-col gap-1.5">
        <li class="flex gap-2">
          <svg lucide="LayoutList" class="mt-0.5 h-4 w-4 shrink-0 text-[#013366]" aria-hidden="true"></svg>
          <span>
            <strong>Full form</strong> shows every question on a step at once — good if you know the process and want
            to move fast.
          </span>
        </li>
        <li class="flex gap-2">
          <svg lucide="Sparkles" class="mt-0.5 h-4 w-4 shrink-0 text-[#013366]" aria-hidden="true"></svg>
          <span>
            <strong>Guided</strong> asks one question at a time with extra explanation, and lets you answer in your own
            words where that helps — good if this is your first application.
          </span>
        </li>
      </ul>
    </ui-dismissible-notice>
  `,
})
export class ModeExplainer {
  readonly mode = input.required<ApplicationMode>()
  /** `onChange` in the prototype. */
  readonly modeChange = output<ApplicationMode>()
  protected readonly other = computed<ApplicationMode>(() => (this.mode() === "guided" ? "form" : "guided"))
}
