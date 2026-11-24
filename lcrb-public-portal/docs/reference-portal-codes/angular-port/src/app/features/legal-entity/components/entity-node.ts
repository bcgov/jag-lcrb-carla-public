/** Custom ngx-vflow card nodes: EntityNode (company-style) and PersonNode (individual). Display-only; editing happens in the inspector. */
import { ChangeDetectionStrategy, Component, computed, inject, output } from '@angular/core';
import { CustomNodeComponent, Vflow } from 'ngx-vflow';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { ENTITY_META, isPersonType, ROLE_META } from '@domain/legal-entity/legal-entity';
import { type LENode, type LERole } from '@domain/models/legal-entity';
import { LICENCES_REPOSITORY } from '@data-access/repositories/licences/licences.repository';

export interface LENodeData {
  node: LENode;
  readOnly: boolean;
  [key: string]: unknown;
}

/** Order roles are displayed in within a card. */
const ROLE_ORDER: LERole[] = ['director', 'shareholder', 'partner', 'trustee', 'beneficiary', 'key-personnel', 'officer'];

function groupPeople(node: LENode) {
  const groups = new Map<LERole, string[]>();
  for (const p of node.people) {
    const list = groups.get(p.role) ?? [];
    list.push(p.name);
    groups.set(p.role, list);
  }
  return ROLE_ORDER.filter((r) => groups.has(r)).map((role) => ({
    role,
    names: groups.get(role)!,
  }));
}

/** Shared handle drawing: the prototype's `!h-2.5 !w-2.5 !border-2 !border-white !bg-[#38598A]`. */
const HANDLE_TEMPLATE = `
  <ng-template #handleTpl let-ctx>
    <svg:circle
      r="4"
      fill="#38598A"
      stroke="#ffffff"
      stroke-width="2"
      [attr.cx]="ctx.point().x"
      [attr.cy]="ctx.point().y"
      [style.pointer-events]="connectable() ? 'all' : 'none'"
      [style.cursor]="connectable() ? 'crosshair' : null"
    />
  </ng-template>
  <handle type="target" position="top" id="t" [template]="handleTpl" />
  <handle type="target" position="left" id="l" [template]="handleTpl" />
  <handle type="source" position="bottom" id="b" [template]="handleTpl" />
  <handle type="source" position="right" id="r" [template]="handleTpl" />
`;

