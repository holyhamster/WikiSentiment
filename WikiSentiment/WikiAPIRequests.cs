using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace WikiSentiment
{
    /// <summary>
    /// Collection of API requests that parse json data
    /// </summary>
    public static class WikiAPIRequests
    {
        /// <summary>
        /// Gets english title of given article. Returns empty string if none exist
        /// </summary>
        /// <param name="client">http client, api headers required</param>
        /// <param name="title"></param>
        /// <param name="countryCode">two letter language code</param>
        /// <returns>English title, returns empty if none</returns>
        public static async Task<string> GetLangLink(
                HttpClient client, string title, string countryCode)
        {
            string languageTarget = "en";
            title = stripSubmenuLink(title);
            JsonNode langlinkNode;

            string url = $"https://{countryCode}.wikipedia.org/w/api.php?action=query&titles=" +
                $"{title}&prop=langlinks&format=json&lllang={languageTarget}";

            var langlinkResponse = await client.GetStringAsync(url);
            var jsonObject = JsonNode.Parse(langlinkResponse);

            //validate json response, return empty string if failed
            if (!(jsonObject.AsObject().ContainsKey("query") &&
                    JsonNode.Parse(langlinkResponse).AsObject()["query"].AsObject().ContainsKey("pages")))
                return "";

            langlinkNode = JsonNode.Parse(langlinkResponse).AsObject()["query"]["pages"];

            var articleID = JsonSerializer.Deserialize<Dictionary<string, JsonNode>>(langlinkNode).Keys.First();

            //if article has an english counterpart, it'll have a langlink
            if (langlinkNode[articleID].AsObject().ContainsKey("langlinks"))
            {
                //TODO add conditions
                return normalizeTitle((string)langlinkNode[articleID]["langlinks"][0]["*"]);
            }
            else
                return "";
        }

        /// <summary>
        /// Returns title, if given article is a redirect page. Returns empty if not
        /// </summary>
        /// <param name="client"></param>
        /// <param name="title"></param>
        /// <param name="country">two letter language code</param>
        /// <returns>Title of a redirect page, returns empty if not</returns>
        public static async Task<string> GetRedirect(
            HttpClient client, string title, string country)
        {
            title = stripSubmenuLink(title);
            var request = await client.GetStringAsync(
                $"https://{country}.wikipedia.org/w/api.php?action=parse&" +
                $"formatversion=2&page={title}&prop=wikitext&format=json");

            JsonObject jObject = JsonNode.Parse(request).AsObject();

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
                return normalizeTitle(articleText.Split("[[", 2)[1].Split("]]")[0]);
            }

            return "";
        }

        /// <summary>
        /// Get top pairs of (title, views) for given language
        /// </summary>
        /// <param name="client">Http client with API headers</param>
        /// <param name="languageCode">Two letter country code</param>
        /// <param name="date">Date for lookup</param>
        /// <returns>Tuple with articleArray(name, views) of articles</returns>
        public static async Task<(string title, int views)[]>
            GetArticleList(HttpClient client, string languageCode, DateTime date)
        {
            var articlesString = await client.GetStringAsync(
                    "https://wikimedia.org/api/rest_v1/metrics/pageviews/" +
                    $"top/{languageCode}.wikipedia.org/all-access/{date.Year}/{date.Month:D2}/{date.Day:D2}");

            var jArticleArray = JsonNode.Parse(articlesString).AsObject()["items"][0]["articles"].AsArray();

            var articleArray = new (string title, int views)[jArticleArray.Count];

            for (int i = 0; i < jArticleArray.Count; i++)
            {
                articleArray[i] = (
                    normalizeTitle((string)jArticleArray[i]["article"].AsValue()), 
                    (int)jArticleArray[i]["views"].AsValue());
            }

            return articleArray;
        }

        /// <summary>
        /// Gets total daily views for given language
        /// </summary>
        /// <param name="client"></param>
        /// <param name="languageCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static async Task<int> GetTotalViews(
            HttpClient client, string languageCode, DateTime date)
        {
            var viewsString = await client.GetStringAsync(
                    "https://wikimedia.org/api/rest_v1/metrics/pageviews/" +
                    $"aggregate/{languageCode}.wikipedia.org/all-access/user/daily/" +
                    $"{date.Year}{date.Month:D2}{date.Day:D2}/{date.Year}{date.Month:D2}{date.Day:D2}");

            JsonObject jObject = JsonNode.Parse(viewsString).AsObject();

            //validate object and return total views
            if (jObject.ContainsKey("items") &&
                jObject["items"].AsArray().Count > 0 &&
                jObject["items"][0].AsObject().ContainsKey("views") )
            {
                return (int)JsonNode.Parse(viewsString).AsObject()["items"][0]["views"];
            }

            throw new HttpRequestException($"Http response has bad format. Date: {date}; language:{languageCode}");
        }

        
        static string normalizeTitle(string title)
        {

            return title.Trim().Replace(' ', '_');
        }

        /// <summary>
        /// Strip subitems from a title (in "/wiki/Albert_Einstein#Childhood", #Childhood is a subitem)
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        static string stripSubmenuLink(string title)
        {
            if (title.Contains('#'))
                return title.Split('#', 2)[0];
            else
                return title;
        }
    }
}
