/** Choice field controls: select, radio, checkbox and checkbox-group. */
import { booleanAttribute, ChangeDetectionStrategy, Component, computed, input } from "@angular/core"
import { Checkbox } from "@shared/ui/primitives/checkbox"
import { Label } from "@shared/ui/primitives/label"
import { RADIO_GROUP } from "@shared/ui/primitives/radio-group"
import { SELECT } from "@shared/ui/primitives/select"
import { cn } from "@shared/util/utils"
import type { FieldValidation } from "@domain/forms/field-validation"
import type { Option } from "@domain/forms/field-schema"
import { FormField } from "../layout"
import { injectApplicationContext } from "../provider"
import { FieldShell, injectField, registerFieldValidator } from "./field-binding"

/** Marker class for portalled `SelectContent`; the global stylesheet enlarges these items only in Guided Mode, since the portal sits outside the wrapper. */
export const GUIDED_OPTION_CLASS = "guided-options"

@Component({
  selector: "div[selectFieldF]",
  imports: [FormField, SELECT],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div formField [label]="label()" [required]="required()" [hint]="hint()" [error]="f.error()" [htmlFor]="name()">
      <ui-select [value]="f.value()" (valueChange)="choose($event)">
        <button uiSelectTrigger [attr.id]="name()" [attr.aria-invalid]="f.error() ? 'true' : 'false'" [class]="triggerClass()">
          <span uiSelectValue [placeholder]="placeholder()"></span>
        </button>
        <ng-template uiSelectContent [class]="guidedClass">
          @for (o of visibleOptions(); track o.value) {
            <ui-select-item [value]="o.value">{{ o.label }}</ui-select-item>
          }
        </ng-template>
      </ui-select>
    </div>
  `,
})
export class SelectFieldF extends FieldShell {
  readonly name = input.required<string>()
  readonly label = input.required<string>()
  readonly required = input<boolean | undefined>(undefined)
  readonly hint = input<string | undefined>(undefined)
  readonly placeholder = input("Select an option")
  readonly options = input.required<Option[]>()
  readonly validation = input<FieldValidation | undefined>(undefined)
  protected readonly f = injectField(() => ({
    name: this.name(),
    rules: this.validation(),
    required: this.required(),
    label: this.label(),
  }))
  protected readonly visibleOptions = computed(() => this.options().filter((o) => o.value !== ""))
  protected readonly guidedClass = GUIDED_OPTION_CLASS
  protected triggerClass = () => cn(this.f.error() && "border-destructive")
  protected choose(v: string | undefined): void {
    this.f.set(v ?? "")
    this.f.onBlur()
  }
}

@Component({
  selector: "div[radioFieldF]",
  imports: [FormField, RADIO_GROUP, Label],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div formField [label]="label()" [required]="required()" [hint]="hint()" [error]="f.error()">
      <div uiRadioGroup [value]="f.value()" (valueChange)="choose($event)" class="flex flex-col gap-2">
        @for (o of options(); track o.value) {
          <div class="flex items-center gap-2">
            <button uiRadioGroupItem [value]="o.value" [attr.id]="name() + '-' + o.value"></button>
            <label uiLabel [attr.for]="name() + '-' + o.value" class="font-normal">{{ o.label }}</label>
          </div>
        }
      </div>
    </div>
  `,
})
export class RadioFieldF extends FieldShell {
  readonly name = input.required<string>()
  readonly label = input.required<string>()
  readonly required = input<boolean | undefined>(undefined)
  readonly hint = input<string | undefined>(undefined)
  readonly options = input.required<Option[]>()
  readonly validation = input<FieldValidation | undefined>(undefined)
  protected readonly f = injectField(() => ({
    name: this.name(),
    rules: this.validation(),
    required: this.required(),
    label: this.label(),
  }))
  protected choose(v: string | undefined): void {
    this.f.set(v)
    this.f.onBlur()
  }
}

@Component({
  selector: "div[checkboxFieldF]",
  imports: [Checkbox, Label],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="flex items-start gap-2">
      <button
        uiCheckbox
        [attr.id]="name()"
        [checked]="checked()"
        (checkedChange)="onChecked($event)"
        class="mt-1"
        [attr.aria-invalid]="error() ? 'true' : 'false'"
      ></button>
      <label uiLabel [attr.for]="name()" class="font-normal leading-relaxed">{{ label() }}@if (required()) {<span class="text-destructive ml-1">*</span>}</label>
    </div>
    @if (error()) {
      <p class="text-xs text-destructive mt-1">{{ error() }}</p>
    }
  `,
})
export class CheckboxFieldF extends FieldShell {
  readonly name = input.required<string>()
  readonly label = input.required<string>()
  readonly required = input(false, { transform: booleanAttribute })
  private readonly ctx = injectApplicationContext()
  protected readonly checked = computed(() => Boolean(this.ctx.formData()[this.name()]))
  private readonly rules = computed<FieldValidation | undefined>(() =>
    this.required() ? { required: "You must check this box to continue." } : undefined,
  )
  protected readonly error = computed(() => this.ctx.getFieldError(this.name(), this.rules()))

  constructor() {
    super()
    registerFieldValidator(this.ctx, this.name, this.rules, this.label, () => [this.required()])
  }

  protected onChecked(c: boolean): void {
    this.ctx.updateFormData({ [this.name()]: c === true })
    this.ctx.setFieldTouched(this.name())
  }
}

@Component({
  selector: "div[checkboxGroupFieldF]",
  imports: [FormField, Checkbox, Label],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div formField [label]="label()" [hint]="hint()" [error]="error()" [required]="required()">
      <div class="flex flex-col gap-2">
        @for (o of options(); track o.value) {
          <div class="flex items-center gap-2">
            <button
              uiCheckbox
              [attr.id]="name() + '-' + o.value"
              [checked]="selected().includes(o.value)"
              (checkedChange)="toggle(o.value, $event)"
            ></button>
            <label uiLabel [attr.for]="name() + '-' + o.value" class="font-normal">{{ o.label }}</label>
          </div>
        }
      </div>
    </div>
  `,
})
export class CheckboxGroupFieldF extends FieldShell {
  readonly name = input.required<string>()
  readonly label = input.required<string>()
  readonly options = input.required<Option[]>()
  readonly hint = input<string | undefined>(undefined)
  readonly required = input<boolean | undefined>(undefined)
  readonly validation = input<FieldValidation | undefined>(undefined)
  private readonly ctx = injectApplicationContext()
  protected readonly selected = computed(() => (this.ctx.formData()[this.name()] as string[]) || [])
  private readonly rules = computed<FieldValidation | undefined>(() => {
    const validation = this.validation()
    const required = this.required()
    return validation || required ? { ...(validation ?? {}), required: required ?? validation?.required } : undefined
  })
  protected readonly error = computed(() => this.ctx.getFieldError(this.name(), this.rules()))

  constructor() {
    super()
    registerFieldValidator(this.ctx, this.name, this.rules, this.label, () => [
      this.required(),
      JSON.stringify(this.validation()),
    ])
  }

  protected toggle(val: string, on: boolean): void {
    const selected = this.selected()
    this.ctx.updateFormData({ [this.name()]: on ? [...selected, val] : selected.filter((v) => v !== val) })
    this.ctx.setFieldTouched(this.name())
  }
}
