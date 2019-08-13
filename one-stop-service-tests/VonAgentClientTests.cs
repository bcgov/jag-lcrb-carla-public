using System;
using System.Net.Http;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.OneStopService;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace one_stop_service_tests
{
    public class VonAgentClientTests
    {
        [Fact]
        public async Task TestCreateLicenceCredential()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/lcrb/issue-credential")
                    .Respond("application/json", "[{'sucess': true, 'result': '123-123-123', 'served_by': 'django-83'}]");
            var httpClient = new HttpClient(mockHttp);

            var mock = new Mock<ILogger<VonAgentClient>>();
            ILogger<VonAgentClient> logger = mock.Object;
            
            MicrosoftDynamicsCRMadoxioLicences licence = new MicrosoftDynamicsCRMadoxioLicences()
            {
                AdoxioLicencenumber = "400",
                AdoxioEstablishment = new MicrosoftDynamicsCRMadoxioEstablishment()
                {
                    AdoxioName = "Cannabis Establishment"
                },
                AdoxioEffectivedate = DateTime.UtcNow,
                AdoxioExpirydate = DateTime.Parse("2019-12-27T07:50:11.455516-07:00"),
                AdoxioEstablishmentaddressstreet = "159 Mary Jane Lane",
                AdoxioEstablishmentaddresscity = "Victoria",
                AdoxioEstablishmentaddresspostalcode = "V9A4V1"
            };

            var subject = new VonAgentClient(httpClient, logger, "cannabis", "1", "http://localhost");

            await subject.CreateLicenceCredential(licence, "BC1234567");

            
        }
    }
}
