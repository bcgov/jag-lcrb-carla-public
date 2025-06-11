import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeToALicenseeLegalEntityReviewComponent } from '@components/applications/permanent-change-to-a-licensee-legal-entity-review/permanent-change-to-a-licensee-legal-entity-review.component';

describe('PermanentChangeToALicenseeLegalEntityReviewComponent', () => {
  let component: PermanentChangeToALicenseeLegalEntityReviewComponent;
  let fixture: ComponentFixture<PermanentChangeToALicenseeLegalEntityReviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeToALicenseeLegalEntityReviewComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeToALicenseeLegalEntityReviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
