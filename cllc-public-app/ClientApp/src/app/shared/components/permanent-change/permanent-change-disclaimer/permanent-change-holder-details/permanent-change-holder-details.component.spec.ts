import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeHolderDetailsComponent } from '@shared/components/permanent-change/permanent-change-disclaimer/permanent-change-holder-details/permanent-change-holder-details.component';

describe('PermanentChangeHolderDetailsComponent', () => {
  let component: PermanentChangeHolderDetailsComponent;
  let fixture: ComponentFixture<PermanentChangeHolderDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeHolderDetailsComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeHolderDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
