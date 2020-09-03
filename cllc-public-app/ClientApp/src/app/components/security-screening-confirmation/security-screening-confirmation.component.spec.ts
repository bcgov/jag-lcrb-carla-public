import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SecurityScreeningConfirmationComponent } from './security-screening-confirmation.component';

describe('SecurityScreeningConfirmationComponent', () => {
  let component: SecurityScreeningConfirmationComponent;
  let fixture: ComponentFixture<SecurityScreeningConfirmationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SecurityScreeningConfirmationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SecurityScreeningConfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
