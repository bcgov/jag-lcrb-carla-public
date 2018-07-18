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

namespace Gov.Lclb.Cllb.Public.Test
{
	public class SecurityUrlManglingTests : ApiIntegrationTestBaseWithLogin
	{
		public SecurityUrlManglingTests(CustomWebApplicationFactory<Startup> factory)
		  : base(factory)
		{ }

		[Fact]
        public async System.Threading.Tasks.Task UserCantAccessAnotherUsersShareholders()
        {
            // verify (before we log in) that we are not logged in
            await GetCurrentUserIsUnauthorized();

            // register as a new user (creates an account and contact)
            var loginUser1 = randomNewUserName("NewSecUser1", 6);
            var businessName1 = randomNewUserName(loginUser1, 6);
            var strId1 = await LoginAndRegisterAsNewUser(loginUser1, businessName1);

            // verify the current user represents our new user
            ViewModels.User user1 = await GetCurrentUser();
            Assert.Equal(user1.name, loginUser1 + " TestUser");
            Assert.Equal(user1.businessname, businessName1 + " TestBusiness");

            // fetch our current account
            ViewModels.Account account1 = await GetAccountForCurrentUser();
            ViewModels.AdoxioLegalEntity legalEntity1 = await SecurityHelper.GetLegalEntityRecordForCurrent(_client);
            Assert.Equal(user1.accountid, account1.id);

			// try to "hack" the query
			string hackId = legalEntity1.id + " or (adoxio_isshareholder eq true)";
			List<ViewModels.AdoxioLegalEntity> doss = await SecurityHelper.GetLegalEntitiesByPosition(_client, hackId, "director-officer-shareholder", false);
            Assert.Null(doss);

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId1);
            await GetCurrentUserIsUnauthorized();
        }
	}
}
