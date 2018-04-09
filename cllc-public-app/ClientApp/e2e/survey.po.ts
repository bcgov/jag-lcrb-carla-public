import { browser, by, element } from 'protractor';

export class AppSurveyPage {
    navigateTo() {
        return browser.get('/cannabislicensing/prv/');
    }

    getMainHeading() {
        return element(by.css('app-root h3')).getText();
    }

    surveyPageTitle() {
        return element(by.class("sv_q_title"));
    }

    surveyPageButton(selectedButton) {
        return element(by.class("btn btn-primary sv_" + selectedButton + "_btn"));
    }

    getSurveyQuestion(surveyConfig, q) {
        for (var p in surveyConfig["pages"]) {
            if (p["name"] == q) {
                return p;
            }
        }
        expect("Error no page found for").isEqual(q);
    }

    executeSurvey(navPath, surveyConfig) {
        // execute each step of the survey navigation
        var len = navPath.length;
        for (var i = 0; i < len; i++) {
            var myNav = navPath[i];
            var myQ = myNav['q'];
            var myR = myNav['r'];
            var myButton = myNav['button'];

            var currentSurveyp = getSurveyQuestion(surveyConfig, myQ);

            // 1. confirm page title
            var currentPageTitle = surveyPageTitle();
            expect(currentPageTitle).toEqual(currentSurveyQ['elements'][0]['title']);

            // 2. set question response (value == myR)

            // 3. click on selected button (class "btn btn-primary sv_<<myButton>>_btn")
            surveyPageButton(myButton).click();
            browser.waitForAngular();
        }

        // TODO return the final result page

    }
}
