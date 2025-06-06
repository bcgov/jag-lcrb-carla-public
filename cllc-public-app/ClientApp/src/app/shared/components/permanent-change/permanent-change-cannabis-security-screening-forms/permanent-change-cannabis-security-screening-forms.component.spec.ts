import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeCannabisSecurityScreeningFormsComponent } from '@shared/components/permanent-change/permanent-change-cannabis-security-screening-forms/permanent-change-cannabis-security-screening-forms.component';

describe('PermanentChangeCannabisSecurityScreeningFormsComponent', () => {
  let component: PermanentChangeCannabisSecurityScreeningFormsComponent;
  let fixture: ComponentFixture<PermanentChangeCannabisSecurityScreeningFormsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeCannabisSecurityScreeningFormsComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeCannabisSecurityScreeningFormsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
