import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LiquorComponent } from './liquor.component';

describe('LiquorComponent', () => {
  let component: LiquorComponent;
  let fixture: ComponentFixture<LiquorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LiquorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LiquorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
