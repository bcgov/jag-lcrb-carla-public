import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TotalServingsComponent } from './total-servings.component';

describe('TotalServingsComponent', () => {
  let component: TotalServingsComponent;
  let fixture: ComponentFixture<TotalServingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TotalServingsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TotalServingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
