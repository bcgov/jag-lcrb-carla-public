/** The planner's AI advisor chat. `useChat` becomes `Chat` from @ai-sdk/angular; the plan summary is read through a getter so the transport always sends fresh context without rebuilding the chat. */
import { afterRenderEffect, ChangeDetectionStrategy, Component, computed, type ElementRef, inject, input, signal, viewChild } from '@angular/core';
import { Chat } from '@ai-sdk/angular';
import { DefaultChatTransport, type UIMessage } from 'ai';
import { Button } from '@shared/ui/primitives/button';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { Textarea } from '@shared/ui/primitives/textarea';
import { shouldAutoResendToolResults } from '@domain/ai/auto-continue';
import { AI_REPOSITORY } from '@data-access/repositories/ai/ai.repository';

interface TextPart {
  type: 'text';
  text: string;
}

const DEFAULT_PROMPTS = [
  'Explain my flags in plain language',
  'What should I do first?',
  'How do I get local government approval?',
];

@Component({
  selector: 'advisor-message',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (isUser()) {
      <div class="flex justify-end">
        <div class="max-w-[85%] rounded-2xl rounded-br-sm bg-primary px-3 py-2 text-sm text-primary-foreground">{{ text() }}</div>
      </div>
    } @else if (text().trim()) {
      <div class="flex gap-2">
        <span class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-primary/10 text-primary">
          <svg lucide="Bot" class="h-4 w-4"></svg>
        </span>
        <div class="max-w-[90%] whitespace-pre-wrap rounded-2xl rounded-tl-sm bg-muted px-3 py-2 text-sm leading-relaxed text-foreground">{{ text() }}</div>
      </div>
    }
  `,
})
export class AdvisorMessage {
  readonly role = input.required<string>();
  readonly parts = input.required<{ type: string; [key: string]: unknown }[]>();
  protected readonly isUser = computed(() => this.role() === 'user');
  protected readonly text = computed(() =>
    this.parts()
      .filter((p) => p.type === 'text')
      .map((p) => (p as unknown as TextPart).text)
      .join(''),
  );
}

/** Compact chat panel that explains the applicant's plan; streams from /api/planner-advisor with the plan summary as context. */
@Component({
  selector: 'planner-advisor-panel',
  imports: [AdvisorMessage, Button, LucideIcon, Textarea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex h-full flex-col">
      <div class="flex items-center gap-2 border-b border-border px-4 py-3">
        <span class="flex h-8 w-8 items-center justify-center rounded-full bg-primary/10 text-primary">
          <svg lucide="Bot" class="h-4 w-4"></svg>
        </span>
        <div>
          <p class="text-sm font-semibold text-foreground">Planner advisor</p>
          <p class="text-[11px] text-muted-foreground">Ask about your plan</p>
        </div>
      </div>

      <div #scroll class="flex-1 overflow-y-auto px-4 py-4">
        @if (!started()) {
          <div class="flex flex-col gap-3">
            <p class="text-sm leading-relaxed text-muted-foreground">
              I can explain your feasibility results, flags, fees, and next steps for opening your establishment. Ask me
              anything.
            </p>
            <div class="flex flex-col gap-2">
              @for (p of defaultPrompts; track p) {
                <button
                  type="button"
                  (click)="submit(p)"
                  class="rounded-lg border border-border bg-background px-3 py-2 text-left text-sm transition-colors hover:border-primary hover:bg-muted/50"
                >
                  {{ p }}
                </button>
              }
            </div>
          </div>
        } @else {
          <div class="flex flex-col gap-4">
            @for (m of messages(); track m.id) {
              <advisor-message [role]="m.role" [parts]="$any(m.parts)" />
            }
            @if (busy() || retry.reconnecting()) {
              <div class="flex items-center gap-2 text-sm text-muted-foreground">
                <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
                {{ retry.reconnecting() ? 'Reconnecting…' : 'Thinking…' }}
              </div>
            }
            @if (retry.showError()) {
              <div class="rounded-lg border border-amber-300 bg-amber-50 px-3 py-2 text-sm text-amber-900">
                <p>{{ retry.message() }}</p>
                @if (!retry.isQuota()) {
                  <button
                    type="button"
                    (click)="retry.retry()"
                    [disabled]="retry.waitSeconds() > 0"
                    class="mt-1 font-medium underline underline-offset-2 disabled:no-underline disabled:opacity-70"
                  >
                    {{ retry.waitSeconds() > 0 ? 'Try again in ' + retry.waitSeconds() + 's' : 'Try again' }}
                  </button>
                }
              </div>
            }
          </div>
        }
      </div>

      @if (quickReplies().length > 0) {
        <div class="flex flex-wrap gap-2 px-4 pb-2">
          @for (r of quickReplies(); track r + '-' + $index) {
            <button
              type="button"
              (click)="submit(r)"
              class="rounded-full border border-primary/30 bg-background px-3 py-1.5 text-xs font-medium text-primary transition-colors hover:bg-primary hover:text-primary-foreground"
            >
              {{ r }}
            </button>
          }
        </div>
      }

      <div class="border-t border-border p-3">
        <div class="flex items-end gap-2">
          <textarea
            uiTextarea
            [value]="input()"
            (input)="input.set($any($event.target).value)"
            (keydown)="onKeyDown($event)"
            placeholder="Ask about your plan…"
            rows="1"
            class="max-h-28 min-h-[40px] resize-none"
            aria-label="Ask the planner advisor"
          ></textarea>
          <button
            uiButton
            (click)="submit(input())"
            [disabled]="busy() || !input().trim()"
            size="icon"
            class="h-10 w-10 shrink-0"
            aria-label="Send"
          >
            <svg lucide="Send" class="h-4 w-4"></svg>
          </button>
        </div>
        <div class="mt-2 flex items-center justify-between">
          <p class="text-[11px] text-muted-foreground">Prototype AI · verify against official policy</p>
          @if (started()) {
            <button
              type="button"
              (click)="chat.messages = []"
              class="text-[11px] font-medium text-muted-foreground hover:text-primary"
            >
              Clear
            </button>
          }
        </div>
      </div>
    </div>
  `,
})
export class PlannerAdvisorPanel {
  private readonly aiRepository = inject(AI_REPOSITORY);
  readonly planSummary = input.required<string>();
  protected readonly defaultPrompts = DEFAULT_PROMPTS;
  protected readonly input = signal('');
  private readonly scroll = viewChild<ElementRef<HTMLDivElement>>('scroll');

