import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AccountDataService } from '@services/account-data.service';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { ConnectionToProducersComponent } from './connection-to-producers.component';

describe('ConnectionToProducersComponent', () => {
  let component: ConnectionToProducersComponent;
  let fixture: ComponentFixture<ConnectionToProducersComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ConnectionToProducersComponent],
      providers: [
        FormBuilder,
        { provide: MatSnackBar, useValue: {} },
        { provide: TiedHouseConnectionsDataService, useValue: {} },
        { provide: AccountDataService, useValue: {} }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
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
