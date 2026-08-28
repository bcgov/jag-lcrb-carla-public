extern alias DV;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using DV::Gov.Lclb.Cllb.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
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
                result = inputValue == (int?)DefaultYesNoLookup.Yes;
            }
            return result;
        }

        public static void CopyValues(this DV::Gov.Lclb.Cllb.Interfaces.adoxio_application to, ViewModels.Application from)
        {
            to.adoxio_name = from.Name;
            to.adoxio_EstablishmentPropsedName = from.EstablishmentName;
            to.adoxio_EstablishmentAddressStreet = from.EstablishmentAddressStreet;
            to.adoxio_EstablishmentAddressCity = from.EstablishmentAddressCity;
            to.adoxio_EstablishmentAddressPostalCode = from.EstablishmentAddressPostalCode;
            to.adoxio_PIN = from.Pin;
            to.adoxio_EstablishmentParcelID = from.EstablishmentParcelId;
            to.adoxio_EstablishmentPhone = from.EstablishmentPhone;
            to.adoxio_EstablishmentEmail = from.EstablishmentEmail;
            to.adoxio_ContactPersonFirstName = from.ContactPersonFirstName;
            to.adoxio_ContactPersonLastName = from.ContactPersonLastName;
            to.adoxio_Role = from.ContactPersonRole;
            to.adoxio_Email = from.ContactPersonEmail;
            to.adoxio_ContactPersonPhone = from.ContactPersonPhone;
            to.adoxio_AuthorizedtoSubmit = from.AuthorizedToSubmit;
            to.adoxio_SignatureAgreement = from.SignatureAgreement;
            to.adoxio_AdditionalPropertyInformation = from.AdditionalPropertyInformation;
            to.adoxio_FederalProducerNames = from.FederalProducerNames;
            to.adoxio_InvoiceTrigger = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno?)(int?)from.InvoiceTrigger;
            to.adoxio_RenewalCriminalOffenceCheck = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalcriminaloffencecheck?)(int?)from.RenewalCriminalOffenceCheck;
            to.adoxio_RenewalUnreportedSaleofBusiness = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalunreportedsaleofbusiness?)(int?)from.RenewalUnreportedSaleOfBusiness;
            to.adoxio_RenewalBusinessType = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalbusinesstype?)(int?)from.RenewalBusinessType;
            to.adoxio_RenewalTiedHouse = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewaltiedhouse?)(int?)from.RenewalTiedhouse;
            to.adoxio_RenewalOrgLeadership = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalorgleadership?)(int?)from.RenewalOrgLeadership;
            to.adoxio_RenewalKeyPersonnel = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalkeypersonnel?)(int?)from.Renewalkeypersonnel;
            to.adoxio_RenewalShareholders = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalshareholders?)(int?)from.RenewalShareholders;
            to.adoxio_RenewalOutstandingFines = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewaloutstandingfines?)(int?)from.RenewalOutstandingFines;
            to.adoxio_RenewalBranding = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalbranding?)(int?)from.RenewalBranding;
            to.adoxio_RenewalSignage = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalsignage?)(int?)from.RenewalSignage;
            to.adoxio_RenewalEstablishmentAddress = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalestablishmentaddress?)(int?)from.RenewalEstablishmentAddress;
            to.adoxio_RenewalValidInterest = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalvalidinterest?)(int?)from.RenewalValidInterest;
            to.adoxio_RenewalZoning = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalzoning?)(int?)from.RenewalZoning;
            to.adoxio_RenewalFloorPlan = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalfloorplan?)(int?)from.RenewalFloorPlan;
            to.adoxio_RenewalSiteMap = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalsitemap?)(int?)from.RenewalSiteMap;
            to.adoxio_RenewalTiedHouseFederalInterest = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewaltiedhousefederalinterest?)(int?)from.TiedhouseFederalInterest;
            to.adoxio_renewalfedlic = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalfedlic?)(int?)from.RenewalFederalLicence;
            to.adoxio_renewalfedsec = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalfedsec?)(int?)from.RenewalFederalSecurity;
            to.adoxio_Description1 = from.Description1;
            to.adoxio_Description2 = from.Description2;
            to.adoxio_Description3 = from.Description3;
            to.adoxio_TempDateFrom = from.TempDateFrom?.UtcDateTime;
            to.adoxio_TempDateTo = from.TempDateTo?.UtcDateTime;
            to.adoxio_M01 = from.IsMonth01;
            to.adoxio_M02 = from.IsMonth02;
            to.adoxio_M03 = from.IsMonth03;
            to.adoxio_M04 = from.IsMonth04;
            to.adoxio_M05 = from.IsMonth05;
            to.adoxio_M06 = from.IsMonth06;
            to.adoxio_M07 = from.IsMonth07;
            to.adoxio_M08 = from.IsMonth08;
            to.adoxio_M09 = from.IsMonth09;
            to.adoxio_M10 = from.IsMonth10;
            to.adoxio_M11 = from.IsMonth11;
            to.adoxio_M12 = from.IsMonth12;
            to.adoxio_IsReadyWorkers = from.IsReadyWorkers;
            to.adoxio_IsReadyNameBranding = from.IsReadyNameBranding;
            to.adoxio_IsReadyDisplays = from.IsReadyDisplays;
            to.adoxio_IsReadyIntruderAlarm = from.IsReadyIntruderAlarm;
            to.adoxio_IsReadyFireAlarm = from.IsReadyFireAlarm;
            to.adoxio_IsReadyLockedCases = from.IsReadyLockedCases;
            to.adoxio_IsReadyLockedStorage = from.IsReadyLockedStorage;
            to.adoxio_IsReadyPerimeter = from.IsReadyPerimeter;
            to.adoxio_IsReadyRetailArea = from.IsReadyRetailArea;
            to.adoxio_IsReadyStorage = from.IsReadyStorage;
            to.adoxio_IsReadyEntranceExit = from.IsReadyExtranceExit;
            to.adoxio_IsReadySurveillanceNotice = from.IsReadySurveillanceNotice;
            to.adoxio_IsReadyProductNotVisibleOutside = from.IsReadyProductNotVisibleOutside;
            to.adoxio_isLocatedInGroceryStore = from.IsLocatedInGroceryStore;
            to.adoxio_EstablishmentOpeningDate = from.Establishmentopeningdate?.UtcDateTime;
            to.adoxio_IsReadyValidInterest = from.IsReadyValidInterest;
            to.adoxio_ApplicantType = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicanttypecodes?)(int?)from.ApplicantType == 0 ? null : (DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicanttypecodes?)(int?)from.ApplicantType;
            to.adoxio_LGZoning = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_lgzoning?)(int?)from.LgZoning;
            to.adoxio_LGDecisionComments = from.LGDecisionComments;
            to.adoxio_PreviousLicenceApplication = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_previouslicenceapplication?)(int?)from.PreviousApplication;
            to.adoxio_PreviousLicenceApplicationDetails = from.PreviousApplicationDetails;
            to.adoxio_RuralAgencyStoreAppointment = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_ruralagencystoreappointment?)(int?)from.RuralAgencyStoreAppointment;
            to.adoxio_LiquorIndustryConnections = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_liquorindustryconnections?)(int?)from.LiquorIndustryConnections;
            to.adoxio_LiquorIndustryConnectionsDetails = from.LiquorIndustryConnectionsDetails;
            to.adoxio_Otherbusinessesatthesamelocation = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno?)(int?)from.OtherBusinesses;
            to.adoxio_OtherBusinessSameLocationDetails = from.OtherBusinessesDetails;
            to.adoxio_IsApplicationComplete = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno?)(int?)from.IsApplicationComplete;
            to.adoxio_RenewalDUI = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewaldui?)(int?)from.RenewalDUI;
            to.adoxio_RenewalThirdParty = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_renewalthirdparty?)(int?)from.RenewalThirdParty;
            to.adoxio_IsOwnerBusiness = from.IsOwnerBusiness;
            to.adoxio_IsOwnerHasValidInterest = from.HasValidInterest;
            to.adoxio_IsOwnerWillHaveValidInterest = from.WillHaveValidInterest;
            to.adoxio_ZoningStatus = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_zoningstatus?)(int?)from.ZoningStatus;
            to.adoxio_IsHasPatio = from.IsHasPatio;
            to.adoxio_LGNoObjection = from.LgNoObjection;
            to.adoxio_LGNameofOfficial = from.LGNameOfOfficial;
            to.adoxio_LGTitlePosition = from.LGTitlePosition;
            to.adoxio_LGContactPhone = from.LGContactPhone;
            to.adoxio_LGContactEmail = from.LGContactEmail;
            to.adoxio_LGDecisionSubmissionDate = from.LGDecisionSubmissionDate?.UtcDateTime;
            to.adoxio_LGApprovalDecision = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_lgapprovaldecision?)(int?)from.LGApprovalDecision;
            to.adoxio_IsPackaging = from.IsPackaging;
            to.adoxio_MFGPipedInProduct = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_mfgpipedinproduct?)(int?)from.MfgPipedInProduct;
            to.adoxio_MFGBrewpubOnSite = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_mfgbrewpubonsite?)(int?)from.MfgBrewPubOnSite;
            to.adoxio_MFGAcresOfFruit = from.MfgAcresOfFruit;
            to.adoxio_MFGAcresOfGrapes = from.MfgAcresOfGrapes;
            to.adoxio_MFGAcresOfHoney = from.MfgAcresOfHoney;
            to.adoxio_MFGMeetsProductionMinimum = from.MfgMeetsProductionMinimum;
            to.adoxio_MFGStepBlending = from.MfgStepBlending;
            to.adoxio_MFGStepCrushing = from.MfgStepCrushing;
            to.adoxio_MFGStepFiltering = from.MfgStepFiltering;
            to.adoxio_MFGStepSecFermOrCarb = from.MfgStepSecFermOrCarb;
            to.adoxio_MFGUsesNeutralGrainSpirits = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_mfgusesneutralgrainspirits?)(int?)from.MfgUsesNeutralGrainSpirits;
            to.adoxio_PIDList = from.PidList;
            to.adoxio_IsPermittedInZoning = from.IsPermittedInZoning;
            to.adoxio_FirstNameOld = from.FirstNameOld;
            to.adoxio_FirstNameNew = from.FirstNameNew;
            to.adoxio_LastNameOld = from.LastNameOld;
            to.adoxio_LastNameNew = from.LastNameNew;
            to.adoxio_CSInternalTransferofShares = from.CsInternalTransferOfShares;
            to.adoxio_CSExternalTransferofShares = from.CsExternalTransferOfShares;
            to.adoxio_CSChangeofDirectorsorOfficers = from.CsChangeOfDirectorsOrOfficers;
            to.adoxio_CSNameChangeLicenseeCorporation = from.CsNameChangeLicenseeCorporation;
            to.adoxio_CSNameChangeLicenseePartnership = from.CsNameChangeLicenseePartnership;
            to.adoxio_CSNameChangeLicenseeSociety = from.CsNameChangeLicenseeSociety;
            to.adoxio_CSNameChangePerson = from.CsNameChangeLicenseePerson;
            to.adoxio_CSAdditionofReceiverorExecutor = from.CsAdditionalReceiverOrExecutor;
            to.adoxio_CSChangeToTiedHouse = from.CsTiedHouseDeclaration;
            to.adoxio_PatioCompDescription = from.PatioCompDescription;
            to.adoxio_PatioLocationDescription = from.PatioLocationDescription;
            to.adoxio_PatioAccessDescription = from.PatioAccessDescription;
            to.adoxio_PatioIsLiquorCarried = from.PatioIsLiquorCarried;
            to.adoxio_PatioLiquorCarriedDescription = from.PatioLiquorCarriedDescription;
            to.adoxio_PatioAccessControlDescription = from.PatioAccessControlDescription;
            to.adoxio_LocatedAboveDescription = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_locatedabovedescription?)(int?)from.LocatedAboveDescription;
            to.adoxio_PatioServiceBar = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_patioservicebar?)(int?)from.PatioServiceBar;
            to.adoxio_proposedestablishmentisALR = from.IsAlr;
            to.adoxio_HasCoolerAccess = from.HasCoolerAccess;
            to.adoxio_LocatedAboveOther = from.LocatedAboveOther;
            to.adoxio_IsonINLand = from.IsOnINLand == true
                ? DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_isoninland.Yes
                : (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_isoninland?)null;
            to.adoxio_IsRLRSLocatedinRuralCommunityAlone = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_isrlrslocatedinruralcommunityalone?)(int?)from.IsRlrsLocatedInRuralCommunityAlone;
            to.adoxio_IsRLRSLocatedAtTouristDestinationAlone = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_isrlrslocatedattouristdestinationalone?)(int?)from.IsRlrsLocatedAtTouristDestinationAlone;
            to.adoxio_DescribeRLRSResortCommunity = from.RlrsResortCommunityDescription;
            to.adoxio_HasYearRoundAllWeatherRoadAccess = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_hasyearroundallweatherroadaccess?)(int?)from.HasYearRoundAllWeatherRoadAccess;
            to.adoxio_DoesGeneralStoreOperateSeasonally = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_doesgeneralstoreoperateseasonally?)(int?)from.DoesGeneralStoreOperateSeasonally;
            to.adoxio_SurroundingResidentsOfRLRS = from.SurroundingResidentsOfRlrs;
            to.adoxio_IsRLRSAtLeast10KMFromAnotherStore = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_isrlrsatleast10kmfromanotherstore?)(int?)from.IsRlrsAtLeast10kmFromAnotherStore;
            to.adoxio_IsApplicantOwnerofStore = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_isapplicantownerofstore?)(int?)from.IsApplicantOwnerOfStore;
            to.adoxio_LegalandBeneficialOwnersofStore = from.LegalAndBeneficialOwnersOfStore;
            to.adoxio_IsApplicantFranchiseorAffiliated = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_isapplicantfranchiseoraffiliated?)(int?)from.IsApplicantFranchiseOrAffiliated;
            to.adoxio_FranchiseOrAffiliatedBusiness = from.FranchiseOrAffiliatedBusiness;
            to.adoxio_HasSufficientRangeofProducts = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_hassufficientrangeofproducts?)(int?)from.HasSufficientRangeOfProducts;
            to.adoxio_HasOtherProducts = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_hasotherproducts?)(int?)from.HasOtherProducts;
            to.adoxio_HasAdditionalServices = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_hasadditionalservices?)(int?)from.HasAdditionalServices;
            to.adoxio_StoreOpenDate = from.StoreOpenDate?.UtcDateTime;
            to.adoxio_ConfirmLiquorSalesIsNotPrimaryBusiness = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_confirmliquorsalesisnotprimarybusiness?)(int?)from.ConfirmLiquorSalesIsNotPrimaryBusiness;
            to.adoxio_manufacturerproductionamountforprevyear = from.ManufacturerProductionAmountForPrevYear;
            to.adoxio_manufacturerproductionamountunit = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_manufacturerproductionamountunit?)(int?)from.ManufacturerProductionAmountUnit;
            to.adoxio_PicnicConfirmsLGFNSupportsCapacity = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno?)(int?)from.PicnicConfirmLGFNCapacity;
            to.adoxio_PicnicConfirmsZoning = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno?)(int?)from.PicnicConfirmZoning;
            to.adoxio_PicnicReadandAcceptTermsandConditions = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno?)(int?)from.PicnicReadAndAccept;
            to.adoxio_FederalLicenceNumber = from.FederalLicenceNumber;
            to.adoxio_FederalLicenceName = from.FederalLicenceName;
            to.adoxio_FPAddressCity = from.FPAddressCity;
            to.adoxio_FPAddressPostalCode = from.FPAddressPostalCode;
            to.adoxio_FPAddressStreet = from.FPAddressStreet;
            to.adoxio_productslistanddescription = from.ProductsListAndDescription;
            to.adoxio_uploaddeclarations = from.UploadDeclarations;
            to.adoxio_MfrSupInfoReadUnderstand = from.MfrSupInfoReadUnderstand;
            to.adoxio_MfrSupInfoIntendProduce = from.MfrSupInfoIntendProduce;
            to.adoxio_MfrSupInfoOwnRent = from.MfrSupInfoOwnRent;
            to.adoxio_MfrSupInfoProductionEquipment = from.MfrSupInfoProductionEquipment;
            to.adoxio_volumeproduced = from.VolumeProduced;
            to.adoxio_volumedestroyed = from.VolumeDestroyed;
            to.adoxio_ldbordertotals = from.LdbOrderTotals == 0 ? (decimal?)null : from.LdbOrderTotals;
            to.adoxio_TiedHouseExemption = from.WillHaveTiedHouseExemption;
            to.adoxio_tempsuspensionorpatronparticipationstart = from.TempSuspensionOrPatronParticipationStart?.UtcDateTime;
            to.adoxio_tempsuspensionorpatronparticipationend = from.TempSuspensionOrPatronParticipationEnd?.UtcDateTime;
            to.adoxio_relocateOnSiteStore = from.RelocateOnSiteStore ? DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.Yes : DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.No;
            to.adoxio_confirmPermitsRetailSales = from.ConfirmPermitsRetailSales ? DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.Yes : DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.No;
            to.adoxio_relocatePicnicAreaEndorsement = from.RelocatePicnicAreaEndorsement ? DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.Yes : DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.No;
            to.adoxio_confirmrelocatePicnicAreaEndorsement = from.ConfirmrelocatePicnicAreaEndorsement ? DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.Yes : DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.No;
            to.adoxio_RelocateWineryLicence = from.RelocateWinaryLicence ? DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.Yes : DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.No;
            to.adoxio_confirmUnderstandingWineryLicence = from.ConfirmRelocateWinaryLicence ? DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.Yes : DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.No;
            to.adoxio_DormancyStartDate = from.DormancyStartDate?.UtcDateTime;
            to.adoxio_DormancyEndDate = from.DormancyEndDate?.UtcDateTime;
            to.adoxio_DormancyNotes = from.DormancyNotes;
            to.adoxio_DormancyReasons = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_dormancyreasons?)(int?)from.DormancyReasons;
            to.adoxio_EstablishmentStatus = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_establishmentstatus?)(int?)from.EstablishmentStatus;
            to.adoxio_DormancyIntentionforReopening = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_dormancyintentionforreopening?)(int?)from.DormancyIntentionForReopening;
            to.adoxio_IsPatioBoundingSufficientForControl = from.isBoundingSufficientForControl;
            to.adoxio_IsPatioBoundingSufficientToDefineArea = from.isBoundingSufficientToDefine;
            to.adoxio_IsAdequateCareandControlOverthePatio = from.isAdequateCare;
            to.adoxio_IsPatioInCompliance = from.isInCompliance;
            to.adoxio_StatusOfConstruction = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_statusofconstruction?)(int?)from.statusOfConstruction;
            to.adoxio_ValidInterestDormancyPeriod = from.validInterestDormancyPeriod ? DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.Yes : DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.No;
            to.adoxio_AffirmInformationProividedTrueAndComplete = from.affirmInformationProividedTrueAndComplete ? DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.Yes : DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.No;
            to.adoxio_ValidInterestEstablishmentLocation = from.validInterestEstablishmentLocation ? DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.Yes : DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno.No;
            to.adoxio_EstablishmentReopeningDate = from.EstablishmentReopeningDate?.UtcDateTime;
            to.adoxio_ChecklistDrivingRecordComplete = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_adoxio_checklistdrivingrecordcomplete?)(int?)from.temporaryRelocationCriteria;
            to.statuscode = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_statuscode?)(int?)from.ApplicationStatus == 0 ? null : (DV::Gov.Lclb.Cllb.Interfaces.adoxio_application_statuscode?)(int?)from.ApplicationStatus;
        }


        // -------------------------------------------------------------------------
        // Dataverse SDK extensions
        // -------------------------------------------------------------------------

        public static ApplicationSummary ToSummaryViewModel(this adoxio_application app, adoxio_applicationtype? appType = null)
        {
            var appTypeName = appType?.adoxio_name;
            var summary = new ApplicationSummary
            {
                Id = app.adoxio_applicationId?.ToString(),
                Name = app.adoxio_name,
                JobNumber = app.adoxio_JobNumber,
                EstablishmentName = app.adoxio_EstablishmentPropsedName,
                LicenceId = app.adoxio_AssignedLicence?.Id.ToString(),
                IsPaid = app.adoxio_PaymentRecieved == true,
                EstablishmentAddressStreet = app.adoxio_EstablishmentAddressStreet,
                EstablishmentAddressCity = app.adoxio_EstablishmentAddressCity,
                EstablishmentAddressPostalCode = app.adoxio_EstablishmentAddressPostalCode,
                EstablishmentAddress = $"{app.adoxio_EstablishmentAddressStreet}, {app.adoxio_EstablishmentAddressCity} {app.adoxio_EstablishmentAddressPostalCode}",
                EstablishmentPhone = app.adoxio_EstablishmentPhone,
                EstablishmentEmail = app.adoxio_EstablishmentEmail,
                EstablishmentParcelId = app.adoxio_EstablishmentParcelID,
                IndigenousNationId = app.adoxio_localgovindigenousnationid?.Id.ToString(),
                PoliceJurisdictionId = app.adoxio_PoliceJurisdictionId?.Id.ToString(),
                IsApplicationComplete = (GeneralYesNo?)(int?)app.adoxio_IsApplicationComplete,
                IsStructuralChange = appType?.adoxio_IsStructuralChange == true,
                DateApplicationSubmitted = app.adoxio_DateApplicationSubmitted,
                DateApplicantSentToLG = app.adoxio_DateApplicantSenttoLG,
            };

            if (app.statuscode != null)
                summary.ApplicationStatus = StatusUtility.GetTranslatedApplicationStatusV2(app, appTypeName);

            if (appType != null)
            {
                summary.ApplicationTypeName = appType.adoxio_name;
                summary.IsForLicence = appType.adoxio_LicenceType != null;
                summary.Portallabel = appType.adoxio_PortalLabel;
                summary.ApplicationTypeCategory = (ApplicationTypeCategory?)(int?)appType.adoxio_Category;
            }

            summary.LGHasApproved =
                app.adoxio_LGApprovalDecision == adoxio_application_adoxio_lgapprovaldecision.Resolved
                || app.adoxio_LGApprovalDecision == adoxio_application_adoxio_lgapprovaldecision.OptOut
                || app.adoxio_LGApprovalDecision == adoxio_application_adoxio_lgapprovaldecision.AcceptedPendingResolution
                || app.adoxio_LGZoning == adoxio_application_adoxio_lgzoning.Allows;

            summary.IsIndigenousNation = app.adoxio_ApplicantType == adoxio_applicanttypecodes.IndigenousNation;

            return summary;
        }

        public static async Task<ViewModels.Application> ToViewModelAsync(
            this adoxio_application app,
            IDataverseClient dataverse,
            IMemoryCache cache,
            ILogger logger,
            adoxio_applicationextension? extension = null)
        {
            var appId = app.adoxio_applicationId?.ToString();

            // parallel: applying person, applicant, licence type, licence sub-category
            var applyingPersonTask = app.adoxio_ApplyingPerson != null
                ? dataverse.GetContactByIdAsync(app.adoxio_ApplyingPerson.Id.ToString())
                : Task.FromResult((DV::Gov.Lclb.Cllb.Interfaces.Contact?)null);
            var applicantTask = app.adoxio_Applicant != null
                ? dataverse.GetAccountByIdAsync(app.adoxio_Applicant.Id.ToString())
                : Task.FromResult((DV::Gov.Lclb.Cllb.Interfaces.Account?)null);
            var licenceTypeTask = app.adoxio_LicenceType != null
                ? dataverse.GetLicenceTypeByIdAsync(app.adoxio_LicenceType.Id.ToString())
                : Task.FromResult((adoxio_licencetype?)null);
            var licenceSubCategoryTask = app.adoxio_LicenceSubCategoryId != null
                ? dataverse.GetLicenceSubCategoryByIdAsync(app.adoxio_LicenceSubCategoryId.Id.ToString())
                : Task.FromResult((adoxio_licencesubcategory?)null);
            var serviceAreasTask = appId != null
                ? dataverse.GetServiceAreasByApplicationIdAsync(appId)
                : Task.FromResult((IList<adoxio_servicearea>)new List<adoxio_servicearea>());
            var hoursTask = appId != null
                ? dataverse.GetHoursOfServiceByApplicationIdAsync(appId)
                : Task.FromResult((adoxio_hoursofservice?)null);

            await Task.WhenAll(applyingPersonTask, applicantTask, licenceTypeTask, licenceSubCategoryTask, serviceAreasTask, hoursTask);

            var applyingPerson = await applyingPersonTask;
            var applicant = await applicantTask;
            var licenceType = await licenceTypeTask;
            var licenceSubCategory = await licenceSubCategoryTask;
            var serviceAreas = await serviceAreasTask;
            var hours = await hoursTask;

            var vm = new ViewModels.Application
            {
                Id = appId,
                Name = app.adoxio_name,
                JobNumber = app.adoxio_JobNumber,
                EstablishmentName = app.adoxio_EstablishmentPropsedName,
                EstablishmentAddressStreet = app.adoxio_EstablishmentAddressStreet,
                EstablishmentAddressCity = app.adoxio_EstablishmentAddressCity,
                EstablishmentAddressPostalCode = app.adoxio_EstablishmentAddressPostalCode,
                EstablishmentAddress = $"{app.adoxio_EstablishmentAddressStreet}, {app.adoxio_EstablishmentAddressCity} {app.adoxio_EstablishmentAddressPostalCode}",
                EstablishmentPhone = app.adoxio_EstablishmentPhone,
                EstablishmentEmail = app.adoxio_EstablishmentEmail,
                EstablishmentParcelId = app.adoxio_EstablishmentParcelID,
                FederalProducerNames = app.adoxio_FederalProducerNames,
                IsApplicationComplete = (GeneralYesNo?)(int?)app.adoxio_IsApplicationComplete,

                RenewalCriminalOffenceCheck = (ValueNotChanged?)(int?)app.adoxio_RenewalCriminalOffenceCheck,
                RenewalUnreportedSaleOfBusiness = (ValueNotChanged?)(int?)app.adoxio_RenewalUnreportedSaleofBusiness,
                RenewalBusinessType = (ValueNotChanged?)(int?)app.adoxio_RenewalBusinessType,
                RenewalTiedhouse = (ValueNotChanged?)(int?)app.adoxio_RenewalTiedHouse,
                RenewalOrgLeadership = (ValueNotChanged?)(int?)app.adoxio_RenewalOrgLeadership,
                Renewalkeypersonnel = (ValueNotChanged?)(int?)app.adoxio_RenewalKeyPersonnel,
                RenewalShareholders = (ValueNotChanged?)(int?)app.adoxio_RenewalShareholders,
                RenewalOutstandingFines = (ValueNotChanged?)(int?)app.adoxio_RenewalOutstandingFines,
                RenewalBranding = (ValueNotChanged?)(int?)app.adoxio_RenewalBranding,
                RenewalSignage = (ValueNotChanged?)(int?)app.adoxio_RenewalSignage,
                RenewalEstablishmentAddress = (ValueNotChanged?)(int?)app.adoxio_RenewalEstablishmentAddress,
                RenewalValidInterest = (ValueNotChanged?)(int?)app.adoxio_RenewalValidInterest,
                RenewalZoning = (ValueNotChanged?)(int?)app.adoxio_RenewalZoning,
                RenewalFloorPlan = (ValueNotChanged?)(int?)app.adoxio_RenewalFloorPlan,
                RenewalSiteMap = (ValueNotChanged?)(int?)app.adoxio_RenewalSiteMap,
                TiedhouseFederalInterest = (ValueNotChanged?)(int?)app.adoxio_RenewalTiedHouseFederalInterest,
                RenewalDUI = (ValueNotChanged?)(int?)app.adoxio_RenewalDUI,
                RenewalThirdParty = (ValueNotChanged?)(int?)app.adoxio_RenewalThirdParty,
                RenewalFederalLicence = (ValueNotChanged?)(int?)app.adoxio_renewalfedlic,
                RenewalFederalSecurity = (ValueNotChanged?)(int?)app.adoxio_renewalfedsec,

                AuthorizedToSubmit = app.adoxio_AuthorizedtoSubmit,
                SignatureAgreement = app.adoxio_SignatureAgreement,

                LicenceFeeInvoicePaid = app.adoxio_LicenceFeeInvoicePaid == true,
                IsPaid = app.adoxio_PaymentRecieved == true,

                IndigenousNationId = app.adoxio_localgovindigenousnationid?.Id.ToString(),
                PoliceJurisdictionId = app.adoxio_PoliceJurisdictionId?.Id.ToString(),

                Pin = app.adoxio_PIN,
                AdditionalPropertyInformation = app.adoxio_AdditionalPropertyInformation,
                InvoiceId = app.adoxio_Invoice?.Id.ToString(),
                SecondaryInvoiceId = app.adoxio_SecondaryApplicationInvoice?.Id.ToString(),

                PaymentReceivedDate = app.adoxio_PaymentReceivedDate,
                Description1 = app.adoxio_Description1,
                Description2 = app.adoxio_Description2,
                Description3 = app.adoxio_Description3,
                TempDateFrom = app.adoxio_TempDateFrom,
                TempDateTo = app.adoxio_TempDateTo,

                IsMonth01 = app.adoxio_M01,
                IsMonth02 = app.adoxio_M02,
                IsMonth03 = app.adoxio_M03,
                IsMonth04 = app.adoxio_M04,
                IsMonth05 = app.adoxio_M05,
                IsMonth06 = app.adoxio_M06,
                IsMonth07 = app.adoxio_M07,
                IsMonth08 = app.adoxio_M08,
                IsMonth09 = app.adoxio_M09,
                IsMonth10 = app.adoxio_M10,
                IsMonth11 = app.adoxio_M11,
                IsMonth12 = app.adoxio_M12,

                ContactPersonFirstName = app.adoxio_ContactPersonFirstName,
                ContactPersonLastName = app.adoxio_ContactPersonLastName,
                ContactPersonRole = app.adoxio_Role,
                ContactPersonEmail = app.adoxio_Email,
                ContactPersonPhone = app.adoxio_ContactPersonPhone,

                CreatedOn = app.CreatedOn,
                ModifiedOn = app.ModifiedOn,

                IsReadyWorkers = app.adoxio_IsReadyWorkers,
                IsReadyNameBranding = app.adoxio_IsReadyNameBranding,
                IsReadyDisplays = app.adoxio_IsReadyDisplays,
                IsReadyIntruderAlarm = app.adoxio_IsReadyIntruderAlarm,
                IsReadyFireAlarm = app.adoxio_IsReadyFireAlarm,
                IsReadyLockedCases = app.adoxio_IsReadyLockedCases,
                IsReadyLockedStorage = app.adoxio_IsReadyLockedStorage,
                IsReadyPerimeter = app.adoxio_IsReadyPerimeter,
                IsReadyRetailArea = app.adoxio_IsReadyRetailArea,
                IsReadyStorage = app.adoxio_IsReadyStorage,
                IsReadyExtranceExit = app.adoxio_IsReadyEntranceExit,
                IsReadySurveillanceNotice = app.adoxio_IsReadySurveillanceNotice,
                IsReadyProductNotVisibleOutside = app.adoxio_IsReadyProductNotVisibleOutside,
                IsLocatedInGroceryStore = app.adoxio_isLocatedInGroceryStore,
                Establishmentopeningdate = app.adoxio_EstablishmentOpeningDate,
                IsReadyValidInterest = app.adoxio_IsReadyValidInterest,

                IsHasPatio = app.adoxio_IsHasPatio,

                LgNoObjection = app.adoxio_LGNoObjection,
                LGNameOfOfficial = app.adoxio_LGNameofOfficial,
                LGTitlePosition = app.adoxio_LGTitlePosition,
                LGContactPhone = app.adoxio_LGContactPhone,
                LGContactEmail = app.adoxio_LGContactEmail,
                LGDecisionSubmissionDate = app.adoxio_LGDecisionSubmissionDate,
                LGApprovalDecision = (LGDecision?)(int?)app.adoxio_LGApprovalDecision,
                LgZoning = (Zoning?)(int?)app.adoxio_LGZoning,
                LGDecisionComments = app.adoxio_LGDecisionComments,
                DateApplicantSentToLG = app.adoxio_datesentlgin,

                PreviousApplicationDetails = app.adoxio_PreviousLicenceApplicationDetails,
                LiquorIndustryConnectionsDetails = app.adoxio_LiquorIndustryConnectionsDetails,
                OtherBusinessesDetails = app.adoxio_OtherBusinessSameLocationDetails,
                ServiceAreas = new List<CapacityArea>(),
                OutsideAreas = new List<CapacityArea>(),
                CapacityArea = new List<CapacityArea>(),

                IsPackaging = app.adoxio_IsPackaging,

                MfgAcresOfFruit = app.adoxio_MFGAcresOfFruit,
                MfgAcresOfGrapes = app.adoxio_MFGAcresOfGrapes,
                MfgAcresOfHoney = app.adoxio_MFGAcresOfHoney,
                MfgMeetsProductionMinimum = app.adoxio_MFGMeetsProductionMinimum,
                MfgStepBlending = app.adoxio_MFGStepBlending,
                MfgStepCrushing = app.adoxio_MFGStepCrushing,
                MfgStepFiltering = app.adoxio_MFGStepFiltering,
                MfgStepSecFermOrCarb = app.adoxio_MFGStepSecFermOrCarb,
                IsOwnerBusiness = app.adoxio_IsOwnerBusiness,
                HasValidInterest = app.adoxio_IsOwnerHasValidInterest,
                WillHaveValidInterest = app.adoxio_IsOwnerWillHaveValidInterest,
                ZoningStatus = (int?)app.adoxio_ZoningStatus,

                PidList = app.adoxio_PIDList,
                IsPermittedInZoning = app.adoxio_IsPermittedInZoning,

                PatioCompDescription = app.adoxio_PatioCompDescription,
                PatioLocationDescription = app.adoxio_PatioLocationDescription,
                PatioAccessDescription = app.adoxio_PatioAccessDescription,
                PatioIsLiquorCarried = app.adoxio_PatioIsLiquorCarried,
                PatioLiquorCarriedDescription = app.adoxio_PatioLiquorCarriedDescription,
                PatioAccessControlDescription = app.adoxio_PatioAccessControlDescription,
                IsAlr = app.adoxio_proposedestablishmentisALR == true,
                HasCoolerAccess = app.adoxio_HasCoolerAccess == true,

                FirstNameOld = app.adoxio_FirstNameOld,
                FirstNameNew = app.adoxio_FirstNameNew,
                LastNameOld = app.adoxio_LastNameOld,
                LastNameNew = app.adoxio_LastNameNew,
                CsInternalTransferOfShares = app.adoxio_CSInternalTransferofShares,
                CsExternalTransferOfShares = app.adoxio_CSExternalTransferofShares,
                CsChangeOfDirectorsOrOfficers = app.adoxio_CSChangeofDirectorsorOfficers,
                CsNameChangeLicenseeCorporation = app.adoxio_CSNameChangeLicenseeCorporation,
                CsNameChangeLicenseePartnership = app.adoxio_CSNameChangeLicenseePartnership,
                CsNameChangeLicenseeSociety = app.adoxio_CSNameChangeLicenseeSociety,
                CsNameChangeLicenseePerson = app.adoxio_CSNameChangePerson,
                CsAdditionalReceiverOrExecutor = app.adoxio_CSAdditionofReceiverorExecutor,
                CsTiedHouseDeclaration = app.adoxio_CSChangeToTiedHouse,
                PrimaryInvoicePaid = app.adoxio_PrimaryApplicationInvoicePaid == adoxio_generalyesno.Yes,
                SecondaryInvoicePaid = app.adoxio_SecondaryApplicationInvoicePaid == adoxio_generalyesno.Yes,
                IsOnINLand = ConvertYesNoLookupToBool((int?)app.adoxio_IsonINLand),

                LocatedAboveOther = app.adoxio_LocatedAboveOther,

                IsRlrsLocatedInRuralCommunityAlone = (int?)app.adoxio_IsRLRSLocatedinRuralCommunityAlone,
                IsRlrsLocatedAtTouristDestinationAlone = (int?)app.adoxio_IsRLRSLocatedAtTouristDestinationAlone,
                RlrsResortCommunityDescription = app.adoxio_DescribeRLRSResortCommunity,
                HasYearRoundAllWeatherRoadAccess = (int?)app.adoxio_HasYearRoundAllWeatherRoadAccess,
                DoesGeneralStoreOperateSeasonally = (int?)app.adoxio_DoesGeneralStoreOperateSeasonally,
                SurroundingResidentsOfRlrs = app.adoxio_SurroundingResidentsOfRLRS,
                IsRlrsAtLeast10kmFromAnotherStore = (int?)app.adoxio_IsRLRSAtLeast10KMFromAnotherStore,
                IsApplicantOwnerOfStore = (int?)app.adoxio_IsApplicantOwnerofStore,
                LegalAndBeneficialOwnersOfStore = app.adoxio_LegalandBeneficialOwnersofStore,
                IsApplicantFranchiseOrAffiliated = (int?)app.adoxio_IsApplicantFranchiseorAffiliated,
                FranchiseOrAffiliatedBusiness = app.adoxio_FranchiseOrAffiliatedBusiness,

                HasSufficientRangeOfProducts = (int?)app.adoxio_HasSufficientRangeofProducts,
                HasOtherProducts = (int?)app.adoxio_HasOtherProducts,
                HasAdditionalServices = (int?)app.adoxio_HasAdditionalServices,
                StoreOpenDate = app.adoxio_StoreOpenDate,
                ConfirmLiquorSalesIsNotPrimaryBusiness = (int?)app.adoxio_ConfirmLiquorSalesIsNotPrimaryBusiness,
                ManufacturerProductionAmountForPrevYear = app.adoxio_manufacturerproductionamountforprevyear,
                ManufacturerProductionAmountUnit = (int?)app.adoxio_manufacturerproductionamountunit,
                PicnicConfirmLGFNCapacity = (int?)app.adoxio_PicnicConfirmsLGFNSupportsCapacity,
                PicnicConfirmZoning = (int?)app.adoxio_PicnicConfirmsZoning,
                PicnicReadAndAccept = (int?)app.adoxio_PicnicReadandAcceptTermsandConditions,
                FederalLicenceNumber = app.adoxio_FederalLicenceNumber,
                FederalLicenceName = app.adoxio_FederalLicenceName,
                FPAddressCity = app.adoxio_FPAddressCity,
                FPAddressPostalCode = app.adoxio_FPAddressPostalCode,
                FPAddressStreet = app.adoxio_FPAddressStreet,

                UploadDeclarations = app.adoxio_uploaddeclarations,
                ProductsListAndDescription = app.adoxio_productslistanddescription,
                MfrSupInfoReadUnderstand = app.adoxio_MfrSupInfoReadUnderstand,
                MfrSupInfoIntendProduce = app.adoxio_MfrSupInfoIntendProduce,
                MfrSupInfoOwnRent = app.adoxio_MfrSupInfoOwnRent,
                MfrSupInfoProductionEquipment = app.adoxio_MfrSupInfoProductionEquipment,

                VolumeProduced = app.adoxio_volumeproduced ?? 0,
                VolumeDestroyed = app.adoxio_volumedestroyed ?? 0,
                LdbOrderTotals = app.adoxio_ldbordertotals ?? 0,

                WillHaveTiedHouseExemption = app.adoxio_TiedHouseExemption,
                TempSuspensionOrPatronParticipationStart = app.adoxio_tempsuspensionorpatronparticipationstart,
                TempSuspensionOrPatronParticipationEnd = app.adoxio_tempsuspensionorpatronparticipationend,

                ConfirmrelocatePicnicAreaEndorsement = app.adoxio_confirmrelocatePicnicAreaEndorsement == adoxio_generalyesno.Yes,
                RelocatePicnicAreaEndorsement = app.adoxio_relocatePicnicAreaEndorsement == adoxio_generalyesno.Yes,
                ConfirmPermitsRetailSales = app.adoxio_confirmPermitsRetailSales == adoxio_generalyesno.Yes,
                RelocateOnSiteStore = app.adoxio_relocateOnSiteStore == adoxio_generalyesno.Yes,
                RelocateWinaryLicence = app.adoxio_RelocateWineryLicence == adoxio_generalyesno.Yes,
                ConfirmRelocateWinaryLicence = app.adoxio_confirmUnderstandingWineryLicence == adoxio_generalyesno.Yes,
                DormancyStartDate = app.adoxio_DormancyStartDate,
                DormancyEndDate = app.adoxio_DormancyEndDate,
                DormancyNotes = app.adoxio_DormancyNotes,
                DormancyReasons = (int?)app.adoxio_DormancyReasons,
                EstablishmentStatus = (int?)app.adoxio_EstablishmentStatus,
                DormancyIntentionForReopening = (int?)app.adoxio_DormancyIntentionforReopening,

                isBoundingSufficientForControl = app.adoxio_IsPatioBoundingSufficientForControl,
                isBoundingSufficientToDefine = app.adoxio_IsPatioBoundingSufficientToDefineArea,
                isAdequateCare = app.adoxio_IsAdequateCareandControlOverthePatio,
                isInCompliance = app.adoxio_IsPatioInCompliance,

                statusOfConstruction = (int?)app.adoxio_StatusOfConstruction,

                validInterestDormancyPeriod = app.adoxio_ValidInterestDormancyPeriod == adoxio_generalyesno.Yes,
                affirmInformationProividedTrueAndComplete = app.adoxio_AffirmInformationProividedTrueAndComplete == adoxio_generalyesno.Yes,
                validInterestEstablishmentLocation = app.adoxio_ValidInterestEstablishmentLocation == adoxio_generalyesno.Yes,
                EstablishmentReopeningDate = app.adoxio_EstablishmentReopeningDate,

                temporaryRelocationCriteria = (int?)app.adoxio_ChecklistDrivingRecordComplete,

                ApplicationExtension = extension?.ToViewModel()
            };

            // mfg fields
            if (app.adoxio_MFGPipedInProduct != null)
                vm.MfgPipedInProduct = (YesNoNotApplicable?)(int?)app.adoxio_MFGPipedInProduct;
            if (app.adoxio_MFGBrewpubOnSite != null)
                vm.MfgBrewPubOnSite = (YesNoNotApplicable?)(int?)app.adoxio_MFGBrewpubOnSite;
            if (app.adoxio_MFGUsesNeutralGrainSpirits != null)
                vm.MfgUsesNeutralGrainSpirits = (YesNoNotApplicable)(int)app.adoxio_MFGUsesNeutralGrainSpirits;

            if (app.adoxio_LocatedAboveDescription != null)
                vm.LocatedAboveDescription = (int?)app.adoxio_LocatedAboveDescription;
            if (app.adoxio_PatioServiceBar != null)
                vm.PatioServiceBar = (int?)app.adoxio_PatioServiceBar;
            if (app.adoxio_PreviousLicenceApplication != null)
                vm.PreviousApplication = (int?)app.adoxio_PreviousLicenceApplication;
            if (app.adoxio_RuralAgencyStoreAppointment != null)
                vm.RuralAgencyStoreAppointment = (int?)app.adoxio_RuralAgencyStoreAppointment;
            if (app.adoxio_LiquorIndustryConnections != null)
                vm.LiquorIndustryConnections = (int?)app.adoxio_LiquorIndustryConnections;
            if (app.adoxio_Otherbusinessesatthesamelocation != null)
                vm.OtherBusinesses = (int?)app.adoxio_Otherbusinessesatthesamelocation;

            if (app.statuscode != null)
                vm.ApplicationStatus = (AdoxioApplicationStatusCodes)(int)app.statuscode;
            if (app.adoxio_ApplicantType != null)
                vm.ApplicantType = (AdoxioApplicantTypeCodes)(int)app.adoxio_ApplicantType;

            vm.InvoiceTrigger = (GeneralYesNo?)(int?)app.adoxio_InvoiceTrigger;
            vm.IsSubmitted = app.adoxio_InvoiceTrigger == adoxio_generalyesno.Yes;
            vm.PrevPaymentFailed = app.adoxio_Invoice != null && !vm.IsSubmitted;

            // applying person
            if (applyingPerson != null)
                vm.ApplyingPerson = applyingPerson.FullName;

            // applicant account
            if (applicant != null)
                vm.Applicant = applicant.ToViewModel();

            // licence type
            if (licenceType != null)
                vm.LicenseType = licenceType.adoxio_name;

            // licence sub-category
            if (licenceSubCategory != null)
                vm.LicenceSubCategory = licenceSubCategory.adoxio_name;

            // assigned licence (populated by GetApplicationByIdWithChildrenAsync via N:1 nav property)
            var assignedLicence = app.adoxio_adoxio_licences_adoxio_application_AssignedLicence;

            // Round 1: launch all independent Dataverse lookups in parallel
            var invoiceTask = app.adoxio_LicenceFeeInvoice != null
                                        ? dataverse.GetInvoiceByIdAsync(app.adoxio_LicenceFeeInvoice.Id.ToString())
                                        : null;
            var appTypeTask = app.adoxio_ApplicationTypeId != null
                                        ? dataverse.GetApplicationTypeByIdAsync(app.adoxio_ApplicationTypeId.Id.ToString())
                                        : null;
            var assignedLicenceTask = assignedLicence != null
                                        ? assignedLicence.ToViewModelAsync(dataverse)
                                        : null;
            var lginTask = app.adoxio_localgovindigenousnationid != null
                                        ? dataverse.GetLginByIdAsync(app.adoxio_localgovindigenousnationid.Id.ToString())
                                        : null;

            var round1 = new List<Task>();
            if (invoiceTask != null) round1.Add(invoiceTask);
            if (appTypeTask != null) round1.Add(appTypeTask);
            if (assignedLicenceTask != null) round1.Add(assignedLicenceTask);
            if (lginTask != null) round1.Add(lginTask);
            if (round1.Count > 0) await Task.WhenAll(round1);

            var feeInvoice = invoiceTask != null ? await invoiceTask : null;
            var appType = appTypeTask != null ? await appTypeTask : null;
            vm.AssignedLicence = assignedLicenceTask != null ? await assignedLicenceTask : null;
            var lgin = lginTask != null ? await lginTask : null;

            // Map round-1 results before launching round-2 (needed to determine sub-task conditions)
            if (feeInvoice != null)
                vm.LicenceFeeInvoice = feeInvoice.ToViewModel();

            // application type
            if (appType != null)
                vm.ApplicationType = appType.ToViewModel();

            // LGIN
            if (lgin != null)
                vm.IndigenousNation = lgin.ToViewModel();

            // Round 2: sub-lookups that depend on round-1 results — also in parallel
            var formTask = vm.ApplicationType != null && !string.IsNullOrEmpty(vm.ApplicationType.FormReference)
                                    ? dataverse.GetSystemformViewModelAsync(cache, logger, vm.ApplicationType.FormReference)
                                    : null;
            // only fetch content types if not already loaded via nav property
            var contentsTask = vm.ApplicationType != null && vm.ApplicationType.ContentTypes == null
                                    ? dataverse.GetApplicationTypeContentsByTypeIdAsync(app.adoxio_ApplicationTypeId.Id.ToString())
                                    : null;
            var lginAccountTask = lgin != null
                                    ? dataverse.GetAccountByLginLinkIdAsync(lgin.adoxio_localgovindigenousnationId?.ToString())
                                    : null;

            var round2 = new List<Task>();
            if (formTask != null) round2.Add(formTask);
            if (contentsTask != null) round2.Add(contentsTask);
            if (lginAccountTask != null) round2.Add(lginAccountTask);
            if (round2.Count > 0) await Task.WhenAll(round2);

            // Assign round-2 results
            if (formTask != null)
                vm.ApplicationType.DynamicsForm = await formTask;

            if (contentsTask != null)
            {
                var contents = await contentsTask;
                if (contents.Count > 0)
                    vm.ApplicationType.ContentTypes = contents.Select(c => c.ToViewModel()).ToList();
            }

            if (lginAccountTask != null)
            {
                var linkedAccount = await lginAccountTask;
                if (linkedAccount?.WebSiteURL != null)
                    vm.IndigenousNation.WebsiteUrl = linkedAccount.WebSiteURL;
            }

            // police jurisdiction
            if (app.adoxio_PoliceJurisdictionId != null)
            {
                var pj = app.adoxio_PoliceJurisdictionId;
                vm.PoliceJurisdiction = new ViewModels.PoliceJurisdiction { id = pj.Id.ToString(), name = pj.Name };
            }

            // service areas
            foreach (var area in serviceAreas)
            {
                var areaVm = area.ToViewModel();
                if ((int?)area.adoxio_areacategory == (int)AdoxioAreaCategories.Service)
                    vm.ServiceAreas.Add(areaVm);
                else if ((int?)area.adoxio_areacategory == (int)AdoxioAreaCategories.OutdoorArea)
                    vm.OutsideAreas.Add(areaVm);
                else if ((int?)area.adoxio_areacategory == (int)AdoxioAreaCategories.Capacity)
                    vm.CapacityArea.Add(areaVm);
            }

            // hours of service
            if (hours != null)
            {
                vm.ServiceHoursSundayOpen = (ServiceHours?)(int?)hours.adoxio_SundayOpen;
                vm.ServiceHoursSundayClose = (ServiceHours?)(int?)hours.adoxio_SundayClose;
                vm.ServiceHoursMondayOpen = (ServiceHours?)(int?)hours.adoxio_MondayOpen;
                vm.ServiceHoursMondayClose = (ServiceHours?)(int?)hours.adoxio_MondayClose;
                vm.ServiceHoursTuesdayOpen = (ServiceHours?)(int?)hours.adoxio_TuesdayOpen;
                vm.ServiceHoursTuesdayClose = (ServiceHours?)(int?)hours.adoxio_TuesdayClose;
                vm.ServiceHoursWednesdayOpen = (ServiceHours?)(int?)hours.adoxio_WednesdayOpen;
                vm.ServiceHoursWednesdayClose = (ServiceHours?)(int?)hours.adoxio_WednesdayClose;
                vm.ServiceHoursThursdayOpen = (ServiceHours?)(int?)hours.adoxio_ThursdayOpen;
                vm.ServiceHoursThursdayClose = (ServiceHours?)(int?)hours.adoxio_ThursdayClose;
                vm.ServiceHoursFridayOpen = (ServiceHours?)(int?)hours.adoxio_FridayOpen;
                vm.ServiceHoursFridayClose = (ServiceHours?)(int?)hours.adoxio_FridayClose;
                vm.ServiceHoursSaturdayOpen = (ServiceHours?)(int?)hours.adoxio_SaturdayOpen;
                vm.ServiceHoursSaturdayClose = (ServiceHours?)(int?)hours.adoxio_SaturdayClose;
                vm.RequestOutsideServiceHours = hours.adoxio_RequestOutsideServiceHours;
            }

            return vm;
        }

        public static async Task<CovidApplication> ToCovidViewModelAsync(
            this adoxio_application app,
            IDataverseClient dataverse,
            IMemoryCache cache,
            ILogger logger)
        {
            var vm = new CovidApplication
            {
                Id = app.adoxio_applicationId?.ToString(),
                Name = app.adoxio_name,
                JobNumber = app.adoxio_JobNumber,
                EstablishmentName = app.adoxio_EstablishmentPropsedName,
                EstablishmentAddressStreet = app.adoxio_EstablishmentAddressStreet,
                EstablishmentAddressCity = app.adoxio_EstablishmentAddressCity,
                EstablishmentAddressPostalCode = app.adoxio_EstablishmentAddressPostalCode,
                EstablishmentAddress = $"{app.adoxio_EstablishmentAddressStreet}, {app.adoxio_EstablishmentAddressCity} {app.adoxio_EstablishmentAddressPostalCode}",
                EstablishmentPhone = app.adoxio_EstablishmentPhone,
                EstablishmentEmail = app.adoxio_EstablishmentEmail,
                IsApplicationComplete = (GeneralYesNo?)(int?)app.adoxio_IsApplicationComplete,
                AddressStreet = app.adoxio_AddressStreet,
                AddressCity = app.adoxio_AddressCity,
                AddressPostalCode = app.adoxio_AddressPostalCode,
                NameOfApplicant = app.adoxio_NameofApplicant,
                AuthorizedToSubmit = app.adoxio_AuthorizedtoSubmit,
                EstablishmentParcelId = app.adoxio_EstablishmentParcelID,
                AdditionalPropertyInformation = app.adoxio_AdditionalPropertyInformation,
                InvoiceId = app.adoxio_Invoice?.Id.ToString(),
                Description1 = app.adoxio_Description1,
                ContactPersonFirstName = app.adoxio_ContactPersonFirstName,
                ContactPersonLastName = app.adoxio_ContactPersonLastName,
                ContactPersonRole = app.adoxio_Role,
                ContactPersonEmail = app.adoxio_Email,
                ContactPersonPhone = app.adoxio_ContactPersonPhone,
                CreatedOn = app.CreatedOn,
                ModifiedOn = app.ModifiedOn,
                ProposedEstablishmentIsAlr = app.adoxio_proposedestablishmentisALR
            };

            if (app.adoxio_ApplicantType != null)
                vm.ApplicantType = (AdoxioApplicantTypeCodes)(int)app.adoxio_ApplicantType;

            if (app.adoxio_ApplyingPerson != null)
            {
                var contact = await dataverse.GetContactByIdAsync(app.adoxio_ApplyingPerson.Id.ToString());
                vm.ApplyingPerson = contact?.FullName;
            }

            if (app.adoxio_LicenceType != null)
            {
                var lt = await dataverse.GetLicenceTypeByIdAsync(app.adoxio_LicenceType.Id.ToString());
                vm.LicenceType = lt?.adoxio_name;
            }

            if (app.adoxio_ApplicationTypeId != null)
            {
                var appType = await dataverse.GetApplicationTypeByIdAsync(app.adoxio_ApplicationTypeId.Id.ToString());
                if (appType != null)
                {
                    vm.ApplicationType = appType.ToViewModel();
                    if (!string.IsNullOrEmpty(vm.ApplicationType.FormReference))
                        vm.ApplicationType.DynamicsForm = await dataverse.GetSystemformViewModelAsync(cache, logger, vm.ApplicationType.FormReference);
                }
            }

            return vm;
        }
    }
}
