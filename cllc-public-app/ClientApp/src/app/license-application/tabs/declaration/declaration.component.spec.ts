import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeclarationComponent } from './declaration.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('DeclarationComponent', () => {
  let component: DeclarationComponent;
  let fixture: ComponentFixture<DeclarationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeclarationComponent ],
      schemas: [ NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeclarationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
