extern alias DV;

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Spice.Models;
using Serilog;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public class Validation
    {
        public static bool ValidatePostalCode(string postalCode)
        {
            if(postalCode == null)
            {
                return false;
            }

            string canadianPattern = @"^[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ] ?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]$";
            string usPattern = @"^\d{5}(?:[-\s]\d{4})?$";

            bool valid = Regex.IsMatch(postalCode, canadianPattern, RegexOptions.IgnoreCase);
            if (!valid)
            {
                valid = Regex.IsMatch(postalCode, usPattern);
            }
            return valid;
        }

        /// <summary>
        /// Validates the consent of a legal entity list (including all of its children)
        /// </summary>
        /// <returns><c>true</c>, if associate consent was validated, <c>false</c> otherwise.</returns>
        /// <param name="associates">Associates.</param>
        public static async Task<bool> ValidateAssociateConsentAsync(IDataverseClient dataverse, List<LegalEntity> associates)
        {
            bool consentValidated = true;
            foreach (var entity in associates)
            {
                if ((bool)entity.IsIndividual)
                {
                    var id = entity.Contact.ContactId;
                    var contact = await dataverse.GetContactByIdAsync(id);
                    if (contact == null || contact.StatusCode == contact_statuscode.Active && contact.adoxio_ConsentValidated == null)
                    {
                        Log.Logger.Error($"Consent not validated for associate: {id}");
                        consentValidated = false;
                        continue;
                    }

                    if (contact.StatusCode == contact_statuscode.Active &&
                        contact.adoxio_ConsentValidated != adoxio_contact_adoxio_consentvalidated.Yes)
                    {
                        Log.Logger.Error($"Consent not validated for associate: {id}");
                        consentValidated = false;
                    }
                }
                else
                {
                    if (!await ValidateAssociateConsentAsync(dataverse, (List<LegalEntity>)entity.Account.Associates))
                    {
                        consentValidated = false;
                    }
                }
            }
            return consentValidated;
        }
    }
}
