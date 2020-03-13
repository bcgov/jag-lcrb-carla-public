import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { StatusBadgeComponent } from './status-badge.component';

let component: StatusBadgeComponent;
let fixture: ComponentFixture<StatusBadgeComponent>;

describe('statusbadge component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ StatusBadgeComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(StatusBadgeComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
