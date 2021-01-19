/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { TuaEventComponent } from './tua-event.component';

describe('TuaEventComponent', () => {
  let component: TuaEventComponent;
  let fixture: ComponentFixture<TuaEventComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TuaEventComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TuaEventComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
