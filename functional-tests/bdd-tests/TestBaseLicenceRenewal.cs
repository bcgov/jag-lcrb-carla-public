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
        [And(@"I renew the licence with (.*)")]
        public void RenewLicence(string responses)
        {
            if (responses == "negative responses for Cannabis")
            {
                // select 'No'
                // 1. Have you or any partner, shareholder, director, or officer of this licensee been arrested for, charged with, or convicted of a criminal offence within the past 12 months that you have not reported to the LCRB?
                NgWebElement uiCriminalOffence = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalCriminalOffenceCheck'] button#mat-button-toggle-10-button"));
                uiCriminalOffence.Click();

                // select 'No'
                // 2. Has there been an unreported sale of the business associated with the licence within the past 12 months? 
                NgWebElement uiUnreportedSale = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalUnreportedSaleOfBusiness'] button#mat-button-toggle-12-button"));
                uiUnreportedSale.Click();

                // select 'No'
                // 3. Our records show that this establishment is licensed as a Private Corporation. Has this changed? 
                NgWebElement uiBusinessType = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalBusinessType'] button#mat-button-toggle-14-button"));
                uiBusinessType.Click();

                // select 'No'
                // 4. Have you, any partner, shareholder, director, officer, or an immediate family member of any of the aforementioned associates acquired a new interest or expanded an existing interest - financial or otherwise - in a federal producer of cannabis within the past 12 months? 
                NgWebElement uiTiedHouse = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalTiedhouse'] button#mat-button-toggle-16-button"));
                uiTiedHouse.Click();

                // select 'No'
                // 5. Has a federal produce of cannabis acquired a new interest or expanded an existing interest - financial or otherwise - in the licensee Private Corporation within the past 12 months? 
                NgWebElement uiTiedHouseFederalInterest = ngDriver.FindElement(By.CssSelector("[formcontrolname='tiedhouseFederalInterest'] button#mat-button-toggle-18-button"));
                uiTiedHouseFederalInterest.Click();

                // select 'No'
                // 6. Have you made any unreported changes to your organizational leadership within the past 12 months?
                NgWebElement uiOrgLeadership = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalOrgLeadership'] button#mat-button-toggle-20-button"));
                uiOrgLeadership.Click();

                // select 'No'
                // 7. Have you made any unreported changes to your key personnel within the past 12 months?
                NgWebElement uiKeyPersonnel = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalkeypersonnel'] button#mat-button-toggle-22-button"));
                uiKeyPersonnel.Click();

                // select 'No'
                // 8. Have you made any unreported changes to your share structure within the past 12 months? 
                NgWebElement uiShareholderStructure = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalShareholders'] button#mat-button-toggle-24-button"));
                uiShareholderStructure.Click();

                // select 'No'
                // 9. Do you have an outstanding payable fine under the Offence Act or outstanding payable monetary penalty under the Cannabis Control and Licensing Act that has not yet been paid?
                NgWebElement uiOutstandingFine = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalOutstandingFines'] button#mat-button-toggle-26-button"));
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
            }

            if (responses == "positive responses for Cannabis")
            {
                // select 'Yes'
                // 1. Have you or any partner, shareholder, director, or officer of this licensee been arrested for, charged with, or convicted of a criminal offence within the past 12 months that you have not reported to the LCRB?
                NgWebElement uiCriminalOffence = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalCriminalOffenceCheck'] button#mat-button-toggle-9-button"));
                uiCriminalOffence.Click();

                // select 'Yes'
                // 2. Has there been an unreported sale of the business associated with the licence within the past 12 months? 
                NgWebElement uiUnreportedSale = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalUnreportedSaleOfBusiness'] button#mat-button-toggle-11-button"));
                uiUnreportedSale.Click();

                // select 'Yes'
                // 3. Our records show that this establishment is licensed as a Private Corporation. Has this changed? 
                NgWebElement uiBusinessType = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalBusinessType'] button#mat-button-toggle-13-button"));
                uiBusinessType.Click();

                // select 'Yes'
                // 4. Have you, any partner, shareholder, director, officer, or an immediate family member of any of the aforementioned associates acquired a new interest or expanded an existing interest - financial or otherwise - in a federal producer of cannabis within the past 12 months? 
                NgWebElement uiTiedHouse = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalTiedhouse'] button#mat-button-toggle-15-button"));
                uiTiedHouse.Click();

                // select 'Yes'
                // 5. Has a federal produce of cannabis acquired a new interest or expanded an existing interest - financial or otherwise - in the licensee Private Corporation within the past 12 months? 
                NgWebElement uiTiedHouseFederalInterest = ngDriver.FindElement(By.CssSelector("[formcontrolname='tiedhouseFederalInterest'] button#mat-button-toggle-17-button"));
                uiTiedHouseFederalInterest.Click();

                // select 'Yes'
                // 6. Have you made any unreported changes to your organizational leadership within the past 12 months?
                NgWebElement uiOrgLeadership = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalOrgLeadership'] button#mat-button-toggle-19-button"));
                uiOrgLeadership.Click();

                // select 'Yes'
                // 7. Have you made any unreported changes to your key personnel within the past 12 months?
                NgWebElement uiKeyPersonnel = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalkeypersonnel'] button#mat-button-toggle-21-button"));
                uiKeyPersonnel.Click();

                // select 'Yes'
                // 8. Have you made any unreported changes to your share structure within the past 12 months? 
                NgWebElement uiShareholderStructure = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalShareholders'] button#mat-button-toggle-23-button"));
                uiShareholderStructure.Click();

                // select 'Yes'
                // 9. Do you have an outstanding payable fine under the Offence Act or outstanding payable monetary penalty under the Cannabis Control and Licensing Act that has not yet been paid?
                NgWebElement uiOutstandingFine = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalOutstandingFines'] button#mat-button-toggle-25-button"));
                uiOutstandingFine.Click();

                // select 'Yes'
                // 10. Have you made an unreported change to your store’s name in the past 12 months?
                NgWebElement uiBrandingChange = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalBranding'] button#mat-button-toggle-33-button"));
                uiBrandingChange.Click();

                // select 'Yes'
                // 11. Have you updated the store’s signage or branding in the past 12 months?
                NgWebElement uiSignageChange = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalSignage'] button#mat-button-toggle-35-button"));
                uiSignageChange.Click();

                // select 'Yes'
                // 12. Have you made an unreported change of location of your establishment within the past 12 months? (This includes any changes to the Parcel Identification Number where your establishment is located, even if the physical location has not changed).
                NgWebElement uiEstablishmentAddress = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalEstablishmentAddress'] button#mat-button-toggle-37-button"));
                uiEstablishmentAddress.Click();

                // select 'Yes'
                // 13. Have you sold the property or transferred the lease associated with this cannabis retail store licence within the past 12 months?
                NgWebElement uiValidInterest = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalValidInterest'] button#mat-button-toggle-39-button"));
                uiValidInterest.Click();

                // select 'Yes'
                // 14. Are you aware of any local government or Indigenous nation zoning changes with respect to your establishment location?
                NgWebElement uiZoning = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalZoning'] button#mat-button-toggle-41-button"));
                uiZoning.Click();

                // select 'Yes'
                // 15. Have you made any unreported changes to the store’s floor plan within the past 12 months? 
                NgWebElement uiFloorPlan = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalFloorPlan'] button#mat-button-toggle-43-button"));
                uiFloorPlan.Click();

                // confirm that correct information re positive responses for a Cannabis licensing renewal is displayed
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'You can still renew your licence. Please contact us as soon as possible to transfer this licence to its new owner. ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'You can still renew your licence. Please contact us to update this information. ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' You can still renew your licence. After submitting your renewal, you can update your federal producer information on the Account Profile page on the Dashboard. A member of LCRB may contact you to determine any additional next steps. ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' You can still renew your licence. Please contact us to to pay your fines. ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' You can still renew your licence. After submitting your renewal, please start a Establishment Name or Branding Change application from the Dashboard. ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'You can still renew your licence. After submitting your renewal, please start a Relocation Request application from the Dashboard. ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'A LCRB Licensing Staff member may contact you as part of this renewal process ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' You can still renew your licence. After submitting your renewal, please start a Structural Change Request application from the Dashboard. ')]")).Displayed);
            }

            if (responses == "negative responses for Catering")
            {
                // select 'No'
                // 1.Have you or any partner, shareholder, director, or officer of this licensee been arrested for, charged with, or convicted of a criminal offence within the past 12 months that you have not reported to the LCRB ?
                NgWebElement uiCriminalOffence = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalCriminalOffenceCheck'] button#mat-button-toggle-11-button"));
                uiCriminalOffence.Click();

                // select 'No'
                // 2. Have you or any of your partners, shareholders or directors of this establishment received any alcohol related driving infractions in the past 12 months?
                NgWebElement uiAlcoholInfraction = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalDUI'] button#mat-button-toggle-13-button"));
                uiAlcoholInfraction.Click();

                // select 'No'
                // 3. Our records show that this establishment is licensed as a PrivateCorporation. Has this changed?
                NgWebElement uiBusinessType = ngDriver.FindElement(By.CssSelector("[formcontrolname = 'renewalBusinessType'] button#mat-button-toggle-15-button"));
                uiBusinessType.Click();

                // select 'No'
                // 4. Have you redistributed any shares within the past 12 months without notifying LCRB?
                NgWebElement uiRenewalShareholders = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalShareholders'] button#mat-button-toggle-17-button"));
                uiRenewalShareholders.Click();

                // select 'No'
                // 5. Have you entered into an agreement allowing another person or business to use your licence within the past 12 months?
                NgWebElement uiRenewalThirdParty = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalThirdParty'] button#mat-button-toggle-19-button"));
                uiRenewalThirdParty.Click();

                // select 'No'
                // 6. Have you made any unreported structural changes to your establishment within the past 12 months?
                NgWebElement uiRenewalFloorPlan = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalFloorPlan'] button#mat-button-toggle-21-button"));
                uiRenewalFloorPlan.Click();

                // select 'No'
                // 7. Have you acquired a new interest or expanded an existing interest financial or otherwise in a winery, brewery, distillery, liquor agent and/or a UBrew/UVin within the past 12 months without notifying LCRB?
                NgWebElement uiRenewalTiedhouse = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalTiedhouse'] button#mat-button-toggle-23-button"));
                uiRenewalTiedhouse.Click();

                // select 'No'
                // 8. Have you sold the business associated with this liquor licence within the last 12 months without notifying LCRB?
                NgWebElement uiRenewalUnreportedSaleOfBusiness = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalUnreportedSaleOfBusiness'] button#mat-button-toggle-25-button"));
                uiRenewalUnreportedSaleOfBusiness.Click();

                // select 'No'
                // 9.Have you sold the property or transferred the lease associated with this liquor licence within the last 12 months?
                NgWebElement uiRenewalValidInterest = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalValidInterest'] button#mat-button-toggle-27-button"));
                uiRenewalValidInterest.Click();

                // select 'No'
                // 10. Have you added, changed or removed a licensee representative within the past 12 months?
                NgWebElement uiRenewalKeyPersonnel = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalkeypersonnel'] button#mat-button-toggle-29-button"));
                uiRenewalKeyPersonnel.Click();
            }

            if (responses == "positive responses for Catering")
            {
                // select 'Yes'
                // 1.Have you or any partner, shareholder, director, or officer of this licensee been arrested for, charged with, or convicted of a criminal offence within the past 12 months that you have not reported to the LCRB ?
                NgWebElement uiCriminalOffence = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalCriminalOffenceCheck'] button#mat-button-toggle-10-button"));
                uiCriminalOffence.Click();

                // select 'Yes'
                // 2. Have you or any of your partners, shareholders or directors of this establishment received any alcohol related driving infractions in the past 12 months?
                NgWebElement uiAlcoholInfraction = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalDUI'] button#mat-button-toggle-12-button"));
                uiAlcoholInfraction.Click();

                // select 'Yes'
                // 3. Our records show that this establishment is licensed as a PrivateCorporation. Has this changed?
                NgWebElement uiBusinessType = ngDriver.FindElement(By.CssSelector("[formcontrolname = 'renewalBusinessType'] button#mat-button-toggle-14-button"));
                uiBusinessType.Click();

                // select 'Yes'
                // 4. Have you redistributed any shares within the past 12 months without notifying LCRB?
                NgWebElement uiRenewalShareholders = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalShareholders'] button#mat-button-toggle-16-button"));
                uiRenewalShareholders.Click();

                // select 'Yes'
                // 5. Have you entered into an agreement allowing another person or business to use your licence within the past 12 months?
                NgWebElement uiRenewalThirdParty = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalThirdParty'] button#mat-button-toggle-18-button"));
                uiRenewalThirdParty.Click();

                // select 'Yes'
                // 6. Have you made any unreported structural changes to your establishment within the past 12 months?
                NgWebElement uiRenewalFloorPlan = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalFloorPlan'] button#mat-button-toggle-20-button"));
                uiRenewalFloorPlan.Click();

                // select 'Yes'
                // 7. Have you acquired a new interest or expanded an existing interest financial or otherwise in a winery, brewery, distillery, liquor agent and/or a UBrew/UVin within the past 12 months without notifying LCRB?
                NgWebElement uiRenewalTiedhouse = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalTiedhouse'] button#mat-button-toggle-22-button"));
                uiRenewalTiedhouse.Click();

                // select 'Yes'
                // 8. Have you sold the business associated with this liquor licence within the last 12 months without notifying LCRB?
                NgWebElement uiRenewalUnreportedSaleOfBusiness = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalUnreportedSaleOfBusiness'] button#mat-button-toggle-24-button"));
                uiRenewalUnreportedSaleOfBusiness.Click();

                // select 'Yes'
                // 9.Have you sold the property or transferred the lease associated with this liquor licence within the last 12 months?
                NgWebElement uiRenewalValidInterest = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalValidInterest'] button#mat-button-toggle-26-button"));
                uiRenewalValidInterest.Click();

                // select 'Yes'
                // 10. Have you added, changed or removed a licensee representative within the past 12 months?
                NgWebElement uiRenewalKeyPersonnel = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalkeypersonnel'] button#mat-button-toggle-28-button"));
                uiRenewalKeyPersonnel.Click();

                // confirm that correct information re positive responses for a Catering licensing renewal is displayed
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Update your Shareholder Information on the Organization Details page')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Submit a Third Party Operator application from the Licences Dashboard')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Submit a Structural Change from the Licences Dashboard')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Update your Financial Interest information on the Account Profile')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Transfer this Licence from the Licences Dashboard')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Add, Remove or Update your Licensee Representative from the Licences Dashboard')]")).Displayed);   
            }

            if ((responses == "positive responses for a brewery") || (responses == "positive responses for a winery") || (responses == "positive responses for a distillery") || (responses == "positive responses for a co-packer"))
            {
                if (responses == "positive responses for a winery")
                {
                    // click on manufacturer minimum checkbox
                    NgWebElement uiIsManufacturedMinimum = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isManufacturedMinimum']"));
                    uiIsManufacturedMinimum.Click();
                }

                // select 'Yes'
                // 1. Have you or any partner, shareholder, director, or officer of this licensee been arrested for, charged with, or convicted of a criminal offence within the past 12 months that you have not reported to the LCRB?
                NgWebElement uiRenewalCriminalOffenceCheckYes = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalCriminalOffenceCheck'] button#mat-button-toggle-10-button"));
                uiRenewalCriminalOffenceCheckYes.Click();

                // select 'Yes'
                // 2. Have you or any of your partners, shareholders or directors of this establishment received any alcohol related driving infractions in the past 12 months?
                NgWebElement uiRenewalDUIYes = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalDUI'] button#mat-button-toggle-12-button"));
                uiRenewalDUIYes.Click();

                // select 'Yes'
                // 3. Our records show that this establishment is licensed as a PrivateCorporation. Has this changed?
                NgWebElement uiRenewalBusinessTypeYes = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalBusinessType'] button#mat-button-toggle-14-button"));
                uiRenewalBusinessTypeYes.Click();

                // select 'Yes'
                // 4. Have you redistributed any shares within the past 12 months without notifying LCRB?
                NgWebElement uiRenewalShareholdersYes = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalShareholders'] button#mat-button-toggle-16-button"));
                uiRenewalShareholdersYes.Click();

                // select 'Yes'
                // 5. Have you entered into an agreement allowing another person or business to use your licence within the past 12 months?
                NgWebElement uiRenewalThirdPartyYes = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalThirdParty'] button#mat-button-toggle-18-button"));
                uiRenewalThirdPartyYes.Click();

                // select 'Yes'
                // 6. Have you made any unreported structural changes to your establishment within the past 12 months?
                NgWebElement uiRenewalFloorPlanYes = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalFloorPlan'] button#mat-button-toggle-20-button"));
                uiRenewalFloorPlanYes.Click();

                // select 'Yes'
                // 7. Have you acquired a new interest or expanded an existing interest financial or otherwise in a winery, brewery, distillery, liquor agent and/or a UBrew/UVin within the past 12 months without notifying LCRB?
                NgWebElement uiRenewalTiedhouseYes = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalTiedhouse'] button#mat-button-toggle-22-button"));
                uiRenewalTiedhouseYes.Click();

                // select 'Yes'
                // 8. Have you sold the business associated with this liquor licence within the last 12 months without notifying LCRB?
                NgWebElement uiRenewalUnreportedSaleOfBusinessYes = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalUnreportedSaleOfBusiness'] button#mat-button-toggle-24-button"));
                uiRenewalUnreportedSaleOfBusinessYes.Click();

                // select 'Yes'
                // 9.Have you sold the property or transferred the lease associated with this liquor licence within the last 12 months?
                NgWebElement uiRenewalValidInterestYes = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalValidInterest'] button#mat-button-toggle-26-button"));
                uiRenewalValidInterestYes.Click();

                // select 'Yes'
                // 10. Have you added, changed or removed a licensee representative within the past 12 months?
                NgWebElement uiRenewalKeyPersonnelYes = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalkeypersonnel'] button#mat-button-toggle-28-button"));
                uiRenewalKeyPersonnelYes.Click();

                // confirm that correct information re positive responses for a Catering licensing renewal is displayed
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Update your Shareholder Information on the Organization Details page')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Submit a Third Party Operator application from the Licences Dashboard')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Submit a Structural Change from the Licences Dashboard')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Update your Financial Interest information on the Account Profile')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Transfer this Licence from the Licences Dashboard')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Add, Remove or Update your Licensee Representative from the Licences Dashboard')]")).Displayed);
            }

            if ((responses == "negative responses for a brewery") || (responses == "negative responses for a winery") || (responses == "negative responses for a distillery") || (responses == "negative responses for a co-packer"))
            { 
                if (responses == "negative responses for a winery")
                {
                    // click on manufacturer minimum checkbox
                    NgWebElement uiIsManufacturedMinimum = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isManufacturedMinimum']"));
                    uiIsManufacturedMinimum.Click();
                }

                // select 'No'
                // 1. Have you or any partner, shareholder, director, or officer of this licensee been arrested for, charged with, or convicted of a criminal offence within the past 12 months that you have not reported to the LCRB?
                NgWebElement uiRenewalCriminalOffenceCheckNo = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalCriminalOffenceCheck'] button#mat-button-toggle-11-button"));
                uiRenewalCriminalOffenceCheckNo.Click();

                // select 'No'
                // 2. Have you or any of your partners, shareholders or directors of this establishment received any alcohol related driving infractions in the past 12 months?
                NgWebElement uiRenewalDUINo = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalDUI'] button#mat-button-toggle-13-button"));
                uiRenewalDUINo.Click();

                // select 'No'
                // 3. Our records show that this establishment is licensed as a PrivateCorporation. Has this changed?
                NgWebElement uiRenewalBusinessTypeNo = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalBusinessType'] button#mat-button-toggle-15-button"));
                uiRenewalBusinessTypeNo.Click();

                // select 'No'
                // 4. Have you redistributed any shares within the past 12 months without notifying LCRB?
                NgWebElement uiRenewalShareholdersNo = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalShareholders'] button#mat-button-toggle-17-button"));
                uiRenewalShareholdersNo.Click();

                // select 'No'
                // 5. Have you entered into an agreement allowing another person or business to use your licence within the past 12 months?
                NgWebElement uiRenewalThirdPartyNo = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalThirdParty'] button#mat-button-toggle-19-button"));
                uiRenewalThirdPartyNo.Click();

                // select 'No'
                // 6. Have you made any unreported structural changes to your establishment within the past 12 months?
                NgWebElement uiRenewalFloorPlanNo = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalFloorPlan'] button#mat-button-toggle-21-button"));
                uiRenewalFloorPlanNo.Click();

                // select 'No'
                // 7. Have you acquired a new interest or expanded an existing interest financial or otherwise in a winery, brewery, distillery, liquor agent and/or a UBrew/UVin within the past 12 months without notifying LCRB?
                NgWebElement uiRenewalTiedhouseNo = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalTiedhouse'] button#mat-button-toggle-23-button"));
                uiRenewalTiedhouseNo.Click();

                // select 'No'
                // 8. Have you sold the business associated with this liquor licence within the last 12 months without notifying LCRB?
                NgWebElement uiRenewalUnreportedSaleOfBusinessNo = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalUnreportedSaleOfBusiness'] button#mat-button-toggle-25-button"));
                uiRenewalUnreportedSaleOfBusinessNo.Click();

                // select 'No'
                // 9.Have you sold the property or transferred the lease associated with this liquor licence within the last 12 months?
                NgWebElement uiRenewalValidInterestNo = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalValidInterest'] button#mat-button-toggle-27-button"));
                uiRenewalValidInterestNo.Click();

                // select 'No'
                // 10. Have you added, changed or removed a licensee representative within the past 12 months?
                NgWebElement uiRenewalKeyPersonnelNo = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalkeypersonnel'] button#mat-button-toggle-29-button"));
                uiRenewalKeyPersonnelNo.Click();
            }

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            NgWebElement uiSubmitAndPay = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
            uiSubmitAndPay.Click();

            MakePayment();

            // click on Licences tab
            ClickLicencesTab();

            // reload Licences page
            ClickLicencesTab();

            // check that the licence is now active after renewal
            Assert.True(ngDriver.FindElement(By.XPath("//app-licence-row/div/div/form/table/tr[2]/td[2]/span[3][contains(.,'Active')]")).Displayed);

            // confirm that Renew Licence messages are no longer displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'Renew Licence'))]")).Displayed);

            // confirm that Reinstate Licence messages are no longer displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'Reinstate Licence'))]")).Displayed);
        }
    }
}