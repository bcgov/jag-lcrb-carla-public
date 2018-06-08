using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

using System.Text;
using Newtonsoft.Json;
using System.Net;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;

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
