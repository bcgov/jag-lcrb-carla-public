import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerHomeComponent } from './worker-home.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { PolicyDocumentDataService } from '@app/services/policy-document-data.service';
import { of } from 'rxjs';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material';
import { PolicyDocument } from '@models/policy-document.model';
import { PolicyDocumentComponent } from '@app/policy-document/policy-document.component';


const PolicyDocumentDataServiceStub: Partial<PolicyDocumentDataService> = {
  getPolicyDocument: () => of(null)
};
const activatedRouteStub: ActivatedRouteStub = new ActivatedRouteStub({});
const dialogStub: Partial<MatDialog> = {};

describe('WorkerHomeComponent', () => {
  let component: WorkerHomeComponent;
  let fixture: ComponentFixture<WorkerHomeComponent>;

  beforeEach(async(() => {
    activatedRouteStub.data = of(null);

    TestBed.configureTestingModule({
      declarations: [WorkerHomeComponent, PolicyDocumentComponent],
      providers: [
        { provide: MatDialog, useValue: dialogStub },
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: PolicyDocumentDataService, useValue: PolicyDocumentDataServiceStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
