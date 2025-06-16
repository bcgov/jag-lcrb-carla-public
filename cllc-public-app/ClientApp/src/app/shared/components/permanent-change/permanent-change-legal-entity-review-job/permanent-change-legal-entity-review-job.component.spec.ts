import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeLegalEntityReviewJobComponent } from '@shared/components/permanent-change/permanent-change-legal-entity-review-job/permanent-change-legal-entity-review-job.component';

describe('PermanentChangeLegalEntityReviewJobComponent', () => {
  let component: PermanentChangeLegalEntityReviewJobComponent;
  let fixture: ComponentFixture<PermanentChangeLegalEntityReviewJobComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeLegalEntityReviewJobComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeLegalEntityReviewJobComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
