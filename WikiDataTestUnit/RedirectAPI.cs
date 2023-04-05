using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiLibrary.API;

namespace WikiDataTestUnit
{
    [TestClass]
    public class RedirectAPI
    {
        
        const string expected = "Albert_Einstein";
        [TestMethod]
        public async Task Get()
        {
            string mockResponse = File.ReadAllText(Path.Join("HttpResponseStrings", "Redirect1.json"));
            var mockClient = HttpClientMock.Get(mockResponse);
            var result = await Redirect.Get(mockClient, "Einstein", "en");
            Assert.AreEqual(expected, result);
        }  
    }
}
