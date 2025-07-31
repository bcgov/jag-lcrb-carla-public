import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LegalEntityReviewJobComponent } from '@shared/components/legal-entity-review/legal-entity-review-job/legal-entity-review-job.component';

describe('LegalEntityReviewJobComponent', () => {
  let component: LegalEntityReviewJobComponent;
  let fixture: ComponentFixture<LegalEntityReviewJobComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LegalEntityReviewJobComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalEntityReviewJobComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
