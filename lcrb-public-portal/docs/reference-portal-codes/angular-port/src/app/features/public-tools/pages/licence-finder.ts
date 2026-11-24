/** `useChat` becomes `Chat` from @ai-sdk/angular (same UI message stream protocol). Both tools resolve on the server, so the UI renders from the tool input and adds no client results. */
import {
  afterRenderEffect,
  ChangeDetectionStrategy,
  Component,
  computed,
  type ElementRef,
  inject,
  input,
  output,
  signal,
  viewChild,
} from '@angular/core';
import { Router } from '@angular/router';
import { Chat } from '@ai-sdk/angular';
import { DefaultChatTransport, type UIMessage } from 'ai';
import { Button } from '@shared/ui/primitives/button';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { Textarea } from '@shared/ui/primitives/textarea';
import type { StartableApplication } from '@data-access/repositories/ai/application-actions';
import { shouldAutoResendToolResults } from '@domain/ai/auto-continue';
import { AI_REPOSITORY } from '@data-access/repositories/ai/ai.repository';
import { path } from '@/app/route-registry';

interface Recommendation {
  slug: string;
  title: string;
  reason: string;
  confidence: 'high' | 'medium' | 'low';
  establishmentName?: string | null;
  legalName?: string | null;
}

interface TextPart {
  type: 'text';
  text: string;
}

interface Starter {
  icon: IconName;
  label: string;
  hint: string;
  /** The natural-language prompt sent to the AI when the card is picked. */
  prompt: string;
}

// Visual starting points for the finder. Each card seeds a natural-language prompt that the AI reasons over to recommend the exact application flow.
const STARTERS: Starter[] = [
  {
    icon: 'UtensilsCrossed',
    label: 'Restaurant',
    hint: 'Serve liquor with meals',
    prompt: 'I want to open a restaurant that serves wine, beer, and cocktails with meals.',
  },
  {
    icon: 'Beer',
    label: 'Bar, pub or nightclub',
    hint: 'Liquor is the main draw',
    prompt: 'I want to open a bar, pub, or nightclub where serving liquor is the primary focus.',
  },
  {
    icon: 'Leaf',
    label: 'Cannabis retail store',
    hint: 'Sell non-medical cannabis',
    prompt: "I'm opening a store to sell non-medical cannabis to the public.",
  },
  {
    icon: 'Store',
    label: 'Private liquor store',
    hint: 'Sell packaged liquor to go',
    prompt: 'I want to open a private retail store that sells packaged liquor to take away.',
  },
  {
    icon: 'Factory',
    label: 'Manufacturer',
    hint: 'Brewery, winery or distillery',
    prompt: "I'm starting a manufacturer — a brewery, winery, or distillery — and want to make and sell my own product.",
  },
  {
    icon: 'Truck',
    label: 'Agent or marketer',
    hint: 'Represent or promote products',
    prompt: 'I want to represent or promote liquor or cannabis products to licensees on behalf of manufacturers.',
  },
];

/** Build a deep link into the right application flow, carrying any prefill. */
function hrefForRecommendation(rec: Recommendation, apps: StartableApplication[]): string {
  const match = apps.find((a) => a.slug === rec.slug);
  const base = match ? match.href : path('applications.flow', { slug: rec.slug });
  const params = new URLSearchParams();
  if (rec.establishmentName) params.set('establishmentName', rec.establishmentName);
  if (rec.legalName) params.set('legalName', rec.legalName);
  const qs = params.toString();
  return qs ? `${base}?${qs}` : base;
}

const CONFIDENCE_STYLES: Record<Recommendation['confidence'], string> = {
  high: 'bg-[#2E8540]/10 text-[#1E5631] border-[#2E8540]/30',
  medium: 'bg-amber-50 text-amber-800 border-amber-200',
  low: 'bg-muted text-muted-foreground border-border',
};

