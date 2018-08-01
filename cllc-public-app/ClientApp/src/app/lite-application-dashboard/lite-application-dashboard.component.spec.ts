import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LiteApplicationDashboardComponent } from './lite-application-dashboard.component';

describe('LiteApplicationDashboardComponent', () => {
  let component: LiteApplicationDashboardComponent;
  let fixture: ComponentFixture<LiteApplicationDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LiteApplicationDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LiteApplicationDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
