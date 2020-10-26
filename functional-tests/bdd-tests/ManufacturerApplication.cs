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
Feature: ManufacturerApplication
    As a logged in business user
    I want to submit a Manufacturer Applications for different manufacturer and business types

#-----------------------
# Private Corporation
#-----------------------

@e2e @privatecorporation @manufacturerapp @winery
Scenario: Private Corporation Winery Manufacturer Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturerapp @distillery
Scenario: Private Corporation Distillery Manufacturer Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturerapp @brewery
Scenario: Private Corporation Brewery Manufacturer Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturerapp @copacker
Scenario: Private Corporation Co-packer Manufacturer Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

#-----------------------
# Public Corporation
#-----------------------

@e2e @publiccorporation @manufacturerapp @winery
Scenario: Public Corporation Winery Manufacturer Application
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @publiccorporation @manufacturerapp @distillery
Scenario: Public Corporation Distillery Manufacturer Application
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I review the security screening requirements for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @publiccorporation @manufacturerapp @brewery
Scenario: Public Corporation Brewery Manufacturer Application
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I review the security screening requirements for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @publiccorporation @manufacturerapp @copacker
Scenario: Public Corporation Co-packer Manufacturer Application
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I review the security screening requirements for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

#-----------------------
# Sole Proprietorship
#-----------------------

@e2e @soleproprietorship @manufacturerapp @winery
Scenario: Sole Proprietorship Winery Manufacturer Application
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @soleproprietorship @manufacturerapp @distillery
Scenario: Sole Proprietorship Distillery Manufacturer Application
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @soleproprietorship @manufacturerapp @brewery
Scenario: Sole Proprietorship Brewery Manufacturer Application
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @soleproprietorship @manufacturerapp @copacker
Scenario: Sole Proprietorship Co-packer Manufacturer Application
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

#-----------------------
# Partnership
#-----------------------

@e2e @partnership @manufacturerapp @winery
Scenario: Partnership Winery Manufacturer Application
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @partnership @manufacturerapp @distillery
Scenario: Partnership Distillery Manufacturer Application
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I review the security screening requirements for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @partnership @manufacturerapp @brewery
Scenario: Partnership Brewery Manufacturer Application
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I review the security screening requirements for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @partnership @manufacturerapp @copacker
Scenario: Partnership Co-packer Manufacturer Application
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I review the security screening requirements for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

#-----------------------
# Society
#-----------------------

@e2e @society @manufacturerapp @winery
Scenario: Society Winery Manufacturer Application
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @society @manufacturerapp @distillery
Scenario: Society Distillery Manufacturer Application
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I review the security screening requirements for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @society @manufacturerapp @brewery
Scenario: Society Brewery Manufacturer Application
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I review the security screening requirements for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @society @manufacturerapp @copacker
Scenario: Society Co-packer Manufacturer Application
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I review the security screening requirements for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

#-----------------------
# University
#-----------------------

@e2e @university @manufacturerapp @winery
Scenario: University Winery Manufacturer Application
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a university
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @university @manufacturerapp @distillery
Scenario: University Distillery Manufacturer Application
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I review the security screening requirements for a university
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @university @manufacturerapp @brewery
Scenario: University Brewery Manufacturer Application
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I review the security screening requirements for a university
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @university @manufacturerapp @copacker
Scenario: University Co-packer Manufacturer Application
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I review the security screening requirements for a university
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

#-----------------------
# Local Government
#-----------------------

@e2e @localgovernment @manufacturerapp @winery
Scenario: Local Government Winery Manufacturer Application
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @localgovernment @manufacturerapp @distillery
Scenario: Local Government Distillery Manufacturer Application
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I review the security screening requirements for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @localgovernment @manufacturerapp @brewery
Scenario: Local Government Brewery Manufacturer Application
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I review the security screening requirements for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @localgovernment @manufacturerapp @copacker
Scenario: Local Government Co-packer Manufacturer Application
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I review the security screening requirements for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

#-----------------------
# Indigenous Nation
#-----------------------

@e2e @indigenousnation @manufacturerapp @winery
Scenario: Indigenous Nation Winery Manufacturer Application
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @indigenousnation @manufacturerapp @distillery
Scenario: Indigenous Nation Distillery Manufacturer Application
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I review the security screening requirements for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @indigenousnation @manufacturerapp @brewery
Scenario: Indigenous Nation Brewery Manufacturer Application
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I review the security screening requirements for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @indigenousnation @manufacturerapp @copacker
Scenario: Indigenous Nation Co-packer Manufacturer Application
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I review the security screening requirements for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ManufacturerApplication.feature")]
    [Collection("Liquor")]
    public sealed class ManufacturerApplication : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLiquorTwo();

            CheckFeatureFlagsLicenseeChanges();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
