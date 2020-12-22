import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PermanentChangesToALicenseeComponent } from './permanent-changes-to-a-licensee.component';

describe('PermanentChangesToALicenseeComponent', () => {
  let component: PermanentChangesToALicenseeComponent;
  let fixture: ComponentFixture<PermanentChangesToALicenseeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PermanentChangesToALicenseeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangesToALicenseeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
