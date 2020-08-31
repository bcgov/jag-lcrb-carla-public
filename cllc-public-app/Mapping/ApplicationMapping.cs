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
                { "adoxio_previouslicenceapplication", new FieldMapping ("previousApplication", true) },
                { "adoxio_previouslicenceapplicationdetails", new FieldMapping ("previousApplicationDetails", true) },
                { "adoxio_ruralagencystoreappointment", new FieldMapping ("ruralAgencyStoreAppointment", true) },
                { "adoxio_liquorindustryconnections", new FieldMapping ("liquorIndustryConnections", true) },
                { "adoxio_liquorindustryconnectionsdetails", new FieldMapping ("liquorIndustryConnectionsDetails", true) },                
                { "adoxio_otherbusinessesatthesamelocation", new FieldMapping ("otherBusinesses", true)},
                { "adoxio_otherbusinesssamelocationdetails", new FieldMapping ("otherBusinessesDetails", true) },


                {"adoxio_name",new FieldMapping ("name", true) },
                {"adoxio_establishmentpropsedname",new FieldMapping ("establishmentName", true) },
                {"adoxio_establishmentaddressstreet", new FieldMapping ("establishmentAddressStreet", true) },
                {"adoxio_establishmentaddresscity",new FieldMapping ("establishmentAddressCity", true) },
                {"adoxio_establishmentaddresspostalcode",new FieldMapping ("establishmentAddressPostalCode", true) },
                {"adoxio_establishmentparcelid",new FieldMapping ("establishmentParcelId", true) },
                {"adoxio_establishmentphone",new FieldMapping ("establishmentPhone", true) },
                {"adoxio_establishmentemail",new FieldMapping ("establishmentEmail", true) },
                {"adoxio_contactpersonfirstname",new FieldMapping ("contactPersonFirstName", true)},
                {"adoxio_contactpersonlastname",new FieldMapping ("contactPersonLastName", true) },
                {"adoxio_role",new FieldMapping ("contactPersonRole", true)},
                {"adoxio_email", new FieldMapping ("contactPersonEmail", true)},
                {"adoxio_contactpersonphone", new FieldMapping ("contactPersonPhone", true)},
                {"adoxio_authorizedtosubmit", new FieldMapping ("authorizedToSubmit", true)},
                {"adoxio_signatureagreement", new FieldMapping ("signatureAgreement", true)},
                {"adoxio_additionalpropertyinformation", new FieldMapping ("additionalPropertyInformation", true)},
                {"adoxio_federalproducernames", new FieldMapping ("federalProducerNames", true)},
                {"adoxio_renewalcriminaloffencecheck", new FieldMapping ("renewalCriminalOffenceCheck", true)},
                {"adoxio_renewalunreportedsaleofbusiness", new FieldMapping ("renewalUnreportedSaleOfBusiness", true)},
                {"adoxio_renewalbusinesstype", new FieldMapping ("renewalBusinessType", true)},
                {"adoxio_renewaltiedhouse", new FieldMapping ("renewalTiedhouse", true)},
                {"adoxio_renewalorgleadership", new FieldMapping ("renewalOrgLeadership", true)},
                {"adoxio_renewalkeypersonnel", new FieldMapping ("renewalkeypersonnel", true)},
                {"adoxio_renewalshareholders", new FieldMapping ("renewalShareholders", true)},
                {"adoxio_renewaloutstandingfines", new FieldMapping ("renewalOutstandingFines", true)},
                {"adoxio_renewalbranding", new FieldMapping( "renewalBranding", true)},
                {"adoxio_renewalsignage", new FieldMapping ("renewalSignage", true)},
                {"adoxio_renewalestablishmentaddress", new FieldMapping ("renewalEstablishmentAddress", true)},
                {"adoxio_renewalvalidinterest", new FieldMapping ("renewalValidInterest", true)},
                {"adoxio_renewalzoning", new FieldMapping ("renewalZoning", true)},
                {"adoxio_renewalfloorplan", new FieldMapping ("renewalFloorPlan", true)},
                {"adoxio_renewalsitemap", new FieldMapping ("renewalSiteMap", true)},
                {"adoxio_renewaltiedhousefederalinterest", new FieldMapping ("tiedhouseFederalInterest", true)},
                {"adoxio_description1", new FieldMapping ("description1", true)},

                //store opening
                {"adoxio_isreadyworkers", new FieldMapping ("isReadyWorkers", true)},
                {"adoxio_isreadynamebranding", new FieldMapping ("isReadyNameBranding", true)},
                {"adoxio_isreadydisplays", new FieldMapping ("isReadyDisplays", true)},
                {"adoxio_isreadyintruderalarm", new FieldMapping ("isReadyIntruderAlarm", true)},
                {"adoxio_isreadyfirealarm", new FieldMapping ("isReadyFireAlarm", true)},
                {"adoxio_isreadylockedcases", new FieldMapping ("isReadyLockedCases", true)},
                {"adoxio_isreadylockedstorage", new FieldMapping ("isReadyLockedStorage", true)},
                {"adoxio_isreadyperimeter", new FieldMapping ("isReadyPerimeter", true)},
                {"adoxio_Isreadyretailarea", new FieldMapping ("isReadyRetailArea", true)},
                {"adoxio_isreadystorage", new FieldMapping ("isReadyStorage", true)},
                {"adoxio_isreadyentranceexit", new FieldMapping ("isReadyExtranceExit", true)},
                {"adoxio_isreadysurveillancenotice", new FieldMapping ("isReadySurveillanceNotice", true)},
                {"adoxio_isreadyproductnotvisibleoutside", new FieldMapping ("isReadyProductNotVisibleOutside", true)},
                {"adoxio_establishmentopeningdate", new FieldMapping ("establishmentopeningdate", true)},
                {"adoxio_isreadyvalidinterest", new FieldMapping ("isReadyValidInterest", true)},

                {"adoxio_applicanttype", new FieldMapping ("applicantType", true)},

                // manufacturer fields.

                { "adoxio_ispackaging", new FieldMapping ("isPackaging", true)},
                { "adoxio_mfgpipedinproduct", new FieldMapping ("mfgPipedInProduct", true)},
                { "adoxio_mfgbrewpubonsite", new FieldMapping ("mfgBrewPubOnSite", true)},

                // manufacturer structural change fields

                { "adoxio_patiocompdescription", new FieldMapping ("patioCompDescription", true)},
                { "adoxio_patiolocationdescription", new FieldMapping ("patioLocationDescription", true) },
                { "adoxio_patioaccessdescription", new FieldMapping ("patioAccessDescription", true)},
                { "adoxio_patioisliquorcarried", new FieldMapping ("patioIsLiquorCarried", true)},
                { "adoxio_patioliquorcarrieddescription", new FieldMapping ("patioLiquorCarriedDescription", true)},
                { "adoxio_patioaccesscontroldescription", new FieldMapping ("patioAccessControlDescription", true)},
                { "adoxio_locatedabovedescription", new FieldMapping ("locatedAboveDescription", true)},
                { "adoxio_patioservicebar", new FieldMapping ("patioServiceBar", true)}

        };
        }
    }
}
