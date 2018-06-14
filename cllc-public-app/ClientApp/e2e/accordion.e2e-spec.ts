import { browser, by, element } from 'protractor';
import { AppHomePage } from './app.po';

describe('Accordion component test', () => {
    let page: AppHomePage;

  beforeEach(async() => {
      page = new AppHomePage();
      await page.navigateTo();
  });

  it('should display a title', async () => {
    let heading = await page.getAccordionHeading();
    expect(heading).toEqual('A cannabis retail store licence allows licensees to sell non-medical cannabis and cannabis accessories in British Columbia.');
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

});
