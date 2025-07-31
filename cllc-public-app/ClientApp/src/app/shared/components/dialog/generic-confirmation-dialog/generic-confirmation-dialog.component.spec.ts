import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GenericConfirmationDialogComponent } from '@shared/components/dialog/generic-confirmation-dialog/generic-confirmation-dialog.component';

describe('GenericConfirmationDialogComponent', () => {
  let component: GenericConfirmationDialogComponent;
  let fixture: ComponentFixture<GenericConfirmationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GenericConfirmationDialogComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GenericConfirmationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
