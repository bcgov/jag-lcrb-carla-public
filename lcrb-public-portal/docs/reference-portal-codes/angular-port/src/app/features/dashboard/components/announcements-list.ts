import { afterNextRender, ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { type IconName } from '@shared/ui/primitives/icons';
import { CATEGORY_LABELS } from '@domain/notifications/announcements';
import { type AnnouncementCategory, type ChangelogEntry } from '@domain/models/notifications';
import { NOTIFICATIONS_REPOSITORY } from '@data-access/repositories/notifications/notifications.repository';

const CATEGORY_STYLES: Record<AnnouncementCategory, string> = {
  policy: 'bg-[#013366]/10 text-[#013366]',
  regulatory: 'bg-[#9F2241]/10 text-[#9F2241]',
  release: 'bg-[#2E8540]/10 text-[#1E5631]',
  service: 'bg-[#F2A900]/15 text-[#8A6100]',
};

const CHANGELOG_META: Record<ChangelogEntry['type'], { label: string; className: string; icon: IconName }> = {
  added: { label: 'Added', className: 'text-[#1E5631]', icon: 'Plus' },
  improved: { label: 'Improved', className: 'text-[#013366]', icon: 'ArrowUp' },
  fixed: { label: 'Fixed', className: 'text-[#8A6100]', icon: 'Wrench' },
  changed: { label: 'Changed', className: 'text-[#9F2241]', icon: 'RefreshCw' },
};

function formatDate(iso: string) {
  try {
    return new Date(iso).toLocaleDateString('en-CA', { year: 'numeric', month: 'long', day: 'numeric' });
  } catch {
    return iso;
  }
}

@Component({
  selector: 'announcements-list',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="mx-auto max-w-3xl px-4 py-8 sm:py-12">
      <ol class="space-y-5">
        @for (a of announcements; track a.id) {
          <li>
            <article class="overflow-hidden rounded-xl border border-border bg-card shadow-sm">
              <div class="p-5 sm:p-6">
                <div class="flex flex-wrap items-center gap-2">
                  <span [attr.class]="'rounded-full px-2.5 py-0.5 text-xs font-semibold ' + categoryStyles[a.category]">
                    {{ categoryLabels[a.category] }}
                  </span>
                  @if (a.version) {
                    <span class="rounded-full bg-muted px-2.5 py-0.5 font-mono text-xs font-medium text-muted-foreground">
                      v{{ a.version }}
                    </span>
                  }
                  @if (a.id === latestId) {
                    <span class="rounded-full bg-[#D8292F] px-2.5 py-0.5 text-xs font-semibold text-white">
                      Latest
                    </span>
                  }
                  <time class="ml-auto text-xs text-muted-foreground" [attr.datetime]="a.date">
                    {{ formatDate(a.date) }}
                  </time>
                </div>

                <h2 class="mt-3 text-balance text-lg font-semibold text-foreground">{{ a.title }}</h2>
                <p class="mt-1.5 leading-relaxed text-muted-foreground">{{ a.summary }}</p>

                <div class="mt-4 space-y-3 text-sm leading-relaxed text-foreground">
                  @for (p of a.body; track $index) {
                    <p>{{ p }}</p>
                  }
                </div>

                @if (a.changelog && a.changelog.length > 0) {
                  <div class="mt-5 rounded-lg border border-border bg-muted/30 p-4">
                    <h3 class="text-xs font-semibold uppercase tracking-wide text-muted-foreground">
                      Changelog
                    </h3>
                    <ul class="mt-3 space-y-2">
                      @for (entry of a.changelog; track $index) {
                        @let meta = changelogMeta[entry.type];
                        <li class="flex items-start gap-2.5 text-sm">
                          <span [attr.class]="'mt-0.5 flex shrink-0 items-center gap-1 font-medium ' + meta.className">
                            <svg [lucide]="meta.icon" class="h-3.5 w-3.5" aria-hidden="true"></svg>
                            {{ meta.label }}
                          </span>
                          <span class="text-foreground">{{ entry.text }}</span>
                        </li>
                      }
                    </ul>
                  </div>
                }
              </div>
            </article>
          </li>
        }
      </ol>
    </div>
  `,
})
export class AnnouncementsList {
  private readonly notificationsRepository = inject(NOTIFICATIONS_REPOSITORY);
  protected readonly announcements = this.notificationsRepository.announcements();
  protected readonly categoryStyles = CATEGORY_STYLES;
  protected readonly categoryLabels = CATEGORY_LABELS;
  protected readonly changelogMeta = CHANGELOG_META;
  protected readonly formatDate = formatDate;
  protected readonly latestId = this.notificationsRepository.latestAnnouncementId();

  private readonly unread = this.notificationsRepository.unreadAnnouncements();

  constructor() {
    // Viewing the page marks all current announcements as seen, clearing the red indicator in the navigation.
    afterNextRender(() => this.unread().markAllSeen());
  }
}
