import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountProfileComponent } from './account-profile.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { provideMockStore } from '@ngrx/store/testing';
import { UserDataService } from '@services/user-data.service';
import { AccountDataService } from '@services/account-data.service';
import { ContactDataService } from '@services/contact-data.service';
import { DynamicsDataService } from '@services/dynamics-data.service';
import { FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { of } from 'rxjs';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';

const routerSpy = jasmine.createSpyObj('Router', ['navigateByUrl']);
const initialState = {
  currentAccountState: { currentAccount: new Account() },
  currentUserState: { currentUser: {} }
} as AppState;

describe('AccountProfileComponent', () => {
  let component: AccountProfileComponent;
  let fixture: ComponentFixture<AccountProfileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AccountProfileComponent],
      providers: [
        FormBuilder,
        provideMockStore({initialState}),
        { provide: ActivatedRoute, useValue: new ActivatedRouteStub() },
        { provide: UserDataService, useValue: {} },
        { provide: AccountDataService, useValue: {} },
        { provide: ContactDataService, useValue: {} },
        { provide: DynamicsDataService, useValue: { getRecord: () => of(null) } },
        { provide: Router, useValue: routerSpy },
        { provide: TiedHouseConnectionsDataService, useValue: {} },
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
