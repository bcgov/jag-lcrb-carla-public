using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I submit a temporary use area endorsement application")]
        public void TempUseAreaApplication()
        {
            /* 
            Page Title: Temporary Use Area Endorsement at a Ski Hill or Golf Course Application
            */

            // create test data
            var patioCompDescription = "Patio comp description.";
            var patioAccessControlDescription = "Patio access control description.";
            var patioLiquorCarriedDescription = "Patio liquor carried description.";
            var removeIntoxicatedPatrons = "Removal of intoxicated patrons description.";
            var respectForNeighbours = "Respect for neighbours description.";
            var areaDescription = "Area description.";
            var occupantLoad = "99999";

            // enter patio comp description
            var uiPatioCompDescription = ngDriver.FindElement(By.CssSelector("textarea#patioCompDescription"));
            uiPatioCompDescription.SendKeys(patioCompDescription);

            // enter patio access control description
            var uiPatioAccessControlDescription =
                ngDriver.FindElement(By.CssSelector("textarea#patioAccessControlDescription"));
            uiPatioAccessControlDescription.SendKeys(patioAccessControlDescription);

            // enter patio liquor carried description
            var uiPatioLiquorCarriedDescription =
                ngDriver.FindElement(By.CssSelector("textarea#patioLiquorCarriedDescription"));
            uiPatioLiquorCarriedDescription.SendKeys(patioLiquorCarriedDescription);

            // enter removal of intoxicated patrons description
            var uiRemoveIntoxicatedPatrons = ngDriver.FindElement(By.CssSelector("textarea#description1"));
            uiRemoveIntoxicatedPatrons.SendKeys(removeIntoxicatedPatrons);

            // enter respect for neighbours
            var uiRespectForNeighbours = ngDriver.FindElement(By.CssSelector("textarea#description2"));
            uiRespectForNeighbours.SendKeys(respectForNeighbours);

            // click Fixed option
            var uiFixedOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-25-button"));
            JavaScriptClick(uiFixedOption);

            // click Portable option
            var uiPortableOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-26-button"));
            uiPortableOption.Click();

            // click Interior option
            var uiInteriorOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-27-button"));
            uiInteriorOption.Click();

            // select the outside areas button
            var uiOutsideAreas = ngDriver.FindElement(By.CssSelector("[formcontrolname='outsideAreas'] button"));
            uiOutsideAreas.Click();

            // enter the area description
            var uiAreaDescription = ngDriver.FindElement(By.CssSelector("input[formcontrolname='areaLocation']"));
            uiAreaDescription.SendKeys(areaDescription);

            // enter the occupant load
            var uiOccupantLoad = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiOccupantLoad.SendKeys(occupantLoad);

            // upload the signage document
            FileUpload("signage.pdf", "(//input[@type='file'])[2]");

            // upload the exterior photos
            FileUpload("exterior_photos.pdf", "(//input[@type='file'])[5]");

            // upload the supporting documents
            FileUpload("associates.pdf", "(//input[@type='file'])[8]");

            // click on the authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();

            // retrieve the current URL to get the application ID (needed downstream)
            var URL = ngDriver.Url;

            // retrieve the application ID
            var parsedURL = URL.Split('/');

            endorsementID = parsedURL[5];
        }
    }
}