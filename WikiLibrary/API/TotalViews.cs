using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WikiLibrary.API
{
    public static class TotalViews
    {
        public static async Task<int> Get(HttpClient client, string languageCode, DateTime date)
        {
            string url = "https://wikimedia.org/api/rest_v1/metrics/pageviews/" +
                $"aggregate/{languageCode}.wikipedia.org/all-access/user/daily/" +
                $"{date.Year}{date.Month:D2}{date.Day:D2}/{date.Year}{date.Month:D2}{date.Day:D2}";
            return parseResponse(await client.GetStringAsync(url));
        }

        static int parseResponse(string viewsString)
        {
            JsonObject jObject = JsonNode.Parse(viewsString).AsObject();

            //validate object and return total views
            if (!jObject.ContainsKey("items") ||
                jObject["items"].AsArray().Count == 0 ||
                !jObject["items"][0].AsObject().ContainsKey("views"))
            {
                throw new HttpRequestException($"Views string has bad format: {viewsString}");
            }
            return (int)jObject["items"][0]["views"];
        }
    }
}
