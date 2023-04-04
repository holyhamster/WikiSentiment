using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WikiLibrary.API
{
    public static class Redirect
    {
        public static async Task<string> Get(
            HttpClient client, string title, string country)
        {
            title = Parsing.StripSubmenuLink(title);
            string url = $"https://{country}.wikipedia.org/w/api.php?action=parse&" +
                $"formatversion=2&page={title}&prop=wikitext&format=json";
            return parseResponse(await client.GetStringAsync(url));
        }

        static string parseResponse(string response)
        {
            JsonObject jObject = JsonNode.Parse(response).AsObject();

            //validate return json
            if (!jObject.ContainsKey("parse") ||
                !jObject["parse"].AsObject().ContainsKey("wikitext"))
            {
                return "";
            }

            //extract title, redirect article text format is "#REDIRECT [[original_title]]"
            string articleText = ((string)jObject["parse"].AsObject()["wikitext"]).Trim();
            if (articleText.StartsWith("#") &&
                articleText.Contains("[[") &&
                articleText.Contains("]]"))
            {
                return Parsing.NormalizeTitle(articleText.Split("[[", 2)[1].Split("]]")[0]);
            }

            return "";
        }
    }
}
