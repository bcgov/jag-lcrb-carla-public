
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from '@angular/platform-browser';
import { BceidConfirmationComponent } from './bceid-confirmation.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { DynamicsDataService } from '@services/dynamics-data.service';
import { UserDataService } from '@services/user-data.service';
import { provideMockStore } from '@ngrx/store/testing';
import { AccountDataService } from '@services/account-data.service';

let component: BceidConfirmationComponent;
let fixture: ComponentFixture<BceidConfirmationComponent>;
let dynamicsDataServiceStub: Partial<DynamicsDataService>;
let userDataServiceStub: Partial<UserDataService>;
let accountDataServiceStub: Partial<AccountDataService>;

describe('bceid-confirmation component', () => {
    dynamicsDataServiceStub = {};
    userDataServiceStub = {};
    accountDataServiceStub = {};
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [BceidConfirmationComponent],
            imports: [BrowserModule],
            providers: [
                provideMockStore({}),
                { provide: AccountDataService, useValue: accountDataServiceStub },
                { provide: UserDataService, useValue: userDataServiceStub },
                { provide: DynamicsDataService, useValue: dynamicsDataServiceStub },
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ],
            schemas: [NO_ERRORS_SCHEMA]
        });
        fixture = TestBed.createComponent(BceidConfirmationComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