@Component({
  selector: 'le-entity-node',
  imports: [Vflow, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: block' },
  template: `
    <div
      [class]="rootClass()"
      [style.cursor]="cursor()"
      (click)="nodeClick.emit()"
    >
      ${HANDLE_TEMPLATE}

      <!-- Header -->
      <div class="flex items-start justify-between gap-2 px-3 py-2 text-white" [style.background-color]="meta().color">
        <div class="min-w-0">
          <p class="text-[10px] font-semibold uppercase tracking-wide opacity-90">{{ meta().label }}</p>
          <p class="truncate text-sm font-bold leading-tight" [attr.title]="entity().legalName">{{ entity().legalName }}</p>
        </div>
        @if (external()) {
          <span title="Mirrored from Tied House declarations — read-only">
            <svg lucide="Link2" class="mt-0.5 h-3.5 w-3.5 shrink-0 opacity-90" aria-label="From Tied House"></svg>
          </span>
        } @else {
          @if (entity().locked) {
            <span title="From LCRB records — not editable">
              <svg lucide="Lock" class="mt-0.5 h-3.5 w-3.5 shrink-0 opacity-90" aria-label="Locked identity"></svg>
            </span>
          }
        }
      </div>
      @if (external()) {
        <p class="bg-[#0F766E]/10 px-3 py-1 text-[10px] font-semibold uppercase tracking-wide text-[#0F766E]">Tied House association</p>
      }

      <!-- Identity facts -->
      <div class="space-y-1 border-b px-3 py-2 text-xs text-muted-foreground">
        @if (entity().businessNumber) {
          <div class="flex items-center gap-1.5">
            <svg lucide="IdCard" class="h-3.5 w-3.5 shrink-0"></svg>
            <span>BN {{ entity().businessNumber }}</span>
          </div>
        }
        @if (entity().incorporationNumber && entity().incorporationNumber !== entity().businessNumber) {
          <div class="flex items-center gap-1.5">
            <svg lucide="IdCard" class="h-3.5 w-3.5 shrink-0"></svg>
            <span>Inc. {{ entity().incorporationNumber }}</span>
          </div>
        }
        @if (licences().length > 0) {
          <div class="flex flex-wrap gap-1 pt-0.5">
            @for (l of licences(); track l.id) {
              <span
                class="inline-flex items-center gap-1 rounded bg-[#013366]/10 px-1.5 py-0.5 text-[10px] font-medium text-[#013366]"
                [attr.title]="l.type + ' — ' + l.name"
              >
                <svg lucide="FileText" class="h-3 w-3"></svg>
                {{ l.id }}
              </span>
            }
          </div>
        }
      </div>

      <!-- People grouped by role (Directors / Shareholders / Key Personnel ...) -->
      <div class="p-2">
        @if (groups().length === 0) {
          <p class="px-1 py-1 text-[11px] italic text-muted-foreground">No people recorded yet</p>
        } @else {
          <div class="grid grid-cols-3 gap-1.5">
            @for (g of groups(); track g.role) {
              <div class="rounded-md bg-muted/60 p-1.5">
                <p class="mb-0.5 text-[9px] font-bold uppercase tracking-wide text-muted-foreground">{{ roleMeta[g.role] }}</p>
                <ul class="space-y-0.5">
                  @for (name of g.names; track $index) {
                    <li class="truncate text-[11px] leading-tight text-foreground" [attr.title]="name">{{ name }}</li>
                  }
                </ul>
              </div>
            }
          </div>
        }
      </div>
    </div>
  `,
})
export class EntityNode extends CustomNodeComponent<LENodeData> {
  private readonly licencesRepository = inject(LICENCES_REPOSITORY);
  readonly nodeClick = output<void>();
  protected readonly roleMeta = ROLE_META;

  protected readonly entity = computed(() => this.data()!.node);
  protected readonly meta = computed(() => ENTITY_META[this.entity().type]);
  protected readonly groups = computed(() => groupPeople(this.entity()));
  protected readonly licences = computed(() =>
    this.entity()
      .linkedLicenceIds.map((id) => this.licencesRepository.licence(id))
      .filter((l): l is NonNullable<typeof l> => Boolean(l)),
  );
  protected readonly external = computed(() => this.entity().external === 'tied-house');
  protected readonly connectable = computed(() => !this.data()!.readOnly);
  protected readonly cursor = computed(() => (this.entity().external || this.data()!.readOnly ? 'pointer' : 'grab'));
  protected readonly rootClass = computed(
    () =>
      `w-[300px] overflow-hidden rounded-lg border bg-card shadow-sm transition-shadow ${
        this.selected() ? 'ring-2 ring-[#013366] ring-offset-2' : 'hover:shadow-md'
      } ${this.external() ? 'border-dashed border-[#0F766E]/60' : ''}`,
  );
}

@Component({
  selector: 'le-person-node',
  imports: [Vflow],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: block' },
  template: `
    <div
      [class]="rootClass()"
      [style.cursor]="cursor()"
      (click)="nodeClick.emit()"
    >
      ${HANDLE_TEMPLATE}

      <div class="flex items-center gap-2 px-3 py-2 text-white" [style.background-color]="meta().color">
        <div class="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-white/20 text-xs font-bold" aria-hidden="true">
          {{ initials() }}
        </div>
        <div class="min-w-0">
          <p class="text-[10px] font-semibold uppercase tracking-wide opacity-90">{{ meta().label }}</p>
          <p class="truncate text-sm font-bold leading-tight" [attr.title]="entity().legalName">{{ entity().legalName }}</p>
        </div>
      </div>
      @if (external()) {
        <p class="bg-[#0F766E]/10 px-3 py-1 text-[10px] font-semibold uppercase tracking-wide text-[#0F766E]">Tied House association</p>
      }
      @if (entity().note) {
        <p class="px-3 py-1.5 text-[11px] leading-snug text-muted-foreground">{{ entity().note }}</p>
      }
    </div>
  `,
})
export class PersonNode extends CustomNodeComponent<LENodeData> {
  readonly nodeClick = output<void>();

  protected readonly entity = computed(() => this.data()!.node);
  protected readonly meta = computed(() => ENTITY_META[this.entity().type]);
  protected readonly external = computed(() => this.entity().external === 'tied-house');
  protected readonly connectable = computed(() => !this.data()!.readOnly);
  protected readonly cursor = computed(() => (this.entity().external || this.data()!.readOnly ? 'pointer' : 'grab'));
  protected readonly initials = computed(() =>
    this.entity()
      .legalName.split(' ')
      .map((w) => w[0])
      .slice(0, 2)
      .join(''),
  );
  protected readonly rootClass = computed(
    () =>
      `w-[210px] overflow-hidden rounded-lg border bg-card shadow-sm transition-shadow ${
        this.selected() ? 'ring-2 ring-[#013366] ring-offset-2' : 'hover:shadow-md'
      } ${this.external() ? 'border-dashed border-[#0F766E]/60' : ''}`,
  );
}

/** Choose the canvas node component for an LE node (the prototype's `rfNodeKind` + `nodeTypes`). */
export function rfNodeKind(node: LENode): 'entity' | 'person' {
  return isPersonType(node.type) ? 'person' : 'entity';
}

/** Node type registry, keyed by our data-driven node kind. */
export const nodeTypes = {
  entity: EntityNode,
  person: PersonNode,
};
