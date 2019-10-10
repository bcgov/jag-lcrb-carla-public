namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class NewsletterExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.Newsletter ToViewModel(this Models.Newsletter newsletter)
        {
            ViewModels.Newsletter result = null;
            if (newsletter != null)
            {
                result = new ViewModels.Newsletter();
                result.id = newsletter.Id.ToString();
                result.description = newsletter.Description;
                result.slug = newsletter.Slug;
                result.title = newsletter.Title;
            }
            return result;
        }
    }
}
