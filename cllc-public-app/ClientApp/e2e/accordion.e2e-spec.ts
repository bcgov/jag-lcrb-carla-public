import { browser, by, element } from 'protractor';
import { AppHomePage } from './app.po';

describe('Accordion component test', () => {
    let page: AppHomePage;

  beforeEach(() => {
      page = new AppHomePage();
  });

  //it('displays the accordion component in home page', () => {
  //  page.navigateTo();
  //  browser.waitForAngular();
  //  var accordionHeading = page.getAccordionHeading();
  //  //console.log('***********************************************************************');
  //  //console.log(accordionHeading);
  //  expect(accordionHeading).toEqual('Selling Cannabis Step-by-Step XXXX');
  //  });

  it('should display a title', async () => {
    await page.navigateTo();
    let heading = await page.getAccordionHeading();
    expect(heading).toEqual('Selling Cannabis Step-by-Step');
  });


});
