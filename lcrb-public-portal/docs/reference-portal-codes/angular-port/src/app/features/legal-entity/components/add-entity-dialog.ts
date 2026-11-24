/** Dialog for adding a card: search registry records (locked, record-sourced) or add a new entity as a portal suggestion. */
import { ChangeDetectionStrategy, Component, computed, effect, type ElementRef, inject, input, output, signal, untracked, viewChild } from '@angular/core';
import { Button } from '@shared/ui/primitives/button';
import { DIALOG } from '@shared/ui/primitives/dialog';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SELECT } from '@shared/ui/primitives/select';
import { Textarea } from '@shared/ui/primitives/textarea';
import type { DirectoryResult, DirectoryScope, DirectorySource } from '@server-lib/data/entity-directory';
import { ENTITY_META, isPersonType } from '@domain/legal-entity/legal-entity';
import { type LEEntityType } from '@domain/models/legal-entity';
import type { DirectoryBusinessInput, NewNodeInput } from '../state/legal-entity-store';
import { cn } from '@shared/util/utils';
import { LICENCES_REPOSITORY } from '@data-access/repositories/licences/licences.repository';
import { LEGAL_ENTITY_REPOSITORY } from '@data-access/repositories/legal-entity/legal-entity.repository';

/** Local copy of SOURCE_META: importing the server-only directory module would ship the whole mock directory. Keep in sync with the original. */
export const SOURCE_META: Record<DirectorySource, { label: string; short: string }> = {
  "bc-registry": { label: "BC Registry", short: "Registry" },
  dynamics: { label: "LCRB Dynamics", short: "Dynamics" },
};

type Mode = 'search' | 'new';

const SCOPES: { value: DirectoryScope; label: string }[] = [
  { value: 'all', label: 'All' },
  { value: 'business', label: 'Businesses' },
  { value: 'person', label: 'People' },
];

