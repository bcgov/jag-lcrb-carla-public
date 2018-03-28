using System.Linq;

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
    }
}
