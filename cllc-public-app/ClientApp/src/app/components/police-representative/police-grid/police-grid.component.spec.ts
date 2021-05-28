import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PoliceGridComponent } from './police-grid.component';

describe('PoliceGridComponent', () => {
  let component: PoliceGridComponent;
  let fixture: ComponentFixture<PoliceGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PoliceGridComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PoliceGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
