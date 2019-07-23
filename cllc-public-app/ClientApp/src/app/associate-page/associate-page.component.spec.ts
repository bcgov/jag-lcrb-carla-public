import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssociatePageComponent } from './associate-page.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('AssociatePageComponent', () => {
  let component: AssociatePageComponent;
  let fixture: ComponentFixture<AssociatePageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssociatePageComponent ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssociatePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
