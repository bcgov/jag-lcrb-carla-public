
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Contexts
{
    public static class NewsletterExtensions
    {

        public static Newsletter GetNewsletterBySlug(this AppDbContext context, string slug)
        {
            Models.Newsletter newsletter = context.Newsletters.Include(x => x.Subscribers).FirstOrDefault(x => x.Slug == slug);
            return newsletter;
        }

        public static void AddNewsletterSubscriber(this AppDbContext context, string slug, string email)
        {
            Newsletter newsletter = context.GetNewsletterBySlug(slug);
            if (newsletter != null)
            {
                Subscriber existing = newsletter.Subscribers.FirstOrDefault(x => x.Email == email);
                if (existing == null)
                {
                    Subscriber newSubscriber = new Subscriber(email);
                    if (newsletter.Subscribers == null)
                    {
                        newsletter.Subscribers = new List<Subscriber>();
                    }
                    newsletter.Subscribers.Add(newSubscriber);
                    context.Newsletters.Update(newsletter);
                    context.SaveChanges();
                }
            }
        }


        public static void RemoveNewsletterSubscriber(this AppDbContext context, string slug, string email)
        {
            Newsletter newsletter = context.GetNewsletterBySlug(slug);
            if (newsletter != null)
            {
                Subscriber existing = newsletter.Subscribers.FirstOrDefault(x => x.Email == email);
                if (existing != null)
                {
                    newsletter.Subscribers.Remove(existing);
                    context.Newsletters.Update(newsletter);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Add a newsletter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="newsletter"></param>
        public static void AddNewsletter(this AppDbContext context, Newsletter newsletter)
        {
            if (newsletter != null)
            {
                context.Newsletters.Add(newsletter);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Create Newsletters from a (json) file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="NewsletterJsonPath"></param>
        public static void AddInitialNewslettersFromFile(this AppDbContext context, string NewsletterJsonPath)
        {
            if (!string.IsNullOrEmpty(NewsletterJsonPath) && File.Exists(NewsletterJsonPath))
            {
                string NewsletterJson = File.ReadAllText(NewsletterJsonPath);
                context.AddInitialNewsletters(NewsletterJson);
            }
        }

        private static void AddInitialNewsletters(this AppDbContext context, string NewsletterJson)
        {
            List<ViewModels.Newsletter> Newsletters = JsonConvert.DeserializeObject<List<ViewModels.Newsletter>>(NewsletterJson);

            if (Newsletters != null)
            {
                context.AddInitialNewsletters(Newsletters);
            }
        }

        private static void AddInitialNewsletters(this AppDbContext context, List<ViewModels.Newsletter> Newsletters)
        {
            Newsletters.ForEach(context.AddInitialNewsletter);
        }

        /// <summary>
        /// Adds a jurisdiction to the system, only if it does not exist.
        /// </summary>
        private static void AddInitialNewsletter(this AppDbContext context, ViewModels.Newsletter initialNewsletter)
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
        public static void UpdateSeedNewsletterInfo(this AppDbContext context, Models.Newsletter newsletterInfo)
        {
            Newsletter newsletter = context.GetNewsletterBySlug(newsletterInfo.Slug);
            if (newsletter == null)
            {
                context.AddInitialNewsletter(newsletterInfo.ToViewModel());
            }
            else
            {
                newsletter.Description = newsletterInfo.Description;
                newsletter.Title = newsletterInfo.Title;
                context.Newsletters.Update(newsletter);
                context.SaveChanges();
            }
        }
    }
}
