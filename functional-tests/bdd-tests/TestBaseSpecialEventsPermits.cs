using System;
using System.Threading;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the special events permits applicant info")]
        public void SpecialEventsPermitsApplicantInfo()
        {
            /* 
            Page Title: Applicant Info
            */

            string eventName = "SEP test event";

            // enter the event name
            var uiEventName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='eventName']"));
            uiEventName.SendKeys(eventName);

            // click on the terms and conditions checkbox
            var uiTermsAndConditions = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreeToTnC']"));
            uiTermsAndConditions.Click();
        }


        [And(@"I complete the SEP eligibility form")]
        public void SpecialEventsPermitsEligibility()
        {
            /* 
            Page Title: Eligibility
            */

            var responsibleBeverageServiceNumber = "1234567";
            var occasionOfEvent = "Description of event occasion.";
            var organizationName = "Test organization name";
            var organizationAddress = "645 Tyee RdVictoria, BC V9A 6X5";

            // select 'No' for 'Is this event being hosted at a private residence?'
            var uiPrivateResidence = ngDriver.FindElement(By.Id("mat-radio-3"));
            uiPrivateResidence.Click();

            // select 'No' for 'Is your event being held on public property?'
            var uiPublicProperty = ngDriver.FindElement(By.Id("mat-radio-6"));
            uiPublicProperty.Click();

            // select 'No' for 'Is this an event of provincial, national, or international significance?'
            // * already selected 'No' by default

            // select event start date
            var uiEventStartDate = ngDriver.FindElement(By.CssSelector("input[formControlName='eventStartDate']"));
            uiEventStartDate.Click();

            SharedCalendarDate();

            // select public/private options re event
            var uiPrivateEvent = ngDriver.FindElement(By.CssSelector("[formcontrolname='eligibilityPrivateOrPublic'] option[value='2']"));
            uiPrivateEvent.Click();

            // enter name of organization hosting the event
            var uiOrganizationName = ngDriver.FindElement(By.CssSelector("[formcontrolname='hostingOrganizationName']"));
            uiOrganizationName.SendKeys(organizationName);

            // enter the organization address
            var uiOrganizationAddress = ngDriver.FindElement(By.CssSelector("[formcontrolname='hostingOrganizationAddress']"));
            uiOrganizationAddress.SendKeys(organizationAddress);

            // select organization category
            var uiOrganizationCategory = ngDriver.FindElement(By.CssSelector("[formcontrolname='hostingOrganizationCategory'] option[value='incorporated_non_profit']"));
            uiPrivateEvent.Click();

            // enter Responsible Beverage Service Number
            var uiResponsibleBeverageServiceNumber = ngDriver.FindElement(By.CssSelector("input[formControlName='eligibilityResponsibleBevServiceNumber']"));
            uiResponsibleBeverageServiceNumber.SendKeys(responsibleBeverageServiceNumber);

            // enter the occasion of the event
            var uiEventOccasion = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventDescription']"));
            uiEventOccasion.SendKeys(occasionOfEvent);

            // select 'No' for 'Are you charging an event admission price?'
            var uiEventAdmissionPrice = ngDriver.FindElement(By.Id("mat-radio-12"));
            uiEventAdmissionPrice.Click();
            
            // select 'No' for 'Is there currently a liquor licence at your event location?'
            var uiCurrentLiquorLicence = ngDriver.FindElement(By.Id("mat-radio-15"));
            uiCurrentLiquorLicence.Click();
        }


        [And(@"I complete the SEP event form")]
        public void SpecialEventsPermitsEventForm()
        {
            /* 
            Event Location
            */

            var eventLocationName = "Point Ellis Winery";
            var venueType = "banquet hall";
            var addressLine1 = "Point Ellis Avenue";
            var addressLine2 = "Victoria West";
            var postalCode = "V9A6X3";
            var maxGuests = "100";
            var areaDescription = "Description of area";

            // select event location
            var uiEventLocation =
                ngDriver.FindElement(
                By.CssSelector("[formcontrolname='lgIn'] option[value='Vancouver']"));
            uiEventLocation.Click();

            // select 'Yes' for annual event
            var uiAnnualEventYes = ngDriver.FindElement(By.Id("mat-radio-19"));
            uiAnnualEventYes.Click();

            // enter event location name
            var uiEventLocationName = ngDriver.FindElement(By.CssSelector("[formcontrolname='locationName']"));
            uiEventLocationName.SendKeys(eventLocationName);

            // enter venue type
            var uiVenueType = ngDriver.FindElement(By.CssSelector("[formcontrolname='venueType']"));
            uiVenueType.SendKeys(venueType);

            // enter address line 1
            var uiAddressLine1 = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventLocationStreet1']"));
            uiAddressLine1.SendKeys(addressLine1);

            // enter address line 2
            var uiAddressLine2 = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventLocationStreet2']"));
            uiAddressLine2.SendKeys(addressLine2);

            // enter postal code
            var uiPostalCode = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventLocationPostalCode']"));
            uiPostalCode.SendKeys(postalCode);

            // enter maximum number of guests
            var uiMaxGuests = ngDriver.FindElement(By.CssSelector("[formcontrolname='locationMaxGuests']"));
            uiMaxGuests.SendKeys(maxGuests);

            // enter area description
            var uiAreaDescription = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventName']"));
            uiAreaDescription.SendKeys(areaDescription);

            // enter maximum number of guests
            var uiMaxGuests2 = ngDriver.FindElement(By.CssSelector("[formcontrolname='licencedAreaMaxNumberOfGuests']"));
            uiMaxGuests2.SendKeys(maxGuests);

            // select 'No' for minors admission
            var uiMinorsAdmissionNo = ngDriver.FindElement(By.Id("mat-radio-25"));
            uiMinorsAdmissionNo.Click();

            // select event date
            var uiEventDate = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventDate']"));
            uiEventDate.Click();

            SharedCalendarDate();

            // select event start time
            var uiEventStartTime = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventStart'] option[value='8:00 AM']"));
            uiEventStartTime.Click();

            // select event end time
            var uiEventEndTime = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventEnd'] option[value='4:00 PM']"));
            uiEventEndTime.Click();

            // select liquor service start time
            var uiLiquorServiceStartTime = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceStart'] option[value='9:00 AM']"));
            uiLiquorServiceStartTime.Click();

            // select liquor service end time
            var uiLiquorServiceEndTime = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceEnd'] option[value='4:00 PM']"));
            uiLiquorServiceEndTime.Click();
        }


        [And(@"the SEP Checklist content is displayed")]
        public void SpecialEventsPermitsChecklist()
        {
            /* 
            Page Title: Application Checklist
            */

            for (var i = 0; i < 10; i++)
                try
                {
                    Thread.Sleep(3000);

                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Application Checklist')]")).Displayed);
                    break;
                }
                catch (Exception e)
                { }
        }


        [And(@"Account Profile is displayed")]
        public void AccountProfileDisplayed()
        {
            /* 
            Page Title: Account Profile
            */

            for (var i = 0; i < 10; i++)
                try
                {
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Account Profile ')]")).Displayed);
                    break;
                }
                catch (Exception e)
                { }
        }


        [And(@"the Current Applications label is displayed")]
        public void CurrentApplicationsDisplayed()
        {
            /* 
            Page Title: Current Applications
            */

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Current Applications')]")).Displayed);
        }


        [And(@"the Plan Your Drinks label is displayed")]
        public void DrinkPlannerDisplayed()
        {
            /* 
            Page Title: Drink Planner
            */

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Drink Planner')]")).Displayed);
        }


        [And(@"I complete the Drink Planner form")]
        public void CompleteDrinkPlanner()
        {
            /* 
            Page Title: Drink Planner
            */

            string hours = "5";
            string guests = "500";
            string beer = "30";
            string wine = "20";
            string spirits = "10";

            // enter the event hours
            var uiEventHours = ngDriver.FindElement(By.CssSelector("input[formcontrolname='hours']"));
            uiEventHours.Clear();
            uiEventHours.SendKeys(hours);

            // enter the event guests
            var uiEventGuests = ngDriver.FindElement(By.CssSelector("input[formcontrolname='guests']"));
            uiEventGuests.Clear();
            uiEventGuests.SendKeys(guests);
 
            // enter the beer percentage
            var uiBeer = ngDriver.FindElement(By.XPath("//app-drink-planner/div/form/div[3]/div[1]/app-field/section/div/section/input"));
            uiBeer.Clear();
            uiBeer.SendKeys(beer);

            // enter the wine percentage
            var uiWine = ngDriver.FindElement(By.XPath("//app-drink-planner/div/form/div[3]/div[2]/app-field/section/div/section/input"));
            uiWine.Clear();
            uiWine.SendKeys(wine);
    
            // enter the spirits percentage
            var uiSpirits = ngDriver.FindElement(By.XPath("//app-drink-planner/div/form/div[3]/div[3]/app-field/section/div/section/input"));
            uiSpirits.Clear();
            uiSpirits.SendKeys(spirits);
        }


        [And(@"the Drink Planner calculations are correct")]
        public void DrinkPlannerCalculations()
        {
            /* 
            Page Title: Drink Planner
            */

                for (var i = 0; i< 10; i++)
                try
                {
                    Thread.Sleep(3000);

                    // check the total number of drinks
                    Assert.True(ngDriver.FindElement(By.XPath("//app-drink-planner/div/form/div[4]/div/div/div[2]/h3/span[contains(.,'3,333')]")).Displayed);
                    break;
                }
                catch (Exception e)
                { }
        }
    }
}