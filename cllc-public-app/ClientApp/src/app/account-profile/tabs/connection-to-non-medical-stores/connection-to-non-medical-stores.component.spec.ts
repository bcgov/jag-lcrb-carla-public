import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConnectionToNonMedicalStoresComponent } from './connection-to-non-medical-stores.component.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { AccountDataService } from '@services/account-data.service';

describe('ConnectionToNonMedicalStoresComponent', () => {
  let component: ConnectionToNonMedicalStoresComponent;
  let fixture: ComponentFixture<ConnectionToNonMedicalStoresComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ConnectionToNonMedicalStoresComponent],
      providers: [
        FormBuilder,
        { provide: MatSnackBar, useValue: {} },
        { provide: TiedHouseConnectionsDataService, useValue: {} },
        { provide: AccountDataService, useValue: {} },
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConnectionToNonMedicalStoresComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
