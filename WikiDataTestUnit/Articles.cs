using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiLibrary;
using WikiLibrary.DataObjects;

namespace WikiDataTestUnit
{
    [TestClass]
    public class Articles
    {
        [TestMethod]
        public async Task Test()
        {
            string mockResponse = File.ReadAllText(Path.Join("HttpResponseStrings", "TotalViews1.json"));
            var mockClient = HttpClientMock.Get(mockResponse);
            new Article()
        }
    }
}
