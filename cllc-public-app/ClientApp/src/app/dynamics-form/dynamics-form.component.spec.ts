
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { DynamicsFormComponent } from './dynamics-form.component';

let component: DynamicsFormComponent;
let fixture: ComponentFixture<DynamicsFormComponent>;

describe('dynamics-form component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ DynamicsFormComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(DynamicsFormComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
