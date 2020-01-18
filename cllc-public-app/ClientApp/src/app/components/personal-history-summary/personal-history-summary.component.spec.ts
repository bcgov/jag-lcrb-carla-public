import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonalHistorySummaryComponent } from './personal-history-summary.component';

describe('PersonalHistorySummaryComponent', () => {
  let component: PersonalHistorySummaryComponent;
  let fixture: ComponentFixture<PersonalHistorySummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PersonalHistorySummaryComponent ]
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
