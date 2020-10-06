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
            switch (applicationType)
            {
                case " licensee representative":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Representative Name is a required field')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Telephone is a required field')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'E-mail Address is a required field')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one scope of authority must be selected')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Declaration Checkbox')]")).Displayed);
                    break;
                case "n event authorization":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Representative Name is a required field')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Telephone is a required field')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'E-mail Address is a required field')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one scope of authority must be selected')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Declaration Checkbox')]")).Displayed);
                    break;
                case "n account profile":
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
                    break;
                case " private corporation org structure":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'needs to have one or more key personnel ')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'needs to have one or more shareholders ')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please upload the Corporation Notice of Articles ')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please upload the Central Securities Register ')]")).Displayed); break;
                case " partnership org structure":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'needs to have one or more shareholders ')]")).Displayed);
                    break;
                case " public corporation org structure":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'needs to have one or more key personnel ')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please upload the Corporation Notice of Articles ')]")).Displayed);
                    break;
                case " society org structure":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'needs to have one or more  directors & officers ')]")).Displayed);
                    break;
                case " sole proprietorship org structure":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'needs to have a leader ')]")).Displayed);
                    break;
                case " market event":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please agree to all terms')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the contact name')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the contact phone number')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the contact email address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the market type')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the market name')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the market frequency')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the market business legal name')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the city')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the postal code')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the start date')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the end date')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter either the 'Market Business Number' or the 'Incorporation/Registration Number')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'No market dates selected')]")).Displayed);
                    break;
                case " Manufacturing application":
                    // under review - see LCSD-3846
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Police Jurisdiction is not valid')]")).Displayed);
                    // under review - see LCSD-3846
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Indigenous Nation is not valid')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the street address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the city')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the postal code')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the Parcel Identifier (format: 9 digits)')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the business contact')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s 10-digit phone number')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s email address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Establishment Name is required')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case " Cannabis application":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please confirm that product will not be visible from the outside')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one zoning document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Financial Integrity form is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one supporting document is required.')]")).Displayed);
                    // under review - see LCSD-3846
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Police Jurisdiction is not valid')]")).Displayed);
                    // under review - see LCSD-3846
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Indigenous Nation is not valid')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the street address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the city')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the postal code')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the Parcel Identifier (format: 9 digits)')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the business contact')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s 10-digit phone number')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s email address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Establishment Name is required')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case " Catering application":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the street address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the city')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the postal code')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the Parcel Identifier (format: 9 digits)')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the business contact')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s 10-digit phone number')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s email address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Establishment Name is required')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case " Rural Store application":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the street address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the city')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the postal code')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the Parcel Identifier (format: 9 digits)')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the business contact')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s 10-digit phone number')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s email address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Establishment Name is required')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case "n indigenous nation Cannabis application":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please confirm that product will not be visible from the outside')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one zoning document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Financial Integrity form is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one supporting document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                    // under review - see LCSD-3846
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Police Jurisdiction is not valid')]")).Displayed);
                    // under review - see LCSD-3846
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Indigenous Nation is not valid')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the street address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the city')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the postal code')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the Parcel Identifier (format: 9 digits)')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the business contact')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s 10-digit phone number')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s email address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Establishment Name is required')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case " location change application":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter a description')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one supporting document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case " facility structural change application":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case "n on-site store endorsement":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case " transfer of ownership":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please select a value')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please consent to the transfer')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case " Catering transfer of ownership":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please select the proposed transferee')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please consent to the transfer')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case " CRS transfer of ownership":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please select the proposed transferee')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please consent to the transfer')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case " CRS Branding Change application":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'proposedChange is not valid')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case " Branding Change application":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
                    break;
                case " store relocation application":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the street address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the city')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the postal code')]")).Displayed);
                    break;
                case " Catering store relocation application":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the street address')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the city')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the postal code')]")).Displayed);
                    break;
                case " structural change application":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please confirm that product will not be visible from the outside')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter a description')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                    break;
                case " Catering third party application":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please select a value')]")).Displayed);
                    break;
                case " licence renewal application":
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
                    break;
                case " structural alterations request":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                    break;
                case " special event area endorsement":
                    // under review - see LCSD-3849
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Service Hours Wednesday Open is not valid')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                    // under review - see LCSD-3849
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Hours of sale are to be confirmed')]")).Displayed);
                    break;
                case " lounge area endorsement":
                    // under review - see LCSD-3849
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Service Hours Wednesday Open is not valid')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                    // under review - see LCSD-3849
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Hours of sale are to be confirmed')]")).Displayed);
                    break;
                case " third party operator":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Please select the business name to be a third party operator of  your licence ')]")).Displayed);
                    break;
                case " picnic area endorsement":
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);
                    break;
                default:
                    throw (new Exception($"Unknown application type {applicationType}"));
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
