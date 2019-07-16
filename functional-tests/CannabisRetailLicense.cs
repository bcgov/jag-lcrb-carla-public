using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using Xunit;

namespace FunctionalTest
{
    public class CannabisRetailLicenceTests : TestBase
    {

        

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

            JavaScriptClick("stage_0");

            XrmTestBrowser.Driver.SwitchTo().ParentFrame();

            SetOptionSet("header_process_adoxio_appchecklistpaymentreceived", "Yes");
            
            SetOptionSet("header_process_adoxio_checklistverifybusinessprofile", "Yes");
            
            SetOptionSet("header_process_adoxio_checklistverifyapplication", "Yes");
            
            SetOptionSet("header_process_adoxio_checklistverifypostalcode", "Yes");
            
            SetOptionSet("header_process_adoxio_checklistsitemapreceived", "Yes");
             
            SetOptionSet("header_process_adoxio_checklistfloorplanreceived", "Yes");            

            SetOptionSet("header_process_adoxio_checklistphotosreceived", "Yes");

            SetOptionSet("header_process_adoxio_checklistassociateformreceived", "Yes");
            
            SetOptionSet("header_process_adoxio_checklistvalidinterestreceived", "Yes");

            SetOptionSet("header_process_adoxio_checklistlebuilt", "Yes");

            SetOptionSet("header_process_adoxio_checklistspdconsentreceived", "Yes");

            SetOptionSet("header_process_adoxio_checklistfinintegrityreceived", "Yes");

            var nextButton = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"stageNavigateActionContainer\"]"));
            if (nextButton.Displayed && nextButton.Enabled)
            {
                nextButton.Click();
            }
            //XrmTestBrowser.Driver.SwitchTo().Frame("contentIFrame0");

            JavaScriptClick("stage_1");

            //XrmTestBrowser.Driver.SwitchTo().ParentFrame();
            
            SetOptionSet("header_process_adoxio_checklistsenttolgin", "Sent");
            SetOptionSet("header_process_adoxio_checklistmarketcapacity", "Yes");
            SetOptionSet("header_process_adoxio_checklistsentforfisla", "Yes");

            SetOptionSet("header_process_adoxio_checklistsenttospd", "Yes");
            SetOptionSet("header_process_adoxio_checklisttiedhouseassessed", "T&C Added");
            SetOptionSet("header_process_adoxio_checklistbrandingassessed", "Meets Requirements");

            SetOptionSet("header_process_adoxio_checklistphotosassessed", "Meets Requirements");
            SetOptionSet("header_process_adoxio_checklistsitemapassessed", "Meets Requirements");
            SetOptionSet("header_process_adoxio_checklistfloorplanassessed", "Meets Requirements");

            SetOptionSet("header_process_adoxio_checklistvalidinterestassessed", "Owned");
            SetOptionSet("header_process_adoxio_checklistlicencehistorycheck", "Yes");

            // save the entity.

            nextButton = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"stageNavigateActionContainer\"]"));
            if (nextButton.Displayed && nextButton.Enabled)
            {
                nextButton.Click();
            }
            //XrmTestBrowser.Driver.SwitchTo().Frame("contentIFrame0");

            JavaScriptClick("stage_2");


            IWebElement saveButton = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"footer_statuscontrol\"]/div[2]/a"));
            saveButton.Click();



            XrmTestBrowser.Driver.SwitchTo().Window(currentWindow); // switch back to main frame
        }
    }
}