import { ControlValueAccessor } from '@angular/forms';

export class BaseControlValueAccessor<T> implements ControlValueAccessor {
  public disabled = false;

  /**
   * Call when value has changed programmatically
   */
  public onChange(newVal: T) {}
  public onTouched(_?: any) {}
  public value: T;

  /**
   * Model -> View changes
   */
  public writeValue(obj: T): void { this.value = obj; }
  public registerOnChange(fn: any): void { this.onChange = fn; }
  public registerOnTouched(fn: any): void { this.onTouched = fn; }
  public setDisabledState?(isDisabled: boolean): void { this.disabled = isDisabled; }
}
