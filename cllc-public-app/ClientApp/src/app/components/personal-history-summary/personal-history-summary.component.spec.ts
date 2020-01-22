import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonalHistorySummaryComponent } from './personal-history-summary.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { ContactDataService } from '@services/contact-data.service';
import { of } from 'rxjs';
import { ActivatedRoute } from '@angular/router';



describe('PersonalHistorySummaryComponent', () => {
  let component: PersonalHistorySummaryComponent;
  let fixture: ComponentFixture<PersonalHistorySummaryComponent>;
  let activatedRouteStub = new ActivatedRouteStub({ token: 1 });

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PersonalHistorySummaryComponent],
      providers: [
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        {
          provide: ContactDataService, useValue: {
            getContactByPhsToken: () => of({})
          }
        },
        FormBuilder
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonalHistorySummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
}); 
