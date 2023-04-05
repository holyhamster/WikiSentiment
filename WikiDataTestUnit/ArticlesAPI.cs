using WikiLibrary.API;

namespace WikiDataTestUnit
{
    [TestClass]
    public class ArticlesAPI
    {
        static readonly (string, int)[] expected = { 
            ("Main_Page", 4331243), 
            ("Special:Search", 1163225), 
            ("Indian_Premier_League", 831656),
            ("2023_Indian_Premier_League", 579122),
            ("Caitlin_Clark", 413078),
            ("Fool's_Gold_Loaf", 389304),
            ("April_Fools'_Day", 334238),
            ("WrestleMania_39", 323190),
            ("ChatGPT", 299135),
            ("List_of_American_films_of_2023", 238867) };

        
        [TestMethod]
        public async Task Get()
        {
            string mockResponse = File.ReadAllText(Path.Join("HttpResponseStrings", "Articles1.json"));
            var mockClient = HttpClientMock.Get(mockResponse);
            var result = await Articles.Get(mockClient, "en", new DateTime(2023, 04, 01));
            CollectionAssert.AreEqual(expected, result);
        }
    }
}