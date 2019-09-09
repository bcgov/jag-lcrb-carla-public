import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountPickerComponent } from './account-picker.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { MatAutocompleteModule } from '@angular/material';
import { AccountDataService } from '@services/account-data.service';
import { FormBuilder } from '@angular/forms';

const accountDataServiceStub = {};
describe('AccountPickerComponent', () => {
  let component: AccountPickerComponent;
  let fixture: ComponentFixture<AccountPickerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AccountPickerComponent],
      imports: [
  MatAutocompleteModule
      ],
      providers: [
        FormBuilder,
        { provide: AccountDataService, useValue: accountDataServiceStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountPickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
