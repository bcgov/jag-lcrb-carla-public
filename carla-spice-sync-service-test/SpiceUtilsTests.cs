using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Gov.Lclb.Cllb.CarlaSpiceSync.Test
{
    public class SpiceUtilsTests
    {
        private readonly IConfiguration Configuration;
        private readonly ILoggerFactory LoggerFactory;

        public SpiceUtilsTests()
        {
            // fake connection to Dynamics
            var conf = new Dictionary<string, string>
            {
                {"DYNAMICS_ODATA_URI", "http://localhost"},
                {"SSG_USERNAME", "test"},
                {"SSG_PASSWORD", "test"}
            };

            Configuration = new ConfigurationBuilder().AddInMemoryCollection(conf).Build();
            LoggerFactory = new LoggerFactory();

        }

        [Fact]
        public void Should_DoThis_When_That()
        {
            // All requests made with HttpClient go through its handler's SendAsync() which we mock
            var handler = new Mock<HttpMessageHandler>();

            // subject under test (SUT)
            var spiceUtils = new SpiceUtils(Configuration, LoggerFactory, new TestHttpHandler(handler.Object));

        }
    }
}
