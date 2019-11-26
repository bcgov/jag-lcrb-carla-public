import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-product-inventory-sales-report',
  templateUrl: './product-inventory-sales-report.component.html',
  styleUrls: ['./product-inventory-sales-report.component.scss']
})
export class ProductInventorySalesReportComponent implements OnInit {
  @Input() productForm: FormGroup;

  constructor() { }

  ngOnInit() {
  }

  isFieldInvalid(fieldName: string) {
    return !this.productForm.get(fieldName).valid && (this.productForm.get(fieldName).dirty || this.productForm.get(fieldName).touched);
  }
}
