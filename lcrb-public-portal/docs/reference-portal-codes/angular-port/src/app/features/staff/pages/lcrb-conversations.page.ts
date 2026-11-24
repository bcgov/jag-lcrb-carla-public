/** Staff conversations hub: every in-process file is listed, threads with unread messages float up, closed threads stay readable; `?thread=` and `?kind=` hold the selection. */
import { afterNextRender, ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { CaseThread } from '@shared/ui/case-thread/case-thread';
import { LcrbGate } from '@core/permissions/lcrb-gate';
import { injectSession } from '@core/session/session-provider';
import { Badge } from '@shared/ui/primitives/badge';
import { CARD } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { TABS } from '@shared/ui/primitives/tabs';
import { MY_APPLICATION_STATUS_LABELS } from '@domain/applications/my-applications';
import { SEP_STATUS_LABELS } from '@domain/special-events/seps';
import { formatMessageTime, isThreadOpen } from '@domain/messaging/chat';
import { type ThreadKind } from '@domain/models/messaging';
import { LG_REVIEW_REPOSITORY } from '@data-access/repositories/lg-review/lg-review.repository';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { APPLICATIONS_REPOSITORY } from '@data-access/repositories/applications/applications.repository';
import { MESSAGING_REPOSITORY } from '@data-access/repositories/messaging/messaging.repository';
import { path } from '@/app/route-registry';

export interface ConversationItem {
  threadId: string;
  kind: ThreadKind;
  title: string;
  subtitle: string;
  status: string;
  statusLabel: string;
  open: boolean;
  unread: number;
  lastActivity: string | null;
  preview: string | null;
  partnerLabel: string;
}

@Component({
  selector: 'lcrb-conversations-content',
  imports: [...PAGE_HERO, ...CARD, ...TABS, Badge, LucideIcon, CaseThread],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <page-hero
      [breadcrumbs]="[{ label: 'Intake Overview', href: path('staff.home') }, { label: 'Conversations' }]"
      title="Case conversations"
      description="Live threads with applicants and partner agencies on files that are in process. Licence files include the local government; Special Event Permits include police."
    >
      @if (totalUnread() > 0) {
        <span heroBadge uiBadge class="bg-bc-gold text-bc-blue hover:bg-bc-gold">{{ totalUnread() }} unread</span>
      }
    </page-hero>
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div uiTabs [value]="kindFilter()" (valueChange)="setKind($event ?? 'all')" class="mb-4">
        <div uiTabsList>
          <button uiTabsTrigger value="all">All</button>
          <button uiTabsTrigger value="application" class="gap-1.5">
            <svg lucide="Building2" class="h-3.5 w-3.5"></svg>
            Licence applications
          </button>
          <button uiTabsTrigger value="sep" class="gap-1.5">
            <svg lucide="ShieldCheck" class="h-3.5 w-3.5"></svg>
            Special Event Permits
          </button>
        </div>
      </div>

      <div class="grid gap-6 lg:grid-cols-[380px_1fr]">
        <div uiCard class="self-start">
          <div uiCardContent class="p-0">
            @if (items().length === 0) {
              <p class="p-6 text-sm text-muted-foreground">No files in process.</p>
            } @else {
              <ul class="divide-y divide-border" aria-label="Conversations">
                @for (item of items(); track item.kind + '-' + item.threadId) {
                  <li>
                    <button
                      type="button"
                      (click)="select(item.threadId)"
                      [attr.aria-current]="item.threadId === selectedId() ? 'true' : null"
                      [class]="
                        'flex w-full flex-col gap-1 px-4 py-3 text-left transition-colors hover:bg-muted/50 ' +
                        (item.threadId === selectedId() ? 'bg-[#013366]/5 border-l-2 border-[#013366]' : 'border-l-2 border-transparent')
                      "
                    >
                      <div class="flex items-start justify-between gap-2">
                        <span [class]="'font-medium leading-snug ' + (item.unread ? 'text-foreground' : 'text-foreground/90')">
                          {{ item.title }}
                        </span>
                        @if (item.unread > 0) {
                          <span uiBadge class="h-5 min-w-5 justify-center rounded-full bg-bc-gold px-1.5 text-[11px] text-bc-blue hover:bg-bc-gold">
                            {{ item.unread }}
                          </span>
                        } @else if (!item.open) {
                          <svg lucide="Lock" class="h-3.5 w-3.5 shrink-0 text-muted-foreground" aria-label="Closed"></svg>
                        }
                      </div>
                      <span class="text-xs text-muted-foreground">{{ item.subtitle }}</span>
                      <div class="flex flex-wrap items-center gap-x-2 gap-y-1 text-xs text-muted-foreground">
                        <span uiBadge variant="outline" class="h-5 px-1.5 text-[11px]">{{ item.statusLabel }}</span>
                        <span>{{ item.kind === 'sep' ? 'Police' : 'LG/IN' }}: {{ item.partnerLabel }}</span>
                      </div>
                      @if (item.preview) {
                        <p class="line-clamp-1 text-xs text-muted-foreground">
                          <span class="text-foreground/70">{{ formatMessageTime(item.lastActivity!) }}</span> ·{{ ' ' }}{{ item.preview }}
                        </p>
                      } @else {
                        <p class="text-xs italic text-muted-foreground">No messages yet</p>
                      }
                    </button>
                  </li>
                }
              </ul>
            }
          </div>
        </div>

        @for (sel of selectedList(); track sel.threadId) {
          <case-thread
            [threadId]="sel.threadId"
            [kind]="sel.kind"
            [status]="sel.status"
            [subjectLabel]="sel.title + ' ' + sel.threadId"
            [title]="sel.title + ' — ' + sel.threadId"
            [hrefFor]="hrefFor(sel)"
          />
        } @empty {
          <div class="flex min-h-72 flex-col items-center justify-center rounded-lg border border-dashed border-border text-center">
            <svg lucide="MessageSquare" class="h-8 w-8 text-muted-foreground/60"></svg>
            <p class="mt-3 text-sm font-medium text-foreground">Select a file to open its conversation</p>
            <p class="mt-1 max-w-sm text-sm text-muted-foreground">
              Messages you send are delivered to the applicant and the partner agency on the file, with a
              notification linking them straight back here.
            </p>
          </div>
        }
      </div>
    </div>
  `,
})
export class LcrbConversationsContent {
  protected readonly path = path;
  private readonly lgReviewRepository = inject(LG_REVIEW_REPOSITORY);
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  private readonly messagingRepository = inject(MESSAGING_REPOSITORY);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly params = toSignal(this.route.queryParamMap, { initialValue: this.route.snapshot.queryParamMap });

  protected readonly selectedId = computed(() => this.params().get('thread'));
  protected readonly kindFilter = computed(() => (this.params().get('kind') as ThreadKind | null) ?? 'all');

  private readonly sepStore = this.specialEventsRepository.store();
  private readonly lgStore = this.lgReviewRepository.store();
  private readonly cancellations = this.applicationsRepository.cancellations();
  private readonly threads = this.messagingRepository.threads('lcrb');
  protected readonly formatMessageTime = formatMessageTime;

  constructor() {
    afterNextRender(() => this.messagingRepository.seedIfEmpty());
  }

  protected readonly items = computed<ConversationItem[]>(() => {
    const threads = this.threads();
    const seps = this.sepStore.records();
    const lgApps = this.lgStore.applications();
    const kindFilter = this.kindFilter();
    const byThread = new Map(threads.map((t) => [t.threadId, t]));
    const cancelledIds = new Set(this.cancellations().map((c) => c.id));
    const out: ConversationItem[] = [];

    // Licence applications: the applicant's submitted files, with the LG referral (if any) supplying the partner label.
    for (const a of this.applicationsRepository.myApplications().submitted) {
      const status = cancelledIds.has(a.id) ? 'cancelled' : a.status;
      const t = byThread.get(a.id);
      const lg = lgApps.find((l) => l.jobNumber === a.id);
      out.push({
        threadId: a.id,
        kind: 'application',
        title: a.type,
        subtitle: `${a.id} · ${a.reference}`,
        status,
        statusLabel: MY_APPLICATION_STATUS_LABELS[status as keyof typeof MY_APPLICATION_STATUS_LABELS] ?? status,
        open: isThreadOpen('application', status),
        unread: t?.unread ?? 0,
        lastActivity: t?.lastMessage.createdAt ?? null,
        preview: t ? `${t.lastMessage.authorName}: ${t.lastMessage.body}` : null,
        partnerLabel: lg ? lg.jurisdiction : 'LCRB only',
      });
    }

    // SEPs: everything in process, plus anything that already has a thread.
    for (const s of seps) {
      const t = byThread.get(s.id);
      const open = isThreadOpen('sep', s.status);
      if (!open && !t) continue;
      out.push({
        threadId: s.id,
        kind: 'sep',
        title: s.eventName,
        subtitle: `${s.id} · ${s.applicantName}${s.applicantOrg ? ` (${s.applicantOrg})` : ''}`,
        status: s.status,
        statusLabel: SEP_STATUS_LABELS[s.status],
        open,
        unread: t?.unread ?? 0,
        lastActivity: t?.lastMessage.createdAt ?? null,
        preview: t ? `${t.lastMessage.authorName}: ${t.lastMessage.body}` : null,
        partnerLabel: s.jurisdiction,
      });
    }

    return out
      .filter((i) => kindFilter === 'all' || i.kind === kindFilter)
      .sort((a, b) => {
        if (a.unread !== b.unread) return b.unread - a.unread;
        if (a.open !== b.open) return a.open ? -1 : 1;
        return (b.lastActivity ?? '').localeCompare(a.lastActivity ?? '');
      });
  });

  protected readonly selected = computed(() => this.items().find((i) => i.threadId === this.selectedId()) ?? null);
  /** The selection as a list: the prototype's `key={selected.threadId}` remounts the thread per file. */
  protected readonly selectedList = computed(() => {
    const s = this.selected();
    return s ? [s] : [];
  });
  protected readonly totalUnread = computed(() => this.threads().reduce((n, t) => n + t.unread, 0));

  protected hrefFor(sel: ConversationItem) {
    return sel.kind === 'sep'
      ? {
          applicant: path('special-events.detail', { id: sel.threadId }),
          licensee: path('special-events.detail', { id: sel.threadId }),
          police: path('police.review', { id: sel.threadId }),
          lcrb: `${path('staff.conversations')}?thread=${encodeURIComponent(sel.threadId)}`,
        }
      : {
          licensee: path('applications.submitted', { id: sel.threadId }),
          'local-government': path('lg.review', { id: sel.threadId }),
          lcrb: `${path('staff.conversations')}?thread=${encodeURIComponent(sel.threadId)}`,
        };
  }

  private search(): URLSearchParams {
    const p = new URLSearchParams();
    const map = this.params();
    for (const key of map.keys) for (const v of map.getAll(key)) p.append(key, v);
    return p;
  }
  protected select(threadId: string): void {
    const params = this.search();
    params.set('thread', threadId);
    this.router.navigateByUrl(`${path('staff.conversations')}?${params.toString()}`, { replaceUrl: true });
  }
  protected setKind(kind: string): void {
    const params = this.search();
    if (kind === 'all') params.delete('kind');
    else params.set('kind', kind);
    this.router.navigateByUrl(`${path('staff.conversations')}?${params.toString()}`, { replaceUrl: true });
  }
}

@Component({
  selector: 'lcrb-conversations-page',
  imports: [BCGovHeader, BCGovFooter, LcrbGate, LcrbConversationsContent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header currentPage="/lcrb/conversations" />
      <main class="flex-1">
        <lcrb-gate>
          @if (allowed()) {
            <lcrb-conversations-content />
          }
        </lcrb-gate>
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class LcrbConversationsPage {
  private readonly session = injectSession();
  /** What `LcrbGate` lets through: its content is created only then, as in React. */
  protected readonly allowed = computed(() => !this.session.loading() && this.session.session()?.role === 'lcrb');

}
