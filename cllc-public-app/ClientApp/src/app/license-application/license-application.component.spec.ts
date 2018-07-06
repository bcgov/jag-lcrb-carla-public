import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LicenseApplicationComponent } from './license-application.component';

describe('LicenseApplicationComponent', () => {
  let component: LicenseApplicationComponent;
  let fixture: ComponentFixture<LicenseApplicationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LicenseApplicationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseApplicationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
