using System;
using System.Net.Http;
using Xunit;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.Test
{
	public class PaymentControllerTests : ApiIntegrationTestBaseWithLogin
	{
		public PaymentControllerTests(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
		{ }

        [Fact]
        public async System.Threading.Tasks.Task TestNoAccessToAnonymousUser()
        {
            string service = "payment";
			string id = "SomeRandomId";
			string ordernum = "SomeRandomO";

            // first confirm we are not logged in
            await GetCurrentUserIsUnauthorized();

            // try each GET, should return unauthorized
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/submit/" + id);
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            string _discard = await response.Content.ReadAsStringAsync();

			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/update/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            _discard = await response.Content.ReadAsStringAsync();

			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/verify/" + ordernum + "/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            _discard = await response.Content.ReadAsStringAsync();
        }

		[Fact]
		public async System.Threading.Tasks.Task PaymentSubmitReturnsValidRedirectUrl()
		{
			string service = "payment";
            
            // first confirm we are not logged in
            await GetCurrentUserIsUnauthorized();

			// login as default and get account for current user
            string loginUser = randomNewUserName("TestPayUser_", 6);
            var strId = await LoginAndRegisterAsNewUser(loginUser);

            ViewModels.User user = await GetCurrentUser();
            ViewModels.Account currentAccount = await GetAccountForCurrentUser();

            // create an application to test with (need a valid id)
			var request = new HttpRequestMessage(HttpMethod.Post, "/api/adoxioapplication");

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                licenseType = "34dfca90-1602-e811-813d-480fcff40721", //*Mandatory field **This is an entity** E.g.Cannabis Retail Store
                applicantType = ViewModels.Adoxio_applicanttypecodes.PrivateCorporation, //*Mandatory (label=business type)
                registeredEstablishment = ViewModels.GeneralYesNo.No, //*Mandatory (Yes=1, No=0)
                                                                     //,name = initialName
                                                                     //,applyingPerson = "Applying Person" //contact
                applicant = currentAccount, //account
                                           //,jobNumber = "123"
                establishmentName = "Not a Dispensary",
                establishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1",
                establishmentaddressstreet = "123 Any Street",
                establishmentaddresscity = "Victoria, BC",
                establishmentaddresspostalcode = "V1X 1X1"
                //,applicationStatus = "0"
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            ViewModels.AdoxioApplication responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);

            //Assert.Equal("Applying Person", responseViewModel.applyingPerson);
            Assert.Equal("Not a Dispensary", responseViewModel.establishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.establishmentaddresscity);
            Assert.Equal("V1X 1X1", responseViewModel.establishmentaddresspostalcode);

            string id = responseViewModel.id;

			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/submit/" + id);
            response = await _client.SendAsync(request);
			response.EnsureSuccessStatusCode();

			string json = await response.Content.ReadAsStringAsync();
			Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			Assert.True(values.ContainsKey("url"));

			string ordernum = values["url"].Substring(values["url"].IndexOf("trnOrderNumber=")+15, 10);
			Assert.Equal(10, ordernum.Length);

			string actual_url = "https://google.ca/Payment/Payment.asp?merchant_id=123456&trnType=P&trnOrderNumber=" + ordernum + 
				"&ref1=http://localhost:5000/cannabislicensing/payment-confirmation&ref3=" + id +
				"&trnAmount=7500.00&hashExpiry=";
			Assert.True(values["url"].Length > actual_url.Length);
			Assert.Equal(actual_url, values["url"].Substring(0, actual_url.Length));

            // get a response
			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/verify/" + ordernum + "/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            json = await response.Content.ReadAsStringAsync();
            values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			Assert.True(values.ContainsKey("status"));

			Assert.Equal("NotFound:Not Found", values["status"]);

            // delete application
			request = new HttpRequestMessage(HttpMethod.Post, "/api/adoxioapplication/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
			// logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId);
		}
	}
}
