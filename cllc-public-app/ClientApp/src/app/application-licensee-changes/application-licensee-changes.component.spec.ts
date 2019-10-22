import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationLicenseeChangesComponent } from './application-licensee-changes.component';

describe('ApplicationLicenseeChangesComponent', () => {
  let component: ApplicationLicenseeChangesComponent;
  let fixture: ComponentFixture<ApplicationLicenseeChangesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApplicationLicenseeChangesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationLicenseeChangesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
