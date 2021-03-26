using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

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
            var firstName = "Firstname";
            var lastName = "Lastname";
            var newFirstName = "Newfirstname";
            var newLastName = "Newlastname";
            var societyName = "Societyname";
            var newSocietyName = "Newsocietyname";
            var partnershipName = "Partnershipname";
            var newPartnershipName = "Newpartnershipname";
            var companyName = "Companyname";
            var newCompanyName = "Newcompanyname";
            var executorFirstName = "Executorfirstname";
            var executorLastName = "Executorlastname";
            var receiverFirstName = "Receiverfirstname";
            var receiverLastName = "Receiverlastname";

            switch (appType)
            {
                case "society":

                    /* 
                    *  TYPES OF CHANGES REQUESTED
                    */

                    // click on Change of Directors or Officers
                    var uiChangeOfDirectorsOrOfficers =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiChangeOfDirectorsOrOfficers.Click();

                    // click on Name Change, Licensee -- Society
                    var uiNameChangeLicenseeSociety =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-4.mat-checkbox"));
                    uiNameChangeLicenseeSociety.Click();

                    // click on Name Change, Person
                    var uiNameChangePerson = ngDriver.FindElement(By.CssSelector("#mat-checkbox-5.mat-checkbox"));
                    uiNameChangePerson.Click();

                    // click on Addition of Receiver or Executor
                    var uiAdditionOfReceiverOrExecutor =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-6.mat-checkbox"));
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
                    var uiFirstNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-2"));
                    uiFirstNameSociety.SendKeys(firstName);

                    // enter person last name
                    var uiLastNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-3"));
                    uiLastNameSociety.SendKeys(lastName);

                    // enter person new first name
                    var uiNewFirstNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-4"));
                    uiNewFirstNameSociety.SendKeys(newFirstName);

                    // enter person new last name
                    var uiNewLastNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-5"));
                    uiNewLastNameSociety.SendKeys(newLastName);

                    // upload copy of marriage certificate
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[6]");

                    /* 
                    *  SOCIETY NAME CHANGE
                    */

                    // enter society name
                    var uiSocietyName = ngDriver.FindElement(By.CssSelector("input#mat-input-0"));
                    uiSocietyName.SendKeys(societyName);

                    // enter society name
                    var uiNewSocietyName = ngDriver.FindElement(By.CssSelector("input#mat-input-1"));
                    uiNewSocietyName.SendKeys(newSocietyName);

                    // upload name change certificate
                    FileUpload("certificate_of_name_change.pdf", "(//input[@type='file'])[8]");

                    /* 
                    *  ADDITION OF EXECUTOR OR RECEIVER
                    */

                    // enter executor first name
                    var uiExecutorFirstNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-6"));
                    uiExecutorFirstNameSociety.SendKeys(executorFirstName);

                    // enter executor last name
                    var uiExecutorLastNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-7"));
                    uiExecutorLastNameSociety.SendKeys(executorLastName);

                    // upload assignment of executor
                    FileUpload("assignment_of_executor.pdf", "(//input[@type='file'])[11]");

                    // upload death certificate
                    FileUpload("death_certificate.pdf", "(//input[@type='file'])[14]");

                    // enter receiver first name
                    var uiReceiverFirstNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-8"));
                    uiReceiverFirstNameSociety.SendKeys(receiverFirstName);

                    // enter receiver last name
                    var uiReceiverLastNameSociety = ngDriver.FindElement(By.CssSelector("input#mat-input-9"));
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
                    var uiInternalTransferOfShares =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiInternalTransferOfShares.Click();

                    // click on External Transfer of Shares
                    var uiExternalTransferOfShares =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-4.mat-checkbox"));
                    uiExternalTransferOfShares.Click();

                    // click on Change of Directors or Officers
                    var uiChangeOfDirectorsOrOfficersPrivateCorporation =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-5.mat-checkbox"));
                    uiChangeOfDirectorsOrOfficersPrivateCorporation.Click();

                    // click on Name Change, Licensee -- Corporation
                    var uiNameChangeLicenseePrivateCorporation =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-6.mat-checkbox"));
                    uiNameChangeLicenseePrivateCorporation.Click();

                    // click on Name Change, Person
                    var uiNameChangePersonPrivateCorporation =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-7.mat-checkbox"));
                    uiNameChangePersonPrivateCorporation.Click();

                    // click on Addition of Receiver or Executor
                    var uiAdditionOfReceiverOrExecutorPrivateCorporation =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-8.mat-checkbox"));
                    uiAdditionOfReceiverOrExecutorPrivateCorporation.Click();

                    /* 
                    *  INTERNAL SHARE TRANSFER
                    */

                    // upload Central Securities Register document
                    FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

                    // upload shareholders < 10% interest
                    FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[6]");

                    // click 'Yes' for amalgamation with another company
                    var uiAmalgamationYes = ngDriver.FindElement(By.CssSelector("mat-radio-button#mat-radio-2"));
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
                    var uiFirstNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-2"));
                    uiFirstNamePrivateCorporation.SendKeys(firstName);

                    // enter person last name
                    var uiLastNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-3"));
                    uiLastNamePrivateCorporation.SendKeys(lastName);

                    // enter person new first name
                    var uiNewFirstNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-4"));
                    uiNewFirstNamePrivateCorporation.SendKeys(newFirstName);

                    // enter person new last name
                    var uiNewLastNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-5"));
                    uiNewLastNamePrivateCorporation.SendKeys(newLastName);

                    // upload copy of marriage certificate
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[30]");

                    /* 
                    *  CORPORATION NAME CHANGE
                    */

                    // enter company name
                    var uiCompanyNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-0"));
                    uiCompanyNamePrivateCorporation.SendKeys(companyName);

                    // enter new company name
                    var uiNewCompanyNamePrivateCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-1"));
                    uiNewCompanyNamePrivateCorporation.SendKeys(newCompanyName);

                    // upload certificate of name change
                    FileUpload("certificate_of_name_change.pdf", "(//input[@type='file'])[32]");

                    /* 
                    *  ADDITION OF EXECUTOR OR RECEIVER
                    */

                    // enter executor first name
                    var uiExecutorFirstNamePrivateCorporation =
                        ngDriver.FindElement(By.CssSelector("input#mat-input-6"));
                    uiExecutorFirstNamePrivateCorporation.SendKeys(executorFirstName);

                    // enter executor last name
                    var uiExecutorLastNamePrivateCorporation =
                        ngDriver.FindElement(By.CssSelector("input#mat-input-7"));
                    uiExecutorLastNamePrivateCorporation.SendKeys(executorLastName);

                    // upload assignment of executor
                    FileUpload("assignment_of_executor.pdf", "(//input[@type='file'])[35]");

                    // upload death certificate
                    FileUpload("death_certificate.pdf", "(//input[@type='file'])[38]");

                    // enter receiver first name
                    var uiReceiverFirstNamePrivateCorporation =
                        ngDriver.FindElement(By.CssSelector("input#mat-input-8"));
                    uiReceiverFirstNamePrivateCorporation.SendKeys(receiverFirstName);

                    // enter receiver last name
                    var uiReceiverLastNamePrivateCorporation =
                        ngDriver.FindElement(By.CssSelector("input#mat-input-9"));
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
                    var uiInternalTransferOfSharesPublicCorp =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiInternalTransferOfSharesPublicCorp.Click();

                    // click on External Transfer of Shares
                    var uiExternalTransferOfSharesPublicCorp =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-4.mat-checkbox"));
                    uiExternalTransferOfSharesPublicCorp.Click();

                    // click on Change of Directors or Officers
                    var uiChangeOfDirectorsPublicCorp =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-5.mat-checkbox"));
                    uiChangeOfDirectorsPublicCorp.Click();

                    // click on Name Change, Licensee -- Corporation
                    var uiNameChangePublicCorp = ngDriver.FindElement(By.CssSelector("#mat-checkbox-6.mat-checkbox"));
                    uiNameChangePublicCorp.Click();

                    // click on Name Change, Person
                    var uiNameChangePersonPublicCorp =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-7.mat-checkbox"));
                    uiNameChangePersonPublicCorp.Click();

                    // click on Addition of Receiver or Executor
                    var uiAdditionReceiverExecutor =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-8.mat-checkbox"));
                    uiAdditionReceiverExecutor.Click();

                    /*
                     *  INTERNAL SHARE TRANSFER
                     */

                    // upload central securities register
                    FileUpload("central_securities_register.pdf", "(//input[@type='file'])[2]");

                    // upload Shareholders (individuals) Holding Less Than 10% Interest
                    FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[5]");

                    // select 'Yes' for 'Have you amalgamated with another company?'
                    var uiAmalgamatedYes = ngDriver.FindElement(By.CssSelector("#mat-radio-2"));
                    uiAmalgamatedYes.Click();

                    // upload certificate of amalgamation
                    FileUpload("certificate_of_amalgamation.pdf", "(//input[@type='file'])[8]");

                    // upload central securities register
                    FileUpload("central_securities_register.pdf", "(//input[@type='file'])[11]");

                    // upload notice of articles
                    FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[14]");

                    /*
                    *  EXTERNAL SHARE TRANSFER
                    */

                    // upload central securities register
                    FileUpload("central_securities_register.pdf", "(//input[@type='file'])[17]");

                    // upload supporting business documentation
                    FileUpload("business_plan.pdf", "(//input[@type='file'])[20]");

                    // upload Shareholders (individuals) Holding Less Than 10% Interest
                    FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[23]");

                    /*
                     *  CHANGE OF DIRECTORS OR OFFICERS
                     */

                    // upload notice of articles
                    FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[26]");

                    /* 
                    *  PERSON'S NAME CHANGE
                    */

                    // enter person first name
                    var uiFirstNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-2"));
                    uiFirstNamePublicCorporation.SendKeys(firstName);

                    // enter person last name
                    var uiLastNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-3"));
                    uiLastNamePublicCorporation.SendKeys(lastName);

                    // enter person new first name
                    var uiNewFirstNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-4"));
                    uiNewFirstNamePublicCorporation.SendKeys(newFirstName);

                    // enter person new last name
                    var uiNewLastNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-5"));
                    uiNewLastNamePublicCorporation.SendKeys(newLastName);

                    // upload copy of marriage certificate
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[29]");

                    /* 
                    *  CORPORATION NAME CHANGE
                    */

                    // enter company name
                    var uiCompanyNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-0"));
                    uiCompanyNamePublicCorporation.SendKeys(companyName);

                    // enter new company name
                    var uiNewCompanyNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-1"));
                    uiNewCompanyNamePublicCorporation.SendKeys(newCompanyName);

                    // upload certificate of name change
                    FileUpload("certificate_of_name_change.pdf", "(//input[@type='file'])[32]");

                    /* 
                    *  ADDITION OF EXECUTOR OR RECEIVER
                    */

                    // enter executor first name
                    var uiExecutorFirstNamePublicCorporation =
                        ngDriver.FindElement(By.CssSelector("input#mat-input-6"));
                    uiExecutorFirstNamePublicCorporation.SendKeys(executorFirstName);

                    // enter executor last name
                    var uiExecutorLastNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-7"));
                    uiExecutorLastNamePublicCorporation.SendKeys(executorLastName);

                    // upload assignment of executor
                    FileUpload("assignment_of_executor.pdf", "(//input[@type='file'])[35]");

                    // upload death certificate
                    FileUpload("death_certificate.pdf", "(//input[@type='file'])[38]");

                    // enter receiver first name
                    var uiReceiverFirstNamePublicCorporation =
                        ngDriver.FindElement(By.CssSelector("input#mat-input-8"));
                    uiReceiverFirstNamePublicCorporation.SendKeys(receiverFirstName);

                    // enter receiver last name
                    var uiReceiverLastNamePublicCorporation = ngDriver.FindElement(By.CssSelector("input#mat-input-9"));
                    uiReceiverLastNamePublicCorporation.SendKeys(receiverLastName);

                    // upload receiver appointment order
                    FileUpload("receiver_appointment_order.pdf", "(//input[@type='file'])[41]");

                    // upload court order
                    FileUpload("court_order.pdf", "(//input[@type='file'])[44]");

                    /* 
                    *  PERSONAL HISTORY SUMMARY FORMS
                    */

                    // upload Personal History Summary document
                    FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[47]");

                    break;

                case "partnership":

                    /* 
                    *  TYPES OF CHANGES REQUESTED
                    */

                    // click on Name Change, Licensee -- Partnership
                    var uiNameChangeLicenseePartnership =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiNameChangeLicenseePartnership.Click();

                    // click on Name Change, Person
                    var uiNameChangePersonPartnership =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-4.mat-checkbox"));
                    uiNameChangePersonPartnership.Click();

                    // click on Addition of Receiver or Executor
                    var uiAdditionOfReceiverOrExecutorPartnership =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-5.mat-checkbox"));
                    uiAdditionOfReceiverOrExecutorPartnership.Click();

                    // enter person first name
                    var uiFirstNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-2"));
                    uiFirstNamePartnership.SendKeys(firstName);

                    // enter person last name
                    var uiLastNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-3"));
                    uiLastNamePartnership.SendKeys(lastName);

                    // enter person new first name
                    var uiNewFirstNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-4"));
                    uiNewFirstNamePartnership.SendKeys(newFirstName);

                    // enter person new last name
                    var uiNewLastNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-5"));
                    uiNewLastNamePartnership.SendKeys(newLastName);

                    // upload copy of marriage certificate
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[3]");

                    // enter partnership name
                    var uiPartnershipName = ngDriver.FindElement(By.CssSelector("input#mat-input-0"));
                    uiPartnershipName.SendKeys(partnershipName);

                    // enter new partnership name
                    var uiNewPartnershipName = ngDriver.FindElement(By.CssSelector("input#mat-input-1"));
                    uiNewPartnershipName.SendKeys(newPartnershipName);

                    // upload partnership registration
                    FileUpload("partnership_agreement.pdf", "(//input[@type='file'])[5]");

                    // enter executor first name
                    var uiExecutorFirstNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-6"));
                    uiExecutorFirstNamePartnership.SendKeys(executorFirstName);

                    // enter executor last name
                    var uiExecutorLastNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-7"));
                    uiExecutorLastNamePartnership.SendKeys(executorLastName);

                    // upload assignment of executor
                    FileUpload("assignment_of_executor.pdf", "(//input[@type='file'])[8]");

                    // upload death certificate
                    FileUpload("death_certificate.pdf", "(//input[@type='file'])[11]");

                    // enter receiver first name
                    var uiReceiverFirstNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-8"));
                    uiReceiverFirstNamePartnership.SendKeys(receiverFirstName);

                    // enter receiver last name
                    var uiReceiverLastNamePartnership = ngDriver.FindElement(By.CssSelector("input#mat-input-9"));
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
                    var uiNameChangeLicenseeSoleProprietorship =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiNameChangeLicenseeSoleProprietorship.Click();

                    /* 
                    *  CORPORATION NAME CHANGE
                    */

                    // enter company name
                    var uiCompanyNameSoleProprietorship = ngDriver.FindElement(By.CssSelector("input#mat-input-0"));
                    uiCompanyNameSoleProprietorship.SendKeys(companyName);

                    // enter new company name
                    var uiNewCompanyNameSoleProprietorship = ngDriver.FindElement(By.CssSelector("input#mat-input-1"));
                    uiNewCompanyNameSoleProprietorship.SendKeys(newCompanyName);

                    // upload certificate of name change
                    FileUpload("certificate_of_name_change.pdf", "(//input[@type='file'])[3]");

                    // upload Personal History Summary document
                    FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[6]");

                    break;

                case "military mess":

                    /* 
                    *  TYPES OF CHANGES REQUESTED
                    */

                    // click on Change of Directors or Officers
                    var uiChangeOfDirectorsOrOfficersMilitaryMess =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-9.mat-checkbox"));
                    uiChangeOfDirectorsOrOfficersMilitaryMess.Click();

                    // click on Name Change, Licensee -- Society
                    var uiNameChangeLicenseeMilitaryMess =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-10.mat-checkbox"));
                    uiNameChangeLicenseeMilitaryMess.Click();

                    // click on Name Change, Person
                    var uiNameChangePersonMilitaryMess = ngDriver.FindElement(By.CssSelector("#mat-checkbox-11.mat-checkbox"));
                    uiNameChangePersonMilitaryMess.Click();

                    // click on Addition of Receiver or Executor
                    var uiAdditionOfReceiverOrExecutorMilitaryMess =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-12.mat-checkbox"));
                    uiAdditionOfReceiverOrExecutorMilitaryMess.Click();

                    /* 
                    *  CHANGE OF DIRECTORS OR OFFICERS
                    */

                    // upload notice of articles
                    FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[2]");

                    /* 
                    *  PERSON'S NAME CHANGE
                    */

                    // enter person first name
                    var uiFirstNameMilitaryMess = ngDriver.FindElement(By.CssSelector("input#mat-input-2"));
                    uiFirstNameMilitaryMess.SendKeys(firstName);

                    // enter person last name
                    var uiLastNameMilitaryMess = ngDriver.FindElement(By.CssSelector("input#mat-input-3"));
                    uiLastNameMilitaryMess.SendKeys(lastName);

                    // enter person new first name
                    var uiNewFirstNameMilitaryMess = ngDriver.FindElement(By.CssSelector("input#mat-input-4"));
                    uiNewFirstNameMilitaryMess.SendKeys(newFirstName);

                    // enter person new last name
                    var uiNewLastNameMilitaryMess = ngDriver.FindElement(By.CssSelector("input#mat-input-5"));
                    uiNewLastNameMilitaryMess.SendKeys(newLastName);

                    // upload copy of marriage certificate
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[6]");

                    /* 
                    *  CORPORATION NAME CHANGE
                    */

                    // enter company name
                    var uiCompanyNameMilitaryMess = ngDriver.FindElement(By.CssSelector("input#mat-input-0"));
                    uiCompanyNameMilitaryMess.SendKeys(companyName);

                    // enter new company name
                    var uiNewCompanyNameMilitaryMess = ngDriver.FindElement(By.CssSelector("input#mat-input-1"));
                    uiNewCompanyNameMilitaryMess.SendKeys(newCompanyName);

                    // upload certificate of name change
                    FileUpload("certificate_of_name_change.pdf", "(//input[@type='file'])[8]");

                    /* 
                    *  ADDITION OF EXECUTOR OR RECEIVER
                    */

                    // enter executor first name
                    var uiExecutorFirstNameMilitaryMess =
                        ngDriver.FindElement(By.CssSelector("input#mat-input-6"));
                    uiExecutorFirstNameMilitaryMess.SendKeys(executorFirstName);

                    // enter executor last name
                    var uiExecutorLastNameMilitaryMess = ngDriver.FindElement(By.CssSelector("input#mat-input-7"));
                    uiExecutorLastNameMilitaryMess.SendKeys(executorLastName);

                    // upload assignment of executor
                    FileUpload("assignment_of_executor.pdf", "(//input[@type='file'])[11]");

                    // upload death certificate
                    FileUpload("death_certificate.pdf", "(//input[@type='file'])[14]");

                    // enter receiver first name
                    var uiReceiverFirstNameMilitaryMess =
                        ngDriver.FindElement(By.CssSelector("input#mat-input-8"));
                    uiReceiverFirstNameMilitaryMess.SendKeys(receiverFirstName);

                    // enter receiver last name
                    var uiReceiverLastNameMilitaryMess = ngDriver.FindElement(By.CssSelector("input#mat-input-9"));
                    uiReceiverLastNameMilitaryMess.SendKeys(receiverLastName);

                    // upload receiver appointment order
                    FileUpload("receiver_appointment_order.pdf", "(//input[@type='file'])[17]");

                    // upload court order
                    FileUpload("court_order.pdf", "(//input[@type='file'])[20]");

                    /* 
                    *  PERSONAL HISTORY SUMMARY FORMS
                    */

                    // upload Personal History Summary document
                    FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[23]");

                    break;
            }

            // select the authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}