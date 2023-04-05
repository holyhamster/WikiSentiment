using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiLibrary.API;

namespace WikiDataTestUnit
{
    [TestClass]
    public class LanglinkAPI
    {
        const string expected = "Byzantine_Empire";
        [TestMethod]
        public async Task Get()
        {
            string mockResponse = File.ReadAllText(Path.Join("HttpResponseStrings", "Langlink1.json"));
            var mockClient = HttpClientMock.Get(mockResponse);
            var result = await Langlink.Get(mockClient, "Byzantinisches_Reich", "en");
            Assert.AreEqual(expected, result);
        }
    }
}
