import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangePersonalHistorySummaryFormsComponent } from '@shared/components/permanent-change/permanent-change-personal-history-summary-forms/permanent-change-personal-history-summary-forms.component';

describe('PermanentChangePersonalHistorySummaryFormsComponent', () => {
  let component: PermanentChangePersonalHistorySummaryFormsComponent;
  let fixture: ComponentFixture<PermanentChangePersonalHistorySummaryFormsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangePersonalHistorySummaryFormsComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangePersonalHistorySummaryFormsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
