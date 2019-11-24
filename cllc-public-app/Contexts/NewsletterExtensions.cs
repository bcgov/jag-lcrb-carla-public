
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Contexts
{
    public static class NewsletterExtensions
    {

        public static Newsletter GetNewsletterBySlug(this IDynamicsClient context, string slug)
        {
            // Newsletter is now Marketing List            
            string filter = $"purpose eq '{slug}'";
            Newsletter result = null;
            MicrosoftDynamicsCRMlist list = null;
            try
            {
                list = context.Lists.Get(filter: filter).Value.FirstOrDefault();

                result = new Newsletter()
                {
                    Slug = list.Purpose,
                    Id = Guid.Parse(list.Listid),
                    Title = list.Listname,
                    Description = list.Description

                };
            }
            catch (HttpOperationException)
            {
                list = null;
            }
            catch (Exception)
            {
                list = null;
            }

            return result;
        }

        public static MicrosoftDynamicsCRMlead GetSubscriberByEmail(this IDynamicsClient context, string email)
        {
            MicrosoftDynamicsCRMlead result = null;
            string emailEscaped = email.Replace("'", "''");

            // see if the lead exists.
            string filter = $"email eq '{emailEscaped}'";

            try
            {
                result = context.Leads.Get(filter: filter).Value.FirstOrDefault();
            }
            catch (HttpOperationException)
            {
                result = null;
            }
            catch (Exception)
            {
                result = null;
            }

            return result;


        }

        public static void AddNewsletterSubscriber(this IDynamicsClient dynamicsClient, string slug, string email)
        {
            bool newSubscriber = false;
            Newsletter newsletter = dynamicsClient.GetNewsletterBySlug(slug);
            if (newsletter != null)
            {
                MicrosoftDynamicsCRMlead subscriber = dynamicsClient.GetSubscriberByEmail(email);
                if (subscriber == null)
                {
                    // add the new subscriber
                    MicrosoftDynamicsCRMlead newLead = new MicrosoftDynamicsCRMlead()
                    {
                        Emailaddress1 = email
                    };

                    try
                    {

                        subscriber = dynamicsClient.Leads.Create(newLead);
                        newSubscriber = true;
                    }
                    catch (HttpOperationException)
                    {
                        subscriber = null;
                    }
                    catch (Exception)
                    {
                        subscriber = null;
                    }
                }

                if (subscriber != null)
                {
                    // add the subscriber to the newsletter (Marketing List)

                    // need to check to determine if it is there already.
                }
            }
        }


        public static void RemoveNewsletterSubscriber(this IDynamicsClient context, string slug, string email)
        {
            Newsletter newsletter = context.GetNewsletterBySlug(slug);
            if (newsletter != null)
            {
                /*
                Subscriber existing = newsletter.Subscribers.FirstOrDefault(x => x.Email == email);
                if (existing != null)
                {
                    newsletter.Subscribers.Remove(existing);
                    context.Newsletters.Update(newsletter);
                    context.SaveChanges();
                }
                */
            }
        }


        /// <summary>
        /// Add a newsletter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="newsletter"></param>
        public static void AddNewsletter(this IDynamicsClient context, Newsletter newsletter)
        {
            if (newsletter != null && context != null)
            {
                MicrosoftDynamicsCRMlist list = new MicrosoftDynamicsCRMlist()
                {
                    Purpose = newsletter.Slug,
                    Description = newsletter.Description,
                    Listname = newsletter.Title
                };
                list = context.Lists.Create(list);

            }
        }

        /// <summary>
        /// Create Newsletters from a (json) file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="NewsletterJsonPath"></param>
        public static void AddInitialNewslettersFromFile(this IDynamicsClient context, string NewsletterJsonPath)
        {
            if (!string.IsNullOrEmpty(NewsletterJsonPath) && File.Exists(NewsletterJsonPath))
            {
                string NewsletterJson = File.ReadAllText(NewsletterJsonPath);
                context.AddInitialNewsletters(NewsletterJson);
            }
        }

        private static void AddInitialNewsletters(this IDynamicsClient context, string NewsletterJson)
        {
            List<ViewModels.Newsletter> Newsletters = JsonConvert.DeserializeObject<List<ViewModels.Newsletter>>(NewsletterJson);

            if (Newsletters != null)
            {
                context.AddInitialNewsletters(Newsletters);
            }
        }

        private static void AddInitialNewsletters(this IDynamicsClient context, List<ViewModels.Newsletter> Newsletters)
        {
            Newsletters.ForEach(context.AddInitialNewsletter);
        }

        /// <summary>
        /// Adds a jurisdiction to the system, only if it does not exist.
        /// </summary>
        private static void AddInitialNewsletter(this IDynamicsClient context, ViewModels.Newsletter initialNewsletter)
        {
            Newsletter newsletter = context.GetNewsletterBySlug(initialNewsletter.slug);
            if (newsletter != null)
            {
                return;
            }

            newsletter = new Newsletter
            (
                initialNewsletter.slug,
                initialNewsletter.title,
                initialNewsletter.description
            );

            context.AddNewsletter(newsletter);
        }


        /// <summary>
        /// Update region
        /// </summary>
        /// <param name="context"></param>
        /// <param name="regionInfo"></param>
        public static void UpdateSeedNewsletterInfo(this IDynamicsClient context, Models.Newsletter newsletterInfo)
        {
            Newsletter newsletter = context.GetNewsletterBySlug(newsletterInfo.Slug);
            if (newsletter == null)
            {
                context.AddInitialNewsletter(newsletterInfo.ToViewModel());
            }
            else
            {
                // update Newsletter.
                MicrosoftDynamicsCRMlist list = new MicrosoftDynamicsCRMlist()
                {
                    Purpose = newsletterInfo.Slug,
                    Description = newsletterInfo.Description,
                    Listname = newsletterInfo.Title
                };

                context.Lists.Update(newsletter.Id.ToString(), list);
            }
        }


    }
}
