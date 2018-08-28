
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Contexts
{
    public static class PolicyDocumentExtensions
    {

        public static MicrosoftDynamicsCRMadoxioPolicydocument GetPolicyDocumentBySlug(this IDynamicsClient dynamicsClient, string slug)
        {

            MicrosoftDynamicsCRMadoxioPolicydocument result = null;
            try
            {
                slug = slug.Replace("'", "''");
                string filter = "adoxio_slug eq '" + slug + "'";
                result = dynamicsClient.Policydocuments.Get(filter: filter).Value
                .FirstOrDefault();

            }
            catch (OdataerrorException)
            {
                result = null;
            }
            return result;
        }
        

        /// <summary>
        /// Add a PolicyDocument
        /// </summary>
        /// <param name="context"></param>
        /// <param name="PolicyDocument"></param>
        public static void AddPolicyDocument(this IDynamicsClient dynamicsClient, MicrosoftDynamicsCRMadoxioPolicydocument PolicyDocument)
        {
            if (PolicyDocument != null)
            {
                try
                {
                    dynamicsClient.Policydocuments.Create(PolicyDocument);                    
                }
                catch (OdataerrorException)
                {
                    
                }                
            }
        }

        /// <summary>
        /// Create PolicyDocuments from a (json) file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="PolicyDocumentJsonPath"></param>
        public static void AddInitialPolicyDocumentsFromFile(this IDynamicsClient dynamicsClient, string PolicyDocumentJsonPath, bool forceUpdate)
        {
            // only add policy documents if they are empty.
            bool addPolicyDocuments = true;
            if (! forceUpdate)
            {
                try
                {
                    var temp = dynamicsClient.Policydocuments.Get();
                    if (temp != null && temp.Value != null && temp.Value.Count > 0)
                    {
                        addPolicyDocuments = false;
                    }
                }
                catch (OdataerrorException)
                {
                    addPolicyDocuments = true;
                }
            }
            
            
            if (addPolicyDocuments && !string.IsNullOrEmpty(PolicyDocumentJsonPath) && File.Exists(PolicyDocumentJsonPath))
            {
                string PolicyDocumentJson = File.ReadAllText(PolicyDocumentJsonPath);
                dynamicsClient.AddInitialPolicyDocuments(PolicyDocumentJson);
            }
        }

        private static void AddInitialPolicyDocuments(this IDynamicsClient dynamicsClient, string PolicyDocumentJson)
        {
            List<ViewModels.PolicyDocument> PolicyDocuments = JsonConvert.DeserializeObject<List<ViewModels.PolicyDocument>>(PolicyDocumentJson);

            if (PolicyDocuments != null)
            {
                dynamicsClient.AddInitialPolicyDocuments(PolicyDocuments);
            }
        }

        private static void AddInitialPolicyDocuments(this IDynamicsClient dynamicsClient, List<ViewModels.PolicyDocument> PolicyDocuments)
        {
            PolicyDocuments.ForEach(dynamicsClient.AddInitialPolicyDocument);
        }

        /// <summary>
        /// Adds a jurisdiction to the system, only if it does not exist.
        /// </summary>
        private static void AddInitialPolicyDocument(this IDynamicsClient dynamicsClient, ViewModels.PolicyDocument initialPolicyDocument)
        {
            MicrosoftDynamicsCRMadoxioPolicydocument PolicyDocument = dynamicsClient.GetPolicyDocumentBySlug(initialPolicyDocument.slug);
            if (PolicyDocument != null)
            {
                return;
            }

            PolicyDocument = new MicrosoftDynamicsCRMadoxioPolicydocument()
            {
                AdoxioSlug = initialPolicyDocument.slug,
                AdoxioName = initialPolicyDocument.title,
                AdoxioMenutext = initialPolicyDocument.menuText,
                AdoxioCategory = initialPolicyDocument.category,
                AdoxioBody = initialPolicyDocument.body,
                AdoxioDisplayorder = initialPolicyDocument.displayOrder
            };


            dynamicsClient.AddPolicyDocument(PolicyDocument);
        }


    }
}
