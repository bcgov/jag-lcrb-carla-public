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
	public class SecurityTests : ApiIntegrationTestBaseWithLogin
	{
		public SecurityTests(CustomWebApplicationFactory<Startup> factory)
		  : base(factory)
		{ }

		[Fact]
		public async System.Threading.Tasks.Task UserCantAccessAnotherUsersAccount()
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

			// logout and verify we are logged out
			await Logout();
			await GetCurrentUserIsUnauthorized();

            // register and login as a second user 
			var loginUser2 = randomNewUserName("NewSecUser2", 6);
            var businessName2 = randomNewUserName(loginUser2, 6);
            var strId2 = await LoginAndRegisterAsNewUser(loginUser2, businessName2);
            ViewModels.User user2 = await GetCurrentUser();
            Assert.Equal(user2.name, loginUser2 + " TestUser");
            Assert.Equal(user2.businessname, businessName2 + " TestBusiness");
            ViewModels.Account account2 = await GetAccountForCurrentUser();
			Assert.NotEqual(account1.id, account2.id);
			Assert.Equal(user2.accountid, account2.id);

			// *** as user 2, try to access account and legal entity of account 1
			var secAccount = await SecurityHelper.GetAccountRecord(_client, account1.id, false);
			Assert.Null(secAccount);
			var secLegalEntity = await SecurityHelper.GetLegalEntityRecord(_client, legalEntity1.id, false);
			Assert.Null(secLegalEntity);

			secAccount = await SecurityHelper.UpdateAccountRecord(_client, account1.id, account1, false);
			Assert.Null(secAccount);
            // ***

            // logout and cleanup second test user
			await LogoutAndCleanupTestUser(strId2);
            await GetCurrentUserIsUnauthorized();

			// login again as the same user as above ^^^
			await Login(loginUser1, businessName1);
			user1 = await GetCurrentUser();
			Assert.Equal(user1.name, loginUser1 + " TestUser");
			Assert.Equal(user1.businessname, businessName1 + " TestBusiness");
			account1 = await GetAccountForCurrentUser();

			// logout and cleanup (deletes the account and contact created above ^^^)
			await LogoutAndCleanupTestUser(strId1);
			await GetCurrentUserIsUnauthorized();
		}

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

			// *** create some shareholders and directors
			ViewModels.AdoxioLegalEntity dos1 = await SecurityHelper.CreateDirectorOrShareholder(_client, user1, legalEntity1.id, true, false, false);
			Assert.NotNull(dos1);
			ViewModels.AdoxioLegalEntity dos2 = await SecurityHelper.CreateDirectorOrShareholder(_client, user1, legalEntity1.id, false, true, false);
			Assert.NotNull(dos2);
			ViewModels.AdoxioLegalEntity dos3 = await SecurityHelper.CreateDirectorOrShareholder(_client, user1, legalEntity1.id, false, false, true);
			Assert.NotNull(dos3);
			List<ViewModels.AdoxioLegalEntity> dos1s = await SecurityHelper.GetLegalEntitiesByPosition(_client, legalEntity1.id, "director-officer-shareholder", true);
			Assert.NotNull(dos1s);
			Assert.Equal(3, dos1s.Count);
            // ***

            // logout and verify we are logged out
            await Logout();
            await GetCurrentUserIsUnauthorized();

			// register and login as a second user 
            var loginUser2 = randomNewUserName("NewSecUser2", 6);
            var businessName2 = randomNewUserName(loginUser2, 6);
            var strId2 = await LoginAndRegisterAsNewUser(loginUser2, businessName2);
            ViewModels.User user2 = await GetCurrentUser();
            Assert.Equal(user2.name, loginUser2 + " TestUser");
            Assert.Equal(user2.businessname, businessName2 + " TestBusiness");
            ViewModels.Account account2 = await GetAccountForCurrentUser();
            Assert.NotEqual(account1.id, account2.id);
            Assert.Equal(user2.accountid, account2.id);

			// *** as user 2, try to access shareholders of account 1
			var tmp = await SecurityHelper.GetLegalEntityRecord(_client, dos1.id, false);
			Assert.Null(tmp);
			tmp = await SecurityHelper.GetLegalEntityRecord(_client, dos2.id, false);
            Assert.Null(tmp);
			tmp = await SecurityHelper.GetLegalEntityRecord(_client, dos3.id, false);
            Assert.Null(tmp);
			List<ViewModels.AdoxioLegalEntity> dos2s = await SecurityHelper.GetLegalEntitiesByPosition(_client, legalEntity1.id, "director-officer-shareholder", false);
			Assert.Null(dos2s);
            // ***

            // logout and cleanup second test user
            await LogoutAndCleanupTestUser(strId2);
            await GetCurrentUserIsUnauthorized();

            // login again as the same user as above ^^^
            await Login(loginUser1, businessName1);
            user1 = await GetCurrentUser();
            Assert.Equal(user1.name, loginUser1 + " TestUser");
            Assert.Equal(user1.businessname, businessName1 + " TestBusiness");
            account1 = await GetAccountForCurrentUser();

			// *** cleanup shareholders records
            tmp = await SecurityHelper.GetLegalEntityRecord(_client, dos3.id, true);
            Assert.NotNull(tmp);
            await SecurityHelper.DeleteLegalEntityRecord(_client, dos3.id);
			tmp = await SecurityHelper.GetLegalEntityRecord(_client, dos2.id, true);
            Assert.NotNull(tmp);
            await SecurityHelper.DeleteLegalEntityRecord(_client, dos2.id);
			tmp = await SecurityHelper.GetLegalEntityRecord(_client, dos1.id, true);
            Assert.NotNull(tmp);
            await SecurityHelper.DeleteLegalEntityRecord(_client, dos1.id);
            // ***

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId1);
            await GetCurrentUserIsUnauthorized();
		}

        [Fact]
        public async System.Threading.Tasks.Task UserCantAccessAnotherUsersBusinessProfiles()
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

            // *** create some org shareholders and child business profiles
			ViewModels.AdoxioLegalEntity org1 = await SecurityHelper.CreateOrganizationalShareholder(_client, user1, legalEntity1.id);
			Assert.NotNull(org1);

            // TODO create a sub-profile of org1 profile as well

            // ***

            // logout and verify we are logged out
            await Logout();
            await GetCurrentUserIsUnauthorized();

            // register and login as a second user 
            var loginUser2 = randomNewUserName("NewSecUser2", 6);
            var businessName2 = randomNewUserName(loginUser2, 6);
            var strId2 = await LoginAndRegisterAsNewUser(loginUser2, businessName2);
            ViewModels.User user2 = await GetCurrentUser();
            Assert.Equal(user2.name, loginUser2 + " TestUser");
            Assert.Equal(user2.businessname, businessName2 + " TestBusiness");
            ViewModels.Account account2 = await GetAccountForCurrentUser();
            Assert.NotEqual(account1.id, account2.id);
            Assert.Equal(user2.accountid, account2.id);

            // *** as user 2, try to access child business profiles of account 1
			var tmp = await SecurityHelper.GetLegalEntityRecord(_client, org1.id, false);
            Assert.Null(tmp);

			// TODO test access to sub-profile of org1 profile as well

            // ***

            // logout and cleanup second test user
            await LogoutAndCleanupTestUser(strId2);
            await GetCurrentUserIsUnauthorized();

            // login again as the same user as above ^^^
            await Login(loginUser1, businessName1);
            user1 = await GetCurrentUser();
            Assert.Equal(user1.name, loginUser1 + " TestUser");
            Assert.Equal(user1.businessname, businessName1 + " TestBusiness");
            account1 = await GetAccountForCurrentUser();

            // *** cleanup business profile records
			tmp = await SecurityHelper.GetLegalEntityRecord(_client, org1.id, true);
            Assert.NotNull(tmp);

            // TODO clean up sub-profile of org1 profile as well

            await SecurityHelper.DeleteLegalEntityRecord(_client, org1.id);
            // ***

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId1);
            await GetCurrentUserIsUnauthorized();
        }

		[Fact]
        public async System.Threading.Tasks.Task UserCantAccessAnotherUsersAccountAttachments()
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

			// *** upload some legal entity attachments
			string file1 = await SecurityHelper.UploadFileToAccount(_client, legalEntity1.id, "TestFileSecurity");
			List<ViewModels.FileSystemItem> file1s = await SecurityHelper.GetFileListForAccount(_client, legalEntity1.id, "TestFileSecurity", true);
			Assert.NotNull(file1s);
			Assert.Single(file1s);
			//string _data1 = await SecurityHelper.DownloadFileForAccount(_client, account1.id, file1s[0].id, true);
            // ***

            // logout and verify we are logged out
            await Logout();
            await GetCurrentUserIsUnauthorized();

            // register and login as a second user 
            var loginUser2 = randomNewUserName("NewSecUser2", 6);
            var businessName2 = randomNewUserName(loginUser2, 6);
            var strId2 = await LoginAndRegisterAsNewUser(loginUser2, businessName2);
            ViewModels.User user2 = await GetCurrentUser();
            Assert.Equal(user2.name, loginUser2 + " TestUser");
            Assert.Equal(user2.businessname, businessName2 + " TestBusiness");
            ViewModels.Account account2 = await GetAccountForCurrentUser();
            Assert.NotEqual(account1.id, account2.id);
            Assert.Equal(user2.accountid, account2.id);

            // *** as user 2, try to access Account attachments of account 1
			List<ViewModels.FileSystemItem> file2s = await SecurityHelper.GetFileListForAccount(_client, account1.id, "TestFileSecurity", false);
            Assert.Null(file2s);
			//string _data2 = await SecurityHelper.DownloadFileForAccount(_client, account1.id, file1s[0].id, true);
            // ***

            // logout and cleanup second test user
            await LogoutAndCleanupTestUser(strId2);
            await GetCurrentUserIsUnauthorized();

            // login again as the same user as above ^^^
            await Login(loginUser1, businessName1);
            user1 = await GetCurrentUser();
            Assert.Equal(user1.name, loginUser1 + " TestUser");
            Assert.Equal(user1.businessname, businessName1 + " TestBusiness");
            account1 = await GetAccountForCurrentUser();

			// *** cleanup user1 uploaded files
			//await SecurityHelper.DeleteFileForAccount(_client, account1.id, file1s[0].id);
            // ***

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId1);
            await GetCurrentUserIsUnauthorized();
        }

		[Fact]
		public async System.Threading.Tasks.Task UserCantAccessAnotherUsersChildBusinesProfileAttachments()
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

            // *** create some org shareholders and child business profiles
            ViewModels.AdoxioLegalEntity org1 = await SecurityHelper.CreateOrganizationalShareholder(_client, user1, legalEntity1.id);
            Assert.NotNull(org1);

			// upload some files under new org1
			ViewModels.Account org1Account = await SecurityHelper.GetAccountRecord(_client, org1.shareholderAccountId, true);
			Assert.NotNull(org1Account);

			string file1 = await SecurityHelper.UploadFileToAccount(_client, org1Account.id, "TestFileSecurity");
			List<ViewModels.FileSystemItem> file1s = await SecurityHelper.GetFileListForAccount(_client, org1Account.id, "TestFileSecurity", true);
            Assert.NotNull(file1s);
            Assert.Single(file1s);

			// TODO add a sub-profile of org1 profile as well, and add some attachments for it
            
            // ***

            // logout and verify we are logged out
            await Logout();
            await GetCurrentUserIsUnauthorized();

            // register and login as a second user 
            var loginUser2 = randomNewUserName("NewSecUser2", 6);
            var businessName2 = randomNewUserName(loginUser2, 6);
            var strId2 = await LoginAndRegisterAsNewUser(loginUser2, businessName2);
            ViewModels.User user2 = await GetCurrentUser();
            Assert.Equal(user2.name, loginUser2 + " TestUser");
            Assert.Equal(user2.businessname, businessName2 + " TestBusiness");
            ViewModels.Account account2 = await GetAccountForCurrentUser();
            Assert.NotEqual(account1.id, account2.id);
            Assert.Equal(user2.accountid, account2.id);

            // *** as user 2, try to access child business profiles of account 1
            var tmp1 = await SecurityHelper.GetLegalEntityRecord(_client, org1.id, false);
            Assert.Null(tmp1);
			var tmp2 = await SecurityHelper.GetAccountRecord(_client, org1.shareholderAccountId, false);
            Assert.Null(tmp2);

			// try to access user1's files under new org1
			List<ViewModels.FileSystemItem> file2s = await SecurityHelper.GetFileListForAccount(_client, org1Account.id, "TestFileSecurity", false);
            Assert.Null(file2s);
			//string _data2 = await SecurityHelper.DownloadFileForAccount(_client, org1Account.id, file1s[0].id, true);

			// TODO test access to sub-profile of org1 profile's attachments as well
            
            // ***

            // logout and cleanup second test user
            await LogoutAndCleanupTestUser(strId2);
            await GetCurrentUserIsUnauthorized();

            // login again as the same user as above ^^^
            await Login(loginUser1, businessName1);
            user1 = await GetCurrentUser();
            Assert.Equal(user1.name, loginUser1 + " TestUser");
            Assert.Equal(user1.businessname, businessName1 + " TestBusiness");
            account1 = await GetAccountForCurrentUser();

            // *** cleanup business profile records
            tmp1 = await SecurityHelper.GetLegalEntityRecord(_client, org1.id, true);
            Assert.NotNull(tmp1);

            // ... and delete files under org1
            //await SecurityHelper.DeleteFileForAccount(_client, org1Account.id, file1s[0].id);

            await SecurityHelper.DeleteLegalEntityRecord(_client, org1.id);
            // ***

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId1);
            await GetCurrentUserIsUnauthorized();
		}

        [Fact]
        public async System.Threading.Tasks.Task UserCantAccessAnotherUsersApplications()
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

			// *** create some license applications
			ViewModels.AdoxioApplication application1 = await SecurityHelper.CreateLicenceApplication(_client, account1);
			Assert.NotNull(application1);
			var tmp = await SecurityHelper.GetLicenceApplication(_client, application1.id, true);
			Assert.NotNull(tmp);
            // ***

            // logout and verify we are logged out
            await Logout();
            await GetCurrentUserIsUnauthorized();

            // register and login as a second user 
            var loginUser2 = randomNewUserName("NewSecUser2", 6);
            var businessName2 = randomNewUserName(loginUser2, 6);
            var strId2 = await LoginAndRegisterAsNewUser(loginUser2, businessName2);
            ViewModels.User user2 = await GetCurrentUser();
            Assert.Equal(user2.name, loginUser2 + " TestUser");
            Assert.Equal(user2.businessname, businessName2 + " TestBusiness");
            ViewModels.Account account2 = await GetAccountForCurrentUser();
            Assert.NotEqual(account1.id, account2.id);
            Assert.Equal(user2.accountid, account2.id);

            // *** as user 2, try to access license applications of account 1
			tmp = await SecurityHelper.GetLicenceApplication(_client, application1.id, false);
            Assert.Null(tmp);
            // ***

            // logout and cleanup second test user
            await LogoutAndCleanupTestUser(strId2);
            await GetCurrentUserIsUnauthorized();

            // login again as the same user as above ^^^
            await Login(loginUser1, businessName1);
            user1 = await GetCurrentUser();
            Assert.Equal(user1.name, loginUser1 + " TestUser");
            Assert.Equal(user1.businessname, businessName1 + " TestBusiness");
            account1 = await GetAccountForCurrentUser();

			// *** delete license applications of account 1
			await SecurityHelper.DeleteLicenceApplication(_client, application1.id);
            // ***

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId1);
            await GetCurrentUserIsUnauthorized();
        }

		[Fact]
        public async System.Threading.Tasks.Task UserCantAccessAnotherUsersApplicationAttachments()
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

            // *** create some license applications
            ViewModels.AdoxioApplication application1 = await SecurityHelper.CreateLicenceApplication(_client, account1);
            Assert.NotNull(application1);
            var tmp = await SecurityHelper.GetLicenceApplication(_client, application1.id, true);
            Assert.NotNull(tmp);

            // add an attachment to the application
			string file1 = await SecurityHelper.UploadFileToApplication(_client, application1.id, "TestAppFileSecurity");
			List<ViewModels.FileSystemItem> file1s = await SecurityHelper.GetFileListForApplication(_client, application1.id, "TestAppFileSecurity", true);
            Assert.NotNull(file1s);
            Assert.Single(file1s);
			//string _data1 = await SecurityHelper.DownloadFileForApplication(_client, application1.id, file1s[0].id, true);
            // ***

            // logout and verify we are logged out
            await Logout();
            await GetCurrentUserIsUnauthorized();

            // register and login as a second user 
            var loginUser2 = randomNewUserName("NewSecUser2", 6);
            var businessName2 = randomNewUserName(loginUser2, 6);
            var strId2 = await LoginAndRegisterAsNewUser(loginUser2, businessName2);
            ViewModels.User user2 = await GetCurrentUser();
            Assert.Equal(user2.name, loginUser2 + " TestUser");
            Assert.Equal(user2.businessname, businessName2 + " TestBusiness");
            ViewModels.Account account2 = await GetAccountForCurrentUser();
            Assert.NotEqual(account1.id, account2.id);
            Assert.Equal(user2.accountid, account2.id);

            // *** as user 2, try to access license applications of account 1
            tmp = await SecurityHelper.GetLicenceApplication(_client, application1.id, false);
            Assert.Null(tmp);

            // test access to the application's attachment
			List<ViewModels.FileSystemItem> file2s = await SecurityHelper.GetFileListForApplication(_client, application1.id, "TestFileSecurity", false);
            Assert.Null(file2s);
			//string _data2 = await SecurityHelper.DownloadFileForApplication(_client, application1.id, file1s[0].id, true);
            // ***

            // logout and cleanup second test user
            await LogoutAndCleanupTestUser(strId2);
            await GetCurrentUserIsUnauthorized();

            // login again as the same user as above ^^^
            await Login(loginUser1, businessName1);
            user1 = await GetCurrentUser();
            Assert.Equal(user1.name, loginUser1 + " TestUser");
            Assert.Equal(user1.businessname, businessName1 + " TestBusiness");
            account1 = await GetAccountForCurrentUser();

            // *** delete license applications of account 1
            // delete the application's attachments
			//await SecurityHelper.DeleteFileForApplication(_client, application1.id, file1s[0].id);

            await SecurityHelper.DeleteLicenceApplication(_client, application1.id);
            // ***

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId1);
            await GetCurrentUserIsUnauthorized();
        }

        [Fact]
        public async System.Threading.Tasks.Task UserCantAccessAnotherUsersInvoices()
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

            // *** create some license applications and invoices
			ViewModels.AdoxioApplication application1 = await SecurityHelper.CreateLicenceApplication(_client, account1);
            Assert.NotNull(application1);
            var tmp = await SecurityHelper.GetLicenceApplication(_client, application1.id, true);
            Assert.NotNull(tmp);

			// create invoices
			Dictionary<string, string> values = await SecurityHelper.PayLicenceApplicationFee(_client, application1.id, false, true);
			Assert.NotNull(values);
			Assert.True(values.ContainsKey("trnApproved"));
            Assert.Equal("0", values["trnApproved"]);
            // ***

            // logout and verify we are logged out
            await Logout();
            await GetCurrentUserIsUnauthorized();

            // register and login as a second user 
            var loginUser2 = randomNewUserName("NewSecUser2", 6);
            var businessName2 = randomNewUserName(loginUser2, 6);
            var strId2 = await LoginAndRegisterAsNewUser(loginUser2, businessName2);
            ViewModels.User user2 = await GetCurrentUser();
            Assert.Equal(user2.name, loginUser2 + " TestUser");
            Assert.Equal(user2.businessname, businessName2 + " TestBusiness");
            ViewModels.Account account2 = await GetAccountForCurrentUser();
            Assert.NotEqual(account1.id, account2.id);
            Assert.Equal(user2.accountid, account2.id);

            // *** as user 2, try to access license application invoices of account 1
			tmp = await SecurityHelper.GetLicenceApplication(_client, application1.id, false);
            Assert.Null(tmp);

            // access invoices and try to pay
			values = await SecurityHelper.PayLicenceApplicationFee(_client, application1.id, false, false);
            Assert.Null(values);
            // ***

            // logout and cleanup second test user
            await LogoutAndCleanupTestUser(strId2);
            await GetCurrentUserIsUnauthorized();

            // login again as the same user as above ^^^
            await Login(loginUser1, businessName1);
            user1 = await GetCurrentUser();
            Assert.Equal(user1.name, loginUser1 + " TestUser");
            Assert.Equal(user1.businessname, businessName1 + " TestBusiness");
            account1 = await GetAccountForCurrentUser();

            // TODO can't delete once invoices are created

            // logout and cleanup (deletes the account and contact created above ^^^)
            await Logout();
            await GetCurrentUserIsUnauthorized();
        }
	}
}
