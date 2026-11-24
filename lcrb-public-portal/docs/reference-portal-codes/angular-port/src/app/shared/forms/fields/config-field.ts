/** ConfigField: renders a single FieldConfig using the bound field controls. */
import { ChangeDetectionStrategy, Component, computed, inject, input, type Signal } from "@angular/core"
import { injectBusinessProfile } from "@core/session/business-profile"
import { FieldHelpContext } from "../layout"
import { isFieldVisible, YES_NO, type FieldConfig } from "@domain/forms/field-schema"
import { injectApplicationContext } from "../provider"
import {
  BUSINESS_READONLY_FIELDS,
  businessReadonlyProps,
  ENTITY_TYPE_FIELDS,
  entityTypeProps,
  EstablishmentSelectFieldF,
  LICENCE_SELECT_FIELDS,
  LicenceSelectFieldF,
  ReadonlyAutoFieldF,
  type ReadonlyAutoProps,
} from "./business-fields"
import { CheckboxFieldF, CheckboxGroupFieldF, RadioFieldF, SelectFieldF } from "./choice-fields"
import { FieldShell } from "./field-binding"
import { FileFieldF } from "./file-field"
import { AreaFieldF, TextFieldF } from "./text-fields"

type ConfigKind =
  | "hidden"
  | "readonly"
  | "licence"
  | "establishment"
  | "textarea"
  | "select"
  | "radio"
  | "checkbox"
  | "file"
  | "checkbox-group"
  | "text"

/** Renders one field config with the bound primitives; `help` goes to <FormField> via context, and the host is the field wrapper (hidden when not visible, so nothing is validated). */
@Component({
  selector: "div[configField]",
  imports: [
    ReadonlyAutoFieldF,
    LicenceSelectFieldF,
    EstablishmentSelectFieldF,
    AreaFieldF,
    TextFieldF,
    SelectFieldF,
    RadioFieldF,
    CheckboxFieldF,
    CheckboxGroupFieldF,
    FileFieldF,
  ],
  providers: [
    {
      provide: FieldHelpContext,
      useFactory: () => {
        const self = inject(ConfigField)
        return () => self.field().help
      },
    },
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { "[attr.hidden]": "visible() ? null : ''" },
  template: `
    @switch (kind()) {
      @case ("readonly") {
        @if (readonlyProps(); as p) {
          <div readonlyAutoFieldF [bare]="true" [name]="p.name" [label]="p.label" [value]="p.value" [storedValue]="p.storedValue" [hint]="p.hint"></div>
        }
      }
      @case ("licence") {
        <div licenceSelectFieldF [bare]="true" [name]="field().name" [label]="field().label" [required]="field().required" [hint]="field().hint" [placeholder]="field().placeholder ?? 'Select one of your licences'" [validation]="field().validation"></div>
      }
      @case ("establishment") {
        <div establishmentSelectFieldF [bare]="true" [name]="field().name" [label]="field().label" [required]="field().required" [hint]="field().hint" [placeholder]="field().placeholder ?? 'Select an establishment'" [validation]="field().validation"></div>
      }
      @case ("textarea") {
        <div areaFieldF [bare]="true" [name]="field().name" [label]="field().label" [required]="field().required" [placeholder]="field().placeholder" [hint]="field().hint" [validation]="field().validation"></div>
      }
      @case ("select") {
        <div selectFieldF [bare]="true" [name]="field().name" [label]="field().label" [required]="field().required" [hint]="field().hint" [options]="field().options || []" [validation]="field().validation"></div>
      }
      @case ("radio") {
        <div radioFieldF [bare]="true" [name]="field().name" [label]="field().label" [required]="field().required" [hint]="field().hint" [options]="field().options || yesNo" [validation]="field().validation"></div>
      }
      @case ("checkbox") {
        <div checkboxFieldF [bare]="true" [name]="field().name" [label]="field().label" [required]="field().required"></div>
      }
      @case ("file") {
        <div fileFieldF [bare]="true" [name]="field().name" [label]="field().label" [required]="field().required" [hint]="field().hint" [validation]="field().validation" [criteria]="field().criteria" [docType]="field().docType" [autoPopulateField]="field().autoPopulateField"></div>
      }
      @case ("checkbox-group") {
        <div checkboxGroupFieldF [bare]="true" [name]="field().name" [label]="field().label" [options]="field().options || []" [hint]="field().hint" [required]="field().required" [validation]="field().validation"></div>
      }
      @case ("text") {
        <div textFieldF [bare]="true" [name]="field().name" [label]="field().label" [type]="field().type" [required]="field().required" [placeholder]="field().placeholder" [hint]="field().hint" [validation]="field().validation"></div>
      }
    }
  `,
})
export class ConfigField extends FieldShell {
  readonly field = input.required<FieldConfig>()
  readonly name = computed(() => this.field().name)
  private readonly ctx = injectApplicationContext()
  private readonly profile = injectBusinessProfile()
  protected readonly yesNo = YES_NO

  protected readonly visible = computed(() => isFieldVisible(this.field(), this.ctx.formData()))
  protected override readonly shellActive: Signal<boolean> = this.visible
  protected override readonly displayStyle: Signal<string | null> = computed(() => (this.visible() ? null : "none"))

  protected readonly kind = computed<ConfigKind>(() => {
    const field = this.field()
    if (!this.visible()) return "hidden"
    if (BUSINESS_READONLY_FIELDS[field.name]) return "readonly"
    // Legal-entity type: from BC Registries, or the editable select when unmatched.
    if (ENTITY_TYPE_FIELDS.has(field.name)) return entityTypeProps(this.profile(), field) ? "readonly" : "select"
    if (LICENCE_SELECT_FIELDS.has(field.name)) return "licence"
    // Establishment name: the preset picker only when a flow opts in with type "select" (existing establishment); new-establishment flows use free text so a new name can be typed.
    if (field.name === "establishmentName" && field.type === "select") return "establishment"
    switch (field.type) {
      case "textarea":
      case "select":
      case "radio":
      case "checkbox":
      case "file":
      case "checkbox-group":
        return field.type
      default:
        return "text"
    }
  })

  protected readonly readonlyProps = computed<ReadonlyAutoProps | null>(() => {
    const field = this.field()
    const profile = this.profile()
    const businessKind = BUSINESS_READONLY_FIELDS[field.name]
    if (businessKind) return businessReadonlyProps(profile, field, businessKind)
    return entityTypeProps(profile, field)
  })
}
