using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class BCEPWrapper
    {
		private string bcep_pay_url;
		private string bcep_merchid;
		private string bcep_hashkey;
		private string bcep_conf_url;

		private const string SVC_VERSION = "1.0";
		private const int HASH_EXPIRY_TIME = 30;
		private const string HASH_EXPIRY_FMT = "yyyyMMddHHmm";

		private const string BCEP_P_SCRIPT = "/Payment/Payment.asp";
		private const string BCEP_P_MERCH_ID = "merchant_id";
		private const string BCEP_P_TRANS_TYPE = "trnType=P";
		private const string BCEP_P_ORDER_NUM = "trnOrderNumber";
		private const string BCEP_P_RESPONSE_PG = "ref1";
		private const string BCEP_P_APPLICATION_ID = "ref3";
		private const string BCEP_P_TRANS_AMT = "trnAmount";
		private const string BCEP_P_HASH_VALUE = "hashValue";
		private const string BCEP_P_HASH_EXPIRY = "hashExpiry";

		private const string BCEP_Q_SCRIPT = "/process_transaction.asp";
		private const string BCEP_Q_REQUEST_TYPE = "requestType=BACKEND";
		private const string BCEP_Q_MERCH_ID = "merchantid";
		private const string BCEP_Q_TRANS_TYPE = "trnType=Q";
		private const string BCEP_Q_ORDER_NUM = "trnOrderNumber";
		private const string BCEP_Q_HASH_VALUE = "hashValue";

		private static readonly HttpClient client = new HttpClient();

		public BCEPWrapper(string pay_url, string merch_id, string hash_key, string conf_url)
        {
			this.bcep_pay_url  = pay_url;
			this.bcep_merchid  = merch_id;
			this.bcep_hashkey  = hash_key;
			this.bcep_conf_url = conf_url;
        }

		/// <summary>
        /// This is used for unit testing
		/// Set the Hash Key to APPROVED or DECLINED to bypass the payment verification process (in Verify) 
		/// and return an APPROVEd or DECLINEd transaction
        /// </summary>
		public void setHashKeyForUnitTesting(string ut_hash_key)
		{
			this.bcep_hashkey = ut_hash_key;
		}

		/// <summary>
        /// GET a payment re-direct url for an Application
        /// </summary>
		/// <param name="orderNum">Unique order number (transaction id from invoice)</param>
		/// <param name="applicationId">GUID of the Application to pay</param>
		/// <param name="amount">amount to pay (from invoice)</param>
        /// <returns></returns>
		public string GeneratePaymentRedirectUrl(string orderNum, string applicationId, string amount) 
        {
            // build the param string for the re-direct url
			string paramString = BCEP_P_MERCH_ID + "=" + bcep_merchid +
				"&" + BCEP_P_TRANS_TYPE +
				"&" + BCEP_P_ORDER_NUM + "=" + orderNum +
				"&" + BCEP_P_RESPONSE_PG + "=" + bcep_conf_url +
				"&" + BCEP_P_APPLICATION_ID + "=" + applicationId +
				"&" + BCEP_P_TRANS_AMT + "=" + amount;

			// Calculate the expiry based on the minutesToExpire value
            DateTime expiry = DateTime.Now.AddMinutes(HASH_EXPIRY_TIME);
            string expiryStr = expiry.ToString(HASH_EXPIRY_FMT);

			// Add expiry to the redirect
            paramString = paramString + "&" + BCEP_P_HASH_EXPIRY + "=" + expiryStr;

			// replace spaces with "%20" (do not do a full url encoding; does not work with BeanStream)
			paramString = paramString.Replace(" ", "%20");

			// add hash key at the end of params
			string paramStringWithHash = paramString + bcep_hashkey;

			// Calculate the MD5 value using the Hash Key set on the Order Settings page (Within Beanstream account).
            // How:
            // Place the hash key after the last parameter. 
            // Perform an MD5 hash on the text up to the end of the key, then  
            // Replace the hash key with hashValue=[hash result]. 
            // Add the result to the hosted service url. 
            // Note: Hash is calculated on the params ONLY.. Does NOT include the hosted payment page url.
            // See http://support.beanstream.com/#docs/about-hash-validation.htm?Highlight=hash for more info. 
			string hashed = getHash(paramStringWithHash);

			// Add hash and expiry to the redirect
			paramString = paramString + "&" + BCEP_P_HASH_VALUE + "=" + hashed;

            // Build re-direct URL
			string redirect = this.bcep_pay_url + BCEP_P_SCRIPT + "?" + paramString;

            return redirect;
        }

		/// <summary>
        /// Process a payment response from Bamboora (payment success or failed)
        /// This can be called if no response is received from Bamboora - it will query the server directly
        /// based on the Application's Invoice number
        /// </summary>
		/// <param name="orderNum">Order number (transaction id from invoice)</param>
		/// <param name="txnId">Bambora transaction id</param>
        /// <returns></returns>
		public async Task<Dictionary<string, string>> ProcessPaymentResponse(string orderNum, string txnId)
        {
            var txn = new BCEPTransaction();

			var query_url = GetVerifyPaymentTransactionUrl(orderNum, txnId);
			Dictionary<string, string> responseDict = new Dictionary<string, string>();
			responseDict["query_url"] = query_url;

            // special case for unit testing
			if (bcep_hashkey.Equals("APPROVE"))
			{
				responseDict["trnId"] = "01234567";
				responseDict["trnApproved"] = "1";
				return responseDict;
			}
			else if (bcep_hashkey.Equals("DECLINE"))
			{
				responseDict["trnApproved"] = "0";
				return responseDict;
			}

			// build an HTTP client and fire off a GET request
            try
            {
                // this is a status request to Bambora, and can be repeated multiple times
				var request = new HttpRequestMessage(HttpMethod.Get, query_url);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    // parse response params into a dictionary
                    var queryString = await response.Content.ReadAsStringAsync();
					var responseParams = HttpUtility.ParseQueryString(queryString);
					foreach (var key in responseParams.AllKeys)
					{
						responseDict[key] = responseParams[key];
					}
                }
                else
                {
                    // if the request fails, record the response status
					responseDict["response_code"] = response.StatusCode.ToString();
					responseDict["response_phrase"] = response.ReasonPhrase;
                }
            }
            catch (Exception e)
            {
				// ignore errors and just return null
				responseDict["message"] = e.Message;
            }
			return responseDict;
        }

		public string GetVerifyPaymentTransactionUrl(string orderNum, string txnId)
        {
			// build the param string for the re-direct url
            string paramString = BCEP_Q_REQUEST_TYPE + 
				"&" + BCEP_Q_MERCH_ID + "=" + bcep_merchid +
                "&" + BCEP_Q_TRANS_TYPE +
                "&" + BCEP_Q_ORDER_NUM + "=" + orderNum;

            // replace spaces with "%20" (do not do a full url encoding; does not work with BeanStream)
            paramString = paramString.Replace(" ", "%20");

            // add hash key at the end of params
            string paramStringWithHash = paramString + bcep_hashkey;

            // Calculate the MD5 value using the Hash Key set on the Order Settings page (Within Beanstream account).
            // How:
            // Place the hash key after the last parameter. 
            // Perform an MD5 hash on the text up to the end of the key, then  
            // Replace the hash key with hashValue=[hash result]. 
            // Add the result to the hosted service url. 
            // Note: Hash is calculated on the params ONLY.. Does NOT include the hosted payment page url.
            // See http://support.beanstream.com/#docs/about-hash-validation.htm?Highlight=hash for more info. 
            string hashed = getHash(paramStringWithHash);

            // Add hash and expiry to the redirect
            paramString = paramString + "&" + BCEP_Q_HASH_VALUE + "=" + hashed;

            // Build re-direct URL
			string query_url = this.bcep_pay_url + BCEP_Q_SCRIPT + "?" + paramString;

			return query_url;
        }

        // getHash - Calculates an MD5 hash on a message with a given key. 
        // 
        // @param message
        // @param keyString
        // @return
        // @throws BeanstreamException
        private string getHash(string message) 
		{
			byte[] bytemessage = Encoding.UTF8.GetBytes(message);
			byte[] byteHashedMessage;

            using (MD5 md5 = MD5.Create())
            {
				byteHashedMessage = md5.ComputeHash(bytemessage);
            }  
			string digest = BitConverter.ToString(byteHashedMessage).Replace("-", "");

			return digest.ToUpper();
        }

	}
}
