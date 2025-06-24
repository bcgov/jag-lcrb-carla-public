import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LegalEntityReviewTiedHouseComponent } from '@shared/components/legal-entity-review/legal-entity-review-tied-house/legal-entity-review-tied-house.component';

describe('LegalEntityReviewTiedHouseComponent', () => {
  let component: LegalEntityReviewTiedHouseComponent;
  let fixture: ComponentFixture<LegalEntityReviewTiedHouseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LegalEntityReviewTiedHouseComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalEntityReviewTiedHouseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
