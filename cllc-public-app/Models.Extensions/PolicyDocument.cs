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
        public static ViewModels.PolicyDocument ToViewModel(this Models.PolicyDocument policyDocument)
        {
            ViewModels.PolicyDocument result = null;
            if (policyDocument != null)
            {
                result = new ViewModels.PolicyDocument();
                result.id = policyDocument.Id.ToString();                
                result.slug = policyDocument.Slug;
                result.title = policyDocument.Title;
                result.intro = policyDocument.Intro;
                result.body = policyDocument.Body;
                result.displayOrder = policyDocument.DisplayOrder;
            }            
            return result;
        }        
    }
}
