import { browser, by, element, protractor } from "protractor";
import { AccountDataService } from "../src/app/services/account-data.service"
import { XhrFactory } from '@angular/common';
import { HttpClient, HttpXhrBackend } from "@angular/common/http"
import { XMLHttpRequest } from "xmlhttprequest"

export class BrowserXhr implements XhrFactory {
  constructor() {}

  build(): any { return (new XMLHttpRequest()) as any; }
}

export class SubmitApplication {
  apiPath = "api/accounts/";
  accountDataService: AccountDataService;

  constructor() {
    const http = new HttpClient(new HttpXhrBackend(new BrowserXhr()));
    this.accountDataService = new AccountDataService(http);
  }

  build(): any { return (new XMLHttpRequest()) as any; }

  waitForApplicationPage() {
    const elem = element(by.xpath("/html/body/app-root/div/div/main/div/app-application"));
    const until = protractor.ExpectedConditions;
    browser.wait(until.presenceOf(elem), 5000, "Element taking too long to appear in the DOM");
  }

  getHeading(header: string) {
    return element(by.css(header)).getText();
  }
}
