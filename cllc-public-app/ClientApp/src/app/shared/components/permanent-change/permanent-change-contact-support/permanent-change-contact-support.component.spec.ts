import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeContactSupportComponent } from '@shared/components/permanent-change/permanent-change-contact-support/permanent-change-contact-support.component';

describe('PermanentChangeContactSupportComponent', () => {
  let component: PermanentChangeContactSupportComponent;
  let fixture: ComponentFixture<PermanentChangeContactSupportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeContactSupportComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeContactSupportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
