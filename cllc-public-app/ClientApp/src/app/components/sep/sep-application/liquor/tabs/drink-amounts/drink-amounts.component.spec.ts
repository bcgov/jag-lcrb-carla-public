import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrinkAmountsComponent } from './drink-amounts.component';

describe('DrinkAmountsComponent', () => {
  let component: DrinkAmountsComponent;
  let fixture: ComponentFixture<DrinkAmountsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DrinkAmountsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DrinkAmountsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
