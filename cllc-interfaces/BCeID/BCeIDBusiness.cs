namespace Gov.Lclb.Cllb.Interfaces
{
    public class BCeIDBusiness
    {
        public string contactEmail { get; set; }
        public string contactPhone { get; set; }

        public string individualFirstname { get; set; }
        public string individualMiddlename { get; set; }
        public string individualOtherMiddlename { get; set; }
        public string individualSurname { get; set; }

        public string businessTypeName { get; set; }
        public string businessTypeDescription { get; set; }
        public string businessTypeCode { get; set; }
        public string businessTypeOther { get; set; }
        public string legalName { get; set; }
        public string businessNumber { get; set; }
        public string incorporationNumber { get; set; }
        public string jurisdictionOfIncorporation { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressCity { get; set; }
        public string addressProv { get; set; }
        public string addressPostal { get; set; }
        public string addressCountry { get; set; }
    }
}