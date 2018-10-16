
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Gov.Lclb.Cllb.Public.Utils
{
	/// <summary>
	/// Helper methods for working with <see cref="Guid"/>.
	/// </summary>
	public static class BusinessNumberSanitizer
	{
		/// <summary>
		/// Removes white spaces from the business number. Returns null if the resulting string
		/// is not 9 digits
		/// </summary>
		/// <param name="businessNumber"></param>
		/// <returns></returns>
		public static string SanitizeNumber(string businessNumber){
			//sanitize the business number
			string result = Regex.Replace(businessNumber, "\\s", "");
			//validate the result
			if(!Regex.IsMatch(result, "^\\d\\d\\d\\d\\d\\d\\d\\d\\d$")){
				result = null;
			}
			return result;
		}
    }
}
