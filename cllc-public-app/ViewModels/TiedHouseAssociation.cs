using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class TiedHouseAssociation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Licencenumber { get; set; }
        public string City { get; set; }
        public string Street1 { get; set; }
        public string Country { get; set; }
        public string Type { get; set; }
        public string Postalcode { get; set; }
        public string Businessname { get; set; }
        public string Province { get; set; }
        public string Street2 { get; set; }
        public TiedHouseConnection TiedHouse { get; set; }
    }
}