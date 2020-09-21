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
        [And(@"the org structure data is present for a(.*)")]
        public void ReviewOrganizationStructureData(string businessType)
        {
            if (businessType == " private corporation")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader0First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader0Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'CTOLeader0')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'leader0@privatecorp.com')]")).Displayed);

                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndyShareholder0First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndyShareholder0Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'1001')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'1002')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'individualshareholder0@privatecorp.com')]")).Displayed);

                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Business Shareholder 1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'1003')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'1004')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'business@shareholder1.com')]")).Displayed);

                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1BizFirst')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1BizLast')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1BizTitle')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'leader1bizshareholder@privatecorp.com')]")).Displayed);

                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder1Biz1First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder1Biz1Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'1005')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'1006')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'individualshareholder1bizshareholder@privatecorp.com')]")).Displayed);
            }

            if (businessType == " sole proprietorship")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'leader1@soleproprietor.com')]")).Displayed);

                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader2First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader2Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'leader2@soleproprietor.com')]")).Displayed);

                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader3First')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader3Last')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'leader3@soleproprietor.com')]")).Displayed);
            }

            if (businessType == " society")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);
            }

            if (businessType == " public corporation")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);
            }

            if (businessType == " partnership")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);
            }

            if (businessType == "n indigenous nation")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);
            }

            if (businessType == " local government")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);
            }

            if (businessType == " university")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);
            }
        }
    }
}
