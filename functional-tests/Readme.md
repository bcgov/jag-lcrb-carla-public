# Functional Tests #

This folder contains the source for functional system tests.  These tests use the following frameworks to test the system:

1. Easy Repro - this is a high level framework that utilizes Selenium and is specifically designed to test Microsoft Dynamics 365.  Easy Repro is an Open Source system located at https://github.com/microsoft/EasyRepro
2. xUnit.net - the test runner for the automated tests.  xUnit.net is compatible with Visual Studio as well as other test environments such as DevOps Spaces. More information is located at https://xunit.net/
3. Dotnet Core 2.2 - some tests are written purely in Dotnet Core.  More information is located at https://github.com/dotnet/core
4. Protractor - used for front end Andgular tests.  Protractor uses Jasmine and Selenium and is a Node.js based test system. More information is available at https://www.protractortest.org

# Environment #

Currently the Microsoft Dynamics 365 tests are intended to be run in a Windows environment.  You may use a Windows Container or Windows Core installation to run the test, and it is also compatible with a type Windows Desktop operating system such as Windows 10 Professional.  It is expected that you would have a browser installed in this environment, and currently the tests are indented for use with Google Chrome.

# Custom HTML Dialogs #

Easy Repro is able to handle most Out of the Box (OOTB) elements of Dynamics.  It does not handle custom HTML dialogs, and you will need to write specific tests for these using Selenium syntax.

Custom HTML Dialogs in Dynamics are rendered as iFrames on the web page, so first switch the Selenium driver to the iFrame:  

		//Switch to the Inline Dialog frame
    	XrmTestBrowser.Driver.SwitchTo().Frame("InlineDialog_Iframe");

To help with troubleshooting custom HTML Dialogs it can be useful to install a tool to permit use of the Browser Inspect option from within Microsoft Dynamics.  This allows you to click on a text field, then go to the Inspect feature of the browser, and thus be able to view the current Elements of the web page.  One example of this sort of tool is the "Enable Right Click" extension, available at https://chrome.google.com/webstore/detail/enable-right-click.  Note that with MS Dynamics you still need to left click inside a text field before you can right click and go to Inspect.

An example of the Selenium syntax is:

		// Change the text field.
        IWebElement userOrTeamText = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"systemuserview_id_ledit\"]"));            
        userOrTeamText.SendKeys(configuration["APPLICATION_ASSIGNEE"]);

		// click the search button.
        IWebElement searchButton = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"systemuserview_id_lookupSearch\"]"));

Note that the usual Selenium rules apply, for example you can't click on an element that is not visible.  Note that MS Dynamics custom HTML dialogs often contain clickable divs that contain the invisible actual element such as a select dropdown or text field; click on the div rather than the hidden element.

Be sure to switch back to the main frame when you are done testing the Custom HTML Dialog.


			// switch back to the main frame.
            XrmTestBrowser.Driver.SwitchTo().ParentFrame();


