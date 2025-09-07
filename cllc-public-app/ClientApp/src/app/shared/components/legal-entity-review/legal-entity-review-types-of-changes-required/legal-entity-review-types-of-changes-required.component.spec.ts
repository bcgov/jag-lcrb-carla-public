import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LegalEntityReviewTypesOfChangesRequiredComponent } from '@shared/components/legal-entity-review/legal-entity-review-types-of-changes-required/legal-entity-review-types-of-changes-required.component';

describe('LegalEntityReviewTypesOfChangesRequiredComponent', () => {
  let component: LegalEntityReviewTypesOfChangesRequiredComponent;
  let fixture: ComponentFixture<LegalEntityReviewTypesOfChangesRequiredComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LegalEntityReviewTypesOfChangesRequiredComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalEntityReviewTypesOfChangesRequiredComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
