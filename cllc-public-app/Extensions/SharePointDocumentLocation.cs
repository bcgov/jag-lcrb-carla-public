using Gov.Lclb.Cllb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Rest;
using Gov.Lclb.Cllb.Interfaces.Models;

namespace Gov.Lclb.Cllb.Public.Extensions
{
    public static class SharePointDocumentLocation
    {
        public static string DocumentLocationExistsWithCleanup(this IDynamicsClient _dynamicsClient, MicrosoftDynamicsCRMsharepointdocumentlocation mdcsdl)
        {
            var relativeUrl = mdcsdl.Relativeurl.Replace("'", "''");
            var filter = $"relativeurl eq '{relativeUrl}'";
            // start by getting the existing document locations.
            string result = null;
            try
            {
                var locations =
                    _dynamicsClient.Sharepointdocumentlocations.Get(filter: filter).Value.ToList();

                foreach (var location in locations)
                    if (string.IsNullOrEmpty(location._regardingobjectidValue))
                    {

                        Log.Error($"Orphan Sharepointdocumentlocation found.  ID is {location.Sharepointdocumentlocationid}");
                        // it is an invalid document location. cleanup.
                        try
                        {
                            _dynamicsClient.Sharepointdocumentlocations.Delete(location.Sharepointdocumentlocationid);
                        }
                        catch (HttpOperationException odee)
                        {

                        }
                    }
                    else
                    {
                        if (result != null)
                            Log.Error($"Duplicate Sharepointdocumentlocation found.  ID is {location.Sharepointdocumentlocationid}");
                        else
                            result = location.Sharepointdocumentlocationid;
                    }
            }
            catch (HttpOperationException odee)
            {
                Log.Error(odee, "Error getting SharepointDocumentLocations");
            }

            return result;

        }

        public static string CreateDocumentLocation(this IDynamicsClient _dynamicsClient, MicrosoftDynamicsCRMsharepointdocumentlocation mdcsdl)
        {

            try
            {
                mdcsdl = _dynamicsClient.Sharepointdocumentlocations.Create(mdcsdl);
            }
            catch (HttpOperationException odee)
            {
                Log.Error(odee, "Error creating SharepointDocumentLocation");
                mdcsdl = null;
            }

            return mdcsdl?.Sharepointdocumentlocationid;
        }

        /// <summary>
        /// Get a document location by reference
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        private static string GetDocumentLocationReferenceByRelativeURL(this IDynamicsClient _dynamicsClient, string relativeUrl)
        {
            string result = null;
            var sanitized = relativeUrl.Replace("'", "''");
            // first see if one exists.
            var locations = _dynamicsClient.Sharepointdocumentlocations.Get(filter: "relativeurl eq '" + sanitized + "'");

            var location = locations.Value.FirstOrDefault();

            if (location == null)
            {
                var newRecord = new MicrosoftDynamicsCRMsharepointdocumentlocation
                {
                    Relativeurl = relativeUrl
                };
                // create a new document location.
                try
                {
                    location = _dynamicsClient.Sharepointdocumentlocations.Create(newRecord);
                }
                catch (HttpOperationException httpOperationException)
                {
                    Log.Error(httpOperationException, "Error creating document location");
                }
            }

            if (location != null) result = location.Sharepointdocumentlocationid;

            return result;
        }

