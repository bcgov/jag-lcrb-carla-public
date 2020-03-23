import { browser, by, element } from 'protractor';
import { CannabisRetailLicensingPage } from './cannabis-retail-licensing.po';

const VOTE_SLUG = 'initialNumberInterested';

describe('Policy Content feature test', () => {
  let page: CannabisRetailLicensingPage;

  beforeEach(() => {
    page = new CannabisRetailLicensingPage();
  });

  it('Cannabis Retail Licence', () => {
    page.navigateTo();
    expect(page.getMainHeading()).toEqual("Cannabis Retail Licence");   
  });
});
