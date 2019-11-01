
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from '@angular/platform-browser';
import { DynamicsFormComponent } from './dynamics-form.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { DynamicsDataService } from '@services/dynamics-data.service';

let component: DynamicsFormComponent;
let fixture: ComponentFixture<DynamicsFormComponent>;
let dynamicsDataServiceStub: Partial<DynamicsDataService>;

describe('dynamics-form component', () => {
    dynamicsDataServiceStub = {};
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ DynamicsFormComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: DynamicsDataService, useValue: dynamicsDataServiceStub },
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ],
            schemas: [ NO_ERRORS_SCHEMA]
        });
        fixture = TestBed.createComponent(DynamicsFormComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
