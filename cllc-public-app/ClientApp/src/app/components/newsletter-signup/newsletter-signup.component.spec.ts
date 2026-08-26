// <reference path="../../../../node_modules/@types/jasmine/index.d.ts" />
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, ComponentFixtureAutoDetect, TestBed, waitForAsync } from '@angular/core/testing';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BrowserModule } from '@angular/platform-browser';
import { NewsletterDataService } from '@services/newsletter-data.service';
import { NewsletterSignupComponent } from './newsletter-signup.component';

let component: NewsletterSignupComponent;
let fixture: ComponentFixture<NewsletterSignupComponent>;

describe('newsletter-signup component', () => {
  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [NewsletterSignupComponent],
      imports: [BrowserModule],
      providers: [
        { provide: ComponentFixtureAutoDetect, useValue: true },
        { provide: MatSnackBar, useValue: {} },
        { provide: NewsletterDataService, useValue: {} }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    });
    fixture = TestBed.createComponent(NewsletterSignupComponent);
    component = fixture.componentInstance;
  }));

  it('should do something', waitForAsync(() => {
    expect(true).toEqual(true);
  }));
});
