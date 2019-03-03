using Gov.Lclb.Cllb.Interfaces.Models;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class PolicyDocumentExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.PolicyDocument ToViewModel(this MicrosoftDynamicsCRMadoxioPolicydocument policyDocument)
        {
            ViewModels.PolicyDocument result = null;
            if (policyDocument != null)
            {
                result = new ViewModels.PolicyDocument()
                {
                    id = policyDocument.AdoxioPolicydocumentid,
                    slug = policyDocument.AdoxioSlug,
                    title = policyDocument.AdoxioName,
                    category = policyDocument.AdoxioCategory,
                    menuText = policyDocument.AdoxioMenutext,
                    body = policyDocument.AdoxioBody
                };
                if (policyDocument.AdoxioDisplayorder != null)
                {
                    result.displayOrder = (int)policyDocument.AdoxioDisplayorder;
                }                
            }            
            return result;
        }

        public static ViewModels.PolicyDocumentSummary ToSummaryViewModel(this MicrosoftDynamicsCRMadoxioPolicydocument policyDocument)
        {
            ViewModels.PolicyDocumentSummary result = null;
            if (policyDocument != null)
            {
                result = new ViewModels.PolicyDocumentSummary()
                {
                    slug = policyDocument.AdoxioSlug,
                    menuText = policyDocument.AdoxioMenutext
                };               
            }
            return result;
        }


    }
}
