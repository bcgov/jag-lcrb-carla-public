import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationTiedHouseExemptionComponent } from './application-tied-house-exemption.component';

describe('ApplicationTiedHouseExemptionComponent', () => {
  let component: ApplicationTiedHouseExemptionComponent;
  let fixture: ComponentFixture<ApplicationTiedHouseExemptionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApplicationTiedHouseExemptionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationTiedHouseExemptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
