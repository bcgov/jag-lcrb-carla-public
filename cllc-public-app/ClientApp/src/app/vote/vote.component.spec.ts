import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from '@angular/platform-browser';
import { VoteComponent } from './vote.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { VoteDataService } from '@services/vote-data.service';

let component: VoteComponent;
let fixture: ComponentFixture<VoteComponent>;
const coockieServiceStub: Partial<CookieService> = {};
const voteDataServiceStub: Partial<VoteDataService> = {};


describe('vote component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ VoteComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: VoteDataService, useValue: voteDataServiceStub },
                { provide: CookieService, useValue: coockieServiceStub },
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ],
            schemas: [NO_ERRORS_SCHEMA]
        });
        fixture = TestBed.createComponent(VoteComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
