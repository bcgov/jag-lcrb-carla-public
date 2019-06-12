import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationsAndLicencesComponent } from './applications-and-licences.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('ApplicationsAndLicencesComponent', () => {
  let component: ApplicationsAndLicencesComponent;
  let fixture: ComponentFixture<ApplicationsAndLicencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApplicationsAndLicencesComponent ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationsAndLicencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
