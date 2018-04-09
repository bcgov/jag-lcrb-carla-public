import { browser, by, element } from 'protractor';

export class AppSurveyPage {
    navigateTo() {
        return browser.get('/cannabislicensing/prv/');
    }

    getMainHeading() {
        return element(by.css('app-root h3')).getText();
    }

    surveyPageTitle() {
        return element(by.className('sv_q_title')).getText();
    }

    getSurveyQuestion(surveyConfig, q) {
        var len = surveyConfig.pages.length;
        for (var i = 0; i < len; i++) {
            var p = surveyConfig.pages[i];
            if (p.name == q) {
                return p;
            }
        }
        expect("Error no question found for ").toEqual(q);
    }

    surveyPageButton(selectedButton) {
        return element(by.className("btn btn-primary sv_" + selectedButton + "_btn"));
    }

    surveyPageInputField(currentPage, i) {
        var fieldName = currentPage.elements[0].name + "_sq_" + (100+i);
        return element(by.name(fieldName));
    }

    surveySetQValue(currentPage, currentElement, toValue) {
    }

    executeSurvey(navPath, surveyConfig) {
        // execute each step of the survey navigation
        var len = navPath.length;
        for (var i = 0; i < len; i++) {
            var myNav = navPath[i];
            var myQ = myNav['q'];
            var myR = myNav['r'];
            var myButton = myNav['button'];

            var currentSurveyP = this.getSurveyQuestion(surveyConfig, myQ);

            // 1. confirm page title
            var currentPageTitle = this.surveyPageTitle();
            expect(currentPageTitle).toEqual("* " + currentSurveyP.elements[0].title);

            // 2. set question response (value == myR)
            var currentQInput = this.surveyPageInputField(currentSurveyP, i);
            this.surveySetQValue(currentSurveyP, currentQInput, myR);

            // 3. click on selected button (class "btn btn-primary sv_<<myButton>>_btn")
            this.surveyPageButton(myButton).click();
            browser.waitForAngular();
        }

        // TODO return the final result page

    }
}
