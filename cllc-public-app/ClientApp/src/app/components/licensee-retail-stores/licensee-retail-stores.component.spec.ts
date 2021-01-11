import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LicenseeRetailStoresComponent } from './licensee-retail-stores.component';

describe('LicenseeRetailStoresComponent', () => {
  let component: LicenseeRetailStoresComponent;
  let fixture: ComponentFixture<LicenseeRetailStoresComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LicenseeRetailStoresComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseeRetailStoresComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
