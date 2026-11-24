/** Side inspector for the selected card: facts, people, relationships and removal. Record-sourced identity facts are read-only, and everything is disabled when locked by LCRB. */
import { ChangeDetectionStrategy, Component, computed, effect, inject, input, output, signal, untracked } from '@angular/core';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { ScrollArea } from '@shared/ui/primitives/scroll-area';
import { SELECT } from '@shared/ui/primitives/select';
import { Separator } from '@shared/ui/primitives/separator';
import { Textarea } from '@shared/ui/primitives/textarea';
import { ENTITY_META, isPersonType, RELATIONSHIP_META, ROLE_META } from '@domain/legal-entity/legal-entity';
import { type LegalEntityBuild, type LENode, type LERole } from '@domain/models/legal-entity';
import { addPerson, removeNode, removePerson, updateNode } from '../state/legal-entity-store';
import { LICENCES_REPOSITORY } from '@data-access/repositories/licences/licences.repository';

const ADDABLE_ROLES: LERole[] = ['director', 'shareholder', 'key-personnel', 'partner', 'trustee', 'beneficiary', 'officer'];

@Component({
  selector: 'div[leFactRow]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'flex items-start justify-between gap-2' },
  template: `
    <span class="text-sm text-muted-foreground">{{ label() }}</span>
    <span class="flex items-center gap-1 text-right text-sm font-medium text-foreground">
      {{ value() }}
      @if (locked()) {
        <svg lucide="Lock" class="h-3 w-3 text-muted-foreground"></svg>
      }
    </span>
  `,
})
export class FactRow {
  readonly label = input.required<string>();
  readonly value = input.required<string>();
  readonly locked = input.required<boolean>();
}

