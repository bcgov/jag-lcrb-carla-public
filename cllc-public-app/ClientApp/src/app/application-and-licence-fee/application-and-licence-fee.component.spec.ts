import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationAndLicenceFeeComponent } from './application-and-licence-fee.component';

describe('ApplicationAndLicenceFeeComponent', () => {
  let component: ApplicationAndLicenceFeeComponent;
  let fixture: ComponentFixture<ApplicationAndLicenceFeeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApplicationAndLicenceFeeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationAndLicenceFeeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
