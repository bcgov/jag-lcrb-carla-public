
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from '@angular/platform-browser';
import { NotFoundComponent } from './not-found.component';
import { ActivatedRouteStub } from './../testing/activated-route-stub';
import { ActivatedRoute, Router } from '@angular/router';

let component: NotFoundComponent;
let fixture: ComponentFixture<NotFoundComponent>;
const routerSpy = jasmine.createSpyObj('Router', ['navigateByUrl']);

describe('NotFound component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [NotFoundComponent],
            imports: [BrowserModule],
            providers: [
                { provide: Router, useValue: routerSpy },
                { provide: ComponentFixtureAutoDetect, useValue: true },
            ]
        });
        fixture = TestBed.createComponent(NotFoundComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
