using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Interfaces
{
    [TestClass]
    public class BCEPWrapperUnitTest
    {
		private static string test_order = "1234567890";
        private static string test_id = "4443-7132-cf6b-4e91-9fe6-fbfc-5594-fc44";
        private static string test_amt = "7500.00";
		private static string actual_url1 = "https://google.ca/Payment/Payment.asp?merchant_id=123456&trnType=P&trnOrderNumber=1234567890&ref1=http://localhost:5000/cannabislicensing/payment-confirmation&ref3=4443-7132-cf6b-4e91-9fe6-fbfc-5594-fc44&trnAmount=7500.00&hashExpiry=";
		private static string actual_url2 = "https://google.ca/process_transaction.asp?requestType=BACKEND&merchantid=123456&trnType=Q&trnOrderNumber=1234567890&hashValue=";

        [TestMethod]
        public void TestGetBCEPWrapperPaymentRedirectUrl()
        {
			var svc_url    = Environment.GetEnvironmentVariable("BCEP_SERVICE_URL");
			var svc_svcid  = Environment.GetEnvironmentVariable("BCEP_MERCHANT_ID");
			var svc_hashid = Environment.GetEnvironmentVariable("BCEP_HASH_KEY");
			var base_uri   = Environment.GetEnvironmentVariable("BASE_URI");
            var base_path  = Environment.GetEnvironmentVariable("BASE_PATH");
			var conf_url   = Environment.GetEnvironmentVariable("BCEP_CONF_PATH");
            
			var bcep = new BCEPWrapper(svc_url, svc_svcid, svc_hashid, 
			                           base_uri + base_path + conf_url);
			var url = bcep.GeneratePaymentRedirectUrl(test_order, test_id, test_amt).Result;

			Assert.IsNotNull(url);
			Assert.AreEqual(actual_url1, url.Substring(0, actual_url1.Length));
        }

		[TestMethod]
		public void TestGetBCEPWrapperPaymentValidationUrl()
        {
            var svc_url = Environment.GetEnvironmentVariable("BCEP_SERVICE_URL");
            var svc_svcid = Environment.GetEnvironmentVariable("BCEP_MERCHANT_ID");
            var svc_hashid = Environment.GetEnvironmentVariable("BCEP_HASH_KEY");
            var base_uri = Environment.GetEnvironmentVariable("BASE_URI");
            var base_path = Environment.GetEnvironmentVariable("BASE_PATH");
            var conf_url = Environment.GetEnvironmentVariable("BCEP_CONF_PATH");

            var bcep = new BCEPWrapper(svc_url, svc_svcid, svc_hashid,
                                       base_uri + base_path + conf_url);
			var url = bcep.GetVerifyPaymentTransactionUrl(test_order, test_id);

            Assert.IsNotNull(url);
            Assert.AreEqual(actual_url2, url.Substring(0, actual_url2.Length));
        }

		[TestMethod]
        public void TestBCEPWrapperVerifyPayment()
        {
            var svc_url = Environment.GetEnvironmentVariable("BCEP_SERVICE_URL");
            var svc_svcid = Environment.GetEnvironmentVariable("BCEP_MERCHANT_ID");
            var svc_hashid = Environment.GetEnvironmentVariable("BCEP_HASH_KEY");
            var base_uri = Environment.GetEnvironmentVariable("BASE_URI");
            var base_path = Environment.GetEnvironmentVariable("BASE_PATH");
            var conf_url = Environment.GetEnvironmentVariable("BCEP_CONF_PATH");

            var bcep = new BCEPWrapper(svc_url, svc_svcid, svc_hashid,
                                       base_uri + base_path + conf_url);
            var url = bcep.GetVerifyPaymentTransactionUrl(test_order, test_id);

            Assert.IsNotNull(url);
            Assert.AreEqual(actual_url2, url.Substring(0, actual_url2.Length));

			var validation_response = bcep.ProcessPaymentResponse(test_order, test_id).Result;

			Assert.IsNotNull(validation_response);
			Assert.IsTrue(validation_response.ContainsKey("query_url"));
			Assert.IsTrue(validation_response.ContainsKey("response_code"));
			Assert.IsTrue(validation_response.ContainsKey("response_phrase"));

			Assert.AreEqual("NotFound", validation_response["response_code"]);
			Assert.AreEqual("Not Found", validation_response["response_phrase"]);
        }

        [TestMethod]
        public void TestBCEPWrapperVerifyAPPROVEPayment()
        {
            var svc_url = Environment.GetEnvironmentVariable("BCEP_SERVICE_URL");
            var svc_svcid = Environment.GetEnvironmentVariable("BCEP_MERCHANT_ID");
            var svc_hashid = Environment.GetEnvironmentVariable("BCEP_HASH_KEY");
            var base_uri = Environment.GetEnvironmentVariable("BASE_URI");
            var base_path = Environment.GetEnvironmentVariable("BASE_PATH");
            var conf_url = Environment.GetEnvironmentVariable("BCEP_CONF_PATH");

            var bcep = new BCEPWrapper(svc_url, svc_svcid, svc_hashid,
                                       base_uri + base_path + conf_url);
			bcep.setHashKeyForUnitTesting("APPROVE");
            var url = bcep.GetVerifyPaymentTransactionUrl(test_order, test_id);

            Assert.IsNotNull(url);
            Assert.AreEqual(actual_url2, url.Substring(0, actual_url2.Length));

            var validation_response = bcep.ProcessPaymentResponse(test_order, test_id).Result;

            Assert.IsNotNull(validation_response);
			Assert.IsTrue(validation_response.ContainsKey("query_url"));
			Assert.IsTrue(validation_response.ContainsKey("trnApproved"));

			Assert.AreEqual("1", validation_response["trnApproved"]);
        }

		[TestMethod]
		public void TestBCEPWrapperVerifyDECLINEPayment()
		{
			var svc_url = Environment.GetEnvironmentVariable("BCEP_SERVICE_URL");
            var svc_svcid = Environment.GetEnvironmentVariable("BCEP_MERCHANT_ID");
            var svc_hashid = Environment.GetEnvironmentVariable("BCEP_HASH_KEY");
            var base_uri = Environment.GetEnvironmentVariable("BASE_URI");
            var base_path = Environment.GetEnvironmentVariable("BASE_PATH");
            var conf_url = Environment.GetEnvironmentVariable("BCEP_CONF_PATH");

            var bcep = new BCEPWrapper(svc_url, svc_svcid, svc_hashid,
                                       base_uri + base_path + conf_url);
            bcep.setHashKeyForUnitTesting("DECLINE");
            var url = bcep.GetVerifyPaymentTransactionUrl(test_order, test_id);

            Assert.IsNotNull(url);
            Assert.AreEqual(actual_url2, url.Substring(0, actual_url2.Length));

            var validation_response = bcep.ProcessPaymentResponse(test_order, test_id).Result;

            Assert.IsNotNull(validation_response);
            Assert.IsTrue(validation_response.ContainsKey("query_url"));
            Assert.IsTrue(validation_response.ContainsKey("trnApproved"));

            Assert.AreEqual("0", validation_response["trnApproved"]);
		}
    }
}
