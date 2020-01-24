import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PhsConfirmationComponent } from './phs-confirmation.component';

describe('PhsConfirmationComponent', () => {
  let component: PhsConfirmationComponent;
  let fixture: ComponentFixture<PhsConfirmationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PhsConfirmationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PhsConfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
