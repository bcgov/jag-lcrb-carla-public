import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegalEntityComponent } from './legal-entity.component';
import { MatTableDataSource, MatDialog, MatSnackBar } from '@angular/material';
import { DynamicsDataService } from '../../../services/dynamics-data.service';
import { FormBuilder } from '@angular/forms';
import { AppState } from '../../../app-state/models/app-state';
import { ActivatedRouteStub } from '../../../testing/activated-route-stub';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { LegalEntityDataService } from '../../../services/legal-entity-data.service';
import { LegalEntity } from '../../../models/legal-entity.model';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { provideMockStore } from '@ngrx/store/testing';

let legalEntityDataServiceStub: Partial<LegalEntityDataService>;
let dynamicsDataServiceStub: Partial<DynamicsDataService>;
let matDialogStub: Partial<MatDialog>;
let matSnackBarStub: Partial<MatSnackBar>;
let activatedRouteStub: ActivatedRouteStub;

describe('LegalEntityComponent', () => {
  let component: LegalEntityComponent;
  let fixture: ComponentFixture<LegalEntityComponent>;


  const initialState = {
    currentAccountState: { currentAccount: { businessType: 'PublicCorporation' } },
    currentUserState: { currentUser: {} }
  } as AppState;


  beforeEach(async(() => {
    legalEntityDataServiceStub = {
      getLegalEntitiesbyPosition: (parentLegalEntityId, positionType: string) => of(<LegalEntity[]>{}),
      getBusinessProfileSummary: () => of(<LegalEntity[]>{}),
    };
    dynamicsDataServiceStub = { getRecord: () => of([]) };
    matDialogStub = {};
    matSnackBarStub = {};
    activatedRouteStub = new ActivatedRouteStub({ applicationId: 1 });

    TestBed.configureTestingModule({
      declarations: [LegalEntityComponent],
      providers: [
        provideMockStore({ initialState }),
        FormBuilder,
        { provide: LegalEntityDataService, useValue: legalEntityDataServiceStub },
        { provide: MatDialog, useValue: matDialogStub },
        { provide: DynamicsDataService, useValue: dynamicsDataServiceStub },
        
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: MatSnackBar, useValue: matSnackBarStub },
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalEntityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  fit('should create', () => {
    expect(component).toBeTruthy();
  });
});
