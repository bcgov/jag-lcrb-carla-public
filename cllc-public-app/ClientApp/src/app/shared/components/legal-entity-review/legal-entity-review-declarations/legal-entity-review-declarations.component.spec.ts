import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LegalEntityReviewDeclarationsComponent } from '@shared/components/legal-entity-review/legal-entity-review-declarations/legal-entity-review-declarations.component';

describe('LegalEntityReviewDeclarationsComponent', () => {
  let component: LegalEntityReviewDeclarationsComponent;
  let fixture: ComponentFixture<LegalEntityReviewDeclarationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LegalEntityReviewDeclarationsComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalEntityReviewDeclarationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
