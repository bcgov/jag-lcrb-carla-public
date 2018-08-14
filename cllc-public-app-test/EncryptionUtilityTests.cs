using Gov.Lclb.Cllb.Public.Utility;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class EncryptionUtilityTest 
    {
		[Fact]
        public void TestEncryptDecrypt()
        {
            string key = "46f44ece-e897-47d1-8ad0-10753208d9f8";
            string input = "String to encrypt.";
            string encrypted = EncryptionUtility.EncryptString(input, key);            
            string resultingData = EncryptionUtility.DecryptString(encrypted, key);
            Assert.Equal(resultingData, input);
        }
    }
}
