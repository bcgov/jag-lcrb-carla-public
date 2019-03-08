using Gov.Lclb.Cllb.Interfaces.Models;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTool
{
    class Obfuscator
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

        


        public Obfuscator(Dictionary<string, string> contactMap,
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
        int CountParagraphs(string inputString)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(inputString))
            {
                int newlineCount = inputString.Count(f => f == '\r');
                result += newlineCount;
            }
            if (result == 0)
            {
                result = 1;
            }

            return result;
        }

        int CountWords(string inputString)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(inputString))
            {
                int newlineCount = inputString.Count(f => f == ' ');
                result += newlineCount;
            }

            if (result == 0)
            {
                result = 1;
            }

            return result;
        }

        string RandomLipsumString(string inputString)
        {
            var paragraphs = CountParagraphs(inputString);
            var randomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextLipsum { Paragraphs = 1 });
            string result = randomizer.Generate();
            return result;
        }

        string RandomCompanyName(string inputString)
        {
            int numberWords = CountWords(inputString);
            var randomizerTextWords = RandomizerFactory.GetRandomizer(new FieldOptionsTextWords { Min = numberWords, Max = numberWords });
            string result = randomizerTextWords.Generate();
            return result;
        }

        string RandomStringNumber(string inputString)
        {
            string result = null;

            if (!string.IsNullOrEmpty(inputString))
            {
                result = "";
                var randomizerInteger = RandomizerFactory.GetRandomizer(new FieldOptionsInteger { Min = 0, Max = 9 });
                for (int i = 0; i < inputString.Length; i++)
                {
                    int? integer = randomizerInteger.Generate();
                    string temp = integer.ToString();
                    result += temp[0];
                }
            }

            return result;
        }

        string RandomPid()
        {
            string result = "006-"
                + RandomStringNumber("111") + "-"
                + RandomStringNumber("111");
            return result;
        }

        DateTimeOffset RandomDateInPast()
        {
            var from = DateTime.Parse("2001-01-01");
            var to = DateTime.Now;

            var randomizerDate = RandomizerFactory.GetRandomizer(new FieldOptionsDateTime { From = from, To = to, IncludeTime = false });

            return new DateTimeOffset(randomizerDate.Generate().Value);
        }

        string RandomEmail()
        {

            var randomizerEmail = RandomizerFactory.GetRandomizer(new FieldOptionsEmailAddress());

            string result = randomizerEmail.Generate() + ".xom";

            return result;
        }

        string RandomCountry()
        {

            var randomizerCountry = RandomizerFactory.GetRandomizer(new FieldOptionsCountry());

            string result = randomizerCountry.Generate();

            return result;
        }

        string RandomCity()
        {
            var randomizerCity = RandomizerFactory.GetRandomizer(new FieldOptionsCity());

            string result = randomizerCity.Generate();

            return result;
        }

        string RandomFirstName()
        {
            var randomizer = RandomizerFactory.GetRandomizer(new FieldOptionsFirstName());

            string result = randomizer.Generate();

            return result;
        }

        string RandomLastName()
        {
            var randomizer = RandomizerFactory.GetRandomizer(new FieldOptionsFirstName());

            string result = randomizer.Generate();

            return result;
        }

        public List<MicrosoftDynamicsCRMcontact> ObfuscateContacts(List<MicrosoftDynamicsCRMcontact> contacts)
        {
            List<MicrosoftDynamicsCRMcontact> result = new List<MicrosoftDynamicsCRMcontact>();

            foreach (var contact in contacts)
            {
                string firstName = RandomFirstName();
                string lastName = RandomLastName();
                string fullName = $"{firstName} {lastName}";
                var newItem = new MicrosoftDynamicsCRMcontact()
                {
                    Contactid = Guid.NewGuid().ToString(),
                    Fullname = fullName,
                    Firstname = firstName,
                    Lastname = lastName,
                    Emailaddress1 = RandomEmail(),
                    Telephone1 = RandomStringNumber(contact.Telephone1),
                    Address1Name = RandomCompanyName(contact.Address1Name),
                    Address1Line1 = RandomCompanyName(contact.Address1Line1),
                    Address1City = contact.Address1City,
                    Address1Country = contact.Address1Country,
                    Address1Postalcode = contact.Address1Postalcode,
                    Address2Name = RandomCompanyName(contact.Address2Name),
                    Address2Line1 = RandomCompanyName(contact.Address2Line1),
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

                result.Add(newItem);
                ContactMap.Add(contact.Contactid, newItem.Contactid);

            }
            return result;
        }


        public List<MicrosoftDynamicsCRMaccount> ObfuscateAccounts(List<MicrosoftDynamicsCRMaccount> accounts)
        {
            List<MicrosoftDynamicsCRMaccount> result = new List<MicrosoftDynamicsCRMaccount>();

            foreach (var account in accounts)
            {
                var newItem = new MicrosoftDynamicsCRMaccount()
                {
                    Name = RandomCompanyName(account.Name),
                    AdoxioBusinesstype = account.AdoxioBusinesstype,
                    AdoxioBcincorporationnumber = RandomStringNumber(account.AdoxioBcincorporationnumber),
                    AdoxioDateofincorporationinbc = RandomDateInPast(),
                    Accountnumber = RandomStringNumber(account.Accountnumber),
                    Accountid = Guid.NewGuid().ToString(),
                    Emailaddress1 = RandomEmail(),
                    Telephone1 = RandomStringNumber(account.Telephone1),
                    Address1Name = RandomCompanyName(account.Address1Name),
                    Address1Line1 = RandomCompanyName(account.Address1Line1),
                    Address1City = account.Address1City,
                    Address1Stateorprovince = account.Address1Stateorprovince,
                    Address1Country = account.Address1Country,
                    Address1Postalcode = account.Address1Postalcode,
                    AdoxioAccounttype = account.AdoxioAccounttype,                    
                };

                if (account.Primarycontactid != null && account.Primarycontactid.Contactid != null)
                {
                    newItem.Primarycontactid = new MicrosoftDynamicsCRMcontact
                    {
                        Contactid = ContactMap[account.Primarycontactid.Contactid]
                    };
                }

                AccountMap.Add(account.Accountid, newItem.Accountid);

                result.Add(newItem);

            }
            return result;
        }


        public List<MicrosoftDynamicsCRMadoxioAlias> ObfuscateAliases(List<MicrosoftDynamicsCRMadoxioAlias> aliases)
        {
            List<MicrosoftDynamicsCRMadoxioAlias> result = new List<MicrosoftDynamicsCRMadoxioAlias>();

            foreach (var alias in aliases)
            {
                string firstName = RandomFirstName();
                string lastName = RandomLastName();

                var newItem = new MicrosoftDynamicsCRMadoxioAlias()
                {
                    AdoxioAliasid = Guid.NewGuid().ToString(),
                    AdoxioFirstname = firstName,
                    AdoxioLastname = lastName
                };

                if (alias.AdoxioContactId != null)
                {
                    newItem.AdoxioContactId = new MicrosoftDynamicsCRMcontact
                    {
                        Contactid = ContactMap[alias.AdoxioContactId.Contactid]
                    };
                }

                if (alias.AdoxioWorkerId != null)
                {
                    newItem.AdoxioWorkerId = new MicrosoftDynamicsCRMadoxioWorker
                    {
                        AdoxioWorkerid = WorkerMap[alias.AdoxioWorkerId.AdoxioWorkerid]
                    };
                }
                AliasMap.Add(alias.AdoxioAliasid, newItem.AdoxioAliasid);
                result.Add(newItem);

            }
            return result;
        }


        public List<MicrosoftDynamicsCRMinvoice> ObfuscateInvoices(List<MicrosoftDynamicsCRMinvoice> invoices)
        {
            List<MicrosoftDynamicsCRMinvoice> result = new List<MicrosoftDynamicsCRMinvoice>();

            foreach (var invoice in invoices)
            {
                string firstName = RandomFirstName();
                string lastName = RandomLastName();

                var newItem = new MicrosoftDynamicsCRMinvoice()
                {
                    Invoiceid = Guid.NewGuid().ToString(),
                    Name = RandomCompanyName(invoice.Name),
                    Invoicenumber = invoice.Invoicenumber,
                    Statecode = invoice.Statecode,
                    Statuscode = invoice.Statuscode,
                    Totaltax = invoice.Totaltax,
                    Totalamount = invoice.Totalamount,
                    AdoxioTransactionid = invoice.AdoxioTransactionid,
                    AdoxioReturnedtransactionid = invoice.AdoxioReturnedtransactionid
                };

                if (invoice._customeridValue != null && AccountMap.ContainsKey(invoice._customeridValue))
                {
                    newItem.CustomeridAccount = new MicrosoftDynamicsCRMaccount
                    {
                        Accountid = AccountMap[invoice._customeridValue]
                    };
                }
                InvoiceMap.Add(invoice.Invoiceid, newItem.Invoiceid);
                result.Add(newItem);

            }
            return result;
        }

        public List<MicrosoftDynamicsCRMadoxioEstablishment> ObfuscateEstablishments(List<MicrosoftDynamicsCRMadoxioEstablishment> establishments)
        {
            List<MicrosoftDynamicsCRMadoxioEstablishment> result = new List<MicrosoftDynamicsCRMadoxioEstablishment>();

            foreach (var establishment in establishments)
            {
                string firstName = RandomFirstName();
                string lastName = RandomLastName();

                var newItem = new MicrosoftDynamicsCRMadoxioEstablishment()
                {                    
                    AdoxioEstablishmentid = Guid.NewGuid().ToString(),
                    AdoxioName = RandomCompanyName(establishment.AdoxioName),
                    AdoxioAddresscity = establishment.AdoxioAddresscity,
                    AdoxioAddresspostalcode = establishment.AdoxioAddresspostalcode,
                    AdoxioAddressstreet = RandomCompanyName(establishment.AdoxioAddressstreet),
                    AdoxioAlreadyopen = establishment.AdoxioAlreadyopen,
                    AdoxioEmail = RandomEmail(),
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
                    Statuscode = establishment.Statuscode,
                    Statecode = establishment.Statecode
                };


                EstablishmentMap.Add(establishment.AdoxioEstablishmentid, newItem.AdoxioEstablishmentid);
                result.Add(newItem);

            }
            return result;
        }



        public List<MicrosoftDynamicsCRMadoxioApplication>  ObfuscateApplications(List<MicrosoftDynamicsCRMadoxioApplication> applications)
        {
            List<MicrosoftDynamicsCRMadoxioApplication> result = new List<MicrosoftDynamicsCRMadoxioApplication>();

            foreach (var application in applications)
            {
                string firstName = RandomFirstName();
                string lastName = RandomLastName();
                string adoxioAdditionalpropertyinformation = RandomLipsumString(application.AdoxioAdditionalpropertyinformation);
                if (adoxioAdditionalpropertyinformation.Length > 250)
                {
                    adoxioAdditionalpropertyinformation = adoxioAdditionalpropertyinformation.Substring(0, 249);
                }

                var newItem = new MicrosoftDynamicsCRMadoxioApplication()
                {
                    AdoxioApplicanttype = application.AdoxioApplicanttype,
                    AdoxioAddresscity = RandomCity(),
                    AdoxioAddresscountry = application.AdoxioAddresscountry,
                    AdoxioAddresspostalcode = application.AdoxioAddresspostalcode,
                    AdoxioAddressprovince = application.AdoxioAddressprovince,
                    AdoxioAddressstreet = application.AdoxioAddressstreet,

                    // adoxio_additionalpropertyinformation
                    AdoxioApplicationid = Guid.NewGuid().ToString(),
                    AdoxioName = RandomCompanyName(application.AdoxioName),
                    AdoxioJobnumber = application.AdoxioJobnumber,
                    // get establishment name and address
                    AdoxioEstablishmentpropsedname = RandomCompanyName(application.AdoxioEstablishmentpropsedname),
                    AdoxioEstablishmentaddressstreet = RandomCompanyName(application.AdoxioEstablishmentaddressstreet),
                    AdoxioEstablishmentaddresscity = application.AdoxioEstablishmentaddresscity,

                    AdoxioEstablishmentaddresscountry = application.AdoxioEstablishmentaddresscountry,

                    AdoxioEstablishmentaddresspostalcode = application.AdoxioEstablishmentaddresspostalcode,
                    AdoxioEstablishmentparcelid = RandomPid(),
                    AdoxioLicencefeeinvoicepaid = application.AdoxioLicencefeeinvoicepaid,
                    Statuscode = application.Statuscode,
                    AdoxioAppchecklistfinaldecision = application.AdoxioAppchecklistfinaldecision,
                    AdoxioPaymentrecieved = application.AdoxioPaymentrecieved,
                    AdoxioAdditionalpropertyinformation = adoxioAdditionalpropertyinformation,
                    AdoxioInvoicetrigger = application.AdoxioInvoicetrigger,
                    AdoxioPaymentreceiveddate = RandomDateInPast(),
                    AdoxioAuthorizedtosubmit = application.AdoxioAuthorizedtosubmit,
                    AdoxioSignatureagreement = application.AdoxioSignatureagreement,
                    //get contact details
                    AdoxioContactpersonfirstname = RandomFirstName(),
                    AdoxioContactpersonlastname = RandomLastName(),
                    AdoxioRole = RandomCompanyName(application.AdoxioRole),
                    AdoxioEmail = RandomEmail(),
                    AdoxioContactpersonphone = RandomStringNumber(application.AdoxioContactpersonphone),
                    Modifiedon = application.Modifiedon,
                    Createdon = application.Createdon
                    
                };



                //get applying person from Contact entity
                if (application._adoxioApplyingpersonValue != null)
                {
                    newItem.AdoxioApplyingPerson = new MicrosoftDynamicsCRMcontact()
                    {
                        Contactid = ContactMap[application._adoxioApplyingpersonValue]
                    };
                }
                if (application._adoxioApplicantValue != null)
                {
                    newItem.AdoxioApplicant = new MicrosoftDynamicsCRMaccount()
                    {
                        Accountid = AccountMap[application._adoxioApplicantValue]
                    };
                }

                //get license type from Adoxio_licencetype entity
                if (application._adoxioLicencetypeValue != null)
                {
                    newItem.AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioLicencetypeid = application._adoxioLicencetypeValue
                    };
                }

                
                if (application._adoxioInvoiceValue != null)
                {
                    newItem.AdoxioInvoice = new MicrosoftDynamicsCRMinvoice()
                    {
                        Invoiceid = InvoiceMap[application._adoxioInvoiceValue]
                    };
                }

                if (application.AdoxioLicenceFeeInvoice != null)
                {
                    newItem.AdoxioLicenceFeeInvoice = new MicrosoftDynamicsCRMinvoice()
                    {
                        Invoiceid = InvoiceMap[application._adoxioInvoiceValue]
                    };
                }

                if (application._adoxioAssignedlicenceValue != null && LicenceMap.ContainsKey(application._adoxioAssignedlicenceValue))
                {
                    newItem.AdoxioAssignedLicence = new MicrosoftDynamicsCRMadoxioLicences()
                    {
                        AdoxioLicencesid = LicenceMap[application._adoxioAssignedlicenceValue]
                    };

                }

                if (application._adoxioLocalgovindigenousnationidValue != null)
                {
                    newItem.AdoxioLocalgovindigenousnationid = new MicrosoftDynamicsCRMadoxioLocalgovindigenousnation()
                    {
                        AdoxioLocalgovindigenousnationid = application._adoxioLocalgovindigenousnationidValue
                    };
                }

                ApplicationMap.Add(application.AdoxioApplicationid, newItem.AdoxioApplicationid);
                result.Add(newItem);

            }
            return result;
        }

        public List<MicrosoftDynamicsCRMadoxioLicences> ObfuscateLicences(List<MicrosoftDynamicsCRMadoxioLicences> licences)
        {
            List<MicrosoftDynamicsCRMadoxioLicences> result = new List<MicrosoftDynamicsCRMadoxioLicences>();

            foreach (var licence in licences)
            {

                var newItem = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicencesid = Guid.NewGuid().ToString(),
                    AdoxioLicencenumber = licence.AdoxioLicencenumber,
                    AdoxioExpirydate = licence.AdoxioExpirydate,
                    Statuscode = licence.Statuscode,
                };

                if (licence._adoxioEstablishmentValue != null)
                {
                    newItem.AdoxioEstablishment = new MicrosoftDynamicsCRMadoxioEstablishment
                    {
                        AdoxioEstablishmentid = EstablishmentMap[licence._adoxioEstablishmentValue]
                    };
                }

                if (licence._adoxioLicencetypeValue != null)
                {
                    newItem.AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype
                    {
                        AdoxioLicencetypeid = licence._adoxioLicencetypeValue
                    };
                }

                if (!LicenceMap.ContainsKey(licence.AdoxioLicencesid))
                {
                    LicenceMap.Add(licence.AdoxioLicencesid, newItem.AdoxioLicencesid);
                }
                result.Add(newItem);

            }
            return result;
        }

        public List<MicrosoftDynamicsCRMadoxioWorker> ObfuscateWorkers(List<MicrosoftDynamicsCRMadoxioWorker> workers)
        {
            List<MicrosoftDynamicsCRMadoxioWorker> result = new List<MicrosoftDynamicsCRMadoxioWorker>();

            foreach (var worker in workers)
            {
                string firstName = RandomFirstName();
                string lastName = RandomLastName();

                var newItem = new MicrosoftDynamicsCRMadoxioWorker()
                {
                    AdoxioWorkerid = Guid.NewGuid().ToString(),
                    AdoxioFirstname = firstName,
                    AdoxioLastname = lastName
                };

                if (worker.AdoxioContactId != null)
                {
                    newItem.AdoxioContactId = new MicrosoftDynamicsCRMcontact
                    {
                        Contactid = ContactMap[worker.AdoxioContactId.Contactid]
                    };
                }

                WorkerMap.Add(worker.AdoxioWorkerid, newItem.AdoxioWorkerid);
                result.Add(newItem);
            }
            return result;
        }

        public List<MicrosoftDynamicsCRMadoxioLegalentity> ObfuscateLegalEntities(List<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities)
        {
            List<MicrosoftDynamicsCRMadoxioLegalentity> result = new List<MicrosoftDynamicsCRMadoxioLegalentity>();

            foreach (var legalEntity in legalEntities)
            {
                string firstName = RandomFirstName();
                string lastName = RandomLastName();

                var newItem = new MicrosoftDynamicsCRMadoxioLegalentity()
                {
                    AdoxioLegalentityid = Guid.NewGuid().ToString(),
                    AdoxioCommonnonvotingshares = legalEntity.AdoxioCommonnonvotingshares,
                    AdoxioCommonvotingshares = legalEntity.AdoxioCommonvotingshares,
                    AdoxioDateofbirth = RandomDateInPast(),
                    AdoxioFirstname = firstName,
                    AdoxioLastname = lastName,
                    AdoxioInterestpercentage = legalEntity.AdoxioInterestpercentage,
                    AdoxioIsindividual = legalEntity.AdoxioIsindividual,
                    AdoxioLegalentitytype = legalEntity.AdoxioLegalentitytype,
                    AdoxioPartnertype = legalEntity.AdoxioPartnertype,
                    AdoxioName = firstName + " " + lastName,
                    AdoxioEmail = RandomEmail(),
                    AdoxioIspartner = legalEntity.AdoxioIspartner,
                    AdoxioIsapplicant = legalEntity.AdoxioIsapplicant,
                    AdoxioIsshareholder = legalEntity.AdoxioIsshareholder,
                    AdoxioIsdirector = legalEntity.AdoxioIsdirector,
                    AdoxioIsofficer = legalEntity.AdoxioIsofficer,
                    AdoxioIsseniormanagement = legalEntity.AdoxioIsseniormanagement,
                    AdoxioPreferrednonvotingshares = legalEntity.AdoxioPreferrednonvotingshares,
                    AdoxioPreferredvotingshares = legalEntity.AdoxioPreferredvotingshares,
                    AdoxioSameasapplyingperson = legalEntity.AdoxioSameasapplyingperson
                };

                if (legalEntity._adoxioAccountValue != null)
                {
                    newItem.AdoxioAccount = new MicrosoftDynamicsCRMaccount()
                    {
                        Accountid = AccountMap[legalEntity._adoxioAccountValue]
                    };
                }
                if (legalEntity._adoxioShareholderaccountidValue != null)
                {
                    newItem.AdoxioShareholderAccountID = new MicrosoftDynamicsCRMaccount()
                    {
                        Accountid = AccountMap[legalEntity._adoxioShareholderaccountidValue]
                    };
                }

                // parent legal entity

                if (legalEntity._adoxioLegalentityownedValue != null)
                {
                    newItem.AdoxioLegalEntityOwned = new MicrosoftDynamicsCRMadoxioLegalentity()
                    {
                        AdoxioLegalentityid = legalEntity._adoxioLegalentityownedValue
                    };
                }

                if (legalEntity.AdoxioDateemailsent != null)
                {
                    newItem.AdoxioDateemailsent = RandomDateInPast();
                }

                if (legalEntity.AdoxioDateofappointment != null)
                {
                    newItem.AdoxioDateofappointment = RandomDateInPast();
                }

                LegalEntityMap.Add(legalEntity.AdoxioLegalentityid, newItem.AdoxioLegalentityid);
                result.Add(newItem);

            }

            // second pass to fix the parent child relationship for legalEntities
            foreach (var legalEntity in result)
            {
                if (legalEntity.AdoxioLegalEntityOwned != null)
                {
                    legalEntity.AdoxioLegalEntityOwned.AdoxioLegalentityid = LegalEntityMap[legalEntity.AdoxioLegalEntityOwned.AdoxioLegalentityid];
                }
            }

            return result;
        }


    }
}
