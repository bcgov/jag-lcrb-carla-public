import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SolePropResultsComponent } from './sole-prop-results.component';

describe('SolePropResultsComponent', () => {
  let component: SolePropResultsComponent;
  let fixture: ComponentFixture<SolePropResultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SolePropResultsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SolePropResultsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
