import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FinancialInformationComponent } from './financial-information.component';

describe('FinancialInformationComponent', () => {
  let component: FinancialInformationComponent;
  let fixture: ComponentFixture<FinancialInformationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FinancialInformationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FinancialInformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
