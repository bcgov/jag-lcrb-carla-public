extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class PolicyDocumentExtensions
    {
        public static ViewModels.PolicyDocument ToViewModel(this adoxio_policydocument policyDocument)
        {
            if (policyDocument == null) return null;
            var result = new ViewModels.PolicyDocument
            {
                id = policyDocument.adoxio_policydocumentId?.ToString(),
                slug = policyDocument.adoxio_Slug,
                title = policyDocument.adoxio_name,
                category = policyDocument.adoxio_Category,
                menuText = policyDocument.adoxio_MenuText,
                body = policyDocument.adoxio_Body
            };
            if (policyDocument.adoxio_DisplayOrder.HasValue)
                result.displayOrder = policyDocument.adoxio_DisplayOrder.Value;
            return result;
        }

        public static ViewModels.PolicyDocumentSummary ToSummaryViewModel(this adoxio_policydocument policyDocument)
        {
            if (policyDocument == null) return null;
            return new ViewModels.PolicyDocumentSummary
            {
                slug = policyDocument.adoxio_Slug,
                menuText = policyDocument.adoxio_MenuText
            };
        }
    }
}
