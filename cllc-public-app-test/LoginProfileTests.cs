using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Controllers;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class LoginProfileTests
    {
        [Fact]
        public void LoginPatchContainsOnlySuppliedContactFields()
        {
            var patch = new Contact();
            patch.CopyLoginFields(new HeaderDictionary
            {
                ["smgov_userdisplayname"] = "Display Only",
                ["smgov_surname"] = " ",
                ["smgov_useremail"] = "new@example.test",
                ["smgov_birthdate"] = "1990-05-04"
            });

            Assert.Equal(2, patch.Attributes.Count);
            Assert.Equal("new@example.test", patch.EMailAddress1);
            Assert.Equal(new DateTime(1990, 5, 4), patch.BirthDate);
        }

        [Theory]
        [InlineData("Ann", " Marie ", "Marie")]
        [InlineData("Ann", "Anna Marie", "a Marie")]
        [InlineData(null, "Marie", "Marie")]
        [InlineData("Ann", "ann", "ann")]
        [InlineData("Ann", "Ann", null)]
        public void LoginPatchUsesLegacyMiddleNameMapping(string firstName, string givenNames, string expectedMiddleName)
        {
            var headers = new HeaderDictionary { ["smgov_givennames"] = givenNames };
            if (firstName != null) headers["smgov_givenname"] = firstName;
            var patch = new Contact();

            patch.CopyLoginFields(headers);

            Assert.Equal(expectedMiddleName, patch.MiddleName);
            Assert.Equal(expectedMiddleName != null, patch.Attributes.Contains("middlename"));
        }

        [Fact]
        public void WorkerContactKeepsSubmittedFieldsWhenRegistrationClaimsAreMissing()
        {
            var contact = new Contact
            {
                FirstName = "Submitted",
                LastName = "Name",
                EMailAddress1 = "submitted@example.test",
                Telephone1 = "250-555-0100",
                BirthDate = new DateTime(1990, 5, 4)
            };
            var registrationClaims = new ViewModels.Contact();
            registrationClaims.CopyHeaderValues(new HeaderDictionary
            {
                ["smgov_givenname"] = "Verified",
                ["smgov_userdisplayname"] = "Verified Different",
                ["smgov_userphone"] = "250-555-0101"
            }, useDisplayName: false);

            contact.CopyLoginFields(registrationClaims);

            Assert.Equal("Verified", contact.FirstName);
            Assert.Equal("Name", contact.LastName);
            Assert.Equal("submitted@example.test", contact.EMailAddress1);
            Assert.Equal("250-555-0101", contact.Telephone1);
            Assert.Equal(new DateTime(1990, 5, 4), contact.BirthDate);
        }

        [Fact]
        public async Task CurrentUserReadsPersistedProfileInsteadOfSessionSnapshot()
        {
            var id = Guid.NewGuid();
            var dataverse = new Mock<IDataverseClient>();
            dataverse.Setup(d => d.GetContactByIdAsync(id.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Contact
                {
                    Id = id, FirstName = "New", LastName = "Name", EMailAddress1 = "new@example.test",
                    ["fullname"] = "New Middle Name"
                });
            var response = await CurrentUser(new UserSettings
            {
                ContactId = id.ToString(),
                AuthenticatedUser = new Models.User { ContactId = id, GivenName = "Old" }
            }, dataverse.Object, new HeaderDictionary());

            Assert.Equal("New Middle Name", response.name);
            Assert.Equal("New", response.firstname);
            Assert.Equal("Name", response.lastname);
            Assert.Equal("new@example.test", response.email);
        }

        [Fact]
        public async Task AccountOnlyBridgeUserDisplaysTokenDetailsUntilContactExists()
        {
            var account = Guid.NewGuid();
            var response = await CurrentUser(new UserSettings
            {
                SiteMinderGuid = Guid.NewGuid().ToString(), AccountId = account.ToString(),
                AuthenticatedUser = new Models.User { AccountId = account }, IsNewUserRegistration = true
            }, new Mock<IDataverseClient>().Object, new HeaderDictionary
            {
                ["smgov_givenname"] = "New", ["smgov_surname"] = "User", ["smgov_useremail"] = "new@example.test"
            });

            Assert.True(response.isNewUser);
            Assert.Equal("New User", response.name);
            Assert.Equal(account.ToString(), response.accountid);
        }

        [Theory]
        [InlineData("", "new-user")]
        [InlineData("true", "old-user")]
        public async Task NewIdentityDoesNotInheritThePreviousContactOrAccount(string bridgeEnabled, string newUserId)
        {
            var settings = new UserSettings
            {
                UserAuthenticated = true,
                UserId = "old-user",
                UserType = "Business",
                SiteMinderGuid = Guid.NewGuid().ToString(),
                ContactId = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString(),
                BusinessLegalName = "Previous Business"
            };
            var session = CreateSession(settings, out var savedSettings);
            var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            config["FEATURE_BRIDGE_LOGIN"] = bridgeEnabled;
            using var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddSingleton(Mock.Of<IDataverseClient>())
                .AddSingleton(Mock.Of<IWebHostEnvironment>(e => e.EnvironmentName == Environments.Production))
                .BuildServiceProvider();
            var context = new DefaultHttpContext { Session = session.Object, RequestServices = services };
            context.Request.Headers["sm_user"] = newUserId;
            context.Request.Headers["smgov_userguid"] = Guid.NewGuid().ToString();
            context.Request.Headers["smgov_usertype"] = "Business";

            var schemeName = SiteMinderAuthOptions.AuthenticationSchemeName;
            var options = Mock.Of<IOptionsMonitor<SiteMinderAuthOptions>>(o => o.Get(schemeName) == new SiteMinderAuthOptions());
            var handler = new SiteminderAuthenticationHandler(options, NullLoggerFactory.Instance, UrlEncoder.Default, Mock.Of<ISystemClock>());
            await handler.InitializeAsync(new AuthenticationScheme(schemeName, null, typeof(SiteminderAuthenticationHandler)), context);
            var result = await handler.AuthenticateAsync();

            Assert.True(result.Succeeded);
            session.Verify(s => s.Remove("UserSettings"), Times.Once);
            var registeredSession = Assert.IsType<UserSettings>(savedSettings());
            Assert.True(registeredSession.IsNewUserRegistration);
            Assert.Equal(newUserId, registeredSession.UserId);
            Assert.Null(registeredSession.ContactId);
            Assert.Null(registeredSession.AccountId);
            Assert.Null(registeredSession.BusinessLegalName);
        }

        [Fact]
        public async Task AccountRegistrationCompletesWithoutRereadingTheNewContact()
        {
            var accountId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            var settings = new UserSettings
            {
                IsNewUserRegistration = true, UserType = "Business", UserId = "new-user",
                SiteMinderGuid = Guid.NewGuid().ToString(), SiteMinderBusinessGuid = Guid.NewGuid().ToString()
            };
            var session = CreateSession(settings, out var savedSettings);
            var context = new DefaultHttpContext
            {
                Session = session.Object,
                User = new ClaimsPrincipal(new ClaimsIdentity("test"))
            };
            context.Request.Headers["smgov_givenname"] = "Verified";
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            configuration["FEATURE_BRIDGE_LOGIN"] = "true";
            var dataverse = new Mock<IDataverseClient>();
            dataverse.Setup(d => d.GetAccountByExternalIdAsync(settings.SiteMinderBusinessGuid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Account { Id = accountId });
            dataverse.Setup(d => d.CreateContactAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>())).ReturnsAsync(contactId);
            var controller = new AccountsController(configuration, new HttpContextAccessor { HttpContext = context },
                null, new BCeIDBusinessQuery(null, null, null, null), NullLoggerFactory.Instance, dataverse.Object, null, null, null)
            {
                ControllerContext = new ControllerContext { HttpContext = context }
            };

            Assert.IsType<JsonResult>(await controller.CreateAccount(new ViewModels.Account()));

            dataverse.Verify(d => d.GetContactByIdAsync(contactId.ToString(), It.IsAny<CancellationToken>()), Times.Never);
            dataverse.Verify(d => d.UpdateContactBridgeLoginAsync(contactId.ToString(), settings.SiteMinderGuid,
                accountId.ToString(), settings.SiteMinderBusinessGuid, It.IsAny<CancellationToken>()), Times.Once);
            var registeredSession = Assert.IsType<UserSettings>(savedSettings());
            Assert.False(registeredSession.IsNewUserRegistration);
            Assert.Equal(contactId, registeredSession.AuthenticatedUser.ContactId);
            Assert.Equal(accountId, registeredSession.AuthenticatedUser.AccountId);
            Assert.Equal("Verified", registeredSession.AuthenticatedUser.GivenName);
        }

        private static async Task<ViewModels.User> CurrentUser(UserSettings settings, IDataverseClient dataverse, HeaderDictionary headers)
        {
            var session = CreateSession(settings, out _);
            var context = new DefaultHttpContext { Session = session.Object };
            foreach (var header in headers) context.Request.Headers[header.Key] = header.Value;
            var controller = new UserController(new HttpContextAccessor { HttpContext = context }, null, dataverse, null, NullLoggerFactory.Instance);
            return Assert.IsType<ViewModels.User>(Assert.IsType<JsonResult>(await controller.UsersCurrentGet()).Value);
        }

        private static Mock<ISession> CreateSession(UserSettings settings, out Func<UserSettings> savedSettings)
        {
            var session = new Mock<ISession>();
            var bytes = Encoding.UTF8.GetBytes(settings.GetJson());
            session.Setup(s => s.TryGetValue("UserSettings", out bytes)).Returns(true);
            UserSettings saved = null;
            session.Setup(s => s.Set("UserSettings", It.IsAny<byte[]>()))
                .Callback<string, byte[]>((_, value) => saved = UserSettings.CreateFromJson(Encoding.UTF8.GetString(value)));
            savedSettings = () => saved;
            return session;
        }
    }
}
