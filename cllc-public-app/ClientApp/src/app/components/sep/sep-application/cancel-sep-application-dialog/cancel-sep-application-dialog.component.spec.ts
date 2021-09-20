import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CancelSepApplicationDialogComponent } from './cancel-sep-application-dialog.component';

describe('CancelSepApplicationDialogComponent', () => {
  let component: CancelSepApplicationDialogComponent;
  let fixture: ComponentFixture<CancelSepApplicationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CancelSepApplicationDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CancelSepApplicationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
