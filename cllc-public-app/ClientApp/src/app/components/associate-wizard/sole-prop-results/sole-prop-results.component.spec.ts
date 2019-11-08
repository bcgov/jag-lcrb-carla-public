import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SolePropResultsComponent } from './sole-prop-results.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('SolePropResultsComponent', () => {
  let component: SolePropResultsComponent;
  let fixture: ComponentFixture<SolePropResultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SolePropResultsComponent ],
      schemas: [NO_ERRORS_SCHEMA]
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
