using System.Runtime.Serialization;
using System;
using System.Globalization;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class BCEPTransaction
    {
        public string txnId { get; set; }
        public string amount { get; set; }

    }
}
