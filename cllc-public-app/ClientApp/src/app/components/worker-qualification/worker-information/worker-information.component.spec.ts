import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerInformationComponent } from './worker-information.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { PolicyDocumentComponent } from '@app/policy-document/policy-document.component';
import { PolicyDocumentDataService } from '@app/services/policy-document-data.service';
import { ActivatedRouteStub } from './../../testing/activated-route-stub';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

const PolicyDocumentDataServiceStub: Partial<PolicyDocumentDataService> = {
  getPolicyDocument: () => of(null)
};
const activatedRouteStub: ActivatedRouteStub = new ActivatedRouteStub({});

describe('WorkerInformationComponent', () => {
  let component: WorkerInformationComponent;
  let fixture: ComponentFixture<WorkerInformationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [WorkerInformationComponent, PolicyDocumentComponent],
      providers: [
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: PolicyDocumentDataService, useValue: PolicyDocumentDataServiceStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerInformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
