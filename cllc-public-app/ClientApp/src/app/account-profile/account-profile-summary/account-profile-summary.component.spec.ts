import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountProfileSummaryComponent } from './account-profile-summary.component';

describe('AccountProfileSummaryComponent', () => {
  let component: AccountProfileSummaryComponent;
  let fixture: ComponentFixture<AccountProfileSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountProfileSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountProfileSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
