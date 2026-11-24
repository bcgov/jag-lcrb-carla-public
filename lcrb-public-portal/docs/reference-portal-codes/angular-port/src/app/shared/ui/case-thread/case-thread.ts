/** Shared conversation panel for one application or permit. Effects match the prototype: seed once, mark read on change, scroll to bottom on new messages. */
import {
  afterNextRender,
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  ElementRef,
  inject,
  Injector,
  input,
  signal,
  untracked,
  viewChild,
} from '@angular/core';
import { injectSession } from '@core/session/session-provider';
import { Badge } from '../primitives/badge';
import { Button } from '../primitives/button';
import { LucideIcon } from '../primitives/lucide-icon';
import { Textarea } from '../primitives/textarea';
import { actorName, type UserRole } from '@domain/roles';
import { formatMessageTime, isThreadOpen, ROLE_LABELS, THREAD_PARTICIPANTS } from '@domain/messaging/chat';
import { type ThreadKind } from '@domain/models/messaging';
import { MESSAGING_REPOSITORY } from '@data-access/repositories/messaging/messaging.repository';

const ROLE_TONE: Record<UserRole, string> = {
  licensee: 'bg-bc-blue/10 text-bc-blue border-bc-blue/20',
  applicant: 'bg-bc-blue/10 text-bc-blue border-bc-blue/20',
  lcrb: 'bg-bc-gold/30 text-[#5a4200] border-bc-gold/60',
  'local-government': 'bg-teal-50 text-teal-900 border-teal-200',
  police: 'bg-slate-100 text-slate-800 border-slate-300',
};

