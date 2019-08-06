import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationRenewalComponent } from './application-renewal.component';

describe('ApplicationRenewalComponent', () => {
  let component: ApplicationRenewalComponent;
  let fixture: ComponentFixture<ApplicationRenewalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApplicationRenewalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationRenewalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
