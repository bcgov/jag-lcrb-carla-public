/** Tied House declaration box, Individual or Legal Entity variant; Save locks it and reveals the payment panel, Cancel discards the draft. */
import { ChangeDetectionStrategy, Component, computed, input, type OnInit, output, signal } from '@angular/core';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SELECT } from '@shared/ui/primitives/select';
import { Textarea } from '@shared/ui/primitives/textarea';
import { BUSINESS_TYPES, businessTypeLabel, CHANGE_TYPE_LABEL, INDIVIDUAL_RELATIONSHIPS, LEGAL_ENTITY_RELATIONSHIPS, relationshipLabel, requiredDocsFor } from '@domain/tied-house/tied-house';
import { type AssociableLicence, type TiedHouseDeclaration, type TiedHouseType } from '@domain/models/tied-house';
import type { SaveDeclarationInput } from '../state/tied-house-store';
import { cn } from '@shared/util/utils';
import { AssociatedLicenceCombo } from './associated-licence-combo';

@Component({
  selector: 'div[thField]',
  imports: [Label],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'space-y-1.5' },
  template: `
    <label uiLabel [attr.for]="htmlFor()"
      >{{ label() }}
      @if (required()) {
        <span class="text-[#D8292F]">*</span>
      }
    </label>
    <ng-content />
    @if (hint()) {
      <p class="text-xs text-muted-foreground">{{ hint() }}</p>
    }
  `,
})
export class DeclarationField {
  readonly label = input.required<string>();
  readonly required = input<boolean | undefined>(undefined);
  readonly htmlFor = input.required<string>();
  readonly hint = input<string | undefined>(undefined);
}

@Component({
  selector: 'div[thRow]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'flex flex-col' },
  template: `
    <dt class="text-xs uppercase tracking-wide text-muted-foreground">{{ label() }}</dt>
    <dd class="font-medium text-foreground">{{ value() }}</dd>
  `,
})
export class DeclarationRow {
  readonly label = input.required<string>();
  readonly value = input.required<string>();
}


