import { browser, by, element } from 'protractor';

export class AppSurveyPage {
    navigateTo() {
      return browser.get('/cannabislicensing/prv/');
    }

    getMainHeading() {
      return element(by.css('app-root h1')).getText();
    }
}
