using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;

namespace WikiLibrary.API
{
    internal class Langlink
    {
        public static async Task<string> Get(
               HttpClient client, string title, string countryCode)
        {
            title = Parsing.StripSubmenuLink(title);
            const string languageTarget = "en";
            string url = $"https://{countryCode}.wikipedia.org/w/api.php?action=query&titles=" +
                $"{title}&prop=langlinks&format=json&lllang={languageTarget}";
            return parseResponse(await client.GetStringAsync(url));
        }

        static string parseResponse(string langlinkResponse)
        {
            var jsonObject = JsonNode.Parse(langlinkResponse);

            //validate json response, return empty string if failed
            if (!(jsonObject.AsObject().ContainsKey("query") &&
                    JsonNode.Parse(langlinkResponse).AsObject()["query"].AsObject().ContainsKey("pages")))
                return "";

            JsonNode langlinkNode = JsonNode.Parse(langlinkResponse).AsObject()["query"]["pages"];

            var articleID = JsonSerializer.Deserialize<Dictionary<string, JsonNode>>(langlinkNode).Keys.First();
            var langlink = langlinkNode[articleID];
            //if article has an english counterpart, it'll have a langlink
            if (langlink.AsObject().ContainsKey("langlinks"))
            {
                //TODO add conditions
                return Parsing.NormalizeTitle((string)langlink["langlinks"][0]["*"]);
            }
            else
                return "";
        }
    }
}
