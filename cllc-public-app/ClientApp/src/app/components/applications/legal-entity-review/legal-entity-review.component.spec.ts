import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LegalEntityReviewComponent } from '@components/applications/legal-entity-review/legal-entity-review.component';

describe('LegalEntityReviewComponent', () => {
  let component: LegalEntityReviewComponent;
  let fixture: ComponentFixture<LegalEntityReviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LegalEntityReviewComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalEntityReviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
