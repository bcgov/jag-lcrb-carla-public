import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeDirectorsOfficers } from '@shared/components/permanent-change/permanent-change-directors-officers/permanent-change-directors-officers.component';

describe('PermanentChangeDirectorsOfficers', () => {
  let component: PermanentChangeDirectorsOfficers;
  let fixture: ComponentFixture<PermanentChangeDirectorsOfficers>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeDirectorsOfficers]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeDirectorsOfficers);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
