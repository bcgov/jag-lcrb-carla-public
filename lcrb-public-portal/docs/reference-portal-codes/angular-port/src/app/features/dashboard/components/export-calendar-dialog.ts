/** Export to calendar with a date-range filter; the initial type set is read once. */
import { ChangeDetectionStrategy, Component, computed, input, signal } from '@angular/core';
import { addMonths, format, parseISO } from 'date-fns';
import { Button } from '@shared/ui/primitives/button';
import { Checkbox } from '@shared/ui/primitives/checkbox';
import { DIALOG } from '@shared/ui/primitives/dialog';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { downloadICS } from '@shared/util/compliance-ics-download';
import { EVENT_META } from '@domain/compliance/compliance-calendar';
import { type CalendarEvent, type CalendarEventType } from '@domain/models/compliance';

type Preset = 'all' | '30' | '90' | '12m' | 'custom';

const PRESETS: { value: Preset; label: string }[] = [
  { value: 'all', label: 'Everything' },
  { value: '30', label: 'Next 30 days' },
  { value: '90', label: 'Next 90 days' },
  { value: '12m', label: 'Next 12 months' },
  { value: 'custom', label: 'Custom range' },
];

const iso = (d: Date) => format(d, 'yyyy-MM-dd');

function startOfDay(d: Date) {
  return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}
function endOfDay(d: Date) {
  return new Date(d.getFullYear(), d.getMonth(), d.getDate(), 23, 59, 59, 999);
}

