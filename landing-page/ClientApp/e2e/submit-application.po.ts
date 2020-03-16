import { browser, by, element, protractor } from 'protractor';
import { AccountDataService } from '../src/app/services/account-data.service'
import { HttpClient, HttpXhrBackend, XhrFactory } from '@angular/common/http'
import { XMLHttpRequest } from 'xmlhttprequest'

export class BrowserXhr implements XhrFactory {
    constructor() {}
    build(): any { return <any>(new XMLHttpRequest()); }
}

export class SubmitApplication {
    apiPath = 'api/accounts/';
    accountDataService: AccountDataService;

    constructor() {
        const http: HttpClient = new HttpClient(new HttpXhrBackend(new BrowserXhr()));
        this.accountDataService = new AccountDataService(http);
    }

    build(): any { return <any>(new XMLHttpRequest()); }

    waitForApplicationPage() {
        var elem = element(by.xpath('/html/body/app-root/div/div/main/div/app-application'));
        var until = protractor.ExpectedConditions;
        browser.wait(until.presenceOf(elem), 5000, 'Element taking too long to appear in the DOM');
    }
    
    getHeading(header: string) {
        return element(by.css(header)).getText();
    }
}
