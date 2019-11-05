
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from '@angular/platform-browser';
import { FormViewerComponent } from './form-viewer.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DynamicsDataService } from '@services/dynamics-data.service';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';

let component: FormViewerComponent;
let fixture: ComponentFixture<FormViewerComponent>;
let dynamicsServiceStub: Partial<DynamicsDataService>;

describe('form-viewer component', () => {
    const activatedRouteStub = new ActivatedRouteStub({ id: '1' });
    dynamicsServiceStub = {};
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [FormViewerComponent],
            imports: [BrowserModule],
            providers: [
                { provide: DynamicsDataService, useValue: dynamicsServiceStub },
                { provide: ActivatedRoute, useValue: activatedRouteStub },
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ],
            schemas: [NO_ERRORS_SCHEMA]
        });
        fixture = TestBed.createComponent(FormViewerComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(component).toBeTruthy();
    }));
});
