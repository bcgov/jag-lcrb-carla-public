import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeNameLicenseePartnership } from '@shared/components/permanent-change/permanent-change-name-licensee-partnership/permanent-change-name-licensee-partnership.component';

describe('PermanentChangeNameLicenseePartnership', () => {
  let component: PermanentChangeNameLicenseePartnership;
  let fixture: ComponentFixture<PermanentChangeNameLicenseePartnership>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeNameLicenseePartnership]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeNameLicenseePartnership);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
