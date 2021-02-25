/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { LiquorFreeEventComponent } from './liquor-free-event.component';

describe('LiquorFreeEventComponent', () => {
  let component: LiquorFreeEventComponent;
  let fixture: ComponentFixture<LiquorFreeEventComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LiquorFreeEventComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LiquorFreeEventComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
