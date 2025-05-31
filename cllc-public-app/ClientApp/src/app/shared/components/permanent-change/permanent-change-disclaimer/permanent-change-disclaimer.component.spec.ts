import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeDisclaimerComponent } from '@shared/components/permanent-change/permanent-change-disclaimer/permanent-change-disclaimer.component';

describe('PermanentChangeDisclaimerComponent', () => {
  let component: PermanentChangeDisclaimerComponent;
  let fixture: ComponentFixture<PermanentChangeDisclaimerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeDisclaimerComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeDisclaimerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
