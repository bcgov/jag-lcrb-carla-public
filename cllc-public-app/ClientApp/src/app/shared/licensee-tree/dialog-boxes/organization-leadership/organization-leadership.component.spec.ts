import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganizationLeadershipComponent } from './organization-leadership.component';

describe('OrganizationLeadershipComponent', () => {
  let component: OrganizationLeadershipComponent;
  let fixture: ComponentFixture<OrganizationLeadershipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrganizationLeadershipComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrganizationLeadershipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
