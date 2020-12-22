using System;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class TestUtilities
    {
		private static Random random = new Random(Guid.NewGuid().GetHashCode());

        public static string RandomAlphaString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomANString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
	}
}
