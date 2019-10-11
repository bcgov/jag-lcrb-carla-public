using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class VoteQuestionExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.VoteQuestion ToViewModel(this Models.VoteQuestion voteQuestion)
        {
            ViewModels.VoteQuestion result = null;
            if (voteQuestion != null)
            {
                result = new ViewModels.VoteQuestion();
                result.id = voteQuestion.Id.ToString();
                List<ViewModels.VoteOption> options = new List<ViewModels.VoteOption>();
                foreach (Models.VoteOption option in voteQuestion.Options)
                {
                    options.Add(option.ToViewModel());
                }
                // sort the options by display order.
                options.Sort(
                    (x, y) =>
                        x == null ? (y == null ? 0 : -1)
                                    : (y == null ? 1 : x.displayOrder.CompareTo(y.displayOrder))
                );
                result.options = options.ToArray();
                result.question = voteQuestion.Question;
                result.slug = voteQuestion.Slug;
                result.title = voteQuestion.Title;
            }
            return result;
        }
    }
}
