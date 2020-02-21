/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { EventFormComponent } from './event-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { LicenceEventsService } from '@services/licence-events.service';
import { provideMockStore } from '@ngrx/store/testing';
import { AppState } from '@app/app-state/models/app-state';
import { Router, ActivatedRoute } from '@angular/router';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { MatCheckboxModule } from '@angular/material';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
const routerSpy = jasmine.createSpyObj('Router', ['navigateByUrl']);


describe('EventFormComponent', () => {
  let component: EventFormComponent;
  let fixture: ComponentFixture<EventFormComponent>;
  const intialState = {

  } as AppState;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, MatCheckboxModule, NgbModule],
      declarations: [EventFormComponent],
      providers: [
        provideMockStore({}),
        { provide: Router, useValue: routerSpy },
        { provide: LicenceEventsService, useValue: {} },
        { provide: ActivatedRoute, useValue: new ActivatedRouteStub() }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EventFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => { fixture.destroy(); });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
