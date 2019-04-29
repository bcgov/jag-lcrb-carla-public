using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Rest;

namespace OrgBookTest
{
    [TestClass]
    public class SearchUnitTest
    {
        [TestMethod]
        public void TestAutocomplete()
        {
            ServiceClientCredentials credentials = new NoCredentials();
            IOrgBookClient client = new OrgBookClient(new Uri("https://orgbook.gov.bc.ca/api"), credentials);

            var searchResult = client.Search.Autocomplete("BC");

            Assert.IsNotNull(searchResult);
        }

        [TestMethod]
        public void TestSearchByName()
        {
            ServiceClientCredentials credentials = new NoCredentials();
            IOrgBookClient client = new OrgBookClient(new Uri("https://orgbook.gov.bc.ca/api"), credentials);

            var searchResult = client.Search.Autocomplete("BC");

            Assert.IsNotNull(searchResult);
        }


    }
}