@Component({
  selector: 'draft-declaration',
  imports: [Button, Input, LucideIcon, ...SELECT, Textarea, AssociatedLicenceCombo, DeclarationField],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (declaration().changeType === 'change-details') {
      <div class="mb-4 flex items-start gap-2 rounded-md border border-amber-300 bg-amber-50 px-3 py-2 text-sm text-amber-900">
        <svg lucide="AlertCircle" class="mt-0.5 h-4 w-4 shrink-0"></svg>
        <p>You are changing the details of an existing tied house relationship.</p>
      </div>
    }

    <fieldset [disabled]="disabled()">
      <legend class="mb-2 text-sm font-medium text-foreground">Select Tied House Type</legend>
      <div class="grid grid-cols-2 gap-2">
        @for (opt of typeOptions; track opt.value) {
          <button type="button" (click)="type.set(opt.value)" [class]="typeClass(opt.value)">
            <svg [lucide]="opt.icon" class="h-4 w-4 shrink-0"></svg>
            <span class="font-medium">{{ opt.label }}</span>
          </button>
        }
      </div>

      <!-- Variant fields -->
      <div class="mt-4 space-y-4">
        @if (type() === 'individual') {
          <div class="grid gap-4 sm:grid-cols-2">
            <div thField label="Date of Birth" [required]="true" htmlFor="th-dob" hint="DD/MM/YYYY">
              <input uiInput id="th-dob" [value]="dob()" (input)="dob.set($any($event.target).value)" placeholder="DD/MM/YYYY" inputmode="numeric" />
            </div>
          </div>
          <div class="grid gap-4 sm:grid-cols-3">
            <div thField label="First Name" [required]="true" htmlFor="th-first">
              <input uiInput id="th-first" [value]="firstName()" (input)="firstName.set($any($event.target).value)" />
            </div>
            <div thField label="Middle Name" htmlFor="th-middle">
              <input uiInput id="th-middle" [value]="middleName()" (input)="middleName.set($any($event.target).value)" />
            </div>
            <div thField label="Last Name" [required]="true" htmlFor="th-last">
              <input uiInput id="th-last" [value]="lastName()" (input)="lastName.set($any($event.target).value)" />
            </div>
          </div>
          <div thField label="Relationship to Declared Licence" [required]="true" htmlFor="th-ind-rel">
            <ui-select [value]="indRel()" (valueChange)="indRel.set($event ?? '')">
              <button uiSelectTrigger id="th-ind-rel"><span uiSelectValue placeholder="Select a relationship"></span></button>
              <ng-template uiSelectContent>
                @for (o of individualRelationships; track o.value) {
                  <ui-select-item [value]="o.value">{{ o.label }}</ui-select-item>
                }
              </ng-template>
            </ui-select>
          </div>
        } @else {
          <div class="grid gap-4 sm:grid-cols-2">
            <div thField label="Business Type" [required]="true" htmlFor="th-biztype">
              <ui-select [value]="businessType()" (valueChange)="businessType.set($event ?? '')">
                <button uiSelectTrigger id="th-biztype"><span uiSelectValue placeholder="Select a business type"></span></button>
                <ng-template uiSelectContent>
                  @for (o of businessTypes; track o.value) {
                    <ui-select-item [value]="o.value">{{ o.label }}</ui-select-item>
                  }
                </ng-template>
              </ui-select>
            </div>
            <div thField label="Relationship Type" [required]="true" htmlFor="th-ent-rel">
              <ui-select [value]="entRel()" (valueChange)="entRel.set($event ?? '')">
                <button uiSelectTrigger id="th-ent-rel"><span uiSelectValue placeholder="Select a relationship type"></span></button>
                <ng-template uiSelectContent>
                  @for (o of legalEntityRelationships; track o.value) {
                    <ui-select-item [value]="o.value">{{ o.label }}</ui-select-item>
                  }
                </ng-template>
              </ui-select>
            </div>
          </div>
          <div thField label="Legal Entity Name" [required]="true" htmlFor="th-entname">
            <input
              uiInput
              id="th-entname"
              [value]="entityName()"
              (input)="entityName.set($any($event.target).value)"
              placeholder="e.g. Example Holdings Ltd."
            />
          </div>
        }

        @if (needsOther()) {
          <div thField label="Describe the relationship" [required]="true" htmlFor="th-rel-other">
            <textarea
              uiTextarea
              id="th-rel-other"
              rows="2"
              [value]="relOther()"
              (input)="relOther.set($any($event.target).value)"
              placeholder="Describe the nature of the association"
            ></textarea>
          </div>
        }

        <div
          thField
          label="Associated Liquor Licence # or Establishment Name"
          [required]="true"
          htmlFor="th-assoc"
          hint="Search by 6-digit licence number or establishment name"
        >
          <div associatedLicenceCombo [id]="'th-assoc'" [value]="licence()?.number ?? ''" (changed)="licence.set($event)"></div>
        </div>

        @if (previewDocs().length > 0) {
          <div class="rounded-md border border-[#013366]/20 bg-[#013366]/5 px-3 py-2.5 text-xs text-[#013366]">
            <p class="flex items-center gap-1.5 font-medium">
              <svg lucide="FileText" class="h-3.5 w-3.5"></svg> Saving this declaration will require:
            </p>
            <ul class="mt-1.5 list-disc space-y-0.5 pl-5">
              @for (d of previewDocs(); track d.id) {
                <li>{{ d.label }}</li>
              }
            </ul>
          </div>
        }
      </div>
    </fieldset>

    <!-- Actions -->
    <div class="mt-5 flex flex-wrap items-center justify-end gap-2">
      <button uiButton variant="outline" (click)="cancelled.emit()" [disabled]="disabled()">Cancel</button>
      <button uiButton (click)="handleSave()" [disabled]="disabled() || !valid()" class="bg-[#013366] hover:bg-[#013366]/90">Save declaration</button>
    </div>
  `,
})
export class DraftDeclaration implements OnInit {
  readonly declaration = input.required<TiedHouseDeclaration>();
  readonly disabled = input<boolean | undefined>(undefined);
  readonly saved = output<SaveDeclarationInput>();
  readonly cancelled = output<void>();

  protected readonly typeOptions = [
    { value: 'individual' as const, label: 'Individual', icon: 'User' as const },
    { value: 'legal-entity' as const, label: 'Legal Entity', icon: 'Building2' as const },
  ];
  protected readonly individualRelationships = INDIVIDUAL_RELATIONSHIPS;
  protected readonly legalEntityRelationships = LEGAL_ENTITY_RELATIONSHIPS;
  protected readonly businessTypes = BUSINESS_TYPES;

  // The prototype's useState initialisers read the declaration once, on mount.
  protected readonly type = signal<TiedHouseType>('individual');

  // Individual fields
  protected readonly dob = signal('');
  protected readonly firstName = signal('');
  protected readonly middleName = signal('');
  protected readonly lastName = signal('');
  protected readonly indRel = signal('');

  // Legal-entity fields
  protected readonly businessType = signal('');
  protected readonly entityName = signal('');
  protected readonly entRel = signal('');

  // Shared
  protected readonly relOther = signal('');
  protected readonly licence = signal<AssociableLicence | null>(null);

  protected readonly relationship = computed(() => (this.type() === 'individual' ? this.indRel() : this.entRel()));
  protected readonly needsOther = computed(() => this.relationship() === 'other');

  protected readonly valid = computed(() => {
    if (!this.licence()) return false;
    if (this.needsOther() && !this.relOther().trim()) return false;
    if (this.type() === 'individual') {
      return Boolean(this.dob().trim() && this.firstName().trim() && this.lastName().trim() && this.indRel());
    }
    return Boolean(this.businessType() && this.entityName().trim() && this.entRel());
  });

  protected readonly previewDocs = computed(() => {
    const licence = this.licence();
    return licence ? requiredDocsFor(this.type(), licence.domain) : [];
  });

  /** Initial state comes from the declaration, read once (the prototype's useState initialisers). */
  ngOnInit(): void {
    const d = this.declaration();
    this.type.set(d.type);
    this.dob.set(d.individual?.dateOfBirth ?? '');
    this.firstName.set(d.individual?.firstName ?? '');
    this.middleName.set(d.individual?.middleName ?? '');
    this.lastName.set(d.individual?.lastName ?? '');
    this.indRel.set(d.individual?.relationship ?? '');
    this.businessType.set(d.entity?.businessType ?? '');
    this.entityName.set(d.entity?.legalEntityName ?? '');
    this.entRel.set(d.entity?.relationshipType ?? '');
    this.relOther.set(d.individual?.relationshipOther ?? d.entity?.relationshipOther ?? '');
  }

  protected typeClass(value: TiedHouseType): string {
    return cn(
      'flex items-center gap-2 rounded-md border px-3 py-2.5 text-left text-sm transition-colors',
      this.type() === value ? 'border-[#013366] bg-[#013366]/5 text-[#013366]' : 'border-border text-muted-foreground hover:bg-muted',
    );
  }

  protected handleSave(): void {
    const licence = this.licence();
    if (!this.valid() || !licence) return;
    const type = this.type();
    const needsOther = this.needsOther();
    this.saved.emit({
      type,
      individual:
        type === 'individual'
          ? {
              dateOfBirth: this.dob().trim(),
              firstName: this.firstName().trim(),
              middleName: this.middleName().trim() || undefined,
              lastName: this.lastName().trim(),
              relationship: this.indRel(),
              relationshipOther: needsOther ? this.relOther().trim() : undefined,
            }
          : undefined,
      entity:
        type === 'legal-entity'
          ? {
              businessType: this.businessType(),
              legalEntityName: this.entityName().trim(),
              relationshipType: this.entRel(),
              relationshipOther: needsOther ? this.relOther().trim() : undefined,
            }
          : undefined,
      associatedLicenceNumber: licence.number,
      associatedEstablishmentName: licence.establishmentName,
      domain: licence.domain,
    });
  }
}


@Component({
  selector: 'saved-declaration',
  imports: [Badge, Button, LucideIcon, DeclarationRow],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex items-start justify-between gap-3">
      <div class="flex items-center gap-2">
        <span class="flex h-8 w-8 items-center justify-center rounded-md bg-[#013366]/10 text-[#013366]">
          @if (declaration().type === 'individual') {
            <svg lucide="User" class="h-4 w-4"></svg>
          } @else {
            <svg lucide="Building2" class="h-4 w-4"></svg>
          }
        </span>
        <div>
          <p class="text-sm font-semibold text-foreground">{{ name() || 'Declaration' }}</p>
          <p class="flex items-center gap-1.5 text-xs text-muted-foreground">
            <svg lucide="Lock" class="h-3 w-3"></svg> Saved · {{ changeTypeLabel[declaration().changeType] }}
          </p>
        </div>
      </div>
      <span
        uiBadge
        variant="secondary"
        [class]="cn(declaration().domain === 'cannabis' ? 'bg-emerald-500/15 text-emerald-700' : 'bg-[#013366]/10 text-[#013366]')"
        >{{ declaration().domain === 'cannabis' ? 'Cannabis' : 'Liquor' }}</span
      >
    </div>

    <dl class="mt-4 grid gap-x-6 gap-y-2 text-sm sm:grid-cols-2">
      @if (declaration().type === 'individual' && declaration().individual?.dateOfBirth) {
        <div thRow label="Date of Birth" [value]="declaration().individual!.dateOfBirth"></div>
      }
      @if (declaration().type === 'legal-entity' && declaration().entity?.businessType) {
        <div thRow label="Business Type" [value]="businessTypeLabel(declaration().entity!.businessType)"></div>
      }
      <div thRow label="Relationship" [value]="rel() || '—'"></div>
      <div thRow label="Associated licence" [value]="declaration().associatedEstablishmentName + ' · #' + declaration().associatedLicenceNumber"></div>
    </dl>

    @if (declaration().requiredDocs.length > 0) {
      <div class="mt-4 rounded-md border border-border bg-card px-3 py-2.5">
        <p class="flex items-center gap-1.5 text-xs font-medium text-foreground">
          <svg lucide="FileText" class="h-3.5 w-3.5"></svg> Supporting documents required
        </p>
        <ul class="mt-1.5 space-y-1 text-xs text-muted-foreground">
          @for (d of declaration().requiredDocs; track d.id) {
            <li>
              <span class="font-medium text-foreground">{{ d.label }}</span> — {{ d.description }}
            </li>
          }
        </ul>
      </div>
    }

    <div class="mt-4 flex justify-end">
      <button uiButton variant="outline" size="sm" (click)="removed.emit()" [disabled]="disabled()" class="gap-1.5 text-[#D8292F]">
        <svg lucide="Trash2" class="h-4 w-4"></svg> Remove
      </button>
    </div>
  `,
})
export class SavedDeclaration {
  readonly declaration = input.required<TiedHouseDeclaration>();
  readonly disabled = input<boolean | undefined>(undefined);
  readonly removed = output<void>();