  // suggestReplies resolves on the server so a suggestion call can't end the turn before the advisor has written an answer; the buttons render from the tool input.
  protected readonly chat: Chat<UIMessage> = new Chat<UIMessage>({
    transport: new DefaultChatTransport({
      api: '/api/planner-advisor',
      prepareSendMessagesRequest: ({ messages }) => ({
        body: { messages, context: { planSummary: this.planSummary() } },
      }),
    }),
    sendAutomaticallyWhen: shouldAutoResendToolResults,
  });

  protected readonly messages = computed(() => this.chat.messages);
  private readonly status = computed(() => this.chat.status);
  private readonly error = computed(() => this.chat.error);
  protected readonly busy = computed(() => this.status() === 'submitted' || this.status() === 'streaming');

  protected readonly retry = this.aiRepository.autoRetry({
    status: this.status,
    error: this.error,
    busy: this.busy,
    clearError: () => this.chat.clearError(),
    regenerate: () => void this.chat.regenerate(),
  });

  protected readonly started = computed(() => this.messages().length > 0);

  protected readonly quickReplies = computed<string[]>(() => {
    const messages = this.messages();
    if (this.busy() || messages.length === 0) return [];
    const last = messages[messages.length - 1];
    if (last.role !== 'assistant') return [];
    const part = last.parts.find((p) => (p as { type?: string }).type === 'tool-suggestReplies') as
      | { input?: { replies?: unknown } }
      | undefined;
    const replies = part?.input?.replies;
    if (!Array.isArray(replies)) return [];
    return replies.filter((r): r is string => typeof r === 'string' && r.trim().length > 0).slice(0, 4);
  });

  constructor() {
    afterRenderEffect(() => {
      this.messages();
      this.status();
      const el = this.scroll()?.nativeElement;
      if (el) el.scrollTop = el.scrollHeight;
    });
  }

  protected submit(text: string): void {
    const trimmed = text.trim();
    if (!trimmed || this.busy()) return;
    void this.chat.sendMessage({ text: trimmed });
    this.input.set('');
  }

  protected onKeyDown(e: KeyboardEvent): void {
    if (e.key === 'Enter' && !e.shiftKey && !e.isComposing && e.keyCode !== 229) {
      e.preventDefault();
      this.submit(this.input());
    }
  }
}
