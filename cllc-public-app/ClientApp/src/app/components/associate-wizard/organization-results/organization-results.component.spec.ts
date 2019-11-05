import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganizationResultsComponent } from './organization-results.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('OrganizationResultsComponent', () => {
  let component: OrganizationResultsComponent;
  let fixture: ComponentFixture<OrganizationResultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrganizationResultsComponent ],
      schemas: [ NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrganizationResultsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
