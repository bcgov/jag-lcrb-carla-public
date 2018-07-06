
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { FormViewerComponent } from './form-viewer.component';

let component: FormViewerComponent;
let fixture: ComponentFixture<FormViewerComponent>;

describe('form-viewer component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ FormViewerComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(FormViewerComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
