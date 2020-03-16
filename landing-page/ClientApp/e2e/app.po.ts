import { browser, by, element } from 'protractor';

export class AppHomePage {
    navigateTo() {
      return browser.get('/');
    }

    getMainHeading() {
      return element(by.css('app-home .jumbotron h2')).getText();
    }

    inputEmail() {
        return element(by.css('app-newsletter-signup form input'));
    }
    inputEmailButton() {
        return element(by.css('app-newsletter-signup form button'));
    }

    inputVoteButton(slug, number) {
      return element(by.id('vote_' + slug + '_' + number));
    }

    hasEmailRegistration() {
        return this.inputEmail().isPresent() && this.inputEmailButton().isPresent();
    }

    linkToSurvey() {
        return element(by.css('app-home .side-box button[routerLink="prv"]'));
    }

    hasLinkToSurvey() {
        return this.linkToSurvey().isPresent();
    }

    hasVotingWidgets() {
        return element(by.id('showVoteResultsButton')).isPresent() || element(by.id('hideVoteResultsButton')).isPresent();
    }

    isVotingButtonsPresent(slug) {
      return element(by.id('voteOptionButtons_' + slug)).isPresent();
    }

    // accordion functions
    getAccordionHeading() {
        return element(by.id('accordion_heading')).getText();
    }

    expandAccordion() {
        element(by.css('.e2e-accordion-show-all')).click();
    }

    collapseAccordion() {
        element(by.css('.e2e-accordion-hide-all')).click();
    }

    getFirstAccodionElement() {
        return element(by.css('.e2e-accordion-first-child'));
    }

}

/*
var AppHomePage = function() {
    var inputEmail = element(by.id("inputEmail"));
    var inputEmailButton = element(by.id("inputEmailButton"));
    var surveyStartButton = element(by.id("surveyStartButton"));
    var showVoteResultsButton = element(by.id("showVoteResultsButton"));
    var hideVoteResultsButton = element(by.id("hideVoteResultsButton"));

    this.navigateTo = function() {
      return browser.get('/cannabislicensing/');
    };

    this.getMainHeading = function() {
      return element(by.css('app-root h1')).getText();
    };

    this.hasEmailRegistration = function() {
        return element(by.id("inputEmail")).isPresent() && element(by.id("inputEmailButton")).isPresent();
    };

    this.hasLinkToSurvey = function() {
        return element(by.id("surveyStartButton")).isPresent();
    };

    this.hasVotingWidgets = function() {
        return element(by.id("showVoteResultsButton")).isPresent() || element(by.id("hideVoteResultsButton")).isPresent();
    };
};
module.exports = new AppHomePage();
*/
