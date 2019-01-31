import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BusinessProfilePpComponent } from './business-profile-pp.component';

describe('BusinessProfilePpComponent', () => {
  let component: BusinessProfilePpComponent;
  let fixture: ComponentFixture<BusinessProfilePpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BusinessProfilePpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BusinessProfilePpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
