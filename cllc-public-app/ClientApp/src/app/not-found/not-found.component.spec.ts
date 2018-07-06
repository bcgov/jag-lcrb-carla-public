
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { NotFoundComponent } from './not-found.component';

let component: NotFoundComponent;
let fixture: ComponentFixture<NotFoundComponent>;

describe('NotFound component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ NotFoundComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(NotFoundComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
