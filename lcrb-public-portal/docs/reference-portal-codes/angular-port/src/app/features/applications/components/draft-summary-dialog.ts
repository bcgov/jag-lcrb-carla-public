/** Read-only "View Details" for an in-progress application with a "Resume editing" link; `sections` and `captured` come from mock draft data, so derive them from the persisted draft in production. */
import { ChangeDetectionStrategy, Component, computed, inject, input, output } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Button } from '@shared/ui/primitives/button';
import { DIALOG } from '@shared/ui/primitives/dialog';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { cn } from '@shared/util/utils';

export type DraftSectionStatus = 'complete' | 'in-progress' | 'not-started';

export interface DraftSection {
  name: string;
  status: DraftSectionStatus;
}

export interface DraftCapturedField {
  label: string;
  value: string;
}

export interface DraftSummary {
  id: string;
  type: string;
  reference: string;
  lastModified: string;
  completionPercent: number;
  sections: DraftSection[];
  captured: DraftCapturedField[];
}

const SECTION_META: Record<DraftSectionStatus, { label: string; icon: IconName; className: string }> = {
  complete: { label: 'Complete', icon: 'CheckCircle2', className: 'text-green-600' },
  'in-progress': { label: 'In progress', icon: 'Clock', className: 'text-amber-600' },
  'not-started': { label: 'Not started', icon: 'CircleDashed', className: 'text-muted-foreground' },
};

@Component({
  selector: 'draft-summary-dialog',
  imports: [...DIALOG, Button, LucideIcon, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (draft(); as d) {
      <ui-dialog [state]="open() ? 'open' : 'closed'" (stateChanged)="onState($event)">
        <ui-dialog-content *uiDialogPortal class="max-h-[85vh] overflow-y-auto sm:max-w-lg">
          <div uiDialogHeader>
            <h2 uiDialogTitle class="text-balance">{{ d.type }}</h2>
            <p uiDialogDescription>Job No: {{ d.id }} &middot; {{ d.reference }}</p>
          </div>

          <!-- Read-only notice -->
          <p class="rounded-md bg-muted px-3 py-2 text-sm text-muted-foreground">
            This is a read-only summary of your draft. Select&ngsp;<span class="font-medium text-foreground">Resume editing</span>&ngsp;to
            continue your application.
          </p>

          <!-- Overall progress -->
          <div>
            <div class="mb-1 flex items-center justify-between text-sm">
              <span class="font-medium text-foreground">Overall progress</span>
              <span class="text-muted-foreground">{{ d.completionPercent }}% complete</span>
            </div>
            <div class="h-2 w-full overflow-hidden rounded-full bg-muted">
              <div class="h-full rounded-full bg-bc-blue" [style.width]="d.completionPercent + '%'"></div>
            </div>
            <p class="mt-1 text-xs text-muted-foreground">Last saved {{ lastSaved() }}</p>
          </div>

          <!-- Section breakdown -->
          <div>
            <h3 class="mb-2 text-sm font-semibold text-foreground">Sections</h3>
            <ul class="divide-y rounded-md border">
              @for (section of d.sections; track section.name) {
                @let meta = metaOf[section.status];
                <li class="flex items-center justify-between gap-3 px-3 py-2">
                  <span class="text-sm text-foreground">{{ section.name }}</span>
                  <span [attr.class]="cn('flex items-center gap-1.5 text-xs font-medium', meta.className)">
                    <svg [lucide]="meta.icon" class="h-4 w-4" aria-hidden="true"></svg>
                    {{ meta.label }}
                  </span>
                </li>
              }
            </ul>
          </div>

          <!-- Captured details so far -->
          @if (d.captured.length > 0) {
            <div>
              <h3 class="mb-2 text-sm font-semibold text-foreground">Details entered so far</h3>
              <dl class="grid grid-cols-1 gap-x-4 gap-y-2 rounded-md border p-3 sm:grid-cols-2">
                @for (field of d.captured; track field.label) {
                  <div>
                    <dt class="text-xs text-muted-foreground">{{ field.label }}</dt>
                    <dd class="text-sm text-foreground">{{ field.value }}</dd>
                  </div>
                }
              </dl>
            </div>
          }

          <div uiDialogFooter class="gap-2 sm:gap-2">
            <button uiButton variant="outline" (click)="openChange.emit(false)">Close</button>
            <a uiButton [routerLink]="resumeTree()" class="gap-2 bg-bc-blue text-white hover:bg-bc-blue/90">
              Resume editing
              <svg lucide="ArrowRight" class="h-4 w-4"></svg>
            </a>
          </div>
        </ui-dialog-content>
      </ui-dialog>
    }
  `,
})
export class DraftSummaryDialog {
  readonly draft = input.required<DraftSummary | null>();
  readonly open = input.required<boolean>();
  readonly openChange = output<boolean>();
  /** Where "Resume editing" navigates (the wizard flow for this draft). */
  readonly resumeHref = input.required<string>();

  private readonly router = inject(Router);
  protected readonly cn = cn;
  protected readonly metaOf = SECTION_META;
  protected readonly resumeTree = computed(() => this.router.parseUrl(this.resumeHref()));
  protected readonly lastSaved = computed(() => new Date(this.draft()!.lastModified).toLocaleDateString());

  /** Radix onOpenChange fires only for user dismissal (Escape, close button, overlay). */
  protected onState(state: string): void {
    if (state === 'closed' && this.open()) this.openChange.emit(false);
  }
}
