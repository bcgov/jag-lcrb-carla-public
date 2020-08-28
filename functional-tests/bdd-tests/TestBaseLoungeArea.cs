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
        [And(@"I request a lounge area endorsement")]
        public void LoungeAreaEndorsement()
        {
            /* 
            Page Title: Licences
            */

            string loungeAreaEndorsement = "Lounge Area Endorsement Application";
            string jobTitle = "CEO";

            // click on the Lounge Area Endorsement Application link
            NgWebElement uiLoungeAreaEndorsement = ngDriver.FindElement(By.LinkText(loungeAreaEndorsement));
            uiLoungeAreaEndorsement.Click();

            System.Threading.Thread.Sleep(9000);


            NgWebElement uiJobTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='jobTitle']"));
            uiJobTitle.SendKeys(jobTitle);

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Please Review Your Account Profile ')]")).Displayed);

            System.Threading.Thread.Sleep(4000);

            // click on the Continue to Application button
            NgWebElement uiContinueToApplicationButton = ngDriver.FindElement(By.CssSelector("button#continueToApp.save-cont.btn-primary"));
            uiContinueToApplicationButton.Click();



            /* 
            Page Title: Lounge Area Endorsement Application
            */

            // select the zoning checkbox
            NgWebElement uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1"));
            uiZoningCheckbox.Click();

            // select 'Yes' for ALR zoning           
            NgWebElement uiYesALRZoning = ngDriver.FindElement(By.CssSelector("[formcontrolname='isAlr'] mat-radio-button#mat-radio-2"));
            uiYesALRZoning.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the floor plan
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiUploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadFloorplan.SendKeys(floorplanPath);

            // add a service area
            NgWebElement uiServiceArea = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceAreas'] button"));
            uiServiceArea.Click();

            // creeate test data
            string areaDescription = "Area description";
            string occupantLoad = "100";

            // enter the area description
            NgWebElement uiAreaDescription = ngDriver.FindElement(By.CssSelector("input[formcontrolname='areaLocation']"));
            uiAreaDescription.SendKeys(areaDescription);

            // enter the occupant load
            NgWebElement uiOccupantLoad = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiOccupantLoad.SendKeys(occupantLoad);

            // upload the site plan
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadSitePlan.SendKeys(sitePlanPath);

            // select the Sunday opening time
            NgWebElement uiSundayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSundayOpen'] option[value='10:00']"));
            uiSundayOpen.Click();

            // select the Sunday closing time
            NgWebElement uiSundayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSundayClose'] option[value='16:00']"));
            uiSundayClose.Click();

            // select the Monday opening time
            NgWebElement uiMondayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursMondayOpen'] option[value='09:00']"));
            uiMondayOpen.Click();

            // select the Monday closing time
            NgWebElement uiMondayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursMondayClose'] option[value='23:00']"));
            uiMondayClose.Click();

            // select the Tuesday opening time
            NgWebElement uiTuesdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursTuesdayOpen'] option[value='09:15']"));
            uiTuesdayOpen.Click();

            // select the Tuesday closing time
            NgWebElement uiTuesdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursTuesdayClose'] option[value='22:45']"));
            uiTuesdayClose.Click();

            // select the Wednesday opening time
            NgWebElement uiWednesdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursWednesdayOpen'] option[value='09:30']"));
            uiWednesdayOpen.Click();

            // select the Wednesday closing time
            NgWebElement uiWednesdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursWednesdayClose'] option[value='12:00']"));
            uiWednesdayClose.Click();

            // select the Thursday opening time
            NgWebElement uiThursdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursThursdayOpen'] option[value='13:00']"));
            uiThursdayOpen.Click();

            // select the Thursday closing time
            NgWebElement uiThursdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursThursdayClose'] option[value='14:00']"));
            uiThursdayClose.Click();

            // select the Friday opening time
            NgWebElement uiFridayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursFridayOpen'] option[value='12:15']"));
            uiFridayOpen.Click();

            // select the Friday closing time
            NgWebElement uiFridayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursFridayClose'] option[value='21:15']"));
            uiFridayClose.Click();

            // select the Saturday opening time
            NgWebElement uiSaturdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSaturdayOpen'] option[value='10:00']"));
            uiSaturdayOpen.Click();

            // select the Saturday closing time
            NgWebElement uiSaturdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSaturdayClose'] option[value='22:00']"));
            uiSaturdayClose.Click();

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            ClickOnSubmitButton();

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Pending External Review ')]")).Displayed);
        }
    }
}
