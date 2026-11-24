/** Existing Tied House relationships, with change and remove (zero-fee) actions. */
import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { formatDate } from '@domain/tied-house/tied-house';
import { type ExistingRelationship } from '@domain/models/tied-house';
import { cn } from '@shared/util/utils';

@Component({
  selector: 'existing-relationships-list',
  imports: [Badge, Button, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (relationships().length === 0) {
      <p class="rounded-md border border-dashed border-border bg-muted/30 px-4 py-6 text-center text-sm text-muted-foreground">
        No existing tied house relationships on file.
      </p>
    } @else {
      <ul class="space-y-2">
        @for (r of relationships(); track r.id) {
          <li class="flex flex-col gap-3 rounded-lg border border-border bg-card p-4 sm:flex-row sm:items-center sm:justify-between">
            <div class="flex min-w-0 items-start gap-3">
              <span class="flex h-9 w-9 shrink-0 items-center justify-center rounded-md bg-muted text-muted-foreground">
                @if (r.type === 'individual') {
                  <svg lucide="User" class="h-4 w-4"></svg>
                } @else {
                  <svg lucide="Building2" class="h-4 w-4"></svg>
                }
              </span>
              <div class="min-w-0">
                <p class="truncate text-sm font-medium text-foreground">{{ r.displayName }}</p>
                <p class="truncate text-xs text-muted-foreground">{{ r.relationshipLabel }} · {{ r.associatedEstablishmentName }} (#{{ r.associatedLicenceNumber }})</p>
                <p class="mt-0.5 text-xs text-muted-foreground">Date tied: {{ formatDate(r.dateTied) }}</p>
              </div>
            </div>

            <div class="flex shrink-0 items-center gap-2">
              <span
                uiBadge
                variant="secondary"
                [class]="cn(r.domain === 'cannabis' ? 'bg-emerald-500/15 text-emerald-700' : 'bg-[#013366]/10 text-[#013366]')"
                >{{ r.domain === 'cannabis' ? 'Cannabis' : 'Liquor' }}</span
              >
              <button uiButton variant="outline" size="sm" (click)="changed.emit(r)" [disabled]="disabled()" class="gap-1.5">
                <svg lucide="Pencil" class="h-3.5 w-3.5"></svg> Change
              </button>
              <button uiButton variant="outline" size="sm" (click)="removed.emit(r)" [disabled]="disabled()" class="gap-1.5 text-[#D8292F]">
                <svg lucide="Trash2" class="h-3.5 w-3.5"></svg> Remove
              </button>
            </div>
          </li>
        }
      </ul>
    }
  `,
})
export class ExistingRelationshipsList {
  readonly relationships = input.required<ExistingRelationship[]>();
  readonly disabled = input<boolean | undefined>(undefined);
  readonly changed = output<ExistingRelationship>();
  readonly removed = output<ExistingRelationship>();

  protected readonly cn = cn;
  protected readonly formatDate = formatDate;
}