@Component({
  selector: 'finder-message',
  imports: [Button, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (isUser()) {
      <div class="flex justify-end">
        <div class="max-w-[85%] rounded-2xl rounded-br-sm bg-[#013366] px-4 py-2.5 text-sm text-white">{{ text() }}</div>
      </div>
    } @else if (text().trim() || recommendations().length > 0) {
      <div class="flex gap-3">
        <span class="mt-0.5 flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-[#013366]/10 text-[#013366]">
          <svg lucide="Bot" class="h-5 w-5"></svg>
        </span>
        <div class="flex w-full max-w-[90%] flex-col gap-3">
          @if (text()) {
            <div class="whitespace-pre-wrap rounded-2xl rounded-tl-sm bg-muted px-4 py-2.5 text-sm leading-relaxed text-foreground">{{ text() }}</div>
          }
          @if (recommendations().length > 0) {
            <div class="flex flex-col gap-2.5">
              @for (rec of recommendations(); track $index) {
                <div class="rounded-lg border border-[#013366]/20 bg-card p-4 shadow-sm">
                  <div class="flex items-start justify-between gap-3">
                    <div class="flex items-center gap-2">
                      <svg lucide="Sparkles" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
                      <h3 class="text-sm font-semibold text-foreground">{{ rec.title }}</h3>
                    </div>
                    <span
                      [class]="'shrink-0 rounded-full border px-2 py-0.5 text-[10px] font-medium uppercase tracking-wide ' + confidenceStyles[rec.confidence]"
                      >{{ rec.confidence }} match</span
                    >
                  </div>
                  <p class="mt-2 text-sm text-muted-foreground leading-relaxed">{{ rec.reason }}</p>
                  <button uiButton (click)="start.emit(rec)" class="mt-3 w-full justify-between gap-2 bg-[#013366] hover:bg-[#38598A]">
                    <span>Start {{ rec.title }}</span>
                    <svg lucide="ArrowRight" class="h-4 w-4"></svg>
                  </button>
                </div>
              }
            </div>
          }
        </div>
      </div>
    }
  `,
})
export class FinderMessage {
  readonly role = input.required<string>();
  readonly parts = input.required<{ type: string; [key: string]: unknown }[]>();
  readonly start = output<Recommendation>();
  protected readonly confidenceStyles = CONFIDENCE_STYLES;

  protected readonly isUser = computed(() => this.role() === 'user');
  protected readonly text = computed(() =>
    this.parts()
      .filter((p) => p.type === 'text')
      .map((p) => (p as unknown as TextPart).text)
      .join(''),
  );
  protected readonly recommendations = computed<Recommendation[]>(() => {
    const recPart = this.parts().find((p) => p.type === 'tool-recommendApplications') as
      | { input?: { recommendations?: unknown } }
      | undefined;
    const rawRecs = recPart?.input?.recommendations;
    return Array.isArray(rawRecs)
      ? (rawRecs.filter(
          (r) =>
            !!r &&
            typeof (r as { slug?: unknown }).slug === 'string' &&
            typeof (r as { title?: unknown }).title === 'string',
        ) as Recommendation[])
      : [];
  });
}

@Component({
  selector: 'licence-finder',
  imports: [Button, FinderMessage, LucideIcon, Textarea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="mx-auto flex min-h-[calc(100vh-220px)] w-full max-w-2xl flex-col">
      <!-- Intro / empty state -->
      @if (!started()) {
        <div class="flex flex-1 flex-col justify-center py-8">
          <div class="flex flex-col items-center text-center">
            <span class="flex h-14 w-14 items-center justify-center rounded-full bg-[#013366]/10 text-[#013366]">
              <svg lucide="Compass" class="h-7 w-7"></svg>
            </span>
            <h1 class="mt-4 text-2xl font-bold text-balance text-foreground">Find the right licence</h1>
            <p class="mt-2 max-w-md text-pretty text-muted-foreground leading-relaxed">
              Answer a few quick questions and I'll recommend the exact application you need — then take you straight to
              it. From restaurants to breweries to retail stores, let's skip the guesswork.
            </p>
          </div>
          <div class="mt-8">
            <p class="mb-3 text-xs font-medium uppercase tracking-wide text-muted-foreground">
              What are you planning to open?
            </p>
            <div class="grid grid-cols-1 gap-3 sm:grid-cols-2">
              @for (s of starters; track s.label) {
                <button
                  type="button"
                  (click)="submit(s.prompt)"
                  class="group flex items-center gap-3 rounded-xl border border-border bg-background p-4 text-left transition-colors hover:border-[#013366] hover:bg-muted/50"
                >
                  <span
                    class="flex h-11 w-11 shrink-0 items-center justify-center rounded-lg bg-[#013366]/10 text-[#013366] transition-colors group-hover:bg-[#013366] group-hover:text-white"
                  >
                    <svg [lucide]="s.icon" class="h-5 w-5"></svg>
                  </span>
                  <span class="min-w-0 flex-1">
                    <span class="block text-sm font-semibold text-foreground">{{ s.label }}</span>
                    <span class="block text-xs text-muted-foreground">{{ s.hint }}</span>
                  </span>
                  <svg
                    lucide="ArrowRight"
                    class="h-4 w-4 shrink-0 text-muted-foreground transition-transform group-hover:translate-x-0.5 group-hover:text-[#013366]"
                  ></svg>
                </button>
              }
            </div>
            <button
              type="button"
              (click)="composer()?.nativeElement?.focus()"
              class="mt-3 flex w-full items-center gap-3 rounded-xl border border-dashed border-border bg-background p-4 text-left transition-colors hover:border-[#013366] hover:bg-muted/50"
            >
              <span class="flex h-11 w-11 shrink-0 items-center justify-center rounded-lg bg-muted text-muted-foreground">
                <svg lucide="HelpCircle" class="h-5 w-5"></svg>
              </span>
              <span class="min-w-0 flex-1">
                <span class="block text-sm font-semibold text-foreground">Something else</span>
                <span class="block text-xs text-muted-foreground">Describe your business in your own words</span>
              </span>
              <svg lucide="ArrowRight" class="h-4 w-4 shrink-0 text-muted-foreground"></svg>
            </button>
          </div>
        </div>
      } @else {
        <div #scroll class="flex-1 overflow-y-auto py-6">
          <div class="flex flex-col gap-5">
            @for (m of messages(); track m.id) {
              <finder-message [role]="m.role" [parts]="$any(m.parts)" (start)="start($event)" />
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
        </div>
      }

      <!-- Quick replies -->
      @if (quickReplies().length > 0) {
        <div class="flex flex-wrap gap-2 pb-3">
          @for (r of quickReplies(); track r + '-' + $index) {
            <button
              type="button"
              (click)="submit(r)"
              class="rounded-full border border-[#013366]/30 bg-background px-3 py-1.5 text-sm font-medium text-[#013366] transition-colors hover:bg-[#013366] hover:text-white"
            >
              {{ r }}
            </button>
          }
        </div>
      }

      <!-- Composer -->
      <div class="sticky bottom-0 border-t border-border bg-background py-3">
        <div class="flex items-end gap-2">
          <textarea
            #composerEl
            uiTextarea
            [value]="input()"
            (input)="input.set($any($event.target).value)"
            (keydown)="onKeyDown($event)"
            placeholder="Describe your business or what you want to do…"
            rows="1"
            class="max-h-32 min-h-[44px] resize-none"
            aria-label="Describe what you need"
          ></textarea>
          <button
            uiButton
            (click)="submit(input())"
            [disabled]="busy() || !input().trim()"
            size="icon"
            class="h-11 w-11 shrink-0 bg-[#013366] hover:bg-[#38598A]"
            aria-label="Send"
          >
            <svg lucide="Send" class="h-5 w-5"></svg>
          </button>
        </div>
        <div class="mt-2 flex items-center justify-between">
          <p class="text-[11px] text-muted-foreground">Prototype AI · verify recommendations against official policy</p>
          @if (started()) {
            <button
              type="button"
              (click)="chat.messages = []"
              class="flex items-center gap-1 text-[11px] font-medium text-muted-foreground hover:text-[#013366]"
            >
              <svg lucide="RotateCcw" class="h-3 w-3"></svg>
              Start over
            </button>
          }
        </div>
      </div>
    </div>
  `,
})
export class LicenceFinder {
  private readonly aiRepository = inject(AI_REPOSITORY);
  private readonly router = inject(Router);
  protected readonly starters = STARTERS;
  protected readonly input = signal('');
  private readonly scroll = viewChild<ElementRef<HTMLDivElement>>('scroll');
  protected readonly composer = viewChild<ElementRef<HTMLTextAreaElement>>('composerEl');

  // Both tools resolve on the server: a client-resolved call ends the turn before any text. Both UIs render from tool input, so a client result would duplicate.
  protected readonly chat: Chat<UIMessage> = new Chat<UIMessage>({
    transport: new DefaultChatTransport({
      api: '/api/licence-finder',
      prepareSendMessagesRequest: ({ messages }) => ({
        body: { messages, context: { role: 'applicant' } },
      }),
    }),
    sendAutomaticallyWhen: shouldAutoResendToolResults,
  });

  protected readonly messages = computed(() => this.chat.messages);
  private readonly status = computed(() => this.chat.status);
  private readonly error = computed(() => this.chat.error);
  protected readonly busy = computed(() => this.status() === 'submitted' || this.status() === 'streaming');

  // Transparently recover from a brief throttle, but never from a hard usage cap.
  protected readonly retry = this.aiRepository.autoRetry({
    status: this.status,
    error: this.error,
    busy: this.busy,
    clearError: () => this.chat.clearError(),
    regenerate: () => void this.chat.regenerate(),
  });

  protected readonly started = computed(() => this.messages().length > 0);

  // Quick replies proposed by the agent in its latest turn.
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

  protected start(rec: Recommendation): void {
    void this.router.navigateByUrl(hrefForRecommendation(rec, this.aiRepository.startableApplications()));
  }
}
