/** Text-like field controls: TextFieldF (Input) and AreaFieldF (Textarea). */
import { ChangeDetectionStrategy, Component, input } from "@angular/core"
import { Input } from "@shared/ui/primitives/input"
import { Textarea } from "@shared/ui/primitives/textarea"
import { cn } from "@shared/util/utils"
import type { FieldValidation } from "@domain/forms/field-validation"
import { FormField } from "../layout"
import { FieldShell, injectField } from "./field-binding"

@Component({
  selector: "div[textFieldF]",
  imports: [FormField, Input],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div formField [label]="label()" [required]="required()" [hint]="hint()" [error]="f.error()" [warning]="f.warning()" [htmlFor]="name()">
      <input
        uiInput
        [attr.id]="name()"
        [type]="type()"
        [value]="f.value()"
        [attr.placeholder]="placeholder()"
        (input)="f.set($any($event.target).value)"
        (blur)="f.onBlur()"
        [attr.aria-invalid]="f.error() ? 'true' : 'false'"
        [class]="inputClass()"
      />
    </div>
  `,
})
export class TextFieldF extends FieldShell {
  readonly name = input.required<string>()
  readonly label = input.required<string>()
  readonly required = input<boolean | undefined>(undefined)
  readonly placeholder = input<string | undefined>(undefined)
  readonly hint = input<string | undefined>(undefined)
  readonly type = input("text")
  readonly validation = input<FieldValidation | undefined>(undefined)
  protected readonly f = injectField(() => ({
    name: this.name(),
    rules: this.validation(),
    required: this.required(),
    label: this.label(),
  }))
  protected inputClass = () => cn(this.f.error() && "border-destructive focus-visible:ring-destructive")
}

@Component({
  selector: "div[areaFieldF]",
  imports: [FormField, Textarea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div formField [label]="label()" [required]="required()" [hint]="hint()" [error]="f.error()" [htmlFor]="name()">
      <textarea
        uiTextarea
        [attr.id]="name()"
        [value]="f.value()"
        [attr.placeholder]="placeholder()"
        (input)="f.set($any($event.target).value)"
        (blur)="f.onBlur()"
        rows="4"
        [attr.aria-invalid]="f.error() ? 'true' : 'false'"
        [class]="inputClass()"
      ></textarea>
    </div>
  `,
})
export class AreaFieldF extends FieldShell {
  readonly name = input.required<string>()
  readonly label = input.required<string>()
  readonly required = input<boolean | undefined>(undefined)
  readonly placeholder = input<string | undefined>(undefined)
  readonly hint = input<string | undefined>(undefined)
  readonly validation = input<FieldValidation | undefined>(undefined)
  protected readonly f = injectField(() => ({
    name: this.name(),
    rules: this.validation(),
    required: this.required(),
    label: this.label(),
  }))
  protected inputClass = () => cn(this.f.error() && "border-destructive focus-visible:ring-destructive")
}
