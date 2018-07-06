import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { PolicyDocumentSidebarComponent } from './policy-document-sidebar.component';

let component: PolicyDocumentSidebarComponent;
let fixture: ComponentFixture<PolicyDocumentSidebarComponent>;

describe('PolicyDocumentSidebar component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ PolicyDocumentSidebarComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(PolicyDocumentSidebarComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