@Component({
  selector: 'add-entity-dialog',
  imports: [...DIALOG, Button, Input, Label, LucideIcon, ...SELECT, Textarea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ui-dialog [state]="open() ? 'open' : 'closed'" (stateChanged)="onState($event)">
      <ui-dialog-content *uiDialogPortal class="sm:max-w-lg">
        <div uiDialogHeader>
          <h2 uiDialogTitle>Add to your structure</h2>
          <p uiDialogDescription>
            Search BC Registry and LCRB records for a business or person, or add an entity that isn't registered yet as a suggestion for LCRB review.
          </p>
        </div>

        <!-- Mode toggle -->
        <div class="grid grid-cols-2 gap-2">
          <button type="button" (click)="mode.set('search')" [class]="modeClass('search')">
            <svg lucide="Search" class="h-4 w-4 shrink-0"></svg>
            <span class="font-medium">Search records</span>
          </button>
          <button type="button" (click)="mode.set('new')" [class]="modeClass('new')">
            <svg lucide="Plus" class="h-4 w-4 shrink-0"></svg>
            <span class="font-medium">New entity</span>
          </button>
        </div>

        @if (mode() === 'search') {
          <div class="space-y-3 py-1">
            <!-- Privacy notice -->
            <div class="flex items-start gap-2 rounded-md border border-[#013366]/20 bg-[#013366]/5 px-3 py-2 text-xs text-[#013366]">
              <svg lucide="ShieldCheck" class="mt-0.5 h-4 w-4 shrink-0"></svg>
              <p>Only limited registry details are shown so you can build the connections. No addresses or personal contact information are disclosed.</p>
            </div>

            <div class="relative">
              <svg lucide="Search" class="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"></svg>
              <input
                #searchInput
                uiInput
                [value]="query()"
                (input)="query.set($any($event.target).value)"
                placeholder="Search by business name, number, or person…"
                class="pl-10"
              />
            </div>

            <div class="flex gap-1.5">
              @for (s of scopes; track s.value) {
                <button type="button" (click)="scope.set(s.value)" [class]="scopeClass(s.value)">{{ s.label }}</button>
              }
            </div>

            <div class="max-h-72 space-y-1.5 overflow-y-auto">
              @if (loading()) {
                <div class="flex items-center justify-center gap-2 py-8 text-sm text-muted-foreground">
                  <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
                  Searching registries…
                </div>
              } @else if (results().length === 0) {
                <p class="rounded-md border border-dashed bg-muted/40 px-3 py-8 text-center text-sm text-muted-foreground">
                  No matches. Try a different name or number, or use <span class="font-medium text-foreground">New entity</span>.
                </p>
              } @else {
                @for (r of results(); track r.id) {
                  <div class="flex items-start gap-3 rounded-md border bg-card px-3 py-2.5">
                    <div
                      [class]="
                        cn(
                          'mt-0.5 flex h-8 w-8 shrink-0 items-center justify-center rounded-md',
                          r.kind === 'business' ? 'bg-[#38598A]/10 text-[#38598A]' : 'bg-[#64748B]/10 text-[#64748B]'
                        )
                      "
                    >
                      @if (r.kind === 'business') {
                        <svg lucide="Building2" class="h-4 w-4"></svg>
                      } @else {
                        <svg lucide="User" class="h-4 w-4"></svg>
                      }
                    </div>

                    <div class="min-w-0 flex-1">
                      <p class="truncate text-sm font-medium">{{ r.kind === 'business' ? r.legalName : r.name }}</p>
                      <p class="truncate text-xs text-muted-foreground">
                        {{ r.kind === 'business' ? entityMeta[r.entityType].short + ' · ' + r.registrationNumber + ' · ' + r.status : r.descriptor }}
                      </p>
                      <div class="mt-1 flex flex-wrap items-center gap-1">
                        @for (s of r.sources; track s) {
                          <span class="rounded bg-muted px-1.5 py-0.5 text-[10px] font-medium uppercase tracking-wide text-muted-foreground">{{ sourceMeta[s].label }}</span>
                        }
                        @if (r.kind === 'business') {
                          @for (lid of r.licenceNumbers; track lid) {
                            <span class="rounded bg-[#2E7D57]/10 px-1.5 py-0.5 text-[10px] font-medium text-[#2E7D57]">{{ lid }}{{ licenceSuffix(lid) }}</span>
                          }
                        }
                      </div>
                    </div>

                    <button
                      uiButton
                      size="sm"
                      [variant]="isAdded(r) ? 'outline' : 'default'"
                      [disabled]="isAdded(r)"
                      (click)="handleAdd(r)"
                      [class]="cn('shrink-0', !isAdded(r) && 'bg-[#013366] hover:bg-[#013366]/90')"
                    >
                      @if (isAdded(r)) {
                        <svg lucide="Check" class="h-3.5 w-3.5"></svg> Added
                      } @else {
                        Add
                      }
                    </button>
                  </div>
                }
              }
            </div>
          </div>
        } @else {
          <div class="space-y-4 py-1">
            <div class="space-y-1.5">
              <label uiLabel for="ent-type">Entity type</label>
              <ui-select [value]="type()" (valueChange)="type.set($any($event))">
                <button uiSelectTrigger id="ent-type"><span uiSelectValue></span></button>
                <ng-template uiSelectContent>
                  @for (entry of entityTypeEntries; track entry.key) {
                    <ui-select-item [value]="entry.key">{{ entry.meta.label }}</ui-select-item>
                  }
                </ng-template>
              </ui-select>
            </div>

            <div class="space-y-1.5">
              <label uiLabel for="ent-name">{{ person() ? 'Full name' : 'Legal / registered name' }}</label>
              <input
                uiInput
                id="ent-name"
                [value]="legalName()"
                (input)="legalName.set($any($event.target).value)"
                [placeholder]="person() ? 'e.g. Jane Doe' : 'e.g. Example Holdings Ltd.'"
              />
            </div>

            @if (!person()) {
              <div class="grid grid-cols-2 gap-3">
                <div class="space-y-1.5">
                  <label uiLabel for="ent-bn">Business number</label>
                  <input uiInput id="ent-bn" [value]="businessNumber()" (input)="businessNumber.set($any($event.target).value)" placeholder="Optional" />
                </div>
                <div class="space-y-1.5">
                  <label uiLabel for="ent-inc">Incorporation no.</label>
                  <input uiInput id="ent-inc" [value]="incorporationNumber()" (input)="incorporationNumber.set($any($event.target).value)" placeholder="Optional" />
                </div>
              </div>
            }

            <div class="flex items-start gap-2 rounded-md border bg-muted/30 px-3 py-2 text-xs text-muted-foreground">
              <svg lucide="Lock" class="mt-0.5 h-3.5 w-3.5 shrink-0"></svg>
              <p>Entities you add manually are treated as suggestions. The LCRB confirms them against the system of record in Dynamics.</p>
            </div>

            <div class="space-y-1.5">
              <label uiLabel for="ent-note">Note for LCRB (optional)</label>
              <textarea
                uiTextarea
                id="ent-note"
                [value]="note()"
                (input)="note.set($any($event.target).value)"
                placeholder="Explain this entity's role in the structure"
                rows="2"
              ></textarea>
            </div>
          </div>
        }

        <div uiDialogFooter>
          @if (mode() === 'search') {
            <button uiButton (click)="openChange.emit(false)" class="bg-[#013366] hover:bg-[#013366]/90">
              {{ added().size > 0 ? 'Done · ' + added().size + ' added' : 'Done' }}
            </button>
          } @else {
            <button uiButton variant="outline" (click)="openChange.emit(false)">Cancel</button>
            <button uiButton (click)="handleCreate()" [disabled]="!newValid()" class="bg-[#013366] hover:bg-[#013366]/90">Add entity</button>
          }
        </div>
      </ui-dialog-content>
    </ui-dialog>
  `,
})
export class AddEntityDialog {
  private readonly legalEntityRepository = inject(LEGAL_ENTITY_REPOSITORY);
  private readonly licencesRepository = inject(LICENCES_REPOSITORY);
  readonly open = input.required<boolean>();
  readonly openChange = output<boolean>();
  readonly create = output<NewNodeInput>();
  /** Business/registration numbers already on the canvas (for de-duplication). */
  readonly existingBusinessNumbers = input.required<string[]>();
  /** Individual names already on the canvas (for de-duplication). */
  readonly existingIndividualNames = input.required<string[]>();
  readonly linkBusiness = output<DirectoryBusinessInput>();
  readonly linkIndividual = output<string>();

  protected readonly cn = cn;
  protected readonly scopes = SCOPES;
  protected readonly entityMeta = ENTITY_META;
  protected readonly sourceMeta = SOURCE_META;
  protected readonly entityTypeEntries = Object.entries(ENTITY_META)
    .filter(([key]) => key !== 'applicant')
    .map(([key, meta]) => ({ key, meta }));

  protected readonly mode = signal<Mode>('search');

  protected readonly query = signal('');
  protected readonly scope = signal<DirectoryScope>('all');
  protected readonly results = signal<DirectoryResult[]>([]);
  protected readonly loading = signal(false);
  // Locally-added ids this session (so the row shows "Added" immediately).
  protected readonly added = signal<Set<string>>(new Set());

  protected readonly type = signal<LEEntityType>('private-corporation');
  protected readonly legalName = signal('');
  protected readonly businessNumber = signal('');
  protected readonly incorporationNumber = signal('');
  protected readonly jurisdiction = signal('British Columbia');
  protected readonly note = signal('');

  protected readonly person = computed(() => isPersonType(this.type()));
  protected readonly newValid = computed(() => this.legalName().trim().length > 0);

  private readonly searchInput = viewChild<ElementRef<HTMLInputElement>>('searchInput');
  private req = 0;

  constructor() {
    effect(() => {
      if (!this.open()) return;
      untracked(() => {
        this.mode.set('search');
        this.query.set('');
        this.scope.set('all');
        this.results.set([]);
        this.added.set(new Set());
        this.type.set('private-corporation');
        this.legalName.set('');
        this.businessNumber.set('');
        this.incorporationNumber.set('');
        this.jurisdiction.set('British Columbia');
        this.note.set('');
      });
    });

    // Debounced directory lookup against the BC Registry / Dynamics API.
    effect((onCleanup) => {
      if (!this.open() || this.mode() !== 'search') return;
      const query = this.query();
      const scope = this.scope();
      const id = ++this.req;
      this.loading.set(true);
      const t = setTimeout(async () => {
        try {
          const params = new URLSearchParams({ q: query, scope });
          const res = await this.legalEntityRepository.fetchDirectory(params.toString());
          const data = (await res.json()) as { results: DirectoryResult[] };
          if (id === this.req) this.results.set(data.results);
        } catch {
          if (id === this.req) this.results.set([]);
        } finally {
          if (id === this.req) this.loading.set(false);
        }
      }, 250);
      onCleanup(() => clearTimeout(t));
    });

    // React's autoFocus on the search box.
    effect(() => this.searchInput()?.nativeElement.focus());
  }

  /** Radix onOpenChange fires only for user dismissal (Escape, close button, overlay). */
  protected onState(state: string): void {
    if (state === 'closed' && this.open()) this.openChange.emit(false);
  }

  protected modeClass(mode: Mode): string {
    return cn(
      'flex items-center gap-2 rounded-md border px-3 py-2 text-left text-sm transition-colors',
      this.mode() === mode ? 'border-[#013366] bg-[#013366]/5 text-[#013366]' : 'border-border text-muted-foreground hover:bg-muted',
    );
  }

  protected scopeClass(value: DirectoryScope): string {
    return cn(
      'rounded-full border px-3 py-1 text-xs font-medium transition-colors',
      this.scope() === value ? 'border-[#013366] bg-[#013366] text-white' : 'border-border text-muted-foreground hover:bg-muted',
    );
  }

  protected licenceSuffix(id: string): string {
    const lic = this.licencesRepository.licence(id);
    return lic ? ` · ${lic.type}` : '';
  }

  protected isAdded(r: DirectoryResult): boolean {
    if (this.added().has(r.id)) return true;
    if (r.kind === 'business') return this.existingBusinessNumbers().includes(r.registrationNumber);
    return this.existingIndividualNames().some((n) => n.toLowerCase() === r.name.toLowerCase());
  }

  protected handleAdd(r: DirectoryResult): void {
    if (r.kind === 'business') {
      this.linkBusiness.emit({
        type: r.entityType,
        legalName: r.legalName,
        registrationNumber: r.registrationNumber,
        jurisdiction: r.jurisdiction,
        linkedLicenceIds: r.licenceNumbers,
      });
    } else {
      this.linkIndividual.emit(r.name);
    }
    this.added.update((prev) => new Set(prev).add(r.id));
  }

  protected handleCreate(): void {
    if (!this.newValid()) return;
    const person = this.person();
    this.create.emit({
      type: this.type(),
      legalName: this.legalName().trim(),
      businessNumber: person ? undefined : this.businessNumber().trim(),
      incorporationNumber: person ? undefined : this.incorporationNumber().trim(),
      jurisdiction: this.jurisdiction().trim(),
      note: this.note().trim(),
    });
    this.openChange.emit(false);
  }
}
