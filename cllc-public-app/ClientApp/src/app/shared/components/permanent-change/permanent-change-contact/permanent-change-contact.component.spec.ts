import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeContactComponent } from '@shared/components/permanent-change/permanent-change-contact/permanent-change-contact.component';

describe('PermanentChangeContactComponent', () => {
  let component: PermanentChangeContactComponent;
  let fixture: ComponentFixture<PermanentChangeContactComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeContactComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeContactComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
