/** Dialog for proposing a relationship between two entity cards; source and target are pre-filled when opened by dragging a connection. */
import { ChangeDetectionStrategy, Component, computed, effect, input, output, signal, untracked } from '@angular/core';
import { Button } from '@shared/ui/primitives/button';
import { DIALOG } from '@shared/ui/primitives/dialog';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { SELECT } from '@shared/ui/primitives/select';
import { RELATIONSHIP_META } from '@domain/legal-entity/legal-entity';
import { type LegalEntityBuild, type LERelationshipType } from '@domain/models/legal-entity';

const SHARE_TYPES: LERelationshipType[] = ['shareholding', 'partnership'];

@Component({
  selector: 'relationship-dialog',
  imports: [...DIALOG, Button, Input, Label, ...SELECT],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ui-dialog [state]="open() ? 'open' : 'closed'" (stateChanged)="onState($event)">
      <ui-dialog-content *uiDialogPortal class="sm:max-w-md">
        <div uiDialogHeader>
          <h2 uiDialogTitle>Add a relationship</h2>
          <p uiDialogDescription>Connect two entities. Relationships you add are suggestions until the LCRB confirms them.</p>
        </div>

        <div class="space-y-4 py-2">
          <div class="space-y-1.5">
            <label uiLabel for="rel-source">From (owner / parent)</label>
            <ui-select [value]="source()" (valueChange)="source.set($event ?? '')">
              <button uiSelectTrigger id="rel-source"><span uiSelectValue placeholder="Select an entity"></span></button>
              <ng-template uiSelectContent>
                @for (n of build().nodes; track n.id) {
                  <ui-select-item [value]="n.id">{{ n.legalName }}</ui-select-item>
                }
              </ng-template>
            </ui-select>
          </div>

          <div class="space-y-1.5">
            <label uiLabel for="rel-type">Relationship</label>
            <ui-select [value]="type()" (valueChange)="type.set($any($event))">
              <button uiSelectTrigger id="rel-type"><span uiSelectValue></span></button>
              <ng-template uiSelectContent>
                @for (entry of relationshipEntries; track entry.key) {
                  <ui-select-item [value]="entry.key">{{ entry.meta.label }}</ui-select-item>
                }
              </ng-template>
            </ui-select>
          </div>

          <div class="space-y-1.5">
            <label uiLabel for="rel-target">To</label>
            <ui-select [value]="target()" (valueChange)="target.set($event ?? '')">
              <button uiSelectTrigger id="rel-target"><span uiSelectValue placeholder="Select an entity"></span></button>
              <ng-template uiSelectContent>
                @for (n of targetNodes(); track n.id) {
                  <ui-select-item [value]="n.id">{{ n.legalName }}</ui-select-item>
                }
              </ng-template>
            </ui-select>
          </div>

          @if (showShare()) {
            <div class="space-y-1.5">
              <label uiLabel for="rel-share">Ownership percentage (optional)</label>
              <div class="relative">
                <input
                  uiInput
                  id="rel-share"
                  type="number"
                  min="0"
                  max="100"
                  [value]="sharePercent()"
                  (input)="sharePercent.set($any($event.target).value)"
                  placeholder="e.g. 50"
                  class="pr-7"
                />
                <span class="pointer-events-none absolute right-3 top-1/2 -translate-y-1/2 text-sm text-muted-foreground"> % </span>
              </div>
            </div>
          }

          @if (source() && target() === source()) {
            <p class="text-sm text-destructive">Choose two different entities.</p>
          }
        </div>

        <div uiDialogFooter>
          <button uiButton variant="outline" (click)="openChange.emit(false)">Cancel</button>
          <button uiButton (click)="handleSubmit()" [disabled]="!valid()" class="bg-[#013366] hover:bg-[#013366]/90">Add relationship</button>
        </div>
      </ui-dialog-content>
    </ui-dialog>
  `,
})
export class RelationshipDialog {
  readonly open = input.required<boolean>();
  readonly openChange = output<boolean>();
  readonly build = input.required<LegalEntityBuild>();
  readonly defaultSource = input<string | null | undefined>(undefined);
  readonly defaultTarget = input<string | null | undefined>(undefined);
  readonly create = output<{
    source: string;
    target: string;
    type: LERelationshipType;
    sharePercent?: number;
    label?: string;
  }>();

  protected readonly relationshipEntries = Object.entries(RELATIONSHIP_META).map(([key, meta]) => ({ key, meta }));
  protected readonly source = signal('');
  protected readonly target = signal('');
  protected readonly type = signal<LERelationshipType>('shareholding');
  protected readonly sharePercent = signal('');

  protected readonly showShare = computed(() => SHARE_TYPES.includes(this.type()));
  protected readonly valid = computed(() => Boolean(this.source() && this.target() && this.source() !== this.target()));
  protected readonly targetNodes = computed(() => this.build().nodes.filter((n) => n.id !== this.source()));

  constructor() {
    effect(() => {
      const open = this.open();
      const defaultSource = this.defaultSource();
      const defaultTarget = this.defaultTarget();
      if (open) {
        untracked(() => {
          this.source.set(defaultSource ?? '');
          this.target.set(defaultTarget ?? '');
          this.type.set('shareholding');
          this.sharePercent.set('');
        });
      }
    });
  }

  /** Radix onOpenChange fires only for user dismissal (Escape, close button, overlay). */
  protected onState(state: string): void {
    if (state === 'closed' && this.open()) this.openChange.emit(false);
  }

  protected handleSubmit(): void {
    if (!this.valid()) return;
    this.create.emit({
      source: this.source(),
      target: this.target(),
      type: this.type(),
      sharePercent: this.showShare() && this.sharePercent() ? Number(this.sharePercent()) : undefined,
    });
    this.openChange.emit(false);
  }
}
