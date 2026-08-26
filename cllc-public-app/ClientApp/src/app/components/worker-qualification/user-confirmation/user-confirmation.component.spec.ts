import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { ContactDataService } from '@services/contact-data.service';
import { UserDataService } from '@services/user-data.service';
import { of } from 'rxjs';
import { UserConfirmationComponent } from './user-confirmation.component';

const userDataServiceStub: Partial<UserDataService> = {
  getCurrentUser: () => of(null)
};
const contactDataServiceStub: Partial<ContactDataService> = {};

describe('UserConfirmationComponent', () => {
  let component: UserConfirmationComponent;
  let fixture: ComponentFixture<UserConfirmationComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [UserConfirmationComponent],
      providers: [
        FormBuilder,
        { provide: UserDataService, useValue: userDataServiceStub },
        { provide: ContactDataService, useValue: contactDataServiceStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
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
