import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubmittedApplicationsComponent } from './submitted-applications.component';

describe('SubmittedApplicationsComponent', () => {
  let component: SubmittedApplicationsComponent;
  let fixture: ComponentFixture<SubmittedApplicationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SubmittedApplicationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SubmittedApplicationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
