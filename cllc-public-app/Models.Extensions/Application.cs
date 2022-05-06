using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Models
{
    enum DefaultYesNoLookup
    {
        Yes = 845280000
    }

    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class ApplicationExtensions
    {

        public static bool? ConvertYesNoLookupToBool(int? inputValue)
        {
            bool? result = null;
            if (inputValue != null)
            {
                result = inputValue == (int?) DefaultYesNoLookup.Yes;
            }
            return result;
        }

        public static void CopyValues(this MicrosoftDynamicsCRMadoxioApplication to, ViewModels.Application from)
        {
            to.AdoxioName = from.Name;
            //to.Adoxio_jobnumber = from.jobNumber;            
            to.AdoxioEstablishmentpropsedname = from.EstablishmentName;
            to.AdoxioEstablishmentaddressstreet = from.EstablishmentAddressStreet;
            to.AdoxioEstablishmentaddresscity = from.EstablishmentAddressCity;
            to.AdoxioEstablishmentaddresspostalcode = from.EstablishmentAddressPostalCode;
            to.AdoxioPin = from.Pin;
            // 12-10-2019 - Removed the update to AdoxioAddressCity as the Dynamics workflow handles that.

            to.AdoxioEstablishmentparcelid = from.EstablishmentParcelId;
            to.AdoxioEstablishmentphone = from.EstablishmentPhone;
            to.AdoxioEstablishmentemail = from.EstablishmentEmail;

            to.AdoxioContactpersonfirstname = from.ContactPersonFirstName;
            to.AdoxioContactpersonlastname = from.ContactPersonLastName;
            to.AdoxioRole = from.ContactPersonRole;
            to.AdoxioEmail = from.ContactPersonEmail;
            to.AdoxioContactpersonphone = from.ContactPersonPhone;
            to.AdoxioAuthorizedtosubmit = from.AuthorizedToSubmit;
            to.AdoxioSignatureagreement = from.SignatureAgreement;
            to.AdoxioAdditionalpropertyinformation = from.AdditionalPropertyInformation;
            to.AdoxioFederalproducernames = from.FederalProducerNames;

            to.AdoxioInvoicetrigger = (int?)from.InvoiceTrigger;

            to.AdoxioRenewalcriminaloffencecheck = (int?)from.RenewalCriminalOffenceCheck;
            to.AdoxioRenewalunreportedsaleofbusiness = (int?)from.RenewalUnreportedSaleOfBusiness;
            to.AdoxioRenewalbusinesstype = (int?)from.RenewalBusinessType;
            to.AdoxioRenewaltiedhouse = (int?)from.RenewalTiedhouse;
            to.AdoxioRenewalorgleadership = (int?)from.RenewalOrgLeadership;
            to.AdoxioRenewalkeypersonnel = (int?)from.Renewalkeypersonnel;
            to.AdoxioRenewalshareholders = (int?)from.RenewalShareholders;
            to.AdoxioRenewaloutstandingfines = (int?)from.RenewalOutstandingFines;
            to.AdoxioRenewalbranding = (int?)from.RenewalBranding;
            to.AdoxioRenewalsignage = (int?)from.RenewalSignage;
            to.AdoxioRenewalestablishmentaddress = (int?)from.RenewalEstablishmentAddress;
            to.AdoxioRenewalvalidinterest = (int?)from.RenewalValidInterest;
            to.AdoxioRenewalzoning = (int?)from.RenewalZoning;
            to.AdoxioRenewalfloorplan = (int?)from.RenewalFloorPlan;
            to.AdoxioRenewalsitemap = (int?)from.RenewalSiteMap;
            to.AdoxioRenewaltiedhousefederalinterest = (int?)from.TiedhouseFederalInterest;
            to.AdoxioDescription1 = from.Description1;
            to.AdoxioDescription2 = from.Description2;
            to.AdoxioDescription3 = from.Description3;
            to.AdoxioTempdatefrom = from.TempDateFrom;
            to.AdoxioTempdateto = from.TempDateTo;

            to.AdoxioM01 = from.IsMonth01;
            to.AdoxioM02 = from.IsMonth02;
            to.AdoxioM03 = from.IsMonth03;

            to.AdoxioM04 = from.IsMonth04;
            to.AdoxioM05 = from.IsMonth05;
            to.AdoxioM06 = from.IsMonth06;

            to.AdoxioM07 = from.IsMonth07;
            to.AdoxioM08 = from.IsMonth08;
            to.AdoxioM09 = from.IsMonth09;

            to.AdoxioM10 = from.IsMonth10;
            to.AdoxioM11 = from.IsMonth11;
            to.AdoxioM12 = from.IsMonth12;

            //store opening
            to.AdoxioIsreadyworkers = from.IsReadyWorkers;
            to.AdoxioIsreadynamebranding = from.IsReadyNameBranding;
            to.AdoxioIsreadydisplays = from.IsReadyDisplays;
            to.AdoxioIsreadyintruderalarm = from.IsReadyIntruderAlarm;
            to.AdoxioIsreadyfirealarm = from.IsReadyFireAlarm;
            to.AdoxioIsreadylockedcases = from.IsReadyLockedCases;
            to.AdoxioIsreadylockedstorage = from.IsReadyLockedStorage;
            to.AdoxioIsreadyperimeter = from.IsReadyPerimeter;
            to.AdoxioIsreadyretailarea = from.IsReadyRetailArea;
            to.AdoxioIsreadystorage = from.IsReadyStorage;
            to.AdoxioIsreadyentranceexit = from.IsReadyExtranceExit;
            to.AdoxioIsreadysurveillancenotice = from.IsReadySurveillanceNotice;
            to.AdoxioIsreadyproductnotvisibleoutside = from.IsReadyProductNotVisibleOutside;
            to.AdoxioIslocatedingrocerystore = from.IsLocatedInGroceryStore;
            to.AdoxioEstablishmentopeningdate = from.Establishmentopeningdate;
            to.AdoxioIsreadyvalidinterest = from.IsReadyValidInterest;

            to.AdoxioAuthorizedtosubmit = from.AuthorizedToSubmit;
            to.AdoxioSignatureagreement = from.SignatureAgreement;

            to.AdoxioApplicanttype = (int?)from.ApplicantType;
            to.AdoxioLgzoning = (int?)from.LgZoning;
            to.AdoxioLgdecisioncomments = from.LGDecisionComments;

            // catering fields
            to.AdoxioPreviouslicenceapplication = from.PreviousApplication;
            to.AdoxioPreviouslicenceapplicationdetails = from.PreviousApplicationDetails;
            to.AdoxioRuralagencystoreappointment = from.RuralAgencyStoreAppointment;
            to.AdoxioLiquorindustryconnections = from.LiquorIndustryConnections;
            to.AdoxioLiquorindustryconnectionsdetails = from.LiquorIndustryConnectionsDetails;
            to.AdoxioOtherbusinessesatthesamelocation = from.OtherBusinesses;
            to.AdoxioOtherbusinesssamelocationdetails = from.OtherBusinessesDetails;
            to.AdoxioIsapplicationcomplete = (int?)from.IsApplicationComplete;

            to.AdoxioRenewaldui = (int?)from.RenewalDUI;
            to.AdoxioRenewalthirdparty = (int?)from.RenewalThirdParty;

            to.AdoxioIsownerbusiness = from.IsOwnerBusiness;
            to.AdoxioIsownerhasvalidinterest = from.HasValidInterest;
            to.AdoxioIsownerwillhavevalidinterest = from.WillHaveValidInterest;
            to.AdoxioZoningstatus = @from.ZoningStatus;

            to.AdoxioIshaspatio = from.IsHasPatio;

            //lg approval fields
            to.AdoxioLgnoobjection = from.LgNoObjection;

            to.AdoxioLgnameofofficial = from.LGNameOfOfficial;
            to.AdoxioLgtitleposition = from.LGTitlePosition;
            to.AdoxioLgcontactphone = from.LGContactPhone;
            to.AdoxioLgcontactemail = from.LGContactEmail;
            to.AdoxioLgdecisionsubmissiondate = from.LGDecisionSubmissionDate;
            to.AdoxioLgapprovaldecision = (int?)from.LGApprovalDecision;

            // Manufacturing fields

            to.AdoxioIspackaging = from.IsPackaging;
            to.AdoxioMfgpipedinproduct = (int?)from.MfgPipedInProduct;
            to.AdoxioMfgbrewpubonsite = (int?)from.MfgBrewPubOnSite;
            to.AdoxioMfgacresoffruit = from.MfgAcresOfFruit;
            to.AdoxioMfgacresofgrapes = from.MfgAcresOfGrapes;
            to.AdoxioMfgacresofhoney = from.MfgAcresOfHoney;
            to.AdoxioMfgmeetsproductionminimum = from.MfgMeetsProductionMinimum;
            to.AdoxioMfgstepblending = from.MfgStepBlending;
            to.AdoxioMfgstepcrushing = from.MfgStepCrushing;
            to.AdoxioMfgstepfiltering = from.MfgStepFiltering;
            to.AdoxioMfgstepsecfermorcarb = from.MfgStepSecFermOrCarb;
            to.AdoxioMfgusesneutralgrainspirits = (int?)from.MfgUsesNeutralGrainSpirits;
            to.AdoxioPidlist = from.PidList;
            to.AdoxioIspermittedinzoning = from.IsPermittedInZoning;

            // Permanent Change to a Licensee
            to.AdoxioFirstnameold = from.FirstNameOld;
            to.AdoxioFirstnamenew = from.FirstNameNew;
            to.AdoxioLastnameold = from.LastNameOld;
            to.AdoxioLastnamenew = from.LastNameNew;

            to.AdoxioCsinternaltransferofshares = from.CsInternalTransferOfShares;
            to.AdoxioCsexternaltransferofshares = from.CsExternalTransferOfShares;
            to.AdoxioCschangeofdirectorsorofficers = from.CsChangeOfDirectorsOrOfficers;
            to.AdoxioCsnamechangelicenseecorporation = from.CsNameChangeLicenseeCorporation;
            to.AdoxioCsnamechangelicenseepartnership = from.CsNameChangeLicenseePartnership;
            to.AdoxioCsnamechangelicenseesociety = from.CsNameChangeLicenseeSociety;
            to.AdoxioCsnamechangeperson = from.CsNameChangeLicenseePerson;
            to.AdoxioCsadditionofreceiverorexecutor = from.CsAdditionalReceiverOrExecutor;

            // Manufacturing structural change fields

            to.AdoxioPatiocompdescription = from.PatioCompDescription;
            to.AdoxioPatiolocationdescription = from.PatioLocationDescription;
            to.AdoxioPatioaccessdescription = from.PatioAccessDescription;
            to.AdoxioPatioisliquorcarried = from.PatioIsLiquorCarried;
            to.AdoxioPatioliquorcarrieddescription = from.PatioLiquorCarriedDescription;
            to.AdoxioPatioaccesscontroldescription = from.PatioAccessControlDescription;
            to.AdoxioLocatedabovedescription = @from.LocatedAboveDescription;
            to.AdoxioPatioservicebar = from.PatioServiceBar;

            to.AdoxioProposedestablishmentisalr = from.IsAlr;
            to.AdoxioHascooleraccess = from.HasCoolerAccess;

            to.AdoxioLocatedaboveother = from.LocatedAboveOther;

            if (from.IsOnINLand == true)
            {
                to.AdoxioIsoninland = (int)DefaultYesNoLookup.Yes;
            }
            else
            {
                to.AdoxioIsoninland = null;
            }

            // RLRS - Eligibility fields

            to.AdoxioIsrlrslocatedinruralcommunityalone = from.IsRlrsLocatedInRuralCommunityAlone;
            to.AdoxioIsrlrslocatedattouristdestinationalone = from.IsRlrsLocatedAtTouristDestinationAlone;
            to.AdoxioDescriberlrsresortcommunity = from.RlrsResortCommunityDescription;
            to.AdoxioHasyearroundallweatherroadaccess = from.HasYearRoundAllWeatherRoadAccess;
            to.AdoxioDoesgeneralstoreoperateseasonally = from.DoesGeneralStoreOperateSeasonally;
            to.AdoxioSurroundingresidentsofrlrs = from.SurroundingResidentsOfRlrs;
            to.AdoxioIsrlrsatleast10kmfromanotherstore = from.IsRlrsAtLeast10kmFromAnotherStore;
            to.AdoxioIsapplicantownerofstore = from.IsApplicantOwnerOfStore;
            to.AdoxioLegalandbeneficialownersofstore = from.LegalAndBeneficialOwnersOfStore;
            to.AdoxioIsapplicantfranchiseoraffiliated = from.IsApplicantFranchiseOrAffiliated;
            to.AdoxioFranchiseoraffiliatedbusiness = from.FranchiseOrAffiliatedBusiness;
            to.AdoxioHassufficientrangeofproducts = from.HasSufficientRangeOfProducts;
            to.AdoxioHasotherproducts = from.HasOtherProducts;
            to.AdoxioHasadditionalservices = from.HasAdditionalServices;
            to.AdoxioStoreopendate = from.StoreOpenDate;
            to.AdoxioConfirmliquorsalesisnotprimarybusiness = from.ConfirmLiquorSalesIsNotPrimaryBusiness;
            to.AdoxioManufacturerproductionamountforprevyear = from.ManufacturerProductionAmountForPrevYear;
            to.AdoxioManufacturerproductionamountunit = from.ManufacturerProductionAmountUnit;
            //LCSD-6304
            to.AdoxioPicnicconfirmslgfnsupportscapacity = from.PicnicConfirmLGFNCapacity;
            to.AdoxioPicnicconfirmszoning = from.PicnicConfirmZoning;
            to.AdoxioPicnicreadandaccepttermsandconditions = from.PicnicReadAndAccept;
        }


        public static void CopyValues(this MicrosoftDynamicsCRMadoxioApplication to, CovidApplication from)
        {
            to.AdoxioName = from.Name;
            //to.Adoxio_jobnumber = from.jobNumber;            
            to.AdoxioEstablishmentpropsedname = from.EstablishmentName;
            to.AdoxioEstablishmentaddressstreet = from.EstablishmentAddressStreet;
            to.AdoxioEstablishmentaddresscity = from.EstablishmentAddressCity;
            to.AdoxioEstablishmentaddresspostalcode = from.EstablishmentAddressPostalCode;
            // 12-10-2019 - Removed the update to AdoxioAddressCity as the Dynamics workflow handles that.

            to.AdoxioEstablishmentparcelid = from.EstablishmentParcelId;
            to.AdoxioEstablishmentphone = from.EstablishmentPhone;
            to.AdoxioEstablishmentemail = from.EstablishmentEmail;

            to.AdoxioContactpersonfirstname = from.ContactPersonFirstName;
            to.AdoxioContactpersonlastname = from.ContactPersonLastName;
            to.AdoxioRole = from.ContactPersonRole;
            to.AdoxioEmail = from.ContactPersonEmail;
            to.AdoxioContactpersonphone = from.ContactPersonPhone;
            to.AdoxioAuthorizedtosubmit = from.AuthorizedToSubmit;
            to.AdoxioAdditionalpropertyinformation = from.AdditionalPropertyInformation;



            //store opening


            to.AdoxioAuthorizedtosubmit = from.AuthorizedToSubmit;

            to.AdoxioApplicanttype = (int?)from.ApplicantType;

            // catering fields
            to.AdoxioIsapplicationcomplete = (int?)from.IsApplicationComplete;
            
        }

        public static void CopyValuesForCovidApplication(this MicrosoftDynamicsCRMadoxioApplication to, CovidApplication from)
        {
            to.CopyValues(from);

            to.AdoxioProposedestablishmentisalr = from.ProposedEstablishmentIsAlr;

            to.AdoxioNameofapplicant = from.NameOfApplicant;

            /* 2020/5/15 - Copy values has a comment that says to not copy this fields because of a dynamics workflow
            * Including this fields for the covid application as the workflow should not be relevant ?
            */
            to.AdoxioAddressstreet = from.AddressStreet;
            to.AdoxioAddresscity = from.AddressCity;
            to.AdoxioAddresspostalcode = from.AddressPostalCode;
        }
        public static void CopyValuesForChangeOfLocation(this MicrosoftDynamicsCRMadoxioApplication to, MicrosoftDynamicsCRMadoxioLicences from, bool copyAddress)
        {
            // copy establishment information
            if (copyAddress)
            {
                // 12-10-2019 - Removed set to AdoxioAddressCity as it is set by Dynamics Workflow, not the portal                
                to.AdoxioEstablishmentaddressstreet = from.AdoxioEstablishmentaddressstreet;
                to.AdoxioEstablishmentaddresscity = from.AdoxioEstablishmentaddresscity;
                to.AdoxioEstablishmentaddresspostalcode = from.AdoxioEstablishmentaddresspostalcode;
            }

            if (from.AdoxioEstablishment != null)
            {
                to.AdoxioEstablishmentpropsedname = from.AdoxioEstablishment.AdoxioName;
                to.AdoxioEstablishmentemail = from.AdoxioEstablishment.AdoxioEmail;
                to.AdoxioEstablishmentphone = from.AdoxioEstablishment.AdoxioPhone;
                to.AdoxioEstablishmentparcelid = from.AdoxioEstablishment.AdoxioParcelid;
                to.AdoxioIsoninland = from.AdoxioEstablishment.AdoxioIsoninland;
                to.AdoxioPoliceJurisdictionId = from.AdoxioEstablishment.AdoxioPDJurisdiction;
                to.AdoxioLocalgovindigenousnationid = from.AdoxioEstablishment.AdoxioLGIN;
            }

        }

        public static MicrosoftDynamicsCRMadoxioLicencetype GetCachedLicenceType(string id, IDynamicsClient dynamicsClient, IMemoryCache memoryCache)
        {
            string cacheKey = CacheKeys.LicenceTypePrefix + id;
            if (memoryCache == null || !memoryCache.TryGetValue(cacheKey, out MicrosoftDynamicsCRMadoxioLicencetype result))
            {
                // Key not in cache, so get data.
                result = dynamicsClient.GetAdoxioLicencetypeById(id);

                if (memoryCache != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(365));
                    // Save data in cache.
                    memoryCache.Set(cacheKey, result, cacheEntryOptions);
                }
            }

            return result;
        }

        public static List<MicrosoftDynamicsCRMpicklistAttributeMetadata> GetCachedApplicationPicklists(this IDynamicsClient dynamicsClient, IMemoryCache memoryCache)
        {
            string cacheKey = CacheKeys.PicklistTypePrefix + "Application";
            if (memoryCache == null || !memoryCache.TryGetValue(cacheKey, out List<MicrosoftDynamicsCRMpicklistAttributeMetadata> result))
            {
                // Key not in cache, so get data.
                try
                {
                    result = dynamicsClient.Entitydefinitions.GetEntityPicklists("adoxio_application").Value;
                    if (memoryCache != null)
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(365));
                        // Save data in cache.
                        memoryCache.Set(cacheKey, result, cacheEntryOptions);
                    }
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e, "ERROR getting accounts picklist metadata");
                    result = new List<MicrosoftDynamicsCRMpicklistAttributeMetadata>();
                }


            }

            return result;
        }


        /// <summary>
        /// Get the licence type
        /// </summary>
        /// <param name="application"></param>
        /// <param name="dynamicsClient"></param>
        /// <param name="memoryCache"></param>
        public static void PopulateLicenceType(this MicrosoftDynamicsCRMadoxioApplication application, IDynamicsClient dynamicsClient, IMemoryCache memoryCache)
        {
            if (application._adoxioLicencetypeValue != null)
            {
                application.AdoxioLicenceType = GetCachedLicenceType(application._adoxioLicencetypeValue, dynamicsClient, memoryCache);
            }

            if (application.AdoxioAssignedLicence != null && application.AdoxioAssignedLicence._adoxioLicencetypeValue != null)
            {
                application.AdoxioAssignedLicence.AdoxioLicenceType = GetCachedLicenceType(application.AdoxioAssignedLicence._adoxioLicencetypeValue, dynamicsClient, memoryCache);
            }
        }

        public async static Task<ViewModels.Application> ToViewModel(this MicrosoftDynamicsCRMadoxioApplication dynamicsApplication, IDynamicsClient dynamicsClient, IMemoryCache cache, ILogger logger)
        {

            ViewModels.Application applicationVM = new ViewModels.Application
            {
                Name = dynamicsApplication.AdoxioName,
                JobNumber = dynamicsApplication.AdoxioJobnumber,
                //get establishment name and address
                EstablishmentName = dynamicsApplication.AdoxioEstablishmentpropsedname,
                EstablishmentAddressStreet = dynamicsApplication.AdoxioEstablishmentaddressstreet,
                EstablishmentAddressCity = dynamicsApplication.AdoxioEstablishmentaddresscity,
                EstablishmentAddressPostalCode = dynamicsApplication.AdoxioEstablishmentaddresspostalcode,
                EstablishmentAddress = dynamicsApplication.AdoxioEstablishmentaddressstreet
                                                    + ", " + dynamicsApplication.AdoxioEstablishmentaddresscity
                                                    + " " + dynamicsApplication.AdoxioEstablishmentaddresspostalcode,
                EstablishmentPhone = dynamicsApplication.AdoxioEstablishmentphone,
                EstablishmentEmail = dynamicsApplication.AdoxioEstablishmentemail,
                FederalProducerNames = dynamicsApplication.AdoxioFederalproducernames,
                IsApplicationComplete = (GeneralYesNo?)dynamicsApplication.AdoxioIsapplicationcomplete,

                RenewalCriminalOffenceCheck = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalcriminaloffencecheck,
                RenewalUnreportedSaleOfBusiness = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalunreportedsaleofbusiness,
                RenewalBusinessType = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalbusinesstype,
                RenewalTiedhouse = (ValueNotChanged?)dynamicsApplication.AdoxioRenewaltiedhouse,
                RenewalOrgLeadership = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalorgleadership,
                Renewalkeypersonnel = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalkeypersonnel,
                RenewalShareholders = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalshareholders,
                RenewalOutstandingFines = (ValueNotChanged?)dynamicsApplication.AdoxioRenewaloutstandingfines,
                RenewalBranding = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalbranding,
                RenewalSignage = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalsignage,
                RenewalEstablishmentAddress = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalestablishmentaddress,
                RenewalValidInterest = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalvalidinterest,
                RenewalZoning = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalzoning,
                RenewalFloorPlan = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalfloorplan,
                RenewalSiteMap = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalsitemap,
                TiedhouseFederalInterest = (ValueNotChanged?)dynamicsApplication.AdoxioRenewaltiedhousefederalinterest,
                RenewalDUI = (ValueNotChanged?)dynamicsApplication.AdoxioRenewaldui,
                RenewalThirdParty = (ValueNotChanged?)dynamicsApplication.AdoxioRenewalthirdparty,

                AuthorizedToSubmit = dynamicsApplication.AdoxioAuthorizedtosubmit,
                SignatureAgreement = dynamicsApplication.AdoxioSignatureagreement,

                LicenceFeeInvoicePaid = (dynamicsApplication.AdoxioLicencefeeinvoicepaid != null && dynamicsApplication.AdoxioLicencefeeinvoicepaid == true),

                // set a couple of read-only flags to indicate status
                IsPaid = (dynamicsApplication.AdoxioPaymentrecieved != null && (bool)dynamicsApplication.AdoxioPaymentrecieved),

                IndigenousNationId = dynamicsApplication._adoxioLocalgovindigenousnationidValue,
                PoliceJurisdictionId = dynamicsApplication._adoxioPolicejurisdictionidValue,

                //get parcel id
                EstablishmentParcelId = dynamicsApplication.AdoxioEstablishmentparcelid,

                //get additional property info
                AdditionalPropertyInformation = dynamicsApplication.AdoxioAdditionalpropertyinformation,
                InvoiceId = dynamicsApplication._adoxioInvoiceValue,
                SecondaryInvoiceId = dynamicsApplication._adoxioSecondaryapplicationinvoiceValue,

                PaymentReceivedDate = dynamicsApplication.AdoxioPaymentreceiveddate,
                Description1 = dynamicsApplication.AdoxioDescription1,
                Description2 = dynamicsApplication.AdoxioDescription2,
                TempDateFrom = dynamicsApplication.AdoxioTempdatefrom,
                TempDateTo = dynamicsApplication.AdoxioTempdateto,

                IsMonth01 = dynamicsApplication.AdoxioM01,
                IsMonth02 = dynamicsApplication.AdoxioM02,
                IsMonth03 = dynamicsApplication.AdoxioM03,
                IsMonth04 = dynamicsApplication.AdoxioM04,
                IsMonth05 = dynamicsApplication.AdoxioM05,
                IsMonth06 = dynamicsApplication.AdoxioM06,
                IsMonth07 = dynamicsApplication.AdoxioM07,
                IsMonth08 = dynamicsApplication.AdoxioM08,
                IsMonth09 = dynamicsApplication.AdoxioM09,
                IsMonth10 = dynamicsApplication.AdoxioM10,
                IsMonth11 = dynamicsApplication.AdoxioM11,
                IsMonth12 = dynamicsApplication.AdoxioM12,

                //get contact details
                ContactPersonFirstName = dynamicsApplication.AdoxioContactpersonfirstname,
                ContactPersonLastName = dynamicsApplication.AdoxioContactpersonlastname,
                ContactPersonRole = dynamicsApplication.AdoxioRole,
                ContactPersonEmail = dynamicsApplication.AdoxioEmail,
                ContactPersonPhone = dynamicsApplication.AdoxioContactpersonphone,

                //get record audit info
                CreatedOn = dynamicsApplication.Createdon,
                ModifiedOn = dynamicsApplication.Modifiedon,

                //store opening 
                IsReadyWorkers = dynamicsApplication.AdoxioIsreadyworkers,
                IsReadyNameBranding = dynamicsApplication.AdoxioIsreadynamebranding,
                IsReadyDisplays = dynamicsApplication.AdoxioIsreadydisplays,
                IsReadyIntruderAlarm = dynamicsApplication.AdoxioIsreadyintruderalarm,
                IsReadyFireAlarm = dynamicsApplication.AdoxioIsreadyfirealarm,
                IsReadyLockedCases = dynamicsApplication.AdoxioIsreadylockedcases,
                IsReadyLockedStorage = dynamicsApplication.AdoxioIsreadylockedstorage,
                IsReadyPerimeter = dynamicsApplication.AdoxioIsreadyperimeter,
                IsReadyRetailArea = dynamicsApplication.AdoxioIsreadyretailarea,
                IsReadyStorage = dynamicsApplication.AdoxioIsreadystorage,
                IsReadyExtranceExit = dynamicsApplication.AdoxioIsreadyentranceexit,
                IsReadySurveillanceNotice = dynamicsApplication.AdoxioIsreadysurveillancenotice,
                IsReadyProductNotVisibleOutside = dynamicsApplication.AdoxioIsreadyproductnotvisibleoutside,
                IsLocatedInGroceryStore = dynamicsApplication.AdoxioIslocatedingrocerystore,
                Establishmentopeningdate = dynamicsApplication.AdoxioEstablishmentopeningdate,
                IsReadyValidInterest = dynamicsApplication.AdoxioIsreadyvalidinterest,

                IsHasPatio = dynamicsApplication.AdoxioIshaspatio,

                LgNoObjection = dynamicsApplication.AdoxioLgnoobjection,
                // LgInName
                LGNameOfOfficial = dynamicsApplication.AdoxioLgnameofofficial,
                LGTitlePosition = dynamicsApplication.AdoxioLgtitleposition,
                LGContactPhone = dynamicsApplication.AdoxioLgcontactphone,
                LGContactEmail = dynamicsApplication.AdoxioLgcontactemail,
                LGDecisionSubmissionDate = dynamicsApplication.AdoxioLgdecisionsubmissiondate,
                LgInName = dynamicsApplication?.AdoxioLocalgovindigenousnationid?.AdoxioName,
                LGApprovalDecision = (LGDecision?)dynamicsApplication.AdoxioLgapprovaldecision,
                LgZoning = (Zoning?)dynamicsApplication.AdoxioLgzoning,
                LGDecisionComments = dynamicsApplication.AdoxioLgdecisioncomments,
                DateApplicantSentToLG = dynamicsApplication.AdoxioDatesentlgin,

                // Catering fields.

                PreviousApplicationDetails = dynamicsApplication.AdoxioPreviouslicenceapplicationdetails,

                LiquorIndustryConnectionsDetails = dynamicsApplication.AdoxioLiquorindustryconnectionsdetails,

                OtherBusinessesDetails = dynamicsApplication.AdoxioOtherbusinesssamelocationdetails,
                ServiceAreas = new List<CapacityArea>(),
                OutsideAreas = new List<CapacityArea>(),
                CapacityArea = new List<CapacityArea>(),

                // Manufacturing fields

                IsPackaging = dynamicsApplication.AdoxioIspackaging,

                MfgAcresOfFruit = dynamicsApplication.AdoxioMfgacresoffruit,
                MfgAcresOfGrapes = dynamicsApplication.AdoxioMfgacresofgrapes,
                MfgAcresOfHoney = dynamicsApplication.AdoxioMfgacresofhoney,
                MfgMeetsProductionMinimum = dynamicsApplication.AdoxioMfgmeetsproductionminimum,
                MfgStepBlending = dynamicsApplication.AdoxioMfgstepblending,
                MfgStepCrushing = dynamicsApplication.AdoxioMfgstepcrushing,
                MfgStepFiltering = dynamicsApplication.AdoxioMfgstepfiltering,
                MfgStepSecFermOrCarb = dynamicsApplication.AdoxioMfgstepsecfermorcarb,
                IsOwnerBusiness = dynamicsApplication.AdoxioIsownerbusiness,
                HasValidInterest = dynamicsApplication.AdoxioIsownerhasvalidinterest,
                WillHaveValidInterest = dynamicsApplication.AdoxioIsownerwillhavevalidinterest,
                ZoningStatus = dynamicsApplication.AdoxioZoningstatus,


                PidList = dynamicsApplication.AdoxioPidlist,
                IsPermittedInZoning = dynamicsApplication.AdoxioIspermittedinzoning,

                // Manufacturing structural change fields

                PatioCompDescription = dynamicsApplication.AdoxioPatiocompdescription,
                PatioLocationDescription = dynamicsApplication.AdoxioPatiolocationdescription,
                PatioAccessDescription = dynamicsApplication.AdoxioPatioaccessdescription,
                PatioIsLiquorCarried = dynamicsApplication.AdoxioPatioisliquorcarried,
                PatioLiquorCarriedDescription = dynamicsApplication.AdoxioPatioliquorcarrieddescription,
                PatioAccessControlDescription = dynamicsApplication.AdoxioPatioaccesscontroldescription,
                IsAlr = dynamicsApplication.AdoxioProposedestablishmentisalr.HasValue && (bool)dynamicsApplication.AdoxioProposedestablishmentisalr,
                HasCoolerAccess = dynamicsApplication.AdoxioHascooleraccess.HasValue && (bool)dynamicsApplication.AdoxioHascooleraccess,

                // Permanent Change to a Licensee
                FirstNameOld = dynamicsApplication.AdoxioFirstnameold,
                FirstNameNew = dynamicsApplication.AdoxioFirstnamenew,
                LastNameOld = dynamicsApplication.AdoxioLastnameold,
                LastNameNew = dynamicsApplication.AdoxioLastnamenew,
                CsInternalTransferOfShares = dynamicsApplication.AdoxioCsinternaltransferofshares,
                CsExternalTransferOfShares = dynamicsApplication.AdoxioCsexternaltransferofshares,
                CsChangeOfDirectorsOrOfficers = dynamicsApplication.AdoxioCschangeofdirectorsorofficers,
                CsNameChangeLicenseeCorporation = dynamicsApplication.AdoxioCsnamechangelicenseecorporation,
                CsNameChangeLicenseePartnership = dynamicsApplication.AdoxioCsnamechangelicenseepartnership,
                CsNameChangeLicenseeSociety = dynamicsApplication.AdoxioCsnamechangelicenseesociety,
                CsNameChangeLicenseePerson = dynamicsApplication.AdoxioCsnamechangeperson,
                CsAdditionalReceiverOrExecutor = dynamicsApplication.AdoxioCsadditionofreceiverorexecutor,
                PrimaryInvoicePaid = dynamicsApplication.AdoxioPrimaryapplicationinvoicepaid == 1,
                SecondaryInvoicePaid = dynamicsApplication.AdoxioSecondaryapplicationinvoicepaid == 1,
                IsOnINLand = ConvertYesNoLookupToBool(dynamicsApplication.AdoxioIsoninland),

                LocatedAboveOther = dynamicsApplication.AdoxioLocatedaboveother,

                // Eligibility fields

                IsRlrsLocatedInRuralCommunityAlone = dynamicsApplication.AdoxioIsrlrslocatedinruralcommunityalone,
                IsRlrsLocatedAtTouristDestinationAlone = dynamicsApplication.AdoxioIsrlrslocatedattouristdestinationalone,
                RlrsResortCommunityDescription = dynamicsApplication.AdoxioDescriberlrsresortcommunity,
                HasYearRoundAllWeatherRoadAccess = dynamicsApplication.AdoxioHasyearroundallweatherroadaccess,
                DoesGeneralStoreOperateSeasonally = dynamicsApplication.AdoxioDoesgeneralstoreoperateseasonally,
                SurroundingResidentsOfRlrs = dynamicsApplication.AdoxioSurroundingresidentsofrlrs,
                IsRlrsAtLeast10kmFromAnotherStore = dynamicsApplication.AdoxioIsrlrsatleast10kmfromanotherstore,
                IsApplicantOwnerOfStore = dynamicsApplication.AdoxioIsapplicantownerofstore,
                LegalAndBeneficialOwnersOfStore = dynamicsApplication.AdoxioLegalandbeneficialownersofstore,
                IsApplicantFranchiseOrAffiliated = dynamicsApplication.AdoxioIsapplicantfranchiseoraffiliated,
                FranchiseOrAffiliatedBusiness = dynamicsApplication.AdoxioFranchiseoraffiliatedbusiness,

                HasSufficientRangeOfProducts = dynamicsApplication.AdoxioHassufficientrangeofproducts,
                HasOtherProducts = dynamicsApplication.AdoxioHasotherproducts,
                HasAdditionalServices = dynamicsApplication.AdoxioHasadditionalservices,
                StoreOpenDate = dynamicsApplication.AdoxioStoreopendate,
                ConfirmLiquorSalesIsNotPrimaryBusiness = dynamicsApplication.AdoxioConfirmliquorsalesisnotprimarybusiness,
                Pin = dynamicsApplication.AdoxioPin,
                ManufacturerProductionAmountForPrevYear = dynamicsApplication.AdoxioManufacturerproductionamountforprevyear,
                ManufacturerProductionAmountUnit = dynamicsApplication.AdoxioManufacturerproductionamountunit,
                //LCSD-6304
                PicnicConfirmLGFNCapacity = dynamicsApplication.AdoxioPicnicconfirmslgfnsupportscapacity,
                PicnicConfirmZoning = dynamicsApplication.AdoxioPicnicconfirmszoning,
                PicnicReadAndAccept = dynamicsApplication.AdoxioPicnicreadandaccepttermsandconditions
            };


            // mfg fields

            if (dynamicsApplication.AdoxioMfgpipedinproduct != null)
            {
                applicationVM.MfgPipedInProduct = (YesNoNotApplicable?)dynamicsApplication.AdoxioMfgpipedinproduct;
            }
            if (dynamicsApplication.AdoxioMfgbrewpubonsite != null)
            {
                applicationVM.MfgBrewPubOnSite = (YesNoNotApplicable?)dynamicsApplication.AdoxioMfgbrewpubonsite;
            }
            if (dynamicsApplication.AdoxioMfgusesneutralgrainspirits != null)
            {
                applicationVM.MfgUsesNeutralGrainSpirits = (YesNoNotApplicable)dynamicsApplication.AdoxioMfgusesneutralgrainspirits;
            }


            if (dynamicsApplication.AdoxioLocatedabovedescription != null)
            {
                applicationVM.LocatedAboveDescription = dynamicsApplication.AdoxioLocatedabovedescription;
            }

            if (dynamicsApplication.AdoxioPatioservicebar != null)
            {
                applicationVM.PatioServiceBar = dynamicsApplication.AdoxioPatioservicebar;
            }

            // Catering yes / no fields
            if (dynamicsApplication.AdoxioPreviouslicenceapplication != null)
            {
                applicationVM.PreviousApplication = dynamicsApplication.AdoxioPreviouslicenceapplication;
            }

            if (dynamicsApplication.AdoxioRuralagencystoreappointment != null)
            {
                applicationVM.RuralAgencyStoreAppointment = dynamicsApplication.AdoxioRuralagencystoreappointment;
            }

            if (dynamicsApplication.AdoxioLiquorindustryconnections != null)
            {
                applicationVM.LiquorIndustryConnections = dynamicsApplication.AdoxioLiquorindustryconnections;
            }

            if (dynamicsApplication.AdoxioOtherbusinessesatthesamelocation != null)
            {
                applicationVM.OtherBusinesses = dynamicsApplication.AdoxioOtherbusinessesatthesamelocation;
            }


            // id
            if (dynamicsApplication.AdoxioApplicationid != null)
            {
                applicationVM.Id = dynamicsApplication.AdoxioApplicationid;

                // service areas
                var filter = $"_adoxio_applicationid_value eq {dynamicsApplication.AdoxioApplicationid}";
                try
                {
                    IList<MicrosoftDynamicsCRMadoxioServicearea> areas = dynamicsClient.Serviceareas.Get(filter: filter).Value;
                    foreach (MicrosoftDynamicsCRMadoxioServicearea area in areas)
                    {
                        if (area.AdoxioAreacategory == (int?)AdoxioAreaCategories.Service)
                        {
                            applicationVM.ServiceAreas.Add(area.ToViewModel());
                        }
                        else if (area.AdoxioAreacategory == (int?)AdoxioAreaCategories.OutdoorArea)
                        {
                            applicationVM.OutsideAreas.Add(area.ToViewModel());
                        }
                        else if (area.AdoxioAreacategory == (int?)AdoxioAreaCategories.Capacity)
                        {
                            applicationVM.CapacityArea.Add(area.ToViewModel());
                        }
                    }
                }
                catch (HttpOperationException httpOperationException)
                {
                    logger.LogError(httpOperationException, "Error getting service areas.");
                }

                // service hours
                try
                {
                    var appFilter = $"_adoxio_application_value eq {dynamicsApplication.AdoxioApplicationid}";
                    IList<MicrosoftDynamicsCRMadoxioHoursofservice> hours = dynamicsClient.Hoursofservices.Get(filter: appFilter).Value;
                    if (hours.Count > 0)
                    {
                        MicrosoftDynamicsCRMadoxioHoursofservice hourEntity = hours[0];
                        applicationVM.ServiceHoursSundayOpen = (ServiceHours?)hourEntity.AdoxioSundayopen;
                        applicationVM.ServiceHoursSundayClose = (ServiceHours?)hourEntity.AdoxioSundayclose;
                        applicationVM.ServiceHoursMondayOpen = (ServiceHours?)hourEntity.AdoxioMondayopen;
                        applicationVM.ServiceHoursMondayClose = (ServiceHours?)hourEntity.AdoxioMondayclose;
                        applicationVM.ServiceHoursTuesdayOpen = (ServiceHours?)hourEntity.AdoxioTuesdayopen;
                        applicationVM.ServiceHoursTuesdayClose = (ServiceHours?)hourEntity.AdoxioTuesdayclose;
                        applicationVM.ServiceHoursWednesdayOpen = (ServiceHours?)hourEntity.AdoxioWednesdayopen;
                        applicationVM.ServiceHoursWednesdayClose = (ServiceHours?)hourEntity.AdoxioWednesdayclose;
                        applicationVM.ServiceHoursThursdayOpen = (ServiceHours?)hourEntity.AdoxioThursdayopen;
                        applicationVM.ServiceHoursThursdayClose = (ServiceHours?)hourEntity.AdoxioThursdayclose;
                        applicationVM.ServiceHoursFridayOpen = (ServiceHours?)hourEntity.AdoxioFridayopen;
                        applicationVM.ServiceHoursFridayClose = (ServiceHours?)hourEntity.AdoxioFridayclose;
                        applicationVM.ServiceHoursSaturdayOpen = (ServiceHours?)hourEntity.AdoxioSaturdayopen;
                        applicationVM.ServiceHoursSaturdayClose = (ServiceHours?)hourEntity.AdoxioSaturdayclose;
                    }
                }
                catch (HttpOperationException httpOperationException)
                {
                    logger.LogError(httpOperationException, "Error getting service hours.");
                }

            }

            if (dynamicsApplication.Statuscode != null)
            {
                applicationVM.ApplicationStatus = (AdoxioApplicationStatusCodes)dynamicsApplication.Statuscode;
            }

            if (dynamicsApplication.AdoxioApplicanttype != null)
            {
                applicationVM.ApplicantType = (AdoxioApplicantTypeCodes)dynamicsApplication.AdoxioApplicanttype;
            }

            //get applying person from Contact entity
            if (dynamicsApplication._adoxioApplyingpersonValue != null)
            {
                Guid applyingPersonId = Guid.Parse(dynamicsApplication._adoxioApplyingpersonValue);
                var contact = await dynamicsClient.GetContactById(applyingPersonId);
                applicationVM.ApplyingPerson = contact.Fullname;
            }

            if (dynamicsApplication._adoxioApplicantValue != null)
            {
                var applicant = await dynamicsClient.GetAccountByIdAsync(Guid.Parse(dynamicsApplication._adoxioApplicantValue));
                applicationVM.Applicant = applicant.ToViewModel();
            }

            //get license type from Adoxio_licencetype entity
            if (dynamicsApplication._adoxioLicencetypeValue != null)
            {
                Guid adoxio_licencetypeId = Guid.Parse(dynamicsApplication._adoxioLicencetypeValue);
                var adoxio_licencetype = dynamicsClient.GetAdoxioLicencetypeById(adoxio_licencetypeId);
                applicationVM.LicenseType = adoxio_licencetype.AdoxioName;
            }

            // get the license sub type.

            if (dynamicsApplication._adoxioLicencesubcategoryidValue != null)
            {
                try
                {
                    var adoxioLicencesubcategory = dynamicsClient.Licencesubcategories.GetByKey(dynamicsApplication._adoxioLicencesubcategoryidValue);
                    applicationVM.LicenceSubCategory = adoxioLicencesubcategory.AdoxioName;
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"Problem getting licence sub category {dynamicsApplication._adoxioLicencesubcategoryidValue}");
                }
            }

            if (dynamicsApplication.AdoxioAppchecklistfinaldecision != null)
            {
                applicationVM.AppChecklistFinalDecision = (AdoxioFinalDecisionCodes)dynamicsApplication.AdoxioAppchecklistfinaldecision;
            }

            //get payment info
            applicationVM.InvoiceTrigger = (GeneralYesNo?)dynamicsApplication.AdoxioInvoicetrigger;
            applicationVM.IsSubmitted = (dynamicsApplication.AdoxioInvoicetrigger == 1);


            if (dynamicsApplication.AdoxioLicenceFeeInvoice != null)
            {
                applicationVM.LicenceFeeInvoice = dynamicsApplication.AdoxioLicenceFeeInvoice.ToViewModel();
            }

            if (dynamicsApplication.AdoxioAssignedLicence != null)
            {
                applicationVM.AssignedLicence = dynamicsApplication.AdoxioAssignedLicence.ToViewModel(dynamicsClient);
            }

            if (dynamicsApplication.AdoxioApplicationTypeId != null)
            {
                applicationVM.ApplicationType = dynamicsApplication.AdoxioApplicationTypeId.ToViewModel();

                if (!string.IsNullOrEmpty(applicationVM.ApplicationType.FormReference))
                {
                    applicationVM.ApplicationType.DynamicsForm = dynamicsClient.GetSystemformViewModel(cache, logger, applicationVM.ApplicationType.FormReference);
                }
            }
            if (dynamicsApplication.AdoxioApplicationAdoxioTiedhouseconnectionApplication != null)
            {
                var tiedHouse = dynamicsApplication.AdoxioApplicationAdoxioTiedhouseconnectionApplication.FirstOrDefault();
                if (tiedHouse != null)
                {
                    applicationVM.TiedHouse = tiedHouse.ToViewModel();
                }
            }

            if (dynamicsApplication.AdoxioPoliceJurisdictionId != null)
            {
                applicationVM.PoliceJurisdiction = dynamicsApplication.AdoxioPoliceJurisdictionId.ToViewModel();
            }

            if (dynamicsApplication.AdoxioLocalgovindigenousnationid != null)
            {
                var filter = $"_adoxio_lginlinkid_value eq {dynamicsApplication.AdoxioLocalgovindigenousnationid.AdoxioLocalgovindigenousnationid} and websiteurl ne null";
                var linkedAccount = (await dynamicsClient.Accounts.GetAsync(filter: filter)).Value.FirstOrDefault();
                applicationVM.IndigenousNation = dynamicsApplication.AdoxioLocalgovindigenousnationid.ToViewModel();

                if (linkedAccount != null)
                {
                    applicationVM.IndigenousNation.WebsiteUrl = linkedAccount.Websiteurl;
                }
            }

            applicationVM.PrevPaymentFailed = (dynamicsApplication._adoxioInvoiceValue != null) && (!applicationVM.IsSubmitted);


            if (dynamicsApplication.AdoxioAdoxioApplicationAdoxioApplicationtermsconditionslimitationApplication !=
                null && dynamicsApplication.AdoxioAdoxioApplicationAdoxioApplicationtermsconditionslimitationApplication.Count > 0)
            {
                // add the related term info.
                var term = dynamicsApplication
                    .AdoxioAdoxioApplicationAdoxioApplicationtermsconditionslimitationApplication.First();
                applicationVM.TermConditionId = term.AdoxioApplicationtermsconditionslimitationid;
                applicationVM.TermConditionOriginalText = term.AdoxioTermsandconditions;

            }

            if(dynamicsApplication.AdoxioRelatedLicence != null)
            {
                applicationVM.RelatedLicenceNumber = dynamicsApplication.AdoxioRelatedLicence.AdoxioLicencenumber;
            }

            return applicationVM;
        }


        public async static Task<CovidApplication> ToCovidViewModel(this MicrosoftDynamicsCRMadoxioApplication dynamicsApplication, IDynamicsClient dynamicsClient, IMemoryCache cache, ILogger logger)
        {
            CovidApplication applicationVM = new CovidApplication
            {
                Name = dynamicsApplication.AdoxioName,
                JobNumber = dynamicsApplication.AdoxioJobnumber,
                //get establishment name and address
                EstablishmentName = dynamicsApplication.AdoxioEstablishmentpropsedname,
                EstablishmentAddressStreet = dynamicsApplication.AdoxioEstablishmentaddressstreet,
                EstablishmentAddressCity = dynamicsApplication.AdoxioEstablishmentaddresscity,
                EstablishmentAddressPostalCode = dynamicsApplication.AdoxioEstablishmentaddresspostalcode,
                EstablishmentAddress = dynamicsApplication.AdoxioEstablishmentaddressstreet
                                                    + ", " + dynamicsApplication.AdoxioEstablishmentaddresscity
                                                    + " " + dynamicsApplication.AdoxioEstablishmentaddresspostalcode,
                EstablishmentPhone = dynamicsApplication.AdoxioEstablishmentphone,
                EstablishmentEmail = dynamicsApplication.AdoxioEstablishmentemail,
                IsApplicationComplete = (GeneralYesNo?)dynamicsApplication.AdoxioIsapplicationcomplete,

                AddressStreet = dynamicsApplication.AdoxioAddressstreet,
                AddressCity = dynamicsApplication.AdoxioAddresscity,
                AddressPostalCode = dynamicsApplication.AdoxioAddresspostalcode,

                NameOfApplicant = dynamicsApplication.AdoxioNameofapplicant,



                AuthorizedToSubmit = dynamicsApplication.AdoxioAuthorizedtosubmit,

                //get parcel id
                EstablishmentParcelId = dynamicsApplication.AdoxioEstablishmentparcelid,

                //get additional property info
                AdditionalPropertyInformation = dynamicsApplication.AdoxioAdditionalpropertyinformation,
                InvoiceId = dynamicsApplication._adoxioInvoiceValue,

                Description1 = dynamicsApplication.AdoxioDescription1,

                //get contact details
                ContactPersonFirstName = dynamicsApplication.AdoxioContactpersonfirstname,
                ContactPersonLastName = dynamicsApplication.AdoxioContactpersonlastname,
                ContactPersonRole = dynamicsApplication.AdoxioRole,
                ContactPersonEmail = dynamicsApplication.AdoxioEmail,
                ContactPersonPhone = dynamicsApplication.AdoxioContactpersonphone,

                //get record audit info
                CreatedOn = dynamicsApplication.Createdon,
                ModifiedOn = dynamicsApplication.Modifiedon,


                ProposedEstablishmentIsAlr = dynamicsApplication.AdoxioProposedestablishmentisalr
                //store opening 

                // Catering fields.

            };



            // id
            if (dynamicsApplication.AdoxioApplicationid != null)
            {
                applicationVM.Id = dynamicsApplication.AdoxioApplicationid;
            }


            if (dynamicsApplication.AdoxioApplicanttype != null)
            {
                applicationVM.ApplicantType = (AdoxioApplicantTypeCodes)dynamicsApplication.AdoxioApplicanttype;
            }

            //get applying person from Contact entity
            if (dynamicsApplication._adoxioApplyingpersonValue != null)
            {
                Guid applyingPersonId = Guid.Parse(dynamicsApplication._adoxioApplyingpersonValue);
                var contact = await dynamicsClient.GetContactById(applyingPersonId);
                applicationVM.ApplyingPerson = contact.Fullname;
            }


            //get license type from Adoxio_licencetype entity
            if (dynamicsApplication._adoxioLicencetypeValue != null)
            {
                Guid adoxio_licencetypeId = Guid.Parse(dynamicsApplication._adoxioLicencetypeValue);
                var adoxio_licencetype = dynamicsClient.GetAdoxioLicencetypeById(adoxio_licencetypeId);
                applicationVM.LicenceType = adoxio_licencetype.AdoxioName;
            }




            if (dynamicsApplication.AdoxioApplicationTypeId != null)
            {
                applicationVM.ApplicationType = dynamicsApplication.AdoxioApplicationTypeId.ToViewModel();

                if (!string.IsNullOrEmpty(applicationVM.ApplicationType.FormReference))
                {
                    applicationVM.ApplicationType.DynamicsForm = dynamicsClient.GetSystemformViewModel(cache, logger, applicationVM.ApplicationType.FormReference);
                }
            }

            return applicationVM;
        }


        public static ApplicationSummary ToSummaryViewModel(this MicrosoftDynamicsCRMadoxioApplication dynamicsApplication)
        {
            ApplicationSummary applicationSummary = new ApplicationSummary
            {
                Name = dynamicsApplication.AdoxioName,
                JobNumber = dynamicsApplication.AdoxioJobnumber,
                //get establishment name and address
                EstablishmentName = dynamicsApplication.AdoxioEstablishmentpropsedname,
                LicenceId = dynamicsApplication._adoxioAssignedlicenceValue,
                IsPaid = (dynamicsApplication.AdoxioPaymentrecieved == true),
                EstablishmentAddressStreet = dynamicsApplication.AdoxioEstablishmentaddressstreet,
                EstablishmentAddressCity = dynamicsApplication.AdoxioEstablishmentaddresscity,
                EstablishmentAddressPostalCode = dynamicsApplication.AdoxioEstablishmentaddresspostalcode,
                EstablishmentAddress =  dynamicsApplication.AdoxioEstablishmentaddressstreet
                                        + ", " + dynamicsApplication.AdoxioEstablishmentaddresscity
                                        + " " + dynamicsApplication.AdoxioEstablishmentaddresspostalcode,
                EstablishmentPhone = dynamicsApplication.AdoxioEstablishmentphone,
                EstablishmentEmail = dynamicsApplication.AdoxioEstablishmentemail,
                EstablishmentParcelId = dynamicsApplication.AdoxioEstablishmentparcelid,
                IndigenousNationId = dynamicsApplication._adoxioLocalgovindigenousnationidValue,
                PoliceJurisdictionId = dynamicsApplication._adoxioPolicejurisdictionidValue,
                IsApplicationComplete = (GeneralYesNo?)dynamicsApplication.AdoxioIsapplicationcomplete,
                IsStructuralChange = (dynamicsApplication?.AdoxioApplicationTypeId?.AdoxioIsstructuralchange == true),
                DateApplicationSubmitted = dynamicsApplication?.AdoxioDateapplicationsubmitted,
                DateApplicantSentToLG = dynamicsApplication?.AdoxioDateapplicantsenttolg,
            };

            // id
            if (dynamicsApplication.AdoxioApplicationid != null)
            {
                applicationSummary.Id = dynamicsApplication.AdoxioApplicationid;
            }

            if (dynamicsApplication.Statuscode != null)
            {
                applicationSummary.ApplicationStatus = StatusUtility.GetTranslatedApplicationStatusV2(dynamicsApplication);
            }

            if (dynamicsApplication.AdoxioApplicationTypeId != null)
            {
                applicationSummary.ApplicationTypeName = dynamicsApplication.AdoxioApplicationTypeId.AdoxioName;
                applicationSummary.IsForLicence = dynamicsApplication.AdoxioApplicationTypeId._adoxioLicencetypeValue != null;
                applicationSummary.Portallabel = dynamicsApplication.AdoxioApplicationTypeId.AdoxioPortallabel;
                applicationSummary.ApplicationTypeCategory = (ApplicationTypeCategory?)dynamicsApplication.AdoxioApplicationTypeId.AdoxioCategory;
            }

            applicationSummary.LGHasApproved = (dynamicsApplication.AdoxioLgapprovaldecision == (int?)LGDecision.Approved)
                    || (dynamicsApplication.AdoxioLgapprovaldecision == (int?)LGDecision.OptOut)
                    || (dynamicsApplication.AdoxioLgapprovaldecision == (int?)LGDecision.Pending)
                    || (dynamicsApplication.AdoxioLgzoning == (int?)Zoning.Allows);

            applicationSummary.IsIndigenousNation = (dynamicsApplication.AdoxioApplicanttype == (int)AdoxioApplicantTypeCodes.IndigenousNation);

            return applicationSummary;
        }
    }
}
