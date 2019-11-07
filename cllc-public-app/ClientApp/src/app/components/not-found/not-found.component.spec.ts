
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule } from '@angular/platform-browser';
import { NotFoundComponent } from './not-found.component';
import { Router } from '@angular/router';

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
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
