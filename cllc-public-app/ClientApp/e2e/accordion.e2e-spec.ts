import { browser, by, element } from 'protractor';
import { AppHomePage } from './app.po';

describe('Accordion component test', () => {
    let page: AppHomePage;

  beforeAll(async() => {
      browser.waitForAngularEnabled(false);
      page = new AppHomePage();
      await page.navigateTo();
  });


  it('should show all steps', async () => {
    page.expandAccordion();
    let element = await page.getFirstAccodionElement();
    expect(element.getCssValue('visibility')).toEqual('visible');
  });

  it('should hide all steps', async () => {
    page.collapseAccordion();
    let element = await page.getFirstAccodionElement();
    expect(element.getCssValue('visibility')).toEqual('hidden');
  });

  // it('should display a title', async () => {
  //   let heading = await page.getAccordionHeading();
  //   expect(heading).toEqual('Individuals (sole proprietors), partnerships, corporations and Indigenous nations may apply for a cannabis retail store licence.');
  // });

});
