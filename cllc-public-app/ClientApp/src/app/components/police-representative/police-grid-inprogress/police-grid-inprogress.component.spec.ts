import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PoliceGridInProgressComponent } from './police-grid-inprogress.component';

describe('PoliceGridInProgressComponent', () => {
  let component: PoliceGridInProgressComponent;
  let fixture: ComponentFixture<PoliceGridInProgressComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PoliceGridInProgressComponent]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PoliceGridInProgressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
