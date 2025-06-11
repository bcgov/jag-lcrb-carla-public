import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeLegalEntityReviewTypesOfChangesRequiredComponent } from '@shared/components/permanent-change/permanent-change-legal-entity-review-types-of-changes-required/permanent-change-legal-entity-review-types-of-changes-required.component';

describe('PermanentChangeLegalEntityReviewTypesOfChangesRequiredComponent', () => {
  let component: PermanentChangeLegalEntityReviewTypesOfChangesRequiredComponent;
  let fixture: ComponentFixture<PermanentChangeLegalEntityReviewTypesOfChangesRequiredComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeLegalEntityReviewTypesOfChangesRequiredComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeLegalEntityReviewTypesOfChangesRequiredComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
