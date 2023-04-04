using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WikiLibrary.API
{
    public static class Articles
    {
        public static async Task<(string title, int views)[]>
            Get(HttpClient client, string languageCode, DateTime date)
        {
            string url = "https://wikimedia.org/api/rest_v1/metrics/pageviews/" +
                    $"top/{languageCode}.wikipedia.org/all-access/{date.Year}/{date.Month:D2}/{date.Day:D2}";
            var response = await client.GetStringAsync(url);
            return parseResponse(response);
        }

        static (string title, int views)[] parseResponse(string response)
        {
            var jArticleArray = JsonNode.Parse(response).AsObject()["items"][0]["articles"].AsArray();

            var articleArray = new (string title, int views)[jArticleArray.Count];

            return (from x in jArticleArray
                    select ((string)x["article"].AsValue(), (int)x["views"].AsValue())).ToArray();
        }
    }
}
