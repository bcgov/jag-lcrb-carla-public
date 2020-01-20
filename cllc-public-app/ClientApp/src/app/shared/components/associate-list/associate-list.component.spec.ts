import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssociateListComponent } from './associate-list.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

describe('AssociateListComponent', () => {
  let component: AssociateListComponent;
  let fixture: ComponentFixture<AssociateListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssociateListComponent ],
      imports: [ReactiveFormsModule, FormsModule ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssociateListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
