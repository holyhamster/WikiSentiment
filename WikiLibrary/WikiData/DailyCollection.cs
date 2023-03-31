using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace WikiLibrary.DataObjects
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
           DateTime date, HashSet<string> languageCode, Dictionary<string, HashSet<string>> exceptions, 
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
            };
        }
        
        /// <summary>
        /// Updates old colection with new content, compiles new featured list
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="newAdditions"></param>
        /// <returns></returns>
        public static DailyCollection UpdateGiven(DailyCollection collection, DailyCollection newAdditions)
        {
            var oldDictionary = collection.countrydailydict;
            var newDictionary = newAdditions.countrydailydict;

            return new DailyCollection()
            {
                countrydailydict = newDictionary
                .Concat(oldDictionary.Where(kvp => !newDictionary.ContainsKey(kvp.Key)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),

                featuredlist = getFeaturedCountries(newDictionary, featureArticles)
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
            List<(string countrycode, float percentage)> languageTopArticlesData;
            languageTopArticlesData = collection.Select(kvp => (kvp.Key,
                    (100f * kvp.Value.articles[0].vws) / kvp.Value.totalviews)).ToList();
            
            //order data in the descending order
            languageTopArticlesData = 
                (languageTopArticlesData.OrderBy(data => data.percentage)).Reverse().ToList();
 
            return languageTopArticlesData
               .Take(featuredAmount)
               .Select(x => x.countrycode)
               .ToList();
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
