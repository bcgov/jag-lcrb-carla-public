package pages.app

import geb.Page
import geb.module.TextInput

import pages.app.SurveyPage

class HomePage extends Page {

    static at = { title=="LCLB" }
    static url = "/cannabislicensing/"

    static content = {
    	startSurvey(to: SurveyPage) { $("a", id: "surveyStartButton") }
    	emailSignupForm { $("form", id: "emailSignupForm") }
    	inputEmail(wait: true) { $("input", id: "inputEmail") } // name: "newsletterSignupEmail") } 
    	inputEmailButton { $("a", id: "inputEmailButton") }
    	showVoteResultsButton { $("a", id: "showVoteResultsButton") }
    	hideVoteResultsButton { $("a", id: "hideVoteResultsButton") }
    	voteResultsTable { $("table", id: "voteResultsTable") }
    }
}
