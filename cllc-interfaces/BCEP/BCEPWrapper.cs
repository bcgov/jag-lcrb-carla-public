using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class BCEPWrapper
    {
        private string svcid;
        private string user;
        private string password;
        private string url;

		//private static readonly HttpClient client = new HttpClient();

        public BCEPWrapper(string svcid, string user, string password, string url)
        {
            this.svcid = svcid;
            this.user = user;
            this.password = password;
            this.url = url;
        }

        public async Task<string> GeneratePaymentRedirectUrl(string txnId, string amount) 
        {
            return "https://google.ca?id=" + txnId + "&amt=" + amount;
        }

        public async Task<BCEPTransaction> ProcessPaymentResponse(string txnId)
        {
            var txn = new BCEPTransaction();
            txn.txnId = txnId;
            txn.amount = "7500.00";
            return txn;
        }

        public async Task<BCEPTransaction> VerifyPaymentTransaction(string txnId)
        {
            var txn = new BCEPTransaction();
            txn.txnId = txnId;
            txn.amount = "7500.00";
            return txn;
        }
    }
}
