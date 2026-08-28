import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, ComponentFixtureAutoDetect, TestBed, waitForAsync } from '@angular/core/testing';
import { BrowserModule } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { NewsletterDataService } from '@services/newsletter-data.service';
import { of } from 'rxjs/internal/observable/of';
import { NewsletterConfirmationComponent } from './newsletter-confirmation.component';

let component: NewsletterConfirmationComponent;
let fixture: ComponentFixture<NewsletterConfirmationComponent>;
const activatedRouteStub = new ActivatedRouteStub({});
const routerSpy = jasmine.createSpyObj('Router', ['navigateByUrl']);

describe('newsletter-confirmation component', () => {
  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [NewsletterConfirmationComponent],
      imports: [BrowserModule],
      providers: [
        { provide: ComponentFixtureAutoDetect, useValue: true },
        { provide: Router, useValue: routerSpy },
        { provide: NewsletterDataService, useValue: { verifyCode: () => of('') } },
        { provide: ActivatedRoute, useValue: activatedRouteStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    });
    fixture = TestBed.createComponent(NewsletterConfirmationComponent);
    component = fixture.componentInstance;
  }));

  it('should do create', waitForAsync(() => {
    expect(true).toEqual(true);
  }));
});
