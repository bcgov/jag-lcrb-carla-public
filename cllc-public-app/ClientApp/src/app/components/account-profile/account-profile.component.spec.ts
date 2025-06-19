import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { Account } from '@models/account.model';
import { provideMockStore } from '@ngrx/store/testing';
import { AccountDataService } from '@services/account-data.service';
import { ContactDataService } from '@services/contact-data.service';
import { DynamicsDataService } from '@services/dynamics-data.service';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { UserDataService } from '@services/user-data.service';
import { of } from 'rxjs';
import { AccountProfileComponent } from './account-profile.component';

const routerSpy = jasmine.createSpyObj('Router', ['navigateByUrl']);
const initialState = {
  currentAccountState: { currentAccount: new Account() },
  currentUserState: { currentUser: {} }
} as AppState;

describe('AccountProfileComponent', () => {
  let component: AccountProfileComponent;
  let fixture: ComponentFixture<AccountProfileComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [AccountProfileComponent],
      providers: [
        FormBuilder,
        provideMockStore({ initialState }),
        { provide: ActivatedRoute, useValue: new ActivatedRouteStub() },
        { provide: UserDataService, useValue: {} },
        { provide: AccountDataService, useValue: {} },
        { provide: ContactDataService, useValue: {} },
        { provide: DynamicsDataService, useValue: { getRecord: () => of(null) } },
        { provide: Router, useValue: routerSpy },
        { provide: TiedHouseConnectionsDataService, useValue: {} }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    fixture.destroy();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
