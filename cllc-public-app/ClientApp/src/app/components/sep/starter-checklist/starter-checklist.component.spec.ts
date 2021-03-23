import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StarterChecklistComponent } from './starter-checklist.component';

describe('StarterChecklistComponent', () => {
  let component: StarterChecklistComponent;
  let fixture: ComponentFixture<StarterChecklistComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StarterChecklistComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StarterChecklistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
