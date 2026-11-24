/** Standalone simulated "one door into government" chat. `useChat` becomes `Chat` from @ai-sdk/angular; `display: contents` hosts keep the list's direct children. */
import {
  afterRenderEffect,
  ChangeDetectionStrategy,
  Component,
  computed,
  DestroyRef,
  type ElementRef,
  inject,
  input,
  signal,
  viewChild,
} from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Chat } from '@ai-sdk/angular';
import { DefaultChatTransport, type UIMessage } from 'ai';
import { AccessibilityBar } from '@core/layout/accessibility-bar';
import { BCLogo } from '@shared/ui/bc-gov/bc-logo';
import { Button } from '@shared/ui/primitives/button';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { BRANCHES, type BranchId, getBranch } from '@domain/one-door/branches';

const BRANCH_ICON: Record<BranchId, IconName> = {
  lcrb: 'Wine',
  icbc: 'Car',
  health: 'HeartPulse',
  servicebc: 'Landmark',
  revenue: 'Receipt',
  housing: 'Home',
};

/** Starters chosen so at least two lead straight to LCRB and the rest show the router working. */
const STARTERS = [
  'I want to open a restaurant that serves wine',
  'How do I get a permit to serve beer at our fundraiser?',
  "Renew my driver's licence",
  "I'm opening a cannabis store in Kelowna",
  'Enrol my newborn in MSP',
  'My landlord raised the rent — is that allowed?',
];

const GREETING: UIMessage = {
  id: 'greeting',
  role: 'assistant',
  parts: [
    {
      type: 'text',
      text: "Hello, and welcome. I'm the Province of British Columbia's assistant — one place to start no matter which part of government you need. Tell me what you're trying to do, in your own words, and I'll either help right here or connect you to the right branch with your request already in hand.",
    },
  ],
};

interface HandoffInput {
  branch?: string;
  summary?: string;
  keywords?: string[];
}

