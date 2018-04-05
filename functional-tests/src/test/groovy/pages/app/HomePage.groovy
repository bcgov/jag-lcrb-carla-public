package pages.app

import geb.Page
import pages.app.SurveyPage

class HomePage extends Page {

    static at = { title=="LCLB" }
    static url = "/"

    static content = {
    	startSurvey(to: SurveyPage) { $("a", 0, id: "start_button") }
    }
}
