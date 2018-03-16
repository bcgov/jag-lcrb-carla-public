import geb.spock.GebReportingSpec

import pages.app.HomePage


import spock.lang.Unroll
import org.openqa.selenium.*

class FlowSpecs extends GebReportingSpec {

	@Unroll

	def "Smoke Test"(){
	    given: "I am a public user"			
		when: "I go to the home page"
			to HomePage
        then: "I am on the home page"
			at HomePage	
	}


}
