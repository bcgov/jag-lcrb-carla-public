
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Contexts
{
    public static class VoteQuestionExtensions
    {
        public static void AddVoteQuestion(this AppDbContext context, VoteQuestion voteQuestion)
        {
            // create a new jurisdiction.           
            context.VoteQuestions.Add(voteQuestion);
            context.SaveChanges();
        }

        public static void UpdateVoteQuestion(this AppDbContext context, ViewModels.VoteQuestion voteQuestion)
        {
            VoteQuestion _voteQuestion = context.VoteQuestions.FirstOrDefault(x => x.Slug == voteQuestion.slug);
            _voteQuestion.Question = voteQuestion.question;
            _voteQuestion.Title = voteQuestion.title;
            context.VoteQuestions.Update(_voteQuestion);
            context.SaveChanges();
        }

        public static List<VoteQuestion> GetVoteQuestions(this AppDbContext context)
        {
            List<Models.VoteQuestion> voteQuestions =
                context.VoteQuestions.Include(x => x.Options).ToList<VoteQuestion>();
            return voteQuestions;
        }

        public static ViewModels.VoteQuestion GetViewModelVoteQuestionBySlug(this AppDbContext context, string slug)
        {
            ViewModels.VoteQuestion voteQuestion =
                context.VoteQuestions.Include(x => x.Options).FirstOrDefault(x => x.Slug == slug).ToViewModel();
            return voteQuestion;
        }


        public static Models.VoteQuestion GetVoteQuestionBySlug(this AppDbContext context, string slug)
        {
            Models.VoteQuestion voteQuestion =
                context.VoteQuestions.Include(x => x.Options).FirstOrDefault(x => x.Slug == slug);
            return voteQuestion;
        }

        public static void AddVote(this AppDbContext context, string optionId)
        {
            Models.VoteOption voteOption = context.VoteOptions.FirstOrDefault(x => x.Id == new Guid(optionId));
            if (voteOption != null)
            {
                voteOption.TotalVotes++;
                context.VoteOptions.Update(voteOption);
                context.SaveChanges();
            }
        }


        /// <summary>
        /// Returns a specific VoteQuestion
        /// </summary>
        /// <param name="name">The name of the VoteQuestion</param>
        /// <returns>The VoteQuestion, or null if it does not exist.</returns>
        public static VoteQuestion GetVoteQuestionById(this AppDbContext context, string id)
        {
            VoteQuestion voteQuestion = context.VoteQuestions.Include(x => x.Options).FirstOrDefault(x => x.Id == new Guid(id));
            return voteQuestion;
        }



        /// <summary>
        /// Create VoteQuestions from a (json) file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="voteQuestionJsonPath"></param>
        public static void AddInitialVoteQuestionsFromFile(this AppDbContext context, string voteQuestionJsonPath)
        {
            if (!string.IsNullOrEmpty(voteQuestionJsonPath) && File.Exists(voteQuestionJsonPath))
            {
                string voteQuestionJson = File.ReadAllText(voteQuestionJsonPath);
                context.AddInitialVoteQuestions(voteQuestionJson);
            }
        }

        private static void AddInitialVoteQuestions(this AppDbContext context, string voteQuestionJson)
        {
            List<ViewModels.VoteQuestion> voteQuestions = JsonConvert.DeserializeObject<List<ViewModels.VoteQuestion>>(voteQuestionJson);

            if (voteQuestions != null)
            {
                context.AddInitialVoteQuestions(voteQuestions);
            }
        }

        private static void AddInitialVoteQuestions(this AppDbContext context, List<ViewModels.VoteQuestion> voteQuestions)
        {
            voteQuestions.ForEach(context.UpdateSeedVoteQuestionInfo);
        }

        /// <summary>
        /// Adds a jurisdiction to the system, only if it does not exist.
        /// </summary>
        private static void AddInitialVoteQuestion(this AppDbContext context, ViewModels.VoteQuestion initialVoteQuestion)
        {
            VoteQuestion voteQuestion = context.GetVoteQuestionBySlug(initialVoteQuestion.slug);
            if (voteQuestion != null)
            {
                return;
            }

            voteQuestion = new VoteQuestion
            (
                initialVoteQuestion.question,
                initialVoteQuestion.slug,
                initialVoteQuestion.title,
                initialVoteQuestion.options
            );

            context.AddVoteQuestion(voteQuestion);
        }

        /// <summary>
        /// Update region
        /// </summary>
        /// <param name="context"></param>
        /// <param name="regionInfo"></param>
        public static void UpdateSeedVoteQuestionInfo(this AppDbContext context, ViewModels.VoteQuestion voteQuestionInfo)
        {
            VoteQuestion voteQuestion = context.GetVoteQuestionBySlug(voteQuestionInfo.slug);
            if (voteQuestion == null)
            {
                context.AddInitialVoteQuestion(voteQuestionInfo);
            }
            else
            {
                voteQuestion.Question = voteQuestionInfo.question;
                voteQuestion.Title = voteQuestionInfo.title;
                // update the options.
                if (voteQuestion.Options != null)
                {
                    // first pass to add new items.
                    foreach (var option in voteQuestionInfo.options)
                    {
                        VoteOption voteOption = voteQuestion.Options.FirstOrDefault(x => x != null && x.Option.Equals(option.option));
                        if (voteOption == null)
                        {
                            voteQuestion.Options.Add(option.ToModel());
                        }
                        else
                        {
                            voteOption.DisplayOrder = option.displayOrder;
                        }

                    }
                    List<VoteOption> itemsToRemove = new List<VoteOption>();
                    // second pass to identify items that are no longer present.
                    foreach (var option in voteQuestion.Options)
                    {
                        if (option != null)
                        {
                            if (voteQuestionInfo.options.FirstOrDefault(x => x != null && x.option.Equals(option.Option)) == null)
                            {
                                itemsToRemove.Add(option);
                            }
                        }

                    }
                    // third pass to remove the items.
                    foreach (var option in itemsToRemove)
                    {
                        voteQuestion.Options.Remove(option);
                    }
                }
                context.VoteQuestions.Update(voteQuestion);
                context.SaveChanges();
            }
        }

    }
}
