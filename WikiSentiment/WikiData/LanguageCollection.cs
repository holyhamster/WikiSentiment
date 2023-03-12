using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WikiSentiment.DataObjects
{
    //POJO for collection of articles in single language
    public class LanguageCollection
    {
        public string countrycode { get; set; }

        public int totalviews { get; set; }

        public List<Article> articles { get; set; }

        /// <summary>
        /// Build daily collection of articles in single language
        /// </summary>
        /// <param name="client">Http client for requests</param>
        /// <param name="date"></param>
        /// <param name="languageCode">Two letter code</param>
        /// <param name="exceptions">Dictionary with exceptions in {"en": ["Main_Page", "Search"]} format</param>
        /// <param name="keepMax">keep X amount of articles</param>
        /// <returns></returns>
        public static async Task<LanguageCollection> Create(
            HttpClient client, DateTime date,
            string languageCode, Dictionary<string, string[]> exceptions,
            int keepMax)
        {
            //get total views for a regional wikipedia
            var totalviews = await WikiAPIRequests.GetTotalViews(client, languageCode, date);

            //get raw (title, views) data
            var articleEntries = await WikiAPIRequests.GetArticleList(client, languageCode, date);

            var createdArticles = new Dictionary<string, Article>();

            foreach (var iEntry in articleEntries)
            {
                //stop when created enough articles
                if (createdArticles.Count >= keepMax)
                    break;

                //if the article name is not on the exceptions list
                if (isException(iEntry.title, languageCode, exceptions))
                    continue;

                //if already createed an article with this title,
                //add its views to the record. otherwise make a new article
                bool hasThisTitle = createdArticles.ContainsKey(iEntry.title);

                if (hasThisTitle)
                    createdArticles[iEntry.title].vws += iEntry.views;
                else
                {
                    var article = await Article.Create(client, iEntry.title, languageCode, iEntry.views);
                    createdArticles[article.ttl] = article;
                }
                
            }

            return new LanguageCollection()
            {
                countrycode = languageCode,
                totalviews = totalviews,
                articles = createdArticles.Values.ToList()
            };
        }

        /// <summary>
        /// Returns bool if given title is among exceptions
        /// </summary>
        /// <param name="title">String of a title</param>
        /// <param name="language">Two letter language code</param>
        /// <param name="exceptions">Dictionary with exceptions in {"en": ["Main_Page", "Search"]} format</param>
        /// <returns></returns>
        static bool isException(string title, string language, Dictionary<string, string[]> exceptions)
        {
            title = title.ToLower();
            //check regional exception array
            if (exceptions.ContainsKey(language))
            {
                foreach (string iException in exceptions[language])
                    if (title.Contains(iException))
                        return true;
            }

            //check common exceptions array (all contains . or :)
            if (title.Contains('.') | title.Contains(':'))
            {
                foreach (string iPartException in exceptions["all"])
                    if (title.Contains(iPartException))
                        return true;
            }

            return false;
        }
    }
}
