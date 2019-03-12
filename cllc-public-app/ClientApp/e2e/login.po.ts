import { browser, by, element, protractor } from 'protractor';
import { AccountDataService } from '../src/app/services/account-data.service';
import { HttpClient, HttpXhrBackend, XhrFactory } from '@angular/common/http';
import { XMLHttpRequest } from 'xmlhttprequest';
import { Subject } from 'rxjs';

export class BrowserXhr implements XhrFactory {
    constructor() { }
    build(): any { return <any>(new XMLHttpRequest()); }
}

export class LoginPage {
    apiPath = 'api/account/';
    accountDataService: AccountDataService;

    constructor() {
        const http: HttpClient = new HttpClient(new HttpXhrBackend(new BrowserXhr()));
        this.accountDataService = new AccountDataService(http);
    }

    build(): any { return <any>(new XMLHttpRequest()); }

    navigateToBusinessLogin(user) {
        return browser.get('/login/token/' + user);
    }

    navigateToBCServiceLogin(user) {
        return browser.get('/bcservice/token/' + user);
    }

    logoutAndDelete() {
        return this.accountDataService.deleteCurrentAccount();
    }

    getHeading(header: string) {
        return element(by.css(header)).getText();
    }

    getCheckbox() {
        return element(by.css('input'));
    }

    getButton() {
        return element(by.css('button.termsAccept'));
    }

    getButtonByClass(css_class: string) {
        return element(by.className(css_class));
    }

    getPrivateCorpRadio() {
        return element(by.css('[ng-reflect-value="PrivateCorporation"]'));
    }

    waitForDashboard() {
        const elem = element(by.css('app-dashboard'));
        const until = protractor.ExpectedConditions;
        browser.wait(until.presenceOf(elem), 5000, 'Element taking too long to appear in the DOM1');
    }

    waitForWorkerDashboard() {
        const elem = element(by.css('app-associate-dashboard'));
        const until = protractor.ExpectedConditions;
        browser.wait(until.presenceOf(elem), 5000, 'Element taking too long to appear in the DOM');
    }
}
