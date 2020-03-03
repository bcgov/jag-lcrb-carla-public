using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using Xunit;

/*
Feature: CRSApplication
    As a logged in business user
    I want to submit a CRS Application

Scenario: Start Application
    Given I am logged in to the dashboard
    And I click on the Start Application button
    And I click on the Continue to Application button
    And I complete the application
    And I click on the Submit & Pay button
    And I enter the payment information
    And I return to the dashboard
    And I delete my account
    Then I see login
*/

namespace bdd_tests
{
    [FeatureFile("./CRSApplication.feature")]
    public sealed class CRSApplication : TestBaseCRS
    {

        // Dashboard related common actions

        [Given(@"I am logged in to the dashboard")]
        public void I_view_the_dashboard()
        {
            CarlaLogin();
        }

        [And(@"I am not a marketer")]
        public void check_marketer()
        {
        }

        [And(@"I click on the Start Application button")]
        public void I_start_application()
        {
            ngDriver.WaitForAngular();

            //System.Threading.Thread.Sleep(7000);
            
            NgWebElement startApp_button = ngDriver.FindElement(By.XPath("//button[text()='START APPLICATION']"));
            startApp_button.Click();

            ngDriver.WaitForAngular();

            string bizNumber = "012345678";
            string streetAddress = "645 Tyee Road";
            string city = "Victoria";
            string postalCode = "V8V4Y3";
            string bizPhoneNumber = "2501811818";
            string bizEmail = "test@automation.com";
            string corpContactPhone = "7781811818";
            string corpContactEmail = "automated@test.com";

            NgWebElement uiBizNumber = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-account-profile/div/div[2]/div[1]/div/div/div[1]/app-field[3]/section/div[1]/section/input"));
            uiBizNumber.SendKeys(bizNumber);

            NgWebElement uiStreetAddress = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-account-profile/div/div[2]/div[1]/div/div/div[2]/div[1]/app-field[1]/section/div[1]/section/input"));
            uiStreetAddress.SendKeys(streetAddress);

            NgWebElement uiCity = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-account-profile/div/div[2]/div[1]/div/div/div[2]/div[1]/app-field[3]/section/div[1]/section/input"));
            uiCity.SendKeys(city);

            NgWebElement uiPostalCode = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-account-profile/div/div[2]/div[1]/div/div/div[2]/div[1]/section[2]/app-field/section/div[1]/section/input"));
            uiPostalCode.SendKeys(postalCode);

            NgWebElement uiSameAsMailingAddress = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-account-profile/div/div[2]/div[1]/div/div/div[2]/div[2]/section/input"));
            uiSameAsMailingAddress.Click();

            NgWebElement uiBizPhoneNumber = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-account-profile/div/div[2]/div[1]/div/div/div[2]/app-field[1]/section/div[1]/section/input"));
            uiBizPhoneNumber.SendKeys(bizPhoneNumber);

            NgWebElement uiBizEmail = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-account-profile/div/div[2]/div[1]/div/div/div[2]/app-field[2]/section/div[1]/section/input"));
            uiBizEmail.SendKeys(bizEmail);

            NgWebElement uiCorpContactPhone = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-account-profile/div/div[2]/div[1]/div/div/div[3]/app-field[4]/section/div[1]/section/input"));
            uiCorpContactPhone.SendKeys(corpContactPhone);

            NgWebElement uiCorpContactEmail = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-account-profile/div/div[2]/div[1]/div/div/div[3]/app-field[5]/section/div[1]/section/input"));
            uiCorpContactEmail.SendKeys(corpContactEmail);

            NgWebElement corpConnectionFederalProducer = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-account-profile/div/div[2]/div[1]/div/div/div[4]/app-connection-to-producers/div[2]/div/section[1]/input[2]"));
            corpConnectionFederalProducer.Click();

            NgWebElement federalProducerConnectionToCorp = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-account-profile/div/div[2]/div[1]/div/div/div[4]/app-connection-to-producers/div[2]/div/section[2]/input[2]"));
            federalProducerConnectionToCorp.Click();

            NgWebElement continueApp_button = ngDriver.FindElement(By.Id("continueToApp"));
            continueApp_button.Click();

            ngDriver.WaitForAngular();
        }

