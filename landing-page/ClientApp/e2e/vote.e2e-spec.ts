import { browser, by, element } from 'protractor';
import { AppHomePage } from './app.po';

const VOTE_SLUG = 'initialNumberInterested';

describe('Vote feature test', () => {
  let page: AppHomePage;

  beforeEach(() => {
    page = new AppHomePage();
  });

  it('Initial Visit No Cookie', () => {
    page.navigateTo();
    browser.driver.manage().deleteCookie("HasVoted" + VOTE_SLUG);
    page.navigateTo();
    browser.waitForAngular();
    expect(page.isVotingButtonsPresent(VOTE_SLUG)).toEqual(true);
  });

  it('Vote Test', () => {
    page.navigateTo();
    browser.driver.manage().deleteCookie("HasVoted" + VOTE_SLUG);
    page.navigateTo();
    browser.waitForAngular();
    expect(page.isVotingButtonsPresent(VOTE_SLUG)).toEqual(true);
    // click a vote option.
    page.inputVoteButton(VOTE_SLUG, 1).click();
    expect(page.isVotingButtonsPresent(VOTE_SLUG)).toEqual(false);
    // should be false on reload too.
    page.navigateTo();
    expect(page.isVotingButtonsPresent(VOTE_SLUG)).toEqual(false);
  });

});
