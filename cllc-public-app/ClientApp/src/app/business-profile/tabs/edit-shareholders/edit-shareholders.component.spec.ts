/// <reference path="../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from '@angular/platform-browser';
import { EditShareholdersComponent } from './edit-shareholders.component';

let component: EditShareholdersComponent;
let fixture: ComponentFixture<EditShareholdersComponent>;

describe('EditShareholders component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ EditShareholdersComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(EditShareholdersComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});