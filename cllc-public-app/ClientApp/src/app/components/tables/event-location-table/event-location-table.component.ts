import { Component, Input, forwardRef } from "@angular/core";
import { FormBuilder, FormArray, FormGroup, NG_VALUE_ACCESSOR, FormControl, NG_VALIDATORS, Validators } from "@angular/forms";
import { faPlusCircle, faTrash } from "@fortawesome/free-solid-svg-icons";
import { EventLocation } from "@models/event-location.model";
import { ServiceArea } from "@models/service-area.model";
import { BaseControlValueAccessor } from "../BaseControlValueAccessor";

@Component({
  selector: 'app-event-location-table',
  templateUrl: './event-location-table.component.html',
  styleUrls: ['./event-location-table.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EventLocationTableComponent),
      multi: true
    },
    {
      provide: NG_VALIDATORS,
      useExisting: EventLocationTableComponent,
      multi: true
    }
  ]
})
export class EventLocationTableComponent extends BaseControlValueAccessor<EventLocation[]> {
  // Icons
  faTrash = faTrash;
  faPlusCircle = faPlusCircle;

  // Whether this control is enabled or not (affects edit mode)
  @Input() enabled: boolean = true;

  // Whether to run validation per row
  @Input() shouldValidate: boolean = false;

  // The service areas to show on the location ID drop-down
  @Input() serviceAreas: ServiceArea[] = [];

  @Input() eventId: string = null;

  // Internal table state
  total = 0;
  rows = new FormArray([]);
  validationMessages: string[] = [];

  registerOnChange(fn: any) { this.onChange = fn; }
  registerOnTouched(fn: any) { this.onTouched = fn; }

  constructor(private fb: FormBuilder) {
    super();
    this.rows.valueChanges.subscribe(val => {
      this.updateTotal();
      this.onChange(val);
      this.value = val;
    });
  }

  get showErrorSection(): boolean {
    return this.enabled && this.shouldValidate && this.validationMessages.length > 0;
  }

  updateTotal() {
    const eventLocations: EventLocation[] = this.rows.value;
    const result = eventLocations.reduce((acc, loc) => {
      if (typeof loc.attendance === "number") {
        acc += loc.attendance;
      } else {
        const num = parseInt(loc.attendance, 10);
        if (num > 0) {
          acc += num;
        }
      }
      return acc;
    }, 0);

    this.total = result;
  }

  writeValue(array: EventLocation[]) {
    // return early if no value provided
    if (!array) {
      super.writeValue([]);
      return;
    }

    // First, clear previous state. Then, add the new state.
    super.writeValue(array);
    while (this.rows.length > 0) {
      this.rows.removeAt(0);
    }
    for (const obj of array) {
      this.addInternal(obj);
    }

    // read-only mode
    if (!this.enabled) {
      this.rows.disable();
    }
  }

  private addInternal(value: EventLocation) {
    const group = this.fb.group({
      id: [value.id],
      eventId: [value.eventId],
      serviceAreaId: [value.serviceAreaId, [Validators.required]],
      name: [value.name, [Validators.required]],
      attendance: [value.attendance, [Validators.required]],
    });

    this.rows.push(group);
  }

  addRow() {
    const newRow = new EventLocation();
    newRow.eventId = this.eventId;
    this.writeValue([...this.rows.value, newRow]);
  }

  removeRow(index: number) {
    if (index >= 0 && index < this.rows.length) {
      this.removeInternal(index);
      this.writeValue(this.rows.value);
    }
  }

  private removeInternal(index: number) {
    const row = this.rows.at(index);
    const obj = row.value as EventLocation;
    this.rows.removeAt(index);
  }

  readonly(index: number): boolean {
    return !this.enabled;
  }

  validate({ value }: FormControl) {
    let invalid = false;
    for (const row of this.rows.controls) {
      if (row.invalid) {
        invalid = true;
      }
    }
    this.validationMessages = [...new Set<string>(this.listControlsWithErrors(this.rows))];
    return invalid && { invalid: true };
  }

  private listControlsWithErrors(form: FormGroup | FormArray): string[] {
    let errors = [];
    if (form instanceof FormGroup || form instanceof FormArray) {
      for (const c in form.controls) {
        const control = form.get(c);

        if (control.valid || control.status === 'DISABLED') {
          continue;  // skip valid fields
        }

        if (control instanceof FormGroup || control instanceof FormArray) {
          errors = [...errors, ...this.listControlsWithErrors(control)];
        } else {
          if (control.errors?.required) {
            errors.push('All fields are required');
          }
        }
      }
    }
    return errors;
  }
}
