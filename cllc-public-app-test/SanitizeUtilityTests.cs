
using Gov.Lclb.Cllb.Public.Utils;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class SanitizeUtilityTest
    {
        [Fact]
        public void TestSanitize()
        {
            string input =    "Joe_personal123123";
            string expected = "J**_p*******123123";
            
            string resultingData = WordSanitizer.Sanitize(input);
            Assert.Equal(expected, resultingData );
        }
    }
}

