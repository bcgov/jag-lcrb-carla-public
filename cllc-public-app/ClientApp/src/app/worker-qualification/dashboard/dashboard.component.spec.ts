import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerDashboardComponent } from './dashboard.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { UserDataService } from '@services/user-data.service';
import { provideMockStore } from '@ngrx/store/testing';
import { WorkerDataService } from '@services/worker-data.service.';
import { MatTableModule } from '@angular/material';
import { AppState } from '@appapp-state/models/app-state';

const userDataServiceStub: Partial<UserDataService> = {};
const workerDataServiceStub: Partial<WorkerDataService> = {};
const initialState = {
  currentAccountState: { currentAccount: {} },
  currentUserState: { currentUser: {} }
} as AppState;

describe('WorkerDashboardComponent', () => {
  let component: WorkerDashboardComponent;
  let fixture: ComponentFixture<WorkerDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [WorkerDashboardComponent],
      imports: [MatTableModule],
      providers: [
        provideMockStore({initialState}),
        { provide: UserDataService, useValue: userDataServiceStub },
        { provide: WorkerDataService, useValue: workerDataServiceStub },
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
