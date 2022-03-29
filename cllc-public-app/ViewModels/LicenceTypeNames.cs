using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public static class LicenceTypeNames
    {
        public const string CannabisRetailStore = "Cannabis Retail Store";
        public const string Marketing = "Marketing";
        public const string Catering = "Catering";

        public const string UBV = "UBrew and UVin";

        public const string Agent = "Agent";

        public const string Section119 = "S119 CRS Authorization";
    }
}