        /// <summary>
        /// Returns the SharePoint document Location for a given entity record
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public static string GetEntitySharePointDocumentLocation(this IDynamicsClient _dynamicsClient, string entityName, string entityId)
        {
            string result = null;
            var id = Guid.Parse(entityId);
            try
            {
                switch (entityName.ToLower())
                {
                    case "account":
                        var account = _dynamicsClient.GetAccountById(entityId);
                        var accountLocation = account.AccountSharepointDocumentLocation.FirstOrDefault();
                        if (accountLocation != null && !string.IsNullOrEmpty(accountLocation.Relativeurl)) result = accountLocation.Relativeurl;
                        break;
                    case "application":
                        var application = _dynamicsClient.GetApplicationByIdWithChildren(entityId).GetAwaiter().GetResult();
                        var applicationLocation = application.AdoxioApplicationSharePointDocumentLocations.FirstOrDefault();
                        if (applicationLocation != null && !string.IsNullOrEmpty(applicationLocation.Relativeurl)) result = applicationLocation.Relativeurl;
                        break;
                    case "contact":
                        var contact = _dynamicsClient.GetContactById(entityId).GetAwaiter().GetResult();
                        var contactLocation = contact.ContactSharePointDocumentLocations.FirstOrDefault();
                        if (contactLocation != null && !string.IsNullOrEmpty(contactLocation.Relativeurl)) result = contactLocation.Relativeurl;
                        break;
                    case "worker":
                        var worker = _dynamicsClient.GetWorkerByIdWithChildren(entityId).GetAwaiter().GetResult();
                        var workerLocation = worker.AdoxioWorkerSharePointDocumentLocations.FirstOrDefault();
                        if (workerLocation != null && !string.IsNullOrEmpty(workerLocation.Relativeurl)) result = workerLocation.Relativeurl;
                        break;
                    case "event":
                        var eventEntity = _dynamicsClient.GetEventByIdWithChildren(entityId);
                        var eventLocation = eventEntity.AdoxioEventSharePointDocumentLocations.FirstOrDefault();
                        if (eventLocation != null && !string.IsNullOrEmpty(eventLocation.Relativeurl)) result = eventLocation.Relativeurl;
                        break;
                }
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Returns the folder name for a given entity
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="_dynamicsClient"></param>
        /// <returns></returns>
        public static async Task<string> GetFolderName(this IDynamicsClient _dynamicsClient, string entityName, string entityId)
        {

            string folderName = null;

            folderName = _dynamicsClient.GetEntitySharePointDocumentLocation(entityName, entityId);

            if (folderName == null)
                switch (entityName.ToLower())
                {
                    case "account":
                        var account = await _dynamicsClient.GetAccountByIdAsync(Guid.Parse(entityId)).ConfigureAwait(true);
                        folderName = account.GetDocumentFolderName();
                        break;
                    case "application":
                        var application = await _dynamicsClient.GetApplicationById(Guid.Parse(entityId)).ConfigureAwait(true);
                        folderName = application.GetDocumentFolderName();
                        break;
                    case "contact":
                        var contact = await _dynamicsClient.GetContactById(Guid.Parse(entityId)).ConfigureAwait(true);
                        folderName = contact.GetDocumentFolderName();
                        break;
                    case "worker":
                        var worker = await _dynamicsClient.GetWorkerById(Guid.Parse(entityId)).ConfigureAwait(true);
                        folderName = worker.GetDocumentFolderName();
                        break;
                    case "event":
                        var eventEntity = _dynamicsClient.GetEventById(Guid.Parse(entityId));
                        folderName = eventEntity.GetDocumentFolderName();
                        break;
                }

            return folderName;
        }

        public static void CreateEntitySharePointDocumentLocation(this IDynamicsClient _dynamicsClient, string entityName, string entityId, string folderName, string name)
        {
            var id = Guid.Parse(entityId);
            switch (entityName.ToLower())
            {
                case "account":
                    var account = _dynamicsClient.GetAccountById(entityId);
                    _dynamicsClient.CreateAccountDocumentLocation(account, folderName, name);
                    break;
                case "application":
                    var application = _dynamicsClient.GetApplicationByIdWithChildren(entityId).GetAwaiter().GetResult();
                    _dynamicsClient.CreateApplicationDocumentLocation(application, folderName, name);
                    break;
                case "contact":
                    var contact = _dynamicsClient.GetContactById(entityId).GetAwaiter().GetResult();
                    _dynamicsClient.CreateContactDocumentLocation(contact, folderName, name);
                    break;
                case "worker":
                    var worker = _dynamicsClient.GetWorkerByIdWithChildren(entityId).GetAwaiter().GetResult();
                    _dynamicsClient.CreateWorkerDocumentLocation(worker, folderName, name);
                    break;
                case "event":
                    var eventEntity = _dynamicsClient.GetEventByIdWithChildren(entityId);
                    _dynamicsClient.CreateEventDocumentLocation(eventEntity, folderName, name);
                    break;
            }
        }

        private static async Task CreateAccountDocumentLocation(this IDynamicsClient _dynamicsClient, MicrosoftDynamicsCRMaccount account, string folderName, string name)
        {
            var parentDocumentLibraryReference = _dynamicsClient.GetDocumentLocationReferenceByRelativeURL("account");

            var accountUri = _dynamicsClient.GetEntityURI("accounts", account.Accountid);
            // now create a document location to link them.

            // Create the SharePointDocumentLocation entity
            var mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation
            {
                RegardingobjectIdAccountODataBind = accountUri,
                ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference),
                Relativeurl = folderName,
                Description = "Account Files",
                Name = name
            };


            var sharepointdocumentlocationid = _dynamicsClient.DocumentLocationExistsWithCleanup(mdcsdl);

            if (sharepointdocumentlocationid == null)
            {
                try
                {
                    mdcsdl = _dynamicsClient.Sharepointdocumentlocations.Create(mdcsdl);
                }
                catch (HttpOperationException odee)
                {
                    Log.Error(odee, "Error creating SharepointDocumentLocation");
                    mdcsdl = null;
                }
                if (mdcsdl != null)
                {

                    var sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", mdcsdl.Sharepointdocumentlocationid);

                    var oDataId = new Odataid
                    {
                        OdataidProperty = sharePointLocationData
                    };
                    try
                    {
                        _dynamicsClient.Accounts.AddReference(account.Accountid, "Account_SharepointDocumentLocation", oDataId);
                    }
                    catch (HttpOperationException odee)
                    {
                        Log.Error(odee, "Error adding reference to SharepointDocumentLocation");
                    }
                }
            }
        }

        private static void CreateApplicationDocumentLocation(this IDynamicsClient _dynamicsClient, MicrosoftDynamicsCRMadoxioApplication application, string folderName, string name)
        {

            // now create a document location to link them.
            var parentDocumentLibraryReference = _dynamicsClient.GetDocumentLocationReferenceByRelativeURL("adoxio_application");

            var regardingobjectId = _dynamicsClient.GetEntityURI("adoxio_applications", application.AdoxioApplicationid);

            // Create the SharePointDocumentLocation entity
            var mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation
            {
                RegardingobjectidAdoxioApplicationODataBind = regardingobjectId,
                ParentsiteorlocationSharepointdocumentlocationODataBind = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference),
                Relativeurl = folderName,
                Description = "Application Files",
                Name = name
            };

            var sharepointdocumentlocationid = _dynamicsClient.DocumentLocationExistsWithCleanup(mdcsdl);

            if (sharepointdocumentlocationid == null)
            {
                sharepointdocumentlocationid = _dynamicsClient.CreateDocumentLocation(mdcsdl);

                var sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations", sharepointdocumentlocationid);

                var oDataId = new Odataid
                {
                    OdataidProperty = sharePointLocationData
                };
                try
                {
                    _dynamicsClient.Applications.AddReference(application.AdoxioApplicationid, "adoxio_application_SharePointDocumentLocations", oDataId);
                }
                catch (HttpOperationException odee)
                {
                    Log.Error(odee, "Error adding reference to SharepointDocumentLocation");
                }
            }


        }

