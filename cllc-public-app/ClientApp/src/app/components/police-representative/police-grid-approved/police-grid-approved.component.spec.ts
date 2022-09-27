import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PoliceGridApprovedComponent } from './police-grid-approved.component';

describe('PoliceGridApprovedComponent', () => {
  let component: PoliceGridApprovedComponent;
  let fixture: ComponentFixture<PoliceGridApprovedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PoliceGridApprovedComponent]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PoliceGridApprovedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