  protected readonly cn = cn;
  protected readonly changeTypeLabel = CHANGE_TYPE_LABEL;
  protected readonly businessTypeLabel = businessTypeLabel;

  protected readonly rel = computed(() => {
    const d = this.declaration();
    return d.type === 'individual'
      ? d.individual?.relationship === 'other'
        ? d.individual?.relationshipOther
        : relationshipLabel('individual', d.individual?.relationship ?? '')
      : d.entity?.relationshipType === 'other'
        ? d.entity?.relationshipOther
        : relationshipLabel('legal-entity', d.entity?.relationshipType ?? '');
  });

  protected readonly name = computed(() => {
    const d = this.declaration();
    return d.type === 'individual'
      ? [d.individual?.firstName, d.individual?.middleName, d.individual?.lastName].filter(Boolean).join(' ')
      : d.entity?.legalEntityName;
  });
}


@Component({
  selector: 'div[declarationBox]',
  imports: [DraftDeclaration, SavedDeclaration],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[class]': 'hostClass()' },
  template: `
    @if (declaration().status === 'saved') {
      <saved-declaration [declaration]="declaration()" [disabled]="disabled()" (removed)="removed.emit()" />
    } @else {
      <draft-declaration [declaration]="declaration()" [disabled]="disabled()" (saved)="saved.emit($event)" (cancelled)="cancelled.emit()" />
    }
  `,
})
export class DeclarationBox {
  readonly declaration = input.required<TiedHouseDeclaration>();
  readonly disabled = input<boolean | undefined>(undefined);
  readonly saved = output<SaveDeclarationInput>();
  readonly cancelled = output<void>();
  readonly removed = output<void>();

  protected readonly hostClass = computed(() =>
    this.declaration().status === 'saved'
      ? 'rounded-lg border border-border bg-muted/40 p-4 sm:p-5'
      : 'rounded-lg border border-border bg-card p-4 sm:p-5',
  );
}
