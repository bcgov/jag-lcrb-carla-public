import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeclarationComponent } from './declaration.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ApplicationDataService } from '@services/application-data.service';
import { ActivatedRouteStub } from './../../../testing/activated-route-stub';
import { MatSnackBar } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { provideMockStore } from '@ngrx/store/testing';

const applicationDataServiceStub: Partial<ApplicationDataService> = {};
const activatedRouteStub = new ActivatedRouteStub();
const matSnackBarStub: Partial<MatSnackBar> = {};

describe('DeclarationComponent', () => {
  let component: DeclarationComponent;
  let fixture: ComponentFixture<DeclarationComponent>;

  beforeEach(async(() => {
    activatedRouteStub.parent = activatedRouteStub;
    TestBed.configureTestingModule({
      declarations: [DeclarationComponent],
      providers: [
        provideMockStore({}),
        { provide: MatSnackBar, useValue: matSnackBarStub },
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: ApplicationDataService, useValue: applicationDataServiceStub },
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeclarationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
