import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeToALicenseeComponent } from './permanent-change-to-a-licensee.component';

describe('PermanentChangeToALicenseeComponent', () => {
  let component: PermanentChangeToALicenseeComponent;
  let fixture: ComponentFixture<PermanentChangeToALicenseeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeToALicenseeComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeToALicenseeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
