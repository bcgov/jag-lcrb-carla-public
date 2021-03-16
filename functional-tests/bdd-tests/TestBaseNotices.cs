using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"there are no notices attached to the account")]
        public void NoNotices()
        {
            Assert.True(ngDriver
                .FindElement(By.XPath("//body[contains(.,'There are currently no notices attached to this account.')]"))
                .Displayed);
        }
    }
}