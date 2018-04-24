import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { PolicyDocumentComponent } from './policy-document.component';

let component: PolicyDocumentComponent;
let fixture: ComponentFixture<PolicyDocumentComponent>;

describe('PolicyDocument component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ PolicyDocumentComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(PolicyDocumentComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
