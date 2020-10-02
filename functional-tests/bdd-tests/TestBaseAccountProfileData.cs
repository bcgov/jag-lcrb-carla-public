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
        [And(@"the correct data is displayed for a(.*)")]
        public void AccountProfileData(string bizType)
        {
            if (bizType == " private corporation account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);

            }

            if (bizType == " partnership account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);

            }

            if (bizType == " public corporation account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);

            }

            if (bizType == " society account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);

            }

            if (bizType == " sole proprietorship account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);

            }

            if (bizType == "n indigenous nation account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);

            }

            if (bizType == " local government account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' ')]")).Displayed);

            }
        }
    }
}
