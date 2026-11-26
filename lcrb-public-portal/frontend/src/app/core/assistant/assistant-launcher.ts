import { ChangeDetectionStrategy, Component } from '@angular/core';
import { injectSession } from '@core/session/session-provider';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';

// Placeholder: the assistant panel is not wired yet, so the button does nothing.
@Component({
  selector: 'assistant-launcher',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (!sessionProvider.loading() && sessionProvider.session()) {
      <button
        type="button"
        class="fixed bottom-5 right-5 z-40 flex items-center gap-2 rounded-full bg-[#013366] px-4 py-3 text-white shadow-lg transition-transform hover:scale-105 hover:bg-[#38598A] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#013366] focus-visible:ring-offset-2"
        aria-label="Open the licensing assistant"
      >
        <svg lucide="Bot" class="h-5 w-5"></svg>
        <span class="text-sm font-medium">Ask AI</span>
      </button>
    }
  `,
})
export class AssistantLauncher {
  protected readonly sessionProvider = injectSession();
}
