import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountCompletenessComponent } from './account-completeness.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FileDataService } from '@services/file-data.service';

const httpClientSpy: { get: jasmine.Spy } = jasmine.createSpyObj('HttpClient', ['get']);

describe('AccountCompletenessComponent', () => {
  let component: AccountCompletenessComponent;
  let fixture: ComponentFixture<AccountCompletenessComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountCompletenessComponent ],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {provide: HttpClient, useValue: httpClientSpy},
        {provide: FileDataService, useValue: {}},
      ]
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
