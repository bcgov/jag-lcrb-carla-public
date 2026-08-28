import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, ComponentFixtureAutoDetect, TestBed, waitForAsync } from '@angular/core/testing';
import { BrowserModule, DomSanitizer, Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { PolicyDocumentDataService } from '@services/policy-document-data.service';
import { of } from 'rxjs';
import { PolicyDocumentComponent } from './policy-document.component';

let component: PolicyDocumentComponent;
let fixture: ComponentFixture<PolicyDocumentComponent>;

describe('PolicyDocument component', () => {
  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PolicyDocumentComponent],
      imports: [BrowserModule],
      providers: [
        { provide: ComponentFixtureAutoDetect, useValue: true },
        {
          provide: PolicyDocumentDataService,
          useValue: {
            getPolicyDocument: () => of({})
          }
        },
        { provide: Title, useValue: {} },
        { provide: ActivatedRoute, useValue: new ActivatedRouteStub() },
        {
          provide: DomSanitizer,
          useValue: {
            sanitize: () => 'safeString',
            bypassSecurityTrustHtml: () => 'safeString'
          }
        }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    });
    fixture = TestBed.createComponent(PolicyDocumentComponent);
    component = fixture.componentInstance;
  }));

  it('should do something', waitForAsync(() => {
    expect(true).toEqual(true);
  }));
});
