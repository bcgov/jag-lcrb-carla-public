/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { BcscProfileComponent } from './bcsc-profile.component';

describe('BcscProfileComponent', () => {
  let component: BcscProfileComponent;
  let fixture: ComponentFixture<BcscProfileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BcscProfileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BcscProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
