import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, ValidationErrors } from '@angular/forms';
import { validationErrors } from '../federal-reporting-validation';

@Component({
  selector: 'app-product-inventory-sales-report',
  templateUrl: './product-inventory-sales-report.component.html',
  styleUrls: ['./product-inventory-sales-report.component.scss']
})
export class ProductInventorySalesReportComponent implements OnInit {
  @Input() productForm: FormGroup;
  @Input() visibleInventoryReports: string[] = [];

  constructor() { }

  ngOnInit() {
  }

  getFormValidationErrors() {
    const errorStrings = [];
    Object.keys(this.productForm.controls).forEach(key => {
      const controlErrors: ValidationErrors = this.productForm.get(key).errors;
      if (controlErrors != null) {
        Object.keys(controlErrors).forEach(keyError => {
          const error = validationErrors[key].find(e => e.type === keyError);
          if (errorStrings.findIndex(e => e === error.message) === -1) {
            errorStrings.push(error.message);
          }
        });
      }
    });
    return errorStrings;
  }

  isFieldInvalid(fieldName: string) {
    return !this.productForm.get(fieldName).valid && (this.productForm.dirty || this.productForm.touched);
  }
}
