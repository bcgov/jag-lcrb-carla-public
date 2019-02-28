import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationsAndLicencesComponent } from './applications-and-licences.component';

describe('ApplicationsAndLicencesComponent', () => {
  let component: ApplicationsAndLicencesComponent;
  let fixture: ComponentFixture<ApplicationsAndLicencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApplicationsAndLicencesComponent ]
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
