import { LoginPage } from './login.po';
import { SubmitApplication } from './submit-application.po'

describe('Submit application component test', () => {
    let loginPage: LoginPage;
    let submitApplicationPage: SubmitApplication;

    beforeEach(async () => {
        loginPage = new LoginPage();
        submitApplicationPage = new SubmitApplication();
    });

    afterEach(async () => {
        await loginPage.logoutAndDelete();
    });

    it('should upload file', async () => {
        await loginPage.navigateToBusinessLogin("protractorBusinessTestUser");
        loginPage.getCheckbox().click();
        loginPage.getButton().click();
        loginPage.getButtonByClass("btn-primary").click();
        loginPage.getPrivateCorpRadio().click();
        loginPage.getButtonByClass("btn-primary").click();
        loginPage.waitForDashboard();
        loginPage.getButtonByClass("btn-primary").click();
        
        submitApplicationPage.waitForApplicationPage();
        expect(submitApplicationPage.getHeading('h1')).toEqual('Submit the Cannabis Retail Store Licence Application');

        /*
        Ticket LCSD-107
        Test plan/results of manual test:
        - upload jpg, png, word, xls, pdf files one at a time - pass
        - simultaneously upload multiple jpgs- fail
        - simultaneously upload jpg with other file types - fail
        - simultaneously upload word, xls, pdf - pass
        - simultaneously upload word, xls, pdf, and png - pass
        - simultaneously upload combo of files that exceeds 25mb - pass (so long as one is not a jpg)
        */
    });
});
