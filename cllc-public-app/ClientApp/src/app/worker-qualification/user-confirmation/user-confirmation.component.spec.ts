import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserConfirmationComponent } from './user-confirmation.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('UserConfirmationComponent', () => {
  let component: UserConfirmationComponent;
  let fixture: ComponentFixture<UserConfirmationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserConfirmationComponent ],
      schemas: [ NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserConfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
