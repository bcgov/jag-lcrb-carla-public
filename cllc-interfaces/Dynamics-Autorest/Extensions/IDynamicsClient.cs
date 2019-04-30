namespace Gov.Lclb.Cllb.Interfaces
{
    using Microsoft.Rest;
    using Models;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    using System;

    /// <summary>
    /// Auto Generated
    /// </summary>
    public partial interface IDynamicsClient : System.IDisposable
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        System.Uri NativeBaseUri { get; set; }

        string GetEntityURI(string entityType, string id);

        string GetCreatedRecord(OdataerrorException odee, string errorMessage);

        Task<MicrosoftDynamicsCRMadoxioApplication> GetApplicationById(Guid id);
        Task<MicrosoftDynamicsCRMadoxioApplication> GetApplicationByIdWithChildren(Guid id);
        Task<MicrosoftDynamicsCRMadoxioApplication> GetApplicationByIdWithChildren(string id);

        MicrosoftDynamicsCRMadoxioLicencetype GetAdoxioLicencetypeById(Guid id);
        MicrosoftDynamicsCRMadoxioLicencetype GetAdoxioLicencetypeById(string id);

        Task<MicrosoftDynamicsCRMaccount> GetAccountById(Guid id);
        Task<MicrosoftDynamicsCRMcontact> GetContactById(Guid id);
        
        MicrosoftDynamicsCRMadoxioEstablishment GetEstablishmentById(Guid id);
    }
}