@Component({
  selector: 'node-inspector',
  imports: [Badge, Button, Input, Label, LucideIcon, ScrollArea, ...SELECT, Separator, Textarea, FactRow],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <aside class="flex h-full w-full flex-col bg-card">
      <!-- Header -->
      <div class="flex items-start justify-between gap-2 border-b px-4 py-3">
        <div class="min-w-0">
          <div class="flex items-center gap-2">
            <span class="h-3 w-3 rounded-sm" [style.background-color]="meta().color"></span>
            <span class="text-xs font-semibold uppercase tracking-wide text-muted-foreground">{{ meta().label }}</span>
          </div>
          <h2 class="mt-1 truncate text-base font-bold text-foreground" [attr.title]="node().legalName">{{ node().legalName }}</h2>
          <div class="mt-1 flex flex-wrap gap-1">
            @if (external()) {
              <span uiBadge class="gap-1 bg-[#0F766E]/15 text-[10px] text-[#0F766E] hover:bg-[#0F766E]/15">
                <svg lucide="Link2" class="h-3 w-3"></svg> From Tied House
              </span>
            } @else if (node().locked) {
              <span uiBadge variant="secondary" class="gap-1 text-[10px]"> <svg lucide="Lock" class="h-3 w-3"></svg> From LCRB records </span>
            } @else {
              <span uiBadge class="bg-amber-500/15 text-[10px] text-amber-700 hover:bg-amber-500/15"> Suggested </span>
            }
          </div>
        </div>
        <button uiButton variant="ghost" size="icon" class="h-7 w-7 shrink-0" (click)="closed.emit()">
          <svg lucide="X" class="h-4 w-4"></svg>
          <span class="sr-only">Close inspector</span>
        </button>
      </div>

      <div uiScrollArea class="flex-1">
        <div class="space-y-5 p-4">
          <!-- Identity -->
          <section class="space-y-2">
            <h3 class="text-xs font-bold uppercase tracking-wide text-muted-foreground">Identity</h3>
            @if (editableCard()) {
              <div class="space-y-1.5">
                <label uiLabel for="insp-name">{{ isPerson() ? 'Full name' : 'Legal name' }}</label>
                <input uiInput id="insp-name" [value]="name()" (input)="name.set($any($event.target).value)" (blur)="commitName()" />
              </div>
            } @else {
              <div leFactRow label="Legal name" [value]="node().legalName" [locked]="node().locked"></div>
            }
            @if (node().businessNumber) {
              <div leFactRow label="Business number" [value]="node().businessNumber!" [locked]="node().locked"></div>
            }
            @if (node().incorporationNumber) {
              <div leFactRow label="Incorporation no." [value]="node().incorporationNumber!" [locked]="node().locked"></div>
            }
            @if (node().jurisdiction) {
              <div leFactRow label="Jurisdiction" [value]="node().jurisdiction!" [locked]="node().locked"></div>
            }
          </section>

          <!-- Linked licences -->
          @if (licences().length > 0) {
            <section class="space-y-2">
              <h3 class="flex items-center gap-1.5 text-xs font-bold uppercase tracking-wide text-muted-foreground">
                Linked licences <svg lucide="Lock" class="h-3 w-3"></svg>
              </h3>
              <ul class="space-y-1">
                @for (l of licences(); track l.id) {
                  <li class="rounded-md border bg-muted/40 px-2.5 py-1.5 text-sm">
                    <span class="font-medium text-foreground">{{ l.id }}</span>
                    <span class="text-muted-foreground"> · {{ l.type }}</span>
                    <div class="text-xs text-muted-foreground">{{ l.name }}</div>
                  </li>
                }
              </ul>
            </section>
          }

          <!-- People -->
          @if (!isPerson()) {
            <section class="space-y-2">
              <h3 class="text-xs font-bold uppercase tracking-wide text-muted-foreground">People</h3>
              @if (node().people.length === 0) {
                <p class="text-sm text-muted-foreground">No people recorded yet.</p>
              }
              <ul class="space-y-1">
                @for (p of node().people; track p.id) {
                  <li class="flex items-center justify-between gap-2 rounded-md border px-2.5 py-1.5">
                    <div class="min-w-0">
                      <div class="truncate text-sm font-medium text-foreground">{{ p.name }}</div>
                      <div class="text-xs text-muted-foreground">{{ roleMeta[p.role] }}</div>
                    </div>
                    @if (p.locked) {
                      <svg lucide="Lock" class="h-3.5 w-3.5 shrink-0 text-muted-foreground" aria-label="Locked"></svg>
                    } @else {
                      @if (!effReadOnly()) {
                        <button
                          uiButton
                          variant="ghost"
                          size="icon"
                          class="h-6 w-6 shrink-0 text-muted-foreground hover:text-destructive"
                          (click)="removePersonFrom(p.id)"
                        >
                          <svg lucide="Trash2" class="h-3.5 w-3.5"></svg>
                          <span class="sr-only">Remove {{ p.name }}</span>
                        </button>
                      }
                    }
                  </li>
                }
              </ul>

              @if (!effReadOnly()) {
                <div class="rounded-md border border-dashed p-2.5">
                  <div class="flex gap-2">
                    <input
                      uiInput
                      [value]="newPersonName()"
                      (input)="newPersonName.set($any($event.target).value)"
                      placeholder="Add a person"
                      class="h-8 text-sm"
                      (keydown)="onPersonKeydown($event)"
                    />
                    <ui-select [value]="newPersonRole()" (valueChange)="newPersonRole.set($any($event))">
                      <button uiSelectTrigger class="h-8 w-[130px] text-sm"><span uiSelectValue></span></button>
                      <ng-template uiSelectContent>
                        @for (r of addableRoles; track r) {
                          <ui-select-item [value]="r">{{ roleMeta[r] }}</ui-select-item>
                        }
                      </ng-template>
                    </ui-select>
                  </div>
                  <button uiButton size="sm" variant="secondary" class="mt-2 w-full gap-1" [disabled]="!newPersonName().trim()" (click)="handleAddPerson()">
                    <svg lucide="Plus" class="h-3.5 w-3.5"></svg> Add person
                  </button>
                </div>
              }
            </section>
          }

          <!-- Relationships -->
          <section class="space-y-2">
            <div class="flex items-center justify-between">
              <h3 class="text-xs font-bold uppercase tracking-wide text-muted-foreground">Relationships</h3>
              @if (!effReadOnly()) {
                <button uiButton variant="ghost" size="sm" class="h-6 gap-1 px-2 text-xs" (click)="addRelationship.emit(node().id)">
                  <svg lucide="Link2" class="h-3.5 w-3.5"></svg> Add
                </button>
              }
            </div>
            @if (relationships().length === 0) {
              <p class="text-sm text-muted-foreground">No relationships yet.</p>
            } @else {
              <ul class="space-y-1">
                @for (e of relationships(); track e.id) {
                  <li class="rounded-md border px-2.5 py-1.5 text-sm">
                    <div class="flex items-center gap-1.5">
                      <span class="h-2 w-2 rounded-full" [style.background-color]="relationshipMeta[e.type].color"></span>
                      <span class="font-medium text-foreground">{{ relationshipMeta[e.type].label }}</span>
                      @if (isNumber(e.sharePercent)) {
                        <span class="text-muted-foreground">· {{ e.sharePercent }}%</span>
                      }
                      @if (e.locked) {
                        <svg lucide="Lock" class="h-3 w-3 text-muted-foreground"></svg>
                      }
                    </div>
                    <div class="text-xs text-muted-foreground">{{ e.source === node().id ? 'to ' : 'from ' }}{{ nameOf(e.source === node().id ? e.target : e.source) }}</div>
                  </li>
                }
              </ul>
            }
          </section>

          <!-- Note -->
          <section class="space-y-2">
            <h3 class="text-xs font-bold uppercase tracking-wide text-muted-foreground">Note for LCRB</h3>
            @if (effReadOnly()) {
              <p class="text-sm text-muted-foreground">{{ node().note || '—' }}</p>
            } @else {
              <textarea
                uiTextarea
                [value]="note()"
                (input)="note.set($any($event.target).value)"
                (blur)="commitNote()"
                placeholder="Add context for the reviewer"
                rows="3"
              ></textarea>
            }
          </section>

          <!-- Remove entity -->
          @if (editableCard() && node().type !== 'applicant') {
            <div uiSeparator></div>
            <button
              uiButton
              variant="outline"
              class="w-full gap-1.5 border-destructive/40 text-destructive hover:bg-destructive/10 hover:text-destructive"
              (click)="removeThisNode()"
            >
              <svg lucide="Trash2" class="h-4 w-4"></svg> Remove this entity
            </button>
          }

          @if (external()) {
            <p class="flex items-start gap-1.5 rounded-md bg-[#0F766E]/10 p-2.5 text-xs text-[#0F766E]">
              <svg lucide="Info" class="mt-0.5 h-3.5 w-3.5 shrink-0"></svg>
              This association is mirrored from your Tied House declarations and is read-only here. To add, change, or remove it, use the Tied House section.
            </p>
          } @else {
            @if (node().locked) {
              <p class="flex items-start gap-1.5 rounded-md bg-muted/50 p-2.5 text-xs text-muted-foreground">
                <svg lucide="Info" class="mt-0.5 h-3.5 w-3.5 shrink-0"></svg>
                This card comes from LCRB systems of record. Its legal name, numbers and linked licences can only be changed by the LCRB.
              </p>
            }
          }
        </div>
      </div>
    </aside>
  `,
})
export class NodeInspector {
  private readonly licencesRepository = inject(LICENCES_REPOSITORY);
  readonly build = input.required<LegalEntityBuild>();
  readonly node = input.required<LENode>();
  readonly readOnly = input.required<boolean>();
  readonly actor = input.required<string>();
  readonly closed = output<void>();
  readonly addRelationship = output<string>();

  protected readonly addableRoles = ADDABLE_ROLES;
  protected readonly roleMeta = ROLE_META;
  protected readonly relationshipMeta = RELATIONSHIP_META;

  protected readonly meta = computed(() => ENTITY_META[this.node().type]);
  // Tied House mirror cards are informational only: read-only everywhere.
  protected readonly external = computed(() => this.node().external === 'tied-house');
  protected readonly effReadOnly = computed(() => this.readOnly() || this.external());
  protected readonly editableCard = computed(() => !this.effReadOnly() && this.node().source === 'portal' && !this.node().locked);
  protected readonly isPerson = computed(() => isPersonType(this.node().type));

  protected readonly name = signal('');
  protected readonly note = signal('');
  protected readonly newPersonName = signal('');
  protected readonly newPersonRole = signal<LERole>('director');

  protected readonly relationships = computed(() =>
    this.build().edges.filter((e) => e.source === this.node().id || e.target === this.node().id),
  );
  protected readonly licences = computed(() =>
    this.node()
      .linkedLicenceIds.map((id) => this.licencesRepository.licence(id))
      .filter((l): l is NonNullable<typeof l> => Boolean(l)),
  );

  // The prototype's effect dependencies: [node.id, node.legalName, node.note].
  private readonly nodeId = computed(() => this.node().id);
  private readonly nodeLegalName = computed(() => this.node().legalName);
  private readonly nodeNote = computed(() => this.node().note);

  constructor() {
    // Reset local edit state when a different card is selected or the store changes.
    effect(() => {
      this.nodeId();
      const legalName = this.nodeLegalName();
      const note = this.nodeNote();
      untracked(() => {
        this.name.set(legalName);
        this.note.set(note ?? '');
        this.newPersonName.set('');
      });
    });
  }

  protected isNumber(v: unknown): v is number {
    return typeof v === 'number';
  }

  protected nameOf(id: string): string {
    return this.build().nodes.find((n) => n.id === id)?.legalName ?? 'Unknown';
  }

  protected commitName(): void {
    const node = this.node();
    if (this.editableCard() && this.name().trim() && this.name().trim() !== node.legalName) {
      updateNode(this.build().id, this.actor(), node.id, { legalName: this.name().trim() });
    }
  }

  protected commitNote(): void {
    const node = this.node();
    if (!this.effReadOnly() && this.note().trim() !== (node.note ?? '')) {
      updateNode(this.build().id, this.actor(), node.id, { note: this.note().trim() || undefined });
    }
  }

  protected handleAddPerson(): void {
    if (this.effReadOnly() || !this.newPersonName().trim()) return;
    addPerson(this.build().id, this.actor(), this.node().id, { name: this.newPersonName().trim(), role: this.newPersonRole() });
    this.newPersonName.set('');
  }

  protected onPersonKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.isComposing) this.handleAddPerson();
  }

  protected removePersonFrom(personId: string): void {
    removePerson(this.build().id, this.actor(), this.node().id, personId);
  }

  protected removeThisNode(): void {
    removeNode(this.build().id, this.actor(), this.node().id);
    this.closed.emit();
  }
}
