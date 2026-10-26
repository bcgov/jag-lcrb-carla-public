using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Controllers;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
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
        [InlineData("Ann", "Anna Marie", "Anna Marie")]
        [InlineData(null, "Marie", "Marie")]
        [InlineData("Ann", "ann", null)]
        public void LoginPatchKeepsGivenNamesWithoutALeadingFirstName(string firstName, string givenNames, string expectedMiddleName)
        {
            var headers = new HeaderDictionary { ["smgov_givennames"] = givenNames };
            if (firstName != null) headers["smgov_givenname"] = firstName;
            var patch = new Contact();

            patch.CopyLoginFields(headers);

            Assert.Equal(expectedMiddleName, patch.MiddleName);
            Assert.Equal(expectedMiddleName != null, patch.Attributes.Contains("middlename"));
        }

        [Fact]
        public async Task CurrentUserReadsPersistedProfileInsteadOfSessionSnapshot()
        {
            var id = Guid.NewGuid();
            var dataverse = new Mock<IDataverseClient>();
            dataverse.Setup(d => d.GetContactByIdAsync(id.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Contact { Id = id, FirstName = "New", LastName = "Name", EMailAddress1 = "new@example.test" });
            var response = await CurrentUser(new UserSettings
            {
                ContactId = id.ToString(),
                AuthenticatedUser = new Models.User { ContactId = id, GivenName = "Old" }
            }, dataverse.Object, new HeaderDictionary());

            Assert.Equal("New Name", response.name);
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

        private static async Task<ViewModels.User> CurrentUser(UserSettings settings, IDataverseClient dataverse, HeaderDictionary headers)
        {
            var session = new Mock<ISession>();
            var bytes = Encoding.UTF8.GetBytes(settings.GetJson());
            session.Setup(s => s.TryGetValue("UserSettings", out bytes)).Returns(true);
            var context = new DefaultHttpContext { Session = session.Object };
            foreach (var header in headers) context.Request.Headers[header.Key] = header.Value;
            var controller = new UserController(new HttpContextAccessor { HttpContext = context }, null, dataverse, null, NullLoggerFactory.Instance);
            return Assert.IsType<ViewModels.User>(Assert.IsType<JsonResult>(await controller.UsersCurrentGet()).Value);
        }
    }
}
