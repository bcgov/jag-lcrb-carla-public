using System;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class VoteOptionExtensions
    {
        /// <summary>
        /// Convert a given VoteOption to a view model.
        /// </summary>
        /// <param name="permission">The permission to add.</param>
        public static ViewModels.VoteOption ToViewModel(this Models.VoteOption voteOption)
        {
            var result = new ViewModels.VoteOption();
            result.id = voteOption.Id.ToString();
            result.totalVotes = voteOption.TotalVotes;
            result.option = voteOption.Option;
            result.displayOrder = voteOption.DisplayOrder;
            return result;
        }

        /// <summary>
        /// Covert a view model into a model
        /// </summary>
        /// <param name="voteOption"></param>
        /// <returns></returns>
        public static Models.VoteOption ToModel(this ViewModels.VoteOption voteOption)
        {
            var result = new Models.VoteOption();
            if (voteOption != null)
            {
                if (!string.IsNullOrEmpty(voteOption.id))
                    result.Id = new Guid(voteOption.id);
                result.TotalVotes = voteOption.totalVotes;
                result.Option = voteOption.option;
                result.DisplayOrder = voteOption.displayOrder;
            }
            return result;
        }
    }
}
