using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Text;
using WikiSentiment.DataObjects;

namespace WikiSentiment
{
    public static class DataBaseBuilder
    {
        /// <summary>
        /// Creates article collections for given languages
        /// Uploads them into given DataBaseClient
        /// </summary>
        /// <param name="date">Starting date</param>
        /// <param name="daysToGo">Collections to create</param>
        /// <param name="discardOldEntries">If true will not discard all existing daily data in the DB</param>
        /// <param name="languageCodes">list of two letter language codes</param>
        /// <param name="exceptions"></param>
        /// <param name="httpClient">http client with API headers</param>
        /// <param name="dbClient"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task updateDatabase(DateTime date, int daysToGo, bool discardOldEntries,
            string[] languageCodes, Dictionary<string, string[]> exceptions, 
            HttpClient httpClient, IDataBaseClient dbClient, ILogger logger)
        {
            for (int i = daysToGo; i > 0; i--)
            {
                var newCollection = await DailyCollection.Create(date, languageCodes, 
                    exceptions, httpClient, logger);

                //if not discarding old data, use it as a base for new collection
                if (!discardOldEntries)
                {
                    var dbRequest = await dbClient.Load(date);
                    DailyCollection? oldDaily = null;
                    try
                    {
                        oldDaily = JsonSerializer.Deserialize<DailyCollection>(dbRequest);
                    }
                    catch (Exception _ex)
                    {
                        logger.LogError($"Skipped reading old data on " +
                            $"{date.Year}-{date.Month}-{date.Day:D2}: ${_ex}");
                    }
                    if (oldDaily != null && oldDaily.IsValid())
                        newCollection = DailyCollection.UpdateGiven(oldDaily, newCollection);
                }


                if (newCollection.IsValid())
                    await dbClient.Upload(date, newCollection.ToJSON());
                else
                    logger.LogError($"Skipped uploading collection " +
                        $"{date.Year}-{date.Month}-{date.Day:D2}");

                date = date.AddDays(-1);
            }
        }
    }
}
