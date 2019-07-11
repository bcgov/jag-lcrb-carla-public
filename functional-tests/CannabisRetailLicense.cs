using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using Xunit;

namespace CllcSpiceSyncServiceTest
{
    public class CannabisRetailLicenceTests : TestBase
    {
        [Fact]
        public void ApproveCRL()
        {
            XrmTestBrowser.ThinkTime(1000);
            // 3. Go to Sales/Accounts using the Sitemap
            XrmTestBrowser.Navigation.OpenSubArea("Licensing", "Applications");

            XrmTestBrowser.ThinkTime(500);
            // 4. Change the active view
            XrmTestBrowser.Grid.Sort("Created On");
            XrmTestBrowser.ThinkTime(20);
            XrmTestBrowser.Grid.Sort("Created On");

            XrmTestBrowser.ThinkTime(500);

            XrmTestBrowser.Grid.OpenRecord(0);

            XrmTestBrowser.ThinkTime(500);

            XrmTestBrowser.CommandBar.ClickCommand("Assign");

            // The default view of the assign bar is assign it to me.

            //Switch to the Inline Dialog frame
            XrmTestBrowser.Driver.SwitchTo().Frame("InlineDialog_Iframe");


            

            
            // Set the "Assign To" field to "1" - User or Team
            SelectElement assignToSelect = new SelectElement(XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"rdoMe_id_i\"]")));

            var option = assignToSelect.SelectedOption;
            IWebElement temp = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"rdoMe_id\"]"));
            if (temp.Text == "Me")
            {
                temp.Click();
            }

            XrmTestBrowser.ThinkTime(500);
            IWebElement userOrTeamDiv = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"systemuserview_id_lookupValue\"]"));
            userOrTeamDiv.Click();

            // Change the text field.
            IWebElement userOrTeamText = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"systemuserview_id_ledit\"]"));            
            userOrTeamText.SendKeys(configuration["APPLICATION_ASSIGNEE"]);

            // click the search button.
            IWebElement searchButton = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"systemuserview_id_lookupSearch\"]"));

            searchButton.Click();

            // find the first item and click on it.
            IWebElement firstItem = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"item0\"]"));
            firstItem.Click();
            

            // click the OK button.
            IWebElement okButton = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"ok_id\"]"));

            okButton.Click();


            XrmTestBrowser.ThinkTime(2000);

            // switch back to the main frame.

            XrmTestBrowser.Driver.SwitchTo().ParentFrame();



        }
    }
}