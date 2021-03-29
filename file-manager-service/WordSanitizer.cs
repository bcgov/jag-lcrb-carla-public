using System;
using System.Text.RegularExpressions;

namespace Gov.Lclb.Cllb.Services.FileManager
{
    /// <summary>
    ///     Helper methods for working with <see cref="Guid" />.
    /// </summary>
    public static class WordSanitizer
    {
        /// <summary>
        ///     Replaces all words with just the first character followed by stars for the length of the word.
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public static string Sanitize(string rawData)
        {
            if (string.IsNullOrEmpty(rawData)) return rawData;
            // sanitize the string
            // we want to keep the first character and replace the rest with stars
            return Regex.Replace(rawData, @"[a-zA-Z]+", delegate(Match match)
            {
                var v = match.ToString();
                // result will start with the first character
                var result = "" + v[0];
                // replace the rest of the word
                if (v.Length > 1) result = result.PadRight(v.Length, '*');

                return result;
            });
        }
    }
}