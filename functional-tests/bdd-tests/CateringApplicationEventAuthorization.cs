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
 Feature: CateringApplicationEventAuthorization
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit an event authorization request for different business types

#-----------------------
# No Approval Requests
#-----------------------

 @cateringevent @partnership @noapproval
 Scenario: No Approval Partnership Event Authorization Request
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee  
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And the account is deleted
    Then I see the login page

 @cateringevent @privatecorporation @noapproval @hourlyTest
 Scenario: No Approval Private Corporation Event Authorization Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And the account is deleted
    Then I see the login page

 @cateringevent @publiccorporation @noapproval
 Scenario: No Approval Public Corporation Event Authorization Request
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And the account is deleted
    Then I see the login page

 @cateringevent @society @noapproval
 Scenario: No Approval Society Event Authorization Request
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And the account is deleted
    Then I see the login page

  @cateringevent @soleproprietorship @noapproval
  Scenario: No Approval Sole Proprietorship Event Authorization Request
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And the account is deleted
    Then I see the login page

#-----------------------
# 500+ Attendees Request
#-----------------------

 @cateringevent @privatecorporation @500attendees
 Scenario: 500+ Attendees Event Authorization Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request an event authorization with more than 500 people
    And the event history is updated correctly for an application with more than 500 people
    And the account is deleted
    Then I see the login page

#-----------------------
# Outdoor Request
#-----------------------

 @cateringevent @outdoor
 Scenario: Outdoor Event Authorization Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request an event authorization for an outdoor location
    And the event history is updated correctly for an application for an outdoor location
    And the account is deleted
    Then I see the login page

#-----------------------
# Indoor/Outdoor Request
#-----------------------

 @cateringevent @indooroutdoor
 Scenario: Both Indoor and Outdoor Event Authorization Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request an event authorization for an indoor and outdoor location
    And the event history is updated correctly for an application for an indoor and outdoor location
    And the account is deleted
    Then I see the login page

#-------------------------
# Past 2am (non-community)
#-------------------------

 @cateringevent @past2amnoncommunity
 Scenario: Past 2am Event Authorization Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request an event authorization for after 2am
    And the event history is updated correctly for an application for after 2am
    And the account is deleted
    Then I see the login page

#-------------------------
# Past 2am (community)
#-------------------------

 @cateringevent @past2amcommunity
 Scenario: Past 2am Community Event Authorization Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request an event authorization for a community event after 2am
    And the event history is updated correctly for an application for a community event after 2am
    And the account is deleted
    Then I see the login page

#-------------------------
# Save For Later
#-------------------------

 @cateringevent @saveforlater
 Scenario: Save For Later Event Authorization Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request an event authorization for a draft
    And the event history is updated correctly for an application for a draft
    And I click on the link for Draft
    And the saved event authorization details are correct
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CateringApplicationEventAuthorization.feature")]
    [Collection("Liquor")]
    public sealed class CateringApplicationEventAuthorization : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLiquorOne();

            CheckFeatureFlagsLGIN();

            CheckFeatureFlagsIN();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureFlagsSecurityScreening();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}