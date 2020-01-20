import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonalHistorySummaryComponent } from './personal-history-summary.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('PersonalHistorySummaryComponent', () => {
  let component: PersonalHistorySummaryComponent;
  let fixture: ComponentFixture<PersonalHistorySummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PersonalHistorySummaryComponent ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonalHistorySummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
