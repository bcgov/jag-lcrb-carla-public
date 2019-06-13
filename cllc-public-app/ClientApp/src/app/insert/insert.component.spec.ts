import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InsertComponent } from './insert.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { StaticComponent } from '@appstatic/static.component';
import { SurveySidebarComponent } from '@appsurvey/sidebar.component';
import { InsertService } from './insert.service';

const insertServiceStub: Partial<InsertService> = {};

describe('InsertComponent', () => {
  let component: InsertComponent;
  let fixture: ComponentFixture<InsertComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InsertComponent, StaticComponent, SurveySidebarComponent ],
      providers: [
        { provide: InsertService, useValue: insertServiceStub}
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InsertComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
