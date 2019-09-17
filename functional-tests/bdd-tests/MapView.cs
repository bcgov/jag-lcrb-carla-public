using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Protractor;
using System;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    [FeatureFile("./map-view.feature")]
    public sealed class MapView : TestBase
    {

        [Given(@"I view the map")]
        public void I_view_the_map()
        {
            // navigate to the map page.
            
        }

        [And(@"I zoom in")]
        public void I_zoom_in()
        {
            // zoom in
        }


        [Then(@"the page shows search results in text form for the given area")]
        public void The_result_should_be_z_on_the_screen(int expectedResult)
        {
            // verify that the results are in the right area
        }
    }
}
