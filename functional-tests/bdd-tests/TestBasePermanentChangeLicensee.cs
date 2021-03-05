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
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the Permanent Change to a Licensee application for a (.*)")]
        public void PermanentChangeLicensee(string appType)
        {
            /* 
            Page Title: Permanent Change to a Licensee
            */

            // create test data
            string firstName = "Firstname";
            string lastName = "Lastname";
            string newFirstName = "Newfirstname";
            string newLastName = "Newlastname";
            string societyName = "Societyname";
            string newSocietyName = "Newsocietyname";
            string partnershipName = "Partnershipname";
            string newPartnershipName = "Newpartnershipname";
            string companyName = "Companyname";
            string newCompanyName = "Newcompanyname";
            string executorFirstName = "Executorfirstname";
            string executorLastName = "Executorlastname";
            string receiverFirstName = "Receiverfirstname";
            string receiverLastName = "Receiverlastname";

            switch (appType)
            {
                case "society":

                    /* 
                    *  TYPES OF CHANGES REQUESTED
                    */

                    // click on Change of Directors or Officers
                    NgWebElement uiChangeOfDirectorsOrOfficers = ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiChangeOfDirectorsOrOfficers.Click();
                    
                    // click on Name Change, Licensee -- Society
                    NgWebElement uiNameChangeLicenseeSociety = ngDriver.FindElement(By.CssSelector("#mat-checkbox-4.mat-checkbox"));
                    uiNameChangeLicenseeSociety.Click();
                    
                    // click on Name Change, Person
                    NgWebElement uiNameChangePerson = ngDriver.FindElement(By.CssSelector("#mat-checkbox-5.mat-checkbox"));
                    uiNameChangePerson.Click();
                    
                    // click on Addition of Receiver or Executor
                    NgWebElement uiAdditionOfReceiverOrExecutor = ngDriver.FindElement(By.CssSelector("#mat-checkbox-6.mat-checkbox"));
                    uiAdditionOfReceiverOrExecutor.Click();

                    /* 
                    *  CHANGE OF DIRECTORS OR OFFICERS
                    */

                    // upload notice of articles
                    FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[3]");

                    /* 
                    *  PERSON'S NAME CHANGE
                    */

                    // enter person first name
                    NgWebElement uiFirstNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-2"));
                    uiFirstNameSociety.SendKeys(firstName);

                    // enter person last name
                    NgWebElement uiLastNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-3"));
                    uiLastNameSociety.SendKeys(lastName);

                    // enter person new first name
                    NgWebElement uiNewFirstNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-4"));
                    uiNewFirstNameSociety.SendKeys(newFirstName);

                    // enter person new last name
                    NgWebElement uiNewLastNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-5"));
                    uiNewLastNameSociety.SendKeys(newLastName);

                    // upload copy of marriage certificate
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[6]");

                    /* 
                    *  SOCIETY NAME CHANGE
                    */

                    // enter society name
                    NgWebElement uiSocietyName = ngDriver.FindElement(By.CssSelector("input#mat-input-0"));
                    uiSocietyName.SendKeys(societyName);

                    // enter society name
                    NgWebElement uiNewSocietyName = ngDriver.FindElement(By.CssSelector("input#mat-input-1"));
                    uiNewSocietyName.SendKeys(newSocietyName);

                    // upload name change certificate
                    FileUpload("certificate_of_name_change.pdf", "(//input[@type='file'])[8]");

                    /* 
                    *  ADDITION OF EXECUTOR OR RECEIVER
                    */

                    // enter executor first name
                    NgWebElement uiExecutorFirstNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-6"));
                    uiExecutorFirstNameSociety.SendKeys(executorFirstName);

                    // enter executor last name
                    NgWebElement uiExecutorLastNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-7"));
                    uiExecutorLastNameSociety.SendKeys(executorLastName);

                    // upload assignment of executor
                    FileUpload("assignment_of_executor.pdf", "(//input[@type='file'])[11]");

                    // upload death certificate
                    FileUpload("death_certificate.pdf", "(//input[@type='file'])[14]");

                    // enter receiver first name
                    NgWebElement uiReceiverFirstNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-8"));
                    uiReceiverFirstNameSociety.SendKeys(receiverFirstName);

                    // enter receiver last name
                    NgWebElement uiReceiverLastNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-9"));
                    uiReceiverLastNameSociety.SendKeys(receiverLastName);

                    // upload receiver appointment order
                    FileUpload("receiver_appointment_order.pdf", "(//input[@type='file'])[17]");

                    // upload court order
                    FileUpload("court_order.pdf", "(//input[@type='file'])[20]");

                    /* 
                    *  PERSONAL HISTORY SUMMARY FORMS
                    */

                    // upload Personal History Summary document
                    FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[24]");

                    break;

                case "private corporation":

                    /* 
                    *  TYPES OF CHANGES REQUESTED
                    */

                    // click on Internal Transfer of Shares
                    NgWebElement uiInternalTransferOfShares = ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiInternalTransferOfShares.Click();

                    // click on External Transfer of Shares
                    NgWebElement uiExternalTransferOfShares = ngDriver.FindElement(By.CssSelector("#mat-checkbox-4.mat-checkbox"));
                    uiExternalTransferOfShares.Click();

                    // click on Change of Directors or Officers
                    NgWebElement uiChangeOfDirectorsOrOfficersPrivateCorporation = ngDriver.FindElement(By.CssSelector("#mat-checkbox-5.mat-checkbox"));
                    uiChangeOfDirectorsOrOfficersPrivateCorporation.Click();
                    
                    // click on Name Change, Licensee -- Corporation
                    NgWebElement uiNameChangeLicenseePrivateCorporation = ngDriver.FindElement(By.CssSelector("#mat-checkbox-6.mat-checkbox"));
                    uiNameChangeLicenseePrivateCorporation.Click();
                    
                    // click on Name Change, Person
                    NgWebElement uiNameChangePersonPrivateCorporation = ngDriver.FindElement(By.CssSelector("#mat-checkbox-7.mat-checkbox"));
                    uiNameChangePersonPrivateCorporation.Click();
                    
                    // click on Addition of Receiver or Executor
                    NgWebElement uiAdditionOfReceiverOrExecutorPrivateCorporation = ngDriver.FindElement(By.CssSelector("#mat-checkbox-8.mat-checkbox"));
                    uiAdditionOfReceiverOrExecutorPrivateCorporation.Click();

                    /* 
                    *  INTERNAL SHARE TRANSFER
                    */

                    // upload Central Securities Register document
                    FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

                    // upload shareholders < 10% interest
                    FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[6]");

                    // click 'Yes' for amalgamation with another company
                    NgWebElement uiAmalgamationYes = ngDriver.FindElement(By.CssSelector("mat-radio-button#mat-radio-2"));
                    uiAmalgamationYes.Click();

                    // upload certificate of amalgamation
                    FileUpload("certificate_of_amalgamation.pdf", "(//input[@type='file'])[9]");

                    // upload Central Securities Register document for amalgamated company
                    FileUpload("central_securities_register.pdf", "(//input[@type='file'])[12]");

                    // upload notice of articles for amalgamated company
                    FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[15]");

                    /* 
                    *  EXTERNAL SHARE TRANSFER
                    */

                    // upload central securities register
                    FileUpload("central_securities_register.pdf", "(//input[@type='file'])[18]");

                    // upload supporting business documentation
                    FileUpload("associates.pdf", "(//input[@type='file'])[21]");

                    // upload shareholders < 10% interest
                    FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[24]");

                    /* 
                    *  CHANGE OF DIRECTORS OR OFFICERS
                    */

                    // upload notice of articles
                    FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[27]");

                    /* 
                    *  PERSON'S NAME CHANGE
                    */

                    // enter person first name
                    NgWebElement uiFirstNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-2"));
                    uiFirstNamePrivateCorporation.SendKeys(firstName);

                    // enter person last name
                    NgWebElement uiLastNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-3"));
                    uiLastNamePrivateCorporation.SendKeys(lastName);

                    // enter person new first name
                    NgWebElement uiNewFirstNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-4"));
                    uiNewFirstNamePrivateCorporation.SendKeys(newFirstName);

                    // enter person new last name
                    NgWebElement uiNewLastNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-5"));
                    uiNewLastNamePrivateCorporation.SendKeys(newLastName);

                    // upload copy of marriage certificate
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[30]");

                    /* 
                    *  CORPORATION NAME CHANGE
                    */

                    // enter company name
                    NgWebElement uiCompanyNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-0"));
                    uiCompanyNamePrivateCorporation.SendKeys(companyName);

                    // enter new company name
                    NgWebElement uiNewCompanyNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-1"));
                    uiNewCompanyNamePrivateCorporation.SendKeys(newCompanyName);

                    // upload certificate of name change
                    FileUpload("certificate_of_name_change.pdf", "(//input[@type='file'])[32]");

                    /* 
                    *  ADDITION OF EXECUTOR OR RECEIVER
                    */

                    // enter executor first name
                    NgWebElement uiExecutorFirstNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-6"));
                    uiExecutorFirstNamePrivateCorporation.SendKeys(executorFirstName);

                    // enter executor last name
                    NgWebElement uiExecutorLastNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-7"));
                    uiExecutorLastNamePrivateCorporation.SendKeys(executorLastName);

                    // upload assignment of executor
                    FileUpload("assignment_of_executor.pdf", "(//input[@type='file'])[35]");

                    // upload death certificate
                    FileUpload("death_certificate.pdf", "(//input[@type='file'])[38]");

                    // enter receiver first name
                    NgWebElement uiReceiverFirstNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-8"));
                    uiReceiverFirstNamePrivateCorporation.SendKeys(receiverFirstName);

                    // enter receiver last name
                    NgWebElement uiReceiverLastNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-9"));
                    uiReceiverLastNamePrivateCorporation.SendKeys(receiverLastName);

                    // upload receiver appointment order
                    FileUpload("receiver_appointment_order.pdf", "(//input[@type='file'])[41]");

                    // upload court order
                    FileUpload("court_order.pdf", "(//input[@type='file'])[44]");

                    /* 
                    *  PERSONAL HISTORY SUMMARY FORMS
                    */

                    // upload Personal History Summary document
                    FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[48]");

                    break;

                case "public corporation":

                    /* 
                    *  TYPES OF CHANGES REQUESTED
                    */

                    // click on Internal Transfer of Shares
                    NgWebElement uiInternalTransferOfSharesPublicCorp = ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiInternalTransferOfSharesPublicCorp.Click();

                    // click on External Transfer of Shares
                    NgWebElement uiExternalTransferOfSharesPublicCorp = ngDriver.FindElement(By.CssSelector("#mat-checkbox-4.mat-checkbox"));
                    uiExternalTransferOfSharesPublicCorp.Click();

                    // click on Change of Directors or Officers
                    NgWebElement uiChangeOfDirectorsPublicCorp = ngDriver.FindElement(By.CssSelector("#mat-checkbox-5.mat-checkbox"));
                    uiChangeOfDirectorsPublicCorp.Click();

                    // click on Name Change, Licensee -- Corporation
                    NgWebElement uiNameChangePublicCorp = ngDriver.FindElement(By.CssSelector("#mat-checkbox-6.mat-checkbox"));
                    uiNameChangePublicCorp.Click();

                    // click on Name Change, Person
                    NgWebElement uiNameChangePersonPublicCorp = ngDriver.FindElement(By.CssSelector("#mat-checkbox-7.mat-checkbox"));
                    uiNameChangePersonPublicCorp.Click();

                    // click on Addition of Receiver or Executor
                    NgWebElement uiAdditionReceiverExecutor = ngDriver.FindElement(By.CssSelector("#mat-checkbox-8.mat-checkbox"));
                    uiAdditionReceiverExecutor.Click();

                    /* 
                    *  PERSON'S NAME CHANGE
                    */

                    // enter person first name
                    NgWebElement uiFirstNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-0"));
                    uiFirstNamePublicCorporation.SendKeys(firstName);

                    // enter person last name
                    NgWebElement uiLastNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-3"));
                    uiLastNamePublicCorporation.SendKeys(lastName);

                    // enter person new first name
                    NgWebElement uiNewFirstNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-4"));
                    uiNewFirstNamePublicCorporation.SendKeys(newFirstName);

                    // enter person new last name
                    NgWebElement uiNewLastNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-5"));
                    uiNewLastNamePublicCorporation.SendKeys(newLastName);

                    // upload copy of marriage certificate
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[6]");

                    /* 
                    *  CORPORATION NAME CHANGE
                    */

                    // enter company name
                    NgWebElement uiCompanyNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-0"));
                    uiCompanyNamePublicCorporation.SendKeys(companyName);

                    // enter new company name
                    NgWebElement uiNewCompanyNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-1"));
                    uiNewCompanyNamePublicCorporation.SendKeys(newCompanyName);

                    // upload certificate of name change
                    FileUpload("certificate_of_name_change.pdf", "(//input[@type='file'])[8]");

                    /* 
                    *  ADDITION OF EXECUTOR OR RECEIVER
                    */

                    // enter executor first name
                    NgWebElement uiExecutorFirstNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-6"));
                    uiExecutorFirstNamePublicCorporation.SendKeys(executorFirstName);

                    // enter executor last name
                    NgWebElement uiExecutorLastNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-7"));
                    uiExecutorLastNamePublicCorporation.SendKeys(executorLastName);

                    // upload assignment of executor
                    FileUpload("assignment_of_executor.pdf", "(//input[@type='file'])[11]");

                    // upload death certificate
                    FileUpload("death_certificate.pdf", "(//input[@type='file'])[14]");

                    // enter receiver first name
                    NgWebElement uiReceiverFirstNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-8"));
                    uiReceiverFirstNamePublicCorporation.SendKeys(receiverFirstName);

                    // enter receiver last name
                    NgWebElement uiReceiverLastNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-9"));
                    uiReceiverLastNamePublicCorporation.SendKeys(receiverLastName);

                    // upload receiver appointment order
                    FileUpload("receiver_appointment_order.pdf", "(//input[@type='file'])[17]");

                    // upload court order
                    FileUpload("court_order.pdf", "(//input[@type='file'])[20]");

                    /* 
                    *  PERSONAL HISTORY SUMMARY FORMS
                    */

                    // upload Personal History Summary document
                    FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[24]");

                    break;

                case "partnership":

                    /* 
                    *  TYPES OF CHANGES REQUESTED
                    */

                    // click on Name Change, Licensee -- Partnership
                    NgWebElement uiNameChangeLicenseePartnership = ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiNameChangeLicenseePartnership.Click();
                    
                    // click on Name Change, Person
                    NgWebElement uiNameChangePersonPartnership = ngDriver.FindElement(By.CssSelector("#mat-checkbox-4.mat-checkbox"));
                    uiNameChangePersonPartnership.Click();
                    
                    // click on Addition of Receiver or Executor
                    NgWebElement uiAdditionOfReceiverOrExecutorPartnership = ngDriver.FindElement(By.CssSelector("#mat-checkbox-5.mat-checkbox"));
                    uiAdditionOfReceiverOrExecutorPartnership.Click();
                    
                    // enter person first name
                    NgWebElement uiFirstNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-2"));
                    uiFirstNamePartnership.SendKeys(firstName);
                    
                    // enter person last name
                    NgWebElement uiLastNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-3"));
                    uiLastNamePartnership.SendKeys(lastName);
                    
                    // enter person new first name
                    NgWebElement uiNewFirstNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-4"));
                    uiNewFirstNamePartnership.SendKeys(newFirstName);
                    
                    // enter person new last name
                    NgWebElement uiNewLastNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-5"));
                    uiNewLastNamePartnership.SendKeys(newLastName);
                    
                    // upload copy of marriage certificate
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[3]");
                    
                    // enter partnership name
                    NgWebElement uiPartnershipName = ngDriver.FindElement(By.CssSelector("input#mat-input-0"));
                    uiPartnershipName.SendKeys(partnershipName);
                    
                    // enter new partnership name
                    NgWebElement uiNewPartnershipName = ngDriver.FindElement(By.CssSelector("input#mat-input-1"));
                    uiNewPartnershipName.SendKeys(newPartnershipName);
                    
                    // upload partnership registration
                    FileUpload("partnership_agreement.pdf", "(//input[@type='file'])[5]");
                    
                    // enter executor first name
                    NgWebElement uiExecutorFirstNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-6"));
                    uiExecutorFirstNamePartnership.SendKeys(executorFirstName);
                    
                    // enter executor last name
                    NgWebElement uiExecutorLastNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-7"));
                    uiExecutorLastNamePartnership.SendKeys(executorLastName);
                    
                    // upload assignment of executor
                    FileUpload("assignment_of_executor.pdf", "(//input[@type='file'])[8]");
                    
                    // upload death certificate
                    FileUpload("death_certificate.pdf", "(//input[@type='file'])[11]");
                    
                    // enter receiver first name
                    NgWebElement uiReceiverFirstNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-8"));
                    uiReceiverFirstNamePartnership.SendKeys(receiverFirstName);
                    
                    // enter receiver last name
                    NgWebElement uiReceiverLastNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-9"));
                    uiReceiverLastNamePartnership.SendKeys(receiverLastName);

                    // upload receiver appointment order
                    FileUpload("receiver_appointment_order.pdf", "(//input[@type='file'])[14]");

                    // upload court order
                    FileUpload("court_order.pdf", "(//input[@type='file'])[17]");

                    // upload Personal History Summary document
                    FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[21]");

                    break;

                case "sole proprietorship":

                    /* 
                    *  TYPES OF CHANGES REQUESTED
                    */

                    // click on Name Change, Licensee -- Corporation
                    NgWebElement uiNameChangeLicenseeSoleProprietorship = ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiNameChangeLicenseeSoleProprietorship.Click();

                    /* 
                    *  CORPORATION NAME CHANGE
                    */

                    // enter company name
                    NgWebElement uiCompanyNameSoleProprietorship = ngDriver.FindElement(By.CssSelector("input#mat-input-0"));
                    uiCompanyNameSoleProprietorship.SendKeys(companyName);

                    // enter new company name
                    NgWebElement uiNewCompanyNameSoleProprietorship = ngDriver.FindElement(By.CssSelector("input#mat-input-1"));
                    uiNewCompanyNameSoleProprietorship.SendKeys(newCompanyName);

                    // upload certificate of name change
                    FileUpload("certificate_of_name_change.pdf", "(//input[@type='file'])[3]");

                    // upload Personal History Summary document
                    FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[6]");
                    
                    break;
            }

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}
