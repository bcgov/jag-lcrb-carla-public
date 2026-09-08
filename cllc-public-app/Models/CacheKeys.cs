namespace Gov.Lclb.Cllb.Public.Models
{
    public static class CacheKeys
    {
        public static string PolicyDocumentPrefix { get { return "_PD_"; } }
        public static string PolicyDocumentCategoryPrefix { get { return "_PDC_"; } }
        public static string ApplicationPrefix { get { return "_APP_"; } }
        public static string ApplicationTypePrefix { get { return "_AT_"; } }
        public static string LicenceTypePrefix { get { return "_LT_"; } }

        public static string LicenceTypeIDByNamePrefix { get { return "_LTIDN_"; } }
        public static string PicklistTypePrefix { get { return "_PL_"; } }
        // Read in License.GetCachedEstablishmentAsync, evicted in
        // EstablishmentsController.UpdateEstablishment. Both must use this
        // constant — a mismatch silently stops the eviction working and the
        // Licences page serves stale establishment data for up to an hour.
        public static string EstablishmentPrefix { get { return "Establishment_"; } }
        public static string EndorsementsByLicencePrefix { get { return "_ENDORSE_"; } }
        public static string OffsiteStorageByLicencePrefix { get { return "_OFFSITE_"; } }
        public static string ServiceAreasByLicencePrefix { get { return "_SVCAREA_"; } }
    }
}
