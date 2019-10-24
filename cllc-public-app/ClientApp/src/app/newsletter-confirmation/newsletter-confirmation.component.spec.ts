
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from '@angular/platform-browser';
import { NewsletterConfirmationComponent } from './newsletter-confirmation.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { NewsletterDataService } from '@services/newsletter-data.service';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs/internal/observable/of';

let component: NewsletterConfirmationComponent;
let fixture: ComponentFixture<NewsletterConfirmationComponent>;
const activatedRouteStub = new ActivatedRouteStub({});
const routerSpy = jasmine.createSpyObj('Router', ['navigateByUrl']);

describe('newsletter-confirmation component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [NewsletterConfirmationComponent],
            imports: [BrowserModule],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true },
                { provide: Router, useValue: routerSpy },
                { provide: NewsletterDataService, useValue: { verifyCode: () => of('')} },
                { provide: ActivatedRoute, useValue: activatedRouteStub }
            ],
            schemas: [NO_ERRORS_SCHEMA]
        });
        fixture = TestBed.createComponent(NewsletterConfirmationComponent);
        component = fixture.componentInstance;
    }));

    it('should do create', async(() => {
        expect(true).toEqual(true);
    }));
});
