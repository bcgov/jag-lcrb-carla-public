import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConnectionToOtherLiquorLicencesComponent } from '@components/account-profile/tabs/connection-to-other-liquor-licences/connection-to-other-liquor-licences.component';
import { AccountDataService } from '@services/account-data.service';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';

describe('ConnectionToOtherLiquorLicencesComponent', () => {
  let component: ConnectionToOtherLiquorLicencesComponent;
  let fixture: ComponentFixture<ConnectionToOtherLiquorLicencesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ConnectionToOtherLiquorLicencesComponent],
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
    fixture = TestBed.createComponent(ConnectionToOtherLiquorLicencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