        [And(@"I click on the Continue to Application button")]
        public void I_continue_to_application()
        {
            ngDriver.WaitForAngular();

            string electricSignature = "Automated Test";

            NgWebElement noRadio1 = ngDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div/mat-dialog-container/app-eligibility-form/div/form/div[3]/section/mat-radio-group/mat-radio-button[2]"));
            noRadio1.Click();

            NgWebElement noRadio2 = ngDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div/mat-dialog-container/app-eligibility-form/div/form/div[4]/section/mat-radio-group/mat-radio-button[2]"));
            noRadio2.Click();

            NgWebElement matCheckbox = ngDriver.FindElement(By.XPath("//mat-checkbox[@id='mat-checkbox-1']/label/div"));
            matCheckbox.Click();

            NgWebElement sigCheckbox = ngDriver.FindElement(By.Id("eligibilitySignature"));
            sigCheckbox.SendKeys(electricSignature);

            NgWebElement submit_button = ngDriver.FindElement(By.XPath("//button[text()='SUBMIT']"));
            submit_button.Click();

            ngDriver.WaitForAngular();
        }

        [And(@"I complete the application")]
        public void I_complete_the_application()
        {
            string estName = "Point Ellis Greenhouse";
            string estAddress = "645 Tyee Rd";
            string estCity = "Victoria";
            string estPostal = "V9A6X5";
            string estPID = "012345678";
            string estEmail = "test@test.com";
            string estPhone = "2505555555";
            string conGiven = "Given";
            string conSurname = "Surname";
            string conRole = "CEO";
            string conPhone = "2508888888";
            string conEmail = "contact@email.com";

            NgWebElement estabName = ngDriver.FindElement(By.Id("establishmentName"));
            estabName.SendKeys(estName);

            NgWebElement estabAddress = ngDriver.FindElement(By.Id("establishmentAddressStreet"));
            estabAddress.SendKeys(estAddress);

            NgWebElement estabCity = ngDriver.FindElement(By.Id("establishmentAddressCity"));
            estabCity.SendKeys(estCity);

            NgWebElement estabPostal = ngDriver.FindElement(By.Id("establishmentAddressPostalCode"));
            estabPostal.SendKeys(estPostal);

            NgWebElement estabPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
            estabPID.SendKeys(estPID);

            NgWebElement estabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            estabEmail.SendKeys(estEmail);

            NgWebElement estabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            estabPhone.SendKeys(estPhone);

            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSignage.SendKeys(signagePath);

            string validInterestPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "valid_interest.pdf");
            NgWebElement uploadValidInterest = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
            uploadValidInterest.SendKeys(validInterestPath);

            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uploadFloorplan.SendKeys(floorplanPath);

            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
            uploadSitePlan.SendKeys(sitePlanPath);

            string finIntegrityPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "fin_integrity.pdf");
            NgWebElement uploadFinIntegrity = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uploadFinIntegrity.SendKeys(finIntegrityPath);

            NgWebElement contactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            contactGiven.SendKeys(conGiven);

            NgWebElement contactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            contactSurname.SendKeys(conSurname);

            NgWebElement contactRole = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonRole]"));
            contactRole.SendKeys(conRole);

            NgWebElement contactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonPhone]"));
            contactPhone.SendKeys(conPhone);

            NgWebElement contactEmail = ngDriver.FindElement(By.Id("contactPersonEmail"));
            contactEmail.SendKeys(conEmail);

            NgWebElement authorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            authorizedSubmit.Click();

            NgWebElement signatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            signatureAgree.Click();
        }

        [And(@"I click on the Submit & Pay button")]
        public void click_on_submit_and_pay()
        {
            NgWebElement submitpay_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT & PAY')]"));
            System.Threading.Thread.Sleep(7000);

            submitpay_button.Click();
            System.Threading.Thread.Sleep(7000);
        }

        [Then(@"I CLICK on 'SAVE FOR LATER'")]
        public void click_on_save_for_later()
        {
            NgWebElement saveforlater_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SAVE FOR LATER')]"));
            saveforlater_button.Click();
        }

        [And(@"I enter the payment information")]
        public void enter_payment_info()
        {
            MakeCRSPayment();
        }

        [And(@"I return to the dashboard")]
        public void return_to_dashboard()
        {
            string retDash = "Return to Dashboard";
            NgWebElement returnDash = ngDriver.FindElement(By.LinkText(retDash));
            returnDash.Click();
        }

        [And(@"I delete my account")]
        public void Delete_my_account()
        {
            this.CarlaDeleteCurrentAccount();
        }

        [Then(@"I see login")]
        public void I_see_login()
        {
            Assert.True (ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }
    }
}