@Component({
  selector: 'export-calendar-dialog',
  imports: [...DIALOG, Button, Checkbox, Input, Label, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ui-dialog [state]="open() ? 'open' : 'closed'" (stateChanged)="open.set($event === 'open')">
      <button uiButton uiDialogTrigger class="bg-[#013366] hover:bg-[#012a52] gap-2 shrink-0" [disabled]="events().length === 0">
        <svg lucide="Download" class="h-4 w-4" aria-hidden="true"></svg>
        Export to calendar (.ics)
      </button>
      <ui-dialog-content *uiDialogPortal class="sm:max-w-lg">
        <div uiDialogHeader>
          <h2 uiDialogTitle>Export to your calendar</h2>
          <p uiDialogDescription>
            Download an .ics file you can open in Outlook, Google Calendar or Apple Calendar. Choose which dates and types to include.
          </p>
        </div>

        <div class="space-y-5">
          <!-- Range -->
          <fieldset>
            <legend class="mb-2 text-sm font-medium">Date range</legend>
            <div class="flex flex-wrap gap-2" role="radiogroup">
              @for (p of presets; track p.value) {
                <button
                  type="button"
                  role="radio"
                  [attr.aria-checked]="preset() === p.value"
                  (click)="preset.set(p.value)"
                  [attr.class]="
                    'rounded-full border px-3 py-1 text-sm font-medium transition-colors ' +
                    (preset() === p.value ? 'border-[#013366] bg-[#013366] text-white' : 'bg-white text-foreground hover:bg-muted')
                  "
                >
                  {{ p.label }}
                </button>
              }
            </div>
            @if (preset() === 'custom') {
              <div class="mt-3 grid grid-cols-2 gap-3">
                <div class="flex flex-col gap-1.5">
                  <label uiLabel for="export-from">From</label>
                  <input uiInput id="export-from" type="date" [value]="from()" (input)="from.set($any($event.target).value)" />
                </div>
                <div class="flex flex-col gap-1.5">
                  <label uiLabel for="export-to">To</label>
                  <input uiInput id="export-to" type="date" [value]="to()" [min]="from()" (input)="to.set($any($event.target).value)" />
                </div>
                @if (invalidRange()) {
                  <p class="col-span-2 text-sm text-red-700" role="alert">The end date must be on or after the start date.</p>
                }
              </div>
            }
          </fieldset>

          <!-- Types -->
          <fieldset>
            <legend class="mb-2 text-sm font-medium">Include</legend>
            <div class="grid grid-cols-2 gap-2">
              @for (t of typeKeys; track t) {
                <label class="flex items-center gap-2 text-sm">
                  <button uiCheckbox [checked]="types().has(t)" (checkedChange)="toggleType(t)"></button>
                  <span>{{ meta[t].label }} <span class="text-muted-foreground">({{ countFor(t) }})</span></span>
                </label>
              }
            </div>
          </fieldset>

          <p class="text-sm text-muted-foreground" aria-live="polite">
            <span class="font-semibold text-foreground">{{ selected().length }}</span>
            {{ selected().length === 1 ? 'item' : 'items' }} will be exported{{ rangeText() }}.
          </p>
        </div>

        <div uiDialogFooter>
          <button uiButton variant="outline" (click)="open.set(false)">Cancel</button>
          <button uiButton (click)="handleExport()" [disabled]="selected().length === 0 || invalidRange()" class="bg-[#013366] hover:bg-[#012a52] gap-2">
            <svg lucide="Download" class="h-4 w-4" aria-hidden="true"></svg>
            Download .ics
          </button>
        </div>
      </ui-dialog-content>
    </ui-dialog>
  `,
})
export class ExportCalendarDialog {
  readonly events = input.required<CalendarEvent[]>();
  readonly defaultTypes = input.required<CalendarEventType[]>();

  protected readonly presets = PRESETS;
  protected readonly meta = EVENT_META;
  protected readonly typeKeys = Object.keys(EVENT_META) as CalendarEventType[];

  protected readonly open = signal(false);
  protected readonly preset = signal<Preset>('all');
  private readonly today = new Date();
  protected readonly from = signal(iso(this.today));
  protected readonly to = signal(iso(addMonths(this.today, 3)));

  private readonly typesState = signal<Set<CalendarEventType> | undefined>(undefined);
  private initialTypes?: Set<CalendarEventType>;
  protected readonly types = computed(
    () => this.typesState() ?? (this.initialTypes ??= new Set(this.defaultTypes())),
  );

  private readonly range = computed((): { from: Date | null; to: Date | null } => {
    const today = this.today;
    switch (this.preset()) {
      case 'all':
        return { from: null, to: null };
      case '30':
        return { from: today, to: addMonths(today, 1) };
      case '90':
        return { from: today, to: addMonths(today, 3) };
      case '12m':
        return { from: today, to: addMonths(today, 12) };
      case 'custom':
        return { from: this.from() ? parseISO(this.from()) : null, to: this.to() ? parseISO(this.to()) : null };
    }
  });

  protected readonly selected = computed(() => {
    const types = this.types();
    const range = this.range();
    return this.events().filter((e) => {
      if (!types.has(e.type)) return false;
      const d = parseISO(e.date);
      if (range.from && d < startOfDay(range.from)) return false;
      if (range.to && d > endOfDay(range.to)) return false;
      return true;
    });
  });

  protected readonly invalidRange = computed(() => {
    const range = this.range();
    return Boolean(this.preset() === 'custom' && range.from && range.to && range.from > range.to);
  });

  protected readonly rangeText = computed(() => {
    const { from, to } = this.range();
    return from && to ? ` between ${format(from, 'MMM d, yyyy')} and ${format(to, 'MMM d, yyyy')}` : '';
  });

  protected countFor(t: CalendarEventType): number {
    return this.events().filter((e) => e.type === t).length;
  }

  protected toggleType(t: CalendarEventType): void {
    const next = new Set(this.types());
    if (next.has(t)) next.delete(t);
    else next.add(t);
    this.typesState.set(next);
  }

  protected handleExport(): void {
    const range = this.range();
    const suffix = this.preset() === 'all' ? 'all' : `${iso(range.from ?? this.today)}_to_${iso(range.to ?? this.today)}`;
    downloadICS(this.selected(), `lcrb-compliance-calendar-${suffix}.ics`);
    this.open.set(false);
  }
}
