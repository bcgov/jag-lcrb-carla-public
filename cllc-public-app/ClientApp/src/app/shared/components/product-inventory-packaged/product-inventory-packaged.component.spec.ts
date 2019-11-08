import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductInventoryPackagedComponent } from './product-inventory-packaged.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { MatTableModule } from '@angular/material';

describe('ProductInventoryPackagedComponent', () => {
  let component: ProductInventoryPackagedComponent;
  let fixture: ComponentFixture<ProductInventoryPackagedComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductInventoryPackagedComponent ],
      imports: [ MatTableModule ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductInventoryPackagedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
