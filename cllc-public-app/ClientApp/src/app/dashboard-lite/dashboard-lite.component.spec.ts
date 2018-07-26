import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardLiteComponent } from './dashboard-lite.component';

describe('DashboardLiteComponent', () => {
  let component: DashboardLiteComponent;
  let fixture: ComponentFixture<DashboardLiteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardLiteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardLiteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
