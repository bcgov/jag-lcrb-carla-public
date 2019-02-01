import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganizationResultsComponent } from './organization-results.component';

describe('OrganizationResultsComponent', () => {
  let component: OrganizationResultsComponent;
  let fixture: ComponentFixture<OrganizationResultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrganizationResultsComponent ]
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
