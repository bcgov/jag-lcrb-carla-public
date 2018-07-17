using Xunit;

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
            var loginUser = randomNewUserName("NewLoginUser", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            ViewModels.User user = await GetCurrentUser();

            // Verify the Default development user.
            Assert.Equal(user.name, loginUser + " TestUser");

            await LogoutAndCleanupTestUser(strId);

            await GetCurrentUserIsUnauthorized();
        }

        [Fact]
        public async System.Threading.Tasks.Task NewUserRegistrationProcessWorks()
        {
            // verify (before we log in) that we are not logged in
            await GetCurrentUserIsUnauthorized();

            // register as a new user (creates an account and contact)
            var loginUser = randomNewUserName("NewLoginUser", 6);
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

		[Fact]
        public async System.Threading.Tasks.Task NewUserRegistrationProcessWorksForDifferentBusinessName()
        {
            // verify (before we log in) that we are not logged in
            await GetCurrentUserIsUnauthorized();

            // register as a new user (creates an account and contact)
			var loginUser = randomNewUserName("NewLoginUser", 6);
			var businessName = randomNewUserName(loginUser, 6);
			var strId = await LoginAndRegisterAsNewUser(loginUser, businessName);

            // verify the current user represents our new user
            ViewModels.User user = await GetCurrentUser();
            Assert.Equal(user.name, loginUser + " TestUser");
			Assert.Equal(user.businessname, businessName + " TestBusiness");

            // fetch our current account
            ViewModels.Account account = await GetAccountForCurrentUser();

            // logout and verify we are logged out
            await Logout();
            await GetCurrentUserIsUnauthorized();

            // login again as the same user as above ^^^
			await Login(loginUser, businessName);
            user = await GetCurrentUser();
            Assert.Equal(user.name, loginUser + " TestUser");
			Assert.Equal(user.businessname, businessName + " TestBusiness");
            account = await GetAccountForCurrentUser();

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId);

            // verify we are now logged out and un-authorized
            await GetCurrentUserIsUnauthorized();
        }

		[Fact]
		public async System.Threading.Tasks.Task NewUserRegistrationProcessTwoUsersUnderSameBusinessWorks()
		{
			// verify (before we log in) that we are not logged in
            await GetCurrentUserIsUnauthorized();

            // register as a new user (creates an account and contact)
            var temp = randomNewUserName("NewLoginUser", 6);
			var loginUser1 = temp + "-1";
            var businessName = randomNewUserName(temp, 6);
            var strId1 = await LoginAndRegisterAsNewUser(loginUser1, businessName);

            // verify the current user represents our new user
            ViewModels.User user = await GetCurrentUser();
            Assert.Equal(user.name, loginUser1 + " TestUser");
            Assert.Equal(user.businessname, businessName + " TestBusiness");

            // fetch our current account
            ViewModels.Account account1 = await GetAccountForCurrentUser();

            // logout and verify we are logged out
            await Logout();
            await GetCurrentUserIsUnauthorized();

            // login again as a new user under the same business as above ^^^
			var loginUser2 = temp + "-2";
			var strId2 = await LoginAndRegisterAsNewUser(loginUser2, businessName);
            user = await GetCurrentUser();
            Assert.Equal(user.name, loginUser2 + " TestUser");
            Assert.Equal(user.businessname, businessName + " TestBusiness");
            var account2 = await GetAccountForCurrentUser();
			Assert.Equal(account1.id, account2.id);

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId2);
			await Login(loginUser1, businessName);
			await LogoutAndCleanupTestUser(strId1);

            // verify we are now logged out and un-authorized
            await GetCurrentUserIsUnauthorized();
		}
    }
}
