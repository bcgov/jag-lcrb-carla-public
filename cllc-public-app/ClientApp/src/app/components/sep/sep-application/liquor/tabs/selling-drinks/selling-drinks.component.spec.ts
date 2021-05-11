import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SellingDrinksComponent } from './selling-drinks.component';

describe('SellingDrinksComponent', () => {
  let component: SellingDrinksComponent;
  let fixture: ComponentFixture<SellingDrinksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SellingDrinksComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SellingDrinksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
