import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssociatesDashboardComponent } from './associates-dashboard.component';

describe('AssociateDashboardComponent', () => {
  let component: AssociatesDashboardComponent;
  let fixture: ComponentFixture<AssociatesDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssociatesDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssociatesDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
