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
        [Fact]
        public async System.Threading.Tasks.Task DefaultUserIsAnonymous()
        {
			await GetCurrentUserIsUnauthorized();
        }

        [Fact]
        public async System.Threading.Tasks.Task LoginSetsCurrentUserThenLogoutIsAnonymous()
        {
			await LoginAsDefault();

			string jsonString = await GetCurrentUser();
            
            // Verify the Default development user.
			ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(jsonString);
            Assert.Equal(user.name, "TMcTesterson TestUser");
            Assert.False(user.isNewUser);

			await Logout();

			await GetCurrentUserIsUnauthorized();
        }

        [Fact]
        public async System.Threading.Tasks.Task NewUserRegistrationProcessWorks()
        {
			//string accountService = "account";

			await GetCurrentUserIsUnauthorized();

			// register as a new user
			var loginUser = randomNewUserName("NewUser", 6);

			var strId = await LoginAndRegisterAsNewUser(loginUser);

			string jsonString = await GetCurrentUser();
			ViewModels.User user = JsonConvert.DeserializeObject<ViewModels.User>(jsonString);
			Assert.Equal(user.name, loginUser + " TestUser");
			Assert.Equal(user.businessname, loginUser + " TestBusiness");

			await Logout();
            await GetCurrentUserIsUnauthorized();

			await Login(loginUser);
			jsonString = await GetCurrentUser();
            user = JsonConvert.DeserializeObject<ViewModels.User>(jsonString);
            Assert.Equal(user.name, loginUser + " TestUser");
            Assert.Equal(user.businessname, loginUser + " TestBusiness");

			await LogoutAndCleanupTestUser(strId);

            await GetCurrentUserIsUnauthorized();
        }
	}
}
