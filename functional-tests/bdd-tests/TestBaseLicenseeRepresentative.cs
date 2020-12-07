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
        [And(@"I request a licensee representative")]
        public void RequestLicenseeRepresentative()
        {
            /* 
            Page Title: Licences & Authorizations
            */

            // click on the Licensee Representative link
            string addLicensee = "Add Licensee Representative";
            NgWebElement uiAddLicensee = ngDriver.FindElement(By.LinkText(addLicensee));
            uiAddLicensee.Click();

            // create test data
            string representativeName = "Automated Test";
            string telephone = "2005081818";
            string email = "automated@test.com";

            // enter the representative name
            NgWebElement uiFullName = ngDriver.FindElement(By.CssSelector("input[formControlName='representativeFullName']"));
            uiFullName.SendKeys(representativeName);

            // enter the representative telephone number
            NgWebElement uiPhoneNumber = ngDriver.FindElement(By.CssSelector("input[formControlName='representativePhoneNumber']"));
            uiPhoneNumber.SendKeys(telephone);

            // enter the representative email address
            NgWebElement uiEmail = ngDriver.FindElement(By.CssSelector("input[formControlName='representativeEmail']"));
            uiEmail.SendKeys(email);

            // click on the submit permanent change applications checkbox
            NgWebElement uiCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanSubmitPermanentChangeApplications']"));
            IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
            executor.ExecuteScript("arguments[0].click();", uiCheckbox);

            // click on the sign temporary change applications checkbox
            NgWebElement uiCheckbox1 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanSignTemporaryChangeApplications']"));
            uiCheckbox1.Click();

            // click on the obtain licence info from branch checkbox
            NgWebElement uiCheckbox2 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanObtainLicenceInformation']"));
            uiCheckbox2.Click();

            // click on sign grocery annual proof of sales revenue checkbox
            NgWebElement uiCheckbox3 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanSignGroceryStoreProofOfSale']"));
            uiCheckbox3.Click();

            // click on attend education sessions checkbox
            NgWebElement uiCheckbox4 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanAttendEducationSessions']"));
            uiCheckbox4.Click();

            // click on attend compliance meetings checkbox
            NgWebElement uiCheckbox5 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanAttendComplianceMeetings']"));
            uiCheckbox5.Click();

            // click on represent licensee at enforcement hearings checkbox
            NgWebElement uiCheckbox6 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanRepresentAtHearings']"));
            uiCheckbox6.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgree = ngDriver.FindElement(By.XPath("//app-field/section/div/section/section/input"));
            uiSignatureAgree.Click();

            ClickOnSubmitButton();

            // check that new licensee representation is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//app-licence-row/div/div/form/table/tr[2]/td[2]/div[4]/a/span[contains(.,'Licensee Representative: ')]")).Displayed);
        }
    }
}
