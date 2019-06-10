using System;
using System.Text.RegularExpressions;

namespace CarlaSpiceSync
{
    public class Validation
    {
        public static bool ValidatePostalCode(string postalCode)
        {
            if(postalCode == null)
            {
                return false;
            }

            string canadianPattern = @"^[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ] ?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]$";
            string usPattern = @"^\d{5}(?:[-\s]\d{4})?$";

            bool valid = Regex.IsMatch(postalCode, canadianPattern, RegexOptions.IgnoreCase);
            if (!valid)
            {
                valid = Regex.IsMatch(postalCode, usPattern);
            }
            return valid;
        }
    }
}
