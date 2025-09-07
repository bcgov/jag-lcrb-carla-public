import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LegalEntityReviewPermanentChangeToALicenseeComponent } from '@components/applications/legal-entity-review-permanent-change-to-a-licensee/legal-entity-review-permanent-change-to-a-licensee.component';

describe('LegalEntityReviewPermanentChangeToALicenseeComponent', () => {
  let component: LegalEntityReviewPermanentChangeToALicenseeComponent;
  let fixture: ComponentFixture<LegalEntityReviewPermanentChangeToALicenseeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LegalEntityReviewPermanentChangeToALicenseeComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalEntityReviewPermanentChangeToALicenseeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
