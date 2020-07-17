import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LgZoningConfirmationComponent } from './lg-zoning-confirmation.component';

describe('LgZoningComfirmationComponent', () => {
  let component: LgZoningConfirmationComponent;
  let fixture: ComponentFixture<LgZoningConfirmationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LgZoningConfirmationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LgZoningConfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
