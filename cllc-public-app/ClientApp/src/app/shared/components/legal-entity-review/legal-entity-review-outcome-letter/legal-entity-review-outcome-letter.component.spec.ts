import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LegalEntityReviewOutcomeLetterComponent } from '@shared/components/legal-entity-review/legal-entity-review-outcome-letter/legal-entity-review-outcome-letter.component';

describe('LegalEntityReviewOutcomeLetterComponent', () => {
  let component: LegalEntityReviewOutcomeLetterComponent;
  let fixture: ComponentFixture<LegalEntityReviewOutcomeLetterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LegalEntityReviewOutcomeLetterComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalEntityReviewOutcomeLetterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
