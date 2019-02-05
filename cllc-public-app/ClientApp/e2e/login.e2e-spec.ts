import { LoginPage } from './login.po';
import { browser, by, element, protractor } from 'protractor';

describe('Login component test', () => {
    let page: LoginPage;

    beforeEach(async () => {
        jasmine.DEFAULT_TIMEOUT_INTERVAL = 100000;
        page = new LoginPage();
        await browser.get('/logout');
    });

    afterEach(async () => {
        await page.logoutAndDelete();
        await browser.get('/logout');
    });

    it('should finish login process and redirect to dashboard', async () => {
        await page.navigateToBusinessLogin('protractorBusinessTestUser');
        expect(page.getHeading('h1')).toEqual('Terms of Use');
        page.getCheckbox().click();
        page.getButton().click();

        expect(page.getHeading('h2')).toEqual('Please confirm the business or organization name associated to the Business BCeID.');


        element(by.css('.btn-primary.confirmYes')).click();
        expect(page.getHeading('h2')).toEqual('Please confirm the organization type associated with the Business BCeID:');
        page.getPrivateCorpRadio().click();
        page.getButtonByClass('btn-primary').click();
        page.getButtonByClass('btn-primary').click();

        const until = protractor.ExpectedConditions;
        const elem = element(by.css('h1'));
        browser.wait(until.presenceOf(elem), 5000, 'Could not reach the business profile');
        expect(elem.getText()).toEqual('Business Profile');
    });


    it('should login and populate the business profile', async () => {
        await page.navigateToBusinessLogin('protractorBusinessProfileTestUser');
        expect(page.getHeading('h1')).toEqual('Terms of Use');
        element(by.css('.terms-cb')).click();
        element(by.css('.btn.btn-primary.termsAccept')).click();

        expect(page.getHeading('h2')).toEqual('Please confirm the business or organization name associated to the Business BCeID.');


        element(by.css('.btn-primary.confirmYes')).click();
        expect(page.getHeading('h2')).toEqual('Please confirm the organization type associated with the Business BCeID:');
        page.getPrivateCorpRadio().click();
        page.getButtonByClass('btn-primary').click();
        page.getButtonByClass('btn-primary').click();

        const until = protractor.ExpectedConditions;
        const elem = element(by.css('h1'));
        browser.wait(until.presenceOf(elem), 5000, 'Could not reach the business profile');
        expect(elem.getText()).toEqual('Business Profile');

        // populate business information data
        element(by.css('[formcontrolname="businessNumber"]')).sendKeys('123412390');
        element(by.css('[formcontrolname="bcIncorporationNumber"]')).sendKeys('INC 11111');
        element(by.css('[formcontrolname="dateOfIncorporationInBC"]')).click();

        // browser.wait(until.presenceOf(element(by.css('h10'))), 15000, 'Could not reach the business profile');
        element(by.css('.mat-calendar-body-cell-content.mat-calendar-body-today')).click();

        // populate business addresses

        element(by.css('[formcontrolname="mailingAddressStreet"]')).sendKeys('m645 Tyee rd');
        element(by.css('[formcontrolname="mailingAddressStreet2"]')).sendKeys('smuite 204');
        element(by.css('[formcontrolname="mailingAddressCity"]')).sendKeys('mVictoria');
        element(by.css('[formcontrolname="mailingAddressPostalCode"]')).sendKeys('M8F 6T0');
        element(by.css('[formcontrolname="mailingAddressProvince"]')).sendKeys('BC');
        element(by.css('[formcontrolname="mailingAddressCountry"]')).sendKeys('Canada');

        const elm = element(by.css('[formcontrolname="_mailingSameAsPhysicalAddress"]'));
        browser.actions().mouseMove(elem).click().perform();

        element(by.css('[formcontrolname="physicalAddressStreet"]')).sendKeys('645 Tyee rd');
        element(by.css('[formcontrolname="physicalAddressStreet2"]')).sendKeys('suite 204');
        element(by.css('[formcontrolname="physicalAddressCity"]')).sendKeys('Victoria');
        element(by.css('[formcontrolname="physicalAddressPostalCode"]')).sendKeys('V8F 6T0');

        element(by.css('[formcontrolname="contactPhone"]')).sendKeys('7787787787');
        element(by.css('[formcontrolname="contactEmail"]')).sendKeys('em@il');

        // populate budiness contact
        element(by.css('[formcontrolname="jobTitle"]')).sendKeys('Founder');
        element(by.css('[formcontrolname="telephone1"]')).sendKeys('2502502502');
        element(by.css('[formcontrolname="emailaddress1"]')).sendKeys('Founder@m');
        browser.actions().mouseMove(element(by.css('[formcontrolname="emailaddress1"]'))).perform();


        element(by.css('[formcontrolname="corpConnectionFederalProducer"][ng-reflect-value="1"]')).click();

        element(by.css('[formcontrolname="corpConnectionFederalProducerDetails"]'))
            .sendKeys('corpConnectionFederalProducerDetai\nasdfsadfsdffsdf\nasdfasdfasdf');

        browser.actions().mouseMove(element(by.css('[formcontrolname="federalProducerConnectionToCorp"][ng-reflect-value="1"]'))).perform();
        element(by.css('[formcontrolname="federalProducerConnectionToCorp"][ng-reflect-value="1"]')).click();

        element(by.css('[formcontrolname="federalProducerConnectionToCorpDetails"]'))
            .sendKeys('corpConnectionFederalProducerDetai\nasdfsadfsdffsdf\nasdfasdfasdf');


        element(by.css('.btn.btn-primary.save-cont')).click();
        page.waitForDashboard();

    });


    // Uncomment this when BC Service dashboard is ready.

    // it('should finish login (worker) process and redirect to worker qualification page', async () => {
    //     await page.navigateToBCServiceLogin("protractorServiceTestUser");
    //     expect(page.getHeading('h2')).toEqual('Please confirm the name associated with the BC Service card login provided.');
    //     page.getButtonByClass("btn-primary").click();
    //     page.waitForDashboard();
    //     expect(page.getHeading('h1')).toEqual('BC Service Card Dashboard');
    // });
});
