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
        [And(@"I confirm the terms and conditions for (.*)")]
        public void TermsAndConditions(string licenceType)
        {
            /* 
            Page Title: Licences & Authorizations
            */

            // click on the Terms and Conditions element
            NgWebElement uiTermsAndConditions = ngDriver.FindElement(By.CssSelector("mat-expansion-panel-header#mat-expansion-panel-header-0"));
            uiTermsAndConditions.Click();

            // confirm the Terms and Conditions by licence type
            if (licenceType == "an agent licence")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'The terms and conditions to which this licence is subject include the terms and conditions contained in the licensee Terms and Conditions Handbook, which is available on the Liquor and Cannabis Regulation Branch website. The Terms and Conditions Handbook is amended from time to time.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Pursuant to Section 39, Liquor Control and Licensing Regulation, this licence allows the above named person or company to act as an agent of a liquor importer or as a liquor importer.')]")).Displayed);
            }

            if (licenceType == "a Catering licence")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' For sale and service of liquor at another person's event where food service is catered by the licensee, unless otherwise permitted. ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' The terms and conditions to which this licence is subject include the terms and conditions contained in the licensee Terms and Conditions Handbook, which is available on the Liquor and Cannabis Regulation Branch website. The Terms and Conditions Handbook is amended from time to time. ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Licensee may only serve liquor at a catered event for which LCRB has issued a catering authorization. ')]")).Displayed);
            }

            if (licenceType == "a CRS licence")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'This licence is subject to the terms and conditions specified in the restriction or approval letter(s) and those contained in the Cannabis Retail Store Handbook, which may be amended from time to time.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Packaged cannabis may only be sold within the service area outlined in blue on the LCRB approved floor plan, unless otherwise endorsed or approved by the LCRB.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'The establishment may be open anytime between the hours of 9 a.m. and 11 p.m., subject to further restriction by the local government or Indigenous nation.')]")).Displayed);
            }

            if (licenceType == "a Cannabis marketing licence")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'This licence is subject to the terms and conditions specified in the restriction or approval letter(s) and those contained in the Marketing Licence Handbook, which may be amended from time to time.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Pursuant to Section 11, Cannabis Licensing Regulation, this licence allows the above named person or company to act as a marketing licensee to promote cannabis for the purpose of selling it.')]")).Displayed);
            }

            if (licenceType == "a Food Primary licence")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'')]")).Displayed);
            }
        }
    }
}
