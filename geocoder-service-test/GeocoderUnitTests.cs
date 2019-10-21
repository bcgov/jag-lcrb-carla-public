using System;
using Xunit;
using Moq;
using System.Net.Http;
using Moq.Protected;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using Gov.Lclb.Cllb.Geocoder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace geocoder_tests
{
    public class GeocodeUnitTests
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly GeocodeUtils _geocodeUtils;

        public GeocodeUnitTests()
        {
            // fake connection to Dynamics
            var conf = new Dictionary<string, string>
             {
                {"DYNAMICS_ODATA_URI", "http://localhost"},
                {"GEOCODER_API_URI", "http://localhost"},                
                {"SSG_USERNAME", "test"},
                {"SSG_PASSWORD", "test"},
                
            };
            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger("GeocoderUnitTests");
            loggerFactory.Dispose();

            _configuration = new ConfigurationBuilder().AddInMemoryCollection(conf).Build();

            _geocodeUtils = new GeocodeUtils(_configuration, _logger);
        }

        /// <summary>
        /// No unit
        /// </summary>
        [Fact]
        public void TestAddressWithNoUnit()
        {

            string address = "1234 Main Street";
            string result = _geocodeUtils.SanitizeStreetAddress(address);

            Assert.Equal(address, result);
        }

        /// <summary>
        /// Proper format - no change expected
        /// </summary>
        [Fact]
        public void TestAddressWithUnitNoSpace()
        {

            string address = "123-1234 Main Street";
            string result = _geocodeUtils.SanitizeStreetAddress(address);

            Assert.Equal(address, result);
        }

        /// <summary>
        /// Spaces before unit, between unit and street and streetname
        /// </summary>
        [Fact]
        public void TestAddressWithUnitAndSpaces1()
        {

            string address = "  123  -   1234  Main Street";
            string result = _geocodeUtils.SanitizeStreetAddress(address);

            Assert.Equal("123-1234 Main Street", result);
        }

        /// <summary>
        /// spaces before dash
        /// </summary>
        [Fact]
        public void TestAddressWithUnitAndSpaces2()
        {

            string address = "123  -1234  Main Street";
            string result = _geocodeUtils.SanitizeStreetAddress(address);

            Assert.Equal("123-1234 Main Street", result);
        }

        /// <summary>
        /// spaces after the dash
        /// </summary>
        [Fact]
        public void TestAddressWithUnitAndSpaces3()
        {

            string address = "123-   1234 Main Street";
            string result = _geocodeUtils.SanitizeStreetAddress(address);

            Assert.Equal("123-1234 Main Street", result);
        }

        /// <summary>
        /// spaces before street name
        /// </summary>
        [Fact]
        public void TestAddressWithUnitAndSpaces4()
        {

            string address = "123-1234   Main Street";
            string result = _geocodeUtils.SanitizeStreetAddress(address);

            Assert.Equal("123-1234 Main Street", result);
        }

        /// <summary>
        /// Spaces before and after the dash and before street name
        /// </summary>
        [Fact]
        public void TestAddressWithUnitAndSpaces5()
        {

            string address = "123   -   1234  Main Street";
            string result = _geocodeUtils.SanitizeStreetAddress(address);

            Assert.Equal("123-1234 Main Street", result);
        }

        /// <summary>
        /// Spaces before and after the dash
        /// </summary>
        [Fact]
        public void TestAddressWithUnitAndSpaces6()
        {

            string address = "123   -   1234 Main Street";
            string result = _geocodeUtils.SanitizeStreetAddress(address);

            Assert.Equal("123-1234 Main Street", result);
        }

        /// <summary>
        /// Null
        /// </summary>
        [Fact]
        public void TestAddressNull()
        {

            string address = null;
            string result = _geocodeUtils.SanitizeStreetAddress(address);

            Assert.Null(result);
        }

    }
}

