using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

using System.Text;
using Newtonsoft.Json;
using System.Net;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using Logos.Utility;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class LoginControllerTests : ApiIntegrationTestBaseWithLogin
    {
        public LoginControllerTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory)
        { }

        [Fact]
        public async System.Threading.Tasks.Task DefaultUserIsAnonymous()
        {
            await GetCurrentUserIsUnauthorized();
        }

        [Fact]
        public async System.Threading.Tasks.Task LoginSetsCurrentUserThenLogoutIsAnonymous()
        {
            await LoginAsDefault();

            ViewModels.User user = await GetCurrentUser();

            // Verify the Default development user.
            Assert.Equal(user.name, "TMcTesterson TestUser");
            Assert.False(user.isNewUser);

            await Logout();

            await GetCurrentUserIsUnauthorized();
        }

        [Fact]
        public async System.Threading.Tasks.Task NewUserRegistrationProcessWorks()
        {
            // verify (before we log in) that we are not logged in
            await GetCurrentUserIsUnauthorized();

            // register as a new user (creates an account and contact)
            var loginUser = randomNewUserName("NewUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            // verify the current user represents our new user
            ViewModels.User user = await GetCurrentUser();
            Assert.Equal(user.name, loginUser + " TestUser");
            Assert.Equal(user.businessname, loginUser + " TestBusiness");

            // fetch our current account
            ViewModels.Account account = await GetAccountForCurrentUser();

            // logout and verify we are logged out
            await Logout();
            await GetCurrentUserIsUnauthorized();

            // login again as the same user as above ^^^
            await Login(loginUser);
            user = await GetCurrentUser();
            Assert.Equal(user.name, loginUser + " TestUser");
            Assert.Equal(user.businessname, loginUser + " TestBusiness");
            account = await GetAccountForCurrentUser();

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId);

            // verify we are now logged out and un-authorized
            await GetCurrentUserIsUnauthorized();
        }
    }
}
