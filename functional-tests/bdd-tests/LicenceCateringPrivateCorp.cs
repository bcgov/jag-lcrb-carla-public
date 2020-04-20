using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using Xunit;

/*
Feature: LicenceCateringPrivateCorp
    As a logged in business user
    I want to pay the first year catering licence fee
    And complete the available application types

Scenario: Pay First Year Catering Licence and Complete Applications
    #Given the Catering application has been approved
    #And I am logged in to the dashboard as a private corporation
    Given I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And I pay the licensing fee
    And I request an event authorization
    #And I request a valid store name or branding change
    And I request a store relocation
    And I submit a third party application
    And I request a third party operator
    And I request a transfer of ownership
    Then the requested applications are visible on the dashboard
*/

namespace bdd_tests
{
    [FeatureFile("./LicenceCateringPrivateCorp.feature")]
    public sealed class LicenceCateringPrivateCorp : TestBase
    {
        [Given(@"the Catering application has been approved")]
        public void catering_application_approved()
        {
        }

        [Given(@"I am logged in to the dashboard as a (.*)")]
        //[And(@"I am logged in to the dashboard as a (.*)")]
        public void And_I_view_the_dashboard(string businessType)
        {
            CarlaLoginNoCheck(businessType);
        }

        [And(@"I click on the Licences tab")]
        public void click_on_licences_tab()
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }

        [And(@"I pay the licensing fee")]
        public void click_pay_first_year_licensing_fee()
        {
            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            string firstYearLicenceFee = "Pay First Year Licensing Fee";

            // click on the pay first year licence fee link
            NgWebElement uiFirstYearLicenceFee = ngDriver.FindElement(By.LinkText(firstYearLicenceFee));
            uiFirstYearLicenceFee.Click();

            // pay the licencing fee
            MakePayment();

            System.Threading.Thread.Sleep(7000);

            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }

        [And(@"I request an event authorization")]
        public void request_event_authorization()
        {
            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            string requestEventAuthorization = "Request Event Authorization";

            // click on the request event authorization link
            NgWebElement uiRequestEventAuthorization = ngDriver.FindElement(By.LinkText(requestEventAuthorization));
            uiRequestEventAuthorization.Click();

            /* 
            Page Title: Catered Event Authorization Request
            */

            // create event authorization data
            string eventContactName = "AutoTestEventContactName";
            string eventContactPhone = "2500000000";

            string eventDescription = "Automated test event description added here.";
            string eventClientOrHostName = "Automated test event";
            string maximumAttendance = "100";
            string maximumStaffAttendance = "25";

            string venueNameDescription = "Automated test venue name or description";
            string venueAdditionalInfo = "Automated test additional venue information added here.";
            string physicalAddStreetAddress1 = "Automated test street address 1";
            string physicalAddStreetAddress2 = "Automated test street address 2";
            string physicalAddCity = "Automated test city";
            string physicalAddPostalCode = "V8V4Y3";

            // enter event contact name
            NgWebElement uiEventContactName = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiEventContactName.SendKeys(eventContactName);

            // enter event contact phone
            NgWebElement uiEventContactPhone = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiEventContactPhone.SendKeys(eventContactPhone);

            // select event type
            NgWebElement uiEventType = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-event-form/div/form/div[6]/div[1]/app-field/section/div/section/select/option[2]"));
            uiEventType.Click();

            // enter event description
            NgWebElement uiEventDescription = ngDriver.FindElement(By.XPath("//textarea"));
            uiEventDescription.SendKeys(eventDescription);

            // enter event client or host name
            NgWebElement uiEventClientOrHostName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiEventClientOrHostName.SendKeys(eventClientOrHostName);

            // enter maximum attendance
            NgWebElement uiMaxAttendance = ngDriver.FindElement(By.XPath("//input[@type='number']"));
            uiMaxAttendance.SendKeys(maximumAttendance);

            // enter maximum staff attendance
            NgWebElement uiMaxStaffAttendance = ngDriver.FindElement(By.XPath("(//input[@type='number'])[2]"));
            uiMaxStaffAttendance.SendKeys(maximumStaffAttendance);

            // select whether minors are attending - yes
            NgWebElement uiMinorsAttending = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-event-form/div/form/div[6]/div[6]/app-field/section/div[1]/section/select/option[1]"));
            uiMinorsAttending.Click();

            // select type of food service provided - Full Service Meal
            NgWebElement uiFoodServiceProvided = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-event-form/div/form/div[6]/div[7]/app-field/section/div[1]/section/select/option[3]"));
            uiFoodServiceProvided.Click();

            // select type of entertainment provided - Live Entertainment
            NgWebElement uiEntertainmentProvided = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-event-form/div/form/div[6]/div[8]/app-field/section/div[1]/section/select/option[4]"));
            uiEntertainmentProvided.Click();

            // enter venue name description
            NgWebElement uiVenueNameDescription = ngDriver.FindElement(By.XPath("//div[7]/div/app-field/section/div/section/textarea"));
            uiVenueNameDescription.SendKeys(venueNameDescription);

            // select venue location - both (indoors/outdoors)
            NgWebElement uiVenueLocation = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-event-form/div/form/div[7]/div[2]/app-field/section/div[1]/section/select/option[3]"));
            uiVenueLocation.Click();

            // enter venue additional info
            NgWebElement uiVenueAdditionalInfo = ngDriver.FindElement(By.XPath("//div[3]/app-field/section/div/section/textarea"));
            uiVenueAdditionalInfo.SendKeys(venueAdditionalInfo);

            // enter physical address - street address 1
            NgWebElement uiPhysicalAddStreetAddress1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            uiPhysicalAddStreetAddress1.SendKeys(physicalAddStreetAddress1);

            // enter physical address - street address 2 
            NgWebElement uiPhysicalAddStreetAddress2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiPhysicalAddStreetAddress2.SendKeys(physicalAddStreetAddress2);

            // enter physical address - city
            NgWebElement uiPhysicalAddCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
            uiPhysicalAddCity.SendKeys(physicalAddCity);

            // enter physical address - postal code
            NgWebElement uiPhysicalAddPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiPhysicalAddPostalCode.SendKeys(physicalAddPostalCode);

            // select start date
            NgWebElement uiVenueStartDate1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            uiVenueStartDate1.Click();

            NgWebElement uiVenueStartDate2 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-0']/div/mat-month-view/table/tbody/tr[5]/td[5]/div"));
            uiVenueStartDate2.Click();

            // select end date
            NgWebElement uiVenueEndDate1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiVenueEndDate1.Click();

            NgWebElement uiVenueEndDate2 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-1']/div/mat-month-view/table/tbody/tr[5]/td[5]/div"));
            uiVenueEndDate2.Click();

            // select event and liquor service times are different on specific dates checkbox
            NgWebElement uiEventLiquorServiceTimesDifferent = ngDriver.FindElement(By.Id("mat-checkbox-1"));
            uiEventLiquorServiceTimesDifferent.Click();

            // select terms and conditions checkbox
            NgWebElement uiTermsAndConditions = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-event-form/div/form/div[9]/div/mat-checkbox"));
            uiTermsAndConditions.Click();

            // click on the submit button
            NgWebElement submitOrgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT')]"));
            submitOrgInfoButton.Click();

            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            // click on the Event History bar - TODO
            // NgWebElement expandEventHistory = ngDriver.FindElement(By.Id("mat-expansion-panel-header-1"));
            // expandEventHistory.Click();
            
            // confirm that the Event Status = In Review and the Client or Host Name is present - TODO
            // Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,eventContactName)]")).Displayed);
        }

