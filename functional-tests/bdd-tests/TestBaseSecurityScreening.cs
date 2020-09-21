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
        [And(@"I review the security screening requirements for (.*)")]
        public void ReviewSecurityScreeningRequirements(string businessType)
        {
            /* 
            Page Title: Security Screening Requirements
            */

            // confirm that private corporation personnel are present
            if (businessType == "a private corporation")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader0First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader0Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndyShareholder0First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndyShareholder0Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1BizFirst')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1BizLast')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder1Biz1First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder1Biz1Last')]")).Displayed);
            }

            // confirm that sole proprietor personnel are present
            if (businessType == "a sole proprietorship")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader2First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader2Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader3First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader3Last')]")).Displayed);
            }

            // confirm that society personnel are present
            if (businessType == "a society")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Director1First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Director1Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Director2First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Director2Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Director3First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Director3Last')]")).Displayed);
            }

            // confirm that public corporation personnel are present
            if (businessType == "a public corporation")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1FirstPubCorp')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1LastPubCorp')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader2FirstPubCorp')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader2LastPubCorp')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader3FirstPubCorp')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader3LastPubCorp')]")).Displayed);
            }

            // confirm that partnership personnel are present
            if (businessType == "a partnership")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualPartner1First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualPartner1Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Business Partner')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualPartner2First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualPartner2Last')]")).Displayed);
            }

            // confirm that university personnel are present
            if (businessType == "a university")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader0FirstUniversity')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader0LastUniversity')]")).Displayed);
            }

            // confirm that private corporation personnel are present for a multilevel business
            if (businessType == "a multilevel business")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder0')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader0')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholderBiz2First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'LeaderBiz2First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholderBiz3First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'LeaderBiz3First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholderBiz4First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'LeaderBiz4First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholderBiz5First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'LeaderBiz5First')]")).Displayed);
            }

            // confirm that mixed business shareholder personnel are present
            if (businessType == "mixed business shareholders")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader0')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder0')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'LeaderPubCorp')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'LeaderSoleProp')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'DirectorSociety')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'TrusteeTrust')]")).Displayed);
            }

            // confirm that mixed business shareholder personnel are present after save for later button clicked
            if (businessType == "saved for later mixed business shareholder")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader0')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder0')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Business Shareholder 1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Public Corporation')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'LeaderPubCorp')]")).Displayed);
            }
        }
    }
}
