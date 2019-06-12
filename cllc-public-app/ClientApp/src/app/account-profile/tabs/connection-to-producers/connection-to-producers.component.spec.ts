import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConnectionToProducersComponent } from './connection-to-producers.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('ConnectionToProducersComponent', () => {
  let component: ConnectionToProducersComponent;
  let fixture: ComponentFixture<ConnectionToProducersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConnectionToProducersComponent ],
      schemas: [ NO_ERRORS_SCHEMA ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConnectionToProducersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
