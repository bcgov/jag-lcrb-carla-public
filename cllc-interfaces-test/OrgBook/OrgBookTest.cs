using System.Net.Http;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OrgBookTest
{
    [TestClass]
    public class SearchUnitTest
    {
        [TestMethod]
        public async Task TestAutocomplete()
        {
            var client = new OrgBookClient(new HttpClient());
            var searchResult = await client.V2SearchAutocompleteGetAsync(null, "BC", null, null, null, null);
            Assert.IsNotNull(searchResult);
        }

        [TestMethod]
        public async Task TestSearchByName()
        {
            var client = new OrgBookClient(new HttpClient());
            var searchResult = await client.V2SearchAutocompleteGetAsync(null, "BC", null, null, null, null);
            Assert.IsNotNull(searchResult);
        }
    }
}
