using System.Threading.Tasks;
using Xunit;
using Gov.Lclb.Cllb.OneStopService;
using System.Net.Http;
using System.Net;
using RichardSzalay.MockHttp;

namespace one_stop_service_tests
{
    public class OrgBookClientTests
    {
        [Fact]
        public async Task TestGetTopicIdSuccess()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/v2/topic/ident/registration/BC1182851")
                    .Respond("application/json", "{'id': 1667553,'create_timestamp': '2019-06-27T07:50:11.455516-07:00','update_timestamp': '2019-06-27T07:50:11.455544-07:00','source_id': 'BC1182851','type': 'registration','related_to': [],'related_from': []}");
            var httpClient = new HttpClient(mockHttp);
            var client = new OrgBookClient(httpClient, "http://localhost");
            
            int? topicId = await client.GetTopicId("BC1182851");

            Assert.Equal(1667553, topicId);
        }

        [Fact]
        public async Task TestGetTopicIdFail()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/v2/topic/ident/registration/BC1182851")
                    .Respond(HttpStatusCode.NotFound);
            var httpClient = new HttpClient(mockHttp);
            var client = new OrgBookClient(httpClient, "http://localhost");
            
            int? topicId = await client.GetTopicId("BC1182851");

            Assert.Null(topicId);
        }

        [Fact]
        public async Task TestGetSchemaIdSuccess()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/v2/schema")
                    .WithQueryString("name", "cannabis-retail-store-licence.lcrb")
                    .WithQueryString("version", "1.0.15")
                    .Respond("application/json", "{'total': 1,'page_size': 10,'page': 1,'first_index': 1,'last_index': 1,'next': null,'previous': null,'results': [{'id': 10,'create_timestamp': '2019-07-11T14:44:22.391816-07:00','update_timestamp': '2019-08-12T14:08:29.517898-07:00','name': 'cannabis-retail-store-licence.lcrb','version': '1.0.15','origin_did': 'MkxkKxTX4NdKFsPkMEWd9b'}]}");
            var httpClient = new HttpClient(mockHttp);
            var client = new OrgBookClient(httpClient, "http://localhost");
            
            int? schemaId = await client.GetSchemaId("cannabis-retail-store-licence.lcrb", "1.0.15");

            Assert.Equal(10, schemaId);
        }

        [Fact]
        public async Task TestGetSchemaIdFail()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/v2/schema")
                    .WithQueryString("name", "cannabis-retail-store-licence.lcrb")
                    .WithQueryString("version", "1.0.15")
                    .Respond("application/json", "{'total': 0,'page_size': 10,'page': 1,'first_index': 0,'last_index': 0,'next': null,'previous': null,'results': []}");
            var httpClient = new HttpClient(mockHttp);
            var client = new OrgBookClient(httpClient, "http://localhost");
            
            int? schemaId = await client.GetSchemaId("cannabis-marketing-licence.lcrb", "1.0.15");

            Assert.Null(schemaId);
        }

        [Fact]
        public async Task TestGetLicenceCredentialIdSuccess()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/v2/search/credential/topic")
                    .WithQueryString("inactive", "false")
                    .WithQueryString("latest", "true")
                    .WithQueryString("credential_type_id", "10")
                    .WithQueryString("topic_id", "1667553")
                    .Respond("application/json", "{'total': 1,'page_size': 10,'page': 1,'first_index': 1,'last_index': 1,'next': null,'previous': null,'results': [{'id': 100,'create_timestamp': '2019-07-11T14:44:22.391816-07:00','update_timestamp': '2019-08-12T14:08:29.517898-07:00'}]}");
            var httpClient = new HttpClient(mockHttp);
            var client = new OrgBookClient(httpClient, "http://localhost");
            
            int? licenceCredentialId = await client.GetLicenceCredentialId(1667553, 10);

            Assert.Equal(100, licenceCredentialId);
        }

        [Fact]
        public async Task TestGetLicenceCredentialIdFail()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/v2/search/credential/topic")
                    .WithQueryString("inactive", "false")
                    .WithQueryString("latest", "true")
                    .WithQueryString("credential_type_id", "10")
                    .WithQueryString("topic_id", "1667553")
                    .Respond("application/json", "{'total': 0,'page_size': 10,'page': 1,'first_index': 0,'last_index': 0,'next': null,'previous': null,'results': []}");
            var httpClient = new HttpClient(mockHttp);
            var client = new OrgBookClient(httpClient, "http://localhost");
            
            int? licenceCredentialId = await client.GetLicenceCredentialId(1667553, 10);

            Assert.Null(licenceCredentialId);
        }
    }
}
