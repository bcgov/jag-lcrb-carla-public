import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IndividualAssociatesResultsComponent } from './individual-associates-results.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('IndividualAssociatesResultsComponent', () => {
  let component: IndividualAssociatesResultsComponent;
  let fixture: ComponentFixture<IndividualAssociatesResultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IndividualAssociatesResultsComponent ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IndividualAssociatesResultsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
