import { Component, forwardRef, Input } from '@angular/core';
import { FormGroup, FormBuilder, NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { BaseControlValueAccessor } from './BaseControlValueAccessor';
import { ServiceArea } from '@models/service-area.model';

@Component({
  selector: '[capacity-table-row]',
  styleUrls: ['./capacity-table-row.component.scss'],
  template: `
    <ng-container [formGroup]="rowGroup">
        <td><input type="text" formControlName="areaNumber" /></td>
        <td><input type="text" formControlName="areaLocation" /></td>
        <td *ngIf="isIndoor"><mat-checkbox formControlName="isIndoor"></mat-checkbox></td>
        <td *ngIf="isIndoor"><mat-checkbox formControlName="isPatio"></mat-checkbox></td>
        <td><input type="text" formControlName="capacity" mask="0*"/></td>
        <td><button (click)="removeRow()" class="btn-clear"><i class="fa fa-minus-square danger"></i></button></td>
    </ng-container>
  `,
  styles: [``],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CapacityTableRowComponent),
      multi: true
    }
  ]
})
export class CapacityTableRowComponent extends BaseControlValueAccessor<ServiceArea> implements ControlValueAccessor {
  @Input() isIndoor: boolean;
  @Input() index: number;
  @Input() onDelete: (index) => void;
  rowGroup: FormGroup;

  public value: ServiceArea;
  registerOnChange(fn: any) { this.onChange = fn; }
  registerOnTouched(fn: any) { this.onTouched = fn; }

  constructor(private formBuilder: FormBuilder) {
    super();
    this.rowGroup = formBuilder.group({
        areaNumber: [''],
        areaLocation: [''],
        isIndoor: [''],
        isPatio: [''],
        capacity: ['']
    });

    this.rowGroup.valueChanges.subscribe(val => {
      this.onChange(val);
      this.value = val;
    });
  }

  writeValue(val: object) {
    this.rowGroup.patchValue(val);
  }

  removeRow() {
    this.onDelete(this.index);
  }
}
