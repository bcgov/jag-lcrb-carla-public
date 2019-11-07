import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConnectionToProducersComponent } from './connection-to-producers.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { AccountDataService } from '@services/account-data.service';

describe('ConnectionToProducersComponent', () => {
  let component: ConnectionToProducersComponent;
  let fixture: ComponentFixture<ConnectionToProducersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ConnectionToProducersComponent],
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
    fixture = TestBed.createComponent(ConnectionToProducersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
