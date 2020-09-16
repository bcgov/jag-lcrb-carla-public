using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"the expected validation errors are thrown for a(.*)")]
        public void ValidationErrorMessages(string applicationType)
        {
            if (applicationType == " licensee representative")
            {
                // check  missing representative name error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Representative Name is a required field')]")).Displayed);

                // check missing telephone error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Telephone is a required field')]")).Displayed);

                // check missing email error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'E-mail Address is a required field')]")).Displayed);

                // check missing signature agreement error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);

                // check missing scope of authority error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one scope of authority must be selected')]")).Displayed);

                // check missing declaration checkbox error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Declaration Checkbox')]")).Displayed);
            }
            else if (applicationType == "n event authorization")
            {
                // check maximum attendance error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Please enter the maximum attendance (must be a number) ')]")).Displayed);

                // check maximum staff attendance error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Please enter the maximum staff attendance (must be a number) ')]")).Displayed);
            }
            else if (applicationType == "n account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Business Number')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Corporation Address Business Phone')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'businessProfile.contactEmail is not valid')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Physical Address Street')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Physical Address City')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Physical Address Postal Code')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Mailing Address Street')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Mailing Address City')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Mailing Address Postal Code')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Mailing Address Province')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Mailing Address Country')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Corporation Contact Telephone')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Corporation Contact Email')]")).Displayed);
            }
            else if (applicationType == " private corporation org structure")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'needs to have one or more key personnel ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'needs to have one or more shareholders ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please upload the Corporation Notice of Articles ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please upload the Central Securities Register ')]")).Displayed);
            }
            else if (applicationType == " partnership org structure")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'needs to have one or more shareholders ')]")).Displayed);
            }
            else if (applicationType == " public corporation org structure")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'needs to have one or more key personnel ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please upload the Corporation Notice of Articles ')]")).Displayed);
            }
            else if (applicationType == " society org structure")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'needs to have one or more  directors & officers ')]")).Displayed);
            }
            else if (applicationType == " sole proprietorship org structure")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'needs to have a leader ')]")).Displayed);
            }
            else
            {
                // check missing authorized to submit error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);

                // check missing signature agreement error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);

                if ((applicationType == " Manufacturing application") || (applicationType == " Cannabis application") || (applicationType == " Catering application") || (applicationType == " Rural Store application") || (applicationType == "n indigenous nation Cannabis application"))
                {
                    // check missing street address error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the street address')]")).Displayed);

                    // check missing city error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the city')]")).Displayed);

                    // check missing postal code error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the postal code')]")).Displayed);

                    // check missing PID error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the Parcel Identifier (format: 9 digits)')]")).Displayed);

                    // check missing business contact error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the business contact')]")).Displayed);

                    // check missing business contact phone number error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s 10-digit phone number')]")).Displayed);

                    // check missing business contact email error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s email address')]")).Displayed);

                    // check missing establishment name error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Establishment Name is required')]")).Displayed);
                }

                if ((applicationType == " Manufacturing application") || (applicationType == " Cannabis application") || (applicationType == "n indigenous nation Cannabis application"))
                {
                    // check missing police jurisdiction error is thrown
                    // under review - see LCSD-3846
                    // Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'policeJurisdiction is not valid')]")).Displayed);

                    // check missing indigenous nation error is thrown
                    // under review - see LCSD-3846
                    // Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'indigenousNation is not valid')]")).Displayed);
                }

                if ((applicationType == " Manufacturing application") || (applicationType == " Cannabis application") || (applicationType == " Catering application") || (applicationType == " location change application") || (applicationType == "n indigenous nation Cannabis application"))
                {
                    // check missing signage document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                }

                if ((applicationType == " Manufacturing application") || (applicationType == " Cannabis application") || (applicationType == " facility structural change application") || (applicationType == " location change application") || (applicationType == "n indigenous nation Cannabis application") || (applicationType == "n on-site store endorsement"))
                {
                    // check missing site plan document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);

                    // check missing floor plan document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                }

                if ((applicationType == " Cannabis application") || (applicationType == "n indigenous nation Cannabis application"))
                {
                    // check missing product not visible from outside error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please confirm that product will not be visible from the outside')]")).Displayed);

                    // check missing zoning document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one zoning document is required.')]")).Displayed);

                    // check missing Financial Integrity document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Financial Integrity form is required.')]")).Displayed);

                    // check missing supporting document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one supporting document is required.')]")).Displayed);
                }

                if (applicationType == " transfer of ownership")
                {
                    // check missing value error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please select a value')]")).Displayed);

                    // check missing transfer consent error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please consent to the transfer')]")).Displayed);
                }

                if ((applicationType == " Catering transfer of ownership") || (applicationType == " CRS transfer of ownership"))
                {
                    // check missing proposed transferee error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please select the proposed transferee')]")).Displayed);

                    // check missing transfer consent error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please consent to the transfer')]")).Displayed);
                }

                if (applicationType == " CRS Branding Change application")
                {
                    // check missing proposed change error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'proposedChange is not valid')]")).Displayed);

                    // check missing signage document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                }

                if ((applicationType == " Branding Change application") || (applicationType == " location change application"))
                {
                    // check missing signage document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                }

                if ((applicationType == " store relocation application") || (applicationType == " Catering store relocation application"))
                {
                    // check missing street address error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the street address')]")).Displayed);

                    // check missing city error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the city')]")).Displayed);

                    // check missing postal code error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the postal code')]")).Displayed);
                }

                if (applicationType == " structural change application")
                {
                    // check missing product not visible from outside error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please confirm that product will not be visible from the outside')]")).Displayed);
                }

                if (applicationType == " structural change application")
                {
                    // check missing description error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter a description')]")).Displayed);

                    // check missing floor plan document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                }

                if (applicationType == " Catering third party application")
                {
                    // check missing value error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please select a value')]")).Displayed);
                }

                if (applicationType == " location change application")
                {
                    // check missing description error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter a description')]")).Displayed);

                    // check missing supporting document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one supporting document is required.')]")).Displayed);
                }

                if (applicationType == "n indigenous nation Cannabis application")
                {
                    // check missing IN error is shown
                    // waiting for bug fix: LCSD-3671
                }

                if (applicationType == " licence renewal application")
                {
                    // check for missing error messages
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 1')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 2')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 3')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 4')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 5')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 6')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 7')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 8')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 9')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 10')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 11')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 12')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 13')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 14')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 15')]")).Displayed);
                }

                if (applicationType == " structural alterations request")
                {
                    // check that missing capacity error is thrown
                    // under review - see LCSD-3803
                    // Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'capacityArea.capacity is not valid')]")).Displayed);

                    // check missing floor plan document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                }

                if ((applicationType == " special event area endorsement") || (applicationType == " lounge area endorsement"))
                {
                    // check that service hours error is thrown
                    // under review - see LCSD-3849
                    // Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'serviceHoursWednesdayOpen is not valid')]")).Displayed);

                    // check missing site plan document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);

                    // check missing floor plan document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);

                    // check that missing hours of sale error is thrown
                    // under review - see LCSD-3849
                    // Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Hours of sale are required')]")).Displayed);
                }

                if (applicationType == " third party operator")
                {
                    // check that the third party operator error is shown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Please select the business name to be a third party operator of  your licence ')]")).Displayed);
                }

                if (applicationType == " picnic area endorsement")
                {
                    // check that missing capacity error is thrown
                    // under review - see LCSD-3803
                    // Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'capacityArea.capacity is not valid')]")).Displayed);

                    // check missing site plan document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);
                }
            }
        }


        [And(@"the correct terms and conditions are displayed for (.*)")]
        public void CorrectTermsAndConditionsDisplayed(string licenceType)
        {
            if (licenceType == "Catering")
            {
                // check that the correct text is displayed for Catering
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'For sale and service of liquor at another person')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s event where food service is catered by the licensee, unless otherwise permitted.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'The terms and conditions to which this licence is subject include the terms and conditions contained in the licensee Terms and Conditions Handbook, which is available on the Liquor and Cannabis Regulation Branch website. The Terms and Conditions Handbook is amended from time to time.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Licensee may only serve liquor at a catered event for which LCRB has issued a catering authorization.')]")).Displayed);
            }

            if (licenceType == "CRS")
            {
                // check that the correct text is displayed for CRS
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'This licence is subject to the terms and conditions specified in the restriction or approval letter(s) and those contained in the Cannabis Retail Store Handbook, which may be amended from time to time.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Packaged cannabis may only be sold within the service area outlined in blue on the LCRB approved floor plan, unless otherwise endorsed or approved by the LCRB.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'The establishment may be open anytime between the hours of 9 a.m. and 11 p.m., subject to further restriction by the local government or Indigenous nation.')]")).Displayed);
            }
        }
    }
}
