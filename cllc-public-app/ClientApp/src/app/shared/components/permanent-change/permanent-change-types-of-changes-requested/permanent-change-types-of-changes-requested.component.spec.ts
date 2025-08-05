import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeTypesOfChangesRequestedComponent } from '@shared/components/permanent-change/permanent-change-types-of-changes-requested/permanent-change-types-of-changes-requested.component';

describe('PermanentChangeTypesOfChangesRequestedComponent', () => {
  let component: PermanentChangeTypesOfChangesRequestedComponent;
  let fixture: ComponentFixture<PermanentChangeTypesOfChangesRequestedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeTypesOfChangesRequestedComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeTypesOfChangesRequestedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
