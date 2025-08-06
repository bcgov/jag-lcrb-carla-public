import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeNameLicenseeSocitey } from '@shared/components/permanent-change/permanent-change-name-licensee-socitey/permanent-change-name-licensee-socitey.component';

describe('PermanentChangeNameLicenseeSocitey', () => {
  let component: PermanentChangeNameLicenseeSocitey;
  let fixture: ComponentFixture<PermanentChangeNameLicenseeSocitey>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeNameLicenseeSocitey]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeNameLicenseeSocitey);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
