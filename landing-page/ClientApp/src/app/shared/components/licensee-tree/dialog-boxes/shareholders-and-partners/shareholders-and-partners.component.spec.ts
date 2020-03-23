import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShareholdersAndPartnersComponent } from './shareholders-and-partners.component';
import { FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('ShareholdersAndPartnersComponent', () => {
  let component: ShareholdersAndPartnersComponent;
  let fixture: ComponentFixture<ShareholdersAndPartnersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShareholdersAndPartnersComponent ],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        FormBuilder,
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: {} },
        { provide: MatDialog, useValue: {} }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShareholdersAndPartnersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
