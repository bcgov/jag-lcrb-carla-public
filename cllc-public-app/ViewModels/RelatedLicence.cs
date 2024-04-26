namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class RelatedLicence
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EstablishmentName {get; set; }
        public string Streetaddress { get; set; }
        public string City { get; set; }
        public string Provstate { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }        
        public string Licensee { get; set; }

        // 2024-03-25 LCSD-6368 waynezen
        public string JobNumber { get; set; }
        public string LicenceNumber { get; set; }
        public bool Valid { get; set; }
    }
}
