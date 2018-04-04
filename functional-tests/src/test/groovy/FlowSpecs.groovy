import geb.spock.GebReportingSpec

import pages.app.HomePage
import pages.app.SurveyPage

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
	}

	def "Smoke Test Survey"(){
	    given: "I am a public user"			
		when: "I go to the survey page"
			to SurveyPage
        then: "I am on the survey page"
			at SurveyPage	
	}

}
