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
        [And(@"I renew the licence")]
        public void RenewLicence()
        {
            string renewLicenceLink = "Renew Licence";

            // click on the Renew Licence link
            NgWebElement uiRenewLicence = ngDriver.FindElement(By.LinkText(renewLicenceLink));
            uiRenewLicence.Click();

            // select 'No'
            // 1. Have you or any partner, shareholder, director, or officer of this licensee been arrested for, charged with, or convicted of a criminal offence within the past 12 months that you have not reported to the LCRB?
            NgWebElement uiCriminalOffence = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalCriminalOffenceCheck'] button#mat-button-toggle-16-button"));
            uiCriminalOffence.Click();

            // select 'No'
            // 2. Has there been an unreported sale of the business associated with the licence within the past 12 months? 
            NgWebElement uiUnreportedSale = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalUnreportedSaleOfBusiness'] button#mat-button-toggle-18-button"));
            uiUnreportedSale.Click();

            // select 'No'
            // 3. Our records show that this establishment is licensed as a Private Corporation. Has this changed? 
            NgWebElement uiBusinessType = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalBusinessType'] button#mat-button-toggle-20-button"));
            uiBusinessType.Click();

            // select 'No'
            // 4. Have you, any partner, shareholder, director, officer, or an immediate family member of any of the aforementioned associates acquired a new interest or expanded an existing interest - financial or otherwise - in a federal producer of cannabis within the past 12 months? 
            NgWebElement uiTiedHouse = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalTiedhouse'] button#mat-button-toggle-22-button"));
            uiTiedHouse.Click();

            // select 'No'
            // 5. Has a federal produce of cannabis acquired a new interest or expanded an existing interest - financial or otherwise - in the licensee Private Corporation within the past 12 months? 
            NgWebElement uiTiedHouseFederalInterest = ngDriver.FindElement(By.CssSelector("[formcontrolname='tiedhouseFederalInterest'] button#mat-button-toggle-24-button"));
            uiTiedHouseFederalInterest.Click();

            // select 'No'
            // 6. Have you made any unreported changes to your organizational leadership within the past 12 months?
            NgWebElement uiOrgLeadership = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalOrgLeadership'] button#mat-button-toggle-26-button"));
            uiOrgLeadership.Click();

            // select 'No'
            // 7. Have you made any unreported changes to your key personnel within the past 12 months?
            NgWebElement uiKeyPersonnel = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalkeypersonnel'] button#mat-button-toggle-28-button"));
            uiKeyPersonnel.Click();

            // select 'No'
            // 8. Have you made any unreported changes to your share structure within the past 12 months? 
            NgWebElement uiShareholderStructure = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalShareholders'] button#mat-button-toggle-30-button"));
            uiShareholderStructure.Click();

            // select 'No'
            // 9. Do you have an outstanding payable fine under the Offence Act or outstanding payable monetary penalty under the Cannabis Control and Licensing Act that has not yet been paid?
            NgWebElement uiOutstandingFine = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalOutstandingFines'] button#mat-button-toggle-32-button"));
            uiOutstandingFine.Click();

            // select 'No'
            // 10. Have you made an unreported change to your store’s name in the past 12 months?
            NgWebElement uiBrandingChange = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalBranding'] button#mat-button-toggle-34-button"));
            uiBrandingChange.Click();

            // select 'No'
            // 11. Have you updated the store’s signage or branding in the past 12 months?
            NgWebElement uiSignageChange = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalSignage'] button#mat-button-toggle-36-button"));
            uiSignageChange.Click();

            // select 'No'
            // 12. Have you made an unreported change of location of your establishment within the past 12 months? (This includes any changes to the Parcel Identification Number where your establishment is located, even if the physical location has not changed).
            NgWebElement uiEstablishmentAddress = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalEstablishmentAddress'] button#mat-button-toggle-38-button"));
            uiEstablishmentAddress.Click();

            // select 'No'
            // 13. Have you sold the property or transferred the lease associated with this cannabis retail store licence within the past 12 months?
            NgWebElement uiValidInterest = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalValidInterest'] button#mat-button-toggle-40-button"));
            uiValidInterest.Click();

            // select 'No'
            // 14. Are you aware of any local government or Indigenous nation zoning changes with respect to your establishment location?
            NgWebElement uiZoning = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalZoning'] button#mat-button-toggle-42-button"));
            uiZoning.Click();

            // select 'No'
            // 15. Have you made any unreported changes to the store’s floor plan within the past 12 months? 
            NgWebElement uiFloorPlan = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalFloorPlan'] button#mat-button-toggle-44-button"));
            uiFloorPlan.Click();

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgree.Click();

            ClickOnSubmitButton();

            MakePayment();

            // confirm renewal amount
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$1500.00')]")).Displayed);
        }
    }
}
