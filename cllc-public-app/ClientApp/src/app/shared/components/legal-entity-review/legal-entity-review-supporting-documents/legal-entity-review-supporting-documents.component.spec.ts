import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LegalEntityReviewSupportingDocumentsComponent } from '@shared/components/legal-entity-review/legal-entity-review-supporting-documents/legal-entity-review-supporting-documents.component';

describe('LegalEntityReviewSupportingDocumentsComponent', () => {
  let component: LegalEntityReviewSupportingDocumentsComponent;
  let fixture: ComponentFixture<LegalEntityReviewSupportingDocumentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LegalEntityReviewSupportingDocumentsComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalEntityReviewSupportingDocumentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
