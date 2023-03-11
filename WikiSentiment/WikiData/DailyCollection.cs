using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace WikiSentiment.DataObjects
{
    /// <summary>
    /// POJO class for a collection of languages for a single day
    /// </summary>
    public class DailyCollection
    {
        const int featureArticles = 5; //amount of languages to put on featured list 

        const int keepArticles = 5;  //amount of articles to keep for each language

        public Dictionary<string, LanguageCollection> countrydailydict { get; set; }

        public List<string> featuredlist { get; set; }

        /// <summary>
        /// Build a collection for a day with given languages
        /// </summary>
        /// <param name="date"></param>
        /// <param name="languageCode">Array of two letter language codes</param>
        /// <param name="exceptions">Dictionary with exceptions in {"en": ["Main_Page", "Search"]} format</param>
        /// <param name="client">Client for requests, wiki API headers required</param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async static Task<DailyCollection> Create(
           DateTime date, string[] languageCode, Dictionary<string, string[]> exceptions, 
           HttpClient client, ILogger logger)
        {
            //start daily data tasks for each country
            var regionalTasks = new Dictionary<string, Task<LanguageCollection>>();
            foreach (var iLanguage in languageCode)
            {
                regionalTasks[iLanguage] = LanguageCollection.Create(
                        client, date, iLanguage, exceptions, keepArticles);
            }

            //try waiting for each task, log any errors
            var regionalCollections = new Dictionary<string, LanguageCollection>();
            foreach (var iCountry in regionalTasks.Keys)
            {
                try { 
                    var result = await regionalTasks[iCountry]; 
                    regionalCollections[iCountry] = result; }
                catch (Exception e) {
                    logger.LogError($"Error building {iCountry}-collection for " +
                        $"{date.Year}-{date.Month:D2}-{date.Day:D2}: {e.Message}"); }
            }

            var featuredCountries = getFeaturedCountries(regionalCollections, featureArticles);

            return new DailyCollection()
            {
                countrydailydict = regionalCollections,
                featuredlist = featuredCountries
            }; ;
        }

        /// <summary>
        /// Updates old colection with new content, compiles new featured list
        /// </summary>
        /// <param name="base"></param>
        /// <param name="newAdditions"></param>
        /// <returns></returns>
        public static DailyCollection UpdateGiven(DailyCollection @base, DailyCollection newAdditions)
        {
            var newCollection = new Dictionary<string, LanguageCollection>(@base.countrydailydict);

            //override old data with new ones
            foreach(string iCountry in newAdditions.countrydailydict.Keys)
            {
                newCollection[iCountry] = newAdditions.countrydailydict[iCountry];
            }
            var featured = getFeaturedCountries(newCollection, featureArticles);

            return new DailyCollection()
            {
                countrydailydict = newCollection,
                featuredlist = featured
            };
        }

        /// <summary>
        /// Get ordered list of languages with the top viewed (proportionally) articles
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="featuredAmount">amount of languages to keep</param>
        /// <returns>list of two letter language codes</returns>
        static List<string> getFeaturedCountries(
            Dictionary<string, LanguageCollection> collection, int featuredAmount)
        {
            //compile the list of country codes and viewership scores for top articles
            var languageTopArticlesData = new List<(string countrycode, float percentage)>();
            foreach (string iLanguage in collection.Keys)
            {
                languageTopArticlesData.Add((iLanguage,
                    (100f * collection[iLanguage].articles[0].vws) / collection[iLanguage].totalviews));
            }

            //order data in the descending order
            languageTopArticlesData = (languageTopArticlesData.OrderBy(data => data.percentage)).Reverse().ToList();

            var amountToFeature = Math.Min(featuredAmount, collection.Count);
            List<string> result = new List<string>();

            //return countrycodes
            for (int i = 0; i < amountToFeature; i++)
                result.Add(languageTopArticlesData[i].countrycode);
            
            return result;
        }

        public string ToJSON()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            });
        }

        public bool IsValid()
        {
            return countrydailydict != null && countrydailydict.Count > 0;
        }
    }
}
