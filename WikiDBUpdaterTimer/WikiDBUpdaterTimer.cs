using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using WikiSentiment;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.ObjectPool;
using System.Linq;

namespace WikiDBUpdaterTimer
{
    public class WikiDBUpdaterTimer
    {
        HashSet<string> allLanguageCodes;
        Dictionary<string, HashSet<string>> articleExceptions;

        [FunctionName("WikiDBUpdaterTimer")]
        public async Task Run([TimerTrigger("0 0 6 * * *")]TimerInfo myTimer,
            [Table("datatables"), StorageAccount("AzureWebJobsStorage")] TableClient tableClient, ILogger log,
            ExecutionContext context)
        {
            var rawConfig = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            ConfigurationWrapper config = new ConfigurationWrapper(rawConfig);

            var exceptionsArrays = config.GetValue<Dictionary<string, string[]>>("FunctionValues:CountryExceptions");
            articleExceptions = new Dictionary<string, HashSet<string>>();
            foreach (var iKey in exceptionsArrays.Keys)
                articleExceptions[iKey] = exceptionsArrays[iKey].ToHashSet();

            allLanguageCodes = config.GetValue<string[]>("FunctionValues:CountryCodes").ToHashSet();
            //articleExceptions = getLanguageExceptions(config);

            var date = DateTime.Now.AddDays(-1);
            bool discardOldData = true;
            int daysToUpdate = 1;

            await DataBaseBuilder.updateDatabase(date, daysToUpdate, discardOldData, allLanguageCodes, 
                articleExceptions, getHttpClient(config, log), new AzureStorageClient(tableClient), log);
        }

        HttpClient getHttpClient(ConfigurationWrapper config, ILogger log)
        {
            var result = new HttpClient();

            var apiToken = config.GetValue<string>("WikiKeys:WikiAPIToken");
            var contact = config.GetValue<string>("WikiKeys:WikiUserContact");
            result.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
            result.DefaultRequestHeaders.Add("Api-User-Agent", contact);

            return result;
        }
    }
}
