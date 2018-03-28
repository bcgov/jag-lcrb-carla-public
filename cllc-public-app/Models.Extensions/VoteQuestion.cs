using System.Collections.Generic;
using System.Linq;

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
                result.options = options.ToArray();
                result.question = voteQuestion.Question;
                result.slug = voteQuestion.Slug;
                result.title = voteQuestion.Title;
            }            
            return result;
        }        
    }
}
