import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { PolicyDocumentDataService } from '@app/services/policy-document-data.service';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { PolicyDocumentComponent } from '@components/policy-document/policy-document.component';
import { of } from 'rxjs';
import { WorkerHomeComponent } from './worker-home.component';

const PolicyDocumentDataServiceStub: Partial<PolicyDocumentDataService> = {
  getPolicyDocument: () => of(null)
};
const activatedRouteStub = new ActivatedRouteStub({});
const dialogStub: Partial<MatDialog> = {};

describe('WorkerHomeComponent', () => {
  let component: WorkerHomeComponent;
  let fixture: ComponentFixture<WorkerHomeComponent>;

  beforeEach(waitForAsync(() => {
    activatedRouteStub.data = of(null);

    TestBed.configureTestingModule({
      declarations: [WorkerHomeComponent, PolicyDocumentComponent],
      providers: [
        { provide: MatDialog, useValue: dialogStub },
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: PolicyDocumentDataService, useValue: PolicyDocumentDataServiceStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
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
