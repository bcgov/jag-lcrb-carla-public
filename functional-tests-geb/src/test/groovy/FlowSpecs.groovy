import geb.spock.GebReportingSpec

import pages.app.HomePage
import pages.app.SurveyPage

import utils.DataUtils

import spock.lang.Unroll
import org.openqa.selenium.*

class FlowSpecs extends GebReportingSpec {

	@Unroll

	def "Smoke Test Home"(){
	    given: "I am a public user"			
		when: "I go to the home page"
			to HomePage
        then: "I am on the home page"
			at HomePage	
			startSurvey.displayed
			inputEmail.displayed
			inputEmailButton.displayed
	}

	def "Smoke Test Survey"(){
	    given: "I am a public user"			
		when: "I go to the survey page"
			to HomePage
			startSurvey.click()
        then: "I am on the survey page"
			at SurveyPage	
	}
	
/* TODO commenting this out as setting the email field doesn't work (we have this covered by Protractor tests)
	def "Smoke Test Email Newsletter"(){
	    given: "I am a public user"			
		when: "I register for email notifications"
			to HomePage
			//waitFor { inputEmail.value(DataUtils.randomEmail()) }
			waitFor { inputEmail.singleElement().sendKeys(DataUtils.randomEmail()) }
			// emailSignupForm.newsletterSignupEmail = DataUtils.randomEmail()
			inputEmailButton.click()
        then: "I am registered for email notifications"
			at HomePage
			// TODO test for something	
	}
*/
}
