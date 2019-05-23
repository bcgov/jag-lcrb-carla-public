import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganizationStructureComponent } from './organization-structure.component';

describe('OrganizationStructureComponent', () => {
  let component: OrganizationStructureComponent;
  let fixture: ComponentFixture<OrganizationStructureComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrganizationStructureComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrganizationStructureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
