import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { AccountDataService } from '@services/account-data.service';
import { AccountPickerComponent } from './account-picker.component';

const accountDataServiceStub = {};
describe('AccountPickerComponent', () => {
  let component: AccountPickerComponent;
  let fixture: ComponentFixture<AccountPickerComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [AccountPickerComponent],
      imports: [MatAutocompleteModule],
      providers: [FormBuilder, { provide: AccountDataService, useValue: accountDataServiceStub }],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
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
