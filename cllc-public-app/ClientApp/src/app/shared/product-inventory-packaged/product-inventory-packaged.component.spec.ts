import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductInventoryPackagedComponent } from './product-inventory-packaged.component';

describe('ProductInventoryPackagedComponent', () => {
  let component: ProductInventoryPackagedComponent;
  let fixture: ComponentFixture<ProductInventoryPackagedComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductInventoryPackagedComponent ]
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
