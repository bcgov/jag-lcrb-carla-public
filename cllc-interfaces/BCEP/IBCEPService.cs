using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public interface IBCEPService
    {
        void setHashKeyForUnitTesting(string ut_hash_key);
        string GeneratePaymentRedirectUrl(string orderNum, string applicationId, string amount, PaymentType paymentType, string confUrl = null);
        Task<Dictionary<string, string>> ProcessPaymentResponse(string orderNum, string txnId, PaymentType paymentType);
        string GetVerifyPaymentTransactionUrl(string orderNum, string txnId, PaymentType paymentType);

    }
}