@Component({
  selector: 'case-thread',
  imports: [Badge, Button, LucideIcon, Textarea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <section [attr.aria-label]="title()" [attr.class]="'flex flex-col rounded-lg border border-border bg-card ' + className()">
      <header
        [attr.class]="'flex flex-wrap items-center justify-between gap-2 border-b border-border px-4 ' + (compact() ? 'py-2.5' : 'py-3')"
      >
        <div class="flex items-center gap-2">
          <svg lucide="MessageSquare" class="h-4 w-4 text-[#013366]"></svg>
          <h2 [attr.class]="'font-semibold text-foreground ' + (compact() ? 'text-sm' : 'text-base')">{{ title() }}</h2>
          @if (open()) {
            <span uiBadge variant="outline" class="border-green-200 bg-green-50 text-green-800">Live</span>
          } @else {
            <span uiBadge variant="outline" class="border-zinc-200 bg-zinc-100 text-zinc-600">
              <svg lucide="Lock" class="mr-1 h-3 w-3"></svg>
              Closed
            </span>
          }
        </div>
        <p class="text-xs text-muted-foreground">With: {{ participantLabels().join(' · ') }}</p>
      </header>

      <div
        #list
        [attr.class]="'flex flex-col gap-3 overflow-y-auto px-4 py-4 ' + (compact() ? 'max-h-72' : 'max-h-96')"
        role="log"
        aria-live="polite"
        aria-relevant="additions"
      >
        @if (messages().length === 0) {
          <p class="py-6 text-center text-sm text-muted-foreground">
            {{
              open()
                ? 'No messages yet. Use this thread to resolve questions on the file without email back-and-forth.'
                : 'No messages were exchanged on this file.'
            }}
          </p>
        } @else {
          @for (m of messages(); track m.id) {
            @let mine = isMine(m.authorRole);
            <div [attr.class]="'flex ' + (mine ? 'justify-end' : 'justify-start')">
              <div [attr.class]="'max-w-[85%] ' + (mine ? 'items-end text-right' : 'items-start') + ' flex flex-col gap-1'">
                <div class="flex items-center gap-2 text-xs">
                  <span class="font-medium text-foreground">{{ mine ? 'You' : m.authorName }}</span>
                  <span uiBadge variant="outline" [class]="'h-5 px-1.5 text-[11px] ' + tone[m.authorRole]">
                    {{ labels[m.authorRole] }}
                  </span>
                  <span class="text-muted-foreground">{{ time(m.createdAt) }}</span>
                </div>
                <p
                  [attr.class]="
                    'whitespace-pre-wrap rounded-lg px-3 py-2 text-left text-sm leading-relaxed ' +
                    (mine ? 'bg-[#013366] text-white' : 'bg-muted text-foreground')
                  "
                >{{ m.body }}</p>
                @if (!mine && m.authorOrg) {
                  <span class="text-[11px] text-muted-foreground">{{ m.authorOrg }}</span>
                }
              </div>
            </div>
          }
        }
      </div>

      <footer class="border-t border-border px-4 py-3">
        @if (!canParticipate()) {
          <p class="text-xs text-muted-foreground">Your role is not a participant in this conversation.</p>
        } @else if (open()) {
          <div class="flex items-end gap-2">
            <textarea
              uiTextarea
              [value]="draft()"
              (input)="draft.set($any($event.target).value)"
              (keydown)="onKeydown($event)"
              rows="2"
              placeholder="Write a message… (Enter to send, Shift+Enter for a new line)"
              aria-label="Message"
              class="min-h-0 resize-none"
            ></textarea>
            <button
              uiButton
              size="icon"
              class="h-10 w-10 shrink-0 bg-[#013366] hover:bg-[#012a52]"
              [disabled]="!draft().trim()"
              (click)="send()"
              aria-label="Send message"
            >
              <svg lucide="Send" class="h-4 w-4"></svg>
            </button>
          </div>
        } @else {
          <p class="flex items-center gap-2 text-xs text-muted-foreground">
            <svg lucide="Lock" class="h-3.5 w-3.5"></svg>
            This conversation is closed because the file is no longer in process. Messages stay on the record.
          </p>
        }
      </footer>
    </section>
  `,
})
export class CaseThread {
  private readonly messagingRepository = inject(MESSAGING_REPOSITORY);
  readonly threadId = input.required<string>();
  readonly kind = input.required<ThreadKind>();
  /** Current status of the file; decides whether the thread is open. */
  readonly status = input.required<string>();
  /** Human label for notifications, e.g. "Liquor Primary Licence LP-125998". */
  readonly subjectLabel = input.required<string>();
  /** Per-role deep links used in notifications. */
  readonly hrefFor = input.required<Partial<Record<UserRole, string>>>();
  /** Optional heading override. */
  readonly title = input('Case conversation');
  readonly className = input('');
  /** Compact mode trims the header for sidebars / sheets. */
  readonly compact = input(false);

  private readonly sessionProvider = injectSession();
  private readonly injector = inject(Injector);
  private readonly list = viewChild<ElementRef<HTMLElement>>('list');
  protected readonly tone = ROLE_TONE;
  protected readonly labels = ROLE_LABELS;
  protected readonly time = formatMessageTime;

  protected readonly role = computed<UserRole>(() => this.sessionProvider.session()?.role ?? 'licensee');
  protected readonly messages = this.messagingRepository.threadMessages(() => this.threadId());
  protected readonly open = computed(() => isThreadOpen(this.kind(), this.status()));
  protected readonly canParticipate = computed(() => THREAD_PARTICIPANTS[this.kind()].includes(this.role()));
  protected readonly draft = signal('');
  private readonly mounted = signal(false);

  protected readonly participantLabels = computed(() => {
    const kind = this.kind();
    const participants = THREAD_PARTICIPANTS[kind].filter((r) => r !== 'applicant' || kind === 'sep');
    return Array.from(new Set(participants.map((r) => ROLE_LABELS[r])));
  });

  constructor() {
    afterNextRender(() => {
      this.messagingRepository.seedIfEmpty();
      this.mounted.set(true);
    });

    // Opening the panel counts as reading it; keep the watermark current as new messages land.
    effect(() => {
      if (!this.mounted()) return;
      const threadId = this.threadId();
      const role = this.role();
      const canParticipate = this.canParticipate();
      this.messages().length;
      untracked(() => {
        if (canParticipate) this.messagingRepository.markThreadRead(threadId, role);
      });
    });

    effect(() => {
      if (!this.mounted()) return;
      this.messages().length;
      untracked(() => {
        // Run once the new messages are rendered, like React's post-commit effect.
        afterNextRender(
          () => {
            const el = this.list()?.nativeElement;
            if (el) el.scrollTop = el.scrollHeight;
          },
          { injector: this.injector },
        );
      });
    });
  }

  protected isMine(authorRole: UserRole): boolean {
    const role = this.role();
    return (
      authorRole === role ||
      // The applicant appears as "licensee" on licence files and "applicant" on SEPs depending on sign-in; treat both as the same seat.
      ((authorRole === 'licensee' || authorRole === 'applicant') && (role === 'licensee' || role === 'applicant'))
    );
  }

  protected onKeydown(e: KeyboardEvent): void {
    if (e.key !== 'Enter' || e.shiftKey || e.isComposing || e.keyCode === 229) return;
    e.preventDefault();
    this.send();
  }

  protected send(): void {
    const body = this.draft().trim();
    if (!body || !this.open() || !this.canParticipate()) return;
    const session = this.sessionProvider.session();
    this.messagingRepository.postMessage({
      threadId: this.threadId(),
      kind: this.kind(),
      authorRole: this.role(),
      authorName: actorName(session),
      authorOrg: session?.organization,
      body,
      subjectLabel: this.subjectLabel(),
      hrefFor: this.hrefFor(),
    });
    this.draft.set('');
  }
}
