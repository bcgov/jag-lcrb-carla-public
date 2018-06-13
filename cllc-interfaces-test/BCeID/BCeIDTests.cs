using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BCeIDWrapper;

namespace BCeIDWrapperTest
{
    [TestClass]
    public class BusinessQueryUnitTest
    {
		private static string test_guid = "44437132CF6B4E919FE6FBFC5594FC44";

        [TestMethod]
        public void TestMethod1()
        {
			var svc_url    = Environment.GetEnvironmentVariable("BCEID_SERVICE_URL");
			var svc_svcid  = Environment.GetEnvironmentVariable("BCEID_SERVICE_SVCID");
			var svc_userid = Environment.GetEnvironmentVariable("BCEID_SERVICE_USER");
			var svc_passwd = Environment.GetEnvironmentVariable("BCEID_SERVICE_PASSWD");
            
			var business = BusinessQuery.ProcessBusinessQuery(svc_svcid, svc_userid, svc_passwd, svc_url, test_guid).Result;
			Assert.IsNotNull(business);

			Assert.AreEqual("ian.costanzo@quartech.com", business.contactEmail);
			Assert.AreEqual("250-555-1234", business.contactPhone);

			Assert.AreEqual("Chief", business.individualFirstname);
			Assert.AreEqual("Developer1", business.individualSurname);

			Assert.AreEqual("Other", business.businessTypeCode);
			Assert.AreEqual("Development Account for Cannabis Licensing System", business.businessTypeOther);
			Assert.AreEqual("ABC Cannabis Sales Dev", business.legalName);
			Assert.AreEqual("123 Any Street", business.addressLine1);
			Assert.AreEqual("Victoria", business.addressCity);
			Assert.AreEqual("BC", business.addressProv);
			Assert.AreEqual("V8W1P6", business.addressPostal);
			Assert.AreEqual("CA", business.addressCountry);
        }
/*
        [TestMethod]
        public void TestMethod2()
        {
        	var svc_userid = Environment.GetEnvironmentVariable("BCREG_SERVICE_ACCT");
        	var svc_passwd = Environment.GetEnvironmentVariable("BCREG_SERVICE_PASSWD");

            var company = CompanyQuery.ProcessCompanyQuery(svc_userid, svc_passwd, "https://twmgateway.gov.bc.ca/rest/ltsa/v1/corporations/IDONTEXIST").Result;
            Assert.IsNull(company);
        }

        [TestMethod]
        public void TestMethod3()
        {
        	var svc_userid = Environment.GetEnvironmentVariable("BCREG_SERVICE_ACCT");

            var company = CompanyQuery.ProcessCompanyQuery(svc_userid, "bad_passwd", "https://twmgateway.gov.bc.ca/rest/ltsa/v1/corporations/0410721").Result;
            Assert.IsNull(company);
        }
*/
    }
}
