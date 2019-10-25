// using System;
// using Xunit;
// using Moq;
// using System.Net.Http;
// using Moq.Protected;
// using System.Threading.Tasks;
// using System.Threading;
// using System.Net;
// using Gov.Lclb.Cllb.OneStopService;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using System.Collections.Generic;

// namespace one_stop_service_tests
// {
//     public class OrgBookUtilsTests
//     {
//         private IConfiguration Configuration;
//         private ILogger _logger;
//         private OrgBookUtils utils;
//         public OrgBookUtilsTests()
//         {
//             var conf = new Dictionary<string, string>
//             {
//                 {"ORGBOOK_URL", "http://localhost"}
//             };
//             Configuration = new ConfigurationBuilder().AddInMemoryCollection(conf).Build();

//             var loggerFactory = new LoggerFactory();
//             _logger = loggerFactory.CreateLogger("OrgBookUtilsTests");
//             loggerFactory.Dispose();

//             // var services = new ServiceCollection();
//             // var serviceProvider = services.BuildServiceProvider();
//             // services.AddTransient<IConfiguration, ConfigurationRoot>(

//             // Configuration = serviceProvider.GetService<IConfiguration>();
//             // _logger = serviceProvider.GetService<ILogger>();

//         }

//         [Fact]
//         public async Task TestGetOrgBookTopicIdSuccess()
//         {
//             var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//             handlerMock
//             .Protected()
//             // Setup the PROTECTED method to mock
//             .Setup<Task<HttpResponseMessage>>(
//                 "SendAsync",
//                 ItExpr.IsAny<HttpRequestMessage>(),
//                 ItExpr.IsAny<CancellationToken>()
//             )
//             // prepare the expected response of the mocked http call
//             .ReturnsAsync(new HttpResponseMessage()
//             {
//                 StatusCode = HttpStatusCode.OK,
//                 Content = new StringContent("{'id': 1667553,'create_timestamp': '2019-06-27T07:50:11.455516-07:00','update_timestamp': '2019-06-27T07:50:11.455544-07:00','source_id': 'BC1182851','type': 'registration','related_to': [],'related_from': []}"),
//             })
//             .Verifiable();

//             var topicId = await new OrgBookUtils(Configuration, _logger).GetOrgBookTopicId("BC1182851");

//             Assert.Equal(1667553, topicId);
//         }
//     }
// }
