import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CancelDialogComponent } from '@components/police-representative/police-summary/cancel-dialog/cancel-dialog.component';

describe('CancelDialogComponent', () => {
  let component: CancelDialogComponent;
  let fixture: ComponentFixture<CancelDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CancelDialogComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CancelDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
