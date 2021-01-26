/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { EventLocationTableComponent } from './event-location-table.component';

describe('TuaTableComponent', () => {
  let component: EventLocationTableComponent;
  let fixture: ComponentFixture<EventLocationTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [EventLocationTableComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EventLocationTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
