import { browser, by, element } from 'protractor';
import { AppHomePage } from './app.po';

describe('App Home Page', () => {
    let page: AppHomePage;

    beforeEach(() => {
        page = new AppHomePage();
    });

    it('should display a title', () => {
        page.navigateTo();
        expect(page.getMainHeading()).toEqual('Apply for a cannabis retail store licence');
    });

    it('should have appropriate content', () => {
        page.navigateTo();
        expect(page.getMainHeading()).toEqual('Apply for a cannabis retail store licence');
        expect(page.hasEmailRegistration()).toBe(true);
        expect(page.hasLinkToSurvey()).toBe(true);
        // expect(page.hasVotingWidgets()).toBe(true);
    });

    it('should link to the Survey page', () => {
        page.navigateTo();
        page.linkToSurvey().click();
        expect(browser.getCurrentUrl()).toContain('/prv/survey');

        /* TODO this doesn't work yet
        var pageSource = function () {
            return browser.getPageSource();
        };
        expect(pageSource).toContain("Find out what you need to apply for a licence");
        expect(pageSource).toContain("Are you 19 years old or over?");
        */
    });

    // it('should accept email registration', () => {
    //     page.navigateTo();
    //     page.inputEmail().sendKeys('some.random' + Math.random() + '@email.com');
    //     page.inputEmailButton().click();

    //     // wait for toast to appear
    //     browser.wait(function () {
    //         return element(by.css('#toast-container .toast-message')).isPresent();
    //     }, 10000);
    //     expect(element(by.css('#toast-container .toast-message')).isPresent()).toBe(true);

    //     // wait for toast to disappear
    //     browser.wait(function () {
    //         return element(by.css('#toast-container .toast-message')).isPresent().then(function (toastPresent) {
    //             return !toastPresent;
    //         });
    //     }, 10000);
    //     expect(element(by.css('#toast-container .toast-message')).isPresent()).toBe(false);

    //     // TODO check the database to ensure the email was saved
    // });
});
