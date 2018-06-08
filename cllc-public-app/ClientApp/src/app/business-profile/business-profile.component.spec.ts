
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { BusinessProfileComponent } from './business-profile.component';

let component: BusinessProfileComponent;
let fixture: ComponentFixture<BusinessProfileComponent>;

describe('BusinessProfile component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ BusinessProfileComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(BusinessProfileComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
