/// <reference path="../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { NewsletterSignupComponent } from './newsletter-signup.component';

let component: NewsletterSignupComponent;
let fixture: ComponentFixture<NewsletterSignupComponent>;

describe('newsletter-signup component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ NewsletterSignupComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(NewsletterSignupComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});