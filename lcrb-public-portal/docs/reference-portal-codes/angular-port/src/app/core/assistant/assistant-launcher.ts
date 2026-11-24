/** Floating "Ask AI" button plus the side panel, shown once a session exists; the panel loads lazily to keep the AI SDK out of the initial bundle. */
import { afterNextRender, ChangeDetectionStrategy, Component, effect, Injector, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router } from '@angular/router';
import { filter, first, map } from 'rxjs';
import { injectAssistant } from './assistant-provider';
import { injectSession } from '@core/session/session-provider';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { AssistantPanel } from './assistant-panel';

@Component({
  selector: 'assistant-launcher',
  imports: [AssistantPanel, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <!-- Only show once a session exists (i.e. not on the login screen). -->
    @if (!sessionProvider.loading() && sessionProvider.session()) {
      @if (!assistant.open()) {
        <button
          type="button"
          (click)="assistant.toggle()"
          class="fixed bottom-5 right-5 z-40 flex items-center gap-2 rounded-full bg-[#013366] px-4 py-3 text-white shadow-lg transition-transform hover:scale-105 hover:bg-[#38598A] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#013366] focus-visible:ring-offset-2"
          aria-label="Open the licensing assistant"
        >
          <svg lucide="Bot" class="h-5 w-5"></svg>
          <span class="text-sm font-medium">Ask AI</span>
        </button>
      }
      <!-- The panel (and the AI SDK) loads in a lazy chunk once the browser is idle, or at once if the user opens it first. -->
      @defer (on idle; when assistant.open()) {
        <assistant-panel />
      }
    }
  `,
})
export class AssistantLauncher {
  protected readonly assistant = injectAssistant();
  protected readonly sessionProvider = injectSession();

  private readonly injector = inject(Injector);
  private readonly navigated = toSignal(
    inject(Router).events.pipe(
      filter((e) => e instanceof NavigationEnd),
      first(),
      map(() => true),
    ),
    { initialValue: false },
  );

  constructor() {
    // The prototype draws the chat id after the first effects, including the first page view; draw it once a session exists and the first navigation has ended.
    effect(() => {
      if (!this.sessionProvider.loading() && this.sessionProvider.session() && this.navigated()) {
        afterNextRender(() => this.assistant.ensureChatId(), { injector: this.injector });
      }
    });
  }
}
