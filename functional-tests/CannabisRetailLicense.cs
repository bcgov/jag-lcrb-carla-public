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

        private void SetChevronItem(string id, string value)
        {

            var optionset = new OptionSet() { Name = id, Value = value };
            XrmTestBrowser.Entity.SetValue(optionset );
            return;

            SelectElement selectElement = new SelectElement(XrmTestBrowser.Driver.FindElement(By.XPath($"//*[@id=\"{id}_i\"]")));

            if (selectElement.WrappedElement.Displayed == false)
            {
                try
                {
                    XrmTestBrowser.Driver.FindElement(By.XPath($"//*[@id=\"{id}\"]")).Click();
                }
                catch (ElementClickInterceptedException)
                {

                }

            }


            XrmTestBrowser.ThinkTime(100);            

            selectElement.SelectByText(value);

        }

        [Fact]
        public void ApproveCRL()
        {
            string currentWindow = XrmTestBrowser.Driver.CurrentWindowHandle;
            

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

            XrmTestBrowser.ThinkTime(1000);

            // click the OK button.
            try
            {
                XrmTestBrowser.Driver.FindElement(By.XPath("//button[@id=\"ok_id\"]")).Click();
            }
            catch (StaleElementReferenceException)
            { // ignore the stale element, we have moved on. 
            }
            

            
            //XrmTestBrowser.ThinkTime(2000);

            // switch back to the main frame.

            XrmTestBrowser.Driver.SwitchTo().ParentFrame();

            
            
            
            XrmTestBrowser.ThinkTime(500);

            // now we need to go through the chevrons.

            XrmTestBrowser.Driver.SwitchTo().Frame("contentIFrame0");

            SetChevronItem("header_process_adoxio_appchecklistpaymentreceived", "Yes");
            
            SetChevronItem("header_process_adoxio_checklistverifybusinessprofile", "Yes");
            
            SetChevronItem("header_process_adoxio_checklistverifyapplication", "Yes");
            
            SetChevronItem("header_process_adoxio_checklistverifypostalcode", "Yes");
            
            SetChevronItem("header_process_adoxio_checklistsitemapreceived", "Yes");
             
            SetChevronItem("header_process_adoxio_checklistfloorplanreceived", "Yes");

            SetChevronItem("header_process_adoxio_checklistassociateformreceived", "Yes");

            SetChevronItem("header_process_adoxio_checklistvalidinterestreceived", "Yes");

            SetChevronItem("header_process_adoxio_checklistlebuilt", "Yes");

            SetChevronItem("header_process_adoxio_checklistspdconsentreceived", "Yes");

            SetChevronItem("processStep_adoxio_checklistfinintegrityreceived", "Yes");

            XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"stageNavigateActionContainer\"]")).Click();

            



            // save the entity.

            IWebElement saveButton = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"footer_statuscontrol\"]/div[2]/a"));
            saveButton.Click();

            XrmTestBrowser.Driver.SwitchTo().Window(currentWindow); // switch back to main frame
        }
    }
}