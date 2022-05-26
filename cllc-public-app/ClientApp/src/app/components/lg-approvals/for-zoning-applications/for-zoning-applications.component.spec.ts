import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForZoningApplicationsComponent } from './for-zoning-applications.component';

describe('ForZoningApplicationsComponent', () => {
  let component: ForZoningApplicationsComponent;
  let fixture: ComponentFixture<ForZoningApplicationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ForZoningApplicationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ForZoningApplicationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
