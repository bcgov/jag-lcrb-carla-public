import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DirectorsAndOfficersComponent } from './directors-and-officers.component';

describe('DirectorsAndOfficersComponent', () => {
  let component: DirectorsAndOfficersComponent;
  let fixture: ComponentFixture<DirectorsAndOfficersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DirectorsAndOfficersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DirectorsAndOfficersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
