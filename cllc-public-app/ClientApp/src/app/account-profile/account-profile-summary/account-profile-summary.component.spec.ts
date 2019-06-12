import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountProfileSummaryComponent } from './account-profile-summary.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { ActivatedRoute } from '@angular/router';

describe('AccountProfileSummaryComponent', () => {
  let component: AccountProfileSummaryComponent;
  let fixture: ComponentFixture<AccountProfileSummaryComponent>;
  const activatedRouteStub = new ActivatedRouteStub({ applicationId: 1 });

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountProfileSummaryComponent ],
      providers: [
        {provide: ActivatedRoute, useValue: activatedRouteStub}
      ],
      schemas: [ NO_ERRORS_SCHEMA ]
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
