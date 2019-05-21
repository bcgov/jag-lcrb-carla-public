namespace Gov.Lclb.Cllb.Interfaces
{
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Auto Generated
    /// </summary>
    public partial class DynamicsClient : ServiceClient<DynamicsClient>, IDynamicsClient
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        public System.Uri NativeBaseUri { get; set; }

        public string GetEntityURI(string entityType, string id)
        {
            string result = "";
            result = NativeBaseUri + entityType + "(" + id.Trim() + ")";
            return result;
        }

        /// <summary>
        /// Get 
        /// </summary>
        /// <param name="odee">the source exception</param>
        /// <param name="errorMessage">The error message to present if no entity was created, or null if no error should be shown.</param>
        /// <returns>The ID of a new record, or null of no record was created</returns>
        public string GetCreatedRecord(OdataerrorException odee, string errorMessage)
        {
            string result = null;
            if (odee.Response.StatusCode == System.Net.HttpStatusCode.NoContent && odee.Response.Headers.ContainsKey("OData-EntityId") && odee.Response.Headers["OData-EntityId"] != null)
            {

                string temp = odee.Response.Headers["OData-EntityId"].FirstOrDefault();
                int guidStart = temp.LastIndexOf("(");
                int guidEnd = temp.LastIndexOf(")");
                result = temp.Substring(guidStart + 1, guidEnd - (guidStart + 1));
                
            }
            else
            {
                if (errorMessage != null)
                {
                    Console.WriteLine(errorMessage);
                    Console.WriteLine(odee.Message);
                    Console.WriteLine(odee.Request.Content);
                    Console.WriteLine(odee.Response.Content);
                }                
            }
            return result;
        }

        public async Task<MicrosoftDynamicsCRMadoxioApplication> GetApplicationById(Guid id)
        {
            MicrosoftDynamicsCRMadoxioApplication result;
            try
            {
                // fetch from Dynamics.
                result = await Applications.GetByKeyAsync(id.ToString());

                if (result._adoxioLicencetypeValue != null)
                {
                    result.AdoxioLicenceType = GetAdoxioLicencetypeById(Guid.Parse(result._adoxioLicencetypeValue));
                }

                if (result._adoxioApplicantValue != null)
                {
                    result.AdoxioApplicant = await GetAccountById(Guid.Parse(result._adoxioApplicantValue));
                }
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
            {
                result = null;
            }
            return result;
        }

        public async Task<MicrosoftDynamicsCRMadoxioApplication> GetApplicationByIdWithChildren(string id)
        {
            MicrosoftDynamicsCRMadoxioApplication result;
            try
            {
                string[] expand = { "adoxio_localgovindigenousnationid", "adoxio_application_SharePointDocumentLocations", "adoxio_application_adoxio_tiedhouseconnection_Application", "adoxio_AssignedLicence", "adoxio_ApplicationTypeId" };

                // fetch from Dynamics.
                result = await Applications.GetByKeyAsync(id, expand: expand);

                if (result._adoxioLicencetypeValue != null)
                {
                    result.AdoxioLicenceType = GetAdoxioLicencetypeById(Guid.Parse(result._adoxioLicencetypeValue));
                }

                if(result.AdoxioApplicationTypeId != null)
                {
                    var filter = $"_adoxio_applicationtype_value eq { result.AdoxioApplicationTypeId.AdoxioApplicationtypeid}"; 
                    var typeContents = this.Applicationtypecontents.Get(filter: filter).Value;
                    result.AdoxioApplicationTypeId.AdoxioApplicationtypeAdoxioApplicationtypecontentApplicationType = typeContents;
                }

                if (result._adoxioApplicantValue != null)
                {
                    result.AdoxioApplicant = await GetAccountById(Guid.Parse(result._adoxioApplicantValue));
                }

                if (result.AdoxioAssignedLicence != null && result.AdoxioAssignedLicence._adoxioEstablishmentValue != null)
                {
                    result.AdoxioAssignedLicence.AdoxioEstablishment = GetEstablishmentById(Guid.Parse(result.AdoxioAssignedLicence._adoxioEstablishmentValue));
                }
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException ex)
            {
                result = null;
            }
            return result;
        }

        public async Task<MicrosoftDynamicsCRMadoxioApplication> GetApplicationByIdWithChildren(Guid id)
        {
            return await this.GetApplicationByIdWithChildren(id.ToString());
        }

        public MicrosoftDynamicsCRMadoxioLicencetype GetAdoxioLicencetypeById(Guid id)
        {
            return this.GetAdoxioLicencetypeById(id.ToString());
        }

        public MicrosoftDynamicsCRMadoxioLicencetype GetAdoxioLicencetypeById(string id)
        {
            MicrosoftDynamicsCRMadoxioLicencetype result = null;

            try
            {
                // EXPAND the list of application types for this licence type
                string[] expand = { "adoxio_licencetypes_applicationtypes" };
                result = this.Licencetypes.GetByKey(adoxioLicencetypeid: id, expand: expand);
                
            }
            catch (OdataerrorException)
            {
                result = null;
            }

            // additional pass to populate the applicationtypes licencetype.
           /*
           if (result.AdoxioLicencetypesApplicationtypes != null)
           {
               foreach (var item in result.AdoxioLicencetypesApplicationtypes)
               {

                   if (item._adoxioLicencetypeidValue != null)
                   {
                       item.AdoxioLicenceTypeId = _dynamicsClient.GetAdoxioLicencetypeById(item._adoxioLicencetypeidValue);
                   }
               }
            }
           */


            return result;
        }

        /// <summary>
        /// Get a Account by their Guid
        /// </summary>
        /// <param name="system"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MicrosoftDynamicsCRMaccount> GetAccountById(Guid id)
        {
            MicrosoftDynamicsCRMaccount result;
            try
            {
                // fetch from Dynamics.
                result = await Accounts.GetByKeyAsync(id.ToString());
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
            {
                result = null;
            }

            // get the primary contact.
            if (result != null && result.Primarycontactid == null && result._primarycontactidValue != null)
            {
                try
                {
                    result.Primarycontactid = await GetContactById(Guid.Parse(result._primarycontactidValue));
                }
                catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
                {
                    result.Primarycontactid = null;
                }
            }
            return result;
        }

        public async Task<MicrosoftDynamicsCRMcontact> GetContactById(Guid id)
        {
            MicrosoftDynamicsCRMcontact result;
            try
            {
                // fetch from Dynamics.
                result = await Contacts.GetByKeyAsync(id.ToString());
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
            {
                result = null;
            }
            return result;
        }

        public MicrosoftDynamicsCRMadoxioEstablishment GetEstablishmentById(Guid id)
        {
            MicrosoftDynamicsCRMadoxioEstablishment result = null;

            try
            {
                result = Establishments.GetByKey(id.ToString());
            }
            catch (OdataerrorException)
            {
                result = null;
            }

            return result;
        }


        public async Task<MicrosoftDynamicsCRMadoxioWorker> GetWorkerById(string id)
        {
            MicrosoftDynamicsCRMadoxioWorker result;
            try
            {
                // fetch from Dynamics.
                result = await Workers.GetByKeyAsync(id);
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
            {
                result = null;
            }
            return result;
        }

        public async Task<MicrosoftDynamicsCRMadoxioWorker> GetWorkerById(Guid id)
        {
            return await GetWorkerById(id.ToString());
        }

        }
    }
