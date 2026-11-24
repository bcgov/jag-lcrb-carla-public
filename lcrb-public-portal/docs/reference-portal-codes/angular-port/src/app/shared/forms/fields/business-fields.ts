/** Business-bound fields tied to the BCeID business: locked auto-fill, licence and establishment pickers, and helpers deciding which business field a config becomes. */
import { ChangeDetectionStrategy, Component, computed, effect, input, untracked } from "@angular/core"
import { Input } from "@shared/ui/primitives/input"
import { LucideIcon } from "@shared/ui/primitives/lucide-icon"
import { SELECT } from "@shared/ui/primitives/select"
import { injectBusinessProfile } from "@core/session/business-profile"
import { resolveEntityTypeOption } from "@domain/legal-entity/entity-type"
import { type BusinessProfile } from "@domain/models/account"
import { cn } from "@shared/util/utils"
import type { FieldValidation } from "@domain/forms/field-validation"
import { proseHelpsForType, type FieldConfig, type Option } from "@domain/forms/field-schema"
import { FormField } from "../layout"
import { injectApplicationContext } from "../provider"
import { GUIDED_OPTION_CLASS } from "./choice-fields"
import { FieldShell, injectField } from "./field-binding"

/** Read-only field whose value comes from the BCeID business account; written into the form data but not editable. */
@Component({
  selector: "div[readonlyAutoFieldF]",
  imports: [FormField, Input, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div formField [label]="label()" [hint]="effectiveHint()" [htmlFor]="name()">
      <div class="relative">
        <input
          uiInput
          [attr.id]="name()"
          [value]="value()"
          readonly
          aria-readonly="true"
          tabindex="-1"
          class="bg-muted text-muted-foreground cursor-not-allowed pr-9"
        />
        <svg lucide="Lock" class="pointer-events-none absolute right-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"></svg>
      </div>
    </div>
  `,
})
export class ReadonlyAutoFieldF extends FieldShell {
  readonly name = input.required<string>()
  readonly label = input.required<string>()
  readonly value = input.required<string>()
  /** Value written to the form when it differs from the displayed text (e.g. a select option value versus its label); defaults to `value`. */
  readonly storedValue = input<string | undefined>(undefined)
  readonly hint = input<string | undefined>(undefined)
  private readonly ctx = injectApplicationContext()
  // The prototype's parameter default: applies when `hint` is undefined.
  protected readonly effectiveHint = computed(
    () => this.hint() ?? "Auto-filled from your BCeID business account · cannot be changed",
  )
  private readonly persisted = computed(() => this.storedValue() ?? this.value())

  constructor() {
    super()
    effect(() => {
      const name = this.name()
      const persisted = this.persisted()
      untracked(() => {
        if (persisted && this.ctx.formData()[name] !== persisted) this.ctx.updateFormData({ [name]: persisted })
      })
    })
  }
}

/** Licence-number picker from a preset list; selecting a licence also fills the matching establishment name. */
@Component({
  selector: "div[licenceSelectFieldF]",
  imports: [FormField, SELECT],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div formField [label]="label()" [required]="required()" [hint]="hint()" [error]="f.error()" [htmlFor]="name()">
      <ui-select [value]="f.value()" (valueChange)="choose($event ?? '')">
        <button uiSelectTrigger [attr.id]="name()" [attr.aria-invalid]="f.error() ? 'true' : 'false'" [class]="triggerClass()">
          <span uiSelectValue [placeholder]="placeholder()"></span>
        </button>
        <ng-template uiSelectContent [class]="guidedClass">
          @for (o of options(); track o.value) {
            <ui-select-item [value]="o.value">{{ o.label }}</ui-select-item>
          }
        </ng-template>
      </ui-select>
    </div>
  `,
})
export class LicenceSelectFieldF extends FieldShell {
  readonly name = input.required<string>()
  readonly label = input.required<string>()
  readonly required = input<boolean | undefined>(undefined)
  readonly hint = input<string | undefined>(undefined)
  readonly placeholder = input("Select one of your licences")
  readonly validation = input<FieldValidation | undefined>(undefined)
  private readonly profile = injectBusinessProfile()
  private readonly ctx = injectApplicationContext()
  protected readonly f = injectField(() => ({
    name: this.name(),
    rules: this.validation(),
    required: this.required(),
    label: this.label(),
  }))

  protected readonly options = computed(() => {
    const options: Option[] = this.profile().licences.map((l) => ({
      value: l.number,
      label: `${l.number} — ${l.establishmentName} (${l.type})`,
    }))
    // Keep a pre-filled or historical value selectable even if not in the list.
    const value = this.f.value()
    if (value && !options.some((o) => o.value === value)) {
      options.unshift({ value, label: value })
    }
    return options
  })
  protected readonly guidedClass = GUIDED_OPTION_CLASS
  protected triggerClass = () => cn(this.f.error() && "border-destructive")

  protected choose(v: string): void {
    const licence = this.profile().licences.find((l) => l.number === v)
    this.ctx.updateFormData(
      licence ? { [this.name()]: v, establishmentName: licence.establishmentName } : { [this.name()]: v },
    )
    this.f.onBlur()
  }
}

/** Establishment-name picker from the business's establishments, preset rather than free text. */
@Component({
  selector: "div[establishmentSelectFieldF]",
  imports: [FormField, SELECT],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div formField [label]="label()" [required]="required()" [hint]="hint()" [error]="f.error()" [htmlFor]="name()">
      <ui-select [value]="f.value()" (valueChange)="choose($event ?? '')">
        <button uiSelectTrigger [attr.id]="name()" [attr.aria-invalid]="f.error() ? 'true' : 'false'" [class]="triggerClass()">
          <span uiSelectValue [placeholder]="placeholder()"></span>
        </button>
        <ng-template uiSelectContent [class]="guidedClass">
          @for (o of options(); track o.value) {
            <ui-select-item [value]="o.value">{{ o.label }}</ui-select-item>
          }
        </ng-template>
      </ui-select>
    </div>
  `,
})
export class EstablishmentSelectFieldF extends FieldShell {
  readonly name = input.required<string>()
  readonly label = input.required<string>()
  readonly required = input<boolean | undefined>(undefined)
  readonly hint = input<string | undefined>(undefined)
  readonly placeholder = input("Select an establishment")
  readonly validation = input<FieldValidation | undefined>(undefined)
  private readonly profile = injectBusinessProfile()
  protected readonly f = injectField(() => ({
    name: this.name(),
    rules: this.validation(),
    required: this.required(),
    label: this.label(),
  }))

  protected readonly options = computed(() => {
    const seen = new Set<string>()
    const options: Option[] = []
    for (const l of this.profile().licences) {
      if (seen.has(l.establishmentName)) continue
      seen.add(l.establishmentName)
      options.push({ value: l.establishmentName, label: l.establishmentName })
    }
    const value = this.f.value()
    if (value && !options.some((o) => o.value === value)) {
      options.unshift({ value, label: value })
    }
    return options
  })
  protected readonly guidedClass = GUIDED_OPTION_CLASS
  protected triggerClass = () => cn(this.f.error() && "border-destructive")

  protected choose(v: string): void {
    this.f.set(v)
    this.f.onBlur()
  }
}


/** Field names tied to the BCeID business, auto-filled and read-only. */
export const BUSINESS_READONLY_FIELDS: Record<string, "name" | "number" | "incorporation"> = {
  legalEntityName: "name",
  legalName: "name",
  businessName: "name",
  legalBusinessName: "name",
  bceidBusinessName: "name",
  businessNumber: "number",
  bceidBusinessNumber: "number",
  incorporationNumber: "incorporation",
}

/** Legal-entity type fields; each form has its own option spellings, so the canonical value is resolved against them. */
export const ENTITY_TYPE_FIELDS = new Set(["entityType", "businessType", "legalEntityType"])

export const LICENCE_SELECT_FIELDS = new Set(["licenceNumber", "licenseNumber"])

export interface ReadonlyAutoProps {
  name: string
  label: string
  value: string
  storedValue?: string
  hint?: string
}

export function businessReadonlyProps(
  profile: BusinessProfile,
  field: FieldConfig,
  kind: "name" | "number" | "incorporation",
): ReadonlyAutoProps {
  const value =
    kind === "name"
      ? profile.legalName
      : kind === "number"
        ? profile.bceidBusinessNumber
        : profile.incorporationNumber

  // An unincorporated entity has no incorporation number; say why it is blank instead of showing an empty locked box.
  if (kind === "incorporation" && !value) {
    return {
      name: field.name,
      label: field.label,
      value: "Not applicable",
      hint: "Your BCeID business account is not an incorporated entity, so it has no incorporation number.",
    }
  }

  return { name: field.name, label: field.label, value }
}

/** Legal-entity type auto-filled from BC Registries; null when it cannot be matched to the form's options, so the editable select renders instead of a locked wrong answer. */
export function entityTypeProps(profile: BusinessProfile, field: FieldConfig): ReadonlyAutoProps | null {
  const resolved = resolveEntityTypeOption(profile.legalEntityType, field.options ?? [])
  if (!resolved) return null
  const label = field.options?.find((o) => o.value === resolved)?.label ?? resolved
  return {
    name: field.name,
    label: field.label,
    value: label,
    // Store the option value, not the label, so the payload matches the form's option set.
    storedValue: resolved,
    hint: "Auto-filled from your BC Registries record · cannot be changed",
  }
}

/** True when a value is chosen from preset account or licence data, so Guided Mode skips its "describe it in your own words" box. */
export function isPresetField(field: FieldConfig): boolean {
  return (
    Boolean(BUSINESS_READONLY_FIELDS[field.name]) ||
    ENTITY_TYPE_FIELDS.has(field.name) ||
    LICENCE_SELECT_FIELDS.has(field.name) ||
    (field.name === "establishmentName" && field.type === "select")
  )
}

/** Whether Guided Mode offers the prose box for a field: the type rule in `field-schema.ts` plus the client-only preset-data exclusion. */
export function proseHelpsFor(field: FieldConfig): boolean {
  return proseHelpsForType(field) && !isPresetField(field)
}
