import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SepApplicationComponent } from './sep-application.component';

describe('SepApplicationComponent', () => {
  let component: SepApplicationComponent;
  let fixture: ComponentFixture<SepApplicationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SepApplicationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SepApplicationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
