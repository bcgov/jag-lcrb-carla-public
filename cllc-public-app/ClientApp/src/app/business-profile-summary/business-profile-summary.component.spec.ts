import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BusinessProfileSummaryComponent } from './business-profile-summary.component';

describe('BusinessProfileSummaryComponent', () => {
  let component: BusinessProfileSummaryComponent;
  let fixture: ComponentFixture<BusinessProfileSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BusinessProfileSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BusinessProfileSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
