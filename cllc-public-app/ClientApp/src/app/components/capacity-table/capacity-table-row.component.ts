import { Component, forwardRef, Input } from '@angular/core';
import { FormGroup, FormBuilder, NG_VALUE_ACCESSOR, ControlValueAccessor, Validators, FormControl, NG_VALIDATORS } from '@angular/forms';
import { BaseControlValueAccessor } from './BaseControlValueAccessor';
import { ServiceArea, AreaCategory } from '@models/service-area.model';

@Component({
  selector: '[capacity-table-row]',
  styleUrls: ['./capacity-table-row.component.scss'],
  template: `
    <ng-container [formGroup]="rowGroup">
        <td><input type="text" formControlName="areaNumber" [readonly]="!enabled"/></td>
        <td><input type="text" formControlName="areaLocation" placeholder="Enter Area Description" [readonly]="!enabled"/></td>
        <td *ngIf="isService()"><mat-checkbox formControlName="isIndoor" [disabled]="!enabled"></mat-checkbox></td>
        <td *ngIf="isService()"><mat-checkbox formControlName="isPatio" [disabled]="!enabled"></mat-checkbox></td>
        <td><input type="text" formControlName="capacity" placeholder="Enter Occupant Load" mask="0*" [readonly]="!enabled"/></td>
        <td><button (click)="removeRow()" class="btn-clear" *ngIf="enabled"><i class="fas fa-trash-alt"></i></button></td>
    </ng-container>
  `,
  styles: [``],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CapacityTableRowComponent),
      multi: true
    },
    {
        provide: NG_VALIDATORS,
        useExisting: CapacityTableRowComponent,
        multi: true
    }
  ]
})
export class CapacityTableRowComponent extends BaseControlValueAccessor<ServiceArea> implements ControlValueAccessor {
  @Input() areaCategory: number;
  @Input() index: number;
  @Input() onDelete: (index) => void;
  @Input() onRowChange: (val) => void;
  @Input() enabled: boolean = true;
  rowGroup: FormGroup;

  public value: ServiceArea;
  registerOnChange(fn: any) { this.onChange = fn; }
  registerOnTouched(fn: any) { this.onTouched = fn; }

  constructor(private formBuilder: FormBuilder) {
    super();
    this.rowGroup = formBuilder.group({
        areaCategory: [this.areaCategory],
        areaNumber: ['', [Validators.required]],
        areaLocation: ['', [Validators.required]],
        isIndoor: [''],
        isOutdoor: [''],
        isPatio: [''],
        capacity: ['', [Validators.required]]
    });

    this.rowGroup.valueChanges.subscribe(val => {
      this.onChange(val);
      this.onRowChange(val);
      this.value = val;
    });
  }

  writeValue(val: object) {
    this.rowGroup.patchValue(val);
  }

  removeRow() {
    this.onDelete(this.index);
  }

  validate({ value }: FormControl) {
    const isNotValid = this.rowGroup.invalid;
    const retVal = {};
    if (this.rowGroup.get('areaNumber').invalid) {
      retVal['areaNumber'] = true;
    }
    if (this.rowGroup.get('areaLocation').invalid) {
      retVal['areaLocation'] = true;
    }
    if (this.rowGroup.get('capacity').invalid) {
      retVal['capacity'] = true;
    }
    return isNotValid && retVal;
  }

  isService(): boolean {
    return this.areaCategory === AreaCategory.Service;
  }
}
