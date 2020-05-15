using Gov.Lclb.Cllb.Public.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class CovidApplicationTests : ApiIntegrationTestBaseWithLogin
    {
        public CovidApplicationTests(CustomWebApplicationFactory<Startup> factory)
          : base(factory)
        { }

        const string service = "applications";

       

        [Fact]
        public async System.Threading.Tasks.Task TestCreateDelete()
        {            

            // C - Create
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/covid");

            CovidApplication viewmodel_application = new CovidApplication()
            {
                LicenceType = "Cannabis Retail Store" //*Mandatory field **This is an entity** E.g.Cannabis Retail Store
                ,
                ApplicantType = AdoxioApplicantTypeCodes.PrivateCorporation //*Mandatory (label=business type)
                ,
                ApplicationType = await GetDefaultCannabisApplicationType(),


                EstablishmentName = "Not a Dispensary"
                ,
                EstablishmentAddress = "123 Any Street, Victoria, BC, V1X 1X1"
                ,
                EstablishmentAddressStreet = "123 Any Street"
                ,
                EstablishmentAddressCity = "Victoria, BC"
                ,
                EstablishmentAddressPostalCode = "V1X 1X1"

                ,
                ContactPersonEmail = "test@test.com",
                ContactPersonFirstName = "Firstname",
                ContactPersonLastName = "Lastname",
                ContactPersonPhone = "1231231234",
                ContactPersonRole = "Owner",



                IsApplicationComplete = GeneralYesNo.Yes
            };

            var jsonString = JsonConvert.SerializeObject(viewmodel_application);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            CovidApplication responseViewModel = JsonConvert.DeserializeObject<CovidApplication>(jsonString);

            //Assert.Equal("Applying Person", responseViewModel.applyingPerson);
            Assert.Equal("Not a Dispensary", responseViewModel.EstablishmentName);
            Assert.Equal("Victoria, BC", responseViewModel.EstablishmentAddressCity);
            Assert.Equal("V1X1X1", responseViewModel.EstablishmentAddressPostalCode); // postal code now has spaces removed by system

            Assert.Equal("test@test.com", responseViewModel.ContactPersonEmail);
            Assert.Equal("Firstname", responseViewModel.ContactPersonFirstName);
            Assert.Equal("Lastname", responseViewModel.ContactPersonLastName);
            Assert.Equal("1231231234", responseViewModel.ContactPersonPhone);
            Assert.Equal("Owner", responseViewModel.ContactPersonRole);


            Assert.Equal(GeneralYesNo.Yes, responseViewModel.IsApplicationComplete);

            Guid id = new Guid(responseViewModel.Id);
            //return;
            // D - Delete

            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/covidDelete");
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // second delete should return a 404.
            request = new HttpRequestMessage(HttpMethod.Post, "/api/" + service + "/" + id + "/covidDelete");
            response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
         
        }

    }
}
