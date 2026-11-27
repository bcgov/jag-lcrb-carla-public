// Header search palette. Placeholder: no search source is wired yet, so it shows no results.
import { ChangeDetectionStrategy, Component, computed, DestroyRef, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { COMMAND } from '@shared/ui/primitives/command';
import { DIALOG } from '@shared/ui/primitives/dialog';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { type SearchKind, type SearchResult } from '@domain/models/search';

const GROUP_ORDER: SearchKind[] = ['licence', 'application', 'sep', 'lg-referral', 'catalog'];

@Component({
  selector: 'global-search',
  imports: [...DIALOG, ...COMMAND, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <button
      type="button"
      (click)="open.set(true)"
      class="flex items-center gap-2 rounded-md border border-[#dbdcdc] bg-white px-3 py-1.5 text-sm text-[#606060] transition-colors hover:bg-[#f2f2f2] hover:text-[#013366]"
      aria-label="Search the portal"
    >
      <svg lucide="Search" class="h-4 w-4" aria-hidden="true"></svg>
      <span class="hidden lg:inline">Search…</span>
      <kbd class="hidden lg:inline rounded bg-[#f2f2f2] px-1.5 py-0.5 text-xs font-medium text-[#606060]">⌘K</kbd>
    </button>

    <ui-dialog [state]="open() ? 'open' : 'closed'" (stateChanged)="open.set($event === 'open')">
      <ui-dialog-content *uiDialogPortal class="overflow-hidden p-0">
        <div uiDialogHeader class="sr-only">
          <h2 uiDialogTitle>Search the portal</h2>
          <p uiDialogDescription>Search licences, applications, permits, and referrals.</p>
        </div>
        <div uiCommand>
          <ui-command-input placeholder="Search licences, applications, permits, referrals…" [(value)]="query" />
          <div uiCommandList>
            <div uiCommandEmpty>No results found.</div>
            @for (group of grouped(); track group.kind) {
              <div uiCommandGroup [heading]="kindLabel(group.kind)">
                @for (r of group.items; track r.id) {
                  <div uiCommandItem (selected)="go(r.href)" class="flex cursor-pointer flex-col items-start gap-0.5">
                    <span class="text-sm font-medium text-foreground">{{ r.title }}</span>
                    <span class="text-xs text-muted-foreground">{{ r.subtitle }}</span>
                  </div>
                }
              </div>
            }
          </div>
        </div>
      </ui-dialog-content>
    </ui-dialog>
  `,
})
export class GlobalSearch {
  private readonly router = inject(Router);
  protected readonly open = signal(false);
  protected readonly query = signal('');
  protected readonly kindLabel = (kind: SearchKind): string => kind;
  protected readonly results = computed<SearchResult[]>(() => []);

  // Group ranked results by kind while preserving overall order via group order.
  protected readonly grouped = computed(() => {
    const map = new Map<SearchKind, SearchResult[]>();
    for (const r of this.results()) {
      const list = map.get(r.kind) ?? [];
      list.push(r);
      map.set(r.kind, list);
    }
    return GROUP_ORDER.filter((k) => map.has(k)).map((k) => ({ kind: k, items: map.get(k)! }));
  });

  constructor() {
    // Cmd/Ctrl+K toggles the palette.
    const onKey = (e: KeyboardEvent) => {
      if ((e.metaKey || e.ctrlKey) && e.key.toLowerCase() === 'k') {
        e.preventDefault();
        this.open.update((o) => !o);
      }
    };
    document.addEventListener('keydown', onKey);
    inject(DestroyRef).onDestroy(() => document.removeEventListener('keydown', onKey));
  }

  protected go(href: string): void {
    this.open.set(false);
    this.query.set('');
    void this.router.navigateByUrl(href);
  }
}
