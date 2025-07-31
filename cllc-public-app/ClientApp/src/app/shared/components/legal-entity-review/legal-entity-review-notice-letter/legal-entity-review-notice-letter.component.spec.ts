import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LegalEntityReviewNoticeLetterComponent } from '@shared/components/legal-entity-review/legal-entity-review-notice-letter/legal-entity-review-notice-letter.component';

describe('LegalEntityReviewNoticeLetterComponent', () => {
  let component: LegalEntityReviewNoticeLetterComponent;
  let fixture: ComponentFixture<LegalEntityReviewNoticeLetterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LegalEntityReviewNoticeLetterComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalEntityReviewNoticeLetterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
