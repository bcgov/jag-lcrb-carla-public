using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Mapping
{
    public class ApplicationMapping : FieldMap
    {
        
        public ApplicationMapping()
        {
            fieldMap = new Dictionary<string, FieldMapping>
            {
                // catering fields
                {"adoxio_previouslicenceapplication", new FieldMapping("previousApplication", true)},
                {"adoxio_previouslicenceapplicationdetails", new FieldMapping("previousApplicationDetails", false)},
                {"adoxio_ruralagencystoreappointment", new FieldMapping("ruralAgencyStoreAppointment", true)},
                {"adoxio_liquorindustryconnections", new FieldMapping("liquorIndustryConnections", true)},
                {
                    "adoxio_liquorindustryconnectionsdetails",
                    new FieldMapping("liquorIndustryConnectionsDetails", false)
                },
                {"adoxio_otherbusinessesatthesamelocation", new FieldMapping("otherBusinesses", true)},
                {"adoxio_otherbusinesssamelocationdetails", new FieldMapping("otherBusinessesDetails", false)},


                {"adoxio_name", new FieldMapping("name", true)},
                {"adoxio_establishmentpropsedname", new FieldMapping("establishmentName", true)},
                {"adoxio_establishmentopeningdate", new FieldMapping("establishmentopeningdate", true)},
                {"adoxio_establishmentaddressstreet", new FieldMapping("establishmentAddressStreet", true)},
                {"adoxio_establishmentaddresscity", new FieldMapping("establishmentAddressCity", true)},
                {"adoxio_establishmentaddresspostalcode", new FieldMapping("establishmentAddressPostalCode", true)},
                {"adoxio_establishmentparcelid", new FieldMapping("establishmentParcelId", true)},
                {"adoxio_establishmentphone", new FieldMapping("establishmentPhone", true)},
                {"adoxio_establishmentemail", new FieldMapping("establishmentEmail", true)},
                {"adoxio_establishmentcomplytoallbylaws", new FieldMapping("establishmentComplyToBylaws", true)},
                {"adoxio_contactpersonfirstname", new FieldMapping("contactPersonFirstName", true)},
                {"adoxio_contactpersonlastname", new FieldMapping("contactPersonLastName", true)},
                {"adoxio_role", new FieldMapping("contactPersonRole", true)},
                {"adoxio_email", new FieldMapping("contactPersonEmail", true)},
                {"adoxio_contactpersonphone", new FieldMapping("contactPersonPhone", true)},
                {"adoxio_authorizedtosubmit", new FieldMapping("authorizedToSubmit", true)},
                {"adoxio_signatureagreement", new FieldMapping("signatureAgreement", true)},
                {"adoxio_additionalpropertyinformation", new FieldMapping("additionalPropertyInformation", true)},
                {"adoxio_federalproducernames", new FieldMapping("federalProducerNames", true)},
                {"adoxio_renewalcriminaloffencecheck", new FieldMapping("renewalCriminalOffenceCheck", true)},
                {"adoxio_renewalunreportedsaleofbusiness", new FieldMapping("renewalUnreportedSaleOfBusiness", true)},
                {"adoxio_renewalbusinesstype", new FieldMapping("renewalBusinessType", true)},
                {"adoxio_renewaltiedhouse", new FieldMapping("renewalTiedhouse", true)},
                {"adoxio_renewalorgleadership", new FieldMapping("renewalOrgLeadership", true)},
                {"adoxio_renewalkeypersonnel", new FieldMapping("renewalkeypersonnel", true)},
                {"adoxio_renewalshareholders", new FieldMapping("renewalShareholders", true)},
                {"adoxio_renewaloutstandingfines", new FieldMapping("renewalOutstandingFines", true)},
                {"adoxio_renewalbranding", new FieldMapping("renewalBranding", true)},
                {"adoxio_renewalsignage", new FieldMapping("renewalSignage", true)},
                {"adoxio_renewalestablishmentaddress", new FieldMapping("renewalEstablishmentAddress", true)},
                {"adoxio_renewalvalidinterest", new FieldMapping("renewalValidInterest", true)},
                {"adoxio_renewalzoning", new FieldMapping("renewalZoning", true)},
                {"adoxio_renewalfloorplan", new FieldMapping("renewalFloorPlan", true)},
                {"adoxio_renewalsitemap", new FieldMapping("renewalSiteMap", true)},
                {"adoxio_renewaltiedhousefederalinterest", new FieldMapping("tiedhouseFederalInterest", true)},
                {"adoxio_description1", new FieldMapping("description1", true)},

                //store opening
                {"adoxio_isreadyworkers", new FieldMapping("isReadyWorkers", true)},
                {"adoxio_isreadynamebranding", new FieldMapping("isReadyNameBranding", true)},
                {"adoxio_isreadydisplays", new FieldMapping("isReadyDisplays", true)},
                {"adoxio_isreadyintruderalarm", new FieldMapping("isReadyIntruderAlarm", true)},
                {"adoxio_isreadyfirealarm", new FieldMapping("isReadyFireAlarm", true)},
                {"adoxio_isreadylockedcases", new FieldMapping("isReadyLockedCases", true)},
                {"adoxio_isreadylockedstorage", new FieldMapping("isReadyLockedStorage", true)},
                {"adoxio_isreadyperimeter", new FieldMapping("isReadyPerimeter", true)},
                {"adoxio_Isreadyretailarea", new FieldMapping("isReadyRetailArea", true)},
                {"adoxio_isreadystorage", new FieldMapping("isReadyStorage", true)},
                {"adoxio_isreadyentranceexit", new FieldMapping("isReadyExtranceExit", true)},
                {"adoxio_isreadysurveillancenotice", new FieldMapping("isReadySurveillanceNotice", true)},
                {"adoxio_isreadyproductnotvisibleoutside", new FieldMapping("isReadyProductNotVisibleOutside", true)},                
                {"adoxio_isreadyvalidinterest", new FieldMapping("isReadyValidInterest", true)},

                {"adoxio_applicanttype", new FieldMapping("applicantType", true)},

                // manufacturer fields.

                {"adoxio_ispackaging", new FieldMapping("isPackaging", true)},
                {"adoxio_mfgpipedinproduct", new FieldMapping("mfgPipedInProduct", true)},
                {"adoxio_mfgbrewpubonsite", new FieldMapping("mfgBrewPubOnSite", true)},

                // manufacturer structural change fields

                {"adoxio_patiocompdescription", new FieldMapping("patioCompDescription", true)},
                {"adoxio_patiolocationdescription", new FieldMapping("patioLocationDescription", true)},
                {"adoxio_patioaccessdescription", new FieldMapping("patioAccessDescription", true)},
                {"adoxio_patioisliquorcarried", new FieldMapping("patioIsLiquorCarried", false)},
                {"adoxio_patioliquorcarrieddescription", new FieldMapping("patioLiquorCarriedDescription", false)},
                {"adoxio_patioaccesscontroldescription", new FieldMapping("patioAccessControlDescription", true)},
                {"adoxio_locatedabovedescription", new FieldMapping("locatedAboveDescription", true)},
                {"adoxio_patioservicebar", new FieldMapping("patioServiceBar", true)},

                {"adoxio_locatedaboveother", new FieldMapping("locatedAboveOther", false)},

                {"adoxio_description2", new FieldMapping("description2", false)},
                {"adoxio_description3", new FieldMapping("description3", false)},

                //Temporary changes dates
                {"adoxio_tempdatefrom", new FieldMapping("tempDateFrom", true)},
                {"adoxio_tempdateto", new FieldMapping("tempDateTo", true)},
                // eligibility fields
                {"adoxio_isrlrslocatedinruralcommunityalone", new FieldMapping("isRlrsLocatedInRuralCommunityAlone", true)},
                {"adoxio_isrlrslocatedattouristdestinationalone", new FieldMapping("isRlrsLocatedAtTouristDestinationAlone", false)},
                {"adoxio_describerlrsresortcommunity", new FieldMapping("rlrsResortCommunityDescription", false)},
                {"adoxio_hasyearroundallweatherroadaccess", new FieldMapping("hasYearRoundAllWeatherRoadAccess", true)},
                {"adoxio_doesgeneralstoreoperateseasonally", new FieldMapping("doesGeneralStoreOperateSeasonally", true)},
                {"adoxio_surroundingresidentsofrlrs", new FieldMapping("surroundingResidentsOfRlrs", true)},
                {"adoxio_isrlrsatleast10kmfromanotherstore", new FieldMapping("isRlrsAtLeast10kmFromAnotherStore", true)},
                {"adoxio_isapplicantownerofstore", new FieldMapping("isApplicantOwnerOfStore", true)},
                {"adoxio_legalandbeneficialownersofstore", new FieldMapping("legalAndBeneficialOwnersOfStore", false)},
                {"adoxio_isapplicantfranchiseoraffiliated", new FieldMapping("isApplicantFranchiseOrAffiliated", true)},
                {"adoxio_franchiseoraffiliatedbusiness", new FieldMapping("franchiseOrAffiliatedBusiness", false)},
                {"adoxio_hassufficientrangeofproducts", new FieldMapping("hasSufficientRangeOfProducts", true)},
                {"adoxio_hasotherproducts", new FieldMapping("hasOtherProducts", true)},
                {"adoxio_hasadditionalservices", new FieldMapping("hasAdditionalServices", true)},
                
                {"adoxio_confirmliquorsalesisnotprimarybusiness", new FieldMapping("confirmLiquorSalesIsNotPrimaryBusiness", true)},
                // patio application fields
                {"adoxio_ispatioboundingsufficientforcontrol", new FieldMapping("isBoundingSufficientForControl", true)},
                {"adoxio_ispatioboundingsufficienttodefinearea", new FieldMapping("isBoundingSufficientToDefine", true)},
                {"adoxio_isadequatecareandcontroloverthepatio", new FieldMapping("isAdequateCare", true)},
                {"adoxio_ispatioincompliancewithbylaws", new FieldMapping("isInCompliance", true)},                
                {"adoxio_storeopendate", new FieldMapping("storeOpenDate", false)},
                {"adoxio_statusofconstruction", new FieldMapping("statusOfConstruction", true)},

                // 2024-04-03 LCSD-6975 waynezen
                {"adoxio_ishaspatio", new FieldMapping("isHasPatio", false)},

                {"adoxio_ispatiotesa", new FieldMapping("isTESA", false)},
                {"adoxio_m01", new FieldMapping("isMonth01", false)},
                {"adoxio_m02", new FieldMapping("isMonth02", false)},
                {"adoxio_m03", new FieldMapping("isMonth03", false)},
                {"adoxio_m04", new FieldMapping("isMonth04", false)},
                {"adoxio_m05", new FieldMapping("isMonth05", false)},
                {"adoxio_m06", new FieldMapping("isMonth06", false)},
                {"adoxio_m07", new FieldMapping("isMonth07", false)},
                {"adoxio_m08", new FieldMapping("isMonth08", false)},
                {"adoxio_m09", new FieldMapping("isMonth09", false)},
                {"adoxio_m10", new FieldMapping("isMonth10", false)},
                {"adoxio_m11", new FieldMapping("isMonth11", false)},
                {"adoxio_m12", new FieldMapping("isMonth12", false)},
                {"adoxio_dormancyreasons", new FieldMapping("dormancyReasons", true)},
                {"adoxio_dormancystartdate", new FieldMapping("dormancyStartDate", true)},
                {"adoxio_dormancyenddate", new FieldMapping("dormancyEndDate", true)},
                {"adoxio_dormancyreporteddate", new FieldMapping("dormancyReportedDate", true)},
                {"adoxio_dormancyintentionforreopening", new FieldMapping("dormancyIntentionForReopening", true)},
                {"adoxio_dormancynotes", new FieldMapping("dormancyNotes", true)},
                {"adoxio_establishmentstatus", new FieldMapping("establishmentStatus", true)},
                {"adoxio_validinterestestablishmentlocation", new FieldMapping("validInterestEstablishmentLocation", true)},
                {"adoxio_validinterestdormancyperiod", new FieldMapping("validInterestDormancyPeriod", true)},
                {"adoxio_affirminformationproividedtrueandcomplete", new FieldMapping("affirmInformationProividedTrueAndComplete", true)},
                {"adoxio_establishmentreopeningdate", new FieldMapping("establishmentReopeningDate", true)}
        };
        }
    }
}
