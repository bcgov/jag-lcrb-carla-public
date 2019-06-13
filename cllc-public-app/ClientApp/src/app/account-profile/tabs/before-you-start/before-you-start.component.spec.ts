import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BeforeYouStartComponent } from './before-you-start.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { MockStore, provideMockStore } from '@ngrx/store/testing';
import { AppState } from '@app/app-state/models/app-state';
let store: MockStore<AppState>;

describe('BeforeYouStartComponent', () => {
  let component: BeforeYouStartComponent;
  let fixture: ComponentFixture<BeforeYouStartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [BeforeYouStartComponent],
      providers: [provideMockStore({})],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BeforeYouStartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

});
