import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LicenceEventComponent } from './licence-event.component';

describe('LicenceEventComponent', () => {
  let component: LicenceEventComponent;
  let fixture: ComponentFixture<LicenceEventComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LicenceEventComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenceEventComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