        private static void CreateContactDocumentLocation(this IDynamicsClient _dynamicsClient, MicrosoftDynamicsCRMcontact contact, string folderName, string name)
        {
            var parentDocumentLibraryReference = _dynamicsClient.GetDocumentLocationReferenceByRelativeURL("contact");

            var contactUri = _dynamicsClient.GetEntityURI("contacts", contact.Contactid);
            // now create a document location to link them.

            // Create the SharePointDocumentLocation entity
            var mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation
            {
                RegardingobjectIdContactODataBind = contactUri,
                ParentsiteorlocationSharepointdocumentlocationODataBind =
                    _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference),
                Relativeurl = folderName,
                Description = "Contact Files",
                Name = name
            };

            var sharepointdocumentlocationid = _dynamicsClient.DocumentLocationExistsWithCleanup(mdcsdl);

            if (sharepointdocumentlocationid == null)
            {
                try
                {
                    mdcsdl = _dynamicsClient.Sharepointdocumentlocations.Create(mdcsdl);
                }
                catch (HttpOperationException odee)
                {
                    Log.Error(odee, "Error creating SharepointDocumentLocation");
                    mdcsdl = null;
                }

                if (mdcsdl != null)
                {

                    var sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations",
                        mdcsdl.Sharepointdocumentlocationid);

                    var oDataId = new Odataid
                    {
                        OdataidProperty = sharePointLocationData
                    };
                    try
                    {
                        _dynamicsClient.Contacts.AddReference(contact.Contactid,
                            "adoxio_application_SharePointDocumentLocations", oDataId);
                    }
                    catch (HttpOperationException odee)
                    {
                        Log.Error(odee, "Error adding reference to SharepointDocumentLocation");
                    }

                }
            }
        }

        private static void CreateWorkerDocumentLocation(this IDynamicsClient _dynamicsClient, MicrosoftDynamicsCRMadoxioWorker worker, string folderName, string name)
        {
            // set the parent document library.
            var parentDocumentLibraryReference = _dynamicsClient.GetDocumentLocationReferenceByRelativeURL("adoxio_worker");

            var workerUri = _dynamicsClient.GetEntityURI("adoxio_workers", worker.AdoxioWorkerid);
            // now create a document location to link them.

            // Create the SharePointDocumentLocation entity
            var mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation
            {
                RegardingobjectidWorkerApplicationODataBind = workerUri,
                ParentsiteorlocationSharepointdocumentlocationODataBind =
                    _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference),
                Relativeurl = folderName,
                Description = "Worker Files",
                Name = name
            };

            var sharepointdocumentlocationid = _dynamicsClient.DocumentLocationExistsWithCleanup(mdcsdl);

            if (sharepointdocumentlocationid == null)
            {

                try
                {
                    mdcsdl = _dynamicsClient.Sharepointdocumentlocations.Create(mdcsdl);
                }
                catch (HttpOperationException odee)
                {
                    Log.Error(odee, "Error creating SharepointDocumentLocation");
                    mdcsdl = null;
                }

                if (mdcsdl != null)
                {

                    var sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations",
                        mdcsdl.Sharepointdocumentlocationid);

                    var oDataId = new Odataid
                    {
                        OdataidProperty = sharePointLocationData
                    };
                    try
                    {
                        _dynamicsClient.Workers.AddReference(worker.AdoxioWorkerid,
                            "adoxio_worker_SharePointDocumentLocations", oDataId);
                    }
                    catch (HttpOperationException odee)
                    {
                        Log.Error(odee, "Error adding reference to SharepointDocumentLocation");
                    }
                }
            }
        }

        private static void CreateEventDocumentLocation(this IDynamicsClient _dynamicsClient, MicrosoftDynamicsCRMadoxioEvent eventEntity, string folderName, string name)
        {
            // set the parent document library.
            var parentDocumentLibraryReference = _dynamicsClient.GetDocumentLocationReferenceByRelativeURL("adoxio_event");

            var eventUri = _dynamicsClient.GetEntityURI("adoxio_events", eventEntity.AdoxioEventid);
            // now create a document location to link them.

            // Create the SharePointDocumentLocation entity
            var mdcsdl = new MicrosoftDynamicsCRMsharepointdocumentlocation
            {
                RegardingobjectIdEventODataBind = eventUri,
                ParentsiteorlocationSharepointdocumentlocationODataBind =
                    _dynamicsClient.GetEntityURI("sharepointdocumentlocations", parentDocumentLibraryReference),
                Relativeurl = folderName,
                Description = "Event Files",
                Name = name
            };

            var sharepointdocumentlocationid = _dynamicsClient.DocumentLocationExistsWithCleanup(mdcsdl);

            if (sharepointdocumentlocationid == null)
            {
                try
                {
                    mdcsdl = _dynamicsClient.Sharepointdocumentlocations.Create(mdcsdl);
                }
                catch (HttpOperationException odee)
                {
                    Log.Error(odee, "Error creating SharepointDocumentLocation");
                    mdcsdl = null;
                }

                if (mdcsdl != null)
                {
                    var sharePointLocationData = _dynamicsClient.GetEntityURI("sharepointdocumentlocations",
                        mdcsdl.Sharepointdocumentlocationid);

                    var oDataId = new Odataid
                    {
                        OdataidProperty = sharePointLocationData
                    };
                    try
                    {
                        _dynamicsClient.Events.AddReference(eventEntity.AdoxioEventid,
                            "adoxio_event_SharePointDocumentLocations", oDataId);
                    }
                    catch (HttpOperationException odee)
                    {
                        Log.Error(odee, "Error adding reference to SharepointDocumentLocation");
                    }
                }
            }
        }

    }
}
