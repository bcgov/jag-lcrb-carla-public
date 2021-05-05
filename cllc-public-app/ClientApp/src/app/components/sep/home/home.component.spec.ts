/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SepHomeComponent } from './home.component';

describe('HomeComponent', () => {
  let component: SepHomeComponent;
  let fixture: ComponentFixture<SepHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SepHomeComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SepHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
