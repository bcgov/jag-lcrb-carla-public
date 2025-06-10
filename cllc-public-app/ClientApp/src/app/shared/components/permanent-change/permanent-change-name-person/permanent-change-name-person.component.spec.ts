import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeNamePersonComponent } from '@shared/components/permanent-change/permanent-change-name-licensee-society/permanent-change-name-person.component';

describe('PermanentChangeNamePersonComponent', () => {
  let component: PermanentChangeNamePersonComponent;
  let fixture: ComponentFixture<PermanentChangeNamePersonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeNamePersonComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeNamePersonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
