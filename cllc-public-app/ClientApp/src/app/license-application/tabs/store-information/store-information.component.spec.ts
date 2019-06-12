import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StoreInformationComponent } from './store-information.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('StoreInformationComponent', () => {
  let component: StoreInformationComponent;
  let fixture: ComponentFixture<StoreInformationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StoreInformationComponent ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StoreInformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
