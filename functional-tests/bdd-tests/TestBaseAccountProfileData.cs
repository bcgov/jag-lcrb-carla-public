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
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'012345678')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'645 Tyee Road')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'West of Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'V9A6X5')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'P.O. Box 123')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'303 Prideaux St.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'B.C.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Canada')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(250) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'test@automation.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'https://www.liquorpolicy.org')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'AT')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'CEO')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(778) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'automated@test.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name and details of federal producer (automated test) for IN.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Details of the financial interest (automated test).')]")).Displayed);
            }

            if (bizType == " partnership account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'012345678')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'645 Tyee Road')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'West of Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'V9A6X5')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'P.O. Box 123')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'303 Prideaux St.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'B.C.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Canada')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(250) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'test@automation.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'https://www.liquorpolicy.org')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'AT')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'CEO')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(778) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'automated@test.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name and details of federal producer (automated test) for IN.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Details of the financial interest (automated test).')]")).Displayed);
            }

            if (bizType == " public corporation account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'012345678')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'645 Tyee Road')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'West of Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'V9A6X5')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'P.O. Box 123')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'303 Prideaux St.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'B.C.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Canada')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(250) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'test@automation.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'https://www.liquorpolicy.org')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'AT')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'CEO')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(778) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'automated@test.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name and details of federal producer (automated test) for IN.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Details of the financial interest (automated test).')]")).Displayed);
            }

            if (bizType == " society account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'012345678')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'645 Tyee Road')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'West of Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'V9A6X5')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'P.O. Box 123')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'303 Prideaux St.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'B.C.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Canada')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(250) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'test@automation.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'https://www.liquorpolicy.org')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'AT')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'CEO')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(778) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'automated@test.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name and details of federal producer (automated test) for IN.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Details of the financial interest (automated test).')]")).Displayed);
            }

            if (bizType == " sole proprietorship account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'012345678')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'645 Tyee Road')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'West of Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'V9A6X5')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'P.O. Box 123')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'303 Prideaux St.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'B.C.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Canada')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(250) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'test@automation.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'https://www.liquorpolicy.org')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'AT')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'CEO')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(778) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'automated@test.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name and details of federal producer (automated test) for IN.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Details of the financial interest (automated test).')]")).Displayed);
            }

            if (bizType == "n indigenous nation account profile")
            {
                NgWebElement uiEventContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='businessNumber']"));
                Assert.True(uiEventContactName.GetAttribute("value") == "012345678");

                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'012345678')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'645 Tyee Road')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'West of Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'V9A6X5')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'P.O. Box 123')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'303 Prideaux St.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'B.C.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Canada')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(250) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'test@automation.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'https://www.liquorpolicy.org')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'AT')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'CEO')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(778) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'automated@test.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name and details of federal producer (automated test) for IN.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Details of the financial interest (automated test).')]")).Displayed);
            }

            if (bizType == " local government account profile")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'012345678')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'645 Tyee Road')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'West of Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Victoria')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'V9A6X5')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'P.O. Box 123')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'303 Prideaux St.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'B.C.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Canada')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(250) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'test@automation.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'https://www.liquorpolicy.org')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'AT')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'CEO')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(778) 181-1818')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'automated@test.com')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name and details of federal producer (automated test) for IN.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Details of the financial interest (automated test).')]")).Displayed);
            }
        }
    }
}
