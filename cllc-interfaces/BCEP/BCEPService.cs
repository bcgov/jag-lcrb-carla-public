using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gov.Lclb.Cllb.Interfaces
{
    public enum PaymentType
    {
        CANNABIS,
        LIQUOR,
        SPECIAL_EVENT
    };

    public class BCEPService: IBCEPService
    {
        
        private readonly bool pcir_enabled;
        private readonly string bcep_pay_url;
        private readonly string bcep_pcir_pay_url;
        private readonly string bcep_verify_url;
        private readonly string bcep_pcir_verify_url;
        private readonly string bcep_merchid;
        private readonly string bcep_alt_merchid;
        private readonly string bcep_sep_merchid;
        private string bcep_hashkey;
        private readonly string bcep_alt_hashkey;
        private readonly string bcep_sep_hashkey;
        private readonly string bcep_sep_hashtype;
        private readonly string bcep_conf_url;

        private readonly string bcep_sep_pay_url;
        private readonly string bcep_sep_verify_url;


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

        private readonly HttpClient client;

        public BCEPService(HttpClient httpClient, IConfiguration configuration)
        {

            client = httpClient;
            pcir_enabled = bool.Parse(configuration["PCIR_ENABLED"]);
            bcep_pay_url = configuration["BCEP_SERVICE_URL"];
            bcep_pcir_pay_url = configuration["BCEP_PCIR_SERVICE_URL"];
            bcep_verify_url = configuration["BCEP_SERVICE_VERIFY_URL"];
            bcep_pcir_verify_url = configuration["BCEP_PCIR_SERVICE_VERIFY_URL"];

            bcep_sep_pay_url = configuration["BCEP_SEP_SERVICE_URL"];
            bcep_sep_verify_url = configuration["BCEP_SEP_SERVICE_VERIFY_URL"];

            if (string.IsNullOrEmpty(bcep_verify_url))
            {
                bcep_verify_url = bcep_pay_url;
            }

            if (string.IsNullOrEmpty(bcep_pcir_verify_url))
            {
                bcep_pcir_verify_url = bcep_pcir_pay_url;
            }

            if (string.IsNullOrEmpty(bcep_sep_pay_url))
            {
                bcep_sep_pay_url = bcep_pay_url;
            }

            if (string.IsNullOrEmpty(bcep_sep_verify_url))
            {
                bcep_sep_verify_url = bcep_sep_pay_url;
            }
            bcep_merchid = configuration["BCEP_MERCHANT_ID"]; 
            bcep_alt_merchid = configuration["BCEP_ALTERNATE_MERCHANT_ID"];
            bcep_sep_merchid = configuration["BCEP_SEP_MERCHANT_ID"];
            bcep_hashkey = configuration["BCEP_HASH_KEY"];
            bcep_alt_hashkey = configuration["BCEP_ALTERNATE_HASH_KEY"];
            bcep_sep_hashkey = configuration["BCEP_SEP_HASH_KEY"];
            bcep_sep_hashtype = configuration["BCEP_SEP_HASH_TYPE"];

            bcep_conf_url = configuration["BASE_URI"] + configuration["BASE_PATH"] + configuration["BCEP_CONF_PATH"];
        }

        /// <summary>
        /// This is used for unit testing
        /// Set the Hash Key to APPROVED or DECLINED to bypass the payment verification process (in Verify) 
        /// and return an APPROVEd or DECLINEd transaction
        /// </summary>
        public void setHashKeyForUnitTesting(string ut_hash_key)
        {
            bcep_hashkey = ut_hash_key;
        }

        private string GetMerchId (PaymentType paymentType)
        {
            string merchid = null;
            switch (paymentType)
            {
                case PaymentType.CANNABIS:
                    merchid = bcep_merchid;
                    break;
                case PaymentType.LIQUOR:
                    merchid = bcep_alt_merchid;
                    break;
                case PaymentType.SPECIAL_EVENT:
                    merchid = bcep_sep_merchid;
                    break;
            }
            return merchid;
        }

        private string GetHashKey(PaymentType paymentType)
        {
            string hashKey = null;
            switch (paymentType)
            {
                case PaymentType.CANNABIS:
                    hashKey = bcep_hashkey;
                    break;
                case PaymentType.LIQUOR:
                    hashKey = bcep_alt_hashkey;
                    break;
                case PaymentType.SPECIAL_EVENT:
                    hashKey = bcep_sep_hashkey;
                    break;
            }
            return hashKey;
        }

        private string GetHashType(PaymentType paymentType)
        {
            string hashType = null;
            switch (paymentType)
            {
                case PaymentType.SPECIAL_EVENT:
                    hashType = bcep_sep_hashtype;
                    break;
            }
            return hashType;
        }
        /// <summary>
        /// GET a payment re-direct url for an Application
        /// </summary>
        /// <param name="orderNum">Unique order number (transaction id from invoice)</param>
        /// <param name="applicationId">GUID of the Application to pay</param>
        /// <param name="amount">amount to pay (from invoice)</param>
        /// <returns></returns>
        public string GeneratePaymentRedirectUrl(string orderNum, string applicationId, string amount, PaymentType paymentType, string confUrl = null)
        {
            if (confUrl == null)
            {
                confUrl = bcep_conf_url;
            }

            string merchid = GetMerchId(paymentType);

            // build the param string for the re-direct url
            string paramString = BCEP_P_MERCH_ID + "=" + merchid +
            "&" + BCEP_P_TRANS_TYPE +
            "&" + BCEP_P_ORDER_NUM + "=" + orderNum +
            "&" + BCEP_P_RESPONSE_PG + "=" + confUrl +
            "&" + BCEP_P_APPLICATION_ID + "=" + applicationId +
            "&" + BCEP_P_TRANS_AMT + "=" + amount;

            // Calculate the expiry based on the minutesToExpire value
            DateTime expiry = DateTime.Now.AddMinutes(HASH_EXPIRY_TIME);
            string expiryStr = expiry.ToString(HASH_EXPIRY_FMT);

            // Add expiry to the redirect
            paramString = paramString + "&" + BCEP_P_HASH_EXPIRY + "=" + expiryStr;

            // replace spaces with "%20" (do not do a full url encoding; does not work with BeanStream)
            paramString = paramString.Replace(" ", "%20");

            string hashkey = GetHashKey(paymentType);
            string hashType = GetHashType(paymentType);

            // add hash key at the end of params
            string paramStringWithHash = paramString + hashkey;

            // Calculate the MD5 value using the Hash Key set on the Order Settings page (Within Beanstream account).
            // How:
            // Place the hash key after the last parameter. 
            // Perform an MD5 hash on the text up to the end of the key, then  
            // Replace the hash key with hashValue=[hash result]. 
            // Add the result to the hosted service url. 
            // Note: Hash is calculated on the params ONLY.. Does NOT include the hosted payment page url.
            // See http://support.beanstream.com/#docs/about-hash-validation.htm?Highlight=hash for more info. 
            string hashed = getHash(paramStringWithHash, hashType);

            // Add hash and expiry to the redirect
            paramString = paramString + "&" + BCEP_P_HASH_VALUE + "=" + hashed;

            // Build re-direct URL
            string redirect;

            if (paymentType == PaymentType.SPECIAL_EVENT)
            {
                redirect = bcep_sep_pay_url;
            }
            else
            {
                if (pcir_enabled)
                {
                    redirect = bcep_pcir_pay_url;
                }
                else
                {
                    redirect = bcep_pay_url;
                }
            }

            redirect += BCEP_P_SCRIPT + "?" + paramString;

            return redirect;
        }

        /// <summary>
        /// Process a payment response from Bamboora (payment success or failed)
        /// This can be called if no response is received from Bambora - it will query the server directly
        /// based on the Application's Invoice number
        /// </summary>
        /// <param name="orderNum">Order number (transaction id from invoice)</param>
        /// <param name="txnId">Bambora transaction id</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> ProcessPaymentResponse(string orderNum, string txnId, PaymentType paymentType)
        {
            var txn = new BCEPTransaction();

            var query_url = GetVerifyPaymentTransactionUrl(orderNum, txnId, paymentType);
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

                // PCIR service requires a POST request
                if (pcir_enabled) 
                {
                    request = new HttpRequestMessage(HttpMethod.Post, query_url);
                } 

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
                    responseDict["error"] = "1";
                    responseDict["message"] = $"Status code - {response.StatusCode.ToString()} - is not success. {response.ReasonPhrase}";
                    // if the request fails, record the response status
                    responseDict["response_code"] = response.StatusCode.ToString();
                    responseDict["response_phrase"] = response.ReasonPhrase;
                }
            }
            catch (Exception e)
            {
                responseDict["error"] = "1";
                responseDict["message"] = e.Message;
            }
            return responseDict;
        }

        public string GetVerifyPaymentTransactionUrl(string orderNum, string txnId, PaymentType paymentType)
        {

            string merchid = GetMerchId(paymentType);

            // build the param string for the re-direct url
            string paramString = BCEP_Q_REQUEST_TYPE +
                "&" + BCEP_Q_MERCH_ID + "=" + merchid +
                "&" + BCEP_Q_TRANS_TYPE +
                "&" + BCEP_Q_ORDER_NUM + "=" + orderNum;

            // replace spaces with "%20" (do not do a full url encoding; does not work with BeanStream)
            paramString = paramString.Replace(" ", "%20");

            string hashkey = GetHashKey(paymentType);

            // add hash key at the end of params
            string paramStringWithHash = paramString + hashkey;

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
            string query_url;

            if (paymentType == PaymentType.SPECIAL_EVENT)
            {
                query_url = bcep_sep_verify_url;
            }
            else
            {
                if (pcir_enabled)
                {
                    query_url = bcep_pcir_verify_url;
                }
                else
                {
                    query_url = bcep_verify_url;
                }
            }

            query_url += BCEP_Q_SCRIPT + "?" + paramString;

            return query_url;
        }

        // getHash - Calculates an MD5 hash on a message with a given key. 
        // 
        // @param message
        // @param keyString
        // @return
        // @throws BeanstreamException
        private string getHash(string message, string type="MD5")
        {
            byte[] bytemessage = Encoding.UTF8.GetBytes(message);
            byte[] byteHashedMessage;

            // TG: SEP uses SHA-1 hashing.
            switch(type) {
                case "SHA-1":
                    using (SHA1 sha1 = SHA1.Create())
                    {
                        byteHashedMessage = sha1.ComputeHash(bytemessage);
                    }
                    break;
                default:
                    using (MD5 md5 = MD5.Create())
                    {
                        byteHashedMessage = md5.ComputeHash(bytemessage);
                    }
                    break;
            }
                
            string digest = BitConverter.ToString(byteHashedMessage).Replace("-", "");

            return digest.ToUpper();
        }

    }
}
