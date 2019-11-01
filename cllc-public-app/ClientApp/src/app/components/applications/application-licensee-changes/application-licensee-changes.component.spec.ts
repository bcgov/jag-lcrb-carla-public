import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationLicenseeChangesComponent } from './application-licensee-changes.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { LicenseeTreeComponent } from '@shared/licensee-tree/licensee-tree.component';
import { MatMenuModule, MatTreeModule, MatDialogModule } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { ApplicationDataService } from '@services/application-data.service';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { of } from 'rxjs/internal/observable/of';

describe('ApplicationLicenseeChangesComponent', () => {
  let component: ApplicationLicenseeChangesComponent;
  let fixture: ComponentFixture<ApplicationLicenseeChangesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ApplicationLicenseeChangesComponent, LicenseeTreeComponent],
      imports: [MatMenuModule, MatTreeModule, MatDialogModule],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        { provide: ActivatedRoute, useValue: new ActivatedRouteStub({}) },
        { provide: LegalEntityDataService, useValue: {getChangeLogs: () => of([]), getCurrentHierachy: () => of({})} },
        { provide: ApplicationDataService, useValue: {getApplicationById: () => of({}) } },
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationLicenseeChangesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
