using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Rest;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OrgBookTest
{
    [TestClass]
    public class NoCredentials : ServiceClientCredentials
    {

        public override Task ProcessHttpRequestAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            // add nothing.
            
            return Task.FromResult<object>(null);
        }
    }
}
