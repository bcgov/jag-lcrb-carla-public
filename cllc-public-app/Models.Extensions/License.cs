using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class LicenseExtensions
    {
        public static License ToViewModel(this MicrosoftDynamicsCRMadoxioLicences dynamicsLicense, IDynamicsClient dynamicsClient)
        {
            License adoxioLicenseVM = new License();

            adoxioLicenseVM.id = dynamicsLicense.AdoxioLicencesid;

            // fetch the establishment and get name and address
            Guid? adoxioEstablishmentId = null;
            if (!string.IsNullOrEmpty(dynamicsLicense._adoxioEstablishmentValue))
            {
                adoxioEstablishmentId = Guid.Parse(dynamicsLicense._adoxioEstablishmentValue);
            }
            if (adoxioEstablishmentId != null)
            {
                var establishment = dynamicsClient.Establishments.GetByKey(adoxioEstablishmentId.ToString());
                adoxioLicenseVM.establishmentName = establishment.AdoxioName;
                adoxioLicenseVM.establishmentAddress = establishment.AdoxioAddressstreet
                                                    + ", " + establishment.AdoxioAddresscity
                                                    + " " + establishment.AdoxioAddresspostalcode;
            }
            adoxioLicenseVM.expiryDate = dynamicsLicense.AdoxioExpirydate;

            // fetch the licence status
            int? adoxio_licenceStatusId = dynamicsLicense.Statuscode;
            if (adoxio_licenceStatusId != null)
            {
                adoxioLicenseVM.licenseStatus = dynamicsLicense.Statuscode.ToString();
            }

            // fetch the licence type
            Guid? adoxio_licencetypeId = null;
            if (!string.IsNullOrEmpty(dynamicsLicense._adoxioLicencetypeValue))
            {
                adoxio_licencetypeId = Guid.Parse(dynamicsLicense._adoxioLicencetypeValue);
            }
            if (adoxio_licencetypeId != null)
            {
                var adoxio_licencetype = dynamicsClient.Licencetypes.GetByKey(adoxio_licencetypeId.ToString());
                if (adoxio_licencetype != null)
                {
                    adoxioLicenseVM.licenseType = adoxio_licencetype.AdoxioName;
                }                
            }

            // fetch license number
            adoxioLicenseVM.licenseNumber = dynamicsLicense.AdoxioLicencenumber;

            adoxioLicenseVM.establishmentAddressCity = dynamicsLicense.AdoxioEstablishmentaddresscity;
            adoxioLicenseVM.establishmentAddressPostalCode = dynamicsLicense.AdoxioEstablishmentaddresspostalcode;
            adoxioLicenseVM.establishmentAddressStreet = dynamicsLicense.AdoxioEstablishmentaddressstreet;
            
            if (dynamicsLicense.AdoxioEstablishment != null)
            {
                adoxioLicenseVM.establishmentParcelId = dynamicsLicense.AdoxioEstablishment.AdoxioParcelid;
            }

            return adoxioLicenseVM;
        }

        public static ApplicationLicenseSummary ToLicenseSummaryViewModel(this MicrosoftDynamicsCRMadoxioLicences licence, IList<MicrosoftDynamicsCRMadoxioApplication> applications)
        {
            ApplicationLicenseSummary licenseSummary = new ViewModels.ApplicationLicenseSummary()
            {
                LicenseId = licence.AdoxioLicencesid,
                LicenseNumber = licence.AdoxioLicencenumber,
                EstablishmentAddressStreet = licence.AdoxioEstablishmentaddressstreet,
                EstablishmentAddressCity = licence.AdoxioEstablishmentaddresscity,
                EstablishmentAddressPostalCode = licence.AdoxioEstablishmentaddresspostalcode,
                ExpiryDate = licence.AdoxioExpirydate,
                Status = StatusUtility.GetLicenceStatus(licence, applications),
                AllowedActions = new List<ApplicationType>()
            };


            if (licence.AdoxioEstablishment != null)
            {
                licenseSummary.EstablishmentName = licence.AdoxioEstablishment.AdoxioName;
            }


            if (licence.AdoxioLicenceType != null)
            {
                licenseSummary.LicenceTypeName = licence.AdoxioLicenceType.AdoxioName;
            }

            if (licence != null &&
                licence.AdoxioLicenceType != null &&
                licence.AdoxioLicenceType.AdoxioLicencetypesApplicationtypes != null)
            {
                bool addActions = (applications.Any(app => app.Statuscode != (int)Public.ViewModels.AdoxioApplicationStatusCodes.Approved));
                
                if (addActions)
                {
                    foreach (var item in licence.AdoxioLicenceType.AdoxioLicencetypesApplicationtypes)
                    {
                        // check to see if there is an existing action on this licence.
                        licenseSummary.AllowedActions.Add(item.ToViewModel());
                    }

                }
            }

            return licenseSummary;
        }
    }
}
