import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { SurveyDataService } from '@services/survey-data.service';
import { of } from 'rxjs';
import { ResultComponent } from './result.component';

const surveyDataServiceStub: Partial<SurveyDataService> = {
  getSurveyData: () => of(null)
};
const activateRouteStub = new ActivatedRouteStub();

describe('ResultComponent', () => {
  let component: ResultComponent;
  let fixture: ComponentFixture<ResultComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ResultComponent],
      providers: [
        { provide: ActivatedRoute, useValue: activateRouteStub },
        { provide: SurveyDataService, useValue: surveyDataServiceStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
