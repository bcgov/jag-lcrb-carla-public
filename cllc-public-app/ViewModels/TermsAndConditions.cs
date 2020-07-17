using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Gov.Lclb.Cllb.Public.ViewModels
{

    public class TermsAndConditions
    {
        public string Id { get; set; }
        public string LicenceId { get; set; }
        public string Content { get; set; }
    }
}