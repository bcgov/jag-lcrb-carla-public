import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductionStagesComponent } from './production-stages.component';

describe('ProductionStagesComponent', () => {
  let component: ProductionStagesComponent;
  let fixture: ComponentFixture<ProductionStagesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductionStagesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductionStagesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
