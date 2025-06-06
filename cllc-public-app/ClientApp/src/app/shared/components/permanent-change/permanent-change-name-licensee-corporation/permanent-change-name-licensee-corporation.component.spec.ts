import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeNameLicenseeCorporationComponent } from '@shared/components/permanent-change/permanent-change-name-licensee-corporation/permanent-change-name-licensee-corporation.component';

describe('PermanentChangeNameLicenseeCorporationComponent', () => {
  let component: PermanentChangeNameLicenseeCorporationComponent;
  let fixture: ComponentFixture<PermanentChangeNameLicenseeCorporationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeNameLicenseeCorporationComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeNameLicenseeCorporationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
