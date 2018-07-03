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
		private static string actual_url = "https://google.ca/?merchant_id=123456&trnType=P&trnOrderNumber=4443-7132-cf6b-4e91-9fe6-fbfc-5594-fc44&ref1=http://localhost:5000/cannabislicensing/payment-confirmation&trnAmount=7500.00&hashValue=60977C619A5975B8B23B1E3D42ED784C&hashExpiry=";

        [TestMethod]
        public void TestSuccessfullBCEPWrapperCal()
        {
			var svc_url    = Environment.GetEnvironmentVariable("BCEP_SERVICE_URL");
			var svc_svcid  = Environment.GetEnvironmentVariable("BCEP_MERCHANT_ID");
			var svc_hashid = Environment.GetEnvironmentVariable("BCEP_HASH_KEY");
			var base_uri   = Environment.GetEnvironmentVariable("BASE_URI");
            var base_path  = Environment.GetEnvironmentVariable("BASE_PATH");
			var conf_url   = Environment.GetEnvironmentVariable("BCEP_CONF_PATH");
            
			var bcep = new BCEPWrapper(svc_url, svc_svcid, svc_hashid, 
			                           base_uri + base_path + conf_url);
			var url = bcep.GeneratePaymentRedirectUrl(test_id, test_amt).Result;

			Assert.IsNotNull(url);

			Assert.AreEqual(actual_url, url.Substring(0, 1+url.LastIndexOf("=")));
        }

    }
}
