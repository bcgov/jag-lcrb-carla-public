import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountCompletenessComponent } from './account-completeness.component';

describe('AccountCompletenessComponent', () => {
  let component: AccountCompletenessComponent;
  let fixture: ComponentFixture<AccountCompletenessComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountCompletenessComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountCompletenessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
