using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTool
{
    class Importer
    {
        private readonly Dictionary<string, string> ContactMap = new Dictionary<string, string>();
        private readonly Dictionary<string, string> AccountMap = new Dictionary<string, string>();
        private readonly Dictionary<string, string> WorkerMap = new Dictionary<string, string>();
        private readonly Dictionary<string, string> AliasMap = new Dictionary<string, string>();
        private readonly Dictionary<string, string> InvoiceMap = new Dictionary<string, string>();
        private readonly Dictionary<string, string> LicenceMap = new Dictionary<string, string>();
        private readonly Dictionary<string, string> ApplicationMap = new Dictionary<string, string>();
        private readonly Dictionary<string, string> EstablishmentMap = new Dictionary<string, string>();
        private readonly Dictionary<string, string> LegalEntityMap = new Dictionary<string, string>();
        private readonly Dictionary<string, string> LocalgovindigenousnationMap = new Dictionary<string, string>();




        public Importer(Dictionary<string, string> contactMap,
            Dictionary<string, string> accountMap,
            Dictionary<string, string> workerMap,
            Dictionary<string, string> aliasMap,
            Dictionary<string, string> invoiceMap,
            Dictionary<string, string> licenceMap,
            Dictionary<string, string> applicationMap,
            Dictionary<string, string> establishmentMap,
            Dictionary<string, string> legalEntityMap,
            Dictionary<string, string> localgovindigenousnation
            )
        {
            ContactMap = contactMap;
            AccountMap = accountMap;
            WorkerMap = workerMap;
            AliasMap = aliasMap;
            InvoiceMap = invoiceMap;
            LicenceMap = licenceMap;
            ApplicationMap = applicationMap;
            EstablishmentMap = establishmentMap;
            LegalEntityMap = legalEntityMap;
            LocalgovindigenousnationMap = localgovindigenousnation;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="contact"></param>
        /// <returns>contact.contactid is set with the </returns>
        static bool ContactsNotFound(List<MicrosoftDynamicsCRMcontact> contacts, MicrosoftDynamicsCRMcontact contact)
        {
            bool notFound = true;
            foreach (var item in contacts)
            {
                if (item != null)
                {
                    if (item.Firstname == contact.Firstname
                        && item.Lastname == contact.Lastname && item.Emailaddress1 == contact.Emailaddress1)
                    {
                        notFound = false;
                        contact.Contactid = item.Contactid;
                        break;
                    }
                }
            }
            return notFound;
        }

        void CreateContact(DynamicsClient _dynamicsClient, MicrosoftDynamicsCRMcontact contact)
        {
            string accountId = null;

            if (contact.ParentcustomeridAccount != null)
            {
                accountId = contact.ParentcustomeridAccount.Accountid;
            }

            // we need to ensure that just the core fields are sent during the initial create, not sub objects

            var newItem = new MicrosoftDynamicsCRMcontact()
            {
                Contactid = null,
                Fullname = contact.Fullname,
                Firstname = contact.Firstname,
                Lastname = contact.Lastname,
                Emailaddress1 = contact.Emailaddress1,
                Telephone1 = contact.Telephone1,
                Address1Name = contact.Address1Name,
                Address1Line1 = contact.Address1Line1,
                Address1City = contact.Address1City,
                Address1Country = contact.Address1Country,
                Address1Postalcode = contact.Address1Postalcode,
                Address2Name = contact.Address2Name,
                Address2Line1 = contact.Address2Line1,
                Address2City = contact.Address2City,
                Address2Country = contact.Address2Country,
                Address2Postalcode = contact.Address2Postalcode,
                AdoxioCanattendcompliancemeetings = contact.AdoxioCanattendcompliancemeetings,
                AdoxioCanobtainlicenceinfofrombranch = contact.AdoxioCanobtainlicenceinfofrombranch,
                AdoxioCanrepresentlicenseeathearings = contact.AdoxioCanrepresentlicenseeathearings,
                AdoxioCansigngrocerystoreproofofsalesrevenue = contact.AdoxioCansigngrocerystoreproofofsalesrevenue,
                AdoxioCansignpermanentchangeapplications = contact.AdoxioCansignpermanentchangeapplications,
                AdoxioCansigntemporarychangeapplications = contact.AdoxioCansigntemporarychangeapplications
            };

            contact.Contactid = null;
            try
            {
                contact = _dynamicsClient.Contacts.Create(newItem);
                Console.Out.WriteLine("created contact " + contact.Firstname);
            }
            catch (OdataerrorException odee)
            {
                contact.Contactid = _dynamicsClient.GetCreatedRecord(odee, "Error creating contact");
            }

            // set the business association
            if (accountId != null)
            {
                // parent customer id relationship will be created using the method here:
                //https://msdn.microsoft.com/en-us/library/mt607875.aspx
                MicrosoftDynamicsCRMcontact patchUserContact = new MicrosoftDynamicsCRMcontact()
                {
                    ParentCustomerIdAccountODataBind = _dynamicsClient.GetEntityURI("accounts", AccountMap[accountId])
                };
                try
                {
                    _dynamicsClient.Contacts.Update(contact.Contactid, patchUserContact);
                }
                catch (OdataerrorException odee)
                {
                    //_logger.LogError(LoggingEvents.Error, "Error binding contact to account");
                    //_logger.LogError("Request:");
                    Console.Out.WriteLine(odee.Request.Content);
                    //_logger.LogError("Response:");
                    Console.Out.WriteLine(odee.Response.Content);
                    throw new OdataerrorException("Error binding contact to account");
                }
            }
        }

        static List<MicrosoftDynamicsCRMcontact> GetCurrentContacts(DynamicsClient _dynamicsClient)
        {
            List<string> expand = new List<string>()
            {
                "parentcustomerid_account"
            };
            var data = _dynamicsClient.Contacts.Get(expand: expand);
            return (List<MicrosoftDynamicsCRMcontact>)data.Value;
        }

        public void ImportContacts(DynamicsClient _dynamicsClient, List<MicrosoftDynamicsCRMcontact> contacts)
        {
            var currentContacts = GetCurrentContacts(_dynamicsClient);

            foreach (var contact in contacts)
            {
                string originalKey = contact.Contactid;
                if (ContactsNotFound(currentContacts, contact))
                {
                    CreateContact(_dynamicsClient, contact);
                }
                else
                {
                    Console.WriteLine("Contact found " + contact.Firstname + " " + contact.Lastname);
                }
                ContactMap.Add(originalKey, contact.Contactid);
                Console.Out.Write(".");
            }
            Console.Out.WriteLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="contact"></param>
        /// <returns>contact.contactid is set with the </returns>
        bool AccountNotFound(List<MicrosoftDynamicsCRMaccount> accounts, MicrosoftDynamicsCRMaccount account)
        {
            bool notFound = true;
            foreach (var item in accounts)
            {
                if (item != null)
                {
                    if (item.Name == account.Name)
                    {
                        notFound = false;
                        account.Accountid = item.Accountid;
                        break;
                    }
                }
            }
            return notFound;
        }

        void CreateAccount(DynamicsClient _dynamicsClient, MicrosoftDynamicsCRMaccount account)
        {
            MicrosoftDynamicsCRMaccount newItem = new MicrosoftDynamicsCRMaccount()
            {
                Name = account.Name,
                AdoxioBusinesstype = account.AdoxioBusinesstype,
                Description = account.Description,
                AdoxioAccounttype = account.AdoxioAccounttype,
                AdoxioExternalid = account.AdoxioExternalid,
                AdoxioBcincorporationnumber = account.AdoxioBcincorporationnumber,
                AdoxioDateofincorporationinbc = account.AdoxioDateofincorporationinbc,
                Accountnumber = account.Accountnumber,
                AdoxioPstnumber = account.AdoxioPstnumber,
                Emailaddress1 = account.Emailaddress1,
                Telephone1 = account.Telephone1,

                Address1Name = account.Address1Name,
                Address1Line1 = account.Address1Line1,
                Address1Line2 = account.Address1Line2,
                Address1City = account.Address1City,
                Address1Country = account.Address1Country,
                Address1Stateorprovince = account.Address1Stateorprovince,
                Address1Postalcode = account.Address1Postalcode,

                Address2Name = account.Address2Name,
                Address2Line1 = account.Address2Line1,
                Address2Line2 = account.Address2Line2,
                Address2City = account.Address2City,
                Address2Country = account.Address2Country,
                Address2Stateorprovince = account.Address2Stateorprovince,
                Address2Postalcode = account.Address2Postalcode
            };
            account.Accountid = null;
            try
            {
                account = _dynamicsClient.Accounts.Create(newItem);
                Console.Out.WriteLine("created account " + account.Name);
            }
            catch (OdataerrorException odee)
            {
                account.Accountid = _dynamicsClient.GetCreatedRecord(odee, "Error creating account");
            }
        }

        List<MicrosoftDynamicsCRMaccount> GetCurrentAccounts(DynamicsClient _dynamicsClient)
        {
            var data = _dynamicsClient.Accounts.Get();
            return (List<MicrosoftDynamicsCRMaccount>)data.Value;
        }

        public void ImportAccounts(DynamicsClient _dynamicsClient, List<MicrosoftDynamicsCRMaccount> accounts)
        {
            var currentAccounts = GetCurrentAccounts(_dynamicsClient);

            foreach (var account in accounts)
            {
                string originalKey = account.Accountid;
                if (AccountNotFound(currentAccounts, account))
                {
                    CreateAccount(_dynamicsClient, account);
                }
                else
                {
                    Console.WriteLine("Account found " + account.Name);
                }
                if (account.Accountid != null)
                {
                    AccountMap.Add(originalKey, account.Accountid);
                }
                Console.Out.Write(".");
            }
            Console.Out.WriteLine();

        }

        /***
         * Workers
         ***/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="contact"></param>
        /// <returns>contact.contactid is set with the </returns>
        bool WorkerNotFound(List<MicrosoftDynamicsCRMadoxioWorker> workers, MicrosoftDynamicsCRMadoxioWorker worker)
        {
            bool notFound = true;
            foreach (var item in workers)
            {
                if (item != null)
                {
                    if (
                         item.AdoxioLastname == worker.AdoxioLastname
                        && item.AdoxioFirstname == worker.AdoxioFirstname
                        && item.AdoxioEmail == worker.AdoxioEmail
                        )

                    {
                        notFound = false;
                        worker.AdoxioWorkerid = item.AdoxioWorkerid;
                        break;
                    }
                }
            }
            return notFound;
        }

        void CreateWorker(DynamicsClient _dynamicsClient, MicrosoftDynamicsCRMadoxioWorker worker)
        {
            worker.AdoxioWorkerid = null;
            MicrosoftDynamicsCRMadoxioWorker newItem = new MicrosoftDynamicsCRMadoxioWorker()
            {
                AdoxioIsldbworker = worker.AdoxioIsldbworker,
                AdoxioFirstname = worker.AdoxioFirstname,
                AdoxioMiddlename = worker.AdoxioMiddlename,
                AdoxioLastname = worker.AdoxioLastname,
                AdoxioDateofbirth = worker.AdoxioDateofbirth,
                AdoxioGendercode = worker.AdoxioGendercode,
                Statuscode = worker.Statuscode,
                AdoxioBirthplace = worker.AdoxioBirthplace,
                AdoxioDriverslicencenumber = worker.AdoxioDriverslicencenumber,
                AdoxioBcidcardnumber = worker.AdoxioBcidcardnumber,
                AdoxioSelfdisclosure = worker.AdoxioSelfdisclosure,
                AdoxioPaymentreceived = worker.AdoxioPaymentreceived,
                AdoxioPaymentreceiveddate = worker.AdoxioPaymentreceiveddate,
                AdoxioWorkerid = worker.AdoxioWorkerid,
                AdoxioCurrentaddressdatefrom = worker.AdoxioCurrentaddressdatefrom
            };



            try
            {
                worker = _dynamicsClient.Workers.Create(newItem);
                Console.Out.WriteLine("created worker " + newItem.AdoxioFirstname + " " + newItem.AdoxioLastname);
            }
            catch (OdataerrorException odee)
            {
                worker.AdoxioWorkerid = _dynamicsClient.GetCreatedRecord(odee, "Error creating worker");
            }
        }

        List<MicrosoftDynamicsCRMadoxioWorker> GetCurrentWorkers(DynamicsClient _dynamicsClient)
        {
            var data = _dynamicsClient.Workers.Get();
            return (List<MicrosoftDynamicsCRMadoxioWorker>)data.Value;
        }

        public void ImportWorkers(DynamicsClient _dynamicsClient, List<MicrosoftDynamicsCRMadoxioWorker> workers)
        {
            var currentWorkers = GetCurrentWorkers(_dynamicsClient);

            foreach (var worker in workers)
            {
                //alias.AdoxioContactId.Contactid = ContactMap[alias.AdoxioContactId.Contactid];
                string originalKey = worker.AdoxioWorkerid;
                if (WorkerNotFound(currentWorkers, worker))
                {
                    CreateWorker(_dynamicsClient, worker);
                }
                else
                {
                    Console.WriteLine("Worker found " + worker.AdoxioName);
                }
                WorkerMap.Add(originalKey, worker.AdoxioWorkerid);
                Console.Out.Write(".");
            }
            Console.Out.WriteLine();
        }

        /***
         * Alias
         ***/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        bool AliasNotFound(List<MicrosoftDynamicsCRMadoxioAlias> aliases, MicrosoftDynamicsCRMadoxioAlias alias)
        {
            bool notFound = true;
            foreach (var item in aliases)
            {
                if (item != null)
                {
                    if (
                            item.AdoxioName == alias.AdoxioName
                            && (item.AdoxioContactId == null || (item.AdoxioContactId != null && alias.AdoxioContactId.Contactid != null && item.AdoxioContactId.Contactid == ContactMap[alias.AdoxioContactId.Contactid]))
                            && (item.AdoxioWorkerId == null || (item.AdoxioWorkerId != null && alias.AdoxioWorkerId.AdoxioWorkerid != null && item.AdoxioWorkerId.AdoxioWorkerid == WorkerMap[alias.AdoxioWorkerId.AdoxioWorkerid]))
                       )
                    {
                        notFound = false;
                        alias.AdoxioAliasid = item.AdoxioAliasid;
                        break;
                    }
                }
            }
            return notFound;
        }

        void CreateAlias(DynamicsClient _dynamicsClient, MicrosoftDynamicsCRMadoxioAlias alias)
        {
            string contactId = null;
            string workerId = null;

            if (alias.AdoxioContactId != null)
            {
                contactId = alias.AdoxioContactId.Contactid;
            }

            if (alias.AdoxioWorkerId != null)
            {
                workerId = alias.AdoxioWorkerId.AdoxioWorkerid;
            }

            MicrosoftDynamicsCRMadoxioAlias newAlias = new MicrosoftDynamicsCRMadoxioAlias()
            {
                AdoxioFirstname = alias.AdoxioFirstname,
                AdoxioMiddlename = alias.AdoxioMiddlename,
                AdoxioLastname = alias.AdoxioLastname
            };

            alias.AdoxioAliasid = null;
            try
            {
                alias = _dynamicsClient.Aliases.Create(newAlias);
                Console.Out.WriteLine("created alias " + alias.AdoxioName);
            }
            catch (OdataerrorException odee)
            {
                alias.AdoxioAliasid = _dynamicsClient.GetCreatedRecord(odee, "Error creating contact");
            }
            if (contactId != null)
            {
                var patchAlias = new MicrosoftDynamicsCRMadoxioAlias()
                {
                    ContactIdODataBind = _dynamicsClient.GetEntityURI("contacts", ContactMap[contactId])
                };
                try
                {
                    _dynamicsClient.Aliases.Update(newAlias.AdoxioAliasid, patchAlias);
                }
                catch (OdataerrorException odee)
                {
                    Console.WriteLine("Error updating alias");
                    Console.WriteLine(odee.Message);
                    Console.WriteLine(odee.Request.Content);
                    Console.WriteLine(odee.Response.Content);
                }
            }
        }


        List<MicrosoftDynamicsCRMadoxioAlias> GetCurrentAliases(DynamicsClient _dynamicsClient)
        {
            List<string> expand = new List<string>()
            {
                "adoxio_ContactId","adoxio_WorkerId"
            };
            var data = _dynamicsClient.Aliases.Get(expand: expand);
            return (List<MicrosoftDynamicsCRMadoxioAlias>)data.Value;
        }

        public void ImportAliases(DynamicsClient _dynamicsClient, List<MicrosoftDynamicsCRMadoxioAlias> aliases)
        {
            var currentAliases = GetCurrentAliases(_dynamicsClient);

            foreach (var alias in aliases)
            {
                if (alias.AdoxioContactId != null && alias.AdoxioContactId.Contactid != null)
                {
                    alias.AdoxioContactId.Contactid = ContactMap[alias.AdoxioContactId.Contactid];
                }
                if (alias.AdoxioWorkerId != null && alias.AdoxioWorkerId.AdoxioWorkerid != null)
                {
                    alias.AdoxioWorkerId.AdoxioWorkerid = WorkerMap[alias.AdoxioWorkerId.AdoxioWorkerid];
                }
                string originalKey = alias.AdoxioAliasid;
                if (AliasNotFound(currentAliases, alias))
                {
                    CreateAlias(_dynamicsClient, alias);
                }
                else
                {
                    Console.WriteLine("Alias found " + alias.AdoxioName);
                }
                AliasMap.Add(originalKey, alias.AdoxioAliasid);
                Console.Out.Write(".");
            }
            Console.Out.WriteLine();
        }


        /***
         * Local government
         ***/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="contact"></param>
        /// <returns>contact.contactid is set with the </returns>
        bool LocalGovIndigenousNationNotFound(List<MicrosoftDynamicsCRMadoxioLocalgovindigenousnation> LocalGovIndigenousNations, MicrosoftDynamicsCRMadoxioLocalgovindigenousnation localGovIndigenousNation)
        {
            bool notFound = true;
            foreach (var item in LocalGovIndigenousNations)
            {
                if (item != null)
                {
                    if (
                            item.AdoxioName == localGovIndigenousNation.AdoxioName                            
                       )
                    {
                        notFound = false;
                        localGovIndigenousNation.AdoxioLocalgovindigenousnationid = item.AdoxioLocalgovindigenousnationid;
                        break;
                    }
                }
            }
            return notFound;
        }

        void CreateLocalGovIndigenousNation(DynamicsClient _dynamicsClient, MicrosoftDynamicsCRMadoxioLocalgovindigenousnation localGovIndigenousNation)
        {
            string contactId = null;
            string workerId = null;

            MicrosoftDynamicsCRMadoxioLocalgovindigenousnation newLocalGovIndigenousNation = new MicrosoftDynamicsCRMadoxioLocalgovindigenousnation()
            {
                AdoxioName = localGovIndigenousNation.AdoxioName,
                AdoxioCommunicationsregion = localGovIndigenousNation.AdoxioCommunicationsregion,
                AdoxioIssuingcannabislicences = localGovIndigenousNation.AdoxioIssuingcannabislicences
            };

            localGovIndigenousNation.AdoxioLocalgovindigenousnationid = null;
            try
            {
                localGovIndigenousNation = _dynamicsClient.Localgovindigenousnations.Create(newLocalGovIndigenousNation);
                Console.Out.WriteLine("created LocalGovIndigenousNation " + localGovIndigenousNation.AdoxioName);
            }
            catch (OdataerrorException odee)
            {
                localGovIndigenousNation.AdoxioLocalgovindigenousnationid = _dynamicsClient.GetCreatedRecord(odee, "Error creating lgin record");
            }

        }


        List<MicrosoftDynamicsCRMadoxioLocalgovindigenousnation> GetCurrentLocalGovIndigenousNations(DynamicsClient _dynamicsClient)
        {
            List<string> expand = new List<string>()
            {
                "adoxio_ContactId","adoxio_WorkerId"
            };
            var data = _dynamicsClient.Localgovindigenousnations.Get(); // expand: expand
            return (List<MicrosoftDynamicsCRMadoxioLocalgovindigenousnation>)data.Value;
        }

        public void ImportLocalGovIndigenousNations(DynamicsClient _dynamicsClient, List<MicrosoftDynamicsCRMadoxioLocalgovindigenousnation> localGovIndigenousNations)
        {
            var currentLocalGovIndigenousNations = GetCurrentLocalGovIndigenousNations(_dynamicsClient);

            foreach (var localGovIndigenousNation in localGovIndigenousNations)
            {
                string originalKey = localGovIndigenousNation.AdoxioLocalgovindigenousnationid;
                if (LocalGovIndigenousNationNotFound(currentLocalGovIndigenousNations, localGovIndigenousNation))
                {
                    CreateLocalGovIndigenousNation(_dynamicsClient, localGovIndigenousNation);
                }
                else
                {
                    Console.WriteLine("LocalGovIndigenousNation found " + localGovIndigenousNation.AdoxioName);
                }
                LocalgovindigenousnationMap.Add(originalKey, localGovIndigenousNation.AdoxioLocalgovindigenousnationid);
                Console.Out.Write(".");
            }
            Console.Out.WriteLine();
        }


        /***
         * Applications
         ***/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="contact"></param>
        /// <returns>contact.contactid is set with the </returns>
        bool ApplicationNotFound(List<MicrosoftDynamicsCRMadoxioApplication> applications, MicrosoftDynamicsCRMadoxioApplication application)
        {
            bool notFound = true;
            foreach (var item in applications)
            {
                if (item != null)
                {
                    if (item.AdoxioName == application.AdoxioName && item.AdoxioNameofapplicant == application.AdoxioNameofapplicant)
                    {
                        notFound = false;
                        application.AdoxioApplicationid = item.AdoxioApplicationid;
                        break;
                    }
                }
            }
            return notFound;
        }

        void CreateApplication(DynamicsClient _dynamicsClient, MicrosoftDynamicsCRMadoxioApplication application)
        {
            MicrosoftDynamicsCRMadoxioApplication createdItem = null;
            application.AdoxioApplicationid = null;
            MicrosoftDynamicsCRMadoxioApplication newItem = new MicrosoftDynamicsCRMadoxioApplication()
            {
                AdoxioApplicanttype = application.AdoxioApplicanttype,
                AdoxioAddresscity = application.AdoxioAddresscity,
                AdoxioAddresscountry = application.AdoxioAddresscountry,
                AdoxioAddresspostalcode = application.AdoxioAddresspostalcode,
                AdoxioAddressprovince = application.AdoxioAddressprovince,
                AdoxioAddressstreet = application.AdoxioAddressstreet,

                AdoxioName = application.AdoxioName,
                AdoxioJobnumber = application.AdoxioJobnumber,
                AdoxioEstablishmentpropsedname = application.AdoxioEstablishmentpropsedname,
                AdoxioEstablishmentaddressstreet = application.AdoxioEstablishmentaddressstreet,
                AdoxioEstablishmentaddresscity = application.AdoxioEstablishmentaddresscity,
                AdoxioEstablishmentaddresscountry = application.AdoxioEstablishmentaddresscountry,

                AdoxioEstablishmentaddresspostalcode = application.AdoxioEstablishmentaddresspostalcode,
                AdoxioEstablishmentparcelid = application.AdoxioEstablishmentparcelid,
                AdoxioLicencefeeinvoicepaid = application.AdoxioLicencefeeinvoicepaid,
                Statuscode = application.Statuscode,

                AdoxioAppchecklistapplicantsentemail = application.AdoxioAppchecklistapplicantsentemail,
                AdoxioAppchecklistlgfnapproved = application.AdoxioAppchecklistlgfnapproved,
                AdoxioAppchecklistlicenceissued = application.AdoxioAppchecklistlicenceissued,
                AdoxioAppchecklistlicencehistoryapproval = application.AdoxioAppchecklistlicencehistoryapproval,
                AdoxioHoldsotherlicencesoptionset = application.AdoxioHoldsotherlicencesoptionset,
                AdoxioAppchecklistlgfnnotified = application.AdoxioAppchecklistlgfnnotified,
                AdoxioAppchecklistcasemanageraipdecision = application.AdoxioAppchecklistcasemanageraipdecision,
                AdoxioAreyouthemaincontactforapplication = application.AdoxioAreyouthemaincontactforapplication,
                AdoxioAppchecklistlicencefeecollected = application.AdoxioAppchecklistlicencefeecollected,
                AdoxioIsapplicationcomplete = application.AdoxioIsapplicationcomplete,
                AdoxioChecklistassociateformreceived = application.AdoxioChecklistassociateformreceived,
                AdoxioAppchecklistvalidinterestsubmitted = application.AdoxioAppchecklistvalidinterestsubmitted,
                AdoxioAppchecklisttermsconditionsapplied = application.AdoxioAppchecklisttermsconditionsapplied,
                AdoxioAppchecklistdocumentssubmitted = application.AdoxioAppchecklistdocumentssubmitted,
                AdoxioAppchecklistfinalreview = application.AdoxioAppchecklistfinalreview,
                AdoxioAppchecklistinspectionresults = application.AdoxioAppchecklistinspectionresults,
                AdoxioDatefirstyearpaymentreceived = application.AdoxioDatefirstyearpaymentreceived,
                AdoxioPolicedecision = application.AdoxioPolicedecision,
                AdoxioAppchecklistlicencehistorycheck = application.AdoxioAppchecklistlicencehistorycheck,
                AdoxioAppchecklistinvestigationsapproved = application.AdoxioAppchecklistinvestigationsapproved,
                AdoxioEstablishmentotherbusinessname = application.AdoxioEstablishmentotherbusinessname,
                AdoxioAppchecklistfinaldecisionlettersent = application.AdoxioAppchecklistfinaldecisionlettersent,
                AdoxioUploadedfloorplans = application.AdoxioUploadedfloorplans,
                AdoxioChecklistvalidinterest = application.AdoxioChecklistvalidinterest,
                AdoxioAppchecklistfinancialintegrityapproved = application.AdoxioAppchecklistfinancialintegrityapproved,
                AdoxioSignaturedate = application.AdoxioSignaturedate,
                AdoxioChecklisttiedhouseassessed = application.AdoxioChecklisttiedhouseassessed,
                AdoxioChecklistspdconsentreceived = application.AdoxioChecklistspdconsentreceived,
                AdoxioAreyouthemaincontactafterlicensing = application.AdoxioAreyouthemaincontactafterlicensing,
                AdoxioAppchecklistcasesupervisorapproved = application.AdoxioAppchecklistcasesupervisorapproved,
                AdoxioAppchecklistzoningconfirmed = application.AdoxioAppchecklistzoningconfirmed,
                AdoxioAppchecklistspdapproved = application.AdoxioAppchecklistspdapproved,
                AdoxioAppchecklisttermsconditionsdefined = application.AdoxioAppchecklisttermsconditionsdefined,
                AdoxioAppchecklistinspectionnotesreviewed = application.AdoxioAppchecklistinspectionnotesreviewed,
                AdoxioAppchecklistvalidinterestreceivedfinal = application.AdoxioAppchecklistvalidinterestreceivedfinal,
                AdoxioAppchecklistsentforfi = application.AdoxioAppchecklistsentforfi,
                AdoxioDateaip = application.AdoxioDateaip,
                AdoxioChecklistverifypostalcode = application.AdoxioChecklistverifypostalcode,
                AdoxioAppchecklistspdconsentsubmitted = application.AdoxioAppchecklistspdconsentsubmitted,
                AdoxioChecklistfloorplanapproved = application.AdoxioChecklistfloorplanapproved,
                AdoxioTerminatereason = application.AdoxioTerminatereason,
                AdoxioChecklistfloorplanassessed = application.AdoxioChecklistfloorplanassessed,
                AdoxioAppchecklistapplicationcomplete = application.AdoxioAppchecklistapplicationcomplete,
                AdoxioDatereceivedinvestigations = application.AdoxioDatereceivedinvestigations,
                AdoxioEstablishmentlocatedatwinery = application.AdoxioEstablishmentlocatedatwinery,
                AdoxioAppchecklistsitemapsubmitted = application.AdoxioAppchecklistsitemapsubmitted,
                AdoxioChecklistsitemapreceived = application.AdoxioChecklistsitemapreceived,
                AdoxioAppchecklistsitemapapproved = application.AdoxioAppchecklistsitemapapproved,
                AdoxioDateaipexpired = application.AdoxioDateaipexpired,
                AdoxioAppchecklistfinalreviewcomplete = application.AdoxioAppchecklistfinalreviewcomplete,
                AdoxioDatereceivedlgin = application.AdoxioDatereceivedlgin,
                AdoxioChecklistbrandingassessed = application.AdoxioChecklistbrandingassessed,
                AdoxioAppchecklistshareholdersverified = application.AdoxioAppchecklistshareholdersverified,
                AdoxioAppchecklistverifyapplication = application.AdoxioAppchecklistverifyapplication,
                AdoxioLicenceexpiry = application.AdoxioLicenceexpiry,
                AdoxioAppchecklistlginapproval = application.AdoxioAppchecklistlginapproval,
                AdoxioMarketshareevaluation = application.AdoxioMarketshareevaluation,
                AdoxioAppchecklistverifybusinessprofile = application.AdoxioAppchecklistverifybusinessprofile,
                AdoxioChecklistlicencefeepaid = application.AdoxioChecklistlicencefeepaid,
                AdoxioOtherbusinessesatthesamelocation = application.AdoxioOtherbusinessesatthesamelocation,
                AdoxioDateassignedtosla = application.AdoxioDateassignedtosla,
                AdoxioAppchecklistfinalfloorplan = application.AdoxioAppchecklistfinalfloorplan,
                AdoxioEstablishmentlayoutapplyingforpatio = application.AdoxioEstablishmentlayoutapplyingforpatio,
                AdoxioAppchecklistbusinessproposal = application.AdoxioAppchecklistbusinessproposal,
                AdoxioEstablishmentlocatedatfirstnationland = application.AdoxioEstablishmentlocatedatfirstnationland,
                AdoxioChecklisttermsconditionsadded = application.AdoxioChecklisttermsconditionsadded,
                AdoxioAppchecklistinspectionreviewcomplete = application.AdoxioAppchecklistinspectionreviewcomplete,
                AdoxioAppchecklistzoningaip = application.AdoxioAppchecklistzoningaip,
                AdoxioAppchecklistfinancialintegritysubmitted = application.AdoxioAppchecklistfinancialintegritysubmitted,
                AdoxioEstablishmentdeclarationoption1 = application.AdoxioEstablishmentdeclarationoption1,
                AdoxioAppchecklistmarketcapacity = application.AdoxioAppchecklistmarketcapacity,
                AdoxioChecklistsenttolgin = application.AdoxioChecklistsenttolgin,
                AdoxioHastiedhouseassociations = application.AdoxioHastiedhouseassociations,
                AdoxioEstablishmentcomplytoallbylaws = application.AdoxioEstablishmentcomplytoallbylaws,
                AdoxioChecklistvalidinterestassessed = application.AdoxioChecklistvalidinterestassessed,
                AdoxioCasemanagerassigned = application.AdoxioCasemanagerassigned,
                AdoxioAppchecklistinspectionchangesneeded = application.AdoxioAppchecklistinspectionchangesneeded,
                

                AdoxioAppchecklistfinaldecision = application.AdoxioAppchecklistfinaldecision,
                AdoxioPaymentrecieved = application.AdoxioPaymentrecieved,
                AdoxioPaymentreceiveddate = application.AdoxioPaymentreceiveddate,

                AdoxioProgressstatus = application.AdoxioProgressstatus,
                AdoxioContactpersonfirstname = application.AdoxioContactpersonfirstname,
                AdoxioContactpersonlastname = application.AdoxioContactpersonlastname,
                AdoxioRole = application.AdoxioRole,
                AdoxioEmail = application.AdoxioEmail,
                AdoxioContactpersonphone = application.AdoxioContactpersonphone,
                AdoxioAuthorizedtosubmit = application.AdoxioAuthorizedtosubmit,
                AdoxioSignatureagreement = application.AdoxioSignatureagreement,
                AdoxioAdditionalpropertyinformation = application.AdoxioAdditionalpropertyinformation



            };

            if (application.AdoxioEstablishmentaddressstreet != null && application.AdoxioEstablishmentaddressstreet.Length > 40)
            {
                application.AdoxioEstablishmentaddressstreet = application.AdoxioEstablishmentaddressstreet.Substring(0, 40);
            }



            if (application.AdoxioLicenceType != null)
            {
                newItem.AdoxioLicenceTypeODataBind = _dynamicsClient.GetEntityURI("adoxio_licencetypes", application.AdoxioLicenceType.AdoxioLicencetypeid);
            }
            if (application.AdoxioApplicant != null && AccountMap.ContainsKey(application.AdoxioApplicant.Accountid))
            {
                newItem.AdoxioApplicantODataBind = _dynamicsClient.GetEntityURI("accounts", AccountMap[application.AdoxioApplicant.Accountid]);
            }

            try
            {
                newItem = _dynamicsClient.Applications.Create(newItem);
                Console.Out.WriteLine("created application " + newItem.AdoxioName);
            }
            catch (OdataerrorException odee)
            {
                newItem.AdoxioApplicationid = _dynamicsClient.GetCreatedRecord(odee, "Error creating application");
            }

            // TODO add licence and invoice links.

            if (application.AdoxioInvoice != null && InvoiceMap.ContainsKey(application.AdoxioInvoice.Invoiceid))
            {
                MicrosoftDynamicsCRMadoxioApplication invoiceLinkItem = new MicrosoftDynamicsCRMadoxioApplication()
                {
                    AdoxioInvoiceODataBind = _dynamicsClient.GetEntityURI("invoices", InvoiceMap[application.AdoxioInvoice.Invoiceid])
                };

                try
                {
                    _dynamicsClient.Applications.Update(newItem.AdoxioApplicationid, invoiceLinkItem);
                }
                catch (OdataerrorException odee)
                {
                    Console.WriteLine("Error updating invoice for application");
                    Console.WriteLine(odee.Message);
                    Console.WriteLine(odee.Request.Content);
                    Console.WriteLine(odee.Response.Content);
                }
            }

            if (application.AdoxioLicenceFeeInvoice != null)
            {

                MicrosoftDynamicsCRMadoxioApplication invoiceLinkItem = new MicrosoftDynamicsCRMadoxioApplication()
                {
                    AdoxioLicenceFeeInvoiceODataBind = _dynamicsClient.GetEntityURI("invoices", InvoiceMap[application.AdoxioLicenceFeeInvoice.Invoiceid])
                };

                try
                {
                    _dynamicsClient.Applications.Update(newItem.AdoxioApplicationid, invoiceLinkItem);
                }
                catch (OdataerrorException odee)
                {
                    Console.WriteLine("Error updating licence invoice for application");
                    Console.WriteLine(odee.Message);
                    Console.WriteLine(odee.Request.Content);
                    Console.WriteLine(odee.Response.Content);
                }
            }

            if (application.AdoxioAssignedLicence != null && LicenceMap.ContainsKey(application.AdoxioAssignedLicence.AdoxioLicencesid))
            {
                MicrosoftDynamicsCRMadoxioApplication invoiceLinkItem = new MicrosoftDynamicsCRMadoxioApplication()
                {
                    AdoxioAssignedLicenceODataBind = _dynamicsClient.GetEntityURI("adoxio_licenceses", LicenceMap[application.AdoxioAssignedLicence.AdoxioLicencesid])
                };

                try
                {
                    _dynamicsClient.Applications.Update(newItem.AdoxioApplicationid, invoiceLinkItem);
                }
                catch (OdataerrorException odee)
                {
                    Console.WriteLine("Error updating assigned licence for application");
                    Console.WriteLine(odee.Message);
                    Console.WriteLine(odee.Request.Content);
                    Console.WriteLine(odee.Response.Content);
                }

            }

            if (application.AdoxioLocalgovindigenousnationid != null && application.AdoxioLocalgovindigenousnationid.AdoxioLocalgovindigenousnationid != null && LocalgovindigenousnationMap.ContainsKey(application.AdoxioLocalgovindigenousnationid.AdoxioLocalgovindigenousnationid))
            {
                MicrosoftDynamicsCRMadoxioApplication localGovLinkItem = new MicrosoftDynamicsCRMadoxioApplication()
                {
                    AdoxioLocalgovindigenousnationidODataBind = _dynamicsClient.GetEntityURI("adoxio_localgovindigenousnations", LocalgovindigenousnationMap[application.AdoxioLocalgovindigenousnationid.AdoxioLocalgovindigenousnationid])
                };

                try
                {
                    _dynamicsClient.Applications.Update(newItem.AdoxioApplicationid, localGovLinkItem);
                }
                catch (OdataerrorException odee)
                {
                    Console.WriteLine("Error updating local government for application");
                    Console.WriteLine(odee.Message);
                    Console.WriteLine(odee.Request.Content);
                    Console.WriteLine(odee.Response.Content);
                }

            }





        }

        List<MicrosoftDynamicsCRMadoxioApplication> GetCurrentApplications(DynamicsClient _dynamicsClient)
        {
            var data = _dynamicsClient.Applications.Get();
            return (List<MicrosoftDynamicsCRMadoxioApplication>)data.Value;
        }

        public void ImportApplications(DynamicsClient _dynamicsClient, List<MicrosoftDynamicsCRMadoxioApplication> applications)
        {
            var currentApplications = GetCurrentApplications(_dynamicsClient);

            foreach (var application in applications)
            {
                //alias.AdoxioContactId.Contactid = ContactMap[alias.AdoxioContactId.Contactid];
                string originalKey = application.AdoxioApplicationid;
                if (ApplicationNotFound(currentApplications, application))
                {
                    CreateApplication(_dynamicsClient, application);
                }
                else
                {
                    Console.WriteLine("Application found " + application.AdoxioName);
                }
                ApplicationMap.Add(originalKey, application.AdoxioApplicationid);
                Console.Out.Write(".");
            }

            Console.Out.WriteLine();
        }


        /***
        * Licences
        ***/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="contact"></param>
        /// <returns>contact.contactid is set with the </returns>
        bool LicenceNotFound(List<MicrosoftDynamicsCRMadoxioLicences> licences, MicrosoftDynamicsCRMadoxioLicences licence)
        {
            bool notFound = true;
            foreach (var item in licences)
            {
                if (item != null)
                {
                    if (
                            item.AdoxioName == licence.AdoxioName
                            && (item.AdoxioAccountId == null || (item.AdoxioAccountId != null && licence.AdoxioAccountId != null && item.AdoxioAccountId.Accountid == licence.AdoxioAccountId.Accountid))
                            && (item.AdoxioEffectivedate == licence.AdoxioEffectivedate)
                       )
                    {
                        notFound = false;
                        licence.AdoxioLicencesid = item.AdoxioLicencesid;
                        break;
                    }
                }
            }
            return notFound;
        }

        void CreateLicence(DynamicsClient _dynamicsClient, MicrosoftDynamicsCRMadoxioLicences licence)
        {
            string accountId = null;
            string establishmentId = null;
            string lginId = null;
            string licenceeId = null;


            if (licence.AdoxioAccountId != null)
            {
                accountId = licence.AdoxioAccountId.Accountid;
            }
            if (licence.AdoxioEstablishment != null)
            {
                establishmentId = licence.AdoxioEstablishment.AdoxioEstablishmentid;
            }
            if (licence.AdoxioLGIN != null)
            {
                lginId = licence.AdoxioLGIN.AdoxioLocalgovindigenousnationid;
            }
            if (licence.AdoxioLicencee != null)
            {
                licenceeId = licence.AdoxioLicencee.Accountid;
            }

            MicrosoftDynamicsCRMadoxioLicences newLicence = new MicrosoftDynamicsCRMadoxioLicences()
            {
                AdoxioName = licence.AdoxioName,
                AdoxioLicencenumber = licence.AdoxioLicencenumber,
                Statuscode = licence.Statuscode,
                AdoxioEffectivedate = licence.AdoxioEffectivedate,
                AdoxioExpirydate = licence.AdoxioExpirydate
            };


            try
            {
                licence = _dynamicsClient.Licenceses.Create(newLicence);
                Console.Out.WriteLine("Created licence " + licence.AdoxioName);
            }
            catch (OdataerrorException odee)
            {
                licence.AdoxioLicencesid = _dynamicsClient.GetCreatedRecord(odee, "Error creating contact");
            }
            if (accountId != null && AccountMap.ContainsKey(accountId))
            {
                var patchLicence = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AccountODataBind = _dynamicsClient.GetEntityURI("accounts", AccountMap[accountId])
                };

                try
                {
                    _dynamicsClient.Licenceses.Update(licence.AdoxioLicencesid, patchLicence);
                    Console.Out.WriteLine("Updated licence " + licence.AdoxioName);
                }
                catch (OdataerrorException odee)
                {
                    Console.WriteLine("Error updating licence");
                    Console.WriteLine(odee.Message);
                    Console.WriteLine(odee.Request.Content);
                    Console.WriteLine(odee.Response.Content);
                }
            }

            if (establishmentId != null && EstablishmentMap.ContainsKey(establishmentId))
            {
                var patchLicence = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioEstablishmentODataBind = _dynamicsClient.GetEntityURI("adoxio_establishments", EstablishmentMap[establishmentId])
                };

                try
                {
                    _dynamicsClient.Licenceses.Update(licence.AdoxioLicencesid, patchLicence);
                    Console.Out.WriteLine("Updated licence " + licence.AdoxioName);
                }
                catch (OdataerrorException odee)
                {
                    Console.WriteLine("Error updating licence");
                    Console.WriteLine(odee.Message);
                    Console.WriteLine(odee.Request.Content);
                    Console.WriteLine(odee.Response.Content);
                }
            }

            if (lginId != null && LocalgovindigenousnationMap.ContainsKey (lginId))
            {
                var patchLicence = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLginODataBind = _dynamicsClient.GetEntityURI("adoxio_localgovindigenousnations", LocalgovindigenousnationMap[lginId])
                };

                try
                {
                    _dynamicsClient.Licenceses.Update(licence.AdoxioLicencesid, patchLicence);
                    Console.Out.WriteLine("Updated licence " + licence.AdoxioName);
                }
                catch (OdataerrorException odee)
                {
                    Console.WriteLine("Error updating licence");
                    Console.WriteLine(odee.Message);
                    Console.WriteLine(odee.Request.Content);
                    Console.WriteLine(odee.Response.Content);
                }
            }

            if (licenceeId != null && AccountMap.ContainsKey(licenceeId))
            {
                var patchLicence = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    LicenceeODataBind = _dynamicsClient.GetEntityURI("accounts", AccountMap[licenceeId])
                };

                try
                {
                    _dynamicsClient.Licenceses.Update(licence.AdoxioLicencesid, patchLicence);
                    Console.Out.WriteLine("Updated licence " + licence.AdoxioName);
                }
                catch (OdataerrorException odee)
                {
                    Console.WriteLine("Error updating licence");
                    Console.WriteLine(odee.Message);
                    Console.WriteLine(odee.Request.Content);
                    Console.WriteLine(odee.Response.Content);
                }
            }

        }

        static List<MicrosoftDynamicsCRMadoxioLicences> GetCurrentLicences(DynamicsClient _dynamicsClient)
        {
            List<string> expand = new List<string>()
            {
                "adoxio_AccountId"
            };
            var data = _dynamicsClient.Licenceses.Get(expand: expand);
            return (List<MicrosoftDynamicsCRMadoxioLicences>)data.Value;
        }

        public void ImportLicences(DynamicsClient _dynamicsClient, List<MicrosoftDynamicsCRMadoxioLicences> licences)
        {
            var currentLicences = GetCurrentLicences(_dynamicsClient);

            foreach (var licence in licences)
            {
                if (licence.AdoxioAccountId != null && licence.AdoxioAccountId.Accountid != null)
                {
                    licence.AdoxioAccountId.Accountid = AccountMap[licence.AdoxioAccountId.Accountid];
                }

                string originalKey = licence.AdoxioLicencesid;
                if (LicenceNotFound(currentLicences, licence))
                {
                    CreateLicence(_dynamicsClient, licence);
                }
                else
                {
                    Console.WriteLine("Licence found " + licence.AdoxioName);
                }
                LicenceMap.Add(originalKey, licence.AdoxioLicencesid);
                Console.Out.Write(".");
            }
            Console.Out.WriteLine();
        }


        /***
        * Legal Entities
        ***/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="contact"></param>
        /// <returns>contact.contactid is set with the </returns>
        bool LegalEntityNotFound(List<MicrosoftDynamicsCRMadoxioLegalentity> licences, MicrosoftDynamicsCRMadoxioLegalentity legalEntity)
        {
            bool notFound = true;
            foreach (var item in licences)
            {
                if (item != null)
                {
                    if (
                            item.AdoxioName == legalEntity.AdoxioName
                       //&& (item.AdoxioAccountId == null || (item.AdoxioAccountId != null && licence.AdoxioAccountId != null && item.AdoxioAccountId.Accountid == licence.AdoxioAccountId.Accountid))
                       //&& (item.AdoxioEffectivedate == licence.AdoxioEffectivedate)
                       )
                    {
                        notFound = false;
                        legalEntity.AdoxioLegalentityid = item.AdoxioLegalentityid;
                        break;
                    }
                }
            }
            return notFound;
        }

        void CreateLegalEntity(DynamicsClient _dynamicsClient, MicrosoftDynamicsCRMadoxioLegalentity legalEntity)
        {
            string accountId = null;

            if (legalEntity.AdoxioAccount != null)
            {
                accountId = legalEntity.AdoxioAccount.Accountid;
            }

            MicrosoftDynamicsCRMadoxioLegalentity newLegalEntity = new MicrosoftDynamicsCRMadoxioLegalentity()
            {
                AdoxioCommonnonvotingshares = legalEntity.AdoxioCommonnonvotingshares,
                AdoxioCommonvotingshares = legalEntity.AdoxioCommonvotingshares,
                AdoxioDateofbirth = legalEntity.AdoxioDateofbirth,
                AdoxioFirstname = legalEntity.AdoxioFirstname,
                AdoxioInterestpercentage = legalEntity.AdoxioInterestpercentage,
                AdoxioIsapplicant = legalEntity.AdoxioIsapplicant,
                AdoxioIsindividual = legalEntity.AdoxioIsindividual,
                AdoxioLastname = legalEntity.AdoxioLastname,
                AdoxioLegalentitytype = legalEntity.AdoxioLegalentitytype,
                AdoxioPartnertype = legalEntity.AdoxioPartnertype,
                AdoxioMiddlename = legalEntity.AdoxioMiddlename,
                AdoxioName = legalEntity.AdoxioName,
                AdoxioIspartner = legalEntity.AdoxioIspartner,
                AdoxioIsshareholder = legalEntity.AdoxioIsshareholder,
                AdoxioIstrustee = legalEntity.AdoxioIstrustee,
                AdoxioIsdirector = legalEntity.AdoxioIsdirector,
                AdoxioIsofficer = legalEntity.AdoxioIsofficer,
                AdoxioIsseniormanagement = legalEntity.AdoxioIsseniormanagement,
                AdoxioIsowner = legalEntity.AdoxioIsowner,
                AdoxioPreferrednonvotingshares = legalEntity.AdoxioPreferrednonvotingshares,
                AdoxioPreferredvotingshares = legalEntity.AdoxioPreferredvotingshares,
                AdoxioSameasapplyingperson = legalEntity.AdoxioSameasapplyingperson,
                AdoxioEmail = legalEntity.AdoxioEmail,
                AdoxioDateofappointment = legalEntity.AdoxioDateofappointment,
                // Assigning the account this way throws exception:
                // System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
                //if (from.account.id != null)
                //{
                //    // fetch the account from Dynamics.
                //    var getAccountTask = _system.GetAccountById(null, Guid.Parse(from.account.id));
                //    getAccountTask.Wait();
                //    to.Adoxio_Account= getAccountTask.Result;
                //}
                AdoxioDateemailsent = legalEntity.AdoxioDateemailsent
            };

            try
            {
                legalEntity = _dynamicsClient.Legalentities.Create(newLegalEntity);
                Console.Out.WriteLine("Created legalEntity " + legalEntity.AdoxioName);
            }
            catch (OdataerrorException odee)
            {
                legalEntity.AdoxioLegalentityid = _dynamicsClient.GetCreatedRecord(odee, "Error creating legal entity");
            }

            if (accountId != null && AccountMap.ContainsKey(accountId))
            {
                var patchLegalEntity = new MicrosoftDynamicsCRMadoxioLegalentity()
                {
                    AdoxioAccountValueODataBind = _dynamicsClient.GetEntityURI("accounts", AccountMap[accountId])
                };

                try
                {
                    _dynamicsClient.Legalentities.Update(legalEntity.AdoxioLegalentityid, patchLegalEntity);
                    Console.Out.WriteLine("Updated legalentities " + legalEntity.AdoxioName);
                }
                catch (OdataerrorException odee)
                {
                    Console.WriteLine("Error updated legalEntity");
                    Console.WriteLine(odee.Message);
                    Console.WriteLine(odee.Request.Content);
                    Console.WriteLine(odee.Response.Content);
                }
            }

        }

        List<MicrosoftDynamicsCRMadoxioLegalentity> GetCurrentLegalEntities(DynamicsClient _dynamicsClient)
        {
            List<string> expand = new List<string>()
            {
                "adoxio_Account"
            };
            var data = _dynamicsClient.Legalentities.Get(expand: expand);
            return (List<MicrosoftDynamicsCRMadoxioLegalentity>)data.Value;
        }

        public void ImportLegalEntities(DynamicsClient _dynamicsClient, List<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities)
        {
            var currentLegalEntities = GetCurrentLegalEntities(_dynamicsClient);

            foreach (var legalEntity in legalEntities)
            {
                if (legalEntity.AdoxioAccount != null && legalEntity.AdoxioAccount.Accountid != null
                    && AccountMap.ContainsKey(legalEntity.AdoxioAccount.Accountid)
                    )
                {
                    legalEntity.AdoxioAccount.Accountid = AccountMap[legalEntity.AdoxioAccount.Accountid];
                }

                string originalKey = legalEntity.AdoxioLegalentityid;
                if (LegalEntityNotFound(currentLegalEntities, legalEntity))
                {
                    CreateLegalEntity(_dynamicsClient, legalEntity);
                }
                else
                {
                    Console.WriteLine("LegalEntity found " + legalEntity.AdoxioName);
                }
                LegalEntityMap.Add(originalKey, legalEntity.AdoxioLegalentityid);
                Console.Out.Write(".");
            }
            Console.Out.WriteLine();
        }


        /***
        * Establishments
        ***/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="contact"></param>
        /// <returns>contact.contactid is set with the </returns>
        bool EstablishmentNotFound(List<MicrosoftDynamicsCRMadoxioEstablishment> establishments, MicrosoftDynamicsCRMadoxioEstablishment establishment)
        {
            bool notFound = true;
            foreach (var item in establishments)
            {
                if (item != null)
                {
                    if (
                            item.AdoxioName == establishment.AdoxioName
                       && (item.AdoxioLicencee == null || (item.AdoxioLicencee != null && item.AdoxioLicencee.Accountid != null && item.AdoxioLicencee.Accountid == establishment.AdoxioLicencee.Accountid))
                       //&& (item.AdoxioEffectivedate == licence.AdoxioEffectivedate)
                       )
                    {
                        notFound = false;
                        establishment.AdoxioEstablishmentid = item.AdoxioEstablishmentid;
                        break;
                    }
                }
            }
            return notFound;
        }

        void CreateEstablishment(DynamicsClient _dynamicsClient, MicrosoftDynamicsCRMadoxioEstablishment establishment)
        {
            string accountId = null;

            if (establishment.AdoxioLicencee != null)
            {
                accountId = establishment.AdoxioLicencee.Accountid;
            }


            MicrosoftDynamicsCRMadoxioEstablishment newEstablishment = new MicrosoftDynamicsCRMadoxioEstablishment()
            {
                AdoxioAddresscity = establishment.AdoxioAddresscity,
                AdoxioAddresspostalcode = establishment.AdoxioAddresspostalcode,
                AdoxioAddressstreet = establishment.AdoxioAddressstreet,
                AdoxioAlreadyopen = establishment.AdoxioAlreadyopen,
                AdoxioEmail = establishment.AdoxioEmail,
                AdoxioExpectedopendate = establishment.AdoxioExpectedopendate,
                AdoxioFridayclose = establishment.AdoxioFridayclose,
                AdoxioFridayopen = establishment.AdoxioFridayopen,
                AdoxioHasduallicence = establishment.AdoxioHasduallicence,
                AdoxioIsrural = establishment.AdoxioIsrural,
                AdoxioIsstandalonepatio = establishment.AdoxioIsstandalonepatio,
                AdoxioLocatedatwinery = establishment.AdoxioLocatedatwinery,
                AdoxioLocatedonfirstnationland = establishment.AdoxioLocatedonfirstnationland,
                AdoxioMailsenttorestaurant = establishment.AdoxioMailsenttorestaurant,
                AdoxioMondayclose = establishment.AdoxioMondayclose,
                AdoxioMondayopen = establishment.AdoxioMondayopen,
                AdoxioName = establishment.AdoxioName,
                AdoxioOccupantcapacity = establishment.AdoxioOccupantcapacity,
                AdoxioOccupantload = establishment.AdoxioOccupantload,
                AdoxioParcelid = establishment.AdoxioParcelid,
                AdoxioPatronparticipation = establishment.AdoxioPatronparticipation,
                AdoxioPhone = establishment.AdoxioPhone,
                AdoxioSaturdayclose = establishment.AdoxioSaturdayclose,
                AdoxioSaturdayopen = establishment.AdoxioSaturdayopen,
                AdoxioSendmailtoestablishmentuponapproval = establishment.AdoxioSendmailtoestablishmentuponapproval,
                AdoxioStandardhours = establishment.AdoxioStandardhours,
                AdoxioSundayclose = establishment.AdoxioSundayclose,
                AdoxioSundayopen = establishment.AdoxioSundayopen,
                AdoxioThursdayclose = establishment.AdoxioThursdayclose,
                AdoxioThursdayopen = establishment.AdoxioThursdayopen,
                AdoxioTuesdayclose = establishment.AdoxioTuesdayclose,
                AdoxioTuesdayopen = establishment.AdoxioTuesdayopen,
                AdoxioWednesdayclose = establishment.AdoxioWednesdayclose,
                AdoxioWednesdayopen = establishment.AdoxioWednesdayopen,
                Statecode = establishment.Statecode
            };

            try
            {
                establishment = _dynamicsClient.Establishments.Create(newEstablishment);
                Console.Out.WriteLine("Created establishment " + establishment.AdoxioName);
            }
            catch (OdataerrorException odee)
            {
                establishment.AdoxioEstablishmentid = _dynamicsClient.GetCreatedRecord(odee, "Error creating legal entity");
            }

            if (accountId != null)
            {
                var patchEstablishment = new MicrosoftDynamicsCRMadoxioEstablishment()
                {
                    AdoxioLicenceeODataBind = _dynamicsClient.GetEntityURI("accounts", AccountMap[accountId])
                };

                try
                {
                    _dynamicsClient.Establishments.Update(establishment.AdoxioEstablishmentid, patchEstablishment);
                    Console.Out.WriteLine("Updated establishment " + establishment.AdoxioName);
                }
                catch (OdataerrorException odee)
                {
                    Console.WriteLine("Error updated establishment");
                    Console.WriteLine(odee.Message);
                    Console.WriteLine(odee.Request.Content);
                    Console.WriteLine(odee.Response.Content);
                }
            }

        }

        List<MicrosoftDynamicsCRMadoxioEstablishment> GetCurrentEstablishments(DynamicsClient _dynamicsClient)
        {
            List<string> expand = new List<string>()
            {
                "adoxio_Account"
            };
            var data = _dynamicsClient.Establishments.Get();// expand: expand);
            return (List<MicrosoftDynamicsCRMadoxioEstablishment>)data.Value;
        }

        public void ImportEstablishments(DynamicsClient _dynamicsClient, List<MicrosoftDynamicsCRMadoxioEstablishment> establishments)
        {
            var currentEstablishments = GetCurrentEstablishments(_dynamicsClient);

            foreach (var establishment in establishments)
            {
                if (establishment.AdoxioLicencee != null && establishment.AdoxioLicencee.Accountid != null)
                {
                    establishment.AdoxioLicencee.Accountid = AccountMap[establishment.AdoxioLicencee.Accountid];
                }

                string originalKey = establishment.AdoxioEstablishmentid;
                if (EstablishmentNotFound(currentEstablishments, establishment))
                {
                    CreateEstablishment(_dynamicsClient, establishment);
                }
                else
                {
                    Console.WriteLine("Establishment found " + establishment.AdoxioName);
                }
                EstablishmentMap.Add(originalKey, establishment.AdoxioEstablishmentid);
                Console.Out.Write(".");
            }
            Console.Out.WriteLine();
        }

        /***
         * Invoices
         ***/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="contact"></param>
        /// <returns>contact.contactid is set with the </returns>
        bool InvoiceNotFound(List<MicrosoftDynamicsCRMinvoice> invoices, MicrosoftDynamicsCRMinvoice invoice)
        {
            bool notFound = true;
            foreach (var item in invoices)
            {
                if (item != null)
                {
                    if (
                            item.AdoxioTransactionid == invoice.AdoxioTransactionid
                            && (item.CustomeridAccount == null || (item.CustomeridAccount != null && invoice.CustomeridAccount != null && item.CustomeridAccount.Accountid == invoice.CustomeridAccount.Accountid))
                       )
                    {
                        notFound = false;
                        invoice.Invoiceid = item.Invoiceid;
                        break;
                    }
                }
            }
            return notFound;
        }

        void CreateInvoice(DynamicsClient _dynamicsClient, MicrosoftDynamicsCRMinvoice invoice)
        {
            string accountId = null;
            string workerId = null;

            if (invoice.CustomeridAccount != null)
            {
                accountId = invoice.CustomeridAccount.Accountid;
            }

            MicrosoftDynamicsCRMinvoice newInvoice = new MicrosoftDynamicsCRMinvoice()
            {
                Name = invoice.Name,
                Invoicenumber = invoice.Invoicenumber,
                Statecode = invoice.Statecode,
                //Statuscode = invoice.Statuscode,
                Totaltax = invoice.Totaltax,
                Totalamount = invoice.Totalamount,
                AdoxioTransactionid = invoice.AdoxioTransactionid,
                AdoxioReturnedtransactionid = invoice.AdoxioReturnedtransactionid
            };

            try
            {
                invoice = _dynamicsClient.Invoices.Create(newInvoice);
                Console.Out.WriteLine("created invoice " + invoice.Name);
            }
            catch (OdataerrorException odee)
            {
                invoice.Invoiceid = _dynamicsClient.GetCreatedRecord(odee, "Error creating contact");
            }
            if (accountId != null && AccountMap.ContainsKey(accountId))
            {
                var patchAccount = new MicrosoftDynamicsCRMinvoice()
                {
                    CustomerIdAccountODataBind = _dynamicsClient.GetEntityURI("accounts", AccountMap[accountId])
                };
            }
        }


        static List<MicrosoftDynamicsCRMinvoice> GetCurrentInvoices(DynamicsClient _dynamicsClient)
        {
            List<string> expand = new List<string>()
            {
                "customerid_account"
            };
            var data = _dynamicsClient.Invoices.Get();
            return (List<MicrosoftDynamicsCRMinvoice>)data.Value;
        }

        public void ImportInvoices(DynamicsClient _dynamicsClient, List<MicrosoftDynamicsCRMinvoice> invoices)
        {
            var currentInvoices = GetCurrentInvoices(_dynamicsClient);

            foreach (var invoice in invoices)
            {
                if (invoice.CustomeridAccount != null && invoice.CustomeridAccount.Accountid != null && AccountMap.ContainsKey(invoice.CustomeridAccount.Accountid))
                {
                    invoice.CustomerIdAccountODataBind = _dynamicsClient.GetEntityURI("accounts", AccountMap[invoice.CustomeridAccount.Accountid]);
                }

                string originalKey = invoice.Invoiceid;
                if (InvoiceNotFound(currentInvoices, invoice))
                {
                    CreateInvoice(_dynamicsClient, invoice);
                }
                else
                {
                    Console.WriteLine("Invoice found " + invoice);
                }
                InvoiceMap.Add(originalKey, invoice.Invoiceid);
                Console.Out.Write(".");
            }
            Console.Out.WriteLine();
        }

    }
}
