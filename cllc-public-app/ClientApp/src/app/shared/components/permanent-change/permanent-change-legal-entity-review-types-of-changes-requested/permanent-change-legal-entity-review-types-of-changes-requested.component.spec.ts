import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeLegalEntityReviewTypesOfChangesRequestedComponent } from '@shared/components/permanent-change/permanent-change-legal-entity-review-types-of-changes-requested/permanent-change-legal-entity-review-types-of-changes-requested.component';

describe('PermanentChangeLegalEntityReviewTypesOfChangesRequestedComponent', () => {
  let component: PermanentChangeLegalEntityReviewTypesOfChangesRequestedComponent;
  let fixture: ComponentFixture<PermanentChangeLegalEntityReviewTypesOfChangesRequestedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeLegalEntityReviewTypesOfChangesRequestedComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeLegalEntityReviewTypesOfChangesRequestedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
