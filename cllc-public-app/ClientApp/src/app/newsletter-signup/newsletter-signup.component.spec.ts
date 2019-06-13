// <reference path="../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from '@angular/platform-browser';
import { NewsletterSignupComponent } from './newsletter-signup.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { MatSnackBar } from '@angular/material';
import { NewsletterDataService } from '@services/newsletter-data.service';

let component: NewsletterSignupComponent;
let fixture: ComponentFixture<NewsletterSignupComponent>;

describe('newsletter-signup component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ NewsletterSignupComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true },
                { provide: MatSnackBar, useValue: {} },
                { provide: NewsletterDataService, useValue: {} },
            ],
            schemas: [NO_ERRORS_SCHEMA]
        });
        fixture = TestBed.createComponent(NewsletterSignupComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
