/** Assistant side panel; client tools (navigateTo, highlightField, setField, goToStep) are answered in `onToolCall`, the rest resolve on the server. */
import {
  afterRenderEffect,
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  type ElementRef,
  inject,
  input,
  signal,
  viewChild,
} from '@angular/core';
import { Router } from '@angular/router';
import { Chat } from '@ai-sdk/angular';
import { DefaultChatTransport, type UIMessage } from 'ai';
import { injectAssistant } from './assistant-provider';
import { injectSession } from '@core/session/session-provider';
import { Button } from '@shared/ui/primitives/button';
import { type IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { Textarea } from '@shared/ui/primitives/textarea';
import { injectPathname } from '@core/navigation/pathname';
import { shouldAutoResendToolResults } from '@domain/ai/auto-continue';
import { AI_REPOSITORY } from '@data-access/repositories/ai/ai.repository';
import { isExplicitNavigationRequest, isPathAllowedForRole } from '@domain/ai/navigation-intent';
import type { UserRole } from '@domain/roles';

/** Internal routes go through the router; tel:, mailto: and absolute http(s) links go through the browser. */
function goToHref(router: Router, href: string) {
  const target = href.trim();
  if (/^(tel:|mailto:|https?:\/\/)/i.test(target)) {
    window.location.href = target;
    return;
  }
  void router.navigateByUrl(target);
}

function roleForAi(role: UserRole | undefined): string {
  if (role === 'licensee') return 'licensee';
  if (role === 'applicant') return 'applicant';
  if (role === 'police') return 'police';
  if (role === 'local-government') return 'local-government';
  if (role === 'lcrb') return 'lcrb-staff';
  return 'public';
}

/** Role-specific "How can I help?" introduction shown in the empty state. */
const INTRO_BY_ROLE: Record<string, { title: string; body: string }> = {
  licensee: {
    title: 'How can I help?',
    body: 'I can help you manage your licences, start and track applications, plan a new establishment, and answer questions about liquor and cannabis licensing. Ask me things like “How do I renew my licence?” and I can take you there.',
  },
  applicant: {
    title: 'How can I help?',
    body: 'I can guide you through applying for a Special Event Permit step by step — I can even fill in fields for you — and answer questions like “How do I apply for an SEP?” or “Who needs Serving It Right?”',
  },
  police: {
    title: 'How can I help, officer?',
    body: 'I can take you to your review queue, open the events map, and walk you through reviewing a Special Event Permit — assigning it, adding conditions, and approving or denying. Ask me “How do I add conditions to a permit?”',
  },
  'local-government': {
    title: 'How can I help?',
    body: 'I can show you what’s due for comment, open the applications referred to your government, and walk you through submitting a recommendation or requesting a document. Ask me “How do I comment on an application?”',
  },
  'lcrb-staff': {
    title: 'How can I help?',
    body: 'I can take you to the intake dashboard, case triage, and analytics, and help you find cases that need human review. Ask me “How do I triage a case?” or “Where are my priority cases?”',
  },
  public: {
    title: 'How can I help?',
    body: 'I can help you navigate the portal, find the right licence or permit, explore the licensing map, and answer questions about B.C. liquor and cannabis licensing.',
  },
};

const SUGGESTIONS_BY_ROLE: Record<string, string[]> = {
  licensee: [
    'Check my agent application',
    'When does my liquor licence expire?',
    'What have I submitted recently?',
    'Help me apply for a Food Primary licence',
  ],
  applicant: [
    'How do I apply for a Special Event Permit?',
    'Help me fill out this application',
    'What is Serving It Right?',
    'Can I save and finish later?',
  ],
  police: ['Show me the review queue', 'Open the events map', 'How do I add conditions to a permit?'],
  'local-government': [
    "What's due for comment this week?",
    'Open my review queue',
    'How do I comment on an application?',
  ],
  'lcrb-staff': ['How do I triage a case?', 'Show me priority cases', 'Open intake analytics'],
  public: ['What is a Special Event Permit?', 'How do I get a liquor licence?'],
};

type Part = { type: string; [key: string]: unknown };

const TOOL_CHIPS: Record<string, { icon: IconName; label: (input?: Record<string, unknown>) => string }> = {
  'tool-setField': { icon: 'Pencil', label: (i) => `Filled "${i?.['name']}"` },
  'tool-highlightField': { icon: 'Sparkles', label: (i) => `Highlighted "${i?.['name']}"` },
  'tool-navigateTo': { icon: 'MapPin', label: (i) => `Opened ${i?.['label'] ?? i?.['href'] ?? 'page'}` },
  'tool-goToStep': { icon: 'ArrowRight', label: (i) => `Went to step ${Number(i?.['step'] ?? 0) + 1}` },
};

/** Tool call chip. */
@Component({
  selector: 'assistant-tool-chip',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (chip(); as c) {
      <div
        class="flex w-fit items-center gap-1.5 rounded-full border border-[#2E8540]/30 bg-[#2E8540]/10 px-2.5 py-1 text-xs font-medium text-[#1E5631]"
      >
        <svg [lucide]="c.icon" class="h-3 w-3"></svg>
        {{ c.label }}
      </div>
    }
  `,
})
export class ToolChip {
  readonly type = input.required<string>();
  readonly input = input<Record<string, unknown>>();
  protected readonly chip = computed(() => {
    const c = TOOL_CHIPS[this.type()];
    return c ? { icon: c.icon, label: c.label(this.input()) } : null;
  });
}

/** Chat message bubble. */
@Component({
  selector: 'assistant-message-bubble',
  imports: [LucideIcon, ToolChip],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (isUser()) {
      <div class="flex justify-end">
        <div class="max-w-[85%] rounded-2xl rounded-br-sm bg-[#013366] px-3 py-2 text-sm text-white">{{ text() }}</div>
      </div>
    } @else if (visible()) {
      <div class="flex gap-2">
        <span
          class="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-[#013366]/10 text-[#013366]"
        >
          <svg lucide="Bot" class="h-4 w-4"></svg>
        </span>
        <div class="flex max-w-[85%] flex-col gap-2">
          @if (text()) {
            <div class="whitespace-pre-wrap rounded-2xl rounded-tl-sm bg-muted px-3 py-2 text-sm leading-relaxed text-foreground">{{ text() }}</div>
          }
          @for (a of actions(); track $index) {
            <assistant-tool-chip [type]="a.type" [input]="$any(a['input'])" />
          }
        </div>
      </div>
    }
  `,
})
export class MessageBubble {
  readonly role = input.required<string>();
  readonly parts = input.required<Part[]>();

  protected readonly isUser = computed(() => this.role() === 'user');
  protected readonly text = computed(() =>
    this.parts()
      .filter((p) => p.type === 'text')
      .map((p) => (p as unknown as { text: string }).text)
      .join(''),
  );
  // Collect tool actions for a compact activity log.
  protected readonly actions = computed(() =>
    this.parts().filter((p) => typeof p.type === 'string' && p.type.startsWith('tool-')),
  );
  // An assistant turn with neither text nor a visible tool action renders nothing.
  protected readonly visible = computed(
    () =>
      !!this.text().trim() ||
      this.actions().some((a) =>
        ['tool-setField', 'tool-highlightField', 'tool-navigateTo', 'tool-goToStep'].includes(a.type),
      ),
  );
}

@Component({
  selector: 'assistant-panel',
  imports: [Button, LucideIcon, MessageBubble, Textarea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (assistant.open()) {
      <div
        class="fixed inset-y-0 right-0 z-50 flex w-full max-w-md flex-col border-l border-border bg-background shadow-2xl"
        role="complementary"
        aria-label="AI assistant"
      >
        <!-- Header -->
        <div class="flex items-center justify-between border-b border-border bg-[#013366] px-4 py-3 text-white">
          <div class="flex items-center gap-2">
            <span class="flex h-8 w-8 items-center justify-center rounded-full bg-white/15">
              <svg lucide="Bot" class="h-5 w-5"></svg>
            </span>
            <div>
              <p class="text-sm font-semibold leading-tight">Licensing Assistant</p>
              <p class="text-xs text-white/70 leading-tight">
                {{ assistant.bridge() ? 'Helping with: ' + assistant.bridge()!.flowTitle : 'Navigation & guidance' }}
              </p>
            </div>
          </div>
          <div class="flex items-center gap-1">
            @if (messages().length > 0) {
              <button
                uiButton
                variant="ghost"
                size="sm"
                (click)="startOver()"
                class="h-8 gap-1.5 px-2 text-xs text-white hover:bg-white/15 hover:text-white"
              >
                <svg lucide="RotateCcw" class="h-4 w-4"></svg>
                Start over
              </button>
            }
            <button
              uiButton
              variant="ghost"
              size="icon"
              (click)="assistant.setOpen(false)"
              class="h-8 w-8 text-white hover:bg-white/15 hover:text-white"
              aria-label="Close assistant"
            >
              <svg lucide="X" class="h-5 w-5"></svg>
            </button>
          </div>
        </div>

        <!-- Messages -->
        <div #scroll class="flex-1 overflow-y-auto px-4 py-4">
          @if (messages().length === 0) {
            <div class="flex h-full flex-col">
              <div class="rounded-lg border border-border bg-muted/40 p-4">
                <div class="mb-2 flex items-center gap-2 text-[#013366]">
                  <svg lucide="Sparkles" class="h-4 w-4"></svg>
                  <span class="text-sm font-semibold">{{ intro().title }}</span>
                </div>
                <p class="text-sm text-muted-foreground leading-relaxed">{{ intro().body }}</p>
              </div>
              <p class="mb-2 mt-5 text-xs font-medium uppercase tracking-wide text-muted-foreground">Try asking</p>
              <div class="flex flex-col gap-2">
                @for (s of suggestions(); track s) {
                  <button
                    type="button"
                    (click)="submit(s)"
                    class="flex items-center justify-between rounded-lg border border-border bg-background px-3 py-2 text-left text-sm transition-colors hover:border-[#013366] hover:bg-muted/50"
                  >
                    <span>{{ s }}</span>
                    <svg lucide="ArrowRight" class="h-4 w-4 shrink-0 text-muted-foreground"></svg>
                  </button>
                }
              </div>
            </div>
          } @else {
            <div class="flex flex-col gap-4">
              @for (m of messages(); track m.id) {
                <assistant-message-bubble [role]="m.role" [parts]="$any(m.parts)" />
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
                  <!-- A usage cap can't be retried away, so don't offer a button that is guaranteed to fail. -->
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

        <!-- Offer to navigate instead of moving the user unprompted. -->
        @if (pendingNav() && !busy()) {
          <div class="border-t border-border bg-muted/30 px-3 py-3">
            <p class="mb-2 text-sm text-foreground">
              Want me to open <span class="font-semibold">{{ pendingNav()!.label }}</span>?
            </p>
            <div class="flex items-center gap-2">
              <button uiButton (click)="acceptNav()" class="h-9 flex-1 gap-1.5 bg-[#013366] text-sm hover:bg-[#38598A]">
                Yes, take me there
                <svg lucide="ArrowRight" class="h-4 w-4"></svg>
              </button>
              <button uiButton variant="outline" (click)="pendingNav.set(null)" class="h-9 text-sm">Stay here</button>
            </div>
          </div>
        }

        <!-- Call-to-action buttons + quick replies -->
        @if (actions().length > 0 || quickReplies().length > 0) {
          <div class="border-t border-border bg-muted/30 px-3 pt-3">
            @if (actions().length > 0) {
              <div class="flex flex-col gap-2">
                @for (a of actions(); track a.href + '-' + $index) {
                  <button
                    type="button"
                    (click)="go(a.href)"
                    class="flex items-center justify-between gap-2 rounded-lg bg-[#013366] px-3 py-2.5 text-left text-sm font-medium text-white transition-colors hover:bg-[#38598A]"
                  >
                    <span>{{ a.label }}</span>
                    <svg lucide="ArrowRight" class="h-4 w-4 shrink-0"></svg>
                  </button>
                }
              </div>
            }
            @if (quickReplies().length > 0) {
              <div class="mt-2 flex flex-wrap gap-2 pb-1">
                @for (r of quickReplies(); track r + '-' + $index) {
                  <button
                    type="button"
                    (click)="submit(r)"
                    class="rounded-full border border-[#013366]/30 bg-background px-3 py-1.5 text-xs font-medium text-[#013366] transition-colors hover:bg-[#013366] hover:text-white"
                  >
                    {{ r }}
                  </button>
                }
              </div>
            }
          </div>
        }

        <!-- Composer -->
        <div class="border-t border-border bg-background p-3">
          <div class="flex items-end gap-2">
            <textarea
              uiTextarea
              [value]="input()"
              (input)="input.set($any($event.target).value)"
              (keydown)="onKeyDown($event)"
              placeholder="Ask a question or describe what you need…"
              rows="1"
              class="max-h-32 min-h-[44px] resize-none"
              aria-label="Message the assistant"
            ></textarea>
            <button
              uiButton
              (click)="submit(input())"
              [disabled]="busy() || !input().trim()"
              size="icon"
              class="h-11 w-11 shrink-0 bg-[#013366] hover:bg-[#38598A]"
              aria-label="Send message"
            >
              <svg lucide="Send" class="h-5 w-5"></svg>
            </button>
          </div>
          <p class="mt-2 text-center text-[11px] text-muted-foreground">
            Prototype AI · responses may be inaccurate · verify with official policy
          </p>
        </div>
      </div>
    }
  `,
})
export class AssistantPanel {
  private readonly aiRepository = inject(AI_REPOSITORY);
  protected readonly assistant = injectAssistant();
  private readonly sessionProvider = injectSession();
  private readonly pathname = injectPathname();
  private readonly router = inject(Router);

  protected readonly input = signal('');
  private readonly scroll = viewChild<ElementRef<HTMLDivElement>>('scroll');

  // A navigation the assistant wants to perform but hasn't been given consent for.
  protected readonly pendingNav = signal<{ href: string; label: string } | null>(null);
  // The user's most recent message, used to decide whether they asked to be moved.
  private lastUserText = '';

  private readonly role = computed(() => roleForAi(this.sessionProvider.session()?.role));

  /** The live context object sent with every request. */
  private buildContext() {
    const role = this.role();
    // Snapshot of the user's own records so the assistant can answer with real data.
    const snapshot = this.aiRepository.portalSnapshot(role, this.sessionProvider.session()?.organization);
    const base = { role, pathname: this.pathname(), snapshot };
    const bridge = this.assistant.bridge();
    if (!bridge) return base;
    return {
      ...base,
      flowTitle: bridge.flowTitle,
      stepTitle: bridge.stepTitle,
      currentStep: bridge.currentStep,
      totalSteps: bridge.totalSteps,
      fields: bridge.getFields(),
      // Full catalog so the model knows which step each field lives on.
      allFields: bridge.getAllFields(),
    };
  }

  protected readonly chat: Chat<UIMessage> = new Chat<UIMessage>({
    // Drawn at shell start by the launcher, like the prototype's useChat on mount.
    id: this.assistant.chatId ?? undefined,
    transport: new DefaultChatTransport({
      api: '/api/assistant',
      prepareSendMessagesRequest: ({ messages }) => ({
        body: { messages, context: this.buildContext() },
      }),
    }),
    sendAutomaticallyWhen: shouldAutoResendToolResults,
    onToolCall: ({ toolCall }) => this.onToolCall(toolCall),
  });

  protected readonly messages = computed(() => this.chat.messages);
  private readonly status = computed(() => this.chat.status);
  private readonly error = computed(() => this.chat.error);
  protected readonly busy = computed(() => this.status() === 'submitted' || this.status() === 'streaming');

  // Transparent recovery from a brief throttle, but never from a hard usage cap.
  protected readonly retry = this.aiRepository.autoRetry({
    status: this.status,
    error: this.error,
    busy: this.busy,
    clearError: () => this.chat.clearError(),
    regenerate: () => void this.chat.regenerate(),
  });

  protected readonly suggestions = computed(() => SUGGESTIONS_BY_ROLE[this.role()] ?? SUGGESTIONS_BY_ROLE['public']);
  protected readonly intro = computed(() => INTRO_BY_ROLE[this.role()] ?? INTRO_BY_ROLE['public']);

  /** The latest assistant turn's part of this tool type, when idle. */
  private lastToolInput(type: string): Record<string, unknown> | undefined {
    const messages = this.messages();
    if (this.busy() || messages.length === 0) return undefined;
    const last = messages[messages.length - 1];
    if (last.role !== 'assistant') return undefined;
    const part = last.parts.find((p) => (p as { type?: string }).type === type) as
      | { input?: Record<string, unknown> }
      | undefined;
    return part?.input;
  }

  // Tappable quick-reply buttons proposed by the assistant in its latest turn.
  protected readonly quickReplies = computed<string[]>(() => {
    const replies = this.lastToolInput('tool-suggestReplies')?.['replies'];
    if (!Array.isArray(replies)) return [];
    return replies.filter((r): r is string => typeof r === 'string' && r.trim().length > 0).slice(0, 4);
  });

  // Prominent call-to-action buttons (e.g. "Start a Liquor Primary Application").
  protected readonly actions = computed<{ label: string; href: string }[]>(() => {
    const raw = this.lastToolInput('tool-suggestActions')?.['actions'];
    if (!Array.isArray(raw)) return [];
    return (
      raw
        .filter(
          (a): a is { label: string; href: string } =>
            !!a &&
            typeof (a as { label?: unknown }).label === 'string' &&
            typeof (a as { href?: unknown }).href === 'string',
        )
        // Don't offer buttons into areas this role can't use.
        .filter((a) => isPathAllowedForRole(this.role(), a.href))
        .slice(0, 3)
    );
  });

  constructor() {
    // Auto-scroll to the latest message.
    afterRenderEffect(() => {
      this.messages();
      this.status();
      const el = this.scroll()?.nativeElement;
      if (el) el.scrollTop = el.scrollHeight;
    });

    // Drop a seeded prompt into the composer when one is requested.
    effect(() => {
      const prompt = this.assistant.pendingPrompt();
      if (prompt && this.assistant.open()) {
        this.input.set(prompt);
        this.assistant.setPendingPrompt(null);
      }
    });
  }

  private onToolCall(toolCall: { dynamic?: boolean; toolName: string; toolCallId: string; input: unknown }) {
    if (toolCall.dynamic) return;
    const input = toolCall.input as Record<string, unknown>;
    const bridge = this.assistant.bridge();
    const role = this.role();
    const answer = (tool: string, output: string) =>
      void this.chat.addToolResult({ tool, toolCallId: toolCall.toolCallId, output } as never);
    switch (toolCall.toolName) {
      case 'navigateTo': {
        const href = String(input['href']);
        const label = String(input['label'] ?? href);
        // Hard stop: never drop a user into an area their role can't use.
        if (!isPathAllowedForRole(role, href)) {
          answer(
            'navigateTo',
            `Refused — ${href} is not part of this user's area of the portal, so nothing was opened. ` +
              `Explain that this topic doesn't apply to them and offer the LCRB contact details instead. ` +
              `Do not attempt to navigate there again.`,
          );
          break;
        }
        // Only move the user if they actually asked to be moved.
        if (isExplicitNavigationRequest(this.lastUserText)) {
          goToHref(this.router, href);
          answer('navigateTo', `Navigated to ${label}.`);
        } else {
          this.pendingNav.set({ href, label });
          answer(
            'navigateTo',
            `The user did not ask to be moved, so nothing has been opened. ` +
              `They have been shown a button offering to open ${label}. ` +
              `Answer their question in text now, and mention they can tap the button if they want to go there. ` +
              `Do not call navigateTo again for this.`,
          );
        }
        break;
      }
      case 'highlightField': {
        const name = String(input['name']);
        let ok = false;
        if (bridge) {
          // Navigate to the field's step first so it's actually on screen.
          const step = bridge.stepOfField(name);
          if (step !== null && step !== bridge.currentStep) bridge.goToStep(step);
          ok = step !== null;
        }
        // Defer the highlight so the target step has rendered.
        setTimeout(() => this.assistant.highlightField(name), 60);
        answer('highlightField', ok ? `Highlighted "${name}".` : `Could not find field "${name}".`);
        break;
      }
      case 'setField': {
        const name = String(input['name']);
        let ok = false;
        if (bridge) {
          // setFieldValue auto-navigates to the field's step.
          ok = bridge.setFieldValue(name, input['value']);
          setTimeout(() => this.assistant.highlightField(name), 60);
        }
        answer(
          'setField',
          ok
            ? `Set "${name}" to ${JSON.stringify(input['value'])}.`
            : `Could not set "${name}" — no active form or field not found.`,
        );
        break;
      }
      case 'goToStep': {
        const target = Number(input['step']);
        if (!bridge) {
          answer('goToStep', 'No active form.');
          break;
        }
        // "Can I go to the next page?" is a question, not an instruction.
        if (target > bridge.currentStep && !isExplicitNavigationRequest(this.lastUserText)) {
          const blockers = bridge.blockersPreview();
          answer(
            'goToStep',
            blockers.length > 0
              ? `Nothing was moved — the user only asked a question. They CANNOT continue yet: ` +
                  `${blockers.map((b) => `"${b.label}" (${b.error})`).join('; ')}. ` +
                  `Tell them what still needs completing, then ask if they want you to move on once it's done.`
              : `Nothing was moved — the user only asked a question. Everything required on this step is complete, ` +
                  `so they CAN continue. Tell them that and ask if they'd like you to move to the next step.`,
          );
          break;
        }
        const result = bridge.tryGoToStep(target);
        answer(
          'goToStep',
          result.ok
            ? `Moved to step ${target + 1}.`
            : `BLOCKED — did not move. These required fields must be completed first: ` +
                `${result.blockedBy.map((b) => `"${b.label}" (${b.error})`).join('; ')}. ` +
                `They are now flagged in red on the form. Tell the user what to fix; offer to fill them in if they give you the answers.`,
        );
        break;
      }
      // suggestActions / suggestReplies resolve on the server and render from the tool input, so there is nothing to acknowledge here.
    }
  }

  protected submit(text: string): void {
    const trimmed = text.trim();
    if (!trimmed || this.busy()) return;
    // Record intent before sending so tool handlers can tell a request to be moved from a question.
    this.lastUserText = trimmed;
    this.pendingNav.set(null);
    void this.chat.sendMessage({ text: trimmed });
    this.input.set('');
  }

  protected onKeyDown(e: KeyboardEvent): void {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      this.submit(this.input());
    }
  }

  // Clear the whole conversation and return to the fresh "How can I help?" state.
  protected startOver(): void {
    if (this.busy()) void this.chat.stop();
    this.chat.messages = [];
    this.input.set('');
    this.pendingNav.set(null);
    this.lastUserText = '';
  }

  protected acceptNav(): void {
    const target = this.pendingNav();
    this.pendingNav.set(null);
    if (target) goToHref(this.router, target.href);
  }

  protected go(href: string): void {
    goToHref(this.router, href);
  }
}
