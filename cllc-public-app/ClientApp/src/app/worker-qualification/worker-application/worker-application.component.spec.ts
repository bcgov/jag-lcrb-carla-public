import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { WorkerApplicationComponent } from './worker-application.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { provideMockStore } from '@ngrx/store/testing';
import { ActivatedRouteStub } from './../../testing/activated-route-stub';
import { UserDataService } from '@services/user-data.service';
import { AliasDataService } from '@services/alias-data.service';
import { PreviousAddressDataService } from '@services/previous-address-data.service';
import { ContactDataService } from '@services/contact-data.service';
import { WorkerDataService } from '@services/worker-data.service.';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { of } from 'rxjs';

const userDataServiceStub: Partial<UserDataService> = {
  getCurrentUser: () =>  of(null)
};
const aliasDataServiceStupb: Partial<AliasDataService> = {};
const previousAddressDataServiceStub: Partial<PreviousAddressDataService> = {};
const contactDataServiceStub: Partial<ContactDataService> = {};
const workerDataServiceStub: Partial<WorkerDataService> = {};
const routeStub =  new ActivatedRouteStub();
const routerSpy = jasmine.createSpyObj('Router', ['navigateByUrl']);

describe('WorkerApplicationComponent', () => {
  let component: WorkerApplicationComponent;
  let fixture: ComponentFixture<WorkerApplicationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkerApplicationComponent ],
      providers: [
        provideMockStore({}),
        FormBuilder,
        {provide: ActivatedRoute, useValue: routeStub},
        {provide: Router, useValue: routerSpy},
        {provide: UserDataService, useValue: userDataServiceStub},
        {provide: AliasDataService, useValue: aliasDataServiceStupb},
        {provide: ContactDataService, useValue: contactDataServiceStub},
        {provide: WorkerDataService, useValue: workerDataServiceStub},
        {provide: PreviousAddressDataService, useValue: previousAddressDataServiceStub},
      ],
      schemas: [ NO_ERRORS_SCHEMA ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerApplicationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
