using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Interfaces
{
    [TestClass]
    public class BCEPWrapperUnitTest
    {
        private static string test_id = "4443-7132-cf6b-4e91-9fe6-fbfc-5594-fc44";
        private static string test_amt = "7500.00";

        [TestMethod]
        public void TestSuccessfullBCEPWrapperCal()
        {
			var svc_url    = Environment.GetEnvironmentVariable("BCEID_SERVICE_URL");
			var svc_svcid  = Environment.GetEnvironmentVariable("BCEID_SERVICE_SVCID");
			var svc_userid = Environment.GetEnvironmentVariable("BCEID_SERVICE_USER");
			var svc_passwd = Environment.GetEnvironmentVariable("BCEID_SERVICE_PASSWD");
            
			var bcep = new BCEPWrapper(svc_svcid, svc_userid, svc_passwd, svc_url);
			var url = bcep.GeneratePaymentRedirectUrl(test_id, test_amt).Result;

			Assert.IsNotNull(url);

			Assert.AreEqual("https://google.ca?id=" + test_id + "&amt=" + test_amt, url);
        }

    }
}
