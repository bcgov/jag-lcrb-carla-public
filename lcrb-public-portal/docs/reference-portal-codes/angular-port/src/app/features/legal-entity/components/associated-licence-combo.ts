/** Search combo matching a 6-digit licence number or an establishment name; selecting a result fixes the associated licence. Bind the id as a property so it is not also written on the host. */
import { ChangeDetectionStrategy, Component, computed, ElementRef, inject, input, output, signal } from '@angular/core';
import { Badge } from '@shared/ui/primitives/badge';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { type AssociableLicence } from '@domain/models/tied-house';
import { cn } from '@shared/util/utils';
import { TIED_HOUSE_REPOSITORY } from '@data-access/repositories/tied-house/tied-house.repository';

@Component({
  selector: 'div[associatedLicenceCombo]',
  imports: [Badge, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class]': 'hostClass()',
    '(document:mousedown)': 'onDocClick($event)',
  },
  template: `
    @if (selected(); as sel) {
      <div class="flex min-w-0 items-center gap-2.5">
        <span class="flex h-8 w-8 shrink-0 items-center justify-center rounded-md bg-[#38598A]/10 text-[#38598A]">
          <svg lucide="Building2" class="h-4 w-4"></svg>
        </span>
        <div class="min-w-0">
          <p class="truncate text-sm font-medium text-foreground">{{ sel.establishmentName }}</p>
          <p class="truncate text-xs text-muted-foreground">#{{ sel.number }} · {{ sel.type }} · {{ sel.operatedBy }}</p>
        </div>
      </div>
      <div class="flex shrink-0 items-center gap-2">
        <span
          uiBadge
          variant="secondary"
          [class]="cn(sel.domain === 'cannabis' ? 'bg-emerald-500/15 text-emerald-700' : 'bg-[#013366]/10 text-[#013366]')"
          >{{ sel.domain === 'cannabis' ? 'Cannabis' : 'Liquor' }}</span
        >
        @if (!disabled()) {
          <button
            type="button"
            (click)="changed.emit(null)"
            class="rounded p-1 text-muted-foreground hover:bg-muted hover:text-foreground"
            aria-label="Clear selected licence"
          >
            <svg lucide="X" class="h-4 w-4"></svg>
          </button>
        }
      </div>
    } @else {
      <div class="relative">
        <svg lucide="Search" class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"></svg>
        <input
          [attr.id]="id()"
          type="text"
          [disabled]="disabled()"
          [value]="query()"
          (focus)="open.set(true)"
          (input)="onInput($event)"
          placeholder="Search by licence number (6 digits) or establishment name…"
          class="flex h-10 w-full rounded-md border border-input bg-background pl-10 pr-3 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
        />
      </div>

      @if (open()) {
        <div class="absolute z-20 mt-1 max-h-64 w-full overflow-y-auto rounded-md border border-border bg-popover p-1 shadow-lg">
          @if (results().length === 0) {
            <p class="px-3 py-6 text-center text-sm text-muted-foreground">No matching liquor or cannabis licence found.</p>
          } @else {
            @for (r of results(); track r.number) {
              <button
                type="button"
                (click)="pick(r)"
                class="flex w-full items-center justify-between gap-3 rounded-sm px-3 py-2 text-left hover:bg-muted"
              >
                <div class="min-w-0">
                  <p class="truncate text-sm font-medium text-foreground">{{ r.establishmentName }}</p>
                  <p class="truncate text-xs text-muted-foreground">#{{ r.number }} · {{ r.type }} · {{ r.operatedBy }}</p>
                </div>
                <span
                  [class]="
                    cn(
                      'shrink-0 rounded px-1.5 py-0.5 text-[10px] font-medium',
                      r.domain === 'cannabis' ? 'bg-emerald-500/15 text-emerald-700' : 'bg-[#013366]/10 text-[#013366]'
                    )
                  "
                  >{{ r.domain === 'cannabis' ? 'Cannabis' : 'Liquor' }}</span
                >
              </button>
            }
          }
        </div>
      }
    }
  `,
})
export class AssociatedLicenceCombo {
  private readonly tiedHouseRepository = inject(TIED_HOUSE_REPOSITORY);
  readonly value = input.required<string>();
  readonly changed = output<AssociableLicence | null>();
  readonly disabled = input<boolean | undefined>(undefined);
  readonly id = input<string | undefined>(undefined);

  protected readonly cn = cn;
  protected readonly query = signal('');
  protected readonly open = signal(false);

  protected readonly selected = computed(() => (this.value() ? this.tiedHouseRepository.associableLicence(this.value()) : undefined));
  protected readonly results = computed(() => this.tiedHouseRepository.searchAssociableLicences(this.query()));
  protected readonly hostClass = computed(() =>
    this.selected() ? 'flex items-center justify-between gap-3 rounded-md border border-border bg-muted/40 px-3 py-2.5' : 'relative',
  );

  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef).nativeElement;

  protected onDocClick(event: MouseEvent): void {
    // The prototype's wrapRef exists only while no licence is selected.
    if (!this.selected() && !this.host.contains(event.target as Node)) this.open.set(false);
  }

  protected onInput(event: Event): void {
    this.query.set((event.target as HTMLInputElement).value);
    this.open.set(true);
  }

  protected pick(r: AssociableLicence): void {
    this.changed.emit(r);
    this.open.set(false);
    this.query.set('');
  }
}
