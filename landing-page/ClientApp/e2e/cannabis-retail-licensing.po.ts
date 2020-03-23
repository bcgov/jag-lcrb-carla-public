import { browser, by, element } from 'protractor';

export class CannabisRetailLicensingPage {
    navigateTo() {
      return browser.get('/LCRB/policy-document/cannabis-retail-licence');
    }

    getMainHeading() {
      return element(by.css('app-root h1')).getText();
    }   

}
