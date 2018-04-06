import { browser, by, element } from 'protractor';
import { AppHomePage } from './app.po';

describe('App Home Page', () => {
    let page: AppHomePage;

    beforeEach(() => {
        page = new AppHomePage();
    });

    it('should display a title', () => {
        page.navigateTo();
        browser.waitForAngular();
        expect(page.getMainHeading()).toEqual('Find out what you need to apply for a licence');
    });

    it('should have appropriate content', () => {
        page.navigateTo();
        browser.waitForAngular();
        expect(page.getMainHeading()).toEqual('Find out what you need to apply for a licence');
        expect(page.hasEmailRegistration()).toBe(true);
        expect(page.hasLinkToSurvey()).toBe(true);
        expect(page.hasVotingWidgets()).toBe(true);
    });

    it('should link to the Survey page', () => {
        page.navigateTo();
        page.linkToSurvey().click();
        browser.waitForAngular();
        expect(browser.getCurrentUrl()).toContain("/prv/survey");
        /* TODO this doesn't work yet
        var pageSource = function () {
            return browser.getPageSource();
        };
        expect(pageSource).toContain("Find out what you need to apply for a licence");
        expect(pageSource).toContain("Are you 19 years old or over?");
        */
    })

    it('should accept email registration', () => {
        page.navigateTo();
        page.inputEmail().sendKeys("some.random@email.com");
        browser.waitForAngular();
        page.inputEmailButton().click();
        browser.waitForAngular();
        browser.sleep(2000);
        browser.wait(function () {
            return element(by.css('#toast-container .toast-message')).isPresent().then(function (toastPresent) {
                return !toastPresent;
            });
        }, 10000);
    })
});
