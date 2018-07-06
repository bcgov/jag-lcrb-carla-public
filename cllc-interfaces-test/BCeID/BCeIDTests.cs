using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Interfaces
{
    [TestClass]
    public class BusinessQueryUnitTest
    {
        private static string test_guid  = "44437132CF6B4E919FE6FBFC5594FC44";
        private static string test_guid2 = "4443-7132-cf6b-4e91-9fe6-fbfc-5594-fc44";
        private static string bad_guid   = "44437132CF6ABCD19FE6FBFC55949999";
		private static string tester_guid = "669D7E64FBBA472FBC905BEDC4FB43A6";

        [TestMethod]
        public void TestSuccessfullBCeIDBusinessCal()
        {
			var svc_url    = Environment.GetEnvironmentVariable("BCEID_SERVICE_URL");
			var svc_svcid  = Environment.GetEnvironmentVariable("BCEID_SERVICE_SVCID");
			var svc_userid = Environment.GetEnvironmentVariable("BCEID_SERVICE_USER");
			var svc_passwd = Environment.GetEnvironmentVariable("BCEID_SERVICE_PASSWD");
            
			var bq = new BCeIDBusinessQuery(svc_svcid, svc_userid, svc_passwd, svc_url);
			var business = bq.ProcessBusinessQuery(test_guid).Result;
            // this is a test guid, will have different name and attrs
			//var business = bq.ProcessBusinessQuery(tester_guid).Result;
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

        [TestMethod]
        public void TestCallWithBusinessNotExistsWorks()
        {
            var svc_url    = Environment.GetEnvironmentVariable("BCEID_SERVICE_URL");
            var svc_svcid  = Environment.GetEnvironmentVariable("BCEID_SERVICE_SVCID");
            var svc_userid = Environment.GetEnvironmentVariable("BCEID_SERVICE_USER");
            var svc_passwd = Environment.GetEnvironmentVariable("BCEID_SERVICE_PASSWD");
            
            var bq = new BCeIDBusinessQuery(svc_svcid, svc_userid, svc_passwd, svc_url);
            var business = bq.ProcessBusinessQuery(bad_guid).Result;
            Assert.IsNull(business);
        }

        [TestMethod]
        public void TestGuidnormalizationWorks()
        {
            var svc_url    = Environment.GetEnvironmentVariable("BCEID_SERVICE_URL");
            var svc_svcid  = Environment.GetEnvironmentVariable("BCEID_SERVICE_SVCID");
            var svc_userid = Environment.GetEnvironmentVariable("BCEID_SERVICE_USER");
            var svc_passwd = Environment.GetEnvironmentVariable("BCEID_SERVICE_PASSWD");
            
            var bq = new BCeIDBusinessQuery(svc_svcid, svc_userid, svc_passwd, svc_url);
            Assert.AreEqual(test_guid, bq.NormalizeGuid(test_guid2));
        }

        [TestMethod]
        public void TestBadPasswordReturnsError()
        {
            var svc_url    = Environment.GetEnvironmentVariable("BCEID_SERVICE_URL");
            var svc_svcid  = Environment.GetEnvironmentVariable("BCEID_SERVICE_SVCID");
            var svc_userid = Environment.GetEnvironmentVariable("BCEID_SERVICE_USER");

            var bq = new BCeIDBusinessQuery(svc_svcid, svc_userid, "bad_passwd", svc_url);

            var business = bq.ProcessBusinessQuery(test_guid).Result;
            Assert.IsNull(business);
        }

    }
}