@Component({
  selector: 'handoff-card',
  imports: [Button, LucideIcon, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (branch(); as b) {
      <div
        [class]="'overflow-hidden rounded-xl border-2 ' + (live() ? 'border-[#013366]' : 'border-[#c9c9c9]') + ' bg-white shadow-sm'"
        role="status"
      >
        <div [class]="'flex items-center gap-3 px-4 py-3 ' + (live() ? 'bg-[#013366] text-white' : 'bg-[#f0f0f0] text-[#2d2d2d]')">
          <span [class]="'rounded-md p-1.5 ' + (live() ? 'bg-white/15' : 'bg-white')">
            <svg [lucide]="icon()" class="h-5 w-5" aria-hidden="true"></svg>
          </span>
          <div class="min-w-0 flex-1">
            <p class="text-xs uppercase tracking-wide opacity-80">{{ ready() ? 'Connected to' : 'Connecting you to' }}</p>
            <p class="truncate font-semibold leading-tight">{{ b.name }}</p>
          </div>
          @if (!ready()) {
            <span class="flex gap-1" aria-hidden="true">
              <span class="h-1.5 w-1.5 animate-bounce rounded-full bg-current [animation-delay:-0.3s]"></span>
              <span class="h-1.5 w-1.5 animate-bounce rounded-full bg-current [animation-delay:-0.15s]"></span>
              <span class="h-1.5 w-1.5 animate-bounce rounded-full bg-current"></span>
            </span>
          }
        </div>

        <div class="space-y-3 px-4 py-4">
          <p class="text-sm text-[#474543]">{{ b.blurb }}</p>

          @if (data().summary) {
            <div class="rounded-lg border border-[#FCBA19]/60 bg-[#FCBA19]/10 px-3 py-2.5">
              <p class="text-xs font-semibold uppercase tracking-wide text-[#7a5a00]">Travelling with you</p>
              <p class="mt-0.5 text-sm text-[#2d2d2d]">{{ data().summary }}</p>
            </div>
          }

          @if (data().keywords?.length) {
            <p class="text-xs text-[#6b6b6b]">
              Routed on:
              @for (k of data().keywords; track k; let i = $index) {
                <span><span class="rounded bg-[#f0f0f0] px-1.5 py-0.5 font-medium text-[#2d2d2d]">{{ k }}</span>{{ i < data().keywords!.length - 1 ? ' ' : '' }}</span>
              }
            </p>
          }

          @if (live()) {
            <a
              uiButton
              [class]="
                'w-full justify-between bg-[#013366] hover:bg-[#012a52] transition-opacity ' +
                (ready() ? 'opacity-100' : 'pointer-events-none opacity-50')
              "
              [routerLink]="parse(b.href!)"
              [attr.aria-disabled]="!ready()"
              [attr.tabindex]="ready() ? 0 : -1"
            >
              Continue to the {{ b.short }} licensing portal
              <svg lucide="ArrowRight" class="h-4 w-4" aria-hidden="true"></svg>
            </a>
          } @else {
            <p class="rounded-lg border border-dashed border-[#c9c9c9] px-3 py-2 text-sm text-[#6b6b6b]">
              In the real service you would continue to {{ b.short }} here. That destination isn't part of this
              prototype — try a liquor or cannabis licensing question to see a full hand-off.
            </p>
          }
        </div>
      </div>
    }
  `,
})
export class HandoffCard {
  private readonly router = inject(Router);
  readonly data = input.required<HandoffInput>({ alias: 'input' });
  protected readonly branch = computed(() => getBranch(this.data().branch ?? ''));
  protected readonly live = computed(() => Boolean(this.branch()?.href));
  protected readonly icon = computed(() => BRANCH_ICON[this.branch()!.id]);

  // Brief "connecting…" beat so the transfer reads as a real hand-off.
  protected readonly ready = signal(false);

  constructor() {
    const t = setTimeout(() => this.ready.set(true), 1400);
    inject(DestroyRef).onDestroy(() => clearTimeout(t));
  }

  protected parse(href: string) {
    return this.router.parseUrl(href);
  }
}

@Component({
  selector: 'one-door-message-bubble',
  imports: [HandoffCard, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (isUser()) {
      <div class="flex justify-end">
        <div class="max-w-[85%] rounded-2xl rounded-br-sm bg-[#013366] px-4 py-2.5 text-white">
          <p class="whitespace-pre-wrap text-[15px] leading-relaxed">{{ text() }}</p>
        </div>
      </div>
    } @else {
      <div class="flex gap-3">
        <div
          class="mt-1 flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-[#FCBA19] text-[#013366]"
          aria-hidden="true"
        >
          <svg lucide="Building2" class="h-4 w-4"></svg>
        </div>
        <div class="min-w-0 max-w-[85%] space-y-3">
          @if (text()) {
            <div class="rounded-2xl rounded-tl-sm border border-[#e3e3e3] bg-[#fafafa] px-4 py-2.5">
              <p class="whitespace-pre-wrap text-[15px] leading-relaxed text-[#2d2d2d]">{{ text() }}</p>
            </div>
          }
          @if (handoff(); as h) {
            <handoff-card [input]="h" />
          }
        </div>
      </div>
    }
  `,
})
export class OneDoorMessageBubble {
  readonly message = input.required<UIMessage>();
  protected readonly isUser = computed(() => this.message().role === 'user');
  protected readonly text = computed(() =>
    this.message()
      .parts.filter((p) => p.type === 'text')
      .map((p) => (p as { text: string }).text)
      .join(''),
  );
  protected readonly handoff = computed(() => {
    const part = this.message().parts.find((p) => p.type === 'tool-handoffToBranch') as
      | { input?: HandoffInput; state?: string }
      | undefined;
    return part?.input?.branch ? part.input : null;
  });
}

@Component({
  selector: 'one-door-typing-indicator',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex gap-3" aria-label="Assistant is typing">
      <div class="mt-1 flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-[#FCBA19] text-[#013366]" aria-hidden="true">
        <svg lucide="Building2" class="h-4 w-4"></svg>
      </div>
      <div class="flex items-center gap-1 rounded-2xl rounded-tl-sm border border-[#e3e3e3] bg-[#fafafa] px-4 py-3" aria-hidden="true">
        <span class="h-2 w-2 animate-bounce rounded-full bg-[#38598A] [animation-delay:-0.3s]"></span>
        <span class="h-2 w-2 animate-bounce rounded-full bg-[#38598A] [animation-delay:-0.15s]"></span>
        <span class="h-2 w-2 animate-bounce rounded-full bg-[#38598A]"></span>
      </div>
    </div>
  `,
})
export class OneDoorTypingIndicator {}

@Component({
  selector: 'one-door-experience',
  imports: [AccessibilityBar, BCLogo, Button, LucideIcon, OneDoorMessageBubble, OneDoorTypingIndicator],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col bg-[#F2F2F2] text-[#2d2d2d]">
      <bc-accessibility-bar />

      <!-- Province header: deliberately generic, not the LCRB portal chrome -->
      <header class="bg-[#013366] text-white">
        <div class="mx-auto flex h-16 max-w-6xl items-center justify-between gap-4 px-4 sm:px-6 md:h-20 lg:px-8">
          <div class="flex items-center gap-3 md:gap-4">
            <bc-logo reversed />
            <span class="border-l border-white/30 pl-3 text-base font-semibold md:pl-4 md:text-lg">
              Government of British Columbia
            </span>
          </div>
          <span
            class="hidden rounded-full border border-[#FCBA19]/60 bg-[#FCBA19]/10 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-[#FCBA19] sm:inline-flex"
          >
            Prototype simulation
          </span>
        </div>
      </header>
      <div class="h-1 bg-[#FCBA19]" aria-hidden="true"></div>

      <main id="main-content" class="flex-1">
        <div class="mx-auto grid max-w-6xl gap-8 px-4 py-8 sm:px-6 lg:grid-cols-[1fr_320px] lg:px-8 lg:py-12">
          <!-- Conversation -->
          <section aria-labelledby="one-door-heading" class="flex flex-col">
            <div class="mb-6">
              <p class="mb-2 inline-flex items-center gap-2 text-sm font-semibold uppercase tracking-wide text-[#38598A]">
                <svg lucide="Sparkles" class="h-4 w-4" aria-hidden="true"></svg>
                One door into government
              </p>
              <h1 id="one-door-heading" class="text-balance text-3xl font-bold text-[#013366] md:text-4xl">
                How can we help you today?
              </h1>
              <p class="mt-2 max-w-2xl text-pretty text-base leading-relaxed text-[#474543]">
                One assistant for the whole Province. Describe what you need — it will answer simple questions itself
                and connect you to the right branch of government, carrying your request with you so you never have to
                start over.
              </p>
            </div>

            <div class="flex min-h-[520px] flex-col overflow-hidden rounded-xl border border-[#d9d9d9] bg-white shadow-sm">
              <div
                #scroll
                class="flex-1 space-y-5 overflow-y-auto px-4 py-5 sm:px-6"
                role="log"
                aria-live="polite"
                aria-label="Conversation with the BC Government assistant"
              >
                @for (m of messages(); track m.id) {
                  <one-door-message-bubble [message]="m" />
                }
                @if (busy() && last()?.role === 'user') {
                  <one-door-typing-indicator />
                }
              </div>

              <!-- Quick replies / starters -->
              @if (lastReplies().length > 0 || !hasUserSpoken()) {
                <div class="flex flex-wrap gap-2 border-t border-[#ececec] bg-[#fafafa] px-4 py-3 sm:px-6">
                  @for (r of hasUserSpoken() ? lastReplies() : starters; track r) {
                    <button
                      type="button"
                      (click)="send(r)"
                      [disabled]="busy()"
                      class="rounded-full border border-[#013366]/30 bg-white px-3 py-1.5 text-sm text-[#013366] transition-colors hover:border-[#013366] hover:bg-[#013366]/5 disabled:opacity-50"
                    >
                      {{ r }}
                    </button>
                  }
                </div>
              }

              @if (chat.error; as err) {
                <div role="alert" class="border-t border-red-200 bg-red-50 px-4 py-2 text-sm text-red-800 sm:px-6">
                  {{ err.message }}
                  <button type="button" (click)="chat.clearError()" class="underline">Dismiss</button>
                </div>
              }

              <form
                class="flex items-end gap-2 border-t border-[#ececec] px-3 py-3 sm:px-4"
                (submit)="$event.preventDefault(); send(input())"
              >
                <label for="one-door-input" class="sr-only">Tell us what you need</label>
                <textarea
                  id="one-door-input"
                  [value]="input()"
                  (input)="input.set($any($event.target).value)"
                  (keydown)="onKeyDown($event)"
                  rows="1"
                  placeholder="Tell us what you need, in your own words…"
                  class="max-h-32 min-h-[44px] flex-1 resize-none rounded-lg border border-[#c9c9c9] px-3 py-2.5 text-base leading-relaxed focus-visible:border-[#013366] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#013366]/20"
                ></textarea>
                <button
                  uiButton
                  type="submit"
                  [disabled]="busy() || !input().trim()"
                  class="h-11 w-11 shrink-0 rounded-lg bg-[#013366] p-0 hover:bg-[#012a52]"
                  aria-label="Send"
                >
                  <svg lucide="ArrowUp" class="h-5 w-5" aria-hidden="true"></svg>
                </button>
              </form>
            </div>

            <div class="mt-3 flex items-center justify-between text-xs text-[#6b6b6b]">
              <span>A simulation. The assistant may make mistakes; branch staff will confirm details.</span>
              @if (hasUserSpoken()) {
                <button type="button" (click)="reset()" class="inline-flex items-center gap-1 hover:text-[#013366]">
                  <svg lucide="RotateCcw" class="h-3.5 w-3.5" aria-hidden="true"></svg>
                  Start over
                </button>
              }
            </div>
          </section>

          <!-- Sidebar: what the door can route to -->
          <aside aria-labelledby="branches-heading" class="lg:pt-24">
            <h2 id="branches-heading" class="mb-3 text-sm font-semibold uppercase tracking-wide text-[#38598A]">
              Connected branches
            </h2>
            <ul class="space-y-2">
              @for (b of branches; track b.id) {
                <li
                  [class]="'flex items-start gap-3 rounded-lg border bg-white px-3 py-2.5 ' + (b.href ? 'border-[#013366]/40' : 'border-[#e3e3e3]')"
                >
                  <span
                    [class]="'mt-0.5 rounded-md p-1.5 ' + (b.href ? 'bg-[#013366] text-white' : 'bg-[#f0f0f0] text-[#6b6b6b]')"
                  >
                    <svg [lucide]="branchIcon[b.id]" class="h-4 w-4" aria-hidden="true"></svg>
                  </span>
                  <span class="min-w-0 flex-1">
                    <span class="block text-sm font-medium leading-tight text-[#2d2d2d]">{{ b.name }}</span>
                    <span class="block text-xs text-[#6b6b6b]">
                      {{ b.href ? 'Connected in this prototype' : 'Recognised — destination not simulated' }}
                    </span>
                  </span>
                </li>
              }
            </ul>
            <p class="mt-4 text-xs leading-relaxed text-[#6b6b6b]">
              In the real service every branch would be connected. This prototype fully simulates the hand-off to the
              Liquor and Cannabis Regulation Branch only.
            </p>
          </aside>
        </div>
      </main>

      <footer class="border-t-2 border-[#FCBA19] bg-white text-sm text-[#474543]">
        <div class="mx-auto flex max-w-6xl flex-col items-center justify-between gap-2 px-4 py-6 sm:flex-row sm:px-6 lg:px-8">
          <span>A demonstration prototype for the Province of British Columbia</span>
          <span>Not an official government service</span>
        </div>
      </footer>
    </div>
  `,
})
export class OneDoorExperience {
  protected readonly starters = STARTERS;
  protected readonly branches = BRANCHES;
  protected readonly branchIcon = BRANCH_ICON;
  protected readonly input = signal('');
  private readonly scroll = viewChild<ElementRef<HTMLDivElement>>('scroll');

  protected readonly chat: Chat<UIMessage> = new Chat<UIMessage>({
    transport: new DefaultChatTransport({ api: '/api/one-door' }),
    messages: [GREETING],
  });

  protected readonly messages = computed(() => this.chat.messages);
  private readonly status = computed(() => this.chat.status);
  protected readonly busy = computed(() => this.status() === 'submitted' || this.status() === 'streaming');
  protected readonly hasUserSpoken = computed(() => this.messages().some((m) => m.role === 'user'));
  protected readonly last = computed(() => this.messages()[this.messages().length - 1]);

  // The most recent quick replies: only shown on the last assistant turn.
  protected readonly lastReplies = computed<string[]>(() => {
    const last = this.last();
    return last?.role === 'assistant' && !this.busy()
      ? ((last.parts.find((p) => p.type === 'tool-suggestReplies') as { input?: { replies?: string[] } } | undefined)
          ?.input?.replies ?? [])
      : [];
  });

  constructor() {
    afterRenderEffect(() => {
      this.messages();
      this.status();
      const el = this.scroll()?.nativeElement;
      el?.scrollTo({ top: el.scrollHeight, behavior: 'smooth' });
    });
  }

  protected send(text: string): void {
    const trimmed = text.trim();
    if (!trimmed || this.busy()) return;
    this.input.set('');
    void this.chat.sendMessage({ text: trimmed });
  }

  protected reset(): void {
    this.chat.clearError();
    this.chat.messages = [GREETING];
    this.input.set('');
  }

  protected onKeyDown(e: KeyboardEvent): void {
    if (e.key === 'Enter' && !e.shiftKey && !e.isComposing && e.keyCode !== 229) {
      e.preventDefault();
      this.send(this.input());
    }
  }
}
