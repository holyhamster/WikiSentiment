using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiLibrary.API;

namespace WikiDataTestUnit
{
    [TestClass]
    public class TotalViewsAPI
    {
        const int expected = 242656071;
        [TestMethod]
        public async Task Get()
        {
            string mockResponse = File.ReadAllText(Path.Join("HttpResponseStrings", "TotalViews1.json"));
            var mockClient = HttpClientMock.Get(mockResponse);
            var result = await TotalViews.Get(mockClient, "en", new DateTime(2023,04,01));
            Assert.AreEqual(expected, result);
        }
    }
}
