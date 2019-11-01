import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserConfirmationComponent } from './user-confirmation.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { UserDataService } from '@services/user-data.service';
import { ContactDataService } from '@services/contact-data.service';
import { of } from 'rxjs';
import { FormBuilder } from '@angular/forms';

const userDataServiceStub: Partial<UserDataService> = {
  getCurrentUser: () =>  of(null)
};
const contactDataServiceStub: Partial<ContactDataService> = {};

describe('UserConfirmationComponent', () => {
  let component: UserConfirmationComponent;
  let fixture: ComponentFixture<UserConfirmationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserConfirmationComponent ],
      providers: [
        FormBuilder,
        {provide: UserDataService, useValue: userDataServiceStub},
        {provide: ContactDataService, useValue: contactDataServiceStub},
      ],
      schemas: [ NO_ERRORS_SCHEMA ]
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
