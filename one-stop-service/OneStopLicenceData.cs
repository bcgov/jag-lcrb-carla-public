using System;

namespace Gov.Jag.Lcrb.OneStopService
{
    public class OneStopLicenceData
    {
        public string LicenceId { get; set; }
        public string LicenceNumber { get; set; }
        public string BusinessProgramAccountReferenceNumber { get; set; }
        public bool? OneStopSent { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public OneStopLicenceType LicenceType { get; set; }
        public OneStopEstablishment Establishment { get; set; }
        public OneStopAccount Licencee { get; set; }
    }

    public class OneStopLicenceType
    {
        public string LicenceTypeId { get; set; }
        public string Name { get; set; }
        public int? OneStopProgramAccountType { get; set; }
    }

    public class OneStopEstablishment
    {
        public string Name { get; set; }
        public string AddressStreet { get; set; }
        public string AddressCity { get; set; }
        public string AddressPostalCode { get; set; }
    }

    public class OneStopAccount
    {
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1Line1 { get; set; }
        public string Address1City { get; set; }
        public string Address1PostalCode { get; set; }
        public string PrimaryContactLastName { get; set; }
    }
}
