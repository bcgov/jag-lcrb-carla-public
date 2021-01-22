import { ControlValueAccessor } from "@angular/forms";

export class BaseControlValueAccessor<T> implements ControlValueAccessor {
  disabled = false;

  /**
   * Call when value has changed programmatically
   */
  onChange(newVal: T) {}

  onTouched(_?: any) {}

  value: T;

  /**
   * Model -> View changes
   */
  writeValue(obj: T): void { this.value = obj; }

  registerOnChange(fn: any): void { this.onChange = fn; }

  registerOnTouched(fn: any): void { this.onTouched = fn; }

  setDisabledState?(isDisabled: boolean): void { this.disabled = isDisabled; }
}
