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
            fieldMap = new Dictionary<string, string>
            {
                {"AdoxioName","Name" },
                {"AdoxioEstablishmentpropsedname","EstablishmentName" },
                {"AdoxioEstablishmentaddressstreet", "EstablishmentAddressStreet" },
                {"AdoxioEstablishmentaddresscity","EstablishmentAddressCity" },
                {"AdoxioEstablishmentaddresspostalcode","EstablishmentAddressPostalCode" },
                {"AdoxioEstablishmentparcelid","EstablishmentParcelId" },
                {"AdoxioEstablishmentphone","EstablishmentPhone" },
                {"AdoxioEstablishmentemail","EstablishmentEmail" },
                {"AdoxioContactpersonfirstname","ContactPersonFirstName"},
                {"AdoxioContactpersonlastname","ContactPersonLastName" },
                {"AdoxioRole","ContactPersonRole"},
                {"AdoxioEmail", "ContactPersonEmail"},
                {"AdoxioContactpersonphone", "ContactPersonPhone"},
                {"AdoxioAuthorizedtosubmit", "AuthorizedToSubmit"},
                {"AdoxioSignatureagreement", "SignatureAgreement"},
                {"AdoxioAdditionalpropertyinformation", "AdditionalPropertyInformation"},
                {"AdoxioFederalproducernames", "FederalProducerNames"},
                {"AdoxioRenewalcriminaloffencecheck", "RenewalCriminalOffenceCheck"},
                {"AdoxioRenewalunreportedsaleofbusiness", "RenewalUnreportedSaleOfBusiness"},
                {"AdoxioRenewalbusinesstype", "RenewalBusinessType"},
                {"AdoxioRenewaltiedhouse", "RenewalTiedhouse"},
                {"AdoxioRenewalorgleadership", "RenewalOrgLeadership"},
                {"AdoxioRenewalkeypersonnel", "Renewalkeypersonnel"},
                {"AdoxioRenewalshareholders", "RenewalShareholders"},
                {"AdoxioRenewaloutstandingfines", "RenewalOutstandingFines"},
                {"AdoxioRenewalbranding", "RenewalBranding"},
                {"AdoxioRenewalsignage", "RenewalSignage"},
                {"AdoxioRenewalestablishmentaddress", "RenewalEstablishmentAddress"},
                {"AdoxioRenewalvalidinterest", "RenewalValidInterest"},
                {"AdoxioRenewalzoning", "RenewalZoning"},
                {"AdoxioRenewalfloorplan", "RenewalFloorPlan"},
                {"AdoxioRenewalsitemap", "RenewalSiteMap"},
                {"AdoxioRenewaltiedhousefederalinterest", "TiedhouseFederalInterest"},
                {"AdoxioDescription1", "Description1"},

                //store opening
                {"AdoxioIsreadyworkers", "IsReadyWorkers"},
                {"AdoxioIsreadynamebranding", "IsReadyNameBranding"},
                {"AdoxioIsreadydisplays", "IsReadyDisplays"},
                {"AdoxioIsreadyintruderalarm", "IsReadyIntruderAlarm"},
                {"AdoxioIsreadyfirealarm", "IsReadyFireAlarm"},
                {"AdoxioIsreadylockedcases", "IsReadyLockedCases"},
                {"AdoxioIsreadylockedstorage", "IsReadyLockedStorage"},
                {"AdoxioIsreadyperimeter", "IsReadyPerimeter"},
                {"AdoxioIsreadyretailarea", "IsReadyRetailArea"},
                {"AdoxioIsreadystorage", "IsReadyStorage"},
                {"AdoxioIsreadyentranceexit", "IsReadyExtranceExit"},
                {"AdoxioIsreadysurveillancenotice", "IsReadySurveillanceNotice"},
                {"AdoxioIsreadyproductnotvisibleoutside", "IsReadyProductNotVisibleOutside"},
                {"AdoxioEstablishmentopeningdate", "Establishmentopeningdate"},
                {"AdoxioIsreadyvalidinterest", "IsReadyValidInterest"},
                

                {"AdoxioApplicanttype", "ApplicantType"}
        };
        }
    }
}
