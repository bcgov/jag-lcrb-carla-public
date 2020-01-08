/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductInventorySalesReportComponent } from './product-inventory-sales-report.component';
import { NO_ERRORS_SCHEMA, Component } from '@angular/core';
import { MonthlyReportDataService } from '@services/monthly-report.service';
import { of } from 'rxjs';
import { FormGroup } from '@angular/forms';

describe('ProductInventorySalesReportComponent', () => {
  let component: TestHostComponent;
  let fixture: ComponentFixture<TestHostComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ProductInventorySalesReportComponent, TestHostComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        { provide: MonthlyReportDataService, useValue: { getMonthlyReportsByLicence: () => of([]) } },
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TestHostComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});


//Create test component to pass input to the ProductInventorySalesReportComponent component
@Component({
  selector: `host-component`,
  template: `<app-product-inventory-sales-report [productForm]="productForm"></app-product-inventory-sales-report>`
})
class TestHostComponent {
  private productForm = new FormGroup({});
}
