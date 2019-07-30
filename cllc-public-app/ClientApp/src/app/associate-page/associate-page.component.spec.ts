import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssociatePageComponent } from './associate-page.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { MockStore, provideMockStore } from '@ngrx/store/testing';
import { AppState } from '@app/app-state/models/app-state';
import { ApplicationDataService } from '@app/services/application-data.service';
import { FormBuilder } from '@angular/forms';
import { Store } from '@ngrx/store';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { ActivatedRoute } from '@angular/router';

const activatedRouteStub: ActivatedRouteStub = new ActivatedRouteStub({ applicationId: 1 });

describe('AssociatePageComponent', () => {
  let component: AssociatePageComponent;
  let fixture: ComponentFixture<AssociatePageComponent>;
  let store: MockStore<AppState>;

  const initialState = {
    currentAccountState: { currentAccount: <any>{ businessType: 'PublicCorporation', legalEntity: {id: 1} } },
    currentUserState: { currentUser: {} }
  } as AppState;



  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AssociatePageComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        FormBuilder,
        provideMockStore({ initialState }),
        { provide: ActivatedRoute, useValue: activatedRouteStub }
      ]
    })
      .compileComponents();
    store = TestBed.get(Store);
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssociatePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
