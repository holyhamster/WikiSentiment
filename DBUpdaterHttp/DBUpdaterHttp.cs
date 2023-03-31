using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using Azure.Data.Tables;
using WikiLibrary;
using Azure.Core.Cryptography;

namespace DBUpdaterHttp
{
    /// <summary>
    /// HTTP triggered Azure Function that grabs parameters from the url 
    /// and initiates DataBaseBuilder
    /// url schema is /DBUpdaterHttp?date=2021-12-31&days=10
    /// </summary>
    public class DBUpdaterHttp
    {
        private ConfigurationWrapper config;

        private HashSet<string> allLanguageCodes;

        private static readonly HttpClient httpClient = new HttpClient();

        private Dictionary<string, HashSet<string>> articleExceptions;

        public DBUpdaterHttp(IConfiguration iConfig)
        {
            config = new ConfigurationWrapper(iConfig);

            var exceptionsArrays = config.GetValue<Dictionary<string, string[]>>
                ("FunctionValues:CountryExceptions");

            articleExceptions = new Dictionary<string, HashSet<string>>();
            foreach (var iKey in exceptionsArrays.Keys)
                articleExceptions[iKey] = exceptionsArrays[iKey].ToHashSet();

            allLanguageCodes = config.GetValue<string[]>("FunctionValues:CountryCodes").ToHashSet();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                 config.GetValue<string>("WikiKeys:WikiAPIToken"));

            httpClient.DefaultRequestHeaders.Add("Api-User-Agent",
                config.GetValue<string>("WikiKeys:WikiUserContact"));
        }

        [FunctionName("DBUpdaterHttp")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Table("datatables"), StorageAccount("AzureWebJobsStorage")] TableClient tableClient, 
            ILogger log)
        {
            //url schema is /DBUpdaterHttp?date=2021-12-31&days=3&discard&language=de,fr
            //in the example the data will start from 31-12-2021, and go back for 3 days
            string YYYYMMDD = req.Query["date"];
            DateTime startDate;
            if (YYYYMMDD == null)
                return new BadRequestObjectResult("Missing parameter: date");
            if (!DateTime.TryParse(YYYYMMDD, out startDate))
                return new BadRequestObjectResult("Bad starting date: " + YYYYMMDD);

            int daysToGo = 1;
            string daysToGoString = req.Query["days"];
            if (daysToGoString != null &&
                !int.TryParse(daysToGoString, out daysToGo))
                    return new BadRequestObjectResult($"Missing parameter: {nameof(daysToGoString)}");


            HashSet<string> languages = allLanguageCodes;
            string languageStrings = req.Query["languages"];
            if (languageStrings != null &&
                !validateLanguages(languageStrings, out languages))
                return new BadRequestObjectResult("Bad languages parameter: " + languageStrings);


            string discardString = req.Query["discard"];
            bool discardOldData = discardString != null ? true : false;

            IDataBaseClient dbClient = new AzureStorageClient(tableClient);

            await DataBaseBuilder.updateDatabase(startDate, daysToGo, discardOldData, languages, 
                articleExceptions, httpClient, dbClient, log);
            
            return new OkObjectResult("Successfull execution");
        }

        /// <summary>
        /// Validates list of languages in "","","" format
        /// </summary>
        /// <param name="proposedLanguagesString">string in "","","", format</param>
        /// <param name="languageArray">out for an output array</param>
        /// <returns>returns true if string parsed into array</returns>
        bool validateLanguages(string proposedLanguagesString, out HashSet<string> languageArray)
        {
            languageArray = proposedLanguagesString.ToLowerInvariant().Split(',').ToHashSet();
            foreach (var iLanguage in languageArray)
            {
                if (!allLanguageCodes.Contains(iLanguage))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
