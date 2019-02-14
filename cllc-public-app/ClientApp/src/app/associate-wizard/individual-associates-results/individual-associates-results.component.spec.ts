import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IndividualAssociatesResultsComponent } from './individual-associates-results.component';

describe('IndividualAssociatesResultsComponent', () => {
  let component: IndividualAssociatesResultsComponent;
  let fixture: ComponentFixture<IndividualAssociatesResultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IndividualAssociatesResultsComponent ]
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
