import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PoliceGridDeniedComponent } from './police-grid-denied.component';

describe('PoliceGridDeniedComponent', () => {
  let component: PoliceGridDeniedComponent;
  let fixture: ComponentFixture<PoliceGridDeniedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PoliceGridDeniedComponent]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PoliceGridDeniedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
