using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;



namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class EstablishmentMapData
    {
        // string form of the guid.
        public string id { get; set; }        
        public string AddressCity { get; set; }
        public string AddressPostal { get; set; }
        public string AddressStreet { get; set; }
        public string Name { get; set; }
        public string License { get; set; }
        public string Phone { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
