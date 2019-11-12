/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductInventorySalesReportComponent } from './product-inventory-sales-report.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('ProductInventorySalesReportComponent', () => {
  let component: ProductInventorySalesReportComponent;
  let fixture: ComponentFixture<ProductInventorySalesReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductInventorySalesReportComponent ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductInventorySalesReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
