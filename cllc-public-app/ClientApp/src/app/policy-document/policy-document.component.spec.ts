import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By, Title, DomSanitizer } from '@angular/platform-browser';
import { PolicyDocumentComponent } from './policy-document.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { PolicyDocumentDataService } from '@services/policy-document-data.service';
import { ActivatedRoute } from '@angular/router';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { of } from 'rxjs';

let component: PolicyDocumentComponent;
let fixture: ComponentFixture<PolicyDocumentComponent>;

describe('PolicyDocument component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [PolicyDocumentComponent],
            imports: [BrowserModule],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true },
                {
                    provide: PolicyDocumentDataService, useValue: {
                        getPolicyDocument: () => of({})
                    }
                },
                { provide: Title, useValue: {} },
                { provide: ActivatedRoute, useValue: new ActivatedRouteStub() },
                {
                    provide: DomSanitizer, useValue: {
                        sanitize: () => 'safeString',
                        bypassSecurityTrustHtml: () => 'safeString'
                    }
                },
            ],
            schemas: [NO_ERRORS_SCHEMA]
        });
        fixture = TestBed.createComponent(PolicyDocumentComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
