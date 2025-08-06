import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeDeclarationsComponent } from '@shared/components/permanent-change/permanent-change-declarations/permanent-change-declarations.component';

describe('PermanentChangeDeclarationsComponent', () => {
  let component: PermanentChangeDeclarationsComponent;
  let fixture: ComponentFixture<PermanentChangeDeclarationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeDeclarationsComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeDeclarationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
