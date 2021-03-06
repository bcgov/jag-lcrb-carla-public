﻿using Xunit;
using Xunit.Gherkin.Quick;

/*
To be added from feature file when completed
*/

namespace bdd_tests
{
    [FeatureFile("./**.feature")]
    [Collection("Liquor")]
    public sealed class LoginFlow : TestBase
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

            CheckFeatureLEConnections();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}