        [And(@"I request a valid store name or branding change")]
        public void name_branding_change()
        {
            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            string nameBrandingLink = "Request Store Name or Branding Change";

            // click on the Request Store Name or Branding Change link
            NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText(nameBrandingLink));
            uiRequestChange.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            // click on the Continue to Application button
            NgWebElement continueButton = ngDriver.FindElement(By.XPath("//button[contains(.,'CONTINUE TO APPLICATION')]"));
            continueButton.Click();

            /*
            Page Title: Submit a Name or Branding Change Application
            */

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a supporting document
            string supportingDocument = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadSupportingDoc = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSupportingDoc.SendKeys(supportingDocument);

            // click on the authorized to submit checkbox
            NgWebElement uiAuthSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSigAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSigAgreement.Click();

            // click on the Submit & Pay button
            NgWebElement submitpayButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT')]"));
            submitpayButton.Click();

            // pay for the branding change application
            MakePayment();

            System.Threading.Thread.Sleep(7000);

            // return to the Licences tab
            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }

        [And(@"I request a store relocation")]
        public void request_store_relocation()
        {
            /* 
            Page Title: Licences
            Subtitle: Catering Licences
            */

            string requestRelocationLink = "Request Relocation";

            // click on the request location link
            NgWebElement uiRequestRelocation = ngDriver.FindElement(By.LinkText(requestRelocationLink));
            uiRequestRelocation.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            // click on the Continue to Application button
            NgWebElement continueButton = ngDriver.FindElement(By.XPath("//button[contains(.,'CONTINUE TO APPLICATION')]"));
            continueButton.Click();

            /* 
            Page Title: Submit a Licence Relocation Application
            */

            // create test data
            string streetAddress = "303 Prideaux St";
            string city = "Nanaimo";
            string postal = "V9R2N3";

            // enter the proposed street address
            NgWebElement uiStreetAddress = ngDriver.FindElement(By.XPath("(//input[@id='establishmentAddressStreet'])[2]"));
            uiStreetAddress.SendKeys(streetAddress);

            // enter the proposed street city
            NgWebElement uiCity = ngDriver.FindElement(By.XPath("(//input[@id='establishmentAddressCity'])[2]"));
            uiCity.SendKeys(city);

            // enter the proposed postal code
            NgWebElement uiPostalCode = ngDriver.FindElement(By.XPath("(//input[@id='establishmentAddressPostalCode'])[3]"));
            uiPostalCode.SendKeys(postal);

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a supporting document
            string supportingDocument = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "checklist.pdf");
            NgWebElement uploadSupportingDoc = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSupportingDoc.SendKeys(supportingDocument);

            // select the authorized to submit checkbox
            NgWebElement uiAuthToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSigAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSigAgreement.Click();

            // click on the Submit & Pay button
            NgWebElement submitpayButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT')]"));
            submitpayButton.Click();

            // pay for the relocation application
            MakePayment();

            System.Threading.Thread.Sleep(7000);
        }

        [And(@"I submit a third party application")]
        public void third_party_operator_application()
        {
            // return to the Licences tab
            string licencesLink = "Licences";

            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();

            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            string thirdpartyapp = "Third-Party Operator Application";

            // click on the Third-Party Operator link
            NgWebElement uiThirdPartyOp = ngDriver.FindElement(By.LinkText(thirdpartyapp));
            uiThirdPartyOp.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            // click on the Continue to Application button
            NgWebElement continueButton = ngDriver.FindElement(By.XPath("//button[contains(.,'CONTINUE TO APPLICATION')]"));
            continueButton.Click();

            /* 
            Page Title: Third-Party Operator Application
            */

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[2]"));
            uiSignatureAgreement.Click();

            // click on submit and pay button
            NgWebElement submitButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT')]"));
            submitButton.Click();

            // pay for the application
            MakePayment();

            System.Threading.Thread.Sleep(7000);

            // return to the Licences tab
            string licencesLink2 = "Licences";

            NgWebElement uiLicences2 = ngDriver.FindElement(By.LinkText(licencesLink2));
            uiLicences2.Click();
        }

        [And(@"I request a third party operator")]
        public void request_third_party_operator()
        {
            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            string addOrChangeThirdParty = "Add or Change a Third Party Operator";

            // click on the Add or Change a Third Party Operator Link
            NgWebElement uiAddOrChangeThirdPartyOp = ngDriver.FindElement(By.LinkText(addOrChangeThirdParty));
            uiAddOrChangeThirdPartyOp.Click();

            /* 
            Page Title: Add or Change a Third Party Operator
            */

            string thirdparty = "GunderCorp TestBusiness";

            // search for the proposed licensee
            NgWebElement thirdPartyOperator = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            thirdPartyOperator.SendKeys(thirdparty);

            NgWebElement thirdPartyOperatorOption = ngDriver.FindElement(By.XPath("//mat-option[@id='mat-option-0']/span"));
            thirdPartyOperatorOption.Click();

            // click on authorized to submit checkbox
            NgWebElement authorizedToSubmit = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            authorizedToSubmit.Click();

            // click on signature agreement checkbox
            NgWebElement signatureAgreement = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[2]"));
            signatureAgreement.Click();

            // click on submit button
            NgWebElement submitButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT')]"));
            submitButton.Click();

            // return to the Licences tab
            string licencesLink = "Licences";

            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();

            // confirm that the application has been initiated
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Third Party Operator Application Initiated')]")).Displayed);
        }

        [And(@"I request a transfer of ownership")]
        public void request_transfer_of_ownership()
        {
            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            string transferOwnership = "Transfer Ownership";

            // click on the Transfer Ownership link
            NgWebElement uiTransferOwnership = ngDriver.FindElement(By.LinkText(transferOwnership));
            uiTransferOwnership.Click();

            /* 
            Page Title: Transfer Your Catering Licence
            */

            string licensee = "GunderCorp TestBusiness";

            // search for the proposed licensee
            NgWebElement proposedLicensee = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            proposedLicensee.SendKeys(licensee);

            NgWebElement thirdPartyOperatorOption = ngDriver.FindElement(By.XPath("//*[@id='mat-option-1']/span"));
            thirdPartyOperatorOption.Click();

            // click on consent to licence transfer checkbox
            NgWebElement consentToTransfer = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            consentToTransfer.Click();

            // click on authorize signature checkbox
            NgWebElement authorizeSignature = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[2]"));
            authorizeSignature.Click();

            // click on signature agreement checkbox
            NgWebElement signatureAgreement = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[3]"));
            signatureAgreement.Click();

            // click on submit transfer button
            NgWebElement submitTransferButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT TRANSFER')]"));
            submitTransferButton.Click();

            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();

            // check for transfer initiated status 
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'TRANSFER INITIATED')]")).Displayed);
        }

        [Then(@"the requested applications are visible on the dashboard")]
        public void applications_visible_on_dashboard()
        {
            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            string dashboard = "Dashboard";

            // click on the Dashboard link
            NgWebElement uiDashboard = ngDriver.FindElement(By.LinkText(dashboard));
            uiDashboard.Click();

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // confirm that relocation request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Relocation Request')]")).Displayed);

            // confirm that a name or branding change request is displayed - TODO
            //Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name or Branding Change')]")).Displayed);

            // confirm that a third party operator request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Third-Party Operator')]")).Displayed);
        }
    }
}