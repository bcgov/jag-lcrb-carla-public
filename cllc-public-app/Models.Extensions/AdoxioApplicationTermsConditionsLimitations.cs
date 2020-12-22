using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class AdoxioApplicationTermsConditionsLimitations
    {  
        public static TermsAndConditions ToViewModel(this MicrosoftDynamicsCRMadoxioApplicationtermsconditionslimitation term)
        {
            TermsAndConditions result = null;
            if (term != null)
            {
                result = new TermsAndConditions();
                if (term.AdoxioApplicationtermsconditionslimitationid != null)
                {
                    result.Id = term.AdoxioApplicationtermsconditionslimitationid;
                }

                result.LicenceId = term._adoxioLicenceValue;
                result.Content = term.AdoxioTermsandconditions;
            }
            return result;
        }

    }

}
