import { browser, by, element } from 'protractor';

export class CannabisRetailLicensingPage {
    navigateTo() {
      return browser.get('/cannabislicensing/policy-document/cannabis-retail-licence');
    }

    getMainHeading() {
      return element(by.css('app-root h1')).getText();
    }   

}
