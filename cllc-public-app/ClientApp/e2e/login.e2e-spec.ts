import { LoginPage } from './login.po';

describe('Login component test', () => {
    let page: LoginPage;

    beforeEach(async () => {
        page = new LoginPage();
    });

    afterEach(async () => {
        await page.logoutAndDelete();
    });

    it('should finish login process and redirect to dashboard', async () => {
        await page.navigateToBusinessLogin("protractorBusinessTestUser");
        expect(page.getHeading('h1')).toEqual('Terms of Use');
        page.getCheckbox().click();
        page.getButton().click();
        expect(page.getHeading('h2')).toEqual('Please confirm the business or organization name associated to the Business BCeID.');
        page.getButtonByClass("btn-primary").click();
        expect(page.getHeading('h2')).toEqual('Please confirm the organization type associated with the Business BCeID:');
        page.getPrivateCorpRadio().click();
        expect(page.getHeading('h2')).toEqual('Please confirm the name associated with the Business BCeID login provided.');
        page.getButtonByClass("btn-primary").click();

        page.waitForDashboard();
        expect(page.getHeading('h1')).toEqual('Welcome to Cannabis Retail Store Licensing');
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
