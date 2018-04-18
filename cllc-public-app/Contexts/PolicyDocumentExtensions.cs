
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Contexts
{
    public static class PolicyDocumentExtensions
    {

        public static PolicyDocument GetPolicyDocumentBySlug(this AppDbContext context, string slug)
        {
            Models.PolicyDocument PolicyDocument = context.PolicyDocuments.FirstOrDefault(x => x.Slug == slug);
            return PolicyDocument;
        }
        

        /// <summary>
        /// Add a PolicyDocument
        /// </summary>
        /// <param name="context"></param>
        /// <param name="PolicyDocument"></param>
        public static void AddPolicyDocument(this AppDbContext context, PolicyDocument PolicyDocument)
        {
            if (PolicyDocument != null)
            {
                context.PolicyDocuments.Add(PolicyDocument);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Create PolicyDocuments from a (json) file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="PolicyDocumentJsonPath"></param>
        public static void AddInitialPolicyDocumentsFromFile(this AppDbContext context, string PolicyDocumentJsonPath)
        {
            if (!string.IsNullOrEmpty(PolicyDocumentJsonPath) && File.Exists(PolicyDocumentJsonPath))
            {
                string PolicyDocumentJson = File.ReadAllText(PolicyDocumentJsonPath);
                context.AddInitialPolicyDocuments(PolicyDocumentJson);
            }
        }

        private static void AddInitialPolicyDocuments(this AppDbContext context, string PolicyDocumentJson)
        {
            List<ViewModels.PolicyDocument> PolicyDocuments = JsonConvert.DeserializeObject<List<ViewModels.PolicyDocument>>(PolicyDocumentJson);

            if (PolicyDocuments != null)
            {
                context.AddInitialPolicyDocuments(PolicyDocuments);
            }
        }

        private static void AddInitialPolicyDocuments(this AppDbContext context, List<ViewModels.PolicyDocument> PolicyDocuments)
        {
            PolicyDocuments.ForEach(context.AddInitialPolicyDocument);
        }

        /// <summary>
        /// Adds a jurisdiction to the system, only if it does not exist.
        /// </summary>
        private static void AddInitialPolicyDocument(this AppDbContext context, ViewModels.PolicyDocument initialPolicyDocument)
        {
            PolicyDocument PolicyDocument = context.GetPolicyDocumentBySlug(initialPolicyDocument.slug);
            if (PolicyDocument != null)
            {
                return;
            }

            PolicyDocument = new PolicyDocument
            (                
                initialPolicyDocument.slug,
                initialPolicyDocument.title,
                initialPolicyDocument.intro,
                initialPolicyDocument.body,
                initialPolicyDocument.displayOrder
            );

            context.AddPolicyDocument(PolicyDocument);
        }


        /// <summary>
        /// Update region
        /// </summary>
        /// <param name="context"></param>
        /// <param name="regionInfo"></param>
        public static void UpdateSeedPolicyDocumentInfo(this AppDbContext context, Models.PolicyDocument PolicyDocumentInfo)
        {
            PolicyDocument PolicyDocument = context.GetPolicyDocumentBySlug(PolicyDocumentInfo.Slug);
            if (PolicyDocument == null)
            {
                context.AddInitialPolicyDocument(PolicyDocumentInfo.ToViewModel());
            }
            else
            {
                PolicyDocument.Body = PolicyDocumentInfo.Body;
                PolicyDocument.Title = PolicyDocumentInfo.Title;
                PolicyDocument.Intro = PolicyDocumentInfo.Intro;
                PolicyDocument.DisplayOrder = PolicyDocumentInfo.DisplayOrder;
                context.PolicyDocuments.Update(PolicyDocument);
                context.SaveChanges();
            }
        }
    }
}
