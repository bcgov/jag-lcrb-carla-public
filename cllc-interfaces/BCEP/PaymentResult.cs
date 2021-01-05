using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class PaymentResult
    {
        public bool IsApproved { get; set; }
        public string CardType { get; set; }
        public string TrnAmount { get; set; }
        public string TrnDate { get; set; }
        public string TrnId { get; set; }
        public string AuthCode { get; set; }
        public string Invoice { get; set; }
        public string PaymentTransactionTitle { get; set; }
        public List<string> PaymentTransactionMessage { get; set; }

        public PaymentResult(Dictionary<string, string> verifyPayResponse)
        {
            if (verifyPayResponse == null)
            {
                return;
            }

            switch (verifyPayResponse["cardType"])
            {
                case "VI":
                    CardType = "Visa";
                    break;
                case "PV":
                    CardType = "Visa Debit";
                    break;
                case "MC":
                    CardType = "MasterCard";
                    break;
                case "AM":
                    CardType = "American Express";
                    break;
                case "MD":
                    CardType = "Debit MasterCard";
                    break;
                default:
                    CardType = verifyPayResponse["cardType"];
                    break;
            }

            TrnAmount = verifyPayResponse["trnAmount"];
            IsApproved = verifyPayResponse["trnApproved"] == "1";
            TrnDate = verifyPayResponse["trnDate"];
            TrnId = verifyPayResponse["trnId"];

            AuthCode = verifyPayResponse["authCode"];
            Invoice = verifyPayResponse["invoice"];

            if (!IsApproved)
            {
                if (verifyPayResponse["messageId"] == "559")
                {
                    PaymentTransactionTitle = "Cancelled";
                    PaymentTransactionMessage = new List<string>{
                        "Your payment transaction was cancelled.",
                        "Please note, your application remains listed under Applications In Progress"
                    };
                }
                else if (verifyPayResponse["messageId"] == "7")
                {
                    PaymentTransactionTitle = "Declined";
                    PaymentTransactionMessage = new List<string>{
                        "Your payment transaction was declined.",
                        "Please note, your application remains listed under Applications In Progress."
                    };
                }
                else
                {
                    PaymentTransactionTitle = "Declined";
                    PaymentTransactionMessage = new List<string>{
                        "Your payment transaction was declined.",
                        "Please contact your bank for more information.",
                        "Please note, your application remains listed under Applications In Progress."
                    };
                }
            }
        }
    }
}