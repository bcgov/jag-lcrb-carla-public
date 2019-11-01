import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { OrganizationLeadershipComponent } from './organization-leadership.component';
import { FormBuilder } from '@angular/forms';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material';

describe('OrganizationLeadershipComponent', () => {
  let component: OrganizationLeadershipComponent;
  let fixture: ComponentFixture<OrganizationLeadershipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [OrganizationLeadershipComponent],
      providers: [
        FormBuilder,
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: {} },
        { provide: MatDialog, useValue: {} }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrganizationLeadershipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
