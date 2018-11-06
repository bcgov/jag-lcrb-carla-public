using System;
using System.Net.Http;
using Xunit;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Collections.Generic;
using Gov.Lclb.Cllb.Public.ViewModels;

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

            // first confirm we are not logged in
            await GetCurrentUserIsUnauthorized();

            // try each GET, should return unauthorized
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/submit/" + id);
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            string _discard = await response.Content.ReadAsStringAsync();

			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/verify/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            _discard = await response.Content.ReadAsStringAsync();
        }
        
		[Fact]
		public async System.Threading.Tasks.Task PaymentSubmitReturnsValidRedirectUrlAndCanBePaid()
		{
            if (_client.BaseAddress.ToString() != "http://localhost/")
            {
                return;
            }

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
				licenseType = "Cannabis Retail Store", //*Mandatory field **This is an entity** E.g.Cannabis Retail Store
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation, //*Mandatory (label=business type)
                registeredEstablishment = ViewModels.GeneralYesNo.No, //*Mandatory (Yes=1, No=0)
                                                                     //,name = initialName
                                                                     //,applyingPerson = "Applying Person" //contact
                applicant = currentAccount, //account
                                           //,jobNumber = "123"
                establishmentName = "Not a Dispensary",
                establishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1",
                establishmentaddressstreet = "123 Any Street",
                establishmentaddresscity = "Victoria, BC",
                establishmentaddresspostalcode = "V1X 1X1",
                applicationStatus = AdoxioApplicationStatusCodes.InProgress
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

            string actual_url = "https://web.na.bambora.com/scripts/Payment/Payment.asp?merchant_id=336660000&trnType=P&trnOrderNumber=" + ordernum +
                $"&ref1={_factory.Configuration["BASE_URI"]}/cannabislicensing/payment-confirmation&ref3=" + id +
                "&trnAmount=7500.00&hashExpiry=";
            Assert.True(values["url"].Length > actual_url.Length);
            Assert.Equal(actual_url, values["url"].Substring(0, actual_url.Length));
            
            // get a response
			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/verify/" + id + "/APPROVE");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            json = await response.Content.ReadAsStringAsync();
            values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			Assert.True(values.ContainsKey("query_url"));
			Assert.True(values.ContainsKey("trnApproved"));

			Assert.Equal("1", values["trnApproved"]);

            // fetch updated application
			request = new HttpRequestMessage(HttpMethod.Get, "/api/adoxioapplication/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
			string invoiceId = responseViewModel.adoxioInvoiceId;
			Assert.Equal(ViewModels.GeneralYesNo.Yes, responseViewModel.adoxioInvoiceTrigger);

			// delete invoice - note we can't delete an invoice created by Dynamics
            //request = new HttpRequestMessage(HttpMethod.Post, "/api/invoice/" + invoiceId + "/delete");
            //response = await _client.SendAsync(request);
            //string responseText = await response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();

			// delete application
			request = new HttpRequestMessage(HttpMethod.Post, "/api/adoxioapplication/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
			// should get a 404 if we try a get now.
			request = new HttpRequestMessage(HttpMethod.Get, "/api/adoxioapplication/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

			// logout and cleanup (deletes the account and contact created above ^^^)
            // note we can't delete the account due to the dependency on the invoice created by Dynamics
			await Logout(); // LogoutAndCleanupTestUser(strId); 
		}

		[Fact]
		public async System.Threading.Tasks.Task CantAccessApplicationOfDifferentCompany()
		{
			string service = "payment";
            
            // first confirm we are not logged in
            await GetCurrentUserIsUnauthorized();

            // login as default and get account for current user
			string loginUser = randomNewUserName("TestPayUser_", 6);
			string loginUser1 = loginUser + "_1";
			var strId1 = await LoginAndRegisterAsNewUser(loginUser1);

            ViewModels.User user1 = await GetCurrentUser();
            ViewModels.Account currentAccount1 = await GetAccountForCurrentUser();
            
			// create an application to test with (need a valid id)
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/adoxioapplication");

            ViewModels.AdoxioApplication viewmodel_application = new ViewModels.AdoxioApplication()
            {
                name = "Test Application Name",
				applyingPerson = "Applying Person", //contact
				applicant = currentAccount1, //account
				licenseType = "Cannabis Retail Store", //*Mandatory field **This is an entity** E.g.Cannabis Retail Store
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation, //*Mandatory (label=business type)
				jobNumber = "123",
                registeredEstablishment = ViewModels.GeneralYesNo.No, //*Mandatory (Yes=1, No=0)
                establishmentName = "Not a Dispensary",
                establishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1",
                establishmentaddressstreet = "123 Any Street",
                establishmentaddresscity = "Victoria, BC",
                establishmentaddresspostalcode = "V1X 1X1",
				applicationStatus = AdoxioApplicationStatusCodes.InProgress
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
            
			// logout so we can test as another user
			await Logout();
            
            // login as a second user and business
			string loginUser2 = loginUser + "_2";
			var strId2 = await LoginAndRegisterAsNewUser(loginUser2);

            // try to access user 1's application
			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/submit/" + id);
            response = await _client.SendAsync(request);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/verify/" + id);
            response = await _client.SendAsync(request);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // logout and cleanup (deletes the account and contact created above ^^^)
            await LogoutAndCleanupTestUser(strId2);

			// logout and cleanup (deletes the account and contact created above ^^^)
			await Login(loginUser1);
            
			// delete application
            request = new HttpRequestMessage(HttpMethod.Post, "/api/adoxioapplication/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

			// should get a 404 if we try a get now.
			request = new HttpRequestMessage(HttpMethod.Get, "/api/adoxioapplication/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            
			// note we can't delete the account due to the dependency on the invoice created by Dynamics
			await Logout();
		}

		[Fact]
		public async System.Threading.Tasks.Task PaymentSubmitDeclinedAndThenResubitApprovedWorks()
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
                licenseType = "Cannabis Retail Store", //*Mandatory field **This is an entity** E.g.Cannabis Retail Store
                applicantType = ViewModels.AdoxioApplicantTypeCodes.PrivateCorporation, //*Mandatory (label=business type)
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

            string ordernum = values["url"].Substring(values["url"].IndexOf("trnOrderNumber=") + 15, 10);
            Assert.Equal(10, ordernum.Length);

			// get a response - ask for a DECLINE
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/verify/" + id + "/DECLINE");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

			json = await response.Content.ReadAsStringAsync();
            values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.True(values.ContainsKey("query_url"));
            Assert.True(values.ContainsKey("trnApproved"));

            Assert.Equal("0", values["trnApproved"]);

			// fetch updated application
            request = new HttpRequestMessage(HttpMethod.Get, "/api/adoxioapplication/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            string invoiceId = responseViewModel.adoxioInvoiceId;

			// check application status
			Assert.Equal(ViewModels.GeneralYesNo.No, responseViewModel.adoxioInvoiceTrigger);

            // submit a second time to get it paid
			request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/submit/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            json = await response.Content.ReadAsStringAsync();
            values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.True(values.ContainsKey("url"));

            // we should get a different order number
			string ordernum2 = values["url"].Substring(values["url"].IndexOf("trnOrderNumber=") + 15, 10);
            Assert.Equal(10, ordernum2.Length);
			Assert.NotEqual(ordernum2, ordernum);

            // get a response
            request = new HttpRequestMessage(HttpMethod.Get, "/api/" + service + "/verify/" + id + "/APPROVE");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            json = await response.Content.ReadAsStringAsync();
            values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.True(values.ContainsKey("query_url"));
            Assert.True(values.ContainsKey("trnApproved"));

            Assert.Equal("1", values["trnApproved"]);

            // fetch updated application
            request = new HttpRequestMessage(HttpMethod.Get, "/api/adoxioapplication/" + id);
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            jsonString = await response.Content.ReadAsStringAsync();
            responseViewModel = JsonConvert.DeserializeObject<ViewModels.AdoxioApplication>(jsonString);
            string invoiceId2 = responseViewModel.adoxioInvoiceId;
			Assert.NotEqual(invoiceId2, invoiceId);
			Assert.Equal(ViewModels.GeneralYesNo.Yes, responseViewModel.adoxioInvoiceTrigger);

            // delete invoice - note we can't delete an invoice created by Dynamics
            //request = new HttpRequestMessage(HttpMethod.Post, "/api/invoice/" + invoiceId + "/delete");
            //response = await _client.SendAsync(request);
            //string responseText = await response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();

            // delete application
            request = new HttpRequestMessage(HttpMethod.Post, "/api/adoxioapplication/" + id + "/delete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // should get a 404 if we try a get now.
            request = new HttpRequestMessage(HttpMethod.Get, "/api/adoxioapplication/" + id);
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // logout and cleanup (deletes the account and contact created above ^^^)
            // note we can't delete the account due to the dependency on the invoice created by Dynamics
            await Logout(); // LogoutAndCleanupTestUser(strId); 
		}
	}
}
